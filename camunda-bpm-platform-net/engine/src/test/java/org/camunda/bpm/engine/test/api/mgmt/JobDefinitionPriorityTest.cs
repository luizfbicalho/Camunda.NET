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
namespace org.camunda.bpm.engine.test.api.mgmt
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobDefinitionPriorityTest : PluggableProcessEngineTestCase
	{

	  protected internal const long EXPECTED_DEFAULT_PRIORITY = 0;

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriority()
	  {
		// given a process instance with a job with default priority and a corresponding job definition
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("asyncTaskProcess").startBeforeActivity("task").execute();

		Job job = managementService.createJobQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(job.JobDefinitionId).singleResult();

		// when I set the job definition's priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// then the job definition's priority value has changed
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertEquals(42, (long) updatedDefinition.OverridingJobPriority);

		// the existing job's priority has not changed
		Job updatedExistingJob = managementService.createJobQuery().singleResult();
		assertEquals(job.Priority, updatedExistingJob.Priority);

		// and a new job of that definition receives the updated priority
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("task").execute();

		Job newJob = getJobThatIsNot(updatedExistingJob);
		assertEquals(42, newJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriorityWithCascade()
	  {
		// given a process instance with a job with default priority and a corresponding job definition
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("asyncTaskProcess").startBeforeActivity("task").execute();

		Job job = managementService.createJobQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(job.JobDefinitionId).singleResult();

		// when I set the job definition's priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 52, true);

		// then the job definition's priority value has changed
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertEquals(52, (long) updatedDefinition.OverridingJobPriority);

		// the existing job's priority has changed as well
		Job updatedExistingJob = managementService.createJobQuery().singleResult();
		assertEquals(52, updatedExistingJob.Priority);

		// and a new job of that definition receives the updated priority
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("task").execute();

		Job newJob = getJobThatIsNot(updatedExistingJob);
		assertEquals(52, newJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriorityOverridesBpmnPriority()
	  {
		// given a process instance with a job with default priority and a corresponding job definition
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		Job job = managementService.createJobQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(job.JobDefinitionId).singleResult();

		// when I set the job definition's priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 62);

		// then the job definition's priority value has changed
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertEquals(62, (long) updatedDefinition.OverridingJobPriority);

		// the existing job's priority is still the value as given in the BPMN XML
		Job updatedExistingJob = managementService.createJobQuery().singleResult();
		assertEquals(5, updatedExistingJob.Priority);

		// and a new job of that definition receives the updated priority
		// meaning that the updated priority overrides the priority specified in the BPMN XML
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("task2").execute();

		Job newJob = getJobThatIsNot(updatedExistingJob);
		assertEquals(62, newJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriorityWithCascadeOverridesBpmnPriority()
	  {
		// given a process instance with a job with default priority and a corresponding job definition
		ProcessInstance instance = runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		Job job = managementService.createJobQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(job.JobDefinitionId).singleResult();

		// when I set the job definition's priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 72, true);

		// then the job definition's priority value has changed
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertEquals(72, (long) updatedDefinition.OverridingJobPriority);

		// the existing job's priority has changed as well
		Job updatedExistingJob = managementService.createJobQuery().singleResult();
		assertEquals(72, updatedExistingJob.Priority);

		// and a new job of that definition receives the updated priority
		// meaning that the updated priority overrides the priority specified in the BPMN XML
		runtimeService.createProcessInstanceModification(instance.Id).startBeforeActivity("task2").execute();

		Job newJob = getJobThatIsNot(updatedExistingJob);
		assertEquals(72, newJob.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioProcess.bpmn20.xml")]
	  public virtual void testRedeployOverridesSetJobDefinitionPriority()
	  {
		// given a process instance with a job with default priority and a corresponding job definition
		runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		Job job = managementService.createJobQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(job.JobDefinitionId).singleResult();

		// when I set the job definition's priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 72, true);

		// then the job definition's priority value has changed
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertEquals(72, (long) updatedDefinition.OverridingJobPriority);

		// the existing job's priority has changed as well
		Job updatedExistingJob = managementService.createJobQuery().singleResult();
		assertEquals(72, updatedExistingJob.Priority);

		// if the process definition is redeployed
		string secondDeploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/jobPrioProcess.bpmn20.xml").deploy().Id;

		// then a new job will have the priority from the BPMN xml
		ProcessInstance secondInstance = runtimeService.createProcessInstanceByKey("jobPrioProcess").startBeforeActivity("task2").execute();

		Job newJob = managementService.createJobQuery().processInstanceId(secondInstance.Id).singleResult();
		assertEquals(5, newJob.Priority);

		repositoryService.deleteDeployment(secondDeploymentId, true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testResetJobDefinitionPriority()
	  {

		// given a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when I set a priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		// and I reset the priority
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then the job definition priority is still null
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertNull(updatedDefinition.OverridingJobPriority);

		// and a new job instance does not receive the intermittently set priority
		runtimeService.createProcessInstanceByKey("asyncTaskProcess").startBeforeActivity("task").execute();

		Job job = managementService.createJobQuery().singleResult();
		assertEquals(EXPECTED_DEFAULT_PRIORITY, job.Priority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testResetJobDefinitionPriorityWhenPriorityIsNull()
	  {

		// given a job definition with null priority
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		assertNull(jobDefinition.OverridingJobPriority);

		// when I set a priority
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then the priority remains unchanged
		JobDefinition updatedDefinition = managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult();
		assertNull(updatedDefinition.OverridingJobPriority);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/jobPrioProcess.bpmn20.xml")]
	  public virtual void testGetJobDefinitionDefaultPriority()
	  {
		// with a process with job definitions deployed
		// then the definitions have a default null priority, meaning that they don't override the
		// value in the BPMN XML
		IList<JobDefinition> jobDefinitions = managementService.createJobDefinitionQuery().list();
		assertEquals(4, jobDefinitions.Count);

		assertNull(jobDefinitions[0].OverridingJobPriority);
		assertNull(jobDefinitions[1].OverridingJobPriority);
		assertNull(jobDefinitions[2].OverridingJobPriority);
		assertNull(jobDefinitions[3].OverridingJobPriority);
	  }

	  public virtual void testSetNonExistingJobDefinitionPriority()
	  {
		try
		{
		  managementService.setOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId", 42);
		  fail("should not succeed");
		}
		catch (NotFoundException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist", e.Message);
		}

		try
		{
		  managementService.setOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId", 42, true);
		  fail("should not succeed");
		}
		catch (NotFoundException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist", e.Message);
		}
	  }

	  public virtual void testResetNonExistingJobDefinitionPriority()
	  {
		try
		{
		  managementService.clearOverridingJobPriorityForJobDefinition("someNonExistingJobDefinitionId");
		  fail("should not succeed");
		}
		catch (NotFoundException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("job definition with id 'someNonExistingJobDefinitionId' does not exist", e.Message);
		}
	  }

	  public virtual void testSetNullJobDefinitionPriority()
	  {
		try
		{
		  managementService.setOverridingJobPriorityForJobDefinition(null, 42);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
		}

		try
		{
		  managementService.setOverridingJobPriorityForJobDefinition(null, 42, true);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
		}
	  }

	  public virtual void testResetNullJobDefinitionPriority()
	  {
		try
		{
		  managementService.clearOverridingJobPriorityForJobDefinition(null);
		  fail("should not succeed");
		}
		catch (NotValidException e)
		{
		  // happy path
		  assertTextPresentIgnoreCase("jobDefinitionId is null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobDefinitionPriorityToExtremeValues()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// it is possible to set the max long value
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, long.MaxValue);
		jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		assertEquals(long.MaxValue, (long) jobDefinition.OverridingJobPriority);

		// it is possible to set the min long value
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, long.MinValue + 1); // +1 for informix
		jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		assertEquals(long.MinValue + 1, (long) jobDefinition.OverridingJobPriority);
	  }

	  protected internal virtual Job getJobThatIsNot(Job other)
	  {
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(2, jobs.Count);

		if (jobs[0].Id.Equals(other.Id))
		{
		  return jobs[1];
		}
		else if (jobs[1].Id.Equals(other.Id))
		{
		  return jobs[0];
		}
		else
		{
		  throw new ProcessEngineException("Job with id " + other.Id + " does not exist anymore");
		}
	  }

	}

}