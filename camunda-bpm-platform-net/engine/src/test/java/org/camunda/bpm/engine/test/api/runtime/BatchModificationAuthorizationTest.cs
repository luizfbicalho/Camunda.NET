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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchModificationAuthorizationTest
	public class BatchModificationAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchModificationAuthorizationTest()
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


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchModificationHelper helper;

	  protected internal ProcessDefinition processDefinition;

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
		return AuthorizationTestRule.asParameters(scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ, Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ, Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.UPDATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_MODIFY_PROCESS_INSTANCES), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.READ, Permissions.UPDATE), grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.PROCESS_INSTANCE, "processInstance2", "userId", Permissions.UPDATE), grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.UPDATE_INSTANCE)).succeeds());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcess()
	  public virtual void deployProcess()
	  {
		processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
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
//ORIGINAL LINE: @Test public void executeAsyncModification()
	  public virtual void executeAsyncModification()
	  {
		//given
		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);
		ProcessInstance processInstance2 = engineRule.RuntimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		authRule.init(scenario).withUser("userId").bindResource("processInstance1", processInstance1.Id).bindResource("processInstance2", processInstance2.Id).bindResource("processDefinition", ProcessModels.PROCESS_KEY).bindResource("batchId", "*").start();

		Batch batch = engineRule.RuntimeService.createModification(processDefinition.Id).processInstanceIds(processInstance1.Id, processInstance2.Id).startAfterActivity("userTask2").executeAsync();

		Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();

		//seed job
		engineRule.ManagementService.executeJob(job.Id);

		foreach (Job pending in engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  engineRule.ManagementService.executeJob(pending.Id);
		}

		// then
		authRule.assertScenario(scenario);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void executeModification()
	  public virtual void executeModification()
	  {
		//given
		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);
		ProcessInstance processInstance2 = engineRule.RuntimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		authRule.init(scenario).withUser("userId").bindResource("processInstance1", processInstance1.Id).bindResource("processInstance2", processInstance2.Id).bindResource("processDefinition", ProcessModels.PROCESS_KEY).bindResource("batchId", "*").start();

		// when
		engineRule.RuntimeService.createModification(processDefinition.Id).processInstanceIds(processInstance1.Id, processInstance2.Id).startAfterActivity("userTask2").execute();

		// then
		authRule.assertScenario(scenario);
	  }
	}

}