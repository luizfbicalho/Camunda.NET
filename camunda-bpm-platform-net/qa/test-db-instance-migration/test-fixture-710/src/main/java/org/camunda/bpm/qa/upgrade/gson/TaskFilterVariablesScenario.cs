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
namespace org.camunda.bpm.qa.upgrade.gson
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TaskFilterVariablesScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/upgrade/gson/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("initTaskFilterVariables")]
	  public static ScenarioSetup initTaskFilterVariables()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			// boolean filter
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterBooleanVariable", Variables.createVariables().putValue("booleanVariable", true));

			TaskQuery query = engine.TaskService.createTaskQuery().processVariableValueEquals("booleanVariable", true);

			Filter filter = engine.FilterService.newTaskFilter("filterBooleanVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);

			// int filter
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterIntVariable", Variables.createVariables().putValue("intVariable", 7));

			query = engine.TaskService.createTaskQuery().processVariableValueEquals("intVariable", 7);

			filter = engine.FilterService.newTaskFilter("filterIntVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);

			// int out of range filter
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterIntOutOfRangeVariable", Variables.createVariables().putValue("longVariable", int.MaxValue+1L));

			query = engine.TaskService.createTaskQuery().processVariableValueEquals("longVariable", int.MaxValue+1L);

			filter = engine.FilterService.newTaskFilter("filterIntOutOfRangeVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);

			// double filter
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterDoubleVariable", Variables.createVariables().putValue("doubleVariable", 88.89D));

			query = engine.TaskService.createTaskQuery().processVariableValueEquals("doubleVariable", 88.89D);

			filter = engine.FilterService.newTaskFilter("filterDoubleVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);

			// string filter
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterStringVariable", Variables.createVariables().putValue("stringVariable", "aVariableValue"));

			query = engine.TaskService.createTaskQuery().processVariableValueEquals("stringVariable", "aVariableValue");

			filter = engine.FilterService.newTaskFilter("filterStringVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);

			// filter null
			engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", "TaskFilterVariablesScenario_filterNullVariable", Variables.createVariables().putValue("nullVariable", null));

			query = engine.TaskService.createTaskQuery().processVariableValueEquals("nullVariable", null);

			filter = engine.FilterService.newTaskFilter("filterNullVariable");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);
		  }
	  }
	}

}