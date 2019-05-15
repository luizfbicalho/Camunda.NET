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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public abstract class AbstractBatchAuthorizationTest
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractBatchAuthorizationTest()
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
		}

	  protected internal const string TEST_REASON = "test reason";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;

	  protected internal ProcessDefinition sourceDefinition;
	  protected internal ProcessDefinition sourceDefinition2;
	  protected internal ProcessInstance processInstance;
	  protected internal ProcessInstance processInstance2;
	  protected internal Batch batch;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal int invocationsPerBatchJob;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authRule.createUserAndGroup("userId", "groupId");
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		invocationsPerBatchJob = engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void deployProcesses()
	  public virtual void deployProcesses()
	  {
		sourceDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "ONE_TASK_PROCESS"));
		sourceDefinition2 = testHelper.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "TWO_TASKS_PROCESS"));
		processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		processInstance2 = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition2.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		authRule.deleteUsersAndGroups();
		engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = invocationsPerBatchJob;
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

	  protected internal virtual void executeSeedAndBatchJobs()
	  {
		Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();
		//seed job
		managementService.executeJob(job.Id);

		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		}
	  }

	  protected internal abstract AuthorizationScenario Scenario {get;}
	}

}