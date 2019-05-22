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
	using NestedNonInterruptingBoundaryEventOnInnerSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.boundary.NestedNonInterruptingBoundaryEventOnInnerSubprocessScenario;
	using NestedNonInterruptingBoundaryEventOnOuterSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.boundary.NestedNonInterruptingBoundaryEventOnOuterSubprocessScenario;
	using NonInterruptingBoundaryEventScenario = org.camunda.bpm.qa.upgrade.scenarios.boundary.NonInterruptingBoundaryEventScenario;
	using InterruptingEventSubprocessCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.InterruptingEventSubprocessCompensationScenario;
	using SingleActivityCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SingleActivityCompensationScenario;
	using SubprocessCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SubprocessCompensationScenario;
	using SubprocessParallelCreateCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SubprocessParallelCreateCompensationScenario;
	using SubprocessParallelThrowCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.SubprocessParallelThrowCompensationScenario;
	using TransactionCancelCompensationScenario = org.camunda.bpm.qa.upgrade.scenarios.compensation.TransactionCancelCompensationScenario;
	using InterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.InterruptingEventSubprocessScenario;
	using NestedInterruptingErrorEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NestedInterruptingErrorEventSubprocessScenario;
	using NestedInterruptingEventSubprocessParallelScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NestedInterruptingEventSubprocessParallelScenario;
	using NestedNonInterruptingEventSubprocessNestedSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NestedNonInterruptingEventSubprocessNestedSubprocessScenario;
	using NestedNonInterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NestedNonInterruptingEventSubprocessScenario;
	using NestedParallelNonInterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NestedParallelNonInterruptingEventSubprocessScenario;
	using NonInterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.NonInterruptingEventSubprocessScenario;
	using ParallelNestedNonInterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.ParallelNestedNonInterruptingEventSubprocessScenario;
	using TwoLevelNestedNonInterruptingEventSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.eventsubprocess.TwoLevelNestedNonInterruptingEventSubprocessScenario;
	using EventBasedGatewayScenario = org.camunda.bpm.qa.upgrade.scenarios.gateway.EventBasedGatewayScenario;
	using AsyncParallelMultiInstanceScenario = org.camunda.bpm.qa.upgrade.scenarios.job.AsyncParallelMultiInstanceScenario;
	using AsyncSequentialMultiInstanceScenario = org.camunda.bpm.qa.upgrade.scenarios.job.AsyncSequentialMultiInstanceScenario;
	using MultiInstanceReceiveTaskScenario = org.camunda.bpm.qa.upgrade.scenarios.multiinstance.MultiInstanceReceiveTaskScenario;
	using NestedSequentialMultiInstanceSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.multiinstance.NestedSequentialMultiInstanceSubprocessScenario;
	using ParallelMultiInstanceSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.multiinstance.ParallelMultiInstanceSubprocessScenario;
	using SequentialMultiInstanceSubprocessScenario = org.camunda.bpm.qa.upgrade.scenarios.multiinstance.SequentialMultiInstanceSubprocessScenario;
	using OneScopeTaskScenario = org.camunda.bpm.qa.upgrade.scenarios.task.OneScopeTaskScenario;
	using OneTaskScenario = org.camunda.bpm.qa.upgrade.scenarios.task.OneTaskScenario;
	using ParallelScopeTasksScenario = org.camunda.bpm.qa.upgrade.scenarios.task.ParallelScopeTasksScenario;
	using ParallelTasksScenario = org.camunda.bpm.qa.upgrade.scenarios.task.ParallelTasksScenario;

	/// <summary>
	/// Sets up scenarios for migration from 7.3.0
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class TestFixture
	{

	  public const string ENGINE_VERSION = "7.2.0";

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

		// event subprocesses
		runner.setupScenarios(typeof(InterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(NonInterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(NestedNonInterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(ParallelNestedNonInterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(NestedParallelNonInterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(NestedNonInterruptingEventSubprocessNestedSubprocessScenario));
		runner.setupScenarios(typeof(NestedInterruptingErrorEventSubprocessScenario));
		runner.setupScenarios(typeof(TwoLevelNestedNonInterruptingEventSubprocessScenario));
		runner.setupScenarios(typeof(NestedInterruptingEventSubprocessParallelScenario));

		// multi instance
		runner.setupScenarios(typeof(SequentialMultiInstanceSubprocessScenario));
		runner.setupScenarios(typeof(NestedSequentialMultiInstanceSubprocessScenario));
		runner.setupScenarios(typeof(MultiInstanceReceiveTaskScenario));
		runner.setupScenarios(typeof(ParallelMultiInstanceSubprocessScenario));

		// async
		runner.setupScenarios(typeof(AsyncParallelMultiInstanceScenario));
		runner.setupScenarios(typeof(AsyncSequentialMultiInstanceScenario));

		// boundary event
		runner.setupScenarios(typeof(NonInterruptingBoundaryEventScenario));
		runner.setupScenarios(typeof(NestedNonInterruptingBoundaryEventOnInnerSubprocessScenario));
		runner.setupScenarios(typeof(NestedNonInterruptingBoundaryEventOnOuterSubprocessScenario));

		// compensation
		runner.setupScenarios(typeof(SingleActivityCompensationScenario));
		runner.setupScenarios(typeof(SubprocessCompensationScenario));
		runner.setupScenarios(typeof(TransactionCancelCompensationScenario));
		runner.setupScenarios(typeof(InterruptingEventSubprocessCompensationScenario));
		runner.setupScenarios(typeof(SubprocessParallelThrowCompensationScenario));
		runner.setupScenarios(typeof(SubprocessParallelCreateCompensationScenario));

		// plain tasks
		runner.setupScenarios(typeof(OneTaskScenario));
		runner.setupScenarios(typeof(OneScopeTaskScenario));
		runner.setupScenarios(typeof(ParallelTasksScenario));
		runner.setupScenarios(typeof(ParallelScopeTasksScenario));

		// event-based gateway
		runner.setupScenarios(typeof(EventBasedGatewayScenario));

		processEngine.close();
	  }
	}

}