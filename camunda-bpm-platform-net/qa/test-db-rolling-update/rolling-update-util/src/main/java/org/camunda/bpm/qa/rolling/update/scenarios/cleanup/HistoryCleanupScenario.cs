using System;
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
namespace org.camunda.bpm.qa.rolling.update.scenarios.cleanup
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using DescribesScenario = org.camunda.bpm.qa.upgrade.DescribesScenario;
	using ScenarioSetup = org.camunda.bpm.qa.upgrade.ScenarioSetup;
	using Times = org.camunda.bpm.qa.upgrade.Times;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class HistoryCleanupScenario
	{

	  internal static readonly DateTime FIXED_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deploy()
	  public static string deploy()
	  {
		return "org/camunda/bpm/qa/rolling/update/cleanup/oneTaskProcess.bpmn20.xml";
	  }

	  [DescribesScenario("initHistoryCleanup"), Times(1)]
	  public static ScenarioSetup initHistoryCleanup()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {

			for (int i = 0; i < 60; i++)
			{
			  if (i % 4 == 0)
			  {
				ClockUtil.CurrentTime = FIXED_DATE;

				engine.RuntimeService.startProcessInstanceByKey("oneTaskProcess_710", "HistoryCleanupScenario");

				string taskId = engine.TaskService.createTaskQuery().processInstanceBusinessKey("HistoryCleanupScenario").singleResult().Id;


				ClockUtil.CurrentTime = addMinutes(FIXED_DATE, i);

				engine.TaskService.complete(taskId);
			  }
			}

			ProcessEngineConfigurationImpl configuration = ((ProcessEngineConfigurationImpl) engine.ProcessEngineConfiguration);

			configuration.HistoryCleanupBatchWindowStartTime = "13:00";
			configuration.HistoryCleanupBatchWindowEndTime = "14:00";
			configuration.HistoryCleanupDegreeOfParallelism = 3;
			configuration.initHistoryCleanup();

			engine.HistoryService.cleanUpHistoryAsync();

			IList<Job> jobs = engine.HistoryService.findHistoryCleanupJobs();

			for (int i = 0; i < 4; i++)
			{
			  Job jobOne = jobs[0];
			  engine.ManagementService.executeJob(jobOne.Id);

			  Job jobTwo = jobs[1];
			  engine.ManagementService.executeJob(jobTwo.Id);

			  Job jobThree = jobs[2];
			  engine.ManagementService.executeJob(jobThree.Id);
			}

			ClockUtil.reset();
		  }
	  }

	  protected internal static DateTime addMinutes(DateTime initialDate, int minutes)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(initialDate);
		calendar.AddMinutes(minutes);
		return calendar;
	  }

	}

}