using System;
using System.Text;

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
namespace org.camunda.bpm.engine.rest.helper.variable
{

	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using PrimitiveValue = org.camunda.bpm.engine.variable.value.PrimitiveValue;
	using Description = org.hamcrest.Description;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EqualsPrimitiveValue : EqualsTypedValue<EqualsPrimitiveValue>
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object value_Renamed;

	  public virtual EqualsPrimitiveValue value(object value)
	  {
		this.value_Renamed = value;
		return this;
	  }

	  public override bool matches(object argument)
	  {
		if (!base.matches(argument))
		{
		  return false;
		}

		if (!argument.GetType().IsAssignableFrom(typeof(PrimitiveValue)))
		{
		  return false;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.variable.value.PrimitiveValue<?> primitveValue = (org.camunda.bpm.engine.variable.value.PrimitiveValue<?>) argument;
		PrimitiveValue<object> primitveValue = (PrimitiveValue<object>) argument;

		if (value_Renamed == null)
		{
		  if (primitveValue.Value != null)
		  {
			return false;
		  }
		}
		else
		{
		  if (!matchesValues(primitveValue.Value))
		  {
			return false;
		  }
		}

		return true;
	  }

	  protected internal virtual bool matchesValues(object otherValue)
	  {
		// explicit matching for byte[]
		if (value_Renamed is sbyte[])
		{
		  if (!(otherValue is sbyte[]))
		  {
			return false;
		  }

		  sbyte[] byteValue = (sbyte[]) value_Renamed;
		  sbyte[] otherByteValue = (sbyte[]) otherValue;

		  return Arrays.Equals(byteValue, otherByteValue);
		}

		if (type_Renamed == ValueType.NUMBER)
		{
		  if (!(otherValue is Number))
		  {
			return false;
		  }

		  Number thisNumer = (Number) value_Renamed;
		  Number otherNumber = (Number) otherValue;

		  return thisNumer.doubleValue() == otherNumber.doubleValue();
		}

		return value_Renamed.Equals(otherValue);

	  }

	  public static EqualsPrimitiveValue primitiveValueMatcher()
	  {
		return new EqualsPrimitiveValue();
	  }

	  public static EqualsPrimitiveValue integerValue(int? value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.INTEGER).value(value);
	  }

	  public static EqualsPrimitiveValue stringValue(string value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.STRING).value(value);
	  }

	  public static EqualsPrimitiveValue booleanValue(bool? value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.BOOLEAN).value(value);
	  }

	  public static EqualsPrimitiveValue shortValue(short? value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.SHORT).value(value);
	  }

	  public static EqualsPrimitiveValue doubleValue(double? value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.DOUBLE).value(value);
	  }

	  public static EqualsPrimitiveValue longValue(long? value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.LONG).value(value);
	  }

	  public static EqualsPrimitiveValue bytesValue(sbyte[] value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.BYTES).value(value);
	  }

	  public static EqualsPrimitiveValue dateValue(DateTime value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.DATE).value(value);
	  }

	  public static EqualsPrimitiveValue numberValue(Number value)
	  {
		return (new EqualsPrimitiveValue()).type(ValueType.NUMBER).value(value);
	  }

	  public virtual void describeTo(Description description)
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(this.GetType().Name);
		sb.Append(": ");
		sb.Append("value=");
		sb.Append(value_Renamed);
		sb.Append(", type=");
		sb.Append(type_Renamed);

		description.appendText(sb.ToString());
	  }

	}

}