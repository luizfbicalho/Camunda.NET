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
namespace org.camunda.bpm.engine.test.bpmn.job
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobPrioritizationBpmnConstantValueTest : PluggableProcessEngineTestCase
	{

	  protected internal const long EXPECTED_DEFAULT_PRIORITY = 0;

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDefaultPrioritizationAsyncBefore()
	  {
		// when
		runtimeService.createProcessInstanceByKey("oneTaskProcess").startBeforeActivity("task1").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(EXPECTED_DEFAULT_PRIORITY, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/oneTaskProcess.bpmn20.xml")]
	  public virtual void testDefaultPrioritizationAsyncAfter()
	  {
		// given
		runtimeService.createProcessInstanceByKey("oneTaskProcess").startBeforeActivity("task1").execute();

		// when
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(EXPECTED_DEFAULT_PRIORITY, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/oneTimerProcess.bpmn20.xml")]
	  public virtual void testDefaultPrioritizationTimer()
	  {
		// when
		runtimeService.createProcessInstanceByKey("oneTimerProcess").startBeforeActivity("timer1").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(EXPECTED_DEFAULT_PRIORITY, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionPrioritizationAsyncBefore()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task1").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(10, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionPrioritizationAsyncAfter()
	  {
		// given
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task1").execute();

		// when
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(10, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/intermediateTimerJobPrioProcess.bpmn20.xml")]
	  public virtual void testProcessDefinitionPrioritizationTimer()
	  {
		// when
		runtimeService.createProcessInstanceByKey("intermediateTimerJobPrioProcess").startBeforeActivity("timer1").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(8, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioProcess.bpmn20.xml")]
	  public virtual void testActivityPrioritizationAsyncBefore()
	  {
		// when
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(5, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/jobPrioProcess.bpmn20.xml")]
	  public virtual void testActivityPrioritizationAsyncAfter()
	  {
		// given
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		// when
		managementService.executeJob(managementService.createJobQuery().singleResult().Id);

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(5, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/intermediateTimerJobPrioProcess.bpmn20.xml")]
	  public virtual void testActivityPrioritizationTimer()
	  {
		// when
		runtimeService.createProcessInstanceByKey("intermediateTimerJobPrioProcess").startBeforeActivity("timer2").execute();

		// then
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertEquals(4, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/subProcessJobPrioProcess.bpmn20.xml")]
	  public virtual void testSubProcessPriorityIsNotDefaultForContainedActivities()
	  {
		// when starting an activity contained in the sub process where the
		// sub process has job priority 20
		runtimeService.createProcessInstanceByKey("subProcessJobPrioProcess").startBeforeActivity("task1").execute();

		// then the job for that activity has priority 10 which is the process definition's
		// priority; the sub process priority is not inherited
		Job job = managementService.createJobQuery().singleResult();
		assertEquals(10, job.Priority);
	  }

	  public virtual void testFailOnMalformedInput()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/job/invalidPrioProcess.bpmn20.xml").deploy();
		  fail("deploying a process with malformed priority should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("value 'thisIsNotANumber' for attribute 'jobPriority' " + "is not a valid number", e.Message);
		}
	  }

	  public virtual void testParsePriorityOnNonAsyncActivity()
	  {

		// deploying a process definition where the activity
		// has a priority but defines no jobs succeeds
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/job/JobPrioritizationBpmnTest.testParsePriorityOnNonAsyncActivity.bpmn20.xml").deploy();

		// cleanup
		repositoryService.deleteDeployment(deployment.Id);
	  }

	  public virtual void testTimerStartEventPriorityOnProcessDefinition()
	  {
		// given a timer start job
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/job/JobPrioritizationBpmnConstantValueTest.testTimerStartEventPriorityOnProcessDefinition.bpmn20.xml").deploy();

		Job job = managementService.createJobQuery().singleResult();

		// then the timer start job has the priority defined in the process definition
		assertEquals(8, job.Priority);

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testTimerStartEventPriorityOnActivity()
	  {
		// given a timer start job
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/job/JobPrioritizationBpmnConstantValueTest.testTimerStartEventPriorityOnActivity.bpmn20.xml").deploy();

		Job job = managementService.createJobQuery().singleResult();

		// then the timer start job has the priority defined in the process definition
		assertEquals(1515, job.Priority);

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/boundaryTimerJobPrioProcess.bpmn20.xml")]
	  public virtual void testBoundaryTimerEventPriority()
	  {
		// given an active boundary event timer
		runtimeService.startProcessInstanceByKey("boundaryTimerJobPrioProcess");

		Job job = managementService.createJobQuery().singleResult();

		// then the job has the priority specified in the BPMN XML
		assertEquals(20, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/eventSubprocessTimerJobPrioProcess.bpmn20.xml")]
	  public virtual void testEventSubprocessTimerPriority()
	  {
		// given an active event subprocess timer
		runtimeService.startProcessInstanceByKey("eventSubprocessTimerJobPrioProcess");

		Job job = managementService.createJobQuery().singleResult();

		// then the job has the priority specified in the BPMN XML
		assertEquals(25, job.Priority);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/job/intermediateSignalAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/job/intermediateSignalCatchJobPrioProcess.bpmn20.xml"})]
	  public virtual void testAsyncSignalThrowingEventActivityPriority()
	  {
		// given a receiving process instance with two subscriptions
		runtimeService.startProcessInstanceByKey("intermediateSignalCatchJobPrioProcess");

		// and a process instance that executes an async signal throwing event
		runtimeService.startProcessInstanceByKey("intermediateSignalJobPrioProcess");

		Execution signal1Execution = runtimeService.createExecutionQuery().activityId("signal1").singleResult();
		Job signal1Job = managementService.createJobQuery().executionId(signal1Execution.Id).singleResult();

		Execution signal2Execution = runtimeService.createExecutionQuery().activityId("signal2").singleResult();
		Job signal2Job = managementService.createJobQuery().executionId(signal2Execution.Id).singleResult();

		// then the jobs have the priority as specified for the receiving events, not the throwing
		assertEquals(8, signal1Job.Priority);
		assertEquals(4, signal2Job.Priority);

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/job/intermediateSignalAsyncProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/job/signalStartJobPrioProcess.bpmn20.xml"})]
	  public virtual void testAsyncSignalThrowingEventSignalStartActivityPriority()
	  {
		// given a process instance that executes an async signal throwing event
		runtimeService.startProcessInstanceByKey("intermediateSignalJobPrioProcess");

		// then there is an async job for the signal start event with the priority defined in the BPMN XML
		assertEquals(1, managementService.createJobQuery().count());
		Job signalStartJob = managementService.createJobQuery().singleResult();
		assertNotNull(signalStartJob);
		assertEquals(4, signalStartJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/miBodyAsyncProcess.bpmn20.xml")]
	  public virtual void FAILING_testMultiInstanceBodyActivityPriority()
	  {
		// given a process instance that executes an async mi body
		runtimeService.startProcessInstanceByKey("miBodyAsyncPriorityProcess");

		// then there is a job that has the priority as defined on the activity
		assertEquals(1, managementService.createJobQuery().count());
		Job miBodyJob = managementService.createJobQuery().singleResult();
		assertNotNull(miBodyJob);
		assertEquals(5, miBodyJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/miInnerAsyncProcess.bpmn20.xml")]
	  public virtual void testMultiInstanceInnerActivityPriority()
	  {
		// given a process instance that executes an async mi inner activity
		runtimeService.startProcessInstanceByKey("miBodyAsyncPriorityProcess");

		// then there are three jobs that have the priority as defined on the activity (TODO: or should it be MI characteristics?)
		IList<Job> jobs = managementService.createJobQuery().list();

		assertEquals(3, jobs.Count);
		foreach (Job job in jobs)
		{
		  assertNotNull(job);
		  assertEquals(5, job.Priority);
		}
	  }
	}

}