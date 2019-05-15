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
	public class SubprocessCompensationScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployProcess()
	  public static string deployProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/subprocessCompensationProcess.bpmn20.xml";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployConcurrentCompensationProcess()
	  public static string deployConcurrentCompensationProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/subprocessConcurrentCompensationProcess.bpmn20.xml";
	  }

	  [DescribesScenario("init"), Times(3)]
	  public static ScenarioSetup instantiate()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SubprocessCompensationScenario", scenarioName);

			// create the compensation event subscription and wait before throwing compensation
			Task userTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(userTask.Id);
		  }
	  }

	  [DescribesScenario("init.triggerCompensation"), ExtendsScenario("init"), Times(3)]
	  public static ScenarioSetup instantiateAndTriggerCompensation()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			// throw compensation; the compensation handler for userTask should then be active
			Task beforeCompensateTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(beforeCompensateTask.Id);
		  }
	  }

	  [DescribesScenario("init.concurrent"), Times(3)]
	  public static ScenarioSetup instantiateConcurrent()
	  {
		return new ScenarioSetupAnonymousInnerClass3();
	  }

	  private class ScenarioSetupAnonymousInnerClass3 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("SubprocessConcurrentCompensationScenario", scenarioName);

			// create the compensation event subscriptions and wait before throwing compensation
			Task userTask1 = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(userTask1.Id);

			Task userTask2 = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(userTask2.Id);
		  }
	  }

	  [DescribesScenario("init.concurrent.triggerCompensation"), ExtendsScenario("init.concurrent"), Times(3)]
	  public static ScenarioSetup instantiateConcurrentAndTriggerCompensation()
	  {
		return new ScenarioSetupAnonymousInnerClass4();
	  }

	  private class ScenarioSetupAnonymousInnerClass4 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			// throw compensation; the compensation handler for userTask should then be active
			Task beforeCompensateTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(beforeCompensateTask.Id);
		  }
	  }

	}

}