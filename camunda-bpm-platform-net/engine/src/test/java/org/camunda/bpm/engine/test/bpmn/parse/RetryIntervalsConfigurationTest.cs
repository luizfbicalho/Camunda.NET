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
namespace org.camunda.bpm.engine.test.bpmn.parse
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AbstractAsyncOperationsTest = org.camunda.bpm.engine.test.api.AbstractAsyncOperationsTest;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class RetryIntervalsConfigurationTest : AbstractAsyncOperationsTest
	{
		private bool InstanceFieldsInitialized = false;

		public RetryIntervalsConfigurationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  private static readonly SimpleDateFormat SIMPLE_DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
	  private const string PROCESS_ID = "process";
	  private const string FAILING_CLASS = "this.class.does.not.Exist";

	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.FailedJobRetryTimeCycle = "PT5M,PT20M, PT3M";
			configuration.EnableExceptionsAfterUnhandledBpmnError = true;
			return configuration;
		  }
	  }

	  public new ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public new ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  private new RuntimeService runtimeService;
	  private new ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		Batch batch = managementService.createBatchQuery().singleResult();
		if (batch != null)
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  engineRule.HistoryService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryGlobalConfiguration() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRetryGlobalConfiguration()
	  {
		// given global retry conf. ("PT5M,PT20M, PT3M")
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTask();
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 5);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(2, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 20);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryGlobalConfigurationWithExecutionListener() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRetryGlobalConfigurationWithExecutionListener()
	  {
		// given
		engineRule.ProcessEngineConfiguration.FailedJobRetryTimeCycle = "PT5M";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).endEvent().done();
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 5);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryMixConfiguration() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRetryMixConfiguration()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTaskWithRetryCycle("R3/PT1M");

		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries;

		for (int i = 0; i < 3; i++)
		{
		  jobRetries = executeJob(processInstanceId);
		  assertEquals(2 - i, jobRetries);
		  currentTime = DateUtils.addMinutes(currentTime, 1);
		  assertLockExpirationTime(currentTime);
		  ClockUtil.CurrentTime = currentTime;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryIntervals() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRetryIntervals()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTaskWithRetryCycle("PT3M, PT10M,PT8M");
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(2, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 10);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 8);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSingleRetryInterval() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSingleRetryInterval()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTaskWithRetryCycle("PT8M ");
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 8);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryWithVarList()
	  public virtual void testRetryWithVarList()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTaskWithRetryCycle("${var}");
		testRule.deploy(bpmnModelInstance);

		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("var", "PT1M,PT2M,PT3M,PT4M,PT5M,PT6M,PT7M,PT8M"));

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
		Assert.assertEquals(8, job.Retries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntervalsAfterUpdateRetries() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testIntervalsAfterUpdateRetries()
	  {
		// given
		BpmnModelInstance bpmnModelInstance = prepareProcessFailingServiceTaskWithRetryCycle("PT3M, PT10M,PT8M");
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		managementService.setJobRetries(Arrays.asList(job.Id), 5);

		jobRetries = executeJob(processInstanceId);
		assertEquals(4, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(2, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 10);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 8);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMixConfigurationWithinOneProcess() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testMixConfigurationWithinOneProcess()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask("Task1").camundaClass(typeof(ServiceTaskDelegate).FullName).camundaAsyncBefore().serviceTask("Task2").camundaClass(FAILING_CLASS).camundaAsyncBefore().camundaFailedJobRetryTimeCycle("PT3M, PT10M,PT8M").endEvent().done();
		testRule.deploy(bpmnModelInstance);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		// try to execute the first service task without success
		int jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 5);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		ServiceTaskDelegate.firstAttempt = false;

		// finish the first service task
		jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);

		// try to execute the second service task without success
		jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 3);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testlocalConfigurationWithNestedChangingExpression() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testlocalConfigurationWithNestedChangingExpression()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().serviceTask().camundaClass("foo").camundaAsyncBefore().camundaFailedJobRetryTimeCycle("${var}").endEvent().done();

		SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		DateTime startDate = simpleDateFormat.parse("2017-01-01T09:55:00");
		ClockUtil.CurrentTime = startDate;

		testRule.deploy(bpmnModelInstance);

		VariableMap @params = Variables.createVariables();
		@params.putValue("var", "${nestedVar1},PT15M,${nestedVar3}");
		@params.putValue("nestedVar", "PT13M");
		@params.putValue("nestedVar1", "PT5M");
		@params.putValue("nestedVar3", "PT25M");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("process", @params);

		ClockUtil.CurrentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T09:55:00");

		assertNotNull(pi);

		DateTime currentTime = SIMPLE_DATE_FORMAT.parse("2017-01-01T10:00:00");
		ClockUtil.CurrentTime = currentTime;

		string processInstanceId = pi.ProcessInstanceId;

		int jobRetries = executeJob(processInstanceId);
		assertEquals(3, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 5);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(2, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 15);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		runtimeService.setVariable(pi.ProcessInstanceId, "var", "${nestedVar}");

		jobRetries = executeJob(processInstanceId);
		assertEquals(1, jobRetries);
		currentTime = DateUtils.addMinutes(currentTime, 13);
		assertLockExpirationTime(currentTime);
		ClockUtil.CurrentTime = currentTime;

		jobRetries = executeJob(processInstanceId);
		assertEquals(0, jobRetries);
	  }

	  private int executeJob(string processInstanceId)
	  {
		Job job = fetchJob(processInstanceId);

		try
		{
		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // ignore
		}

		job = fetchJob(processInstanceId);

		return job.Retries;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assertLockExpirationTime(java.util.Date expectedDate) throws java.text.ParseException
	  private void assertLockExpirationTime(DateTime expectedDate)
	  {
		DateTime lockExpirationTime = ((JobEntity) managementService.createJobQuery().singleResult()).LockExpirationTime;
		assertEquals(expectedDate, lockExpirationTime);
	  }

	  private Job fetchJob(string processInstanceId)
	  {
		return managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
	  }

	  private BpmnModelInstance prepareProcessFailingServiceTask()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().endEvent().done();
		return modelInstance;
	  }

	  private BpmnModelInstance prepareProcessFailingServiceTaskWithRetryCycle(string retryTimeCycle)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().camundaFailedJobRetryTimeCycle(retryTimeCycle).endEvent().done();
		return modelInstance;
	  }

	}

}