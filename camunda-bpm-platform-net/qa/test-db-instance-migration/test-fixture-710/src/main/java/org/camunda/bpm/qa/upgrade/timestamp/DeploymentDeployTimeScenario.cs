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
namespace org.camunda.bpm.qa.upgrade.timestamp
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class DeploymentDeployTimeScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "deploymentDeployTimeProcess";
	  protected internal const string DEPLOYMENT_NAME = "DeployTimeDeploymentTest";
	  protected internal static readonly BpmnModelInstance DEPLOYMENT_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").serviceTask("task").camundaDelegateExpression("${true}").endEvent("end").done();

	  [DescribesScenario("initDeploymentDeployTime"), Times(1)]
	  public static ScenarioSetup initDeploymentDeployTime()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string s)
		  {

			ClockUtil.CurrentTime = TIMESTAMP;

			deployModel(processEngine, DEPLOYMENT_NAME, PROCESS_DEFINITION_KEY, DEPLOYMENT_MODEL);

			ClockUtil.reset();
		  }
	  }
	}
}