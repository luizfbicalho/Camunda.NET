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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using Task = org.camunda.bpm.engine.task.Task;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricIdentityLinkLogAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_PROCESS_KEY = "demoAssigneeProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";
	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // historic identity link query (standalone task) - Authorization

	  public virtual void testQueryForStandaloneTaskHistoricIdentityLinkWithoutAuthrorization()
	  {
		// given
		disableAuthorization();

		Task taskAssignee = taskService.newTask("newTask");
		taskAssignee.Assignee = "aUserId";
		taskService.saveTask(taskAssignee);

		enableAuthorization();

		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 1);

		disableAuthorization();
		taskService.deleteTask("newTask", true);
		enableAuthorization();
	  }

	  public virtual void testQueryForTaskHistoricIdentityLinkWithoutUserPermission()
	  {
		// given
		disableAuthorization();
		startProcessInstanceByKey(ONE_PROCESS_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateUser(taskId, "aUserId");

		enableAuthorization();

		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryForTaskHistoricIdentityLinkWithUserPermission()
	  {
		// given
		disableAuthorization();
		startProcessInstanceByKey(ONE_PROCESS_KEY);

		// if
		createGrantAuthorization(PROCESS_DEFINITION, ONE_PROCESS_KEY, userId, READ_HISTORY);

		enableAuthorization();
		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		disableAuthorization();
		startProcessInstanceByKey(ONE_PROCESS_KEY);

		// if
		createGrantAuthorization(PROCESS_DEFINITION, ONE_PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		enableAuthorization();
		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryCaseTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// if
		identityService.AuthenticatedUserId = "aAssignerId";
		taskService.addCandidateUser(taskId, "aUserId");
		enableAuthorization();

		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testMixedQuery()
	  {

		disableAuthorization();
		// given
		startProcessInstanceByKey(ONE_PROCESS_KEY);
		startProcessInstanceByKey(ONE_PROCESS_KEY);
		startProcessInstanceByKey(ONE_PROCESS_KEY);

		createCaseInstanceByKey(CASE_KEY);
		taskService.addCandidateUser(taskService.createTaskQuery().list().get(3).Id, "dUserId");
		createCaseInstanceByKey(CASE_KEY);
		taskService.addCandidateUser(taskService.createTaskQuery().list().get(4).Id, "eUserId");

		createTaskAndAssignUser("one");
		createTaskAndAssignUser("two");
		createTaskAndAssignUser("three");
		createTaskAndAssignUser("four");
		createTaskAndAssignUser("five");

		enableAuthorization();

		// when
		HistoricIdentityLinkLogQuery query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 7);

		disableAuthorization();

		query = historyService.createHistoricIdentityLinkLogQuery();
		// then
		verifyQueryResults(query, 10);

		// if
		createGrantAuthorization(PROCESS_DEFINITION, ONE_PROCESS_KEY, userId, READ_HISTORY);
		enableAuthorization();
		query = historyService.createHistoricIdentityLinkLogQuery();

		// then
		verifyQueryResults(query, 10);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  public virtual void createTaskAndAssignUser(string taskId)
	  {
		Task task = taskService.newTask(taskId);
		task.Assignee = "demo";
		taskService.saveTask(task);
	  }

	}

}