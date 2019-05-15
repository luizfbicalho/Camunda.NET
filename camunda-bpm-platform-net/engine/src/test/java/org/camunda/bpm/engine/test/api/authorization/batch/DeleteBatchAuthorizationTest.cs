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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) @RunWith(Parameterized.class) public class DeleteBatchAuthorizationTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class DeleteBatchAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeleteBatchAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.BATCH, "batchId", "userId", Permissions.DELETE)), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", Permissions.DELETE)).succeeds());
	  }

	  protected internal MigrationPlan migrationPlan;
	  protected internal Batch batch;
	  protected internal bool cascade;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcessesAndCreateMigrationPlan()
	  public virtual void deployProcessesAndCreateMigrationPlan()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).build();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void deleteBatch()
	  public virtual void deleteBatch()
	  {
		if (authRule.scenarioFailed())
		{
		  engineRule.ManagementService.deleteBatch(batch.Id, true);
		}
		else
		{
		  if (!cascade && engineRule.ProcessEngineConfiguration.HistoryLevel == org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL)
		  {
			engineRule.HistoryService.deleteHistoricBatch(batch.Id);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteBatch()
	  public virtual void testDeleteBatch()
	  {

		// given
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		// when
		authRule.init(scenario).withUser("userId").bindResource("batchId", batch.Id).start();

		cascade = false;
		engineRule.ManagementService.deleteBatch(batch.Id, cascade);

		// then
		if (authRule.assertScenario(scenario))
		{
		  Assert.assertEquals(0, engineRule.ManagementService.createBatchQuery().count());

		  IList<UserOperationLogEntry> userOperationLogEntries = engineRule.HistoryService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE).list();

		  assertEquals(1, userOperationLogEntries.Count);

		  UserOperationLogEntry entry = userOperationLogEntries[0];
		  assertEquals("cascadeToHistory", entry.Property);
		  assertEquals("false", entry.NewValue);
		  assertEquals(CATEGORY_OPERATOR, entry.Category);
		}
	  }

	  /// <summary>
	  /// Requires no additional DELETE_HISTORY authorization => consistent with deleteDeployment
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteBatchCascade()
	  public virtual void testDeleteBatchCascade()
	  {
		// given
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		// when
		authRule.init(scenario).withUser("userId").bindResource("batchId", batch.Id).start();

		cascade = true;
		engineRule.ManagementService.deleteBatch(batch.Id, cascade);

		// then
		if (authRule.assertScenario(scenario))
		{
		  Assert.assertEquals(0, engineRule.ManagementService.createBatchQuery().count());
		  Assert.assertEquals(0, engineRule.HistoryService.createHistoricBatchQuery().count());

		  UserOperationLogQuery query = engineRule.HistoryService.createUserOperationLogQuery();

		  IList<UserOperationLogEntry> userOperationLogEntries = query.operationType(OPERATION_TYPE_DELETE).batchId(batch.Id).list();
		  assertEquals(1, userOperationLogEntries.Count);

		  UserOperationLogEntry entry = userOperationLogEntries[0];
		  assertEquals("cascadeToHistory", entry.Property);
		  assertEquals("true", entry.NewValue);
		  assertEquals(CATEGORY_OPERATOR, entry.Category);

		  // Ensure that HistoricBatch deletion is not logged
		  IList<UserOperationLogEntry> userOperationLogHistoricEntries = query.operationType(OPERATION_TYPE_DELETE_HISTORY).batchId(batch.Id).list();
		  assertEquals(0, userOperationLogHistoricEntries.Count);
		}
	  }
	}

}