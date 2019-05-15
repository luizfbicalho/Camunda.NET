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
namespace org.camunda.bpm.engine.test.api.multitenancy.suspensionstate
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateJobDefinitionHandler;
	using TimerSuspendJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendJobDefinitionHandler;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyJobDefinitionSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobDefinitionSuspensionStateTest()
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
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().camundaAsyncBefore().endEvent().done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {

		testRule.deployForTenant(TENANT_ONE, PROCESS);
		testRule.deployForTenant(TENANT_TWO, PROCESS);
		testRule.deploy(PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_TWO).execute();
		engineRule.RuntimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateJobDefinitionsForAllTenants()
	  public virtual void suspendAndActivateJobDefinitionsForAllTenants()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionForTenant()
	  public virtual void suspendJobDefinitionForTenant()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionForNonTenant()
	  public virtual void suspendJobDefinitionForNonTenant()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobDefinitionForTenant()
	  public virtual void activateJobDefinitionForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void jobProcessDefinitionForNonTenant()
	  public virtual void jobProcessDefinitionForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateJobDefinitionsIncludingJobsForAllTenants()
	  public virtual void suspendAndActivateJobDefinitionsIncludingJobsForAllTenants()
	  {
		// given activated job definitions
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeJobs(true).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeJobs(true).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionIncludingJobsForTenant()
	  public virtual void suspendJobDefinitionIncludingJobsForTenant()
	  {
		// given activated job definitions
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).includeJobs(true).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionIncludingJobsForNonTenant()
	  public virtual void suspendJobDefinitionIncludingJobsForNonTenant()
	  {
		// given activated job definitions
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().includeJobs(true).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobDefinitionIncludingJobsForTenant()
	  public virtual void activateJobDefinitionIncludingJobsForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeJobs(true).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).includeJobs(true).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobDefinitionIncludingJobsForNonTenant()
	  public virtual void activateJobDefinitionIncludingJobsForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeJobs(true).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().includeJobs(true).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendJobDefinitionsForAllTenants()
	  public virtual void delayedSuspendJobDefinitionsForAllTenants()
	  {
		// given activated job definitions

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).executionDate(tomorrow()).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendJobDefinitionsForTenant()
	  public virtual void delayedSuspendJobDefinitionsForTenant()
	  {
		// given activated job definitions

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).executionDate(tomorrow()).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendJobDefinitionsForNonTenant()
	  public virtual void delayedSuspendJobDefinitionsForNonTenant()
	  {
		// given activated job definitions

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().executionDate(tomorrow()).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateJobDefinitionsForAllTenants()
	  public virtual void delayedActivateJobDefinitionsForAllTenants()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).executionDate(tomorrow()).activate();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// when execute the job to activate the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(0L));
		assertThat(query.active().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateJobDefinitionsForTenant()
	  public virtual void delayedActivateJobDefinitionsForTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).executionDate(tomorrow()).activate();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// when execute the job to activate the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateJobDefinitionsForNonTenant()
	  public virtual void delayedActivateJobDefinitionsForNonTenant()
	  {
		// given suspend job definitions
		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().executionDate(tomorrow()).activate();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// when execute the job to activate the job definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionNoAuthenticatedTenants()
	  public virtual void suspendJobDefinitionNoAuthenticatedTenants()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionWithAuthenticatedTenant()
	  public virtual void suspendJobDefinitionWithAuthenticatedTenant()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(1L));
		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDefinitionDisabledTenantCheck()
	  public virtual void suspendJobDefinitionDisabledTenantCheck()
	  {
		// given activated job definitions
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE, TENANT_TWO).includeJobDefinitionsWithoutTenantId().count(), @is(3L));
	  }

	  protected internal virtual DateTime tomorrow()
	  {
		DateTime calendar = new DateTime();
		calendar.AddDays(1);
		return calendar;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly MultiTenancyJobDefinitionSuspensionStateTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyJobDefinitionSuspensionStateTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateJobDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	}

}