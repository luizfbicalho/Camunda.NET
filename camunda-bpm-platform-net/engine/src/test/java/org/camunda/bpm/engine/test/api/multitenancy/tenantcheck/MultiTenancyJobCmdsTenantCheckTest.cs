using System;

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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyJobCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobCmdsTenantCheckTest()
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


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "exceptionInJobExecution";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal ProcessInstance processInstance;

	  protected internal ManagementService managementService;

	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal static readonly BpmnModelInstance BPMN_PROCESS = Bpmn.createExecutableProcess("exceptionInJobExecution").startEvent().userTask("aUserTask").boundaryEvent("timerEvent").timerWithDuration("PT4H").serviceTask().camundaExpression("${failing}").endEvent().done();

	  internal BpmnModelInstance BPMN_NO_FAIL_PROCESS = Bpmn.createExecutableProcess("noFail").startEvent().userTask("aUserTask").boundaryEvent("timerEvent").timerWithDuration("PT4H").endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		managementService = engineRule.ManagementService;

		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.deployForTenant(TENANT_ONE, BPMN_NO_FAIL_PROCESS);

		processInstance = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
	  }

	  // set jobRetries
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithAuthenticatedTenant()
	  public virtual void testSetJobRetriesWithAuthenticatedTenant()
	  {

		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.setJobRetries(timerJob.Id, 5);

		assertEquals(5, managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult().Retries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithNoAuthenticatedTenant()
	  public virtual void testSetJobRetriesWithNoAuthenticatedTenant()
	  {

		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");
		managementService.setJobRetries(timerJob.Id, 5);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesWithDisabledTenantCheck()
	  public virtual void testSetJobRetriesWithDisabledTenantCheck()
	  {

		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.setJobRetries(timerJob.Id, 5);

		// then
		assertEquals(5, managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult().Retries);

	  }

	  // set Jobretries based on job definition
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithAuthenticatedTenant()
	  public virtual void testSetJobRetriesDefinitionWithAuthenticatedTenant()
	  {

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.setJobRetries(jobId, 0);

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		// sets the retries for failed jobs - That's the reason why job retries are made 0 in the above step
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);

		// then
		assertEquals(1, selectJobByProcessInstanceId(processInstance.Id).Retries);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithNoAuthenticatedTenant()
	  public virtual void testSetJobRetriesDefinitionWithNoAuthenticatedTenant()
	  {

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.setJobRetries(jobId, 0);
		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");
		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesDefinitionWithDisabledTenantCheck()
	  public virtual void testSetJobRetriesDefinitionWithDisabledTenantCheck()
	  {

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		managementService.setJobRetries(jobId, 0);
		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);
		// then
		assertEquals(1, selectJobByProcessInstanceId(processInstance.Id).Retries);

	  }

	  // set JobDueDate
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithAuthenticatedTenant()
	  public virtual void testSetJobDueDateWithAuthenticatedTenant()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertEquals(0, managementService.createJobQuery().duedateLowerThan(DateTime.Now).count());

		DateTime cal = new DateTime();
		cal = new DateTime(DateTime.Now);
		cal.AddDays(-3);

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.setJobDuedate(timerJob.Id, cal);

		// then
		assertEquals(1, managementService.createJobQuery().duedateLowerThan(DateTime.Now).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithNoAuthenticatedTenant()
	  public virtual void testSetJobDueDateWithNoAuthenticatedTenant()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");
		// when
		managementService.setJobDuedate(timerJob.Id, DateTime.Now);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDueDateWithDisabledTenantCheck()
	  public virtual void testSetJobDueDateWithDisabledTenantCheck()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		DateTime cal = new DateTime();
		cal = new DateTime(DateTime.Now);
		cal.AddDays(-3);

		managementService.setJobDuedate(timerJob.Id, cal);
		// then
		assertEquals(1, managementService.createJobQuery().duedateLowerThan(DateTime.Now).count());

	  }

	  // set jobPriority test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithAuthenticatedTenant()
	  public virtual void testSetJobPriorityWithAuthenticatedTenant()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.setJobPriority(timerJob.Id, 5);

		// then
		assertEquals(1, managementService.createJobQuery().priorityHigherThanOrEquals(5).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithNoAuthenticatedTenant()
	  public virtual void testSetJobPriorityWithNoAuthenticatedTenant()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the job '" + timerJob.Id + "' because it belongs to no authenticated tenant.");

		// when
		managementService.setJobPriority(timerJob.Id, 5);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityWithDisabledTenantCheck()
	  public virtual void testSetJobPriorityWithDisabledTenantCheck()
	  {
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.setJobPriority(timerJob.Id, 5);
		// then
		assertEquals(1, managementService.createJobQuery().priorityHigherThanOrEquals(5).count());
	  }

	  // setOverridingJobPriorityForJobDefinition without cascade
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithAuthenticatedTenant()
	  public virtual void testSetOverridingJobPriorityWithAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, @is(1701L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithNoAuthenticatedTenant()
	  public virtual void testSetOverridingJobPriorityWithNoAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithDisabledTenantCheck()
	  public virtual void testSetOverridingJobPriorityWithDisabledTenantCheck()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);
		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, @is(1701L));
	  }

	  // setOverridingJobPriority with cascade
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndAuthenticatedTenant()
	  public virtual void testSetOverridingJobPriorityWithCascadeAndAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);

		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, @is(1701L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndNoAuthenticatedTenant()
	  public virtual void testSetOverridingJobPriorityWithCascadeAndNoAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetOverridingJobPriorityWithCascadeAndDisabledTenantCheck()
	  public virtual void testSetOverridingJobPriorityWithCascadeAndDisabledTenantCheck()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701, true);
		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, @is(1701L));
	  }

	  // clearOverridingJobPriorityForJobDefinition
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithAuthenticatedTenant()
	  public virtual void testClearOverridingJobPriorityWithAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, @is(1701L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, nullValue());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithNoAuthenticatedTenant()
	  public virtual void testClearOverridingJobPriorityWithNoAuthenticatedTenant()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + jobDefinition.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClearOverridingJobPriorityWithDisabledTenantCheck()
	  public virtual void testClearOverridingJobPriorityWithDisabledTenantCheck()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().list().get(0);

		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 1701);

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);
		// then
		assertThat(managementService.createJobDefinitionQuery().jobDefinitionId(jobDefinition.Id).singleResult().OverridingJobPriority, nullValue());
	  }

	  // getJobExceptionStackTrace
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithAuthenticatedTenant()
	  public virtual void testGetJobExceptionStackTraceWithAuthenticatedTenant()
	  {

		string processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.executeAvailableJobs();

		string timerJobId = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		assertThat(managementService.getJobExceptionStacktrace(timerJobId), notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithNoAuthenticatedTenant()
	  public virtual void testGetJobExceptionStackTraceWithNoAuthenticatedTenant()
	  {

		string processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.executeAvailableJobs();

		string timerJobId = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the job '" + timerJobId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.getJobExceptionStacktrace(timerJobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetJobExceptionStackTraceWithDisabledTenantCheck()
	  public virtual void testGetJobExceptionStackTraceWithDisabledTenantCheck()
	  {

		string processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

		testRule.executeAvailableJobs();

		string timerJobId = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		managementService.getJobExceptionStacktrace(timerJobId);
		assertThat(managementService.getJobExceptionStacktrace(timerJobId), notNullValue());
	  }

	  // deleteJobs
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithAuthenticatedTenant()
	  public virtual void testDeleteJobWithAuthenticatedTenant()
	  {
		string timerJobId = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult().Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.deleteJob(timerJobId);

		// then
		assertEquals(0, managementService.createJobQuery().processInstanceId(processInstance.Id).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithNoAuthenticatedTenant()
	  public virtual void testDeleteJobWithNoAuthenticatedTenant()
	  {
		string timerJobId = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the job '" + timerJobId + "' because it belongs to no authenticated tenant.");

		// when
		managementService.deleteJob(timerJobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobWithDisabledTenantCheck()
	  public virtual void testDeleteJobWithDisabledTenantCheck()
	  {
		string timerJobId = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.deleteJob(timerJobId);

		// then
		assertEquals(0, managementService.createJobQuery().processInstanceId(processInstance.Id).count());
	  }

	  //executeJobs
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithAuthenticatedTenant()
	  public virtual void testExecuteJobWithAuthenticatedTenant()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.startProcessInstanceByKey("noFail").Id;

		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery().processInstanceId(noFailProcessInstanceId);

		assertEquals(1, taskQuery.list().size());

		string timerJobId = managementService.createJobQuery().processInstanceId(noFailProcessInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));
		managementService.executeJob(timerJobId);

		// then
		assertEquals(0, taskQuery.list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithNoAuthenticatedTenant()
	  public virtual void testExecuteJobWithNoAuthenticatedTenant()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.startProcessInstanceByKey("noFail").Id;

		string timerJobId = managementService.createJobQuery().processInstanceId(noFailProcessInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the job '" + timerJobId + "' because it belongs to no authenticated tenant.");
		managementService.executeJob(timerJobId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobWithDisabledTenantCheck()
	  public virtual void testExecuteJobWithDisabledTenantCheck()
	  {

		string noFailProcessInstanceId = engineRule.RuntimeService.startProcessInstanceByKey("noFail").Id;

		string timerJobId = managementService.createJobQuery().processInstanceId(noFailProcessInstanceId).singleResult().Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		managementService.executeJob(timerJobId);

		TaskQuery taskQuery = engineRule.TaskService.createTaskQuery().processInstanceId(noFailProcessInstanceId);

		// then
		assertEquals(0, taskQuery.list().size());
	  }

	  protected internal virtual Job selectJobByProcessInstanceId(string processInstanceId)
	  {
		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		return job;
	  }
	}

}