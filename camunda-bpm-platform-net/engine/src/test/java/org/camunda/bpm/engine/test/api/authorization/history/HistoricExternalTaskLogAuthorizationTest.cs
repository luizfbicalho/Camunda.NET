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
namespace org.camunda.bpm.engine.test.api.authorization.history
{

	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricExternalTaskLogAuthorizationTest : AuthorizationTest
	{

	  protected internal readonly string WORKER_ID = "aWorkerId";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;
	  protected internal readonly string ERROR_DETAILS = "These are the error details!";
	  protected internal readonly string ANOTHER_PROCESS_KEY = "AnotherProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {

		DeploymentBuilder deploymentbuilder = repositoryService.createDeployment();
		BpmnModelInstance defaultModel = createDefaultExternalTaskModel().build();
		BpmnModelInstance modifiedModel = createDefaultExternalTaskModel().processKey(ANOTHER_PROCESS_KEY).build();
		deploymentId = deployment(deploymentbuilder, defaultModel, modifiedModel);

		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }


	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithHistoryReadPermissionOnProcessDefinition()
	  {

		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithHistoryReadPermissionOnAnyProcessDefinition()
	  {

		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultipleAuthorizations()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithHistoryReadPermissionOnOneProcessDefinition()
	  {
		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 6);
	  }

	  public virtual void testQueryWithHistoryReadPermissionOnAnyProcessDefinition()
	  {
		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();

		// then
		verifyQueryResults(query, 8);
	  }

	  public virtual void testGetErrorDetailsWithoutAuthorization()
	  {
		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();

		disableAuthorization();
		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().list().get(0).Id;
		enableAuthorization();

		try
		{
		  // when
		  string stacktrace = historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
		  fail("Exception expected: It should not be possible to retrieve the error details");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(DEFAULT_PROCESS_KEY, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetErrorDetailsWithHistoryReadPermissionOnProcessDefinition()
	  {

		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, READ_HISTORY);

		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().list().get(0).Id;

		// when
		string stacktrace = historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

		// then
		assertNotNull(stacktrace);
		assertEquals(ERROR_DETAILS, stacktrace);
	  }

	  public virtual void testGetErrorDetailsWithHistoryReadPermissionOnProcessAnyDefinition()
	  {

		// given
		startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().list().get(0).Id;

		// when
		string stacktrace = historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

		// then
		assertNotNull(stacktrace);
		assertEquals(ERROR_DETAILS, stacktrace);
	  }

	  protected internal virtual void startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure()
	  {
		disableAuthorization();
		ProcessInstance pi1 = startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		ProcessInstance pi2 = startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		ProcessInstance pi3 = startProcessInstanceByKey(ANOTHER_PROCESS_KEY);

		completeExternalTaskWithFailure(pi1);
		completeExternalTaskWithFailure(pi2);

		runtimeService.deleteProcessInstance(pi3.Id, "Dummy reason for deletion!");
		enableAuthorization();
	  }

	  protected internal virtual void completeExternalTaskWithFailure(ProcessInstance pi)
	  {
		ExternalTask task = externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
		completeExternalTaskWithFailure(task.Id);
	  }

	  protected internal virtual void completeExternalTaskWithFailure(string externalTaskId)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(5, WORKER_ID, false).topic(DEFAULT_TOPIC, LOCK_DURATION).execute();
		externalTaskService.handleFailure(externalTaskId, WORKER_ID, "This is an error!", ERROR_DETAILS, 1, 0L);
		externalTaskService.complete(externalTaskId, WORKER_ID);
		// unlock the remaining tasks
		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  if (!lockedExternalTask.Id.Equals(externalTaskId))
		  {
			externalTaskService.unlock(lockedExternalTask.Id);
		  }
		}
	  }

	}

}