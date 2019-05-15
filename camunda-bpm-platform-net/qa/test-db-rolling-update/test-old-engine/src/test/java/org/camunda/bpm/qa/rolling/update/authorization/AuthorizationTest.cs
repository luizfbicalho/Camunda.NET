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
namespace org.camunda.bpm.qa.rolling.update.authorization
{
	using FormService = org.camunda.bpm.engine.FormService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
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
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("AuthorizationScenario")]
	public class AuthorizationTest : AbstractRollingUpdateTestCase
	{

	  public const string PROCESS_DEF_KEY = "oneTaskProcess";
	  protected internal const string USER_ID = "user";
	  protected internal const string GROUP_ID = "group";

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
		identityService = rule.IdentityService;
		repositoryService = rule.RepositoryService;
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
		historyService = rule.HistoryService;
		formService = rule.FormService;

		identityService.clearAuthentication();
		identityService.setAuthentication(USER_ID + rule.BuisnessKey, Arrays.asList(GROUP_ID + rule.BuisnessKey));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("startProcessInstance.1") public void testAuthorization()
	  [ScenarioUnderTest("startProcessInstance.1")]
	  public virtual void testAuthorization()
	  {
		//test access process related
		testGetDeployment();
		testGetProcessDefinition();
		testGetProcessInstance();
		testGetExecution();
		testGetTask();

		//test access historic
		testGetHistoricProcessInstance();
		testGetHistoricActivityInstance();
		testGetHistoricTaskInstance();

		//test process modification
		testSetVariable();
		testSubmitStartForm();
		testStartProcessInstance();
		testCompleteTaskInstance();
		testSubmitTaskForm();
	  }


	  public virtual void testGetDeployment()
	  {
		IList<Deployment> deployments = repositoryService.createDeploymentQuery().list();
		assertFalse(deployments.Count == 0);
	  }

	  public virtual void testGetProcessDefinition()
	  {
		IList<ProcessDefinition> definitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(definitions.Count == 0);
	  }

	  public virtual void testGetProcessInstance()
	  {
		IList<ProcessInstance> instances = runtimeService.createProcessInstanceQuery().processInstanceBusinessKey(rule.BuisnessKey).processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(instances.Count == 0);
	  }

	  public virtual void testGetExecution()
	  {
		IList<Execution> executions = runtimeService.createExecutionQuery().processInstanceBusinessKey(rule.BuisnessKey).processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(executions.Count == 0);
	  }

	  public virtual void testGetTask()
	  {
		IList<Task> tasks = taskService.createTaskQuery().processInstanceBusinessKey(rule.BuisnessKey).processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(tasks.Count == 0);
	  }

	  public virtual void testGetHistoricProcessInstance()
	  {
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey(rule.BuisnessKey).processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(instances.Count == 0);
	  }

	  public virtual void testGetHistoricActivityInstance()
	  {
		IList<HistoricActivityInstance> instances = historyService.createHistoricActivityInstanceQuery().list();
		assertFalse(instances.Count == 0);
	  }

	  public virtual void testGetHistoricTaskInstance()
	  {
		IList<HistoricTaskInstance> instances = historyService.createHistoricTaskInstanceQuery().processDefinitionKey(PROCESS_DEF_KEY).list();
		assertFalse(instances.Count == 0);
	  }

	  public virtual void testStartProcessInstance()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, rule.BuisnessKey);
		assertNotNull(instance);
	  }

	  public virtual void testSubmitStartForm()
	  {
		ProcessInstance instance = formService.submitStartForm(rule.processInstance().ProcessDefinitionId, rule.BuisnessKey, null);
		assertNotNull(instance);
	  }

	  public virtual void testCompleteTaskInstance()
	  {
		string taskId = taskService.createTaskQuery().processDefinitionKey(PROCESS_DEF_KEY).processInstanceBusinessKey(rule.BuisnessKey).listPage(0, 1).get(0).Id;
		taskService.complete(taskId);
	  }

	  public virtual void testSubmitTaskForm()
	  {
		string taskId = taskService.createTaskQuery().processDefinitionKey(PROCESS_DEF_KEY).processInstanceBusinessKey(rule.BuisnessKey).listPage(0, 1).get(0).Id;
		formService.submitTaskForm(taskId, null);
	  }

	  public virtual void testSetVariable()
	  {
		string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_DEF_KEY).listPage(0, 1).get(0).Id;
		runtimeService.setVariable(processInstanceId, "abc", "def");
	  }
	}

}