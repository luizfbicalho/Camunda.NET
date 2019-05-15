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
	public class NestedCompensationScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployProcess()
	  public static string deployProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/nestedCompensationProcess.bpmn20.xml";
	  }

	  [DescribesScenario("init.throwCompensate"), Times(1)]
	  public static ScenarioSetup instantiate()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("NestedCompensationScenario", scenarioName);

			// create the compensation event subscription and wait before throwing compensation
			Task userTask = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).singleResult();
			engine.TaskService.complete(userTask.Id);
		  }
	  }
	}

}