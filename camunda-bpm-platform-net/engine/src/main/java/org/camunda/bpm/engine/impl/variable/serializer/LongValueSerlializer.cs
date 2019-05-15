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
	using LongValue = org.camunda.bpm.engine.variable.value.LongValue;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class LongValueSerlializer : PrimitiveValueSerializer<LongValue>
	{

	  public LongValueSerlializer() : base(ValueType.LONG)
	  {
	  }

	  public virtual LongValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.longValue((long?) untypedValue.Value, untypedValue.Transient);
	  }

	  public override LongValue readValue(ValueFields valueFields)
	  {
		return Variables.longValue(valueFields.LongValue);
	  }

	  public virtual void writeValue(LongValue value, ValueFields valueFields)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<long> longValue = value.getValue();
		long? longValue = value.Value;

		valueFields.LongValue = longValue;

		if (longValue != null)
		{
		  valueFields.TextValue = longValue.ToString();
		}
		else
		{
		  valueFields.TextValue = null;
		}
	  }

	}

}