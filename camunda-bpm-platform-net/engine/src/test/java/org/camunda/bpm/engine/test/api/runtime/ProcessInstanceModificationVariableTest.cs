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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class ProcessInstanceModificationVariableTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessInstanceModificationVariableTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  internal RuntimeService runtimeService;
	  internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initialize()
	  public virtual void initialize()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyAProcessInstanceWithLocalVariableCreation()
	  public virtual void modifyAProcessInstanceWithLocalVariableCreation()
	  {

		// given a process that sets a local variable when entering the user task
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().userTask("userTask").camundaTaskListenerClass("create", "org.camunda.bpm.engine.test.api.runtime.util.CreateLocalVariableEventListener").endEvent().done();

		testHelper.deployAndGetDefinition(instance);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstance.Id);

		// when I start another activity and delete the old one
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("userTask").cancelActivityInstance(updatedTree.getActivityInstances("userTask")[0].Id).execute(false, false);

		// then migration was successful and I can finish the process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		testHelper.assertProcessEnded(processInstance.Id);

	  }

	}

}