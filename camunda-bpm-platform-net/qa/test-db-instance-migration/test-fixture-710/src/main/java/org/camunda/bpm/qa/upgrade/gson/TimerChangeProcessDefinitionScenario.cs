using System;

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
namespace org.camunda.bpm.qa.upgrade.gson
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TimerChangeProcessDefinitionScenario
	{

	  protected internal static readonly DateTime FIXED_DATE_ONE = new DateTime(1363608000000L);
	  protected internal static readonly DateTime FIXED_DATE_TWO = new DateTime(1363608500000L);
	  protected internal static readonly DateTime FIXED_DATE_THREE = new DateTime(1363608600000L);

	  [DescribesScenario("initTimerChangeProcessDefinition")]
	  public static ScenarioSetup initTimerChangeProcessDefinition()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			string processDefinitionIdWithoutTenant = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessTimer.bpmn20.xml").tenantId(null).deployWithResult().DeployedProcessDefinitions[0].Id;

			engine.RuntimeService.startProcessInstanceById(processDefinitionIdWithoutTenant, "TimerChangeProcessDefinitionScenarioV1");

			engine.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinitionIdWithoutTenant).includeProcessInstances(true).executionDate(FIXED_DATE_ONE).suspend();

			string processDefinitionIdWithTenant = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessTimer.bpmn20.xml").tenantId("aTenantId").deployWithResult().DeployedProcessDefinitions[0].Id;

			engine.RuntimeService.startProcessInstanceById(processDefinitionIdWithTenant, "TimerChangeProcessDefinitionScenarioV2");

			engine.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey("oneTaskProcessTimer_710").processDefinitionTenantId("aTenantId").includeProcessInstances(true).executionDate(FIXED_DATE_TWO).suspend();

			engine.RepositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionKey("oneTaskProcessTimer_710").processDefinitionWithoutTenantId().includeProcessInstances(false).executionDate(FIXED_DATE_THREE).suspend();
		  }
	  }
	}

}