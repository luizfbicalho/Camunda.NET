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

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MultiTenancyMigrationPlanCreateTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationPlanCreateTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException exception = org.junit.rules.ExpectedException.none();
	  public ExpectedException exception = ExpectedException.none();

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanForDefinitionsOfAuthenticatedTenant()
	  public virtual void canCreateMigrationPlanForDefinitionsOfAuthenticatedTenant()
	  {
		// given
		ProcessDefinition tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);


		// when
		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase1()
	  public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase1()
	  {
		// given
		ProcessDefinition tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_TWO));

		// then
		exception.expect(typeof(ProcessEngineException));
		exception.expectMessage("Cannot get process definition '" + tenant1Definition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.createMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).mapEqualActivities().build();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase2()
	  public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase2()
	  {
		// given
		ProcessDefinition tenant1Definition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_TWO));

		// then
		exception.expect(typeof(ProcessEngineException));
		exception.expectMessage("Cannot get process definition '" + tenant2Definition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.createMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).mapEqualActivities().build();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase3()
	  public virtual void cannotCreateMigrationPlanForDefinitionsOfNonAuthenticatedTenantsCase3()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		engineRule.IdentityService.setAuthentication("user", null, null);

		// then
		exception.expect(typeof(ProcessEngineException));
		exception.expectMessage("Cannot get process definition '" + sourceDefinition.Id + "' because it belongs to no authenticated tenant");

		// when
		engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanForSharedDefinitionsWithNoAuthenticatedTenants()
	  public virtual void canCreateMigrationPlanForSharedDefinitionsWithNoAuthenticatedTenants()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		// when
		engineRule.IdentityService.setAuthentication("user", null, null);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanWithDisabledTenantCheck()
	  public virtual void canCreateMigrationPlanWithDisabledTenantCheck()
	  {

		// given
		ProcessDefinition tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		// when
		engineRule.IdentityService.setAuthentication("user", null, null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);

	  }
	}

}