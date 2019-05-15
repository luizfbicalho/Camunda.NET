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
namespace org.camunda.bpm.engine.test.api.authorization.batch
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationTestBaseRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestBaseRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class BatchStatisticsQueryAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchStatisticsQueryAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestBaseRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestBaseRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal MigrationPlan migrationPlan;
	  protected internal Batch batch1;
	  protected internal Batch batch2;
	  protected internal Batch batch3;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("user", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcessesAndCreateMigrationPlan()
	  public virtual void deployProcessesAndCreateMigrationPlan()
	  {
		ProcessInstance pi = createMigrationPlan();

		batch1 = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();

		Job seedJob = engineRule.ManagementService.createJobQuery().singleResult();
		engineRule.ManagementService.executeJob(seedJob.Id);

		batch2 = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void deleteBatches()
	  public virtual void deleteBatches()
	  {
		engineRule.ManagementService.deleteBatch(batch1.Id, true);
		engineRule.ManagementService.deleteBatch(batch2.Id, true);
		if (batch3 != null)
		{
		  engineRule.ManagementService.deleteBatch(batch3.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryList()
	  public virtual void testQueryList()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ);

		// when
		authRule.enableAuthorization("user");
		IList<BatchStatistics> batches = engineRule.ManagementService.createBatchStatisticsQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(1, batches.Count);
		Assert.assertEquals(batch1.Id, batches[0].Id);

		// and the visibility of jobs is not restricted
		Assert.assertEquals(1, batches[0].JobsCreated);
		Assert.assertEquals(1, batches[0].RemainingJobs);
		Assert.assertEquals(1, batches[0].TotalJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ);

		// when
		authRule.enableAuthorization("user");
		long count = engineRule.ManagementService.createBatchStatisticsQuery().count();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(1, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoAuthorizations()
	  public virtual void testQueryNoAuthorizations()
	  {
		// when
		authRule.enableAuthorization("user");
		long count = engineRule.ManagementService.createBatchStatisticsQuery().count();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryListAccessAll()
	  public virtual void testQueryListAccessAll()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, "*", "user", Permissions.READ);

		// when
		authRule.enableAuthorization("user");
		IList<BatchStatistics> batches = engineRule.ManagementService.createBatchStatisticsQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(2, batches.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryListMultiple()
	  public virtual void testQueryListMultiple()
	  {
		// given
		authRule.createGrantAuthorization(Resources.BATCH, "*", "user", Permissions.READ);
		authRule.createGrantAuthorization(Resources.BATCH, batch1.Id, "user", Permissions.READ);

		// when
		authRule.enableAuthorization("user");
		IList<BatchStatistics> batches = engineRule.ManagementService.createBatchStatisticsQuery().list();
		authRule.disableAuthorization();

		// then
		Assert.assertEquals(2, batches.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchStatisticsAndCreateUserId()
	  public virtual void testBatchStatisticsAndCreateUserId()
	  {
		// given
		ProcessInstance pi = createMigrationPlan();

		// when
		authRule.createGrantAuthorization(Resources.BATCH, "*", "userId", Permissions.CREATE);
		authRule.createGrantAuthorization(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.MIGRATE_INSTANCE);

		authRule.enableAuthorization("userId");
		batch3 = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi.Id)).executeAsync();
		authRule.disableAuthorization();

		// then
		BatchStatistics batchStatistics = engineRule.ManagementService.createBatchStatisticsQuery().batchId(batch3.Id).singleResult();
		assertEquals("userId", batchStatistics.CreateUserId);
	  }

	  protected internal virtual ProcessInstance createMigrationPlan()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance pi = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		return pi;
	  }
	}

}