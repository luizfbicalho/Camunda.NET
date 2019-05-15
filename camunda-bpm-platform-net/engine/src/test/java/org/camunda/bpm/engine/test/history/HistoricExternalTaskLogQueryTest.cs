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
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.*;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricExternalTaskLogQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricExternalTaskLogQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal readonly string WORKER_ID = "aWorkerId";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

	  protected internal ProcessInstance processInstance;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		externalTaskService = engineRule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuery()
	  public virtual void testQuery()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryById()
	  public virtual void testQueryById()
	  {
		// given
		startExternalTaskProcesses(2);
		string logId = retrieveFirstHistoricExternalTaskLog().Id;

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().logId(logId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.Id, @is(logId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidId()
	  public virtual void testQueryFailsByInvalidId()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().logId(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingId()
	  public virtual void testQueryByNonExistingId()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().logId("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExternalTaskId()
	  public virtual void testQueryByExternalTaskId()
	  {
		// given
		startExternalTaskProcesses(2);
		string logExternalTaskId = retrieveFirstHistoricExternalTaskLog().ExternalTaskId;

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().externalTaskId(logExternalTaskId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(logExternalTaskId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidExternalTaskId()
	  public virtual void testQueryFailsByInvalidExternalTaskId()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().externalTaskId(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingExternalTaskId()
	  public virtual void testQueryByNonExistingExternalTaskId()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().externalTaskId("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTopicName()
	  public virtual void testQueryByTopicName()
	  {

		// given
		string dummyTopic = "dummy";
		startExternalTaskProcessGivenTopicName(dummyTopic);
		ExternalTask task = startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().topicName(DEFAULT_TOPIC).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidTopicName()
	  public virtual void testQueryFailsByInvalidTopicName()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().topicName(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingTopicName()
	  public virtual void testQueryByNonExistingTopicName()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().topicName("foo bar").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByWorkerId()
	  public virtual void testQueryByWorkerId()
	  {
		// given
		IList<ExternalTask> taskList = startExternalTaskProcesses(2);
		ExternalTask taskToCheck = taskList[1];
		completeExternalTaskWithWorker(taskList[0].Id, "dummyWorker");
		completeExternalTaskWithWorker(taskToCheck.Id, WORKER_ID);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().workerId(WORKER_ID).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(taskToCheck.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidWorkerId()
	  public virtual void testQueryFailsByInvalidWorkerId()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		completeExternalTask(task.Id);

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().workerId(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingWorkerId()
	  public virtual void testQueryByNonExistingWorkerId()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		completeExternalTask(task.Id);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().workerId("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByErrorMessage()
	  public virtual void testQueryByErrorMessage()
	  {
		// given
		IList<ExternalTask> taskList = startExternalTaskProcesses(2);
		string errorMessage = "This is an important error!";
		reportExternalTaskFailure(taskList[0].Id, "Dummy error message");
		reportExternalTaskFailure(taskList[1].Id, errorMessage);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().errorMessage(errorMessage).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(taskList[1].Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidErrorMessage()
	  public virtual void testQueryFailsByInvalidErrorMessage()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().errorMessage(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingErrorMessage()
	  public virtual void testQueryByNonExistingErrorMessage()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().errorMessage("asdfasdf").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityId()
	  public virtual void testQueryByActivityId()
	  {
		// given
		startExternalTaskProcessGivenActivityId("dummyName");
		ExternalTask task = startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().activityIdIn(DEFAULT_EXTERNAL_TASK_NAME).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityIdsIsNull()
	  public virtual void testQueryFailsByActivityIdsIsNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().activityIdIn((string[]) null).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityIdsContainNull()
	  public virtual void testQueryFailsByActivityIdsContainNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainNull = new string[] {"a", null, "b"};
		historyService.createHistoricExternalTaskLogQuery().activityIdIn(activityIdsContainNull).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityIdsContainEmptyString()
	  public virtual void testQueryFailsByActivityIdsContainEmptyString()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainEmptyString = new string[] {"a", "", "b"};
		historyService.createHistoricExternalTaskLogQuery().activityIdIn(activityIdsContainEmptyString).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingActivityIds()
	  public virtual void testQueryByNonExistingActivityIds()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().activityIdIn("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceIds()
	  public virtual void testQueryByActivityInstanceIds()
	  {
		// given
		startExternalTaskProcessGivenActivityId("dummyName");
		ExternalTask task = startExternalTaskProcess();
		string activityInstanceId = historyService.createHistoricActivityInstanceQuery().activityId(DEFAULT_EXTERNAL_TASK_NAME).singleResult().Id;

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().activityInstanceIdIn(activityInstanceId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityInstanceIdsIsNull()
	  public virtual void testQueryFailsByActivityInstanceIdsIsNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().activityInstanceIdIn((string[]) null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityInstanceIdsContainNull()
	  public virtual void testQueryFailsByActivityInstanceIdsContainNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainNull = new string[] {"a", null, "b"};
		historyService.createHistoricExternalTaskLogQuery().activityInstanceIdIn(activityIdsContainNull).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByActivityInstanceIdsContainEmptyString()
	  public virtual void testQueryFailsByActivityInstanceIdsContainEmptyString()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainEmptyString = new string[] {"a", "", "b"};
		historyService.createHistoricExternalTaskLogQuery().activityInstanceIdIn(activityIdsContainEmptyString).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingActivityInstanceIds()
	  public virtual void testQueryByNonExistingActivityInstanceIds()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().activityInstanceIdIn("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionIds()
	  public virtual void testQueryByExecutionIds()
	  {
		// given
		startExternalTaskProcesses(2);
		HistoricExternalTaskLog taskLog = retrieveFirstHistoricExternalTaskLog();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().executionIdIn(taskLog.ExecutionId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.Id, @is(taskLog.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByExecutionIdsIsNull()
	  public virtual void testQueryFailsByExecutionIdsIsNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().executionIdIn((string[]) null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByExecutionIdsContainNull()
	  public virtual void testQueryFailsByExecutionIdsContainNull()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainNull = new string[] {"a", null, "b"};
		historyService.createHistoricExternalTaskLogQuery().executionIdIn(activityIdsContainNull).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByExecutionIdsContainEmptyString()
	  public virtual void testQueryFailsByExecutionIdsContainEmptyString()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		string[] activityIdsContainEmptyString = new string[] {"a", "", "b"};
		historyService.createHistoricExternalTaskLogQuery().executionIdIn(activityIdsContainEmptyString).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingExecutionIds()
	  public virtual void testQueryByNonExistingExecutionIds()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().executionIdIn("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		// given
		startExternalTaskProcesses(2);
		string processInstanceId = retrieveFirstHistoricExternalTaskLog().ProcessInstanceId;

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processInstanceId(processInstanceId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ProcessInstanceId, @is(processInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidProcessInstanceId()
	  public virtual void testQueryFailsByInvalidProcessInstanceId()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().processInstanceId(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingProcessInstanceId()
	  public virtual void testQueryByNonExistingProcessInstanceId()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processInstanceId("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionId()
	  public virtual void testQueryByProcessDefinitionId()
	  {
		// given
		startExternalTaskProcesses(2);
		string definitionId = retrieveFirstHistoricExternalTaskLog().ProcessDefinitionId;

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processDefinitionId(definitionId).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ProcessDefinitionId, @is(definitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidProcessDefinitionId()
	  public virtual void testQueryFailsByInvalidProcessDefinitionId()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().processDefinitionId(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingProcessDefinitionId()
	  public virtual void testQueryByNonExistingProcessDefinitionId()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processDefinitionId("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKey()
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		// given
		startExternalTaskProcessGivenProcessDefinitionKey("dummyProcess");
		ExternalTask task = startExternalTaskProcessGivenProcessDefinitionKey("Process");

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processDefinitionKey(task.ProcessDefinitionKey).singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryFailsByInvalidProcessDefinitionKey()
	  public virtual void testQueryFailsByInvalidProcessDefinitionKey()
	  {

		// given
		startExternalTaskProcess();

		// then
		thrown.expect(typeof(NotValidException));

		// when
		historyService.createHistoricExternalTaskLogQuery().processDefinitionKey(null).singleResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingProcessDefinitionKey()
	  public virtual void testQueryByNonExistingProcessDefinitionKey()
	  {

		// given
		startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().processDefinitionKey("foo").singleResult();

		// then
		assertNull(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCreationLog()
	  public virtual void testQueryByCreationLog()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		completeExternalTask(task.Id);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().creationLog().singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFailureLog()
	  public virtual void testQueryByFailureLog()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		reportExternalTaskFailure(task.Id, "Dummy error message!");

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBySuccessLog()
	  public virtual void testQueryBySuccessLog()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		completeExternalTask(task.Id);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().successLog().singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDeletionLog()
	  public virtual void testQueryByDeletionLog()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		runtimeService.deleteProcessInstance(task.ProcessInstanceId, null);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().deletionLog().singleResult();

		// then
		assertNotNull(log);
		assertThat(log.ExternalTaskId, @is(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByLowerThanOrEqualAPriority()
	  public virtual void testQueryByLowerThanOrEqualAPriority()
	  {

		// given
		startExternalTaskProcesses(5);

		// when
		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().priorityLowerThanOrEquals(2L).list();

		// then
		assertThat(externalTaskLogs.Count, @is(3));
		foreach (HistoricExternalTaskLog log in externalTaskLogs)
		{
		  assertTrue(log.Priority <= 2);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByHigherThanOrEqualAPriority()
	  public virtual void testQueryByHigherThanOrEqualAPriority()
	  {

		// given
		startExternalTaskProcesses(5);

		// when
		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().priorityHigherThanOrEquals(2L).list();

		// then
		assertThat(externalTaskLogs.Count, @is(3));
		foreach (HistoricExternalTaskLog log in externalTaskLogs)
		{
		  assertTrue(log.Priority >= 2);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByPriorityRange()
	  public virtual void testQueryByPriorityRange()
	  {

		// given
		startExternalTaskProcesses(5);

		// when
		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().priorityLowerThanOrEquals(3L).priorityHigherThanOrEquals(1L).list();

		// then
		assertThat(externalTaskLogs.Count, @is(3));
		foreach (HistoricExternalTaskLog log in externalTaskLogs)
		{
		  assertTrue(log.Priority <= 3 && log.Priority >= 1);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDisjunctivePriorityStatements()
	  public virtual void testQueryByDisjunctivePriorityStatements()
	  {

		// given
		startExternalTaskProcesses(5);

		// when
		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().priorityLowerThanOrEquals(1L).priorityHigherThanOrEquals(4L).list();

		// then
		assertThat(externalTaskLogs.Count, @is(0));
	  }


	  // helper methods

	  protected internal virtual HistoricExternalTaskLog retrieveFirstHistoricExternalTaskLog()
	  {
		return historyService.createHistoricExternalTaskLogQuery().list().get(0);
	  }

	  protected internal virtual void completeExternalTaskWithWorker(string externalTaskId, string workerId)
	  {
		completeExternalTask(externalTaskId, DEFAULT_TOPIC, workerId, false);

	  }

	  protected internal virtual void completeExternalTask(string externalTaskId)
	  {
		completeExternalTask(externalTaskId, DEFAULT_TOPIC, WORKER_ID, false);
	  }

	  protected internal virtual void completeExternalTask(string externalTaskId, string topic, string workerId, bool usePriority)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, workerId, usePriority).topic(topic, LOCK_DURATION).execute();
		externalTaskService.complete(externalTaskId, workerId);
		// unlock the remaining tasks
		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  if (!lockedExternalTask.Id.Equals(externalTaskId))
		  {
			externalTaskService.unlock(lockedExternalTask.Id);
		  }
		}
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string errorMessage)
	  {
		reportExternalTaskFailure(externalTaskId, DEFAULT_TOPIC, WORKER_ID, 1, false, errorMessage);
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string topic, string workerId, int? retries, bool usePriority, string errorMessage)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, workerId, usePriority).topic(topic, LOCK_DURATION).execute();
		externalTaskService.handleFailure(externalTaskId, workerId, errorMessage, retries.Value, 0L);

		foreach (LockedExternalTask lockedExternalTask in list)
		{
			externalTaskService.unlock(lockedExternalTask.Id);
		}
	  }

	  protected internal virtual IList<ExternalTask> startExternalTaskProcesses(int count)
	  {
		IList<ExternalTask> list = new LinkedList<ExternalTask>();
		for (int ithPrio = 0; ithPrio < count; ithPrio++)
		{
		  list.Add(startExternalTaskProcessGivenPriority(ithPrio));
		}
		return list;
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenTopicName(string topicName)
	  {
		BpmnModelInstance processModelWithCustomTopic = createDefaultExternalTaskModel().topic(topicName).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomTopic);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenActivityId(string activityId)
	  {
		BpmnModelInstance processModelWithCustomActivityId = createDefaultExternalTaskModel().externalTaskName(activityId).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomActivityId);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenProcessDefinitionKey(string processDefinitionKey)
	  {
		BpmnModelInstance processModelWithCustomKey = createDefaultExternalTaskModel().processKey(processDefinitionKey).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomKey);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenPriority(int priority)
	  {
		BpmnModelInstance processModelWithCustomPriority = createDefaultExternalTaskModel().priority(priority).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomPriority);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcess()
	  {
		BpmnModelInstance oneExternalTaskProcess = createDefaultExternalTaskModel().build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(oneExternalTaskProcess);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	}

}