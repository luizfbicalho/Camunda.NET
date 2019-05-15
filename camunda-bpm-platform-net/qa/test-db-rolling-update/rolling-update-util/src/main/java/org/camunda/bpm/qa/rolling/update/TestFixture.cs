﻿/*
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
namespace org.camunda.bpm.qa.rolling.update
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DeploymentWhichShouldBeDeletedScenario = org.camunda.bpm.qa.rolling.update.scenarios.DeploymentWhichShouldBeDeletedScenario;
	using AuthorizationScenario = org.camunda.bpm.qa.rolling.update.scenarios.authorization.AuthorizationScenario;
	using ProcessWithCallActivityScenario = org.camunda.bpm.qa.rolling.update.scenarios.callactivity.ProcessWithCallActivityScenario;
	using HistoryCleanupScenario = org.camunda.bpm.qa.rolling.update.scenarios.cleanup.HistoryCleanupScenario;
	using ProcessWithEventSubProcessScenario = org.camunda.bpm.qa.rolling.update.scenarios.eventSubProcess.ProcessWithEventSubProcessScenario;
	using ProcessWithExternalTaskScenario = org.camunda.bpm.qa.rolling.update.scenarios.externalTask.ProcessWithExternalTaskScenario;
	using ProcessWithMultiInstanceCallActivityScenario = org.camunda.bpm.qa.rolling.update.scenarios.mulltiInstance.ProcessWithMultiInstanceCallActivityScenario;
	using ProcessWithAsyncServiceTaskScenario = org.camunda.bpm.qa.rolling.update.scenarios.task.ProcessWithAsyncServiceTaskScenario;
	using ProcessWithParallelGatewayAndServiceTaskScenario = org.camunda.bpm.qa.rolling.update.scenarios.task.ProcessWithParallelGatewayAndServiceTaskScenario;
	using ProcessWithParallelGatewayScenario = org.camunda.bpm.qa.rolling.update.scenarios.task.ProcessWithParallelGatewayScenario;
	using ProcessWithUserTaskAndTimerScenario = org.camunda.bpm.qa.rolling.update.scenarios.task.ProcessWithUserTaskAndTimerScenario;
	using ProcessWithUserTaskScenario = org.camunda.bpm.qa.rolling.update.scenarios.task.ProcessWithUserTaskScenario;
	using IncidentTimestampUpdateScenario = org.camunda.bpm.qa.rolling.update.scenarios.timestamp.IncidentTimestampUpdateScenario;
	using JobTimestampsUpdateScenario = org.camunda.bpm.qa.rolling.update.scenarios.timestamp.JobTimestampsUpdateScenario;
	using ScenarioRunner = org.camunda.bpm.qa.upgrade.ScenarioRunner;

	/// <summary>
	/// Sets up scenarios for rolling updates.
	/// 
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class TestFixture
	{

	  public const string DEFAULT_TAG = RollingUpdateConstants.OLD_ENGINE_TAG;
	  public static string currentFixtureTag;

	  public TestFixture(ProcessEngine processEngine)
	  {
	  }

	  public static void Main(string[] args)
	  {
		string tag = DEFAULT_TAG;
		if (args.Length > 0)
		{
		  tag = args[0];
		}
		currentFixtureTag = tag;

		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml");
		ProcessEngine processEngine = processEngineConfiguration.buildProcessEngine();

		// register test scenarios
		ScenarioRunner runner = new ScenarioRunner(processEngine, tag);
		// compensation
		//rolling upgrade test scenarios
		runner.setupScenarios(typeof(ProcessWithUserTaskScenario));
		runner.setupScenarios(typeof(ProcessWithAsyncServiceTaskScenario));
		runner.setupScenarios(typeof(ProcessWithUserTaskAndTimerScenario));
		runner.setupScenarios(typeof(DeploymentWhichShouldBeDeletedScenario));
		runner.setupScenarios(typeof(ProcessWithParallelGatewayScenario));
		runner.setupScenarios(typeof(ProcessWithParallelGatewayAndServiceTaskScenario));
		runner.setupScenarios(typeof(ProcessWithCallActivityScenario));
		runner.setupScenarios(typeof(ProcessWithMultiInstanceCallActivityScenario));
		runner.setupScenarios(typeof(ProcessWithExternalTaskScenario));
		runner.setupScenarios(typeof(ProcessWithEventSubProcessScenario));
		runner.setupScenarios(typeof(JobTimestampsUpdateScenario));
		runner.setupScenarios(typeof(IncidentTimestampUpdateScenario));

		if (RollingUpdateConstants.NEW_ENGINE_TAG.Equals(currentFixtureTag))
		{ // create data with new engine
		  runner.setupScenarios(typeof(HistoryCleanupScenario));
		}

		processEngine.close();

		processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.auth.cfg.xml");
		processEngine = processEngineConfiguration.buildProcessEngine();

		// register test auth scenarios
		runner = new ScenarioRunner(processEngine, tag);

		runner.setupScenarios(typeof(AuthorizationScenario));

		processEngine.close();
	  }
	}

}