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

	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using NonInterruptingBoundaryEventScenario = org.camunda.bpm.qa.upgrade.scenarios.boundary.NonInterruptingBoundaryEventScenario;
	using InterruptingEventSubProcessCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.InterruptingEventSubProcessCompensationScenario;
	using InterruptingEventSubProcessNestedCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.InterruptingEventSubProcessNestedCompensationScenario;
	using NestedCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.NestedCompensationScenario;
	using NestedMultiInstanceCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.NestedMultiInstanceCompensationScenario;
	using NonInterruptingEventSubProcessCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.NonInterruptingEventSubProcessCompensationScenario;
	using ParallelMultiInstanceCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.ParallelMultiInstanceCompensationScenario;
	using SequentialMultiInstanceCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SequentialMultiInstanceCompensationScenario;
	using SingleActivityCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SingleActivityCompensationScenario;
	using SingleActivityConcurrentCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SingleActivityConcurrentCompensationScenario;
	using JobMigrationScenario = org.camunda.bpm.qa.upgrade.scenarios.job.JobMigrationScenario;
	using SentryScenario = org.camunda.bpm.qa.upgrade.scenarios.sentry.SentryScenario;

	/// <summary>
	/// Sets up scenarios for migration from 7.3.0
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class TestFixture
	{

	  public const string ENGINE_VERSION = "7.3.0";

	  protected internal ProcessEngine processEngine;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;

	  public TestFixture(ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		managementService = processEngine.ManagementService;
		taskService = processEngine.TaskService;
	  }

	  public static void Main(string[] args)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("camunda.cfg.xml");
		ProcessEngine processEngine = processEngineConfiguration.buildProcessEngine();

		// register test scenarios
		ScenarioRunner runner = new ScenarioRunner(processEngine, ENGINE_VERSION);

		// cmmn sentries
		runner.setupScenarios(typeof(SentryScenario));

		// compensation
		runner.setupScenarios(typeof(SingleActivityCompensationScenario));
		runner.setupScenarios(typeof(NestedCompensationScenario));
		runner.setupScenarios(typeof(SingleActivityConcurrentCompensationScenario));
		runner.setupScenarios(typeof(ParallelMultiInstanceCompensationScenario));
		runner.setupScenarios(typeof(SequentialMultiInstanceCompensationScenario));
		runner.setupScenarios(typeof(NestedMultiInstanceCompensationScenario));
		runner.setupScenarios(typeof(InterruptingEventSubProcessCompensationScenario));
		runner.setupScenarios(typeof(NonInterruptingEventSubProcessCompensationScenario));
		runner.setupScenarios(typeof(InterruptingEventSubProcessNestedCompensationScenario));

		// job
		runner.setupScenarios(typeof(JobMigrationScenario));

		// boundary events
		runner.setupScenarios(typeof(NonInterruptingBoundaryEventScenario));

		processEngine.close();
	  }
	}

}