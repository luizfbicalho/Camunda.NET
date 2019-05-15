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
namespace org.camunda.bpm.qa.upgrade
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessInstanceModificationScenario = org.camunda.bpm.qa.upgrade.gson.ProcessInstanceModificationScenario;
	using TaskFilterPropertiesScenario = org.camunda.bpm.qa.upgrade.gson.TaskFilterPropertiesScenario;
	using TaskFilterScenario = org.camunda.bpm.qa.upgrade.gson.TaskFilterScenario;
	using TaskFilterVariablesScenario = org.camunda.bpm.qa.upgrade.gson.TaskFilterVariablesScenario;
	using TimerChangeJobDefinitionScenario = org.camunda.bpm.qa.upgrade.gson.TimerChangeJobDefinitionScenario;
	using TimerChangeProcessDefinitionScenario = org.camunda.bpm.qa.upgrade.gson.TimerChangeProcessDefinitionScenario;
	using DeleteHistoricDecisionsBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.DeleteHistoricDecisionsBatchScenario;
	using DeleteHistoricProcessInstancesBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.DeleteHistoricProcessInstancesBatchScenario;
	using DeleteProcessInstancesBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.DeleteProcessInstancesBatchScenario;
	using MigrationBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.MigrationBatchScenario;
	using ModificationBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.ModificationBatchScenario;
	using RestartProcessInstanceBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.RestartProcessInstanceBatchScenario;
	using SetExternalTaskRetriesBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.SetExternalTaskRetriesBatchScenario;
	using SetJobRetriesBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.SetJobRetriesBatchScenario;
	using UpdateProcessInstanceSuspendStateBatchScenario = org.camunda.bpm.qa.upgrade.gson.batch.UpdateProcessInstanceSuspendStateBatchScenario;
	using DeploymentDeployTimeScenario = org.camunda.bpm.qa.upgrade.timestamp.DeploymentDeployTimeScenario;
	using EventSubscriptionCreateTimeScenario = org.camunda.bpm.qa.upgrade.timestamp.EventSubscriptionCreateTimeScenario;
	using ExternalTaskLockExpTimeScenario = org.camunda.bpm.qa.upgrade.timestamp.ExternalTaskLockExpTimeScenario;
	using IncidentTimestampScenario = org.camunda.bpm.qa.upgrade.timestamp.IncidentTimestampScenario;
	using JobTimestampsScenario = org.camunda.bpm.qa.upgrade.timestamp.JobTimestampsScenario;
	using MeterLogTimestampScenario = org.camunda.bpm.qa.upgrade.timestamp.MeterLogTimestampScenario;
	using TaskCreateTimeScenario = org.camunda.bpm.qa.upgrade.timestamp.TaskCreateTimeScenario;
	using UserLockExpTimeScenario = org.camunda.bpm.qa.upgrade.timestamp.UserLockExpTimeScenario;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TestFixture
	{

	  public const string ENGINE_VERSION = "7.10.0";

	  public TestFixture(ProcessEngine processEngine)
	  {
	  }

	  public static void Main(string[] args)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml");
		ProcessEngine processEngine = processEngineConfiguration.buildProcessEngine();

		// register test scenarios
		ScenarioRunner runner = new ScenarioRunner(processEngine, ENGINE_VERSION);

		runner.setupScenarios(typeof(DeleteHistoricDecisionsBatchScenario));
		runner.setupScenarios(typeof(DeleteHistoricProcessInstancesBatchScenario));
		runner.setupScenarios(typeof(DeleteProcessInstancesBatchScenario));
		runner.setupScenarios(typeof(SetExternalTaskRetriesBatchScenario));
		runner.setupScenarios(typeof(SetJobRetriesBatchScenario));
		runner.setupScenarios(typeof(UpdateProcessInstanceSuspendStateBatchScenario));
		runner.setupScenarios(typeof(RestartProcessInstanceBatchScenario));
		runner.setupScenarios(typeof(TimerChangeProcessDefinitionScenario));
		runner.setupScenarios(typeof(TimerChangeJobDefinitionScenario));
		runner.setupScenarios(typeof(ModificationBatchScenario));
		runner.setupScenarios(typeof(ProcessInstanceModificationScenario));
		runner.setupScenarios(typeof(MigrationBatchScenario));
		runner.setupScenarios(typeof(TaskFilterScenario));
		runner.setupScenarios(typeof(TaskFilterVariablesScenario));
		runner.setupScenarios(typeof(TaskFilterPropertiesScenario));
		runner.setupScenarios(typeof(DeploymentDeployTimeScenario));
		runner.setupScenarios(typeof(JobTimestampsScenario));
		runner.setupScenarios(typeof(IncidentTimestampScenario));
		runner.setupScenarios(typeof(TaskCreateTimeScenario));
		runner.setupScenarios(typeof(ExternalTaskLockExpTimeScenario));
		runner.setupScenarios(typeof(EventSubscriptionCreateTimeScenario));
		runner.setupScenarios(typeof(MeterLogTimestampScenario));
		runner.setupScenarios(typeof(UserLockExpTimeScenario));

		processEngine.close();
	  }
	}

}