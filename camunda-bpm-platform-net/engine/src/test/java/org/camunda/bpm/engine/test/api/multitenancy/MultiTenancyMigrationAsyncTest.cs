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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MultiTenancyMigrationAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationAsyncTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			defaultTestRule = new ProcessEngineTestRule(defaultEngineRule);
			migrationRule = new MigrationTestRule(defaultEngineRule);
			defaultRuleChin = RuleChain.outerRule(defaultEngineRule).around(defaultTestRule).around(migrationRule);
			batchHelper = new BatchMigrationHelper(defaultEngineRule, migrationRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule defaultEngineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule defaultTestRule;
	  protected internal MigrationTestRule migrationRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain defaultRuleChin = org.junit.rules.RuleChain.outerRule(defaultEngineRule).around(defaultTestRule).around(migrationRule);
	  public RuleChain defaultRuleChin;

	  protected internal BatchMigrationHelper batchHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		batchHelper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canMigrateInstanceBetweenSameTenantCase1()
	  public virtual void canMigrateInstanceBetweenSameTenantCase1()
	  {
		// given
		ProcessDefinition sourceDefinition = defaultTestRule.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = defaultTestRule.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = defaultEngineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		MigrationPlan migrationPlan = defaultEngineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		Batch batch = defaultEngineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		batchHelper.executeSeedJob(batch);

		// when
		batchHelper.executeJobs(batch);

		// then
		assertMigratedTo(processInstance, targetDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  public virtual void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  {
		// given
		ProcessDefinition sourceDefinition = defaultTestRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = defaultTestRule.deployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.ONE_TASK_PROCESS);

		ProcessInstance processInstance = defaultEngineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		MigrationPlan migrationPlan = defaultEngineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		Batch batch = defaultEngineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		batchHelper.executeSeedJob(batch);

		// when
		batchHelper.executeJobs(batch);

		// then
		Job migrationJob = batchHelper.getExecutionJobs(batch)[0];
		Assert.assertThat(migrationJob.ExceptionMessage, CoreMatchers.containsString("Cannot migrate process instance '" + processInstance.Id + "' without tenant to a process definition with a tenant ('tenant1')"));
	  }

	  protected internal virtual void assertMigratedTo(ProcessInstance processInstance, ProcessDefinition targetDefinition)
	  {
		Assert.assertEquals(1, defaultEngineRule.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).processDefinitionId(targetDefinition.Id).count());
	  }
	}

}