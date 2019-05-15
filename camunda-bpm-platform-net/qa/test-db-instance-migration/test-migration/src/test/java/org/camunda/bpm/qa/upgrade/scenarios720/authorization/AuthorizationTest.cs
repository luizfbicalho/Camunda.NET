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
namespace org.camunda.bpm.qa.upgrade.scenarios720.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using FormService = org.camunda.bpm.engine.FormService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[ScenarioUnderTest("AuthorizationScenario"), Origin("7.2.0")]
	public class AuthorizationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule("camunda.auth.cfg.xml");
		public UpgradeTestRule rule = new UpgradeTestRule("camunda.auth.cfg.xml");

	  protected internal IdentityService identityService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal FormService formService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		ProcessEngine processEngine = rule.ProcessEngine;

		identityService = processEngine.IdentityService;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		historyService = processEngine.HistoryService;
		formService = processEngine.FormService;

		identityService.clearAuthentication();
		identityService.setAuthentication("test", Arrays.asList("accounting"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetDeployment()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetDeployment()
	  {
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().list();
		assertFalse(deployments.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetProcessDefinition()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetProcessDefinition()
	  {
		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();
		assertNotNull(definition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetProcessInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetProcessInstance()
	  {
		IList<ProcessInstance> instances = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").list();
		assertFalse(instances.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetExecution()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetExecution()
	  {
		IList<Execution> executions = runtimeService.createExecutionQuery().processDefinitionKey("oneTaskProcess").list();
		assertFalse(executions.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetTask()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetTask()
	  {
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").list();
		assertFalse(tasks.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetHistoricProcessInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetHistoricProcessInstance()
	  {
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("oneTaskProcess").list();
		assertFalse(instances.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetHistoricActivityInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetHistoricActivityInstance()
	  {
		IList<HistoricActivityInstance> instances = historyService.createHistoricActivityInstanceQuery().list();
		assertFalse(instances.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testGetHistoricTaskInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testGetHistoricTaskInstance()
	  {
		IList<HistoricTaskInstance> instances = historyService.createHistoricTaskInstanceQuery().processDefinitionKey("oneTaskProcess").list();
		assertFalse(instances.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testStartProcessInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testStartProcessInstance()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		assertNotNull(instance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testSubmitStartForm()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testSubmitStartForm()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult().Id;
		ProcessInstance instance = formService.submitStartForm(processDefinitionId, null);
		assertNotNull(instance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testCompleteTaskInstance()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testCompleteTaskInstance()
	  {
		string taskId = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").listPage(0, 1).get(0).Id;
		taskService.complete(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testSubmitTaskForm()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testSubmitTaskForm()
	  {
		string taskId = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess").listPage(0, 1).get(0).Id;
		formService.submitTaskForm(taskId, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testSetVariable()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testSetVariable()
	  {
		string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcess").listPage(0, 1).get(0).Id;
		runtimeService.setVariable(processInstanceId, "abc", "def");
	  }

	}

}