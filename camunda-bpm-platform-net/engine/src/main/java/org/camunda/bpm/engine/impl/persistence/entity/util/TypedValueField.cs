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
namespace org.camunda.bpm.engine.impl.persistence.entity.util
{

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityLifecycleAware = org.camunda.bpm.engine.impl.db.DbEntityLifecycleAware;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandContextListener = org.camunda.bpm.engine.impl.interceptor.CommandContextListener;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ValueFieldsImpl = org.camunda.bpm.engine.impl.variable.serializer.ValueFieldsImpl;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// A field what provide a typed version of a value. It can
	/// be used in an entity which implements <seealso cref="ValueFields"/>.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class TypedValueField : DbEntityLifecycleAware, CommandContextListener
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal string serializerName;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> serializer;
	  protected internal TypedValueSerializer<object> serializer;

	  protected internal TypedValue cachedValue;

	  protected internal string errorMessage;

	  protected internal readonly ValueFields valueFields;

	  protected internal bool notifyOnImplicitUpdates = false;
	  protected internal IList<TypedValueUpdateListener> updateListeners;

	  public TypedValueField(ValueFields valueFields, bool notifyOnImplicitUpdates)
	  {
		this.valueFields = valueFields;
		this.notifyOnImplicitUpdates = notifyOnImplicitUpdates;
		this.updateListeners = new List<TypedValueUpdateListener>();
	  }

	  public virtual object getValue()
	  {
		TypedValue typedValue = TypedValue;
		if (typedValue != null)
		{
		  return typedValue.Value;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return getTypedValue(true);
		  }
	  }

	  public virtual TypedValue getTypedValue(bool deserializeValue)
	  {
		if (cachedValue != null && cachedValue is SerializableValue && Context.CommandContext != null)
		{
		  SerializableValue serializableValue = (SerializableValue) cachedValue;
		  if (deserializeValue && !serializableValue.Deserialized)
		  {
			// clear cached value in case it is not deserialized and user requests deserialized value
			cachedValue = null;
		  }
		}

		if (cachedValue == null && string.ReferenceEquals(errorMessage, null))
		{
		  try
		  {
			cachedValue = Serializer.readValue(valueFields, deserializeValue);

			if (notifyOnImplicitUpdates && isMutableValue(cachedValue))
			{
			  Context.CommandContext.registerCommandContextListener(this);
			}

		  }
		  catch (Exception e)
		  {
			// intercept the error message
			this.errorMessage = e.Message;
			throw e;
		  }
		}
		return cachedValue;
	  }

	  public virtual TypedValue setValue(TypedValue value)
	  {
		// determine serializer to use
		serializer = Serializers.findSerializerForValue(value, Context.ProcessEngineConfiguration.FallbackSerializerFactory);
		serializerName = serializer.Name;

		if (value is UntypedValueImpl)
		{
		  // type has been detected
		  value = serializer.convertToTypedValue((UntypedValueImpl) value);
		}

		// set new value
		writeValue(value, valueFields);

		// cache the value
		cachedValue = value;

		// ensure that we serialize the object on command context flush
		// if it can be implicitly changed
		if (notifyOnImplicitUpdates && isMutableValue(cachedValue))
		{
		  Context.CommandContext.registerCommandContextListener(this);
		}

		return value;
	  }

	  public virtual bool Mutable
	  {
		  get
		  {
			return isMutableValue(cachedValue);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected boolean isMutableValue(org.camunda.bpm.engine.variable.value.TypedValue value)
	  protected internal virtual bool isMutableValue(TypedValue value)
	  {
		return ((TypedValueSerializer<TypedValue>) Serializer).isMutableValue(value);
	  }

	  protected internal virtual bool ValuedImplicitlyUpdated
	  {
		  get
		  {
			if (cachedValue != null && isMutableValue(cachedValue))
			{
			  sbyte[] byteArray = valueFields.ByteArrayValue;
    
			  ValueFieldsImpl tempValueFields = new ValueFieldsImpl();
			  writeValue(cachedValue, tempValueFields);
    
			  sbyte[] byteArrayAfter = tempValueFields.ByteArrayValue;
    
			  return !Arrays.Equals(byteArray, byteArrayAfter);
			}
    
			return false;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void writeValue(org.camunda.bpm.engine.variable.value.TypedValue value, org.camunda.bpm.engine.impl.variable.serializer.ValueFields valueFields)
	  protected internal virtual void writeValue(TypedValue value, ValueFields valueFields)
	  {
		((TypedValueSerializer<TypedValue>) Serializer).writeValue(value, valueFields);
	  }

	  public virtual void onCommandContextClose(CommandContext commandContext)
	  {
		notifyImplicitValueUpdate();
	  }

	  public virtual void notifyImplicitValueUpdate()
	  {
		if (ValuedImplicitlyUpdated)
		{
		  foreach (TypedValueUpdateListener typedValueImplicitUpdateListener in updateListeners)
		  {
			typedValueImplicitUpdateListener.onImplicitValueUpdate(cachedValue);
		  }
		}
	  }

	  public virtual void onCommandFailed(CommandContext commandContext, Exception t)
	  {
		// ignore
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer()
	  public virtual TypedValueSerializer<object> Serializer
	  {
		  get
		  {
			ensureSerializerInitialized();
			return serializer;
		  }
	  }

	  protected internal virtual void ensureSerializerInitialized()
	  {
		if (!string.ReferenceEquals(serializerName, null) && serializer == null)
		{
		  serializer = Serializers.getSerializerByName(serializerName);

		  if (serializer == null)
		  {
			serializer = getFallbackSerializer(serializerName);
		  }

		  if (serializer == null)
		  {
			throw LOG.serializerNotDefinedException(this);
		  }
		}
	  }

	  public static VariableSerializers Serializers
	  {
		  get
		  {
			if (Context.CommandContext != null)
			{
			  VariableSerializers variableSerializers = Context.ProcessEngineConfiguration.VariableSerializers;
			  VariableSerializers paSerializers = CurrentPaSerializers;
    
			  if (paSerializers != null)
			  {
				return variableSerializers.join(paSerializers);
			  }
			  else
			  {
				return variableSerializers;
			  }
			}
			else
			{
			  throw LOG.serializerOutOfContextException();
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public static org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getFallbackSerializer(String serializerName)
	  public static TypedValueSerializer<object> getFallbackSerializer(string serializerName)
	  {
		if (Context.ProcessEngineConfiguration != null)
		{
		  VariableSerializerFactory fallbackSerializerFactory = Context.ProcessEngineConfiguration.FallbackSerializerFactory;
		  if (fallbackSerializerFactory != null)
		  {
			return fallbackSerializerFactory.getSerializer(serializerName);
		  }
		  else
		  {
			return null;
		  }
		}
		else
		{
		  throw LOG.serializerOutOfContextException();
		}
	  }

	  protected internal static VariableSerializers CurrentPaSerializers
	  {
		  get
		  {
			if (Context.CurrentProcessApplication != null)
			{
			  ProcessApplicationReference processApplicationReference = Context.CurrentProcessApplication;
			  try
			  {
				ProcessApplicationInterface processApplicationInterface = processApplicationReference.ProcessApplication;
    
				ProcessApplicationInterface rawPa = processApplicationInterface.RawObject;
				if (rawPa is AbstractProcessApplication)
				{
				  return ((AbstractProcessApplication) rawPa).VariableSerializers;
				}
				else
				{
				  return null;
				}
			  }
			  catch (ProcessApplicationUnavailableException e)
			  {
				throw LOG.cannotDeterminePaDataformats(e);
			  }
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string SerializerName
	  {
		  get
		  {
			return serializerName;
		  }
		  set
		  {
			this.serializerName = value;
		  }
	  }


	  public virtual void addImplicitUpdateListener(TypedValueUpdateListener listener)
	  {
		updateListeners.Add(listener);
	  }

	  /// <returns> the type name of the value </returns>
	  public virtual string TypeName
	  {
		  get
		  {
			if (string.ReferenceEquals(serializerName, null))
			{
			  return ValueType.NULL.Name;
			}
			else
			{
			  return Serializer.Type.Name;
			}
		  }
	  }

	  /// <summary>
	  /// If the variable value could not be loaded, this returns the error message.
	  /// </summary>
	  /// <returns> an error message indicating why the variable value could not be loaded. </returns>
	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual void postLoad()
	  {
	  }

	  public virtual void clear()
	  {
		cachedValue = null;
	  }
	}

}