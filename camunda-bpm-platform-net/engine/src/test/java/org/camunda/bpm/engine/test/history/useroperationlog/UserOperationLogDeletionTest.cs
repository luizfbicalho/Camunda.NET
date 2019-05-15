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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Task = org.camunda.bpm.engine.task.Task;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class UserOperationLogDeletionTest : AbstractUserOperationLogTest
	{

	  public const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";
	  public const string DECISION_DEFINITION_KEY = "testDecision";

	  protected internal const string PROCESS_PATH = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string PROCESS_KEY = "oneTaskProcess";

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testDeleteProcessTaskKeepTaskOperationLog()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setAssignee(taskId, "demo");
		taskService.complete(taskId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().taskId(taskId);
		assertEquals(2, query.count());

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		assertEquals(4, query.count());

		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).taskId(taskId).property("nrOfInstances").singleResult();
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

	  public virtual void testDeleteStandaloneTaskKeepUserOperationLog()
	  {
		// given
		string taskId = "my-task";
		Task task = taskService.newTask(taskId);
		taskService.saveTask(task);

		taskService.setAssignee(taskId, "demo");
		taskService.complete(taskId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().taskId(taskId);
		assertEquals(3, query.count());

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		assertEquals(5, query.count());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDeleteCaseTaskKeepUserOperationLog()
	  {
		// given
		caseService.withCaseDefinitionByKey("oneTaskCase").create();

		caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setAssignee(taskId, "demo");
		taskService.complete(taskId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().taskId(taskId);
		assertEquals(2, query.count());

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		assertEquals(4, query.count());
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testDeleteProcessInstanceKeepUserOperationLog()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		runtimeService.suspendProcessInstanceById(processInstanceId);
		runtimeService.activateProcessInstanceById(processInstanceId);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processInstanceId(processInstanceId);
		assertEquals(4, query.count());

		// when
		historyService.deleteHistoricProcessInstance(processInstanceId);

		// then
		assertEquals(4, query.count());

		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).property("nrOfInstances").singleResult();

		assertNotNull(entry);
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDeleteCaseInstanceKeepUserOperationLog()
	  {
		// given
		string caseInstanceId = caseService.withCaseDefinitionByKey("oneTaskCase").create().Id;

		caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		caseService.closeCaseInstance(caseInstanceId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseInstanceId(caseInstanceId).entityType(EntityTypes.TASK);
		assertEquals(1, query.count());

		// when
		historyService.deleteHistoricCaseInstance(caseInstanceId);

		// then
		assertEquals(1, query.count());

		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).singleResult();

		assertNotNull(entry);
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testDeleteProcessDefinitionKeepUserOperationLog()
	  {
		// given
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		runtimeService.suspendProcessInstanceById(processInstanceId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processInstanceId(processInstanceId);
		assertEquals(2, query.count());

		// when
		repositoryService.deleteProcessDefinition(processDefinitionId, true);

		// then new log is created and old stays
		assertEquals(2, query.count());
	  }

	  public virtual void testDeleteProcessDefinitionsByKey()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deploymentId = repositoryService.createDeployment().addClasspathResource(PROCESS_PATH).deploy().Id;
		  deploymentIds.Add(deploymentId);
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey(PROCESS_KEY).withoutTenantId().delete();

		// then
		assertUserOperationLogs();
	  }

	  public virtual void testDeleteProcessDefinitionsByKeyCascading()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deploymentId = repositoryService.createDeployment().addClasspathResource(PROCESS_PATH).deploy().Id;
		  deploymentIds.Add(deploymentId);
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey(PROCESS_KEY).withoutTenantId().cascade().delete();

		// then
		assertUserOperationLogs();
	  }

	  public virtual void testDeleteProcessDefinitionsByIds()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deploymentId = repositoryService.createDeployment().addClasspathResource(PROCESS_PATH).deploy().Id;
		  deploymentIds.Add(deploymentId);
		}

		// when
		repositoryService.deleteProcessDefinitions().byIds(findProcessDefinitionIdsByKey(PROCESS_KEY)).delete();

		// then
		assertUserOperationLogs();
	  }

	  public virtual void testDeleteProcessDefinitionsByIdsCascading()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deploymentId = repositoryService.createDeployment().addClasspathResource(PROCESS_PATH).deploy().Id;
		  deploymentIds.Add(deploymentId);
		}

		// when
		repositoryService.deleteProcessDefinitions().byIds(findProcessDefinitionIdsByKey(PROCESS_KEY)).cascade().delete();

		// then
		assertUserOperationLogs();
	  }

	  [Deployment(resources : PROCESS_PATH)]
	  public virtual void testDeleteDeploymentKeepUserOperationLog()
	  {
		// given
		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		repositoryService.suspendProcessDefinitionById(processDefinitionId);

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().processDefinitionId(processDefinitionId);
		assertEquals(1, query.count());

		// when
		repositoryService.deleteDeployment(deploymentId, true);

		// then
		assertEquals(1, query.count());
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDeleteDecisionInstanceByDecisionDefinition()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "test";
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		string decisionDefinitionId = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(DECISION_DEFINITION_KEY).singleResult().Id;
		historyService.deleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).property("nrOfInstances").list();

		assertEquals(1, userOperationLogEntries.Count);

		UserOperationLogEntry entry = userOperationLogEntries[0];
		assertEquals("1", entry.NewValue);
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

	  [Deployment(resources : { DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void testDeleteDecisionInstanceById()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input1"] = "test";
		decisionService.evaluateDecisionTableByKey(DECISION_DEFINITION_KEY, variables);

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().singleResult();
		historyService.deleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).property("nrOfInstances").list();

		assertEquals(1, userOperationLogEntries.Count);

		UserOperationLogEntry entry = userOperationLogEntries[0];
		assertEquals("1", entry.NewValue);
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

	  public virtual void assertUserOperationLogs()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		UserOperationLogQuery userOperationLogQuery = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);

		IList<UserOperationLogEntry> userOperationLogs = userOperationLogQuery.list();

		assertEquals(3, userOperationLogs.Count);

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  UserOperationLogEntry userOperationLogEntry = userOperationLogQuery.deploymentId(processDefinition.DeploymentId).singleResult();

		  assertEquals(EntityTypes.PROCESS_DEFINITION, userOperationLogEntry.EntityType);
		  assertEquals(processDefinition.Id, userOperationLogEntry.ProcessDefinitionId);
		  assertEquals(processDefinition.Key, userOperationLogEntry.ProcessDefinitionKey);
		  assertEquals(processDefinition.DeploymentId, userOperationLogEntry.DeploymentId);

		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, userOperationLogEntry.OperationType);

		  assertEquals("cascade", userOperationLogEntry.Property);
		  assertFalse(Convert.ToBoolean(userOperationLogEntry.OrgValue));
		  assertTrue(Convert.ToBoolean(userOperationLogEntry.NewValue));

		  assertEquals(USER_ID, userOperationLogEntry.UserId);

		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, userOperationLogEntry.Category);

		  assertNull(userOperationLogEntry.JobDefinitionId);
		  assertNull(userOperationLogEntry.ProcessInstanceId);
		  assertNull(userOperationLogEntry.CaseInstanceId);
		  assertNull(userOperationLogEntry.CaseDefinitionId);
		}

		assertEquals(6, historyService.createUserOperationLogQuery().count());
	  }

	  private string[] findProcessDefinitionIdsByKey(string processDefinitionKey)
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).list();
		IList<string> processDefinitionIds = new List<string>();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionIds.Add(processDefinition.Id);
		}

		return processDefinitionIds.ToArray();
	  }

	}

}