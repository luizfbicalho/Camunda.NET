using System;

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
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using ProcessEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.ProcessEventJobHandler;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricJobLogTest : PluggableProcessEngineTestCase
	{

	  private bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void storeEngineSettings()
	  public virtual void storeEngineSettings()
	  {
		defaultEnsureJobDueDateSet = processEngineConfiguration.EnsureJobDueDateNotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
		ClockUtil.reset();
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testCreateHistoricJobLogProperties()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().creationLog().singleResult();
		assertNotNull(historicJob);

		assertNotNull(historicJob.Timestamp);

		assertNull(historicJob.JobExceptionMessage);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
		assertEquals(job.Retries, historicJob.JobRetries);
		assertEquals(job.ExecutionId, historicJob.ExecutionId);
		assertEquals(job.ProcessInstanceId, historicJob.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
		assertEquals(job.DeploymentId, historicJob.DeploymentId);
		assertEquals(job.Priority, historicJob.JobPriority);

		assertTrue(historicJob.CreationLog);
		assertFalse(historicJob.FailureLog);
		assertFalse(historicJob.SuccessLog);
		assertFalse(historicJob.DeletionLog);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testFailedHistoricJobLogProperties()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		try
		{
		  managementService.executeJob(job.Id);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().failureLog().singleResult();
		assertNotNull(historicJob);

		assertNotNull(historicJob.Timestamp);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
		assertEquals(3, historicJob.JobRetries);
		assertEquals(job.ExecutionId, historicJob.ExecutionId);
		assertEquals(job.ProcessInstanceId, historicJob.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
		assertEquals(job.DeploymentId, historicJob.DeploymentId);
		assertEquals(FailingDelegate.EXCEPTION_MESSAGE, historicJob.JobExceptionMessage);
		assertEquals(job.Priority, historicJob.JobPriority);

		assertFalse(historicJob.CreationLog);
		assertTrue(historicJob.FailureLog);
		assertFalse(historicJob.SuccessLog);
		assertFalse(historicJob.DeletionLog);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testSuccessfulHistoricJobLogProperties()
	  {
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		Job job = managementService.createJobQuery().singleResult();

		managementService.executeJob(job.Id);

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().successLog().singleResult();
		assertNotNull(historicJob);

		assertNotNull(historicJob.Timestamp);

		assertNull(historicJob.JobExceptionMessage);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
		assertEquals(job.Retries, historicJob.JobRetries);
		assertEquals(job.ExecutionId, historicJob.ExecutionId);
		assertEquals(job.ProcessInstanceId, historicJob.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
		assertEquals(job.DeploymentId, historicJob.DeploymentId);
		assertEquals(job.Priority, historicJob.JobPriority);

		assertFalse(historicJob.CreationLog);
		assertFalse(historicJob.FailureLog);
		assertTrue(historicJob.SuccessLog);
		assertFalse(historicJob.DeletionLog);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testDeletedHistoricJobLogProperties()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		Job job = managementService.createJobQuery().singleResult();

		runtimeService.deleteProcessInstance(processInstanceId, null);

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().deletionLog().singleResult();
		assertNotNull(historicJob);

		assertNotNull(historicJob.Timestamp);

		assertNull(historicJob.JobExceptionMessage);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
		assertEquals(job.Retries, historicJob.JobRetries);
		assertEquals(job.ExecutionId, historicJob.ExecutionId);
		assertEquals(job.ProcessInstanceId, historicJob.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
		assertEquals(job.DeploymentId, historicJob.DeploymentId);
		assertEquals(job.Priority, historicJob.JobPriority);

		assertFalse(historicJob.CreationLog);
		assertFalse(historicJob.FailureLog);
		assertFalse(historicJob.SuccessLog);
		assertTrue(historicJob.DeletionLog);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testAsyncBeforeJobHandlerType()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = false;

		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertNull(historicJob.JobDueDate);

		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testAsyncBeforeJobHandlerTypeDueDateSet()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = true;
		DateTime testDate = ClockTestUtil.setClockToDateWithoutMilliseconds();

		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(testDate, historicJob.JobDueDate);

		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testAsyncAfterJobHandlerType()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = false;

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		Job job = managementService.createJobQuery().singleResult();

		managementService.executeJob(job.Id);

		Job anotherJob = managementService.createJobQuery().singleResult();

		assertFalse(job.Id.Equals(anotherJob.Id));

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(anotherJob.Id).singleResult();

		assertNotNull(historicJob);

		assertNull(historicJob.JobDueDate);

		assertEquals(anotherJob.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_AFTER, historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testAsyncAfterJobHandlerTypeDueDateSet()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = true;
		DateTime testDate = ClockTestUtil.setClockToDateWithoutMilliseconds();

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		Job job = managementService.createJobQuery().singleResult();

		managementService.executeJob(job.Id);

		Job anotherJob = managementService.createJobQuery().singleResult();

		assertFalse(job.Id.Equals(anotherJob.Id));

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(anotherJob.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(testDate, historicJob.JobDueDate);

		assertEquals(anotherJob.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("serviceTask", historicJob.ActivityId);
		assertEquals(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals(MessageJobDeclaration.ASYNC_AFTER, historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuationWithLongId.bpmn20.xml"})]
	  public virtual void testSuccessfulHistoricJobLogEntryStoredForLongActivityId()
	  {
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		Job job = managementService.createJobQuery().singleResult();

		managementService.executeJob(job.Id);

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().successLog().singleResult();
		assertNotNull(historicJob);
		assertEquals("serviceTaskIdIsReallyLongAndItShouldBeMoreThan64CharsSoItWill" + "BlowAnyActivityIdColumnWhereSizeIs64OrLessSoWeAlignItTo255LikeEverywhereElse", historicJob.ActivityId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testStartTimerEvent.bpmn20.xml"})]
	  public virtual void testStartTimerEventJobHandlerType()
	  {
		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("theStart", historicJob.ActivityId);
		assertEquals(TimerStartEventJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals("CYCLE: 0 0/5 * * * ?", historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testStartTimerEventInsideEventSubProcess.bpmn20.xml"})]
	  public virtual void testStartTimerEventInsideEventSubProcessJobHandlerType()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("subprocessStartEvent", historicJob.ActivityId);
		assertEquals(TimerStartEventSubprocessJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals("DURATION: PT1M", historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testIntermediateTimerEvent.bpmn20.xml"})]
	  public virtual void testIntermediateTimerEventJobHandlerType()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("timer", historicJob.ActivityId);
		assertEquals(TimerCatchIntermediateEventJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals("DURATION: PT1M", historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testBoundaryTimerEvent.bpmn20.xml"})]
	  public virtual void testBoundaryTimerEventJobHandlerType()
	  {
		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("timer", historicJob.ActivityId);
		assertEquals(TimerExecuteNestedActivityJobHandler.TYPE, historicJob.JobDefinitionType);
		assertEquals("DURATION: PT5M", historicJob.JobDefinitionConfiguration);
		assertEquals(job.Duedate, historicJob.JobDueDate);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testCatchingSignalEvent.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testThrowingSignalEventAsync.bpmn20.xml" })]
	  public virtual void testCatchingSignalEventJobHandlerType()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = false;

		runtimeService.startProcessInstanceByKey("catchSignal");
		runtimeService.startProcessInstanceByKey("throwSignal");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertNull(historicJob.JobDueDate);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("signalEvent", historicJob.ActivityId);
		assertEquals(ProcessEventJobHandler.TYPE, historicJob.JobDefinitionType);
		assertNull(historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testCatchingSignalEvent.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testThrowingSignalEventAsync.bpmn20.xml" })]
	  public virtual void testCatchingSignalEventJobHandlerTypeDueDateSet()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = true;
		DateTime testDate = ClockTestUtil.setClockToDateWithoutMilliseconds();

		runtimeService.startProcessInstanceByKey("catchSignal");
		runtimeService.startProcessInstanceByKey("throwSignal");

		Job job = managementService.createJobQuery().singleResult();

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(job.Id).singleResult();

		assertNotNull(historicJob);

		assertEquals(testDate, historicJob.JobDueDate);

		assertEquals(job.Id, historicJob.JobId);
		assertEquals(job.JobDefinitionId, historicJob.JobDefinitionId);
		assertEquals("signalEvent", historicJob.ActivityId);
		assertEquals(ProcessEventJobHandler.TYPE, historicJob.JobDefinitionType);
		assertNull(historicJob.JobDefinitionConfiguration);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testCatchingSignalEvent.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testThrowingSignalEventAsync.bpmn20.xml" })]
	  public virtual void testCatchingSignalEventActivityId()
	  {
		// given + when (1)
		string processInstanceId = runtimeService.startProcessInstanceByKey("catchSignal").Id;
		runtimeService.startProcessInstanceByKey("throwSignal");

		string jobId = managementService.createJobQuery().singleResult().Id;

		// then (1)

		HistoricJobLog historicJob = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog().singleResult();
		assertNotNull(historicJob);

		assertEquals("signalEvent", historicJob.ActivityId);

		// when (2)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		historicJob = historyService.createHistoricJobLogQuery().jobId(jobId).failureLog().singleResult();
		assertNotNull(historicJob);

		assertEquals("signalEvent", historicJob.ActivityId);

		// when (3)
		runtimeService.setVariable(processInstanceId, "fail", false);
		managementService.executeJob(jobId);

		// then (3)

		historicJob = historyService.createHistoricJobLogQuery().jobId(jobId).successLog().singleResult();
		assertNotNull(historicJob);

		assertEquals("signalEvent", historicJob.ActivityId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testFailedJobEvents()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery failedQuery = historyService.createHistoricJobLogQuery().jobId(jobId).failureLog().orderByJobRetries().desc();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, failedQuery.count());

		// when (1)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (1)
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, failedQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog failedJobLogEntry = failedQuery.singleResult();
		assertEquals(3, failedJobLogEntry.JobRetries);

		// when (2)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		assertEquals(3, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(2, failedQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		// when (3)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (3)
		assertEquals(4, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(3, failedQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(2);
		assertEquals(1, failedJobLogEntry.JobRetries);

		// when (4)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (4)
		assertEquals(5, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(4, failedQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(2);
		assertEquals(1, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(3);
		assertEquals(0, failedJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testFailedJobEventsExecutedByJobExecutor()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery failedQuery = historyService.createHistoricJobLogQuery().jobId(jobId).failureLog().orderByJobRetries().desc();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, failedQuery.count());

		// when (1)
		executeAvailableJobs();

		// then (1)
		assertEquals(4, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(3, failedQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(2);
		assertEquals(1, failedJobLogEntry.JobRetries);

		// when (2)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		assertEquals(5, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(4, failedQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(2);
		assertEquals(1, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(3);
		assertEquals(0, failedJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testSuccessfulJobEvent()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery succeededQuery = historyService.createHistoricJobLogQuery().jobId(jobId).successLog();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, succeededQuery.count());

		// when
		managementService.executeJob(jobId);

		// then
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, succeededQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog succeededJobLogEntry = succeededQuery.singleResult();
		assertEquals(3, succeededJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testSuccessfulJobEventExecutedByJobExecutor()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("fail", false));

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery succeededQuery = historyService.createHistoricJobLogQuery().jobId(jobId).successLog();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, succeededQuery.count());

		// when
		executeAvailableJobs();

		// then
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, succeededQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog succeededJobLogEntry = succeededQuery.singleResult();
		assertEquals(3, succeededJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testSuccessfulAndFailedJobEvents()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery failedQuery = historyService.createHistoricJobLogQuery().jobId(jobId).failureLog().orderByJobRetries().desc();
		HistoricJobLogQuery succeededQuery = historyService.createHistoricJobLogQuery().jobId(jobId).successLog();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, failedQuery.count());
		assertEquals(0, succeededQuery.count());

		// when (1)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (1)
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, failedQuery.count());
		assertEquals(0, succeededQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog failedJobLogEntry = failedQuery.singleResult();
		assertEquals(3, failedJobLogEntry.JobRetries);

		// when (2)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		assertEquals(3, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(2, failedQuery.count());
		assertEquals(0, succeededQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		// when (3)
		runtimeService.setVariable(processInstanceId, "fail", false);
		managementService.executeJob(jobId);

		// then (3)
		assertEquals(4, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(2, failedQuery.count());
		assertEquals(1, succeededQuery.count());

		createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(0);
		assertEquals(3, failedJobLogEntry.JobRetries);

		failedJobLogEntry = failedQuery.list().get(1);
		assertEquals(2, failedJobLogEntry.JobRetries);

		HistoricJobLog succeededJobLogEntry = succeededQuery.singleResult();
		assertEquals(1, succeededJobLogEntry.JobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateEndEvent()
	  public virtual void testTerminateEndEvent()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process").Id;

		string serviceTask1JobId = managementService.createJobQuery().activityId("serviceTask1").singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();
		assertEquals(2, query.count());

		// serviceTask1
		HistoricJobLogQuery serviceTask1Query = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId);
		HistoricJobLogQuery serviceTask1CreatedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).creationLog();
		HistoricJobLogQuery serviceTask1DeletedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).deletionLog();
		HistoricJobLogQuery serviceTask1SuccessfulQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).successLog();

		assertEquals(1, serviceTask1Query.count());
		assertEquals(1, serviceTask1CreatedQuery.count());
		assertEquals(0, serviceTask1DeletedQuery.count());
		assertEquals(0, serviceTask1SuccessfulQuery.count());

		// serviceTask2
		string serviceTask2JobId = managementService.createJobQuery().activityId("serviceTask2").singleResult().Id;

		HistoricJobLogQuery serviceTask2Query = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId);
		HistoricJobLogQuery serviceTask2CreatedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).creationLog();
		HistoricJobLogQuery serviceTask2DeletedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).deletionLog();
		HistoricJobLogQuery serviceTask2SuccessfulQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).successLog();

		assertEquals(1, serviceTask2Query.count());
		assertEquals(1, serviceTask2CreatedQuery.count());
		assertEquals(0, serviceTask2DeletedQuery.count());
		assertEquals(0, serviceTask2SuccessfulQuery.count());

		// when
		managementService.executeJob(serviceTask1JobId);

		// then
		assertEquals(4, query.count());

		// serviceTas1
		assertEquals(2, serviceTask1Query.count());
		assertEquals(1, serviceTask1CreatedQuery.count());
		assertEquals(0, serviceTask1DeletedQuery.count());
		assertEquals(1, serviceTask1SuccessfulQuery.count());

		HistoricJobLog serviceTask1CreatedJobLogEntry = serviceTask1CreatedQuery.singleResult();
		assertEquals(3, serviceTask1CreatedJobLogEntry.JobRetries);

		HistoricJobLog serviceTask1SuccessfulJobLogEntry = serviceTask1SuccessfulQuery.singleResult();
		assertEquals(3, serviceTask1SuccessfulJobLogEntry.JobRetries);

		// serviceTask2
		assertEquals(2, serviceTask2Query.count());
		assertEquals(1, serviceTask2CreatedQuery.count());
		assertEquals(1, serviceTask2DeletedQuery.count());
		assertEquals(0, serviceTask2SuccessfulQuery.count());

		HistoricJobLog serviceTask2CreatedJobLogEntry = serviceTask2CreatedQuery.singleResult();
		assertEquals(3, serviceTask2CreatedJobLogEntry.JobRetries);

		HistoricJobLog serviceTask2DeletedJobLogEntry = serviceTask2DeletedQuery.singleResult();
		assertEquals(3, serviceTask2DeletedJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testSuperProcessWithCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/history/HistoricJobLogTest.testSubProcessWithErrorEndEvent.bpmn20.xml" })]
	  public virtual void testErrorEndEventInterruptingCallActivity()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process").Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();
		assertEquals(2, query.count());

		// serviceTask1
		string serviceTask1JobId = managementService.createJobQuery().activityId("serviceTask1").singleResult().Id;

		HistoricJobLogQuery serviceTask1Query = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId);
		HistoricJobLogQuery serviceTask1CreatedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).creationLog();
		HistoricJobLogQuery serviceTask1DeletedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).deletionLog();
		HistoricJobLogQuery serviceTask1SuccessfulQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask1JobId).successLog();

		assertEquals(1, serviceTask1Query.count());
		assertEquals(1, serviceTask1CreatedQuery.count());
		assertEquals(0, serviceTask1DeletedQuery.count());
		assertEquals(0, serviceTask1SuccessfulQuery.count());

		// serviceTask2
		string serviceTask2JobId = managementService.createJobQuery().activityId("serviceTask2").singleResult().Id;

		HistoricJobLogQuery serviceTask2Query = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId);
		HistoricJobLogQuery serviceTask2CreatedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).creationLog();
		HistoricJobLogQuery serviceTask2DeletedQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).deletionLog();
		HistoricJobLogQuery serviceTask2SuccessfulQuery = historyService.createHistoricJobLogQuery().jobId(serviceTask2JobId).successLog();

		assertEquals(1, serviceTask2Query.count());
		assertEquals(1, serviceTask2CreatedQuery.count());
		assertEquals(0, serviceTask2DeletedQuery.count());
		assertEquals(0, serviceTask2SuccessfulQuery.count());

		// when
		managementService.executeJob(serviceTask1JobId);

		// then
		assertEquals(4, query.count());

		// serviceTask1
		assertEquals(2, serviceTask1Query.count());
		assertEquals(1, serviceTask1CreatedQuery.count());
		assertEquals(0, serviceTask1DeletedQuery.count());
		assertEquals(1, serviceTask1SuccessfulQuery.count());

		HistoricJobLog serviceTask1CreatedJobLogEntry = serviceTask1CreatedQuery.singleResult();
		assertEquals(3, serviceTask1CreatedJobLogEntry.JobRetries);

		HistoricJobLog serviceTask1SuccessfulJobLogEntry = serviceTask1SuccessfulQuery.singleResult();
		assertEquals(3, serviceTask1SuccessfulJobLogEntry.JobRetries);

		// serviceTask2
		assertEquals(2, serviceTask2Query.count());
		assertEquals(1, serviceTask2CreatedQuery.count());
		assertEquals(1, serviceTask2DeletedQuery.count());
		assertEquals(0, serviceTask2SuccessfulQuery.count());

		HistoricJobLog serviceTask2CreatedJobLogEntry = serviceTask2CreatedQuery.singleResult();
		assertEquals(3, serviceTask2CreatedJobLogEntry.JobRetries);

		HistoricJobLog serviceTask2DeletedJobLogEntry = serviceTask2DeletedQuery.singleResult();
		assertEquals(3, serviceTask2DeletedJobLogEntry.JobRetries);

		// there should be one task after the boundary event
		assertEquals(1, taskService.createTaskQuery().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testDeletedJob()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery deletedQuery = historyService.createHistoricJobLogQuery().jobId(jobId).deletionLog();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, deletedQuery.count());

		// when
		managementService.deleteJob(jobId);

		// then
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, deletedQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog deletedJobLogEntry = deletedQuery.singleResult();
		assertEquals(3, deletedJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testDeletedProcessInstance()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string jobId = managementService.createJobQuery().singleResult().Id;

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery().jobId(jobId);
		HistoricJobLogQuery createdQuery = historyService.createHistoricJobLogQuery().jobId(jobId).creationLog();
		HistoricJobLogQuery deletedQuery = historyService.createHistoricJobLogQuery().jobId(jobId).deletionLog();

		// there exists one historic job log entry
		assertEquals(1, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(0, deletedQuery.count());

		// when
		runtimeService.deleteProcessInstance(processInstanceId, null);

		// then
		assertEquals(2, query.count());
		assertEquals(1, createdQuery.count());
		assertEquals(1, deletedQuery.count());

		HistoricJobLog createdJobLogEntry = createdQuery.singleResult();
		assertEquals(3, createdJobLogEntry.JobRetries);

		HistoricJobLog deletedJobLogEntry = deletedQuery.singleResult();
		assertEquals(3, deletedJobLogEntry.JobRetries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testExceptionStacktrace()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process");

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then
		string failedHistoricJobLogId = historyService.createHistoricJobLogQuery().failureLog().singleResult().Id;

		string stacktrace = historyService.getHistoricJobLogExceptionStacktrace(failedHistoricJobLogId);
		assertNotNull(stacktrace);
		assertTextPresent(FailingDelegate.EXCEPTION_MESSAGE, stacktrace);
	  }

	  public virtual void testgetJobExceptionStacktraceUnexistingJobId()
	  {
		try
		{
		  historyService.getHistoricJobLogExceptionStacktrace("unexistingjob");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("No historic job log found with id unexistingjob", re.Message);
		}
	  }

	  public virtual void testgetJobExceptionStacktraceNullJobId()
	  {
		try
		{
		  historyService.getHistoricJobLogExceptionStacktrace(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("historicJobLogId is null", re.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDifferentExceptions()
	  public virtual void testDifferentExceptions()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when (1)
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (1)
		HistoricJobLog serviceTask1FailedHistoricJobLog = historyService.createHistoricJobLogQuery().failureLog().singleResult();

		string serviceTask1FailedHistoricJobLogId = serviceTask1FailedHistoricJobLog.Id;

		assertEquals(FirstFailingDelegate.FIRST_EXCEPTION_MESSAGE, serviceTask1FailedHistoricJobLog.JobExceptionMessage);

		string serviceTask1Stacktrace = historyService.getHistoricJobLogExceptionStacktrace(serviceTask1FailedHistoricJobLogId);
		assertNotNull(serviceTask1Stacktrace);
		assertTextPresent(FirstFailingDelegate.FIRST_EXCEPTION_MESSAGE, serviceTask1Stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertTextPresent(typeof(FirstFailingDelegate).FullName, serviceTask1Stacktrace);

		// when (2)
		runtimeService.setVariable(processInstanceId, "firstFail", false);
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then (2)
		HistoricJobLog serviceTask2FailedHistoricJobLog = historyService.createHistoricJobLogQuery().failureLog().orderByJobRetries().desc().list().get(1);

		string serviceTask2FailedHistoricJobLogId = serviceTask2FailedHistoricJobLog.Id;

		assertEquals(SecondFailingDelegate.SECOND_EXCEPTION_MESSAGE, serviceTask2FailedHistoricJobLog.JobExceptionMessage);

		string serviceTask2Stacktrace = historyService.getHistoricJobLogExceptionStacktrace(serviceTask2FailedHistoricJobLogId);
		assertNotNull(serviceTask2Stacktrace);
		assertTextPresent(SecondFailingDelegate.SECOND_EXCEPTION_MESSAGE, serviceTask2Stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertTextPresent(typeof(SecondFailingDelegate).FullName, serviceTask2Stacktrace);

		assertFalse(serviceTask1Stacktrace.Equals(serviceTask2Stacktrace));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrowExceptionWithoutMessage()
	  public virtual void testThrowExceptionWithoutMessage()
	  {
		// given
		runtimeService.startProcessInstanceByKey("process").Id;

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then
		HistoricJobLog failedHistoricJobLog = historyService.createHistoricJobLogQuery().failureLog().singleResult();

		string failedHistoricJobLogId = failedHistoricJobLog.Id;

		assertNull(failedHistoricJobLog.JobExceptionMessage);

		string stacktrace = historyService.getHistoricJobLogExceptionStacktrace(failedHistoricJobLogId);
		assertNotNull(stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertTextPresent(typeof(ThrowExceptionWithoutMessageDelegate).FullName, stacktrace);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrowExceptionMessageTruncation()
	  public virtual void testThrowExceptionMessageTruncation()
	  {
		// given
		string exceptionMessage = randomString(10000);
		ThrowExceptionWithOverlongMessageDelegate @delegate = new ThrowExceptionWithOverlongMessageDelegate(exceptionMessage);

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("delegate", @delegate));
		Job job = managementService.createJobQuery().singleResult();

		// when
		try
		{
		  managementService.executeJob(job.Id);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		// then
		HistoricJobLog failedHistoricJobLog = historyService.createHistoricJobLogQuery().failureLog().singleResult();

		assertNotNull(failedHistoricJobLog);
		assertEquals(exceptionMessage.Substring(0, StringUtil.DB_MAX_STRING_LENGTH), failedHistoricJobLog.JobExceptionMessage);
	  }

	  /// <summary>
	  /// returns a random of the given size using characters [0-1]
	  /// </summary>
	  protected internal static string randomString(int numCharacters)
	  {
		return (new System.Numerics.BigInteger(numCharacters, new Random())).ToString(2);
	  }


	  public virtual void testDeleteByteArray()
	  {
		const string processDefinitionId = "myProcessDefition";

		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass(this, processDefinitionId));

		assertEquals(1234, historyService.createHistoricJobLogQuery().count());

		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass2(this, processDefinitionId));

		assertEquals(0, historyService.createHistoricJobLogQuery().count());
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoricJobLogTest outerInstance;

		  private string processDefinitionId;

		  public CommandAnonymousInnerClass(HistoricJobLogTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }


		  public Void execute(CommandContext commandContext)
		  {

			for (int i = 0; i < 1234; i++)
			{
			  HistoricJobLogEventEntity log = new HistoricJobLogEventEntity();
			  log.JobId = i.ToString();
			  log.Timestamp = DateTime.Now;
			  log.JobDefinitionType = MessageEntity.TYPE;
			  log.ProcessDefinitionId = processDefinitionId;


			  sbyte[] aByteValue = StringUtil.toByteArray("abc");
			  ByteArrayEntity byteArray = ExceptionUtil.createJobExceptionByteArray(aByteValue, ResourceTypes.HISTORY);
			  log.ExceptionByteArrayId = byteArray.Id;

			  commandContext.HistoricJobLogManager.insert(log);
			}

			return null;
		  }

	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly HistoricJobLogTest outerInstance;

		  private string processDefinitionId;

		  public CommandAnonymousInnerClass2(HistoricJobLogTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByProcessDefinitionId(processDefinitionId);
			return null;
		  }

	  }

	}

}