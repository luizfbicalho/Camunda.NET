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
namespace org.camunda.bpm.engine.impl.variable.serializer
{
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;

	/// <summary>
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public class IntegerValueSerializer : PrimitiveValueSerializer<IntegerValue>
	{

	  public IntegerValueSerializer() : base(ValueType.INTEGER)
	  {
	  }

	  public virtual IntegerValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.integerValue((int?) untypedValue.Value, untypedValue.Transient);
	  }

	  public virtual void writeValue(IntegerValue variableValue, ValueFields valueFields)
	  {
		int? value = variableValue.Value;

		if (value != null)
		{
		  valueFields.LongValue = ((int?) value).Value;
		  valueFields.TextValue = value.ToString();
		}
		else
		{
		  valueFields.LongValue = null;
		  valueFields.TextValue = null;
		}

	  }

	  public override IntegerValue readValue(ValueFields valueFields)
	  {
		int? intValue = null;

		if (valueFields.LongValue != null)
		{
		  intValue = Convert.ToInt32(valueFields.LongValue.Value);
		}

		return Variables.integerValue(intValue);
	  }

	}

}