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
	public class ExternalTaskLockExpTimeScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string TOPIC_NAME = "externalTaskLockExpTimeTestTopic";
	  protected internal const string WORKER_ID = "extTaskLockExpTimeWorkerId";
	  protected internal const long LOCK_TIME = 1000L;

	  protected internal const string PROCESS_DEFINITION_KEY = "oneExtTaskForLockExpTimeTestProcess";
	  protected internal static readonly BpmnModelInstance EXT_TASK_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").serviceTask("extTask").camundaExternalTask(TOPIC_NAME).endEvent("end").done();

	  [DescribesScenario("initExternalTaskLockExpTime"), Times(1)]
	  public static ScenarioSetup initExternalTaskLockExpTime()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string scenarioName)
		  {

			ClockUtil.CurrentTime = TIMESTAMP;

			deployModel(processEngine, PROCESS_DEFINITION_KEY, PROCESS_DEFINITION_KEY, EXT_TASK_MODEL);

			processEngine.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, scenarioName);

			processEngine.ExternalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

			ClockUtil.reset();
		  }
	  }
	}



}