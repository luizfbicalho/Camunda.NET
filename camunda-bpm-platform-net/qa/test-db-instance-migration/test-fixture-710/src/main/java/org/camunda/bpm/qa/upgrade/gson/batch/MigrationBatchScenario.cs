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
namespace org.camunda.bpm.qa.upgrade.gson.batch
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class MigrationBatchScenario
	{

	  [DescribesScenario("initMigrationBatch")]
	  public static ScenarioSetup initMigrationBatch()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			string sourceProcessDefinitionId = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessMigrationV1.bpmn20.xml").deployWithResult().DeployedProcessDefinitions[0].Id;

			IList<string> processInstanceIds = new List<string>();
			for (int i = 0; i < 10; i++)
			{
			  string processInstanceId = engine.RuntimeService.startProcessInstanceById(sourceProcessDefinitionId, "MigrationBatchScenario").Id;

			  processInstanceIds.Add(processInstanceId);
			}

			string targetProcessDefinitionId = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessMigrationV2.bpmn20.xml").deployWithResult().DeployedProcessDefinitions[0].Id;

			MigrationPlan migrationPlan = engine.RuntimeService.createMigrationPlan(sourceProcessDefinitionId, targetProcessDefinitionId).mapActivities("userTask1", "userTask1").mapActivities("conditional", "conditional").updateEventTrigger().build();

			engine.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstanceIds).skipIoMappings().skipCustomListeners().executeAsync();
		  }
	  }
	}

}