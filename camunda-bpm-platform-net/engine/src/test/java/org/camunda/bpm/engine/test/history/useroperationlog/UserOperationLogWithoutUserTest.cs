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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class UserOperationLogWithoutUserTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_PATH = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testCompleteTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.complete(taskId);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testAssignTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setAssignee(taskId, "demo");

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testClaimTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.claim(taskId, "demo");

		// then
		verifyNoUserOperationLogged();
	  }

	  public virtual void testCreateTask()
	  {
		// when
		Task task = taskService.newTask("a-task-id");
		taskService.saveTask(task);

		// then
		verifyNoUserOperationLogged();

		taskService.deleteTask("a-task-id", true);
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testDelegateTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.delegateTask(taskId, "demo");

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testResolveTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.resolveTask(taskId);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testSetOwnerTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setOwner(taskId, "demo");

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testSetPriorityTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setPriority(taskId, 60);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testUpdateTask()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().singleResult();
		task.CaseInstanceId = "a-case-instance-id";

		// when
		taskService.saveTask(task);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testActivateProcessInstance()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		// when
		runtimeService.activateProcessInstanceById(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testSuspendProcessInstance()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		// when
		runtimeService.suspendProcessInstanceById(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
	  public virtual void testActivateJobDefinition()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneFailingServiceTaskProcess");
		string id = managementService.createJobDefinitionQuery().singleResult().Id;

		// when
		managementService.activateJobByJobDefinitionId(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
	  public virtual void testSuspendJobDefinition()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneFailingServiceTaskProcess");
		string id = managementService.createJobDefinitionQuery().singleResult().Id;

		// when
		managementService.suspendJobByJobDefinitionId(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
	  public virtual void testActivateJob()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneFailingServiceTaskProcess");
		string id = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.activateJobById(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
	  public virtual void testSuspendJob()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneFailingServiceTaskProcess");
		string id = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.suspendJobById(id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
	  public virtual void testSetJobRetries()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneFailingServiceTaskProcess");
		string id = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.setJobRetries(id, 5);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testActivateProcessDefinition()
	  {
		// when
		repositoryService.activateProcessDefinitionByKey(PROCESS_KEY);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testSuspendProcessDefinition()
	  {
		// when
		repositoryService.suspendProcessDefinitionByKey(PROCESS_KEY);

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testModifyProcessInstance()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		// when
		runtimeService.createProcessInstanceModification(id).cancelAllForActivity("theTask").execute();

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testSetVariable()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		// when
		runtimeService.setVariable(id, "aVariable", "aValue");

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testRemoveVariable()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;
		runtimeService.setVariable(id, "aVariable", "aValue");

		// when
		runtimeService.removeVariable(id, "aVariable");

		// then
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteHistoricVariable()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;
		runtimeService.setVariable(id, "aVariable", "aValue");
		runtimeService.deleteProcessInstance(id, "none");
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());
		string historicVariableId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(historicVariableId);

		// then
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : PROCESS_PATH), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testDeleteAllHistoricVariables()
	  {
		// given
		string id = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;
		runtimeService.setVariable(id, "aVariable", "aValue");
		runtimeService.deleteProcessInstance(id, "none");
		assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(id);

		// then
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
		verifyNoUserOperationLogged();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testQueryDeleteVariableHistoryOperationOnCase()
	  {
		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");
		caseService.setVariable(caseInstance.Id, "myVariable", 1);
		caseService.setVariable(caseInstance.Id, "myVariable", 2);
		caseService.setVariable(caseInstance.Id, "myVariable", 3);
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		verifyNoUserOperationLogged();
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testQueryDeleteVariableHistoryOperationOnStandaloneTask()
	  {
		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariable(task.Id, "testVariable", "testValue");
		taskService.setVariable(task.Id, "testVariable", "testValue2");
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		verifyNoUserOperationLogged();

		taskService.deleteTask(task.Id, true);
	  }

	  protected internal virtual void verifyNoUserOperationLogged()
	  {
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(0, query.count());
	  }

	}

}