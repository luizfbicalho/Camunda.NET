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
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class GlobalRetryConfigurationTest
	{
		private bool InstanceFieldsInitialized = false;

		public GlobalRetryConfigurationTest()
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


	  private const string PROCESS_ID = "process";
	  private const string FAILING_CLASS = "this.class.does.not.Exist";
	  private const string FAILING_EVENT = "failingEvent";
	  private const string SCHEDULE = "R5/PT5M";
	  private const int JOB_RETRIES = 4;

	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.FailedJobRetryTimeCycle = SCHEDULE;
			return configuration;
		  }
	  }

	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  private RuntimeService runtimeService;
	  private ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedServiceTaskStandardStrategy()
	  public virtual void testFailedServiceTaskStandardStrategy()
	  {
		engineRule.ProcessEngineConfiguration.FailedJobRetryTimeCycle = null;
		BpmnModelInstance bpmnModelInstance = prepareFailingServiceTask();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedIntermediateThrowingSignalEventAsync()
	  public virtual void testFailedIntermediateThrowingSignalEventAsync()
	  {
		BpmnModelInstance bpmnModelInstance = prepareSignalEventProcessWithoutRetry();

		testRule.deploy(bpmnModelInstance);
		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);
		assertJobRetries(pi, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedServiceTask()
	  public virtual void testFailedServiceTask()
	  {
		BpmnModelInstance bpmnModelInstance = prepareFailingServiceTask();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedServiceTaskMixConfiguration()
	  public virtual void testFailedServiceTaskMixConfiguration()
	  {
		BpmnModelInstance bpmnModelInstance = prepareFailingServiceTaskWithRetryCycle();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, 9);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedBusinessRuleTask()
	  public virtual void testFailedBusinessRuleTask()
	  {
		BpmnModelInstance bpmnModelInstance = prepareFailingBusinessRuleTask();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailedCallActivity()
	  public virtual void testFailedCallActivity()
	  {

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_ID).startEvent().callActivity().calledElement("testProcess2").endEvent().done(), Bpmn.createExecutableProcess("testProcess2").startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().endEvent().done());

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess2");

		assertJobRetries(pi, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingScriptTask()
	  public virtual void testFailingScriptTask()
	  {
		BpmnModelInstance bpmnModelInstance = prepareFailingScriptTask();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingSubProcess()
	  public virtual void testFailingSubProcess()
	  {
		BpmnModelInstance bpmnModelInstance = prepareFailingSubProcess();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey(PROCESS_ID);

		assertJobRetries(pi, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetryOnAsyncStartEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testRetryOnAsyncStartEvent()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("process").startEvent().camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R5/PT5M").serviceTask().camundaClass("bar").endEvent().done();

		testRule.deploy(bpmnModelInstance);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		assertJobRetries(processInstance, 4);
	  }

	  private void assertJobRetries(ProcessInstance pi, int expectedJobRetries)
	  {
		assertThat(pi, @is(notNullValue()));

		Job job = fetchJob(pi.ProcessInstanceId);

		try
		{

		  managementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		}

		// update job
		job = fetchJob(pi.ProcessInstanceId);
		assertEquals(expectedJobRetries, job.Retries);
	  }

	  private Job fetchJob(string processInstanceId)
	  {
		return managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
	  }

	  private BpmnModelInstance prepareSignalEventProcessWithoutRetry()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().intermediateThrowEvent(FAILING_EVENT).camundaAsyncBefore(true).signal("start").serviceTask().camundaClass(FAILING_CLASS).endEvent().done();
		return modelInstance;
	  }

	  private BpmnModelInstance prepareFailingServiceTask()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().endEvent().done();
		return modelInstance;
	  }

	  private BpmnModelInstance prepareFailingServiceTaskWithRetryCycle()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().camundaFailedJobRetryTimeCycle("R10/PT5M").endEvent().done();
		return modelInstance;
	  }

	  private BpmnModelInstance prepareFailingBusinessRuleTask()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().businessRuleTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().endEvent().done();
		return modelInstance;
	  }

	  private BpmnModelInstance prepareFailingScriptTask()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().scriptTask().scriptFormat("groovy").scriptText("x = 5 / 0").camundaAsyncBefore().userTask().endEvent().done();
		return bpmnModelInstance;
	  }

	  private BpmnModelInstance prepareFailingSubProcess()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().subProcess().embeddedSubProcess().startEvent().serviceTask().camundaClass(FAILING_CLASS).camundaAsyncBefore().endEvent().subProcessDone().endEvent().done();
		return bpmnModelInstance;
	  }
	}
}