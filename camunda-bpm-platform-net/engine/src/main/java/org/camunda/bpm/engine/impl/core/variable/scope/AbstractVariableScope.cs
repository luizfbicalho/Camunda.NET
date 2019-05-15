using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.core.variable.scope
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;
	using VariableEventDispatcher = org.camunda.bpm.engine.impl.core.variable.@event.VariableEventDispatcher;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using TypedValueField = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	[Serializable]
	public abstract class AbstractVariableScope : VariableScope, VariableEventDispatcher
	{

	  private const long serialVersionUID = 1L;

	  // TODO: move this?
	  protected internal ELContext cachedElContext;

	  protected internal abstract VariableStore<CoreVariableInstance> VariableStore {get;}
	  protected internal abstract VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory {get;}
	  protected internal abstract IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners {get;}

	  public abstract AbstractVariableScope ParentVariableScope {get;}

	  public virtual void initializeVariableStore(IDictionary<string, object> variables)
	  {
		foreach (string variableName in variables.Keys)
		{
		  TypedValue value = Variables.untypedValue(variables[variableName]);
		  CoreVariableInstance variableValue = VariableInstanceFactory.build(variableName, value, false);
		  VariableStore.addVariable(variableValue);
		}
	  }

	  // get variable map /////////////////////////////////////////


	  public virtual string VariableScopeKey
	  {
		  get
		  {
			return "scope";
		  }
	  }

	  public virtual VariableMapImpl getVariables()
	  {
		return VariablesTyped;
	  }

	  public virtual VariableMapImpl VariablesTyped
	  {
		  get
		  {
			return getVariablesTyped(true);
		  }
	  }

	  public virtual VariableMapImpl getVariablesTyped(bool deserializeValues)
	  {
		VariableMapImpl variableMap = new VariableMapImpl();
		collectVariables(variableMap, null, false, deserializeValues);
		return variableMap;
	  }

	  public virtual VariableMapImpl getVariablesLocal()
	  {
		return VariablesLocalTyped;
	  }

	  public virtual VariableMapImpl VariablesLocalTyped
	  {
		  get
		  {
			return getVariablesLocalTyped(true);
		  }
	  }

	  public virtual VariableMapImpl getVariablesLocalTyped(bool deserializeObjectValues)
	  {
		VariableMapImpl variables = new VariableMapImpl();
		collectVariables(variables, null, true, deserializeObjectValues);
		return variables;
	  }

	  public virtual void collectVariables(VariableMapImpl resultVariables, ICollection<string> variableNames, bool isLocal, bool deserializeValues)
	  {
		bool collectAll = (variableNames == null);

		IList<CoreVariableInstance> localVariables = getVariableInstancesLocal(variableNames);
		foreach (CoreVariableInstance var in localVariables)
		{
		  if (!resultVariables.containsKey(var.Name) && (collectAll || variableNames.Contains(var.Name)))
		  {
			resultVariables.put(var.Name, var.getTypedValue(deserializeValues));
		  }
		}
		if (!isLocal)
		{
		  AbstractVariableScope parentScope = ParentVariableScope;
		  // Do not propagate to parent if all variables in 'variableNames' are already collected!
		  if (parentScope != null && (collectAll || !resultVariables.Keys.Equals(variableNames)))
		  {
			parentScope.collectVariables(resultVariables, variableNames, isLocal, deserializeValues);
		  }
		}
	  }

	  // get single variable /////////////////////////////////////

	  public virtual object getVariable(string variableName)
	  {
		return getVariable(variableName, true);
	  }

	  public virtual object getVariable(string variableName, bool deserializeObjectValue)
	  {
		return getValueFromVariableInstance(deserializeObjectValue, getVariableInstance(variableName));
	  }

	  public virtual object getVariableLocal(string variableName)
	  {
		return getVariableLocal(variableName, true);
	  }

	  public virtual object getVariableLocal(string variableName, bool deserializeObjectValue)
	  {
		return getValueFromVariableInstance(deserializeObjectValue, getVariableInstanceLocal(variableName));
	  }

	  protected internal virtual object getValueFromVariableInstance(bool deserializeObjectValue, CoreVariableInstance variableInstance)
	  {
		if (variableInstance != null)
		{
		  TypedValue typedValue = variableInstance.getTypedValue(deserializeObjectValue);
		  if (typedValue != null)
		  {
			return typedValue.Value;
		  }
		}
		return null;
	  }

	  public virtual T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableTyped(variableName, true);
	  }

	  public virtual T getVariableTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getTypedValueFromVariableInstance(deserializeValue, getVariableInstance(variableName));
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getVariableLocalTyped(variableName, true);
	  }

	  public virtual T getVariableLocalTyped<T>(string variableName, bool deserializeValue) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return getTypedValueFromVariableInstance(deserializeValue, getVariableInstanceLocal(variableName));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getTypedValueFromVariableInstance(boolean deserializeValue, org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance variableInstance)
	  private T getTypedValueFromVariableInstance<T>(bool deserializeValue, CoreVariableInstance variableInstance) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		if (variableInstance != null)
		{
		  return (T) variableInstance.getTypedValue(deserializeValue);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual CoreVariableInstance getVariableInstance(string variableName)
	  {
		CoreVariableInstance variableInstance = getVariableInstanceLocal(variableName);
		if (variableInstance != null)
		{
		  return variableInstance;
		}
		AbstractVariableScope parentScope = ParentVariableScope;
		if (parentScope != null)
		{
		  return parentScope.getVariableInstance(variableName);
		}
		return null;
	  }

	  public virtual CoreVariableInstance getVariableInstanceLocal(string name)
	  {
		return VariableStore.getVariable(name);
	  }

	  public virtual IList<CoreVariableInstance> VariableInstancesLocal
	  {
		  get
		  {
			return VariableStore.Variables;
		  }
	  }

	  public virtual IList<CoreVariableInstance> getVariableInstancesLocal(ICollection<string> variableNames)
	  {
		return VariableStore.getVariables(variableNames);
	  }

	  public virtual bool hasVariables()
	  {
		if (!VariableStore.Empty)
		{
		  return true;
		}
		AbstractVariableScope parentScope = ParentVariableScope;
		return parentScope != null && parentScope.hasVariables();
	  }

	  public virtual bool hasVariablesLocal()
	  {
		return !VariableStore.Empty;
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		if (hasVariableLocal(variableName))
		{
		  return true;
		}
		AbstractVariableScope parentScope = ParentVariableScope;
		return parentScope != null && parentScope.hasVariable(variableName);
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		return VariableStore.containsKey(variableName);
	  }

	  protected internal virtual ISet<string> collectVariableNames(ISet<string> variableNames)
	  {
		AbstractVariableScope parentScope = ParentVariableScope;
		if (parentScope != null)
		{
		  variableNames.addAll(parentScope.collectVariableNames(variableNames));
		}
		foreach (CoreVariableInstance variableInstance in VariableStore.Variables)
		{
		  variableNames.Add(variableInstance.Name);
		}
		return variableNames;
	  }

	  public virtual ISet<string> VariableNames
	  {
		  get
		  {
			return collectVariableNames(new HashSet<string>());
		  }
	  }

	  public virtual ISet<string> VariableNamesLocal
	  {
		  get
		  {
			return VariableStore.Keys;
		  }
	  }

	  public virtual void setVariables<T1>(IDictionary<T1> variables) where T1 : object
	  {
		if (variables != null)
		{
		  foreach (string variableName in variables.Keys)
		  {
			object value = null;
			if (variables is VariableMap)
			{
			  value = ((VariableMap) variables).getValueTyped(variableName);
			}
			else
			{
			  value = variables[variableName];
			}
			setVariable(variableName, value);
		  }
		}
	  }

	  public virtual void setVariablesLocal<T1>(IDictionary<T1> variables) where T1 : object
	  {
		if (variables != null)
		{
		  foreach (string variableName in variables.Keys)
		  {
			object value = null;
			if (variables is VariableMap)
			{
			  value = ((VariableMap) variables).getValueTyped(variableName);
			}
			else
			{
			  value = variables[variableName];
			}
			setVariableLocal(variableName, value);
		  }
		}
	  }

	  public virtual void removeVariables()
	  {
		foreach (CoreVariableInstance variableInstance in VariableStore.Variables)
		{
		  invokeVariableLifecycleListenersDelete(variableInstance, SourceActivityVariableScope);
		}

		VariableStore.removeVariables();
	  }

	  public virtual void removeVariablesLocal()
	  {
		IList<string> variableNames = new List<string>(VariableNamesLocal);
		foreach (string variableName in variableNames)
		{
		  removeVariableLocal(variableName);
		}
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  foreach (string variableName in variableNames)
		  {
			removeVariable(variableName);
		  }
		}
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  foreach (string variableName in variableNames)
		  {
			removeVariableLocal(variableName);
		  }
		}
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		TypedValue typedValue = Variables.untypedValue(value);
		setVariable(variableName, typedValue, SourceActivityVariableScope);

	  }

	  protected internal virtual void setVariable(string variableName, TypedValue value, AbstractVariableScope sourceActivityVariableScope)
	  {
		if (hasVariableLocal(variableName))
		{
		  TypedValue previousTypeValue = getVariableInstanceLocal(variableName).getTypedValue(false);

		  if (value.Transient != previousTypeValue.Transient)
		  {
			throw ProcessEngineLogger.CORE_LOGGER.transientVariableException(variableName);
		  }

		  if (value.Transient)
		  {
			setVariableLocalTransient(variableName, value, sourceActivityVariableScope);
		  }
		  else
		  {
			setVariableLocal(variableName, value, sourceActivityVariableScope);
		  }

		  return;
		}
		AbstractVariableScope parentVariableScope = ParentVariableScope;
		if (parentVariableScope != null)
		{
		  if (sourceActivityVariableScope == null)
		  {
			parentVariableScope.setVariable(variableName, value);
		  }
		  else
		  {
			parentVariableScope.setVariable(variableName, value, sourceActivityVariableScope);
		  }
		  return;
		}
		if (value.Transient)
		{
		  setVariableLocalTransient(variableName, value, sourceActivityVariableScope);
		}
		else
		{
		  setVariableLocal(variableName, value, sourceActivityVariableScope);
		}
	  }

	  public virtual void setVariableLocal(string variableName, TypedValue value, AbstractVariableScope sourceActivityExecution)
	  {

		checkJavaSerialization(variableName, value);

		VariableStore<CoreVariableInstance> variableStore = VariableStore;

		if (variableStore.containsKey(variableName))
		{
		  CoreVariableInstance existingInstance = variableStore.getVariable(variableName);
		  existingInstance.Value = value;
		  invokeVariableLifecycleListenersUpdate(existingInstance, sourceActivityExecution);
		}
		else if (variableStore.isRemoved(variableName))
		{

		  CoreVariableInstance existingInstance = variableStore.getRemovedVariable(variableName);

		  existingInstance.Value = value;
		  VariableStore.addVariable(existingInstance);
		  invokeVariableLifecycleListenersUpdate(existingInstance, sourceActivityExecution);

		  DbEntityManager dbEntityManager = Context.CommandContext.DbEntityManager;
		  dbEntityManager.undoDelete((VariableInstanceEntity) existingInstance);
		}
		else
		{
		  CoreVariableInstance variableValue = VariableInstanceFactory.build(variableName, value, false);
		  VariableStore.addVariable(variableValue);
		  invokeVariableLifecycleListenersCreate(variableValue, sourceActivityExecution);
		}
	  }

	  /// <summary>
	  /// Checks, if Java serialization will be used and if it is allowed to be used. </summary>
	  /// <param name="variableName"> </param>
	  /// <param name="value"> </param>
	  protected internal virtual void checkJavaSerialization(string variableName, TypedValue value)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (value is SerializableValue && !processEngineConfiguration.JavaSerializationFormatEnabled)
		{

		  SerializableValue serializableValue = (SerializableValue) value;

		  // if Java serialization is prohibited
		  if (!serializableValue.Deserialized)
		  {

			string javaSerializationDataFormat = Variables.SerializationDataFormats.JAVA.Name;
			string requestedDataFormat = serializableValue.SerializationDataFormat;

			if (string.ReferenceEquals(requestedDataFormat, null))
			{
			  // check if Java serializer will be used
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer serializerForValue = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField.getSerializers().findSerializerForValue(serializableValue, processEngineConfiguration.getFallbackSerializerFactory());
			  TypedValueSerializer serializerForValue = TypedValueField.Serializers.findSerializerForValue(serializableValue, processEngineConfiguration.FallbackSerializerFactory);
			  if (serializerForValue != null)
			  {
				requestedDataFormat = serializerForValue.SerializationDataformat;
			  }
			}

			if (javaSerializationDataFormat.Equals(requestedDataFormat))
			{
			  throw ProcessEngineLogger.CORE_LOGGER.javaSerializationProhibitedException(variableName);
			}
		  }
		}
	  }

	  protected internal virtual void invokeVariableLifecycleListenersCreate(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		invokeVariableLifecycleListenersCreate(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
	  }

	  protected internal virtual void invokeVariableLifecycleListenersCreate(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope, IList<VariableInstanceLifecycleListener<CoreVariableInstance>> lifecycleListeners)
	  {
		foreach (VariableInstanceLifecycleListener<CoreVariableInstance> lifecycleListener in lifecycleListeners)
		{
		  lifecycleListener.onCreate(variableInstance, sourceScope);
		}
	  }

	  protected internal virtual void invokeVariableLifecycleListenersDelete(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		invokeVariableLifecycleListenersDelete(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
	  }

	  protected internal virtual void invokeVariableLifecycleListenersDelete(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope, IList<VariableInstanceLifecycleListener<CoreVariableInstance>> lifecycleListeners)
	  {
		foreach (VariableInstanceLifecycleListener<CoreVariableInstance> lifecycleListener in lifecycleListeners)
		{
		  lifecycleListener.onDelete(variableInstance, sourceScope);
		}
	  }

	  protected internal virtual void invokeVariableLifecycleListenersUpdate(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		invokeVariableLifecycleListenersUpdate(variableInstance, sourceScope, VariableInstanceLifecycleListeners);
	  }

	  protected internal virtual void invokeVariableLifecycleListenersUpdate(CoreVariableInstance variableInstance, AbstractVariableScope sourceScope, IList<VariableInstanceLifecycleListener<CoreVariableInstance>> lifecycleListeners)
	  {
		foreach (VariableInstanceLifecycleListener<CoreVariableInstance> lifecycleListener in lifecycleListeners)
		{
		  lifecycleListener.onUpdate(variableInstance, sourceScope);
		}
	  }

	  public virtual void setVariableLocal(string variableName, object value)
	  {
		TypedValue typedValue = Variables.untypedValue(value);
		setVariableLocal(variableName, typedValue, SourceActivityVariableScope);

	  }

	  /// <summary>
	  /// Sets a variable in the local scope. In contrast to
	  /// <seealso cref="#setVariableLocal(String, Object)"/>, the variable is transient that
	  /// means it will not be stored in the data base. For example, a transient
	  /// variable can be used for a result variable that is only available for
	  /// output mapping.
	  /// </summary>
	  public virtual void setVariableLocalTransient(string variableName, object value)
	  {
		TypedValue typedValue = Variables.untypedValue(value, true);

		checkJavaSerialization(variableName, typedValue);

		CoreVariableInstance coreVariableInstance = VariableInstanceFactory.build(variableName, typedValue, true);
		VariableStore.addVariable(coreVariableInstance);
	  }

	  public virtual void setVariableLocalTransient(string variableName, object value, AbstractVariableScope sourceActivityVariableScope)
	  {

		VariableStore<CoreVariableInstance> variableStore = VariableStore;
		if (variableStore.containsKey(variableName))
		{
		  CoreVariableInstance existingInstance = variableStore.getVariable(variableName);
		  existingInstance.Value = (TypedValue) value;
		  invokeVariableLifecycleListenersUpdate(existingInstance, sourceActivityVariableScope);
		}
		else
		{
		  setVariableLocalTransient(variableName, value);
		  invokeVariableLifecycleListenersCreate(variableStore.getVariable(variableName), sourceActivityVariableScope);
		}
	  }

	  public virtual void removeVariable(string variableName)
	  {
		removeVariable(variableName, SourceActivityVariableScope);
	  }

	  protected internal virtual void removeVariable(string variableName, AbstractVariableScope sourceActivityExecution)
	  {
		if (VariableStore.containsKey(variableName))
		{
		  removeVariableLocal(variableName, sourceActivityExecution);
		  return;
		}
		AbstractVariableScope parentVariableScope = ParentVariableScope;
		if (parentVariableScope != null)
		{
		  if (sourceActivityExecution == null)
		  {
			parentVariableScope.removeVariable(variableName);
		  }
		  else
		  {
			parentVariableScope.removeVariable(variableName, sourceActivityExecution);
		  }
		}
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		removeVariableLocal(variableName, SourceActivityVariableScope);
	  }

	  protected internal virtual AbstractVariableScope SourceActivityVariableScope
	  {
		  get
		  {
			return this;
		  }
	  }

	  protected internal virtual void removeVariableLocal(string variableName, AbstractVariableScope sourceActivityExecution)
	  {

		if (VariableStore.containsKey(variableName))
		{
		  CoreVariableInstance variableInstance = VariableStore.getVariable(variableName);

		  invokeVariableLifecycleListenersDelete(variableInstance, sourceActivityExecution);
		  VariableStore.removeVariable(variableName);
		}

	  }

	  public virtual ELContext CachedElContext
	  {
		  get
		  {
			return cachedElContext;
		  }
		  set
		  {
			this.cachedElContext = value;
		  }
	  }

	  public virtual void dispatchEvent(VariableEvent variableEvent)
	  {
		// default implementation does nothing
	  }

	}

}