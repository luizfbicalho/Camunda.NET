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
namespace org.camunda.bpm.engine.test.api.runtime.migration.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
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
	public class MigrationHistoricActivityInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationHistoricActivityInstanceTest()
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		historyService = rule.HistoryService;
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoryActivityInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoryActivityInstance()
	  {
		//given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId("Process", "Process2").changeElementId("userTask", "userTask2").changeElementName("userTask", "new activity name"));

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		HistoricActivityInstanceQuery sourceHistoryActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		HistoricActivityInstanceQuery targetHistoryActivityInstanceQuery = historyService.createHistoricActivityInstanceQuery().processDefinitionId(targetProcessDefinition.Id);

		//when
		assertEquals(2, sourceHistoryActivityInstanceQuery.count());
		assertEquals(0, targetHistoryActivityInstanceQuery.count());
		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		runtimeService.newMigration(migrationPlan).processInstanceQuery(sourceProcessInstanceQuery).execute();

		// then one instance of the start event still belongs to the source process
		// and one active user task instances is now migrated to the target process
		assertEquals(1, sourceHistoryActivityInstanceQuery.count());
		assertEquals(1, targetHistoryActivityInstanceQuery.count());

		HistoricActivityInstance instance = targetHistoryActivityInstanceQuery.singleResult();
		assertMigratedTo(instance, targetProcessDefinition, "userTask2");
		assertEquals("new activity name", instance.ActivityName);
		assertEquals(processInstance.Id, instance.ParentActivityInstanceId);
		assertEquals("userTask", instance.ActivityType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoricSubProcessInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoricSubProcessInstance()
	  {
		//given
		ProcessDefinition processDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(processDefinition.Id, processDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(processDefinition.Id);

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).unfinished().orderByActivityId().asc().list();

		Assert.assertEquals(2, historicInstances.Count);

		assertMigratedTo(historicInstances[0], processDefinition, "subProcess");
		assertMigratedTo(historicInstances[1], processDefinition, "userTask");
		assertEquals(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
		assertEquals(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoricSubProcessRename()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoricSubProcessRename()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).changeElementId("subProcess", "newSubProcess"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("subProcess", "newSubProcess").mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).unfinished().orderByActivityId().asc().list();

		Assert.assertEquals(2, historicInstances.Count);

		assertMigratedTo(historicInstances[0], targetDefinition, "newSubProcess");
		assertMigratedTo(historicInstances[1], targetDefinition, "userTask");
		assertEquals(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
		assertEquals(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testHistoricActivityInstanceBecomeScope()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testHistoricActivityInstanceBecomeScope()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).unfinished().orderByActivityId().asc().list();

		Assert.assertEquals(1, historicInstances.Count);

		assertMigratedTo(historicInstances[0], targetDefinition, "userTask");
		assertEquals(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoricActivityInstanceAddScope()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoricActivityInstanceAddScope()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		IList<HistoricActivityInstance> historicInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).unfinished().orderByActivityId().asc().list();

		Assert.assertEquals(2, historicInstances.Count);

		assertMigratedTo(historicInstances[0], targetDefinition, "subProcess");
		assertMigratedTo(historicInstances[1], targetDefinition, "userTask");
		assertEquals(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
		assertEquals(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
	  }

	  protected internal virtual void assertMigratedTo(HistoricActivityInstance activityInstance, ProcessDefinition processDefinition, string activityId)
	  {
		Assert.assertEquals(processDefinition.Id, activityInstance.ProcessDefinitionId);
		Assert.assertEquals(processDefinition.Key, activityInstance.ProcessDefinitionKey);
		Assert.assertEquals(activityId, activityInstance.ActivityId);
	  }

	}

}