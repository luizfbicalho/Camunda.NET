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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
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

	public class MultiTenancyProcessInstanceSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessInstanceSuspensionStateTest()
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

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().parallelGateway("fork").userTask().moveToLastGateway().sendTask().camundaType("external").camundaTopic("test").boundaryEvent().timerWithDuration("PT1M").done();

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
//ORIGINAL LINE: @Test public void suspendAndActivateProcessInstancesForAllTenants()
	  public virtual void suspendAndActivateProcessInstancesForAllTenants()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceForTenant()
	  public virtual void suspendProcessInstanceForTenant()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceForNonTenant()
	  public virtual void suspendProcessInstanceForNonTenant()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceForTenant()
	  public virtual void activateProcessInstanceForTenant()
	  {
		// given suspended process instances
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceForNonTenant()
	  public virtual void activateProcessInstanceForNonTenant()
	  {
		// given suspended process instances
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateProcessInstancesIncludingUserTasksForAllTenants()
	  public virtual void suspendAndActivateProcessInstancesIncludingUserTasksForAllTenants()
	  {
		// given activated user tasks
		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingUserTaskForTenant()
	  public virtual void suspendProcessInstanceIncludingUserTaskForTenant()
	  {
		// given activated user tasks
		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingUserTaskForNonTenant()
	  public virtual void suspendProcessInstanceIncludingUserTaskForNonTenant()
	  {
		// given activated user tasks
		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingUserTaskForTenant()
	  public virtual void activateProcessInstanceIncludingUserTaskForTenant()
	  {
		// given suspended user tasks
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingUserTaskForNonTenant()
	  public virtual void activateProcessInstanceIncludingUserTaskForNonTenant()
	  {
		// given suspended user tasks
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateProcessInstancesIncludingExternalTasksForAllTenants()
	  public virtual void suspendAndActivateProcessInstancesIncludingExternalTasksForAllTenants()
	  {
		// given activated external tasks
		ExternalTaskQuery query = engineRule.ExternalTaskService.createExternalTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingExternalTaskForTenant()
	  public virtual void suspendProcessInstanceIncludingExternalTaskForTenant()
	  {
		// given activated external tasks
		ExternalTaskQuery query = engineRule.ExternalTaskService.createExternalTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingExternalTaskForNonTenant()
	  public virtual void suspendProcessInstanceIncludingExternalTaskForNonTenant()
	  {
		// given activated external tasks
		ExternalTaskQuery query = engineRule.ExternalTaskService.createExternalTaskQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingExternalTaskForTenant()
	  public virtual void activateProcessInstanceIncludingExternalTaskForTenant()
	  {
		// given suspended external tasks
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ExternalTaskQuery query = engineRule.ExternalTaskService.createExternalTaskQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingExternalTaskForNonTenant()
	  public virtual void activateProcessInstanceIncludingExternalTaskForNonTenant()
	  {
		// given suspended external tasks
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		ExternalTaskQuery query = engineRule.ExternalTaskService.createExternalTaskQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendAndActivateProcessInstancesIncludingJobsForAllTenants()
	  public virtual void suspendAndActivateProcessInstancesIncludingJobsForAllTenants()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingJobForTenant()
	  public virtual void suspendProcessInstanceIncludingJobForTenant()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceIncludingJobForNonTenant()
	  public virtual void suspendProcessInstanceIncludingJobForNonTenant()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingJobForTenant()
	  public virtual void activateProcessInstanceIncludingJobForTenant()
	  {
		// given suspended job
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessInstanceIncludingJobForNonTenant()
	  public virtual void activateProcessInstanceIncludingJobForNonTenant()
	  {
		// given suspended jobs
		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceNoAuthenticatedTenants()
	  public virtual void suspendProcessInstanceNoAuthenticatedTenants()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToSuspendProcessInstanceByProcessDefinitionIdNoAuthenticatedTenants()
	  public virtual void failToSuspendProcessInstanceByProcessDefinitionIdNoAuthenticatedTenants()
	  {
		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(PROCESS_DEFINITION_KEY).tenantIdIn(TENANT_ONE).singleResult();

		// declare expected exception
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process definition '" + processDefinition.Id + "' because it belongs to no authenticated tenant");

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceWithAuthenticatedTenant()
	  public virtual void suspendProcessInstanceWithAuthenticatedTenant()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(1L));
		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessInstanceDisabledTenantCheck()
	  public virtual void suspendProcessInstanceDisabledTenantCheck()
	  {
		// given activated process instances
		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(2L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }
	}

}