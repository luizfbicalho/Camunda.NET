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
namespace org.camunda.bpm.qa.upgrade.useroperationlog
{
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Yana.Vasileva
	/// 
	/// </summary>
	public class CreateStandaloneTaskDeleteScenario
	{

	  [DescribesScenario("createUserOperationLogEntriesForDelete")]
	  public static ScenarioSetup createUserOperationLogEntries()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			IdentityService identityService = engine.IdentityService;
			identityService.setAuthentication("mary01", null);

			TaskService taskService = engine.TaskService;

			string taskId = "myTaskForUserOperationLogDel";
			Task task = taskService.newTask(taskId);
			taskService.saveTask(task);

			identityService.clearAuthentication();
		  }
	  }
	}

}