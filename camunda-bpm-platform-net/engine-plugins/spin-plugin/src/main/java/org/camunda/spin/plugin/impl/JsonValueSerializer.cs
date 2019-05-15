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
namespace org.camunda.spin.plugin.impl
{
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using SpinValues = org.camunda.spin.plugin.variable.SpinValues;
	using JsonValueType = org.camunda.spin.plugin.variable.type.JsonValueType;
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using SpinValue = org.camunda.spin.plugin.variable.value.SpinValue;
	using JsonValueImpl = org.camunda.spin.plugin.variable.value.impl.JsonValueImpl;
	using DataFormat = org.camunda.spin.spi.DataFormat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class JsonValueSerializer : SpinValueSerializer
	{

	  public JsonValueSerializer(DataFormat<SpinJsonNode> dataFormat) : base(org.camunda.spin.plugin.variable.type.SpinValueType_Fields.JSON, dataFormat, org.camunda.spin.plugin.variable.type.JsonValueType_Fields.TYPE_NAME)
	  {
	  }

	  public JsonValueSerializer() : this(DataFormats.json())
	  {
	  }

	  public virtual SpinValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return SpinValues.jsonValue((SpinJsonNode) untypedValue.Value).create();
	  }

	  protected internal override SpinValue createDeserializedValue(object deserializedObject, string serializedStringValue, ValueFields valueFields)
	  {
		SpinJsonNode value = (SpinJsonNode) deserializedObject;
		return new JsonValueImpl(value, serializedStringValue, value.DataFormatName, true);
	  }

	  protected internal override SpinValue createSerializedValue(string serializedStringValue, ValueFields valueFields)
	  {
		return new JsonValueImpl(serializedStringValue, serializationDataFormat);
	  }

	}

}