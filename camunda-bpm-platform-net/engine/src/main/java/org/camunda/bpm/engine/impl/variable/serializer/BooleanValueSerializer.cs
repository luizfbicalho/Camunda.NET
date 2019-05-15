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
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;

	/// <summary>
	/// Serializes booleans as long values.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class BooleanValueSerializer : PrimitiveValueSerializer<BooleanValue>
	{

	  // boolean is modeled as long values
	  private const long? TRUE = 1L;
	  private const long? FALSE = 0L;

	  public BooleanValueSerializer() : base(ValueType.BOOLEAN)
	  {
	  }

	  public virtual BooleanValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.booleanValue((bool?) untypedValue.Value, untypedValue.Transient);
	  }

	  public override BooleanValue readValue(ValueFields valueFields)
	  {
		bool? boolValue = null;
		long? longValue = valueFields.LongValue;

		if (longValue != null)
		{
		  boolValue = longValue.Equals(TRUE);
		}

		return Variables.booleanValue(boolValue);
	  }

	  public virtual void writeValue(BooleanValue variableValue, ValueFields valueFields)
	  {
		long? longValue = null;
		bool? boolValue = variableValue.Value;

		if (boolValue != null)
		{
		  longValue = boolValue ? TRUE : FALSE;
		}

		valueFields.LongValue = longValue;
	  }

	}

}