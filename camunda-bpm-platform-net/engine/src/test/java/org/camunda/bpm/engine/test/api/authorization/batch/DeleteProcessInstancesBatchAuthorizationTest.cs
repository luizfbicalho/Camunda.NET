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
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class DeleteProcessInstancesBatchAuthorizationTest extends AbstractBatchAuthorizationTest
	public class DeleteProcessInstancesBatchAuthorizationTest : AbstractBatchAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeleteProcessInstancesBatchAuthorizationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const long BATCH_OPERATIONS = 3L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.DELETE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.DELETE), grant(Resources.PROCESS_DEFINITION, "Process_2", "userId", Permissions.DELETE_INSTANCE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.ALL), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.ALL)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.ALL), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.ALL)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_DEFINITION, "Process_2", "userId", Permissions.READ_INSTANCE, Permissions.DELETE_INSTANCE), grant(Resources.PROCESS_DEFINITION, "Process_1", "userId", Permissions.READ_INSTANCE, Permissions.DELETE_INSTANCE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithTwoInvocationsProcessInstancesList()
	  public virtual void testWithTwoInvocationsProcessInstancesList()
	  {
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
		setupAndExecuteProcessInstancesListTest();

		// then
		assertScenario();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstancesList()
	  public virtual void testProcessInstancesList()
	  {
		setupAndExecuteProcessInstancesListTest();
		// then
		assertScenario();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithQuery()
	  public virtual void testWithQuery()
	  {
		//given
		ProcessInstanceQuery processInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceIds(new HashSet<string>(Arrays.asList(processInstance.Id, processInstance2.Id)));

		authRule.init(scenario).withUser("userId").bindResource("processInstance1", processInstance.Id).bindResource("processInstance2", processInstance2.Id).bindResource("Process_2", sourceDefinition2.Key).start();

		// when

		batch = runtimeService.deleteProcessInstancesAsync(null, processInstanceQuery, TEST_REASON);
		executeSeedAndBatchJobs();

		// then
		if (authRule.assertScenario(scenario))
		{
		  if (testHelper.HistoryLevelFull)
		  {
			assertThat(engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).count(), @is(BATCH_OPERATIONS));
		  }
		}
	  }

	  protected internal virtual void setupAndExecuteProcessInstancesListTest()
	  {
		//given
		IList<string> processInstanceIds = Arrays.asList(processInstance.Id, processInstance2.Id);
		authRule.init(scenario).withUser("userId").bindResource("processInstance1", processInstance.Id).bindResource("processInstance2", processInstance2.Id).bindResource("Process_2", sourceDefinition2.Key).bindResource("Process_1", sourceDefinition.Key).start();

		// when
		batch = runtimeService.deleteProcessInstancesAsync(processInstanceIds, null, TEST_REASON);

		executeSeedAndBatchJobs();
	  }

	  protected internal virtual void assertScenario()
	  {
		if (authRule.assertScenario(Scenario))
		{
		  Batch batch = engineRule.ManagementService.createBatchQuery().singleResult();
		  assertEquals("userId", batch.CreateUserId);

		  if (testHelper.HistoryLevelFull)
		  {
			assertThat(engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).count(), @is(BATCH_OPERATIONS));
			HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().list().get(0);
			assertEquals("userId", historicBatch.CreateUserId);
		  }

		  if (authRule.scenarioSucceeded())
		  {
			assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
		  }
		}
	  }

	  public override AuthorizationScenario Scenario
	  {
		  get
		  {
			return scenario;
		  }
	  }
	}

}