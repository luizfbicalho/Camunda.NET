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
namespace org.camunda.bpm.engine.test.history
{

	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIdentityLinkLogQueryTest : PluggableProcessEngineTestCase
	{
	  private const string A_USER_ID = "aUserId";
	  private const string A_GROUP_ID = "aGroupId";
	  private const int numberOfUsers = 3;
	  private const string A_ASSIGNER_ID = "aAssignerId";

	  private const string INVALID_USER_ID = "InvalidUserId";
	  private const string INVALID_TASK_ID = "InvalidTask";
	  private const string INVALID_GROUP_ID = "InvalidGroupId";
	  private const string INVALID_ASSIGNER_ID = "InvalidAssignerId";
	  private const string INVALID_HISTORY_EVENT_TYPE = "InvalidEventType";
	  private const string INVALID_IDENTITY_LINK_TYPE = "InvalidIdentityLinkType";
	  private const string INVALID_PROCESS_DEFINITION_ID = "InvalidProcessDefinitionId";
	  private const string INVALID_PROCESS_DEFINITION_KEY = "InvalidProcessDefinitionKey";
	  private const string GROUP_1 = "Group1";
	  private const string USER_1 = "User1";
	  private static string PROCESS_DEFINITION_KEY = "oneTaskProcess";
	  private static string PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER = "oneTaskProcessForHistoricIdentityLinkWithMultipleCanidateUser";
	  private const string IDENTITY_LINK_ADD = "add";
	  private const string IDENTITY_LINK_DELETE = "delete";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryAddTaskCandidateforAddIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		ProcessInstance processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);

		// Query test
		HistoricIdentityLinkLog historicIdentityLink = historyService.createHistoricIdentityLinkLogQuery().singleResult();
		assertEquals(historicIdentityLink.UserId, A_USER_ID);
		assertEquals(historicIdentityLink.TaskId, taskId);
		assertEquals(historicIdentityLink.Type, IdentityLinkType.CANDIDATE);
		assertEquals(historicIdentityLink.AssignerId, A_ASSIGNER_ID);
		assertEquals(historicIdentityLink.GroupId, null);
		assertEquals(historicIdentityLink.OperationType, IDENTITY_LINK_ADD);
		assertEquals(historicIdentityLink.ProcessDefinitionId, processInstance.ProcessDefinitionId);
		assertEquals(historicIdentityLink.ProcessDefinitionKey, PROCESS_DEFINITION_KEY);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		ProcessInstance processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateGroup(taskId, A_GROUP_ID);

		// Query test
		HistoricIdentityLinkLog historicIdentityLink = historyService.createHistoricIdentityLinkLogQuery().singleResult();
		assertEquals(historicIdentityLink.UserId, null);
		assertEquals(historicIdentityLink.TaskId, taskId);
		assertEquals(historicIdentityLink.Type, IdentityLinkType.CANDIDATE);
		assertEquals(historicIdentityLink.AssignerId, A_ASSIGNER_ID);
		assertEquals(historicIdentityLink.GroupId, A_GROUP_ID);
		assertEquals(historicIdentityLink.OperationType, IDENTITY_LINK_ADD);
		assertEquals(historicIdentityLink.ProcessDefinitionId, processInstance.ProcessDefinitionId);
		assertEquals(historicIdentityLink.ProcessDefinitionKey, PROCESS_DEFINITION_KEY);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testValidIndividualQueryTaskCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		ProcessInstance processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);
		taskService.deleteCandidateUser(taskId, A_USER_ID);

		// Valid Individual Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.taskId(taskId).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.CANDIDATE).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(A_USER_ID).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.assignerId(A_ASSIGNER_ID).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(processInstance.ProcessDefinitionId).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionKey(PROCESS_DEFINITION_KEY).count(), 2);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testValidGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		ProcessInstance processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);
		taskService.deleteCandidateUser(taskId, A_USER_ID);

		// Valid group query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.taskId(taskId).count(), 2);
		assertEquals(query.type(IdentityLinkType.CANDIDATE).count(), 2);
		assertEquals(query.userId(A_USER_ID).count(), 2);
		assertEquals(query.assignerId(A_ASSIGNER_ID).count(), 2);
		assertEquals(query.processDefinitionId(processInstance.ProcessDefinitionId).count(), 2);
		assertEquals(query.processDefinitionKey(PROCESS_DEFINITION_KEY).count(), 2);
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 1);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInvalidIndividualQueryTaskCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);
		taskService.deleteCandidateUser(taskId, A_USER_ID);

		// Invalid Individual Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.taskId(INVALID_TASK_ID).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(INVALID_IDENTITY_LINK_TYPE).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(INVALID_USER_ID).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.groupId(INVALID_GROUP_ID).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.assignerId(INVALID_ASSIGNER_ID).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(INVALID_HISTORY_EVENT_TYPE).count(), 0);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testInvalidGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);
		taskService.deleteCandidateUser(taskId, A_USER_ID);

		// Invalid Individual Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.taskId(INVALID_TASK_ID).count(), 0);
		assertEquals(query.type(INVALID_IDENTITY_LINK_TYPE).count(), 0);
		assertEquals(query.userId(INVALID_USER_ID).count(), 0);
		assertEquals(query.groupId(INVALID_GROUP_ID).count(), 0);
		assertEquals(query.assignerId(INVALID_ASSIGNER_ID).count(), 0);
		assertEquals(query.operationType(INVALID_HISTORY_EVENT_TYPE).count(), 0);
		assertEquals(query.processDefinitionId(INVALID_PROCESS_DEFINITION_ID).count(), 0);
		assertEquals(query.processDefinitionKey(INVALID_PROCESS_DEFINITION_KEY).count(), 0);
	  }

	  /// <summary>
	  /// Should add 3 history records of identity link addition at 01-01-2016
	  /// 00:00.00 Should add 3 history records of identity link deletion at
	  /// 01-01-2016 12:00.00
	  /// 
	  /// Should add 3 history records of identity link addition at 01-01-2016
	  /// 12:30.00 Should add 3 history records of identity link deletion at
	  /// 01-01-2016 21:00.00
	  /// 
	  /// Test case: Query the number of added records at different time interval.
	  /// </summary>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddTaskOwnerForAddandDeleteIdentityLinkByTimeStamp()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		ClockUtil.CurrentTime = newYearMorning(0);
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		// Adds aUserId1, deletes aUserID1, adds aUserId2, deletes aUserId2, Adds aUserId3 - 5
		addUserIdentityLinks(taskId);

		ClockUtil.CurrentTime = newYearNoon(0);
		//Deletes aUserId3
		deleteUserIdentityLinks(taskId);

		ClockUtil.CurrentTime = newYearNoon(30);
		addUserIdentityLinks(taskId);

		ClockUtil.CurrentTime = newYearEvening();
		deleteUserIdentityLinks(taskId);

		// Query records with time before 12:20
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.dateBefore(newYearNoon(20)).count(), 6);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 3);
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 3);

		// Query records with time between 00:01 and 12:00
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.dateBefore(newYearNoon(0)).count(), 6);
		assertEquals(query.dateAfter(newYearMorning(1)).count(), 1);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 0);
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 1);

		// Query records with time after 12:45
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.dateAfter(newYearNoon(45)).count(), 1);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 0);
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 1);

		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" }) public void testQueryAddAndRemoveIdentityLinksForProcessDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testQueryAddAndRemoveIdentityLinksForProcessDefinition()
	  {

		ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertNotNull(latestProcessDef);
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		assertEquals(0, links.Count);

		// Add candiate group with process definition
		repositoryService.addCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);
		// Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(latestProcessDef.Id).count(), 1);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 1);
		assertEquals(query.groupId(GROUP_1).count(), 1);

		// Add candidate user for process definition
		repositoryService.addCandidateStarterUser(latestProcessDef.Id, USER_1);
		// Query test
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(latestProcessDef.Id).count(), 2);
		assertEquals(query.processDefinitionKey(latestProcessDef.Key).count(), 2);
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 2);
		assertEquals(query.userId(USER_1).count(), 1);

		// Delete candiate group with process definition
		repositoryService.deleteCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
		// Query test
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(latestProcessDef.Id).count(), 3);
		assertEquals(query.processDefinitionKey(latestProcessDef.Key).count(), 3);
		assertEquals(query.groupId(GROUP_1).count(), 2);
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 1);

		// Delete candidate user for process definition
		repositoryService.deleteCandidateStarterUser(latestProcessDef.Id, USER_1);
		// Query test
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(latestProcessDef.Id).count(), 4);
		assertEquals(query.processDefinitionKey(latestProcessDef.Key).count(), 4);
		assertEquals(query.userId(USER_1).count(), 2);
		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 2);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithMultipleCandidateUser.bpmn20.xml" })]
	  public virtual void testHistoricIdentityLinkQueryPaging()
	  {
		startProcessInstance(PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER);

		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		assertEquals(4, query.listPage(0, 4).size());
		assertEquals(1, query.listPage(2, 1).size());
		assertEquals(2, query.listPage(1, 2).size());
		assertEquals(3, query.listPage(1, 4).size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/OneTaskProcessWithMultipleCandidateUser.bpmn20.xml" })]
	  public virtual void testHistoricIdentityLinkQuerySorting()
	  {

		// Pre test - Historical identity link is added as part of deployment
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);
		startProcessInstance(PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER);

		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByAssignerId().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTime().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByGroupId().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByType().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByOperationType().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByProcessDefinitionId().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByProcessDefinitionKey().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTaskId().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByUserId().asc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTenantId().asc().list().size());
		assertEquals("aUser", historyService.createHistoricIdentityLinkLogQuery().orderByUserId().asc().list().get(0).UserId);
		assertEquals("dUser", historyService.createHistoricIdentityLinkLogQuery().orderByUserId().asc().list().get(3).UserId);

		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByAssignerId().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTime().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByGroupId().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByType().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByOperationType().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByProcessDefinitionId().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByProcessDefinitionKey().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTaskId().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByUserId().desc().list().size());
		assertEquals(4, historyService.createHistoricIdentityLinkLogQuery().orderByTenantId().desc().list().size());
		assertEquals("dUser", historyService.createHistoricIdentityLinkLogQuery().orderByUserId().desc().list().get(0).UserId);
		assertEquals("aUser", historyService.createHistoricIdentityLinkLogQuery().orderByUserId().desc().list().get(3).UserId);
	  }

	  public virtual void addUserIdentityLinks(string taskId)
	  {
		for (int userIndex = 1; userIndex <= numberOfUsers; userIndex++)
		{
		  taskService.addUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.ASSIGNEE);
		}
	  }

	  public virtual void deleteUserIdentityLinks(string taskId)
	  {
		for (int userIndex = 1; userIndex <= numberOfUsers; userIndex++)
		{
		  taskService.deleteUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.ASSIGNEE);
		}
	  }

	  public virtual DateTime newYearMorning(int minutes)
	  {
		DateTime calendar = new GregorianCalendar();
		calendar.set(DateTime.YEAR, 2016);
		calendar.set(DateTime.MONTH, 0);
		calendar.set(DateTime.DAY_OF_MONTH, 1);
		calendar.set(DateTime.HOUR_OF_DAY, 0);
		calendar.set(DateTime.MINUTE, minutes);
		calendar.set(DateTime.SECOND, 0);
		calendar.set(DateTime.MILLISECOND, 0);
		DateTime morning = calendar;
		return morning;
	  }

	  public virtual DateTime newYearNoon(int minutes)
	  {
		DateTime calendar = new GregorianCalendar();
		calendar.set(DateTime.YEAR, 2016);
		calendar.set(DateTime.MONTH, 0);
		calendar.set(DateTime.DAY_OF_MONTH, 1);
		calendar.set(DateTime.HOUR_OF_DAY, 12);
		calendar.set(DateTime.MINUTE, minutes);
		calendar.set(DateTime.SECOND, 0);
		calendar.set(DateTime.MILLISECOND, 0);
		DateTime morning = calendar;
		return morning;
	  }

	  public virtual DateTime newYearEvening()
	  {
		DateTime calendar = new GregorianCalendar();
		calendar.set(DateTime.YEAR, 2016);
		calendar.set(DateTime.MONTH, 0);
		calendar.set(DateTime.DAY_OF_MONTH, 1);
		calendar.set(DateTime.HOUR_OF_DAY, 21);
		calendar.set(DateTime.MINUTE, 0);
		calendar.set(DateTime.SECOND, 0);
		calendar.set(DateTime.MILLISECOND, 0);
		DateTime morning = calendar;
		return morning;
	  }

	  protected internal virtual ProcessInstance startProcessInstance(string key)
	  {
		return runtimeService.startProcessInstanceByKey(key);
	  }
	}

}