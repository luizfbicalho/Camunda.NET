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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobExecutorShutdownTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobExecutorShutdownTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule);
		}


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance TWO_ASYNC_TASKS = Bpmn.createExecutableProcess("process").startEvent().serviceTask("task1").camundaClass(typeof(SyncDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).serviceTask("task2").camundaClass(typeof(SyncDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance SINGLE_ASYNC_TASK = Bpmn.createExecutableProcess("process").startEvent().serviceTask("task1").camundaClass(typeof(SyncDelegate).FullName).camundaAsyncBefore().camundaExclusive(true).endEvent().done();

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return configuration.setJobExecutor(buildControllableJobExecutor());
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

	  protected internal ControllableJobExecutor jobExecutor;
	  protected internal ThreadControl acquisitionThread;
	  protected internal static ThreadControl executionThread;

	  protected internal static ControllableJobExecutor buildControllableJobExecutor()
	  {
		ControllableJobExecutor jobExecutor = new ControllableJobExecutor();
		jobExecutor.MaxJobsPerAcquisition = 2;
		jobExecutor.proceedAndWaitOnShutdown(false);
		return jobExecutor;
	  }

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
//ORIGINAL LINE: @Test public void testConcurrentShutdownAndExclusiveFollowUpJob()
	  public virtual void testConcurrentShutdownAndExclusiveFollowUpJob()
	  {
		// given
		Deployment deployment = engineRule.RepositoryService.createDeployment().addModelInstance("foo.bpmn", TWO_ASYNC_TASKS).deploy();
		engineRule.manageDeployment(deployment);

		engineRule.RuntimeService.startProcessInstanceByKey("process");

		Job firstAsyncJob = engineRule.ManagementService.createJobQuery().singleResult();

		jobExecutor.start();

		// wait before acquisition
		acquisitionThread.waitForSync();
		// wait for no more acquisition syncs
		acquisitionThread.ignoreFutureSyncs();
		acquisitionThread.makeContinue();

		// when waiting during execution of first job
		executionThread.waitForSync();

		// and shutting down the job executor
		jobExecutor.shutdown();

		// and continuing job execution
		executionThread.waitUntilDone();

		// then the current job has completed successfully
		Assert.assertEquals(0, engineRule.ManagementService.createJobQuery().jobId(firstAsyncJob.Id).count());

		// but the exclusive follow-up job is not executed and is not locked
		JobEntity secondAsyncJob = (JobEntity) engineRule.ManagementService.createJobQuery().singleResult();
		Assert.assertNotNull(secondAsyncJob);
		Assert.assertFalse(secondAsyncJob.Id.Equals(firstAsyncJob.Id));
		Assert.assertNull(secondAsyncJob.LockOwner);
		Assert.assertNull(secondAsyncJob.LockExpirationTime);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShutdownAndMultipleLockedJobs()
	  public virtual void testShutdownAndMultipleLockedJobs()
	  {
		// given
		Deployment deployment = engineRule.RepositoryService.createDeployment().addModelInstance("foo.bpmn", SINGLE_ASYNC_TASK).deploy();
		engineRule.manageDeployment(deployment);

		// add two jobs by starting two process instances
		engineRule.RuntimeService.startProcessInstanceByKey("process");
		engineRule.RuntimeService.startProcessInstanceByKey("process");

		jobExecutor.start();

		// wait before acquisition
		acquisitionThread.waitForSync();
		// wait for no more acquisition syncs
		acquisitionThread.ignoreFutureSyncs();

		acquisitionThread.makeContinue();

		// when waiting during execution of first job
		executionThread.waitForSync();

		// jobs must now be locked
		IList<Job> lockedJobList = engineRule.ManagementService.createJobQuery().list();
		Assert.assertEquals(2, lockedJobList.Count);
		foreach (Job job in lockedJobList)
		{
		  JobEntity jobEntity = (JobEntity)job;
		  Assert.assertNotNull(jobEntity.LockOwner);
		}

		// shut down the job executor while first job is executing
		jobExecutor.shutdown();

		// then let first job continue
		executionThread.waitUntilDone();

		// check that only one job left, which is not executed nor locked
		JobEntity jobEntity = (JobEntity) engineRule.ManagementService.createJobQuery().singleResult();
		Assert.assertNotNull(jobEntity);
		Assert.assertTrue(lockedJobList[1].Id.Equals(jobEntity.Id) || lockedJobList[0].Id.Equals(jobEntity.Id));
		Assert.assertNull(jobEntity.LockOwner);
		Assert.assertNull(jobEntity.LockExpirationTime);
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