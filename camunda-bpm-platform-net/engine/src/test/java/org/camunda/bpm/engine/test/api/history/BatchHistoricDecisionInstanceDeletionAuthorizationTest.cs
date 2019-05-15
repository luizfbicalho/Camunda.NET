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
namespace org.camunda.bpm.engine.test.api.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.revoke;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) @RunWith(Parameterized.class) public class BatchHistoricDecisionInstanceDeletionAuthorizationTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BatchHistoricDecisionInstanceDeletionAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchHistoricDecisionInstanceDeletionAuthorizationTest()
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
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testRule);
		}


	  protected internal static string DECISION = "decision";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testRule;

	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

	  protected internal IList<string> decisionInstanceIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE)).failsDueToRequired(grant(Resources.DECISION_DEFINITION, "*", "userId", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.DECISION_DEFINITION, "*", "userId", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES), grant(Resources.DECISION_DEFINITION, "*", "userId", Permissions.DELETE_HISTORY)), scenario().withAuthorizations(revoke(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES), grant(Resources.BATCH, "*", "userId", Permissions.CREATE)).failsDueToRequired(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_DELETE_DECISION_INSTANCES)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		historyService = engineRule.HistoryService;
		decisionService = engineRule.DecisionService;
		managementService = engineRule.ManagementService;
		decisionInstanceIds = new List<string>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void executeDecisionInstances()
	  public virtual void executeDecisionInstances()
	  {
		testRule.deploy("org/camunda/bpm/engine/test/api/dmn/Example.dmn");

		VariableMap variables = Variables.createVariables().putValue("status", "silver").putValue("sum", 723);

		for (int i = 0; i < 10; i++)
		{
		  decisionService.evaluateDecisionByKey(DECISION).variables(variables).evaluate();
		}

		IList<HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance decisionInstance in decisionInstances)
		{
		  decisionInstanceIds.Add(decisionInstance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		foreach (Batch batch in managementService.createBatchQuery().list())
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		// remove history of completed batches
		foreach (HistoricBatch historicBatch in historyService.createHistoricBatchQuery().list())
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeBatch()
	  public virtual void executeBatch()
	  {
		// given
		authRule.init(scenario).withUser("userId").start();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		if (batch != null)
		{
		  Job job = managementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();

		  // seed job
		  managementService.executeJob(job.Id);

		  foreach (Job pending in managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		  {
			managementService.executeJob(pending.Id);
		  }
		}
		// then
		if (authRule.assertScenario(scenario))
		{
		  assertEquals("userId", batch.CreateUserId);
		}
	  }
	}

}