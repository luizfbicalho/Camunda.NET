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
//	import static org.junit.Assert.assertThat;

	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyJobSuspensionStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobSuspensionStateTest()
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

	  protected internal static readonly BpmnModelInstance PROCESS = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().camundaAsyncBefore().endEvent().done();

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
//ORIGINAL LINE: @Test public void suspendAndActivateJobsForAllTenants()
	  public virtual void suspendAndActivateJobsForAllTenants()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		// first suspend
		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));

		// then activate
		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).activate();

		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobForTenant()
	  public virtual void suspendJobForTenant()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobsForNonTenant()
	  public virtual void suspendJobsForNonTenant()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().suspend();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobsForTenant()
	  public virtual void activateJobsForTenant()
	  {
		// given suspend jobs
		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionTenantId(TENANT_ONE).activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateJobsForNonTenant()
	  public virtual void activateJobsForNonTenant()
	  {
		// given suspend jobs
		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.active().count(), @is(0L));

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).processDefinitionWithoutTenantId().activate();

		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().count(), @is(1L));
		assertThat(query.active().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobNoAuthenticatedTenants()
	  public virtual void suspendJobNoAuthenticatedTenants()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(2L));
		assertThat(query.suspended().count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobWithAuthenticatedTenant()
	  public virtual void suspendJobWithAuthenticatedTenant()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		engineRule.IdentityService.clearAuthentication();

		assertThat(query.active().count(), @is(1L));
		assertThat(query.suspended().count(), @is(2L));
		assertThat(query.active().tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.suspended().withoutTenantId().count(), @is(1L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendJobDisabledTenantCheck()
	  public virtual void suspendJobDisabledTenantCheck()
	  {
		// given activated jobs
		JobQuery query = engineRule.ManagementService.createJobQuery();
		assertThat(query.active().count(), @is(3L));
		assertThat(query.suspended().count(), @is(0L));

		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		engineRule.IdentityService.setAuthentication("user", null, null);

		engineRule.ManagementService.updateJobSuspensionState().byProcessDefinitionKey(PROCESS_DEFINITION_KEY).suspend();

		assertThat(query.active().count(), @is(0L));
		assertThat(query.suspended().count(), @is(3L));
		assertThat(query.suspended().tenantIdIn(TENANT_ONE, TENANT_TWO).includeJobsWithoutTenantId().count(), @is(3L));
	  }

	}

}