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

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using RecordedWaitEvent = org.camunda.bpm.engine.test.jobexecutor.RecordingAcquireJobsRunnable.RecordedWaitEvent;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
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
	public class JobAcquisitionTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobAcquisitionTest()
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


	  protected internal const int DEFAULT_NUM_JOBS_TO_ACQUIRE = 3;

	  protected internal ControllableJobExecutor jobExecutor1;
	  protected internal ControllableJobExecutor jobExecutor2;

	  protected internal ThreadControl acquisitionThread1;
	  protected internal ThreadControl acquisitionThread2;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return configuration.setJobExecutor(new ControllableJobExecutor());
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		// two job executors with the default settings
		jobExecutor1 = (ControllableJobExecutor)((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;
		jobExecutor1.MaxJobsPerAcquisition = DEFAULT_NUM_JOBS_TO_ACQUIRE;
		acquisitionThread1 = jobExecutor1.AcquisitionThreadControl;

		jobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl) engineRule.ProcessEngine);
		jobExecutor2.MaxJobsPerAcquisition = DEFAULT_NUM_JOBS_TO_ACQUIRE;
		acquisitionThread2 = jobExecutor2.AcquisitionThreadControl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		jobExecutor1.shutdown();
		jobExecutor2.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testJobLockingFailure()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testJobLockingFailure()
	  {
		int numberOfInstances = 3;

		// when starting a number of process instances
		for (int i = 0; i < numberOfInstances; i++)
		{
		  engineRule.RuntimeService.startProcessInstanceByKey("simpleAsyncProcess").Id;
		}

		// when starting job execution, both acquisition threads wait before acquiring something
		jobExecutor1.start();
		acquisitionThread1.waitForSync();
		jobExecutor2.start();
		acquisitionThread2.waitForSync();

		// when having both threads acquire jobs
		// then both wait before committing the acquiring transaction (AcquireJobsCmd)
		acquisitionThread1.makeContinueAndWaitForSync();
		acquisitionThread2.makeContinueAndWaitForSync();

		// when continuing acquisition thread 1
		acquisitionThread1.makeContinueAndWaitForSync();

		// then it has not performed waiting since it was able to acquire and execute all jobs
		Assert.assertEquals(0, engineRule.ManagementService.createJobQuery().active().count());
		IList<RecordedWaitEvent> jobExecutor1WaitEvents = jobExecutor1.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(1, jobExecutor1WaitEvents.Count);
		Assert.assertEquals(0, jobExecutor1WaitEvents[0].TimeBetweenAcquisitions);

		// when continuing acquisition thread 2
		acquisitionThread2.makeContinueAndWaitForSync();

		// then its acquisition cycle fails with OLEs
		// but the acquisition thread immediately tries again
		IList<RecordedWaitEvent> jobExecutor2WaitEvents = jobExecutor2.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(1, jobExecutor2WaitEvents.Count);
		Assert.assertEquals(0, jobExecutor2WaitEvents[0].TimeBetweenAcquisitions);

	  }
	}

}