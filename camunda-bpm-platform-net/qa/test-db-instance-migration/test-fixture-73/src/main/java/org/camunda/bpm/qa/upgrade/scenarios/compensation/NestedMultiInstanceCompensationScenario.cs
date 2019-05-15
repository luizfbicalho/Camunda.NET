using System.Collections.Generic;

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
	public class NestedMultiInstanceCompensationScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployThrowInnerProcess()
	  public static string deployThrowInnerProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/nestedMultiInstanceCompensationThrowInnerProcess.bpmn20.xml";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployThrowOuterProcess()
	  public static string deployThrowOuterProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/compensation/nestedMultiInstanceCompensationThrowOuterProcess.bpmn20.xml";
	  }

	  [DescribesScenario("init.throwInner"), Times(3)]
	  public static ScenarioSetup instantitiateThrowCompensateInSubprocess()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("NestedMultiInstanceCompensationThrowInnerScenario", scenarioName);

			// throw compensation within the mi subprocess
			IList<Task> subProcessTasks = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).list();

			foreach (Task subProcessTask in subProcessTasks)
			{
			  engine.TaskService.complete(subProcessTask.Id);
			}
		  }
	  }

	  [DescribesScenario("init.throwOuter"), Times(3)]
	  public static ScenarioSetup instantitiateThrowCompensateAfterSubprocess()
	  {
		return new ScenarioSetupAnonymousInnerClass2();
	  }

	  private class ScenarioSetupAnonymousInnerClass2 : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			engine.RuntimeService.startProcessInstanceByKey("NestedMultiInstanceCompensationThrowOuterScenario", scenarioName);

			// throw compensation after the mi subprocess has ended
			IList<Task> subProcessTasks = engine.TaskService.createTaskQuery().processInstanceBusinessKey(scenarioName).list();

			foreach (Task subProcessTask in subProcessTasks)
			{
			  engine.TaskService.complete(subProcessTask.Id);
			}
		  }
	  }
	}

}