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
namespace org.camunda.bpm.engine.cdi.impl.context
{


	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using BpmnExecutionContext = org.camunda.bpm.engine.impl.context.BpmnExecutionContext;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Default implementation of the business process association manager. Uses a
	/// fallback-strategy to associate the process instance with the "broadest"
	/// active scope, starting with the conversation.
	/// <p />
	/// Subclass in order to implement custom association schemes and association
	/// with custom scopes.
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public class DefaultContextAssociationManager implements ContextAssociationManager, java.io.Serializable
	[Serializable]
	public class DefaultContextAssociationManager : ContextAssociationManager
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger log = Logger.getLogger(typeof(DefaultContextAssociationManager).FullName);

	  protected internal class ScopedAssociation
	  {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
		internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.TaskService taskService;
		internal TaskService taskService;

		protected internal VariableMap cachedVariables = new VariableMapImpl();
		protected internal VariableMap cachedVariablesLocal = new VariableMapImpl();
		protected internal Execution execution;
		protected internal Task task;

		public virtual Execution Execution
		{
			get
			{
			  return execution;
			}
			set
			{
			  this.execution = value;
			}
		}


		public virtual Task Task
		{
			get
			{
			  return task;
			}
			set
			{
			  this.task = value;
			}
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getVariable(String variableName)
		public virtual T getVariable<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
		  TypedValue value = cachedVariables.getValueTyped(variableName);
		  if (value == null)
		  {
			if (execution != null)
			{
			  value = runtimeService.getVariableTyped(execution.Id, variableName);
			  cachedVariables.put(variableName, value);
			}
		  }
		  return (T) value;
		}

		public virtual void setVariable(string variableName, object value)
		{
		  cachedVariables.put(variableName, value);
		}

		public virtual VariableMap CachedVariables
		{
			get
			{
			  return cachedVariables;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getVariableLocal(String variableName)
		public virtual T getVariableLocal<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
		  TypedValue value = cachedVariablesLocal.getValueTyped(variableName);
		  if (value == null)
		  {
			if (task != null)
			{
			  value = taskService.getVariableLocalTyped(task.Id, variableName);
			  cachedVariablesLocal.put(variableName, value);
			}
			else if (execution != null)
			{
			  value = runtimeService.getVariableLocalTyped(execution.Id, variableName);
			  cachedVariablesLocal.put(variableName, value);
			}
		  }
		  return (T) value;
		}

		public virtual void setVariableLocal(string variableName, object value)
		{
		  if (execution == null && task == null)
		  {
			throw new ProcessEngineCdiException("Cannot set a local cached variable: neither a Task nor an Execution is associated.");
		  }
		  cachedVariablesLocal.put(variableName, value);
		}

		public virtual VariableMap CachedVariablesLocal
		{
			get
			{
			  return cachedVariablesLocal;
			}
		}

		public virtual void flushVariableCache()
		{
		  if (task != null)
		  {
			  taskService.setVariablesLocal(task.Id, cachedVariablesLocal);
			taskService.setVariables(task.Id, cachedVariables);

		  }
		  else if (execution != null)
		  {
			  runtimeService.setVariablesLocal(execution.Id, cachedVariablesLocal);
			runtimeService.setVariables(execution.Id, cachedVariables);

		  }
		  else
		  {
			throw new ProcessEngineCdiException("Cannot flush variable cache: neither a Task nor an Execution is associated.");

		  }

		  // clear variable cache after flush
		  cachedVariables.clear();
		  cachedVariablesLocal.clear();
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConversationScoped protected static class ConversationScopedAssociation extends ScopedAssociation implements java.io.Serializable
	  [Serializable]
	  protected internal class ConversationScopedAssociation : ScopedAssociation
	  {
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequestScoped protected static class RequestScopedAssociation extends ScopedAssociation implements java.io.Serializable
	  [Serializable]
	  protected internal class RequestScopedAssociation : ScopedAssociation
	  {
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private javax.enterprise.inject.spi.BeanManager beanManager;
	  private BeanManager beanManager;

	  protected internal virtual Type BroadestActiveContext
	  {
		  get
		  {
			foreach (Type scopeType in AvailableScopedAssociationClasses)
			{
			  Annotation scopeAnnotation = scopeType.GetCustomAttributes(true).length > 0 ? scopeType.GetCustomAttributes(true)[0] : null;
			  if (scopeAnnotation == null || !beanManager.isScope(scopeAnnotation.annotationType()))
			  {
				throw new ProcessEngineException("ScopedAssociation must carry exactly one annotation and it must be a @Scope annotation");
			  }
			  try
			  {
				beanManager.getContext(scopeAnnotation.annotationType());
				return scopeType;
			  }
			  catch (ContextNotActiveException)
			  {
				log.finest("Context " + scopeAnnotation.annotationType() + " not active.");
			  }
			}
			throw new ProcessEngineException("Could not determine an active context to associate the current process instance / task instance with.");
		  }
	  }

	  /// <summary>
	  /// Override to add different / additional contexts.
	  /// 
	  /// @returns a list of <seealso cref="Scope"/>-types, which are used in the given order
	  ///          to resolve the broadest active context (@link
	  ///          #getBroadestActiveContext()})
	  /// </summary>
	  protected internal virtual IList<Type> AvailableScopedAssociationClasses
	  {
		  get
		  {
			List<Type> scopeTypes = new List<Type>();
			scopeTypes.Add(typeof(ConversationScopedAssociation));
			scopeTypes.Add(typeof(RequestScopedAssociation));
			return scopeTypes;
		  }
	  }

	  protected internal virtual ScopedAssociation getScopedAssociation()
	  {
		return ProgrammaticBeanLookup.lookup(BroadestActiveContext, beanManager);
	  }

	  public virtual Execution Execution
	  {
		  set
		  {
			if (value == null)
			{
			  throw new ProcessEngineCdiException("Cannot associate with execution: null");
			}
			ensureCommandContextNotActive();
    
			ScopedAssociation scopedAssociation = getScopedAssociation();
			Execution associatedExecution = scopedAssociation.Execution;
			if (associatedExecution != null && !associatedExecution.Id.Equals(value.Id))
			{
			  throw new ProcessEngineCdiException("Cannot associate " + value + ", already associated with " + associatedExecution + ". Disassociate first!");
			}
    
			if (log.isLoggable(Level.FINE))
			{
			  log.fine("Associating " + value + " (@" + scopedAssociation.GetType().GetCustomAttributes(true)[0].annotationType().SimpleName + ")");
			}
			scopedAssociation.Execution = value;
		  }
		  get
		  {
			ExecutionEntity execution = ExecutionFromContext;
			if (execution != null)
			{
			  return execution;
			}
			else
			{
			  return getScopedAssociation().Execution;
			}
		  }
	  }

	  public virtual void disAssociate()
	  {
		ensureCommandContextNotActive();
		ScopedAssociation scopedAssociation = getScopedAssociation();
		if (scopedAssociation.Execution == null)
		{
		  throw new ProcessEngineException("Cannot dissasociate execution, no " + scopedAssociation.GetType().GetCustomAttributes(true)[0].annotationType().SimpleName + " execution associated. ");
		}
		if (log.isLoggable(Level.FINE))
		{
		  log.fine("Disassociating");
		}
		scopedAssociation.Execution = null;
		scopedAssociation.Task = null;
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			Execution execution = Execution;
			if (execution != null)
			{
			  return execution.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }


	  public virtual TypedValue getVariable(string variableName)
	  {
		ExecutionEntity execution = ExecutionFromContext;
		if (execution != null)
		{
		  return execution.getVariableTyped(variableName);
		}
		else
		{
		  return getScopedAssociation().getVariable(variableName);
		}
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		ExecutionEntity execution = ExecutionFromContext;
		if (execution != null)
		{
		  execution.setVariable(variableName, value);
		  execution.getVariable(variableName);
		}
		else
		{
		  getScopedAssociation().setVariable(variableName, value);
		}
	  }

	  public virtual TypedValue getVariableLocal(string variableName)
	  {
		ExecutionEntity execution = ExecutionFromContext;
		if (execution != null)
		{
		  return execution.getVariableLocalTyped(variableName);
		}
		else
		{
		  return getScopedAssociation().getVariableLocal(variableName);
		}
	  }

	  public virtual void setVariableLocal(string variableName, object value)
	  {
		ExecutionEntity execution = ExecutionFromContext;
		if (execution != null)
		{
		  execution.setVariableLocal(variableName, value);
		  execution.getVariableLocal(variableName);
		}
		else
		{
		  getScopedAssociation().setVariableLocal(variableName, value);
		}
	  }

	  protected internal virtual ExecutionEntity ExecutionFromContext
	  {
		  get
		  {
			if (Context.CommandContext != null)
			{
			  BpmnExecutionContext executionContext = Context.BpmnExecutionContext;
			  if (executionContext != null)
			  {
				return executionContext.Execution;
			  }
			}
			return null;
		  }
	  }

	  public virtual Task Task
	  {
		  get
		  {
			ensureCommandContextNotActive();
			return getScopedAssociation().Task;
		  }
		  set
		  {
			ensureCommandContextNotActive();
			getScopedAssociation().Task = value;
		  }
	  }


	  public virtual VariableMap CachedVariables
	  {
		  get
		  {
			ensureCommandContextNotActive();
			return getScopedAssociation().CachedVariables;
		  }
	  }

	  public virtual VariableMap CachedLocalVariables
	  {
		  get
		  {
			ensureCommandContextNotActive();
			return getScopedAssociation().CachedVariablesLocal;
		  }
	  }

	  public virtual void flushVariableCache()
	  {
		ensureCommandContextNotActive();
		getScopedAssociation().flushVariableCache();
	  }

	  protected internal virtual void ensureCommandContextNotActive()
	  {
		if (Context.CommandContext != null)
		{
		  throw new ProcessEngineCdiException("Cannot work with scoped associations inside command context.");
		}
	  }

	}

}