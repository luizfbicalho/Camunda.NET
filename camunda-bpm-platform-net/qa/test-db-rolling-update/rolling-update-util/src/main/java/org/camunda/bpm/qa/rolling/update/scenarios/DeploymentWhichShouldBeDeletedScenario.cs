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
namespace org.camunda.bpm.qa.rolling.update.scenarios
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using DescribesScenario = org.camunda.bpm.qa.upgrade.DescribesScenario;
	using ScenarioSetup = org.camunda.bpm.qa.upgrade.ScenarioSetup;
	using Times = org.camunda.bpm.qa.upgrade.Times;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DeploymentWhichShouldBeDeletedScenario
	{

	  public const string PROCESS_DEF_KEY = "processWithAsyncServiceTask";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/rolling/update/processWithAsyncServiceTask.bpmn20.xml";
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
			engine.RuntimeService.startProcessInstanceByKey(PROCESS_DEF_KEY, scenarioName);
		  }
	  }

	}

}