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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyTimerStartEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTimerStartEventTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess().startEvent().timerWithDuration("PT1M").userTask().endEvent().done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ManagementService managementService;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startProcessInstanceWithTenantId()
	  public virtual void startProcessInstanceWithTenantId()
	  {

		testRule.deployForTenant(TENANT_ONE, PROCESS);

		Job job = managementService.createJobQuery().singleResult();
		assertThat(job.TenantId, @is(TENANT_ONE));

		managementService.executeJob(job.Id);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance, @is(notNullValue()));
		assertThat(processInstance.TenantId, @is(TENANT_ONE));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void startProcessInstanceTwoTenants()
	  public virtual void startProcessInstanceTwoTenants()
	  {

		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);

		Job jobForTenantOne = managementService.createJobQuery().tenantIdIn(TENANT_ONE).singleResult();
		assertThat(jobForTenantOne, @is(notNullValue()));
		managementService.executeJob(jobForTenantOne.Id);

		Job jobForTenantTwo = managementService.createJobQuery().tenantIdIn(TENANT_TWO).singleResult();
		assertThat(jobForTenantTwo, @is(notNullValue()));
		managementService.executeJob(jobForTenantTwo.Id);

		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(runtimeService.createProcessInstanceQuery().tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteJobsWhileUndeployment()
	  public virtual void deleteJobsWhileUndeployment()
	  {

		 Deployment deploymentForTenantOne = testRule.deployForTenant(TENANT_ONE, PROCESS);
		 Deployment deploymentForTenantTwo = testRule.deployForTenant(TENANT_TWO, PROCESS);

		 JobQuery query = managementService.createJobQuery();
		 assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		 assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));

		 repositoryService.deleteDeployment(deploymentForTenantOne.Id, true);

		 assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
		 assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));

		 repositoryService.deleteDeployment(deploymentForTenantTwo.Id, true);

		 assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
		 assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void dontCreateNewJobsWhileReDeployment()
	  public virtual void dontCreateNewJobsWhileReDeployment()
	  {

		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);
		testRule.deployForTenant(TENANT_ONE, PROCESS);

		JobQuery query = managementService.createJobQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failedJobRetryTimeCycle()
	  public virtual void failedJobRetryTimeCycle()
	  {

		testRule.deployForTenant(TENANT_ONE, Bpmn.createExecutableProcess("failingProcess").startEvent().timerWithDuration("PT1M").camundaFailedJobRetryTimeCycle("R5/PT1M").serviceTask().camundaExpression("${failing}").endEvent().done());

		testRule.deployForTenant(TENANT_TWO, Bpmn.createExecutableProcess("failingProcess").startEvent().timerWithDuration("PT1M").camundaFailedJobRetryTimeCycle("R4/PT1M").serviceTask().camundaExpression("${failing}").endEvent().done());

		IList<Job> jobs = managementService.createJobQuery().timers().list();
		executeFailingJobs(jobs);

		Job jobTenantOne = managementService.createJobQuery().tenantIdIn(TENANT_ONE).singleResult();
		Job jobTenantTwo = managementService.createJobQuery().tenantIdIn(TENANT_TWO).singleResult();

		assertThat(jobTenantOne.Retries, @is(4));
		assertThat(jobTenantTwo.Retries, @is(3));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void timerStartEventWithTimerCycle()
	  public virtual void timerStartEventWithTimerCycle()
	  {

		testRule.deployForTenant(TENANT_ONE, Bpmn.createExecutableProcess().startEvent().timerWithCycle("R2/PT1M").userTask().endEvent().done());

		// execute first timer cycle
		Job job = managementService.createJobQuery().singleResult();
		assertThat(job.TenantId, @is(TENANT_ONE));
		managementService.executeJob(job.Id);

		// execute second timer cycle
		job = managementService.createJobQuery().singleResult();
		assertThat(job.TenantId, @is(TENANT_ONE));
		managementService.executeJob(job.Id);

		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.withoutTenantId().count(), @is(0L));
	  }

	  protected internal virtual void executeFailingJobs(IList<Job> jobs)
	  {
		foreach (Job job in jobs)
		{

		  try
		  {
			managementService.executeJob(job.Id);

			fail("expected exception");
		  }
		  catch (Exception)
		  {
		  }
		}
	  }

	}

}