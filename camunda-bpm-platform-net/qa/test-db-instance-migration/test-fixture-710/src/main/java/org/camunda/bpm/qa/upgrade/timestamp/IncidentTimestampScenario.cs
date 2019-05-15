using System;

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
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class IncidentTimestampScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "oneIncidentTimestampServiceTaskProcess";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly BpmnModelInstance FAILING_SERVICE_TASK_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").serviceTask("incidentTimestampTask").camundaAsyncBefore().camundaClass(typeof(FailingDelegate).FullName).endEvent("end").done();

	  [DescribesScenario("initIncidentTimestamp"), Times(1)]
	  public static ScenarioSetup initIncidentTimestamp()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string scenarioName)
		  {

			ClockUtil.CurrentTime = TIMESTAMP;

			deployModel(processEngine, PROCESS_DEFINITION_KEY, PROCESS_DEFINITION_KEY, FAILING_SERVICE_TASK_MODEL);

			string processInstanceId = processEngine.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, scenarioName).Id;

			causeIncident(processEngine, processInstanceId);

			ClockUtil.reset();
		  }
	  }

	  private static void causeIncident(ProcessEngine processEngine, string processInstanceId)
	  {

		Job job = processEngine.ManagementService.createJobQuery().processInstanceId(processInstanceId).withRetriesLeft().singleResult();

		if (job == null)
		{
		  return;
		}

		try
		{
		  processEngine.ManagementService.executeJob(job.Id);
		}
		catch (Exception)
		{
		  // noop
		}

		causeIncident(processEngine, processInstanceId);
	  }

	  internal class FailingDelegate : JavaDelegate
	  {

		public const string EXCEPTION_MESSAGE = "Expected_exception.";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{

		  bool? fail = (bool?) execution.getVariable("fail");

		  if (fail == null || fail == true)
		  {
			throw new ProcessEngineException(EXCEPTION_MESSAGE);
		  }

		}

	  }
	}
}