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
namespace org.camunda.bpm.qa.rolling.update.scenarios.externalTask
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using DescribesScenario = org.camunda.bpm.qa.upgrade.DescribesScenario;
	using ScenarioSetup = org.camunda.bpm.qa.upgrade.ScenarioSetup;
	using Times = org.camunda.bpm.qa.upgrade.Times;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ProcessWithExternalTaskScenario
	{

	  public const string PROCESS_DEF_KEY = "processWithExternalTask";
	  public const string EXTERNAL_TASK = "externalTask";
	  public const string EXTERNAL_TASK_TYPE = "external";
	  public const long LOCK_TIME = 5 * 60 * 1000;

	  /// <summary>
	  /// Deploy a process model, which contains an external task. The topic is
	  /// given via parameter so the test cases are independent.
	  /// </summary>
	  /// <param name="engine"> the engine which is used to deploy the instance </param>
	  /// <param name="topicName"> the topic name for the external task </param>
	  public static void deploy(ProcessEngine engine, string topicName)
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess(PROCESS_DEF_KEY).startEvent().serviceTask(EXTERNAL_TASK).camundaType(EXTERNAL_TASK_TYPE).camundaTopic(topicName).endEvent().done();

		engine.RepositoryService.createDeployment().addModelInstance(typeof(ProcessWithExternalTaskScenario).Name + ".startProcessWithFetch.bpmn20.xml", instance).deploy();
	  }

	  [DescribesScenario("init"), Times(1)]
	  public static ScenarioSetup startProcess()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {

		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			deploy(engine, scenarioName);
			engine.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, scenarioName);
		  }
	  }


	  [DescribesScenario("init.fetch"), Times(1)]
	  public static ScenarioSetup startProcessWithFetch()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {

		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			deploy(engine, scenarioName);

			engine.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, scenarioName);
			engine.ExternalTaskService.fetchAndLock(1, scenarioName).topic(scenarioName, LOCK_TIME).execute();
		  }
	  }
	}

}