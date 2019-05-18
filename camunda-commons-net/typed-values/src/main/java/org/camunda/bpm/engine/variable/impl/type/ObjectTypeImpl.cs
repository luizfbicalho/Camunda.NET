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
namespace org.camunda.bpm.engine.variable.impl.type
{

	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using ObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.ObjectValueBuilder;
	using SerializedObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.SerializedObjectValueBuilder;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class ObjectTypeImpl : AbstractValueTypeImpl, SerializableValueType
	{

	  private const long serialVersionUID = 1L;

	  public const string TYPE_NAME = "object";

	  public ObjectTypeImpl() : base(TYPE_NAME)
	  {
	  }

	  public override bool PrimitiveValueType
	  {
		  get
		  {
			return false;
		  }
	  }

	  public override TypedValue createValue(object value, IDictionary<string, object> valueInfo)
	  {
		ObjectValueBuilder builder = Variables.objectValue(value);

		if (valueInfo != null)
		{
		  applyValueInfo(builder, valueInfo);
		}

		return builder.create();
	  }

	  public override IDictionary<string, object> getValueInfo(TypedValue typedValue)
	  {
		if (!(typedValue is ObjectValue))
		{
		  throw new System.ArgumentException("Value not of type Object.");
		}
		ObjectValue objectValue = (ObjectValue) typedValue;

		IDictionary<string, object> valueInfo = new Dictionary<string, object>();

		string serializationDataFormat = objectValue.SerializationDataFormat;
		if (!string.ReferenceEquals(serializationDataFormat, null))
		{
		  valueInfo[org.camunda.bpm.engine.variable.type.SerializableValueType_Fields.VALUE_INFO_SERIALIZATION_DATA_FORMAT] = serializationDataFormat;
		}

		string objectTypeName = objectValue.ObjectTypeName;
		if (!string.ReferenceEquals(objectTypeName, null))
		{
		  valueInfo[org.camunda.bpm.engine.variable.type.SerializableValueType_Fields.VALUE_INFO_OBJECT_TYPE_NAME] = objectTypeName;
		}

		if (objectValue.Transient)
		{
		  valueInfo[org.camunda.bpm.engine.variable.type.ValueType_Fields.VALUE_INFO_TRANSIENT] = objectValue.Transient;
		}

		return valueInfo;
	  }

	  public virtual SerializableValue createValueFromSerialized(string serializedValue, IDictionary<string, object> valueInfo)
	  {
		SerializedObjectValueBuilder builder = Variables.serializedObjectValue(serializedValue);

		if (valueInfo != null)
		{
		  applyValueInfo(builder, valueInfo);
		}

		return builder.create();
	  }

	  protected internal virtual void applyValueInfo(ObjectValueBuilder builder, IDictionary<string, object> valueInfo)
	  {

		string objectValueTypeName = (string) valueInfo[org.camunda.bpm.engine.variable.type.SerializableValueType_Fields.VALUE_INFO_OBJECT_TYPE_NAME];
		if (builder is SerializedObjectValueBuilder)
		{
		  ((SerializedObjectValueBuilder) builder).objectTypeName(objectValueTypeName);
		}

		string serializationDataFormat = (string) valueInfo[org.camunda.bpm.engine.variable.type.SerializableValueType_Fields.VALUE_INFO_SERIALIZATION_DATA_FORMAT];
		if (!string.ReferenceEquals(serializationDataFormat, null))
		{
		  builder.serializationDataFormat(serializationDataFormat);
		}

		builder.Transient = isTransient(valueInfo);
	  }

	}

}