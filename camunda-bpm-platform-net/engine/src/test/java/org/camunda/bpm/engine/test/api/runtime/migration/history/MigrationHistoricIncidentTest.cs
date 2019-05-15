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
namespace org.camunda.bpm.engine.test.api.runtime.migration.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AsyncProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.AsyncProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MigrationHistoricIncidentTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationHistoricIncidentTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		historyService = rule.HistoryService;
		runtimeService = rule.RuntimeService;
		managementService = rule.ManagementService;
		repositoryService = rule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateHistoricIncident()
	  public virtual void testMigrateHistoricIncident()
	  {
		// given
		ProcessDefinition sourceProcess = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcess = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY).changeElementId("userTask", "newUserTask"));

		JobDefinition targetJobDefinition = managementService.createJobDefinitionQuery().processDefinitionId(targetProcess.Id).singleResult();

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcess.Id, targetProcess.Id).mapActivities("userTask", "newUserTask").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcess.Id);

		Job job = managementService.createJobQuery().singleResult();
		managementService.setJobRetries(job.Id, 0);

		HistoricIncident incidentBeforeMigration = historyService.createHistoricIncidentQuery().singleResult();

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		Assert.assertNotNull(historicIncident);

		Assert.assertEquals("newUserTask", historicIncident.ActivityId);
		Assert.assertEquals(targetJobDefinition.Id, historicIncident.JobDefinitionId);
		Assert.assertEquals(targetProcess.Id, historicIncident.ProcessDefinitionId);
		Assert.assertEquals(targetProcess.Key, historicIncident.ProcessDefinitionKey);
		Assert.assertEquals(processInstance.Id, historicIncident.ExecutionId);

		// and other properties have not changed
		Assert.assertEquals(incidentBeforeMigration.CreateTime, historicIncident.CreateTime);
		Assert.assertEquals(incidentBeforeMigration.ProcessInstanceId, historicIncident.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateHistoricIncidentAddScope()
	  public virtual void testMigrateHistoricIncidentAddScope()
	  {
		// given
		ProcessDefinition sourceProcess = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcess = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcess.Id, targetProcess.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcess.Id);

		Job job = managementService.createJobQuery().singleResult();
		managementService.setJobRetries(job.Id, 0);

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();
		Assert.assertNotNull(historicIncident);
		Assert.assertEquals(activityInstance.getTransitionInstances("userTask")[0].ExecutionId, historicIncident.ExecutionId);
	  }
	}

}