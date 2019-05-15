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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class JobTimestampsScenario : AbstractTimestampMigrationScenario
	{

	  protected internal static readonly SimpleDateFormat SDF = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
	  protected internal static readonly DateTime LOCK_EXP_TIME = new DateTime(TIME + 300_000L);
	  protected internal const string PROCESS_DEFINITION_KEY = "jobTimestampsMigrationTestProcess";
	  protected internal static readonly BpmnModelInstance SINGLE_JOB_MODEL = Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent("start").intermediateCatchEvent("catch").timerWithDate(SDF.format(TIMESTAMP)).endEvent("end").done();

	  [DescribesScenario("initJobTimestamps"), Times(1)]
	  public static ScenarioSetup initJobTimestamps()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void execute(final org.camunda.bpm.engine.ProcessEngine processEngine, String scenarioName)
		  public void execute(ProcessEngine processEngine, string scenarioName)
		  {

			ClockUtil.CurrentTime = TIMESTAMP;

			deployModel(processEngine, PROCESS_DEFINITION_KEY, PROCESS_DEFINITION_KEY, SINGLE_JOB_MODEL);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = processEngine.getRuntimeService().startProcessInstanceByKey(PROCESS_DEFINITION_KEY, scenarioName).getId();
			string processInstanceId = processEngine.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY, scenarioName).Id;

			((ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, processEngine, processInstanceId));

			ClockUtil.reset();
		  }

		  private class CommandAnonymousInnerClass : Command<Void>
		  {
			  private readonly ScenarioSetupAnonymousInnerClass outerInstance;

			  private ProcessEngine processEngine;
			  private string processInstanceId;

			  public CommandAnonymousInnerClass(ScenarioSetupAnonymousInnerClass outerInstance, ProcessEngine processEngine, string processInstanceId)
			  {
				  this.outerInstance = outerInstance;
				  this.processEngine = processEngine;
				  this.processInstanceId = processInstanceId;
			  }

			  public Void execute(CommandContext commandContext)
			  {

				JobEntity job = (JobEntity) processEngine.ManagementService.createJobQuery().processInstanceId(processInstanceId).singleResult();

				job.LockExpirationTime = LOCK_EXP_TIME;

				commandContext.JobManager.updateJob(job);

				return null;
			  }
		  }
	  }
	}
}