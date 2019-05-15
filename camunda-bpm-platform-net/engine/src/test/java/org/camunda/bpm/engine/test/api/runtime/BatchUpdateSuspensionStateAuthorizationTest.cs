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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
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
//	import static org.junit.Assert.assertEquals;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchUpdateSuspensionStateAuthorizationTest
	public class BatchUpdateSuspensionStateAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchUpdateSuspensionStateAuthorizationTest()
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
			helper = new BatchModificationHelper(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testRule);
		}


	  protected internal const string TEST_REASON = "test reason";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchModificationHelper helper;

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
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "ProcessDefinition", "userId", Permissions.UPDATE_INSTANCE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", ProcessInstancePermissions.SUSPEND), grant(Resources.PROCESS_DEFINITION, "ProcessDefinition", "userId", ProcessDefinitionPermissions.SUSPEND_INSTANCE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "*", "userId", ProcessInstancePermissions.SUSPEND)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND), grant(Resources.PROCESS_INSTANCE, "*", "userId", Permissions.UPDATE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		Batch batch = engineRule.ManagementService.createBatchQuery().singleResult();
		if (batch != null)
		{
		  engineRule.ManagementService.deleteBatch(batch.Id, true);
		}

		HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  engineRule.HistoryService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeBatch()
	  public virtual void executeBatch()
	  {
		//given
		testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);

		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey("Process");

		authRule.init(scenario).withUser("userId").bindResource("processInstance1", processInstance1.Id).bindResource("updateProcessInstanceSuspensionState", "*").bindResource("ProcessDefinition","Process").bindResource("batchId", "*").start();

		Batch batch = engineRule.RuntimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(processInstance1.Id).suspendAsync();

		if (batch != null)
		{
		  Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();

		  // seed job
		  engineRule.ManagementService.executeJob(job.Id);

		  foreach (Job pending in engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		  {
			engineRule.ManagementService.executeJob(pending.Id);
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