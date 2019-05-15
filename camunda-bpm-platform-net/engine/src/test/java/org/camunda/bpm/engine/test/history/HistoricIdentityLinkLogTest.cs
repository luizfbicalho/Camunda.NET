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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIdentityLinkLogTest : PluggableProcessEngineTestCase
	{
	  private const string A_USER_ID = "aUserId";
	  private const string B_USER_ID = "bUserId";
	  private const string C_USER_ID = "cUserId";
	  private const int numberOfUsers = 3;
	  private const string A_GROUP_ID = "aGroupId";
	  private const string INVALID_USER_ID = "InvalidUserId";
	  private const string A_ASSIGNER_ID = "aAssignerId";
	  private static string PROCESS_DEFINITION_KEY = "oneTaskProcess";
	  private const string GROUP_1 = "Group1";
	  private const string USER_1 = "User1";
	  private const string OWNER_1 = "Owner1";
	  private const string IDENTITY_LINK_ADD = "add";
	  private const string IDENTITY_LINK_DELETE = "delete";

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddTaskCandidateforAddIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);

		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddDelegateTaskCandidateforAddIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addUserIdentityLink(taskId, A_USER_ID, IdentityLinkType.ASSIGNEE);
		taskService.delegateTask(taskId, B_USER_ID);
		taskService.deleteUserIdentityLink(taskId, B_USER_ID, IdentityLinkType.ASSIGNEE);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		// Addition of A_USER, Deletion of A_USER, Addition of A_USER as owner, Addition of B_USER and deletion of B_USER
		assertEquals(historicIdentityLinks.Count, 5);

		//Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(A_USER_ID).count(), 3);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(B_USER_ID).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 3);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 4);
		assertEquals(query.type(IdentityLinkType.OWNER).count(), 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddClaimTaskCandidateforAddIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.claim(taskId, A_USER_ID);

		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		//Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(A_USER_ID).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 0);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 1);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddMultipleDelegateTaskCandidateforAddIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addUserIdentityLink(taskId, A_USER_ID, IdentityLinkType.ASSIGNEE);
		taskService.delegateTask(taskId, B_USER_ID);
		taskService.delegateTask(taskId, C_USER_ID);
		taskService.deleteUserIdentityLink(taskId, C_USER_ID, IdentityLinkType.ASSIGNEE);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		// Addition of A_USER, Deletion of A_USER, Addition of A_USER as owner,
		// Addition of B_USER, Deletion of B_USER, Addition of C_USER, Deletion of C_USER
		assertEquals(historicIdentityLinks.Count, 7);

		//Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(A_USER_ID).count(), 3);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(B_USER_ID).count(), 2);


		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.userId(C_USER_ID).count(), 2);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_ADD).count(), 4);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.operationType(IDENTITY_LINK_DELETE).count(), 3);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 6);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.OWNER).count(), 1);
	  }
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddTaskCandidateForAddAndDeleteIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateUser(taskId, A_USER_ID);
		taskService.deleteCandidateUser(taskId, A_USER_ID);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddGroupCandidateForAddAndDeleteIdentityLink()
	  {

		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addCandidateGroup(taskId, A_GROUP_ID);
		taskService.deleteCandidateGroup(taskId, A_GROUP_ID);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);

		// Basic Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.groupId(A_GROUP_ID).count(), 2);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldNotAddTaskCandidateForInvalidIdentityLinkDelete()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.deleteCandidateUser(taskId, INVALID_USER_ID);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddTaskAssigneeForAddandDeleteIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		addAndDeleteUserWithAssigner(taskId, IdentityLinkType.ASSIGNEE);
		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);

		// Basic Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" }) public void testShouldAddAndRemoveIdentityLinksForProcessDefinition() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddAndRemoveIdentityLinksForProcessDefinition()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// Given
		ProcessDefinition latestProcessDef = repositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).singleResult();
		assertNotNull(latestProcessDef);
		IList<IdentityLink> links = repositoryService.getIdentityLinksForProcessDefinition(latestProcessDef.Id);
		assertEquals(0, links.Count);

		// Add candiate group with process definition
		repositoryService.addCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 1);

		// Add candidate user for process definition
		repositoryService.addCandidateStarterUser(latestProcessDef.Id, USER_1);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);

		// Delete candiate group with process definition
		repositoryService.deleteCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 3);

		// Delete candidate user for process definition
		repositoryService.deleteCandidateStarterUser(latestProcessDef.Id, USER_1);
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 4);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddTaskOwnerForAddandDeleteIdentityLink()
	  {

		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		// given
		startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		addAndDeleteUserWithAssigner(taskId, IdentityLinkType.OWNER);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);

		// Basic Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.OWNER).count(), 2);
	  }

	  public virtual void testShouldAddIdentityLinkForTaskCreationWithAssigneeAndOwner()
	  {

		string taskAssigneeId = "Assigneee";
		string taskOwnerId = "Owner";
		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		Task taskAssignee = taskService.newTask(taskAssigneeId);
		taskAssignee.Assignee = USER_1;
		taskService.saveTask(taskAssignee);

		Task taskOwner = taskService.newTask(taskOwnerId);
		taskOwner.Owner = OWNER_1;
		taskService.saveTask(taskOwner);

		Task taskEmpty = taskService.newTask();
		taskService.saveTask(taskEmpty);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 2);

		// Basic Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 1);
		assertEquals(query.userId(USER_1).count(), 1);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.OWNER).count(), 1);
		assertEquals(query.userId(OWNER_1).count(), 1);

		taskService.deleteTask(taskAssigneeId,true);
		taskService.deleteTask(taskOwnerId,true);
		taskService.deleteTask(taskEmpty.Id, true);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldAddIdentityLinkByProcessDefinitionAndStandalone()
	  {

		string taskAssigneeId = "Assigneee";
		// Pre test
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);

		ProcessInstance processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// given
		Task taskAssignee = taskService.newTask(taskAssigneeId);
		taskAssignee.Assignee = USER_1;
		taskService.saveTask(taskAssignee);

		// if
		addAndDeleteUserWithAssigner(taskId, IdentityLinkType.ASSIGNEE);

		// then
		historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 3);

		// Basic Query test
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.type(IdentityLinkType.ASSIGNEE).count(), 3);

		query = historyService.createHistoricIdentityLinkLogQuery();
		assertEquals(query.processDefinitionId(processInstance.ProcessDefinitionId).count(), 2);
		assertEquals(query.processDefinitionKey(PROCESS_DEFINITION_KEY).count(), 2);

		taskService.deleteTask(taskAssigneeId, true);
	  }

	  //CAM-7456
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testShouldNotDeleteIdentityLinkForTaskCompletion()
	  {
		//given
		IList<HistoricIdentityLinkLog> historicIdentityLinks = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(historicIdentityLinks.Count, 0);
		startProcessInstance(PROCESS_DEFINITION_KEY);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.addCandidateUser(task.Id, "demo");

		//when
		taskService.complete(task.Id);

		//then
		IList<HistoricIdentityLinkLog> historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(1, historicIdentityLinkLogs.Count);
		assertNotEquals(IDENTITY_LINK_DELETE, historicIdentityLinkLogs[0].OperationType);
	  }

	  public virtual void addAndDeleteUserWithAssigner(string taskId, string identityLinkType)
	  {
		identityService.AuthenticatedUserId = A_ASSIGNER_ID;
		taskService.addUserIdentityLink(taskId, A_USER_ID, identityLinkType);
		taskService.deleteUserIdentityLink(taskId, A_USER_ID, identityLinkType);
	  }

	  public virtual void addUserIdentityLinks(string taskId)
	  {
		for (int userIndex = 1; userIndex <= numberOfUsers; userIndex++)
		{
		  taskService.addUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.OWNER);
		}
	  }

	  public virtual void deleteUserIdentityLinks(string taskId)
	  {
		for (int userIndex = 1; userIndex <= numberOfUsers; userIndex++)
		{
		  taskService.deleteUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.OWNER);
		}
	  }

	  protected internal virtual ProcessInstance startProcessInstance(string key)
	  {
		return runtimeService.startProcessInstanceByKey(key);
	  }

	}

}