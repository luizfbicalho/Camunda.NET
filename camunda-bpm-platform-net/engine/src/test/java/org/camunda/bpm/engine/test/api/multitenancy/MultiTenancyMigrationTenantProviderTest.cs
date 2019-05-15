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

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MultiTenancyMigrationTenantProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationTenantProviderTest()
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
			tenantRuleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule()
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {

		  TenantIdProvider tenantIdProvider = new VariableBasedTenantIdProvider();
		  configuration.TenantIdProvider = tenantIdProvider;

		  return configuration;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain tenantRuleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain tenantRuleChain;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotMigrateInstanceBetweenDifferentTenants()
	  public virtual void cannotMigrateInstanceBetweenDifferentTenants()
	  {
		// given
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenantDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = startInstanceForTenant(sharedDefinition, TENANT_ONE);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sharedDefinition.Id, tenantDefinition.Id).mapEqualActivities().build();

		// when
		try
		{
		  engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot migrate process instance '" + processInstance.Id + "' " + "to a process definition of a different tenant ('tenant1' != 'tenant2')"));
		}

		// then
		Assert.assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateInstanceBetweenSameTenantCase2()
	  public virtual void canMigrateInstanceBetweenSameTenantCase2()
	  {
		// given
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = startInstanceForTenant(sharedDefinition, TENANT_ONE);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sharedDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// when
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		assertInstanceOfDefinition(processInstance, targetDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenant()
	  public virtual void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenant()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = startInstanceForTenant(sourceDefinition, TENANT_ONE);
		ProcessInstance processInstance2 = startInstanceForTenant(sourceDefinition, TENANT_TWO);

		// when
		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceQuery(engineRule.RuntimeService.createProcessInstanceQuery()).execute();
		engineRule.IdentityService.clearAuthentication();

		// then
		assertInstanceOfDefinition(processInstance1, targetDefinition);
		assertInstanceOfDefinition(processInstance2, sourceDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenants()
	  public virtual void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenants()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = startInstanceForTenant(sourceDefinition, TENANT_ONE);
		ProcessInstance processInstance2 = startInstanceForTenant(sourceDefinition, TENANT_TWO);

		// when
		engineRule.IdentityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceQuery(engineRule.RuntimeService.createProcessInstanceQuery()).execute();
		engineRule.IdentityService.clearAuthentication();

		// then
		assertInstanceOfDefinition(processInstance1, targetDefinition);
		assertInstanceOfDefinition(processInstance2, targetDefinition);
	  }

	  protected internal virtual void assertInstanceOfDefinition(ProcessInstance processInstance, ProcessDefinition targetDefinition)
	  {
		Assert.assertEquals(1, engineRule.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).processDefinitionId(targetDefinition.Id).count());
	  }

	  protected internal virtual ProcessInstance startInstanceForTenant(ProcessDefinition processDefinition, string tenantId)
	  {
		return engineRule.RuntimeService.startProcessInstanceById(processDefinition.Id, Variables.createVariables().putValue(VariableBasedTenantIdProvider.TENANT_VARIABLE, tenantId));
	  }

	  public class VariableBasedTenantIdProvider : TenantIdProvider
	  {
		public const string TENANT_VARIABLE = "tenantId";

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return (string) ctx.Variables.get(TENANT_VARIABLE);
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return (string) ctx.Variables.get(TENANT_VARIABLE);
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}
	  }
	}

}