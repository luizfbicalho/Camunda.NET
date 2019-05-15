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
namespace org.camunda.bpm.engine.test.api.authorization.externaltask
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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ExternalTaskModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ExternalTaskModels;
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class SetExternalTasksRetriesBatchAuthorizationTest
	public class SetExternalTasksRetriesBatchAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetExternalTasksRetriesBatchAuthorizationTest()
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
			chain = RuleChain.outerRule(engineRule).around(authRule).around(testRule);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testRule;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testRule);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.READ, Permissions.READ_INSTANCE)).failsDueToRequired(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES)), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.READ, Permissions.READ_INSTANCE, Permissions.UPDATE_INSTANCE), grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.READ, Permissions.READ_INSTANCE, Permissions.UPDATE_INSTANCE), grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.READ, Permissions.READ_INSTANCE), grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.UPDATE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.READ, Permissions.READ_INSTANCE), grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE)).failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "processDefinition", "userId", Permissions.UPDATE_INSTANCE), grant(Resources.PROCESS_INSTANCE, "processInstance1", "userId", Permissions.UPDATE)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
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
//ORIGINAL LINE: @Test public void testSetRetriesAsync()
	  public virtual void testSetRetriesAsync()
	  {

		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey("Process");
		IList<ExternalTask> externalTasks = engineRule.ExternalTaskService.createExternalTaskQuery().list();

		List<string> externalTaskIds = new List<string>();

		foreach (ExternalTask task in externalTasks)
		{
		  externalTaskIds.Add(task.Id);
		}

		// when
		authRule.init(scenario).withUser("userId").bindResource("batchId", "*").bindResource("processInstance1", processInstance1.Id).bindResource("processDefinition", processDefinition.Key).start();

		Batch batch = engineRule.ExternalTaskService.setRetriesAsync(externalTaskIds, null, 5);
		if (batch != null)
		{
		  executeSeedAndBatchJobs(batch);
		}

		// then
		if (authRule.assertScenario(scenario))
		{
		  externalTasks = engineRule.ExternalTaskService.createExternalTaskQuery().list();
		  foreach (ExternalTask task in externalTasks)
		  {
		  Assert.assertEquals(5, (int) task.Retries);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithQueryAsync()
	  public virtual void testSetRetriesWithQueryAsync()
	  {

		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessInstance processInstance1 = engineRule.RuntimeService.startProcessInstanceByKey("Process");
		IList<ExternalTask> externalTasks;

		ExternalTaskQuery externalTaskQuery = engineRule.ExternalTaskService.createExternalTaskQuery();

		// when
		authRule.init(scenario).withUser("userId").bindResource("batchId", "*").bindResource("processInstance1", processInstance1.Id).bindResource("processDefinition", processDefinition.Key).start();

		Batch batch = engineRule.ExternalTaskService.setRetriesAsync(null, externalTaskQuery, 5);
		if (batch != null)
		{
		  executeSeedAndBatchJobs(batch);
		}

		// then
		if (authRule.assertScenario(scenario))
		{
		  Assert.assertEquals("userId", batch.CreateUserId);

		  externalTasks = engineRule.ExternalTaskService.createExternalTaskQuery().list();
		  foreach (ExternalTask task in externalTasks)
		  {
			Assert.assertEquals(5, (int) task.Retries);
		  }
		}
	  }

	  public virtual void executeSeedAndBatchJobs(Batch batch)
	  {
		Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();
		// seed job
		engineRule.ManagementService.executeJob(job.Id);

		foreach (Job pending in engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  engineRule.ManagementService.executeJob(pending.Id);
		}
	  }
	}

}