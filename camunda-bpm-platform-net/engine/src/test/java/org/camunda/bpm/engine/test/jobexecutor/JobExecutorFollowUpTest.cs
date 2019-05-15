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
namespace org.camunda.bpm.engine.test.jobexecutor
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// Test cases for handling of new jobs created while a job is executed
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class JobExecutorFollowUpTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobExecutorFollowUpTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testHelper);
		}


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance TWO_TASKS_PROCESS = Bpmn.createExecutableProcess("process").startEvent().serviceTask("serviceTask1").camundaAsyncBefore().camundaClass(typeof(SyncDelegate).FullName).serviceTask("serviceTask2").camundaAsyncBefore().camundaClass(typeof(SyncDelegate).FullName).endEvent().done();

	  protected internal static readonly BpmnModelInstance CALL_ACTIVITY_PROCESS = Bpmn.createExecutableProcess("callActivityProcess").startEvent().callActivity("callActivity").camundaAsyncBefore().calledElement("oneTaskProcess").endEvent().done();

	  protected internal static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess("oneTaskProcess").startEvent().userTask("serviceTask").camundaAsyncBefore().endEvent().done();

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return configuration.setJobExecutor(buildControllableJobExecutor());
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  protected internal ProcessEngineTestRule testHelper;

	  protected internal static ControllableJobExecutor buildControllableJobExecutor()
	  {
		ControllableJobExecutor jobExecutor = new ControllableJobExecutor();
		jobExecutor.MaxJobsPerAcquisition = 2;
		jobExecutor.proceedAndWaitOnShutdown(false);
		return jobExecutor;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal ControllableJobExecutor jobExecutor;
	  protected internal ThreadControl acquisitionThread;
	  protected internal static ThreadControl executionThread;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		jobExecutor = (ControllableJobExecutor)((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;
		jobExecutor.MaxJobsPerAcquisition = 2;
		acquisitionThread = jobExecutor.AcquisitionThreadControl;
		executionThread = jobExecutor.ExecutionThreadControl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void shutdownJobExecutor()
	  public virtual void shutdownJobExecutor()
	  {
		jobExecutor.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteExclusiveFollowUpJobInSameProcessInstance()
	  public virtual void testExecuteExclusiveFollowUpJobInSameProcessInstance()
	  {
		testHelper.deploy(TWO_TASKS_PROCESS);

		// given
		// a process instance with a single job
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("process");

		jobExecutor.start();

		// and first job acquisition that acquires the job
		acquisitionThread.waitForSync();
		acquisitionThread.makeContinueAndWaitForSync();
		// and first job execution
		acquisitionThread.makeContinue();

		// waiting inside delegate
		executionThread.waitForSync();

		// completing delegate
		executionThread.makeContinueAndWaitForSync();

		// then
		// the follow-up job should be executed right away
		// i.e., there is a transition instance for the second service task
		ActivityInstance activityInstance = engineRule.RuntimeService.getActivityInstance(processInstance.Id);
		Assert.assertEquals(1, activityInstance.getTransitionInstances("serviceTask2").Length);

		// and the corresponding job is locked
		JobEntity followUpJob = (JobEntity) engineRule.ManagementService.createJobQuery().singleResult();
		Assert.assertNotNull(followUpJob);
		Assert.assertNotNull(followUpJob.LockOwner);
		Assert.assertNotNull(followUpJob.LockExpirationTime);

		// and the job can be completed successfully such that the process instance ends
		executionThread.makeContinue();
		acquisitionThread.waitForSync();

		// and the process instance has finished
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteExclusiveFollowUpJobInDifferentProcessInstance()
	  public virtual void testExecuteExclusiveFollowUpJobInDifferentProcessInstance()
	  {
		testHelper.deploy(CALL_ACTIVITY_PROCESS, ONE_TASK_PROCESS);

		// given
		// a process instance with a single job
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("callActivityProcess");

		jobExecutor.start();

		// and first job acquisition that acquires the job
		acquisitionThread.waitForSync();
		acquisitionThread.makeContinueAndWaitForSync();
		// and job is executed
		acquisitionThread.makeContinueAndWaitForSync();

		// then
		// the called instance has been created
		ProcessInstance calledInstance = engineRule.RuntimeService.createProcessInstanceQuery().superProcessInstanceId(processInstance.Id).singleResult();
		Assert.assertNotNull(calledInstance);

		// and there is a transition instance for the service task
		ActivityInstance activityInstance = engineRule.RuntimeService.getActivityInstance(calledInstance.Id);
		Assert.assertEquals(1, activityInstance.getTransitionInstances("serviceTask").Length);

		// but the corresponding job is not locked
		JobEntity followUpJob = (JobEntity) engineRule.ManagementService.createJobQuery().singleResult();
		Assert.assertNotNull(followUpJob);
		Assert.assertNull(followUpJob.LockOwner);
		Assert.assertNull(followUpJob.LockExpirationTime);
	  }


	  public class SyncDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  executionThread.sync();
		}
	  }
	}

}