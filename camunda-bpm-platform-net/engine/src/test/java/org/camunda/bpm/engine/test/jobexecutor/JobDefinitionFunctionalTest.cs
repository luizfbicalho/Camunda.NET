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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobDefinitionFunctionalTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobDefinitionFunctionalTest()
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
		}


	  internal static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testRule = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testRule;

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal static readonly BpmnModelInstance SIMPLE_ASYNC_PROCESS = Bpmn.createExecutableProcess("simpleAsyncProcess").startEvent().serviceTask().camundaExpression("${true}").camundaAsyncBefore().endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateJobInstanceSuspended()
	  public virtual void testCreateJobInstanceSuspended()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		// given suspended job definition:
		managementService.suspendJobDefinitionByProcessDefinitionKey("simpleAsyncProcess");

		// if I start a new instance
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// then the new job instance is created as suspended:
		assertNotNull(managementService.createJobQuery().suspended().singleResult());
		assertNull(managementService.createJobQuery().active().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateJobInstanceActive()
	  public virtual void testCreateJobInstanceActive()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		// given that the job definition is not suspended:

		// if I start a new instance
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// then the new job instance is created as active:
		assertNull(managementService.createJobQuery().suspended().singleResult());
		assertNotNull(managementService.createJobQuery().active().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobExecutorOnlyAcquiresActiveJobs()
	  public virtual void testJobExecutorOnlyAcquiresActiveJobs()
	  {
		testRule.deploy(SIMPLE_ASYNC_PROCESS);

		// given suspended job definition:
		managementService.suspendJobDefinitionByProcessDefinitionKey("simpleAsyncProcess");

		// if I start a new instance
		runtimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// then the new job executor will not acquire the job:
		AcquiredJobs acquiredJobs = acquireJobs();
		assertEquals(0, acquiredJobs.size());

		// -------------------------

		// given a active job definition:
		managementService.activateJobDefinitionByProcessDefinitionKey("simpleAsyncProcess", true);

		// then the new job executor will not acquire the job:
		acquiredJobs = acquireJobs();
		assertEquals(1, acquiredJobs.size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExclusiveJobs()
	  public virtual void testExclusiveJobs()
	  {
		testRule.deploy(Bpmn.createExecutableProcess("testProcess").startEvent().serviceTask("task1").camundaExpression("${true}").camundaAsyncBefore().serviceTask("task2").camundaExpression("${true}").camundaAsyncBefore().endEvent().done());

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().activityIdIn("task2").singleResult();

		// given that the second task is suspended
		managementService.suspendJobDefinitionById(jobDefinition.Id);

		// if I start a process instance
		runtimeService.startProcessInstanceByKey("testProcess");

		testRule.waitForJobExecutorToProcessAllJobs(10000);

		// then the second task is not executed
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		// there is a suspended job instance
		Job job = managementService.createJobQuery().singleResult();
		assertEquals(job.JobDefinitionId, jobDefinition.Id);
		assertTrue(job.Suspended);

		// if I unsuspend the job definition, the job is executed:
		managementService.activateJobDefinitionById(jobDefinition.Id, true);

		testRule.waitForJobExecutorToProcessAllJobs(10000);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
	  }

	  protected internal virtual AcquiredJobs acquireJobs()
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;

		return processEngineConfiguration.CommandExecutorTxRequired.execute(new AcquireJobsCmd(jobExecutor));
	  }

	}

}