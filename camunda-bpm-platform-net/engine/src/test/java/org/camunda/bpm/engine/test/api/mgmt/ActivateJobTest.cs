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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class ActivateJobTest : PluggableProcessEngineTestCase
	{

	  public virtual void testActivationById_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.activateJobById(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationById_shouldActivateJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activated
		managementService.activateJobById(job.Id);

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(job.Id, activeJob.Id);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByJobDefinitionId_shouldActivateJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activated
		managementService.activateJobByJobDefinitionId(jobDefinition.Id);

		// then
		// the job should be activated
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(job.Id, activeJob.Id);
		assertEquals(jobDefinition.Id, activeJob.JobDefinitionId);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivateByProcessInstanceId_shouldActivateJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activate
		managementService.activateJobByProcessInstanceId(processInstance.Id);

		// then
		// the job should be suspended
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.active().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertFalse(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionId_shouldActivateJob()
	  {
		// given
		// a deployed process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activated
		managementService.activateJobByProcessDefinitionId(processDefinition.Id);

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(job.Id, activeJob.Id);
		assertFalse(activeJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKey_shouldActivateJob()
	  {
		// given
		// a deployed process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activated
		managementService.activateJobByProcessDefinitionKey(processDefinition.Key);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.suspended().count());
		assertEquals(1, jobQuery.active().count());

		Job activeJob = jobQuery.active().singleResult();

		assertEquals(job.Id, activeJob.Id);
		assertFalse(activeJob.Suspended);
	  }

	  public virtual void testMultipleActivationByProcessDefinitionKey_shouldActivateJob()
	  {
		// given
		string key = "suspensionProcess";

		// Deploy three processes and start for each deployment a process instance
		// with a failed job
		int nrOfProcessDefinitions = 3;
		for (int i = 0; i < nrOfProcessDefinitions; i++)
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn").deploy();
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["fail"] = true;
		  runtimeService.startProcessInstanceByKey(key, @params);
		}

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey(key, true);

		// when
		// the job will be suspended
		managementService.activateJobByProcessDefinitionKey(key);

		// then
		// the job should be activated
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.suspended().count());
		assertEquals(3, jobQuery.active().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertTrue(job.Suspended);

		// when
		// the job will be activated
		managementService.updateJobSuspensionState().byJobId(job.Id).activate();

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByJobDefinitionIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.suspended().count());

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// the job will be activated
		managementService.updateJobSuspensionState().byJobDefinitionId(jobDefinition.Id).activate();

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessInstanceIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.suspended().count());

		// when
		// the job will be activated
		managementService.updateJobSuspensionState().byProcessInstanceId(processInstance.Id).activate();

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.suspended().count());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when
		// the job will be activated
		managementService.updateJobSuspensionState().byProcessDefinitionId(processDefinition.Id).activate();

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testActivationByProcessDefinitionKeyUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// suspended job definitions and corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.suspended().count());

		// when
		// the job will be activated
		managementService.updateJobSuspensionState().byProcessDefinitionKey("suspensionProcess").activate();

		// then
		// the job should be active
		assertEquals(1, jobQuery.active().count());
		assertEquals(0, jobQuery.suspended().count());
	  }

	}

}