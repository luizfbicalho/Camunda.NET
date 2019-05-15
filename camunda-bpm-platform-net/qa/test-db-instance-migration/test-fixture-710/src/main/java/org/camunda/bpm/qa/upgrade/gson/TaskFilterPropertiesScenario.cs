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
namespace org.camunda.bpm.qa.upgrade.gson
{
	using FilterService = org.camunda.bpm.engine.FilterService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Filter = org.camunda.bpm.engine.filter.Filter;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TaskFilterPropertiesScenario
	{

	  [DescribesScenario("initTaskFilterProperties")]
	  public static ScenarioSetup initTaskFilterProperties()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			FilterService filterService = engine.FilterService;

			Filter filterOne = filterService.newTaskFilter("taskFilterOne");

			IDictionary<string, object> primitivesMap = new Dictionary<string, object>();
			primitivesMap["string"] = "aStringValue";
			primitivesMap["int"] = 47;
			primitivesMap["intOutOfRange"] = int.MaxValue + 1L;
			primitivesMap["long"] = long.MaxValue;
			primitivesMap["double"] = 3.14159265359D;
			primitivesMap["boolean"] = true;
			primitivesMap["null"] = null;

			filterOne.Properties = Collections.singletonMap<string, object>("foo", Collections.singletonList(primitivesMap));
			filterService.saveFilter(filterOne);

			Filter filterTwo = engine.FilterService.newTaskFilter("taskFilterTwo");

			IList<object> primitivesList = new List<object>();
			primitivesList.Add("aStringValue");
			primitivesList.Add(47);
			primitivesList.Add(int.MaxValue + 1L);
			primitivesList.Add(long.MaxValue);
			primitivesList.Add(3.14159265359D);
			primitivesList.Add(true);
			primitivesList.Add(null);

			filterTwo.Properties = Collections.singletonMap<string, object>("foo", Collections.singletonMap("bar", primitivesList));
			filterService.saveFilter(filterTwo);
		  }
	  }
	}

}