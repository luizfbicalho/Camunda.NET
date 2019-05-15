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
namespace org.camunda.bpm.qa.rolling.update.cleanup
{
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using After = org.junit.After;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[ScenarioUnderTest("HistoryCleanupScenario")]
	public class HistoryCleanupTest : AbstractRollingUpdateTestCase
	{

	  internal static readonly DateTime FIXED_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initHistoryCleanup.1") public void testHistoryCleanup()
	  [ScenarioUnderTest("initHistoryCleanup.1")]
	  public virtual void testHistoryCleanup()
	  {

		if (RollingUpdateConstants.OLD_ENGINE_TAG.Equals(rule.Tag))
		{ // test cleanup with old engine

		  DateTime currentDate = addDays(FIXED_DATE, 1);
		  ClockUtil.CurrentTime = currentDate;

		  ProcessEngineConfigurationImpl configuration = rule.ProcessEngineConfiguration;

		  configuration.HistoryCleanupBatchWindowStartTime = "13:00";
		  configuration.HistoryCleanupBatchWindowEndTime = "15:00";
		  configuration.HistoryCleanupDegreeOfParallelism = 3;
		  configuration.initHistoryCleanup();

		  IList<Job> jobs = rule.HistoryService.findHistoryCleanupJobs();

		  Job jobOne = jobs[0];
		  rule.ManagementService.executeJob(jobOne.Id);

		  Job jobTwo = jobs[1];
		  rule.ManagementService.executeJob(jobTwo.Id);

		  Job jobThree = jobs[2];
		  rule.ManagementService.executeJob(jobThree.Id);

		  jobs = rule.HistoryService.findHistoryCleanupJobs();

		  // assume
		  foreach (Job job in jobs)
		  {
			assertThat(job.Duedate, @is(addSeconds(currentDate, (int)(Math.Pow(2.0, (double)4) * 10))));
		  }

		  IList<HistoricProcessInstance> processInstances = rule.HistoryService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("HistoryCleanupScenario").list();

		  // assume
		  assertThat(jobs.Count, @is(3));
		  assertThat(processInstances.Count, @is(15));

		  ClockUtil.CurrentTime = addDays(currentDate, 5);

		  // when
		  rule.ManagementService.executeJob(jobOne.Id);

		  processInstances = rule.HistoryService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("HistoryCleanupScenario").list();

		  // then
		  assertThat(processInstances.Count, @is(10));

		  // when
		  rule.ManagementService.executeJob(jobTwo.Id);

		  processInstances = rule.HistoryService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("HistoryCleanupScenario").list();

		  // then
		  assertThat(processInstances.Count, @is(5));

		  // when
		  rule.ManagementService.executeJob(jobThree.Id);

		  processInstances = rule.HistoryService.createHistoricProcessInstanceQuery().processInstanceBusinessKey("HistoryCleanupScenario").list();

		  // then
		  assertThat(processInstances.Count, @is(0));
		}
	  }

	  protected internal virtual DateTime addSeconds(DateTime initialDate, int seconds)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(initialDate);
		calendar.AddSeconds(seconds);
		return calendar;
	  }

	  protected internal virtual DateTime addDays(DateTime initialDate, int days)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(initialDate);
		calendar.AddDays(days);
		return calendar;
	  }

	}
}