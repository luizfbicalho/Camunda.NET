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
namespace org.camunda.bpm.engine.test.api.variable
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.booleanValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.byteArrayValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.createVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.dateValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.doubleValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.integerValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.shortValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.stringValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.untypedNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.BOOLEAN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.BYTES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.DOUBLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.INTEGER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.NULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.SHORT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType_Fields.STRING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using NullValueImpl = org.camunda.bpm.engine.variable.impl.value.NullValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Philipp Ossler *
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class PrimitiveValueTest
	public class PrimitiveValueTest
	{

	  protected internal static readonly DateTime DATE_VALUE = DateTime.Now;
	  protected internal const string LOCAL_DATE_VALUE = "2015-09-18";
	  protected internal const string LOCAL_TIME_VALUE = "10:00:00";
	  protected internal const string PERIOD_VALUE = "P14D";
	  protected internal static readonly sbyte[] BYTES_VALUE = "a".Bytes;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: {0} = {1}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {STRING, "someString", stringValue("someString"), stringValue(null)},
			new object[] {INTEGER, 1, integerValue(1), integerValue(null)},
			new object[] {BOOLEAN, true, booleanValue(true), booleanValue(null)},
			new object[] {NULL, null, untypedNullValue(), untypedNullValue()},
			new object[] {SHORT, (short) 1, shortValue((short) 1), shortValue(null)},
			new object[] {DOUBLE, 1d, doubleValue(1d), doubleValue(null)},
			new object[] {DATE, DATE_VALUE, dateValue(DATE_VALUE), dateValue(null)},
			new object[] {BYTES, BYTES_VALUE, byteArrayValue(BYTES_VALUE), byteArrayValue(null)}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public org.camunda.bpm.engine.variable.type.ValueType valueType;
	  public ValueType valueType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public Object value;
	  public object value;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(2) public org.camunda.bpm.engine.variable.value.TypedValue typedValue;
	  public TypedValue typedValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(3) public org.camunda.bpm.engine.variable.value.TypedValue nullValue;
	  public TypedValue nullValue;

	  protected internal string variableName = "variable";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreatePrimitiveVariableUntyped()
	  public virtual void testCreatePrimitiveVariableUntyped()
	  {
		VariableMap variables = createVariables().putValue(variableName, value);

		assertEquals(value, variables[variableName]);
		assertEquals(value, variables.getValueTyped(variableName).Value);

		// no type information present
		TypedValue typedValue = variables.getValueTyped(variableName);
		if (!(typedValue is NullValueImpl))
		{
		  assertNull(typedValue.Type);
		  assertEquals(variables[variableName], typedValue.Value);
		}
		else
		{
		  assertEquals(NULL, typedValue.Type);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreatePrimitiveVariableTyped()
	  public virtual void testCreatePrimitiveVariableTyped()
	  {
		VariableMap variables = createVariables().putValue(variableName, typedValue);

		// get return value
		assertEquals(value, variables[variableName]);

		// type is not lost
		assertEquals(valueType, variables.getValueTyped(variableName).Type);

		// get wrapper
		object stringValue = variables.getValueTyped(variableName).Value;
		assertEquals(value, stringValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreatePrimitiveVariableNull()
	  public virtual void testCreatePrimitiveVariableNull()
	  {
		VariableMap variables = createVariables().putValue(variableName, nullValue);

		// get return value
		assertEquals(null, variables[variableName]);

		// type is not lost
		assertEquals(valueType, variables.getValueTyped(variableName).Type);

		// get wrapper
		object stringValue = variables.getValueTyped(variableName).Value;
		assertEquals(null, stringValue);
	  }

	}

}