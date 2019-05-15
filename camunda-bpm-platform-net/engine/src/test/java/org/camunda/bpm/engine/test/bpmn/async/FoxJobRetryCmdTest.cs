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
namespace org.camunda.bpm.engine.test.bpmn.async
{

	using Page = org.camunda.bpm.engine.impl.Page;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;
	using Assert = org.junit.Assert;

	public class FoxJobRetryCmdTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" })]
	  public virtual void testFailedServiceTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedServiceTask");

		assertJobRetriesForActivity(pi, "failingServiceTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedUserTask.bpmn20.xml" })]
	  public virtual void testFailedUserTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedUserTask");

		assertJobRetriesForActivity(pi, "failingUserTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedBusinessRuleTask.bpmn20.xml" })]
	  public virtual void testFailedBusinessRuleTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedBusinessRuleTask");

		assertJobRetriesForActivity(pi, "failingBusinessRuleTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedCallActivity.bpmn20.xml" })]
	  public virtual void testFailedCallActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedCallActivity");

		assertJobRetriesForActivity(pi, "failingCallActivity");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedScriptTask.bpmn20.xml" })]
	  public virtual void testFailedScriptTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedScriptTask");

		assertJobRetriesForActivity(pi, "failingScriptTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedSendTask.bpmn20.xml" })]
	  public virtual void testFailedSendTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedSendTask");

		assertJobRetriesForActivity(pi, "failingSendTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedSubProcess.bpmn20.xml" })]
	  public virtual void testFailedSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedSubProcess");

		assertJobRetriesForActivity(pi, "failingSubProcess");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedTask.bpmn20.xml" })]
	  public virtual void testFailedTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedTask");

		assertJobRetriesForActivity(pi, "failingTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedTransaction.bpmn20.xml" })]
	  public virtual void testFailedTransaction()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedTask");

		assertJobRetriesForActivity(pi, "failingTransaction");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedReceiveTask.bpmn20.xml" })]
	  public virtual void testFailedReceiveTask()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedReceiveTask");

		assertJobRetriesForActivity(pi, "failingReceiveTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedBoundaryTimerEvent.bpmn20.xml" })]
	  public virtual void testFailedBoundaryTimerEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedBoundaryTimerEvent");

		assertJobRetriesForActivity(pi, "userTask");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedIntermediateCatchingTimerEvent.bpmn20.xml" })]
	  public virtual void testFailedIntermediateCatchingTimerEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedIntermediateCatchingTimerEvent");

		assertJobRetriesForActivity(pi, "failingTimerEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingMultiInstanceBody()
	  public virtual void testFailingMultiInstanceBody()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failingMultiInstance");

		// multi-instance body of task
		assertJobRetriesForActivity(pi, "task" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFailingMultiInstanceInnerActivity()
	  public virtual void testFailingMultiInstanceInnerActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failingMultiInstance");

		// inner activity of multi-instance body
		assertJobRetriesForActivity(pi, "task");
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testBrokenFoxJobRetryValue.bpmn20.xml" })]
	  public virtual void testBrokenFoxJobRetryValue()
	  {
		Job job = managementService.createJobQuery().list().get(0);
		assertNotNull(job);
		assertEquals(3, job.Retries);

		waitForExecutedJobWithRetriesLeft(0, job.Id);
		job = refreshJob(job.Id);
		assertEquals(0, job.Retries);
		assertEquals(1, managementService.createJobQuery().noRetriesLeft().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedStartTimerEvent.bpmn20.xml" })]
	  public virtual void testFailedTimerStartEvent()
	  {
		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		Job job = managementService.createJobQuery().list().get(0);
		assertNotNull(job);
		string jobId = job.Id;

		waitForExecutedJobWithRetriesLeft(4, jobId);
		stillOneJobWithExceptionAndRetriesLeft(jobId);

		job = refreshJob(jobId);
		assertNotNull(job);

		assertEquals(4, job.Retries);

		waitForExecutedJobWithRetriesLeft(3, jobId);

		job = refreshJob(jobId);
		assertEquals(3, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft(jobId);

		waitForExecutedJobWithRetriesLeft(2, jobId);

		job = refreshJob(jobId);
		assertEquals(2, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft(jobId);

		waitForExecutedJobWithRetriesLeft(1, jobId);

		job = refreshJob(jobId);
		assertEquals(1, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft(jobId);

		waitForExecutedJobWithRetriesLeft(0, jobId);

		job = refreshJob(jobId);
		assertEquals(0, job.Retries);
		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(0, managementService.createJobQuery().jobId(jobId).withRetriesLeft().count());
		assertEquals(1, managementService.createJobQuery().noRetriesLeft().count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedIntermediateThrowingSignalEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.failingSignalStart.bpmn20.xml" })]
	  public virtual void FAILING_testFailedIntermediateThrowingSignalEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedIntermediateThrowingSignalEvent");

		assertJobRetriesForActivity(pi, "failingSignalEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRetryOnTimerStartEventInEventSubProcess()
	  public virtual void testRetryOnTimerStartEventInEventSubProcess()
	  {
		runtimeService.startProcessInstanceByKey("process").Id;

		Job job = managementService.createJobQuery().singleResult();

		assertEquals(3, job.Retries);

		try
		{
		  managementService.executeJob(job.Id);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		job = managementService.createJobQuery().singleResult();

		assertEquals(4, job.Retries);
	  }

	  public virtual void testRetryOnServiceTaskLikeMessageThrowEvent()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().intermediateThrowEvent().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R10/PT5S").messageEventDefinition("messageDefinition").message("message").messageEventDefinitionDone().endEvent().done();

		MessageEventDefinition messageDefinition = bpmnModelInstance.getModelElementById("messageDefinition");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		messageDefinition.CamundaClass = typeof(FailingDelegate).FullName;

		deployment(bpmnModelInstance);

		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(9, job.Retries);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" }) public void FAILING_testFailedRetryWithTimeShift() throws java.text.ParseException
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" })]
	  public virtual void FAILING_testFailedRetryWithTimeShift()
	  {
		// set date to hour before time shift (2015-10-25T03:00:00 CEST =>
		// 2015-10-25T02:00:00 CET)
		DateTime tenMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:50:00 CEST");
		DateTime fiveMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:55:00 CEST");
		DateTime twoMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:58:00 CEST");
		ClockUtil.CurrentTime = tenMinutesBeforeTimeShift;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("failedServiceTask");
		assertNotNull(pi);

		// a job is acquirable
		IList<JobEntity> acquirableJobs = findAndLockAcquirableJobs();
		assertEquals(1, acquirableJobs.Count);

		// execute job
		waitForExecutedJobWithRetriesLeft(4);

		// the job lock time is after the current time but before the time shift
		JobEntity job = (JobEntity) fetchJob(pi.ProcessInstanceId);
		assertTrue(tenMinutesBeforeTimeShift < job.LockExpirationTime);
		assertEquals(fiveMinutesBeforeTimeShift, job.LockExpirationTime);
		assertTrue(twoMinutesBeforeTimeShift > job.LockExpirationTime);

		// the job is not acquirable
		acquirableJobs = findAndLockAcquirableJobs();
		assertEquals(0, acquirableJobs.Count);

		// set clock to two minutes before time shift
		ClockUtil.CurrentTime = twoMinutesBeforeTimeShift;

		// the job is now acquirable
		acquirableJobs = findAndLockAcquirableJobs();
		assertEquals(1, acquirableJobs.Count);

		// execute job
		waitForExecutedJobWithRetriesLeft(3);

		// the job lock time is after the current time
		job = (JobEntity) refreshJob(job.Id);
		assertTrue(twoMinutesBeforeTimeShift < job.LockExpirationTime);

		// the job is not acquirable
		acquirableJobs = findAndLockAcquirableJobs();
		assertEquals("Job shouldn't be acquirable", 0, acquirableJobs.Count);

		ClockUtil.reset();
	  }

	  public virtual void testFailedJobRetryTimeCycleWithExpression()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass("foo").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("${var}").endEvent().done();

		deployment(bpmnModelInstance);

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("var", "R10/PT5M"));

		Job job = managementService.createJobQuery().singleResult();

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(9, job.Retries);
	  }

	  public virtual void testFailedJobRetryTimeCycleWithUndefinedVar()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass("foo").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("${var}").endEvent().done();

		deployment(bpmnModelInstance);

		runtimeService.startProcessInstanceByKey("process");

		Job job = managementService.createJobQuery().singleResult();

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(2, job.Retries); // default behaviour
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testFailedJobRetryTimeCycleWithChangingExpression() throws java.text.ParseException
	  public virtual void testFailedJobRetryTimeCycleWithChangingExpression()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass("foo").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("${var}").endEvent().done();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2017-01-01T09:55:00");
		ClockUtil.CurrentTime = startDate;

		deployment(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("var", "R10/PT5M"));

		startDate = simpleDateFormat.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		Job job = managementService.createJobQuery().singleResult();

		// when
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(9, job.Retries);

		startDate = simpleDateFormat.parse("2017-01-01T10:05:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.setVariable(pi.ProcessInstanceId, "var", "R10/PT10M");

		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		//then
		DateTime expectedDate = simpleDateFormat.parse("2017-01-01T10:15:00");
		DateTime lockExpirationTime = ((JobEntity) managementService.createJobQuery().singleResult()).LockExpirationTime;
		assertEquals(expectedDate, lockExpirationTime);
	  }

	  public virtual void testRetryOnTimerStartEventWithExpression()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().camundaFailedJobRetryTimeCycle("${var}").timerWithDuration("PT5M").serviceTask().camundaClass("bar").endEvent().done();

		deployment(bpmnModelInstance);

		Job job = managementService.createJobQuery().singleResult();

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(2, job.Retries); // default behaviour
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRetryOnAsyncStartEvent() throws Exception
	  public virtual void testRetryOnAsyncStartEvent()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").serviceTask().camundaClass("bar").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testIntermediateCatchEvent() throws Exception
	  public virtual void testIntermediateCatchEvent()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent().message("foo").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testEndEvent() throws Exception
	  public virtual void testEndEvent()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().endEvent().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testExclusiveGateway() throws Exception
	  public virtual void testExclusiveGateway()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().exclusiveGateway().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testInclusiveGateway() throws Exception
	  public virtual void testInclusiveGateway()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().inclusiveGateway().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testEventBasedGateway() throws Exception
	  public virtual void testEventBasedGateway()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().eventBasedGateway().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").intermediateCatchEvent().condition("${true}").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testParallelGateway() throws Exception
	  public virtual void testParallelGateway()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().parallelGateway().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").camundaExecutionListenerClass("start", "foo").endEvent().done();

		deployment(bpmnModelInstance);

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2018-01-01T10:00:00");
		ClockUtil.CurrentTime = startDate;

		runtimeService.startProcessInstanceByKey("process");
		Job job = managementService.createJobQuery().singleResult();

		// assume
		Assert.assertEquals(3, job.Retries);

		// when job fails
		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		// then
		job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(4, job.Retries);

		DateTime expectedDate = simpleDateFormat.parse("2018-01-01T10:05:00");
		assertEquals(expectedDate, ((JobEntity) job).LockExpirationTime);
	  }

	  protected internal virtual void assertJobRetriesForActivity(ProcessInstance pi, string activityId)
	  {
		assertNotNull(pi);

		waitForExecutedJobWithRetriesLeft(4);
		stillOneJobWithExceptionAndRetriesLeft();

		Job job = fetchJob(pi.ProcessInstanceId);
		assertNotNull(job);
		assertEquals(pi.ProcessInstanceId, job.ProcessInstanceId);

		assertEquals(4, job.Retries);

		ExecutionEntity execution = fetchExecutionEntity(pi.ProcessInstanceId, activityId);
		assertNotNull(execution);

		waitForExecutedJobWithRetriesLeft(3);

		job = refreshJob(job.Id);
		assertEquals(3, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft();

		execution = refreshExecutionEntity(execution.Id);
		assertEquals(activityId, execution.ActivityId);

		waitForExecutedJobWithRetriesLeft(2);

		job = refreshJob(job.Id);
		assertEquals(2, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft();

		execution = refreshExecutionEntity(execution.Id);
		assertEquals(activityId, execution.ActivityId);

		waitForExecutedJobWithRetriesLeft(1);

		job = refreshJob(job.Id);
		assertEquals(1, job.Retries);
		stillOneJobWithExceptionAndRetriesLeft();

		execution = refreshExecutionEntity(execution.Id);
		assertEquals(activityId, execution.ActivityId);

		waitForExecutedJobWithRetriesLeft(0);

		job = refreshJob(job.Id);
		assertEquals(0, job.Retries);
		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(0, managementService.createJobQuery().withRetriesLeft().count());
		assertEquals(1, managementService.createJobQuery().noRetriesLeft().count());

		execution = refreshExecutionEntity(execution.Id);
		assertEquals(activityId, execution.ActivityId);
	  }

	  protected internal virtual void waitForExecutedJobWithRetriesLeft(int retriesLeft, string jobId)
	  {
		JobQuery jobQuery = managementService.createJobQuery();

		if (!string.ReferenceEquals(jobId, null))
		{
		  jobQuery.jobId(jobId);
		}

		Job job = jobQuery.singleResult();

		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		}

		// update job
		job = jobQuery.singleResult();

		if (job.Retries != retriesLeft)
		{
		  waitForExecutedJobWithRetriesLeft(retriesLeft, jobId);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void waitForExecutedJobWithRetriesLeft(final int retriesLeft)
	  protected internal virtual void waitForExecutedJobWithRetriesLeft(int retriesLeft)
	  {
		waitForExecutedJobWithRetriesLeft(retriesLeft, null);
	  }

	  protected internal virtual ExecutionEntity refreshExecutionEntity(string executionId)
	  {
		return (ExecutionEntity) runtimeService.createExecutionQuery().executionId(executionId).singleResult();
	  }

	  protected internal virtual ExecutionEntity fetchExecutionEntity(string processInstanceId, string activityId)
	  {
		return (ExecutionEntity) runtimeService.createExecutionQuery().processInstanceId(processInstanceId).activityId(activityId).singleResult();
	  }

	  protected internal virtual Job refreshJob(string jobId)
	  {
		return managementService.createJobQuery().jobId(jobId).singleResult();
	  }

	  protected internal virtual Job fetchJob(string processInstanceId)
	  {
		return managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
	  }

	  protected internal virtual void stillOneJobWithExceptionAndRetriesLeft(string jobId)
	  {
		assertEquals(1, managementService.createJobQuery().jobId(jobId).withException().count());
		assertEquals(1, managementService.createJobQuery().jobId(jobId).withRetriesLeft().count());
	  }

	  protected internal virtual void stillOneJobWithExceptionAndRetriesLeft()
	  {
		assertEquals(1, managementService.createJobQuery().withException().count());
		assertEquals(1, managementService.createJobQuery().withRetriesLeft().count());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.Date createDateFromLocalString(String dateString) throws java.text.ParseException
	  protected internal virtual DateTime createDateFromLocalString(string dateString)
	  {
		// Format: 2015-10-25T02:50:00 CEST
		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss z", Locale.US);
		return dateFormat.parse(dateString);
	  }

	  protected internal virtual IList<JobEntity> findAndLockAcquirableJobs()
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<IList<JobEntity>>
	  {
		  private readonly FoxJobRetryCmdTest outerInstance;

		  public CommandAnonymousInnerClass(FoxJobRetryCmdTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IList<JobEntity> execute(CommandContext commandContext)
		  {
			IList<JobEntity> jobs = commandContext.JobManager.findNextJobsToExecute(new Page(0, 100));
			foreach (JobEntity job in jobs)
			{
			  job.LockOwner = "test";
			}
			return jobs;
		  }
	  }

	}

}