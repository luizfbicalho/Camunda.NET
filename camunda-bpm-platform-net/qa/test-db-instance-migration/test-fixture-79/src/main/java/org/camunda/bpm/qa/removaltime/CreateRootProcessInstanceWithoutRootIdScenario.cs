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
namespace org.camunda.bpm.qa.removaltime
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using DescribesScenario = org.camunda.bpm.qa.upgrade.DescribesScenario;
	using ScenarioSetup = org.camunda.bpm.qa.upgrade.ScenarioSetup;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class CreateRootProcessInstanceWithoutRootIdScenario
	{

	  [DescribesScenario("initRootProcessInstanceWithoutRootId")]
	  public static ScenarioSetup initRootProcessInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			engine.RepositoryService.createDeployment().addModelInstance("process.bpmn", Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done()).deploy();

			engine.RepositoryService.createDeployment().addModelInstance("rootProcess.bpmn", Bpmn.createExecutableProcess("rootProcess").startEvent().callActivity().calledElement("process").endEvent().done()).deploy();

			engine.RuntimeService.startProcessInstanceByKey("rootProcess", "rootProcessInstance");
		  }
	  }

	}

}