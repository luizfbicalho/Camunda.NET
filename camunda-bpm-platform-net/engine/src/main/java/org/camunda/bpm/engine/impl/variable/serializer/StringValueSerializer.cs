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
namespace org.camunda.bpm.engine.impl.variable.serializer
{
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;

	/// 
	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class StringValueSerializer : PrimitiveValueSerializer<StringValue>
	{

	  public StringValueSerializer() : base(ValueType.STRING)
	  {
	  }

	  public virtual StringValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.stringValue((string) untypedValue.Value, untypedValue.Transient);
	  }

	  public override StringValue readValue(ValueFields valueFields)
	  {
		return Variables.stringValue(valueFields.TextValue);
	  }

	  public virtual void writeValue(StringValue variableValue, ValueFields valueFields)
	  {
		valueFields.TextValue = variableValue.Value;
	  }

	}

}