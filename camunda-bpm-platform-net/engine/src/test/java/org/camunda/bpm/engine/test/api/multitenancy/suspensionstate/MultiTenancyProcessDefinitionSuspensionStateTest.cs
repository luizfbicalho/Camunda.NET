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


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyProcessDefinitionSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionSuspensionStateTest()
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
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

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
//ORIGINAL LINE: @Test public void suspendAndActivateProcessDefinitionsForAllTenants()
	  public virtual void suspendAndActivateProcessDefinitionsForAllTenants()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionForTenant()
	  public virtual void suspendProcessDefinitionForTenant()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionForNonTenant()
	  public virtual void suspendProcessDefinitionForNonTenant()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionForTenant()
	  public virtual void activateProcessDefinitionForTenant()
	  {
		// given suspend process definitions
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionForNonTenant()
	  public virtual void activateProcessDefinitionForNonTenant()
	  {
		// given suspend process definitions
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateProcessDefinitionsIncludeInstancesForAllTenants()
	  public virtual void suspendAndActivateProcessDefinitionsIncludeInstancesForAllTenants()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeProcessInstances(true).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeProcessInstances(true).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionIncludeInstancesForTenant()
	  public virtual void suspendProcessDefinitionIncludeInstancesForTenant()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).includeProcessInstances(true).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionIncludeInstancesForNonTenant()
	  public virtual void suspendProcessDefinitionIncludeInstancesForNonTenant()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().includeProcessInstances(true).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionIncludeInstancesForTenant()
	  public virtual void activateProcessDefinitionIncludeInstancesForTenant()
	  {
		// given suspended process instances
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeProcessInstances(true).suspend();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).includeProcessInstances(true).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionIncludeInstancesForNonTenant()
	  public virtual void activateProcessDefinitionIncludeInstancesForNonTenant()
	  {
		// given suspended process instances
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).includeProcessInstances(true).suspend();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().includeProcessInstances(true).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendProcessDefinitionsForAllTenants()
	  public virtual void delayedSuspendProcessDefinitionsForAllTenants()
	  {
		// given activated process definitions

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).executionDate(tomorrow()).suspend();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the process definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendProcessDefinitionsForTenant()
	  public virtual void delayedSuspendProcessDefinitionsForTenant()
	  {
		// given activated process definitions

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).executionDate(tomorrow()).suspend();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the process definition
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedSuspendProcessDefinitionsForNonTenant()
	  public virtual void delayedSuspendProcessDefinitionsForNonTenant()
	  {
		// given activated process definitions

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().executionDate(tomorrow()).suspend();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// when execute the job to suspend the process definition
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateProcessDefinitionsForAllTenants()
	  public virtual void delayedActivateProcessDefinitionsForAllTenants()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).executionDate(tomorrow()).activate();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		// when execute the job to activate the process definitions
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(0L));
		assertThat(query.active().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateProcessDefinitionsForTenant()
	  public virtual void delayedActivateProcessDefinitionsForTenant()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).executionDate(tomorrow()).activate();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		// when execute the job to activate the process definition
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void delayedActivateProcessDefinitionsForNonTenant()
	  public virtual void delayedActivateProcessDefinitionsForNonTenant()
	  {
		// given suspended process definitions
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().executionDate(tomorrow()).activate();

		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		// when execute the job to activate the process definition
		Job job = engineRule.ManagementService.createJobQuery().timers().singleResult();
		assertThat(job, @is(notNullValue()));

		engineRule.ManagementService.executeJob(job.Id);

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  public virtual void suspendProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  {
		// given activated jobs
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionIncludingJobDefinitionsForTenant()
	  public virtual void suspendProcessDefinitionIncludingJobDefinitionsForTenant()
	  {
		// given activated jobs
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  public virtual void suspendProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  {
		// given activated jobs
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  public virtual void activateProcessDefinitionIncludingJobDefinitionsForAllTenants()
	  {
		// given suspended jobs
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.suspended().count(), @is(0L));
		assertThat(query.active().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionIncludingJobDefinitionsForTenant()
	  public virtual void activateProcessDefinitionIncludingJobDefinitionsForTenant()
	  {
		// given suspended jobs
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  public virtual void activateProcessDefinitionIncludingJobDefinitionsForNonTenant()
	  {
		// given suspended jobs
		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionNoAuthenticatedTenants()
	  public virtual void suspendProcessDefinitionNoAuthenticatedTenants()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToSuspendProcessDefinitionByIdNoAuthenticatedTenants()
	  public virtual void failToSuspendProcessDefinitionByIdNoAuthenticatedTenants()
	  {
		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).tenantIdIn(TENANT_ONE).singleResult();

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + processDefinition.Id + "' because it belongs to no authenticated tenant");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionWithAuthenticatedTenant()
	  public virtual void suspendProcessDefinitionWithAuthenticatedTenant()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(1L));
		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionDisabledTenantCheck()
	  public virtual void suspendProcessDefinitionDisabledTenantCheck()
	  {
		// given activated process definitions
		ProcessDefinitionQuery query = engineRule.RepositoryService.createProcessDefinitionQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		processEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE, TENANT_TWO).includeProcessDefinitionsWithoutTenantId().count(), @is(3L));
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
		  private readonly MultiTenancyProcessDefinitionSuspensionStateTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyProcessDefinitionSuspensionStateTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateProcessDefinitionHandler.TYPE);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

	}

}