using System;

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
namespace org.camunda.bpm.engine.variable.impl.value
{
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class ObjectValueImpl : AbstractTypedValue<object>, ObjectValue
	{

	  private const long serialVersionUID = 1L;

	  protected internal string objectTypeName;

	  protected internal string serializationDataFormat;
	  protected internal string serializedValue;
	  protected internal bool isDeserialized;

	  public ObjectValueImpl(object deserializedValue, string serializedValue, string serializationDataFormat, string objectTypeName, bool isDeserialized) : base(deserializedValue, org.camunda.bpm.engine.variable.type.ValueType_Fields.OBJECT)
	  {


		this.serializedValue = serializedValue;
		this.serializationDataFormat = serializationDataFormat;
		this.objectTypeName = objectTypeName;
		this.isDeserialized = isDeserialized;
	  }

	  public ObjectValueImpl(object value) : this(value, null, null, null, true)
	  {
	  }

	  public ObjectValueImpl(object value, bool isTransient) : this(value, null, null, null, true)
	  {
		this.isTransient = isTransient;
	  }

	  public virtual string SerializationDataFormat
	  {
		  get
		  {
			return serializationDataFormat;
		  }
		  set
		  {
			this.serializationDataFormat = value;
		  }
	  }


	  public virtual string ObjectTypeName
	  {
		  get
		  {
			return objectTypeName;
		  }
		  set
		  {
			this.objectTypeName = value;
		  }
	  }


	  public virtual string ValueSerialized
	  {
		  get
		  {
			return serializedValue;
		  }
	  }

	  public virtual string SerializedValue
	  {
		  set
		  {
			this.serializedValue = value;
		  }
	  }

	  public virtual bool Deserialized
	  {
		  get
		  {
			return isDeserialized;
		  }
	  }

	  public override object Value
	  {
		  get
		  {
			if (isDeserialized)
			{
			  return base.Value;
			}
			else
			{
			  throw new System.InvalidOperationException("Object is not deserialized.");
			}
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getValue(Class<T> type)
	  public virtual T getValue<T>(Type type)
	  {
			  type = typeof(T);
		object value = Value;
		if (type.IsAssignableFrom(value.GetType()))
		{
		  return (T) value;
		}
		else
		{
		  throw new System.ArgumentException("Value '" + value + "' is not of type '" + type + "'.");
		}
	  }

	  public virtual Type ObjectType
	  {
		  get
		  {
			object value = Value;
    
			if (value == null)
			{
			  return null;
			}
			else
			{
			  return value.GetType();
			}
		  }
	  }

	  public override SerializableValueType Type
	  {
		  get
		  {
			return (SerializableValueType) base.Type;
		  }
	  }

	  public override bool Transient
	  {
		  set
		  {
			this.isTransient = value;
		  }
	  }

	  public override string ToString()
	  {
		return "ObjectValue ["
			+ "value=" + value + ", isDeserialized=" + isDeserialized + ", serializationDataFormat=" + serializationDataFormat + ", objectTypeName=" + objectTypeName + ", serializedValue=" + (!string.ReferenceEquals(serializedValue, null) ? (serializedValue.Length + " chars") : null) + ", isTransient=" + isTransient + "]";
	  }
	}

}