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
namespace org.camunda.bpm.engine.test.api.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	public class ModifyVariableInSameTransactionTest
	{
		private bool InstanceFieldsInitialized = false;

		public ModifyVariableInSameTransactionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testHelper = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testDeleteAndInsertTheSameVariableByteArray()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testDeleteAndInsertTheSameVariableByteArray()
	  {
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("serviceTaskProcess").startEvent().userTask("userTask").serviceTask("service").camundaClass(typeof(DeleteAndInsertVariableDelegate)).userTask("userTask1").endEvent().done();
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(bpmnModel);
		VariableMap variables = Variables.createVariables().putValue("listVar", Arrays.asList(new int[] {1, 2, 3}));
		ProcessInstance instance = engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id, variables);

		Task task = engineRule.TaskService.createTaskQuery().singleResult();
		engineRule.TaskService.complete(task.Id);

		VariableInstance variable = engineRule.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(instance.Id).variableName("listVar").singleResult();
		assertNotNull(variable);
		assertEquals("stringValue", variable.Value);
		HistoricVariableInstance historicVariable = engineRule.HistoryService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(variable.Name, historicVariable.Name);
		assertEquals(org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_CREATED, historicVariable.State);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testDeleteAndInsertTheSameVariable()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testDeleteAndInsertTheSameVariable()
	  {
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("serviceTaskProcess").startEvent().userTask("userTask").serviceTask("service").camundaClass(typeof(DeleteAndInsertVariableDelegate)).userTask("userTask1").endEvent().done();
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(bpmnModel);
		VariableMap variables = Variables.createVariables().putValue("foo", "firstValue");
		ProcessInstance instance = engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id, variables);

		Task task = engineRule.TaskService.createTaskQuery().singleResult();
		engineRule.TaskService.complete(task.Id);

		VariableInstance variable = engineRule.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(instance.Id).variableName("foo").singleResult();
		assertNotNull(variable);
		assertEquals("secondValue", variable.Value);
		HistoricVariableInstance historicVariable = engineRule.HistoryService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(variable.Name, historicVariable.Name);
		assertEquals(org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_CREATED, historicVariable.State);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testInsertDeleteInsertTheSameVariable()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testInsertDeleteInsertTheSameVariable()
	  {
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("serviceTaskProcess").startEvent().userTask("userTask").serviceTask("service").camundaClass(typeof(InsertDeleteInsertVariableDelegate)).userTask("userTask1").endEvent().done();
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(bpmnModel);
		VariableMap variables = Variables.createVariables().putValue("listVar", Arrays.asList(new int[] {1, 2, 3}));
		ProcessInstance instance = engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id, variables);

		Task task = engineRule.TaskService.createTaskQuery().singleResult();
		engineRule.TaskService.complete(task.Id);

		VariableInstance variable = engineRule.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(instance.Id).variableName("foo").singleResult();
		assertNotNull(variable);
		assertEquals("bar", variable.Value);
		IList<HistoricVariableInstance> historyVariables = engineRule.HistoryService.createHistoricVariableInstanceQuery().list();
		foreach (HistoricVariableInstance historicVariable in historyVariables)
		{
		  if (variable.Name.Equals(historicVariable.Name))
		  {
			assertEquals(org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_CREATED, historicVariable.State);
			break;
		  }
		}
	  }
	}

}