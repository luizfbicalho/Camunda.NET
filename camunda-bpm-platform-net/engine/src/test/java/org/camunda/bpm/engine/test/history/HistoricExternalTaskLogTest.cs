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
	using org.camunda.bpm.engine;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.DEFAULT_TOPIC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.createDefaultExternalTaskModel;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricExternalTaskLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricExternalTaskLogTest()
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
	  protected internal readonly string ERROR_MESSAGE = "This is an error!";
	  protected internal readonly string ERROR_DETAILS = "These are the error details!";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;
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
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		IList<ExternalTask> list = externalTaskService.createExternalTaskQuery().workerId(WORKER_ID).list();
		foreach (ExternalTask externalTask in list)
		{
		  externalTaskService.unlock(externalTask.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskLogCreateProperties()
	  public virtual void testHistoricExternalTaskLogCreateProperties()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().creationLog().singleResult();

		// then
		assertHistoricLogPropertiesAreProperlySet(task, log);
		assertEquals(null, log.WorkerId);
		assertLogIsInCreatedState(log);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskLogFailedProperties()
	  public virtual void testHistoricExternalTaskLogFailedProperties()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		reportExternalTaskFailure(task.Id);
		task = externalTaskService.createExternalTaskQuery().singleResult();

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		// then
		assertHistoricLogPropertiesAreProperlySet(task, log);
		assertEquals(WORKER_ID, log.WorkerId);
		assertLogIsInFailedState(log);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskLogSuccessfulProperties()
	  public virtual void testHistoricExternalTaskLogSuccessfulProperties()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		completeExternalTask(task.Id);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().successLog().singleResult();

		// then
		assertHistoricLogPropertiesAreProperlySet(task, log);
		assertEquals(WORKER_ID, log.WorkerId);
		assertLogIsInSuccessfulState(log);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskLogDeletedProperties()
	  public virtual void testHistoricExternalTaskLogDeletedProperties()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		runtimeService.deleteProcessInstance(task.ProcessInstanceId, "Dummy reason for deletion!");

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().deletionLog().singleResult();

		// then
		assertHistoricLogPropertiesAreProperlySet(task, log);
		assertEquals(null, log.WorkerId);
		assertLogIsInDeletedState(log);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetriesAndWorkerIdWhenFirstFailureAndThenComplete()
	  public virtual void testRetriesAndWorkerIdWhenFirstFailureAndThenComplete()
	  {

		// given
		ExternalTask task = startExternalTaskProcess();
		reportExternalTaskFailure(task.Id);
		completeExternalTask(task.Id);

		// when
		HistoricExternalTaskLog log = historyService.createHistoricExternalTaskLogQuery().successLog().singleResult();

		// then
		assertEquals(WORKER_ID, log.WorkerId);
		assertEquals(Convert.ToInt32(1), log.Retries);
		assertLogIsInSuccessfulState(log);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDetails()
	  public virtual void testErrorDetails()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		reportExternalTaskFailure(task.Id);

		// when
		string failedHistoricExternalTaskLogId = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult().Id;

		// then
		string stacktrace = historyService.getHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
		assertNotNull(stacktrace);
		assertEquals(ERROR_DETAILS, stacktrace);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorDetailsWithTwoDifferentErrorsThrown()
	  public virtual void testErrorDetailsWithTwoDifferentErrorsThrown()
	  {
		// given
		ExternalTask task = startExternalTaskProcess();
		string firstErrorDetails = "Dummy error details!";
		string secondErrorDetails = ERROR_DETAILS;
		reportExternalTaskFailure(task.Id, ERROR_MESSAGE, firstErrorDetails);
		ensureEnoughTimePassedByForTimestampOrdering();
		reportExternalTaskFailure(task.Id, ERROR_MESSAGE, secondErrorDetails);

		// when
		IList<HistoricExternalTaskLog> list = historyService.createHistoricExternalTaskLogQuery().failureLog().orderByTimestamp().asc().list();

		string firstFailedLogId = list[0].Id;
		string secondFailedLogId = list[1].Id;

		// then
		string stacktrace1 = historyService.getHistoricExternalTaskLogErrorDetails(firstFailedLogId);
		string stacktrace2 = historyService.getHistoricExternalTaskLogErrorDetails(secondFailedLogId);
		assertNotNull(stacktrace1);
		assertNotNull(stacktrace2);
		assertEquals(firstErrorDetails, stacktrace1);
		assertEquals(secondErrorDetails, stacktrace2);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetExceptionStacktraceForNonexistentExternalTaskId()
	  public virtual void testGetExceptionStacktraceForNonexistentExternalTaskId()
	  {
		try
		{
		  historyService.getHistoricExternalTaskLogErrorDetails("foo");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  string expectedMessage = "No historic external task log found with id foo";
		  assertTrue(re.Message.contains(expectedMessage));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetExceptionStacktraceForNullExternalTaskId()
	  public virtual void testGetExceptionStacktraceForNullExternalTaskId()
	  {
		try
		{
		  historyService.getHistoricExternalTaskLogErrorDetails(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  string expectedMessage = "historicExternalTaskLogId is null";
		  assertTrue(re.Message.contains(expectedMessage));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testErrorMessageTruncation()
	  public virtual void testErrorMessageTruncation()
	  {
		// given
		string exceptionMessage = createStringOfLength(1000);
		ExternalTask task = startExternalTaskProcess();
		reportExternalTaskFailure(task.Id, exceptionMessage, ERROR_DETAILS);

		// when
		HistoricExternalTaskLog failedLog = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		string errorMessage = failedLog.ErrorMessage;
		string expectedErrorMessage = exceptionMessage.Substring(0, ExternalTaskEntity.MAX_EXCEPTION_MESSAGE_LENGTH);

		// then
		assertNotNull(failedLog);
		assertEquals(ExternalTaskEntity.MAX_EXCEPTION_MESSAGE_LENGTH, errorMessage.Length);
		assertEquals(expectedErrorMessage, errorMessage);

	  }

	  // helper

	  protected internal virtual void assertLogIsInCreatedState(HistoricExternalTaskLog log)
	  {
		assertTrue(log.CreationLog);
		assertFalse(log.FailureLog);
		assertFalse(log.SuccessLog);
		assertFalse(log.DeletionLog);
	  }

	  protected internal virtual void assertLogIsInFailedState(HistoricExternalTaskLog log)
	  {
		assertFalse(log.CreationLog);
		assertTrue(log.FailureLog);
		assertFalse(log.SuccessLog);
		assertFalse(log.DeletionLog);
	  }

	  protected internal virtual void assertLogIsInSuccessfulState(HistoricExternalTaskLog log)
	  {
		assertFalse(log.CreationLog);
		assertFalse(log.FailureLog);
		assertTrue(log.SuccessLog);
		assertFalse(log.DeletionLog);
	  }

	  protected internal virtual void assertLogIsInDeletedState(HistoricExternalTaskLog log)
	  {
		assertFalse(log.CreationLog);
		assertFalse(log.FailureLog);
		assertFalse(log.SuccessLog);
		assertTrue(log.DeletionLog);
	  }

	  protected internal virtual void assertHistoricLogPropertiesAreProperlySet(ExternalTask task, HistoricExternalTaskLog log)
	  {
		assertNotNull(log);
		assertNotNull(log.Id);
		assertNotNull(log.Timestamp);

		assertEquals(task.Id, log.ExternalTaskId);
		assertEquals(task.ActivityId, log.ActivityId);
		assertEquals(task.ActivityInstanceId, log.ActivityInstanceId);
		assertEquals(task.TopicName, log.TopicName);
		assertEquals(task.Retries, log.Retries);
		assertEquals(task.ExecutionId, log.ExecutionId);
		assertEquals(task.ProcessInstanceId, log.ProcessInstanceId);
		assertEquals(task.ProcessDefinitionId, log.ProcessDefinitionId);
		assertEquals(task.ProcessDefinitionKey, log.ProcessDefinitionKey);
		assertEquals(task.Priority, log.Priority);
	  }

	  protected internal virtual void completeExternalTask(string externalTaskId)
	  {
		externalTaskService.fetchAndLock(100, WORKER_ID, false).topic(DEFAULT_TOPIC, LOCK_DURATION).execute();
		externalTaskService.complete(externalTaskId, WORKER_ID);
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId)
	  {
		reportExternalTaskFailure(externalTaskId, ERROR_MESSAGE, ERROR_DETAILS);
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string errorMessage, string errorDetails)
	  {
		externalTaskService.fetchAndLock(100, WORKER_ID, false).topic(DEFAULT_TOPIC, LOCK_DURATION).execute();
		externalTaskService.handleFailure(externalTaskId, WORKER_ID, errorMessage, errorDetails, 1, 0L);
	  }

	  protected internal virtual ExternalTask startExternalTaskProcess()
	  {
		BpmnModelInstance oneExternalTaskProcess = createDefaultExternalTaskModel().build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(oneExternalTaskProcess);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual string createStringOfLength(int count)
	  {
		return repeatString(count, "a");
	  }

	  protected internal virtual string repeatString(int count, string with)
	  {
		return (new string(new char[count])).Replace("\0", with);
	  }

	  protected internal virtual void ensureEnoughTimePassedByForTimestampOrdering()
	  {
		long timeToAddInSeconds = 5 * 1000L;
		DateTime nowPlus5Seconds = new DateTime(ClockUtil.CurrentTime.Ticks + timeToAddInSeconds);
		ClockUtil.CurrentTime = nowPlus5Seconds;
	  }

	}

}