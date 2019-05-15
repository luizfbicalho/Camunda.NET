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
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TimerChangeJobDefinitionScenario
	{

	  protected internal static readonly DateTime FIXED_DATE_ONE = new DateTime(1363607000000L);
	  protected internal static readonly DateTime FIXED_DATE_TWO = new DateTime(1363607500000L);
	  protected internal static readonly DateTime FIXED_DATE_THREE = new DateTime(1363607600000L);
	  protected internal static readonly DateTime FIXED_DATE_FOUR = new DateTime(1363607700000L);

	  [DescribesScenario("initTimerChangeJobDefinition")]
	  public static ScenarioSetup initTimerChangeJobDefinition()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			string processDefinitionIdWithoutTenant = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessTimerJob.bpmn20.xml").deployWithResult().DeployedProcessDefinitions[0].Id;

			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcessTimerJob_710");

			Job job = engine.ManagementService.createJobQuery().processDefinitionKey("oneTaskProcessTimerJob_710").singleResult();

			engine.ManagementService.updateJobDefinitionSuspensionState().byJobDefinitionId(job.JobDefinitionId).includeJobs(true).executionDate(FIXED_DATE_ONE).suspend();

			engine.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionId(processDefinitionIdWithoutTenant).includeJobs(false).executionDate(FIXED_DATE_TWO).suspend();

			engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/qa/upgrade/gson/oneTaskProcessTimerJob.bpmn20.xml").tenantId("aTenantId").deploy();

			engine.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey("oneTaskProcessTimerJob_710").processDefinitionTenantId("aTenantId").includeJobs(false).executionDate(FIXED_DATE_THREE).suspend();

			engine.ManagementService.updateJobDefinitionSuspensionState().byProcessDefinitionKey("oneTaskProcessTimerJob_710").processDefinitionWithoutTenantId().includeJobs(false).executionDate(FIXED_DATE_FOUR).suspend();
		  }
	  }
	}

}