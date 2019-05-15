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
namespace org.camunda.bpm.qa.upgrade.scenarios790
{
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	using Customer = org.camunda.bpm.qa.upgrade.json.beans.Customer;
	using ObjectList = org.camunda.bpm.qa.upgrade.json.beans.ObjectList;
	using Order = org.camunda.bpm.qa.upgrade.json.beans.Order;

	using OrderDetails = org.camunda.bpm.qa.upgrade.json.beans.OrderDetails;
	using RegularCustomer = org.camunda.bpm.qa.upgrade.json.beans.RegularCustomer;
	using SpecialCustomer = org.camunda.bpm.qa.upgrade.json.beans.SpecialCustomer;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	[ScenarioUnderTest("CreateProcessInstanceWithJsonVariablesScenario"), Origin("7.9.0")]
	public class CreateProcessInstanceWithJsonVariablesTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.json.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.json.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initProcessInstance.1") @Test public void testCreateProcessInstanceWithVariable()
	  [ScenarioUnderTest("initProcessInstance.1")]
	  public virtual void testCreateProcessInstanceWithVariable()
	  {
		// then
		ProcessInstance processInstance = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceBusinessKey("processWithJsonVariables79").singleResult();
		IList<VariableInstance> variables = engineRule.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(processInstance.Id).list();
		assertEquals(4, variables.Count);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object objectVariable = engineRule.getRuntimeService().getVariable(processInstance.getId(), "objectVariable");
		object objectVariable = engineRule.RuntimeService.getVariable(processInstance.Id, "objectVariable");
		assertObjectVariable(objectVariable);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object plainTypeArrayVariable = engineRule.getRuntimeService().getVariable(processInstance.getId(), "plainTypeArrayVariable");
		object plainTypeArrayVariable = engineRule.RuntimeService.getVariable(processInstance.Id, "plainTypeArrayVariable");
		assertPlainTypeArrayVariable(plainTypeArrayVariable);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object notGenericObjectListVariable = engineRule.getRuntimeService().getVariable(processInstance.getId(), "notGenericObjectListVariable");
		object notGenericObjectListVariable = engineRule.RuntimeService.getVariable(processInstance.Id, "notGenericObjectListVariable");
		assertNotGenericObjectListVariable(notGenericObjectListVariable);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.variable.value.TypedValue serializedObject = engineRule.getRuntimeService().getVariableTyped(processInstance.getId(), "serializedMapVariable", true);
		TypedValue serializedObject = engineRule.RuntimeService.getVariableTyped(processInstance.Id, "serializedMapVariable", true);
		assertSerializedMap(serializedObject);
	  }

	  private void assertNotGenericObjectListVariable(object notGenericObjectListVariable)
	  {
		assertTrue(notGenericObjectListVariable is ObjectList);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.qa.upgrade.json.beans.ObjectList list = (org.camunda.bpm.qa.upgrade.json.beans.ObjectList) notGenericObjectListVariable;
		ObjectList list = (ObjectList) notGenericObjectListVariable;
		assertEquals(2, list.Count);
		assertTrue(list[0] is RegularCustomer);
		assertEquals("someCustomer", ((RegularCustomer) list[0]).Name);
		assertEquals(5, ((RegularCustomer) list[0]).ContractStartDate);
		assertTrue(list[1] is RegularCustomer);
		assertEquals("secondCustomer", ((RegularCustomer) list[1]).Name);
		assertEquals(666, ((RegularCustomer) list[1]).ContractStartDate);
	  }

	  public virtual void assertObjectVariable(object objectVariable)
	  {
		assertTrue(objectVariable is Order);
		Order order = (Order)objectVariable;
		//check couple of fields
		assertEquals(1234567890987654321L, order.Id);
		assertEquals("order1", order.getOrder());
		assertTrue(order.Active);
	  }

	  public virtual void assertPlainTypeArrayVariable(object plainTypeArrayVariable)
	  {
		assertTrue(plainTypeArrayVariable is int[]);
		int[] array = (int[])plainTypeArrayVariable;
		assertEquals(2, array.Length);
		assertEquals(5, array[0]);
		assertEquals(10, array[1]);
	  }

	  public virtual void assertSerializedMap(TypedValue typedValue)
	  {
		Dictionary<string, string> expected = new Dictionary<string, string>();
		expected["foo"] = "bar";
		Assert.assertEquals(expected, typedValue.Value);
	  }

	}
}