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
namespace org.camunda.bpm.engine.cdi.test.api
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Michael Scholz
	/// </summary>
	public class ProcessVariableMapTest : CdiProcessEngineTestCase
	{

	  private const string VARNAME_1 = "aVariable";
	  private const string VARNAME_2 = "anotherVariable";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableMap()
	  public virtual void testProcessVariableMap()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		VariableMap variables = (VariableMap) getBeanInstance("processVariableMap");
		assertNotNull(variables);

		///////////////////////////////////////////////////////////////////
		// Put a variable via BusinessProcess and get it via VariableMap //
		///////////////////////////////////////////////////////////////////
		string aValue = "aValue";
		businessProcess.setVariable(VARNAME_1, Variables.stringValue(aValue));

		// Legacy API
		assertEquals(aValue, variables.get(VARNAME_1));

		// Typed variable API
		TypedValue aTypedValue = variables.getValueTyped(VARNAME_1);
		assertEquals(ValueType.STRING, aTypedValue.Type);
		assertEquals(aValue, aTypedValue.Value);
		assertEquals(aValue, variables.getValue(VARNAME_1, typeof(string)));

		// Type API with wrong type
		try
		{
		  variables.getValue(VARNAME_1, typeof(Integer));
		  fail("ClassCastException expected!");
		}
		catch (System.InvalidCastException ex)
		{
		  assertEquals("Cannot cast variable named 'aVariable' with value 'aValue' to type 'class java.lang.Integer'.", ex.Message);
		}

		///////////////////////////////////////////////////////////////////
		// Put a variable via VariableMap and get it via BusinessProcess //
		///////////////////////////////////////////////////////////////////
		string anotherValue = "anotherValue";
		variables.put(VARNAME_2, Variables.stringValue(anotherValue));

		// Legacy API
		assertEquals(anotherValue, businessProcess.getVariable(VARNAME_2));

		// Typed variable API
		TypedValue anotherTypedValue = businessProcess.getVariableTyped(VARNAME_2);
		assertEquals(ValueType.STRING, anotherTypedValue.Type);
		assertEquals(anotherValue, anotherTypedValue.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml") public void testProcessVariableMapLocal()
	  [Deployment(resources : "org/camunda/bpm/engine/cdi/test/api/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testProcessVariableMapLocal()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));
		businessProcess.startProcessByKey("businessProcessBeanTest");

		VariableMap variables = (VariableMap) getBeanInstance("processVariableMapLocal");
		assertNotNull(variables);

		///////////////////////////////////////////////////////////////////
		// Put a variable via BusinessProcess and get it via VariableMap //
		///////////////////////////////////////////////////////////////////
		string aValue = "aValue";
		businessProcess.setVariableLocal(VARNAME_1, Variables.stringValue(aValue));

		// Legacy API
		assertEquals(aValue, variables.get(VARNAME_1));

		// Typed variable API
		TypedValue aTypedValue = variables.getValueTyped(VARNAME_1);
		assertEquals(ValueType.STRING, aTypedValue.Type);
		assertEquals(aValue, aTypedValue.Value);
		assertEquals(aValue, variables.getValue(VARNAME_1, typeof(string)));

		// Type API with wrong type
		try
		{
		  variables.getValue(VARNAME_1, typeof(Integer));
		  fail("ClassCastException expected!");
		}
		catch (System.InvalidCastException ex)
		{
		  assertEquals("Cannot cast variable named 'aVariable' with value 'aValue' to type 'class java.lang.Integer'.", ex.Message);
		}

		///////////////////////////////////////////////////////////////////
		// Put a variable via VariableMap and get it via BusinessProcess //
		///////////////////////////////////////////////////////////////////
		string anotherValue = "anotherValue";
		variables.put(VARNAME_2, Variables.stringValue(anotherValue));

		// Legacy API
		assertEquals(anotherValue, businessProcess.getVariableLocal(VARNAME_2));

		// Typed variable API
		TypedValue anotherTypedValue = businessProcess.getVariableLocalTyped(VARNAME_2);
		assertEquals(ValueType.STRING, anotherTypedValue.Type);
		assertEquals(anotherValue, anotherTypedValue.Value);
	  }
	}

}