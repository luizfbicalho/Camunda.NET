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
namespace org.camunda.bpm.qa.upgrade.useroperationlog
{

	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using Task = org.camunda.bpm.engine.task.Task;
	using Deployment = org.camunda.bpm.engine.test.Deployment;

	/// <summary>
	/// @author Yana.Vasileva
	/// 
	/// </summary>
	public class SetAssigneeProcessInstanceTaskScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/upgrade/useroperationlog/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("createUserOperationLogEntries")]
	  public static ScenarioSetup createUserOperationLogEntries()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			IdentityService identityService = engine.IdentityService;
			string processInstanceBusinessKey = "SetAssigneeProcessInstanceTaskScenario";
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess_userOpLog", processInstanceBusinessKey);

			identityService.setAuthentication("mary02", null);

			TaskService taskService = engine.TaskService;
			IList<Task> list = taskService.createTaskQuery().processInstanceBusinessKey(processInstanceBusinessKey).list();
			Task task = list[0];
			taskService.setAssignee(task.Id, "john");

			identityService.clearAuthentication();
		  }
	  }
	}

}