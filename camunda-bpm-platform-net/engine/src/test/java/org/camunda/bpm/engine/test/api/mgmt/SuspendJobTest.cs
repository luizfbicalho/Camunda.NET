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
	public class SuspendJobTest : PluggableProcessEngineTestCase
	{

	  public virtual void testSuspensionById_shouldThrowProcessEngineException()
	  {
		try
		{
		  managementService.suspendJobById(null);
		  fail("A ProcessEngineExcpetion was expected.");
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionById_shouldSuspendJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.suspendJobById(job.Id);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByJobDefinitionId_shouldSuspendJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// the job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.suspendJobByJobDefinitionId(jobDefinition.Id);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessInstanceId_shouldSuspendJob()
	  {
		// given

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// the job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.suspendJobByProcessInstanceId(processInstance.Id);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertEquals(jobDefinition.Id, suspendedJob.JobDefinitionId);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionId_shouldSuspendJob()
	  {
		// given
		// a deployed process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.suspendJobByProcessDefinitionId(processDefinition.Id);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertTrue(suspendedJob.Suspended);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKey_shouldSuspendJob()
	  {
		// given
		// a deployed process definition
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// a running process instance with a failed job
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey("suspensionProcess", @params);

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.suspendJobByProcessDefinitionKey(processDefinition.Key);

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());

		Job suspendedJob = jobQuery.suspended().singleResult();

		assertEquals(job.Id, suspendedJob.Id);
		assertTrue(suspendedJob.Suspended);
	  }

	  public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldSuspendJob()
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

		// when
		// the job will be suspended
		managementService.suspendJobByProcessDefinitionKey(key);

		// then
		// the job should be suspended
		JobQuery jobQuery = managementService.createJobQuery();

		assertEquals(0, jobQuery.active().count());
		assertEquals(3, jobQuery.suspended().count());

		// Clean DB
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		Job job = jobQuery.singleResult();
		assertFalse(job.Suspended);

		// when
		// the job will be suspended
		managementService.updateJobSuspensionState().byJobId(job.Id).suspend();

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByJobDefinitionIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.active().count());

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when
		// the job will be suspended
		managementService.updateJobSuspensionState().byJobDefinitionId(jobDefinition.Id).suspend();

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessInstanceIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.active().count());

		// when
		// the job will be suspended
		managementService.updateJobSuspensionState().byProcessInstanceId(processInstance.Id).suspend();

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionIdUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.active().count());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when
		// the job will be suspended
		managementService.updateJobSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/SuspensionTest.testBase.bpmn"})]
	  public virtual void testSuspensionByProcessDefinitionKeyUsingBuilder()
	  {
		// given

		// a running process instance with a failed job
		runtimeService.startProcessInstanceByKey("suspensionProcess", Variables.createVariables().putValue("fail", true));

		// the failed job
		JobQuery jobQuery = managementService.createJobQuery();
		assertEquals(1, jobQuery.active().count());

		// when
		// the job will be suspended
		managementService.updateJobSuspensionState().byProcessDefinitionKey("suspensionProcess").suspend();

		// then
		// the job should be suspended
		assertEquals(0, jobQuery.active().count());
		assertEquals(1, jobQuery.suspended().count());
	  }

	}

}