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

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Christopher Zell
	/// </summary>
	public class MigrationHistoricProcessInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationHistoricProcessInstanceTest()
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

	  //============================================================================
	  //===================================Migration================================
	  //============================================================================
	  protected internal ProcessDefinition sourceProcessDefinition;
	  protected internal ProcessDefinition targetProcessDefinition;
	  protected internal MigrationPlan migrationPlan;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initTest()
	  public virtual void initTest()
	  {
		runtimeService = rule.RuntimeService;
		historyService = rule.HistoryService;


		sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ModifiableBpmnModelInstance modifiedModel = modify(ProcessModels.ONE_TASK_PROCESS).changeElementId("Process", "Process2").changeElementId("userTask", "userTask2");
		targetProcessDefinition = testHelper.deployAndGetDefinition(modifiedModel);
		migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask2").build();
		runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoryProcessInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoryProcessInstance()
	  {
		//given
		HistoricProcessInstanceQuery sourceHistoryProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		HistoricProcessInstanceQuery targetHistoryProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery().processDefinitionId(targetProcessDefinition.Id);


		//when
		assertEquals(1, sourceHistoryProcessInstanceQuery.count());
		assertEquals(0, targetHistoryProcessInstanceQuery.count());
		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		runtimeService.newMigration(migrationPlan).processInstanceQuery(sourceProcessInstanceQuery).execute();

		//then
		assertEquals(0, sourceHistoryProcessInstanceQuery.count());
		assertEquals(1, targetHistoryProcessInstanceQuery.count());

		HistoricProcessInstance instance = targetHistoryProcessInstanceQuery.singleResult();
		assertEquals(instance.ProcessDefinitionKey, targetProcessDefinition.Key);
	  }


	}

}