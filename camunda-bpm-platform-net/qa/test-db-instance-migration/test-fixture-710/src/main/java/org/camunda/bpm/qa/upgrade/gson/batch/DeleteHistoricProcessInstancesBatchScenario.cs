﻿using System.Collections.Generic;

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
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class DeleteHistoricProcessInstancesBatchScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/upgrade/gson/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("initDeleteHistoricProcessInstancesBatch")]
	  public static ScenarioSetup initDeleteHistoricProcessInstancesBatch()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			for (int i = 0; i < 10; i++)
			{
			  engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess_710", "DeleteHistoricProcessInstancesBatchScenario");

			  string taskId = engine.TaskService.createTaskQuery().processDefinitionKey("oneTaskProcess_710").processInstanceBusinessKey("DeleteHistoricProcessInstancesBatchScenario").singleResult().Id;

			  engine.TaskService.complete(taskId);
			}

			IList<string> processInstanceIds = new List<string>();

			IList<HistoricProcessInstance> processInstances = engine.HistoryService.createHistoricProcessInstanceQuery().processDefinitionKey("oneTaskProcess_710").processInstanceBusinessKey("DeleteHistoricProcessInstancesBatchScenario").list();

			foreach (HistoricProcessInstance processInstance in processInstances)
			{
			  processInstanceIds.Add(processInstance.Id);
			}

			engine.HistoryService.deleteHistoricProcessInstancesAsync(processInstanceIds, null);
		  }
	  }
	}

}