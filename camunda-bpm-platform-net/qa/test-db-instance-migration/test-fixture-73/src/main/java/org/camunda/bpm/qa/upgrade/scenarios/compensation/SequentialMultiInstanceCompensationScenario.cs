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
namespace org.camunda.bpm.qa.upgrade.scenarios.compensation
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SequentialMultiInstanceCompensationScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploySingleActivityHandler()
	  public static string deploySingleActivityHandler()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/sequentialMultiInstanceCompensationSingleActivityHandlerProcess.bpmn20.xml";
	  }

	  [DescribesScenario("singleActivityHandler.multiInstancePartial"), Times(3)]
	  public static ScenarioSetup singleActivityHandlerMultiInstancePartial()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationSingleActivityHandlerScenario", scenarioName);

			// complete two out of three MI tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("singleActivityHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup singleActivityHandlerBeforeCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationSingleActivityHandlerScenario", scenarioName);

			// complete all mi tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("singleActivityHandler.beforeCompensate.throwCompensate"), ExtendsScenario("singleActivityHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup singleActivityHandlerThrowCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass3();
	  }

	  private class ScenarioSetupAnonymousInnerClass3 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			Task beforeCompensateTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();

			engine.TaskService.complete(beforeCompensateTask.Id);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployDefaultHandler()
	  public static string deployDefaultHandler()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/sequentialMultiInstanceCompensationDefaultHandlerProcess.bpmn20.xml";
	  }

	  [DescribesScenario("defaultHandler.multiInstancePartial"), Times(3)]
	  public static ScenarioSetup defaultHandlerMultiInstancePartial()
	  {
		return new ScenarioSetupAnonymousInnerClass4();
	  }

	  private class ScenarioSetupAnonymousInnerClass4 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationDefaultHandlerScenario", scenarioName);

			// complete two out of three MI tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("defaultHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup defaultHandlerBeforeCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass5();
	  }

	  private class ScenarioSetupAnonymousInnerClass5 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationDefaultHandlerScenario", scenarioName);

			// complete all mi tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("defaultHandler.beforeCompensate.throwCompensate"), ExtendsScenario("defaultHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup defaultHandlerThrowCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass6();
	  }

	  private class ScenarioSetupAnonymousInnerClass6 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			Task beforeCompensateTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();

			engine.TaskService.complete(beforeCompensateTask.Id);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploySubProcessHandler()
	  public static string deploySubProcessHandler()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/sequentialMultiInstanceCompensationSubProcessHandlerProcess.bpmn20.xml";
	  }

	  [DescribesScenario("subProcessHandler.multiInstancePartial"), Times(3)]
	  public static ScenarioSetup subProcessHandlerMultiInstancePartial()
	  {
		return new ScenarioSetupAnonymousInnerClass7();
	  }

	  private class ScenarioSetupAnonymousInnerClass7 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationSubProcessHandlerScenario", scenarioName);

			// complete two out of three MI tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("subProcessHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup subProcessHandlerBeforeCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass8();
	  }

	  private class ScenarioSetupAnonymousInnerClass8 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SequentialMultiInstanceCompensationSubProcessHandlerScenario", scenarioName);

			// complete all mi tasks
			Task miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);

			miTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(miTask.Id);
		  }
	  }

	  [DescribesScenario("subProcessHandler.beforeCompensate.throwCompensate"), ExtendsScenario("subProcessHandler.beforeCompensate"), Times(3)]
	  public static ScenarioSetup subProcessHandlerThrowCompensate()
	  {
		return new ScenarioSetupAnonymousInnerClass9();
	  }

	  private class ScenarioSetupAnonymousInnerClass9 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			Task beforeCompensateTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();

			engine.TaskService.complete(beforeCompensateTask.Id);
		  }
	  }
	}

}