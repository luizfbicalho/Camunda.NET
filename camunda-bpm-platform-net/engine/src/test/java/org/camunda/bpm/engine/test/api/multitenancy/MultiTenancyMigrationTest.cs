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

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MultiTenancyMigrationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationTest()
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

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateMigrationPlanBetweenDifferentTenants()
	  public virtual void cannotCreateMigrationPlanBetweenDifferentTenants()
	  {
		// given
		ProcessDefinition tenant1Definition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenant2Definition = testHelper.deployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.ONE_TASK_PROCESS);

		// when
		try
		{
		  engineRule.RuntimeService.createMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).mapEqualActivities().build();
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot migrate process instances between processes of different tenants ('tenant1' != 'tenant2')"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanFromTenantToNoTenant()
	  public virtual void canCreateMigrationPlanFromTenantToNoTenant()
	  {
		// given
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenantDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);


		// when
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(tenantDefinition.Id, sharedDefinition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanFromNoTenantToTenant()
	  public virtual void canCreateMigrationPlanFromNoTenantToTenant()
	  {
		// given
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition tenantDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);


		// when
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sharedDefinition.Id, tenantDefinition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canCreateMigrationPlanForNoTenants()
	  public virtual void canCreateMigrationPlanForNoTenants()
	  {
		// given
		ProcessDefinition sharedDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);


		// when
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sharedDefinition.Id, sharedDefinition.Id).mapEqualActivities().build();

		// then
		Assert.assertNotNull(migrationPlan);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateInstanceBetweenSameTenantCase1()
	  public virtual void canMigrateInstanceBetweenSameTenantCase1()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// when
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		assertMigratedTo(processInstance, targetDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  public virtual void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// when
		try
		{
		  engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();
		  Assert.fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("Cannot migrate process instance '" + processInstance.Id + "' without tenant to a process definition with a tenant ('tenant1')"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateInstanceWithTenantIdToDefinitionWithoutTenantId()
	  public virtual void canMigrateInstanceWithTenantIdToDefinitionWithoutTenantId()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// when
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		assertMigratedTo(processInstance, targetDefinition);
	  }

	  protected internal virtual void assertMigratedTo(ProcessInstance processInstance, ProcessDefinition targetDefinition)
	  {
		Assert.assertEquals(1, engineRule.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).processDefinitionId(targetDefinition.Id).count());
	  }
	}

}