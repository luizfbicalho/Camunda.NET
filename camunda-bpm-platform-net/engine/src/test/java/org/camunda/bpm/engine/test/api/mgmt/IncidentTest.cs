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
namespace org.camunda.bpm.engine.test.api.mgmt
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	public class IncidentTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldCreateOneIncident()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(incident);

		assertNotNull(incident.Id);
		assertNotNull(incident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(AlwaysFailingDelegate.MESSAGE, incident.IncidentMessage);
		assertEquals(processInstance.Id, incident.ExecutionId);
		assertEquals("theServiceTask", incident.ActivityId);
		assertEquals(processInstance.Id, incident.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, incident.ProcessDefinitionId);
		assertEquals(incident.Id, incident.CauseIncidentId);
		assertEquals(incident.Id, incident.RootCauseIncidentId);

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(job);

		assertEquals(job.Id, incident.Configuration);
		assertEquals(job.JobDefinitionId, incident.JobDefinitionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldCreateOneIncidentAfterSetRetries()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidents = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).list();

		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 1);

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(job);

		// set job retries to 1 -> should fail again and a second incident should be created
		managementService.setJobRetries(job.Id, 1);

		executeAvailableJobs();

		incidents = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).list();

		// There is still one incident
		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldCreateOneIncidentAfterExecuteJob()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IList<Incident> incidents = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).list();

		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 1);

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(job);

		// set job retries to 1 -> should fail again and a second incident should be created
		try
		{
		  managementService.executeJob(job.Id);
		  fail("Exception was expected.");
		}
		catch (ProcessEngineException)
		{
		  // exception expected
		}

		incidents = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).list();

		// There is still one incident
		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncidentForNestedExecution.bpmn"})]
	  public virtual void testShouldCreateOneIncidentForNestedExecution()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcessWithNestedExecutions");

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(incident);

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(job);

		string executionIdOfNestedFailingExecution = job.ExecutionId;

		assertFalse(string.ReferenceEquals(processInstance.Id, executionIdOfNestedFailingExecution));

		assertNotNull(incident.Id);
		assertNotNull(incident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, incident.IncidentType);
		assertEquals(AlwaysFailingDelegate.MESSAGE, incident.IncidentMessage);
		assertEquals(executionIdOfNestedFailingExecution, incident.ExecutionId);
		assertEquals("theServiceTask", incident.ActivityId);
		assertEquals(processInstance.Id, incident.ProcessInstanceId);
		assertEquals(incident.Id, incident.CauseIncidentId);
		assertEquals(incident.Id, incident.RootCauseIncidentId);
		assertEquals(job.Id, incident.Configuration);
		assertEquals(job.JobDefinitionId, incident.JobDefinitionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateRecursiveIncidents.bpmn", "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldCreateRecursiveIncidents()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callFailingProcess");

		executeAvailableJobs();

		IList<Incident> incidents = runtimeService.createIncidentQuery().list();
		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 2);

		ProcessInstance failingProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("failingProcess").singleResult();
		assertNotNull(failingProcess);

		ProcessInstance callProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("callFailingProcess").singleResult();
		assertNotNull(callProcess);

		// Root cause incident
		Incident causeIncident = runtimeService.createIncidentQuery().processDefinitionId(failingProcess.ProcessDefinitionId).singleResult();
		assertNotNull(causeIncident);

		Job job = managementService.createJobQuery().executionId(causeIncident.ExecutionId).singleResult();
		assertNotNull(job);

		assertNotNull(causeIncident.Id);
		assertNotNull(causeIncident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, causeIncident.IncidentType);
		assertEquals(AlwaysFailingDelegate.MESSAGE, causeIncident.IncidentMessage);
		assertEquals(job.ExecutionId, causeIncident.ExecutionId);
		assertEquals("theServiceTask", causeIncident.ActivityId);
		assertEquals(failingProcess.Id, causeIncident.ProcessInstanceId);
		assertEquals(causeIncident.Id, causeIncident.CauseIncidentId);
		assertEquals(causeIncident.Id, causeIncident.RootCauseIncidentId);
		assertEquals(job.Id, causeIncident.Configuration);
		assertEquals(job.JobDefinitionId, causeIncident.JobDefinitionId);

		// Recursive created incident
		Incident recursiveCreatedIncident = runtimeService.createIncidentQuery().processDefinitionId(callProcess.ProcessDefinitionId).singleResult();
		assertNotNull(recursiveCreatedIncident);

		Execution theCallActivityExecution = runtimeService.createExecutionQuery().activityId("theCallActivity").singleResult();
		assertNotNull(theCallActivityExecution);

		assertNotNull(recursiveCreatedIncident.Id);
		assertNotNull(recursiveCreatedIncident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, recursiveCreatedIncident.IncidentType);
		assertNull(recursiveCreatedIncident.IncidentMessage);
		assertEquals(theCallActivityExecution.Id, recursiveCreatedIncident.ExecutionId);
		assertEquals("theCallActivity", recursiveCreatedIncident.ActivityId);
		assertEquals(processInstance.Id, recursiveCreatedIncident.ProcessInstanceId);
		assertEquals(causeIncident.Id, recursiveCreatedIncident.CauseIncidentId);
		assertEquals(causeIncident.Id, recursiveCreatedIncident.RootCauseIncidentId);
		assertNull(recursiveCreatedIncident.Configuration);
		assertNull(recursiveCreatedIncident.JobDefinitionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateRecursiveIncidentsForNestedCallActivity.bpmn", "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateRecursiveIncidents.bpmn", "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldCreateRecursiveIncidentsForNestedCallActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("callingFailingCallActivity");

		executeAvailableJobs();

		IList<Incident> incidents = runtimeService.createIncidentQuery().list();
		assertFalse(incidents.Count == 0);
		assertTrue(incidents.Count == 3);

		// Root Cause Incident
		ProcessInstance failingProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("failingProcess").singleResult();
		assertNotNull(failingProcess);

		Incident rootCauseIncident = runtimeService.createIncidentQuery().processDefinitionId(failingProcess.ProcessDefinitionId).singleResult();
		assertNotNull(rootCauseIncident);

		Job job = managementService.createJobQuery().executionId(rootCauseIncident.ExecutionId).singleResult();
		assertNotNull(job);

		assertNotNull(rootCauseIncident.Id);
		assertNotNull(rootCauseIncident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, rootCauseIncident.IncidentType);
		assertEquals(AlwaysFailingDelegate.MESSAGE, rootCauseIncident.IncidentMessage);
		assertEquals(job.ExecutionId, rootCauseIncident.ExecutionId);
		assertEquals("theServiceTask", rootCauseIncident.ActivityId);
		assertEquals(failingProcess.Id, rootCauseIncident.ProcessInstanceId);
		assertEquals(rootCauseIncident.Id, rootCauseIncident.CauseIncidentId);
		assertEquals(rootCauseIncident.Id, rootCauseIncident.RootCauseIncidentId);
		assertEquals(job.Id, rootCauseIncident.Configuration);
		assertEquals(job.JobDefinitionId, rootCauseIncident.JobDefinitionId);

		// Cause Incident
		ProcessInstance callFailingProcess = runtimeService.createProcessInstanceQuery().processDefinitionKey("callFailingProcess").singleResult();
		assertNotNull(callFailingProcess);

		Incident causeIncident = runtimeService.createIncidentQuery().processDefinitionId(callFailingProcess.ProcessDefinitionId).singleResult();
		assertNotNull(causeIncident);

		Execution theCallActivityExecution = runtimeService.createExecutionQuery().activityId("theCallActivity").singleResult();
		assertNotNull(theCallActivityExecution);

		assertNotNull(causeIncident.Id);
		assertNotNull(causeIncident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, causeIncident.IncidentType);
		assertNull(causeIncident.IncidentMessage);
		assertEquals(theCallActivityExecution.Id, causeIncident.ExecutionId);
		assertEquals("theCallActivity", causeIncident.ActivityId);
		assertEquals(callFailingProcess.Id, causeIncident.ProcessInstanceId);
		assertEquals(rootCauseIncident.Id, causeIncident.CauseIncidentId);
		assertEquals(rootCauseIncident.Id, causeIncident.RootCauseIncidentId);
		assertNull(causeIncident.Configuration);
		assertNull(causeIncident.JobDefinitionId);

		// Top level incident of the startet process (recursive created incident for super super process instance)
		Incident topLevelIncident = runtimeService.createIncidentQuery().processDefinitionId(processInstance.ProcessDefinitionId).singleResult();
		assertNotNull(topLevelIncident);

		Execution theCallingCallActivity = runtimeService.createExecutionQuery().activityId("theCallingCallActivity").singleResult();
		assertNotNull(theCallingCallActivity);

		assertNotNull(topLevelIncident.Id);
		assertNotNull(topLevelIncident.IncidentTimestamp);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE, topLevelIncident.IncidentType);
		assertNull(topLevelIncident.IncidentMessage);
		assertEquals(theCallingCallActivity.Id, topLevelIncident.ExecutionId);
		assertEquals("theCallingCallActivity", topLevelIncident.ActivityId);
		assertEquals(processInstance.Id, topLevelIncident.ProcessInstanceId);
		assertEquals(causeIncident.Id, topLevelIncident.CauseIncidentId);
		assertEquals(rootCauseIncident.Id, topLevelIncident.RootCauseIncidentId);
		assertNull(topLevelIncident.Configuration);
		assertNull(topLevelIncident.JobDefinitionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testShouldDeleteIncidentAfterJobHasBeenDeleted()
	  {
		// start failing process
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		// get the job
		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(job);

		// there exists one incident to failed
		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(incident);

		// delete the job
		managementService.deleteJob(job.Id);

		// the incident has been deleted too.
		incident = runtimeService.createIncidentQuery().incidentId(incident.Id).singleResult();
		assertNull(incident);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldDeleteIncidentAfterJobWasSuccessfully.bpmn"})]
	  public virtual void testShouldDeleteIncidentAfterJobWasSuccessfully()
	  {
		// Start process instance
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["fail"] = true;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcessWithUserTask", parameters);

		executeAvailableJobs();

		// job exists
		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(job);

		// incident was created
		Incident incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(incident);

		// set execution variable from "true" to "false"
		runtimeService.setVariable(processInstance.Id, "fail", new bool?(false));

		// set retries of failed job to 1, with the change of the fail variable the job
		// will be executed successfully
		managementService.setJobRetries(job.Id, 1);

		executeAvailableJobs();

		// Update process instance
		processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();
		assertTrue(processInstance is ExecutionEntity);

		// should stay in the user task
		ExecutionEntity exec = (ExecutionEntity) processInstance;
		assertEquals("theUserTask", exec.ActivityId);

		// there does not exist any incident anymore
		incident = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id).singleResult();
		assertNull(incident);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateIncidentOnFailedStartTimerEvent.bpmn"})]
	  public virtual void testShouldCreateIncidentOnFailedStartTimerEvent()
	  {
		// After process start, there should be timer created
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.count());

		Job job = jobQuery.singleResult();
		string jobId = job.Id;

		while (0 != job.Retries)
		{
		  try
		  {
			managementService.executeJob(jobId);
			fail();
		  }
		  catch (Exception)
		  {
			// expected
		  }
		  job = jobQuery.jobId(jobId).singleResult();

		}

		// job exists
		job = jobQuery.singleResult();
		assertNotNull(job);

		assertEquals(0, job.Retries);

		// incident was created
		Incident incident = runtimeService.createIncidentQuery().configuration(job.Id).singleResult();
		assertNotNull(incident);

		// manually delete job for timer start event
		managementService.deleteJob(job.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testDoNotCreateNewIncident()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		IncidentQuery query = runtimeService.createIncidentQuery().processInstanceId(processInstance.Id);
		Incident incident = query.singleResult();
		assertNotNull(incident);

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// set retries to 1 by job definition id
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

		// the incident still exists
		Incident tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);

		// execute the available job (should fail again)
		executeAvailableJobs();

		// the incident still exists and there
		// should be not a new incident
		assertEquals(1, query.count());
		tmp = query.singleResult();
		assertEquals(incident.Id, tmp.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIncidentUpdateAfterCompaction()
	  public virtual void testIncidentUpdateAfterCompaction()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);
		assertNotSame(processInstanceId, incident.ExecutionId);

		runtimeService.correlateMessage("Message");

		incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);

		// incident updated with new execution id after execution tree is compacted
		assertEquals(processInstanceId, incident.ExecutionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn"})]
	  public virtual void testDoNotSetNegativeRetries()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("failingProcess");

		executeAvailableJobs();

		// it exists a job with 0 retries and an incident
		Job job = managementService.createJobQuery().singleResult();
		assertEquals(0, job.Retries);

		assertEquals(1, runtimeService.createIncidentQuery().count());

		// it should not be possible to set negative retries
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobEntity jobEntity = (org.camunda.bpm.engine.impl.persistence.entity.JobEntity) job;
		JobEntity jobEntity = (JobEntity) job;
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, jobEntity));

		assertEquals(0, job.Retries);

		// retries should still be 0 after execution this job again
		try
		{
		  managementService.executeJob(job.Id);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		job = managementService.createJobQuery().singleResult();
		assertEquals(0, job.Retries);

		// also no new incident was created
		assertEquals(1, runtimeService.createIncidentQuery().count());

		// it should not be possible to set the retries to a negative number with the management service
		try
		{
		  managementService.setJobRetries(job.Id, -200);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  managementService.setJobRetriesByJobDefinitionId(job.JobDefinitionId, -300);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly IncidentTest outerInstance;

		  private JobEntity jobEntity;

		  public CommandAnonymousInnerClass(IncidentTest outerInstance, JobEntity jobEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.jobEntity = jobEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			jobEntity.Retries = -100;
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testActivityIdProperty()
	  public virtual void testActivityIdProperty()
	  {
		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().singleResult();

		assertNotNull(incident);

		assertNotNull(incident.ActivityId);
		assertEquals("theStart", incident.ActivityId);
		assertNull(incident.ProcessInstanceId);
		assertNull(incident.ExecutionId);
	  }

	  public virtual void testBoundaryEventIncidentActivityId()
	  {
		deployment(Bpmn.createExecutableProcess("process").startEvent().userTask("userTask").endEvent().moveToActivity("userTask").boundaryEvent("boundaryEvent").timerWithDuration("PT5S").endEvent().done());

		// given
		runtimeService.startProcessInstanceByKey("process");
		Job timerJob = managementService.createJobQuery().singleResult();

		// when creating an incident
		managementService.setJobRetries(timerJob.Id, 0);

		// then
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertNotNull(incident);
		assertEquals("boundaryEvent", incident.ActivityId);
	  }

	}

}