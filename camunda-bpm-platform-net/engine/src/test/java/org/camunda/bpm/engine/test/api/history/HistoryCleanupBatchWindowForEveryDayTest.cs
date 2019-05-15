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
namespace org.camunda.bpm.engine.test.api.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class HistoryCleanupBatchWindowForEveryDayTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryCleanupBatchWindowForEveryDayTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupBatchWindowForEveryDayTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal string defaultStartTime;
	  protected internal string defaultEndTime;
	  protected internal int defaultBatchSize;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.HistoryCleanupBatchSize = 20;
			configuration.HistoryCleanupBatchThreshold = 10;
			configuration.DefaultNumberOfRetries = 5;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  private HistoryService historyService;
	  private ManagementService managementService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

	  private static SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(0) public String startTime;
	  public string startTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(1) public String endTime;
	  public string endTime;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(2) public java.util.Date startDateForCheck;
	  public DateTime startDateForCheck;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(3) public java.util.Date endDateForCheck;
	  public DateTime endDateForCheck;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter(4) public java.util.Date currentDate;
	  public DateTime currentDate;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters public static java.util.Collection<Object[]> scenarios() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"22:00", "23:00", sdf.parse("2017-09-06T22:00:00"), sdf.parse("2017-09-06T23:00:00"), sdf.parse("2017-09-06T22:15:00")},
			new object[] {"23:00", "01:00", sdf.parse("2017-09-06T23:00:00"), sdf.parse("2017-09-07T01:00:00"), sdf.parse("2017-09-07T00:15:00")},
			new object[] {"00:00", "00:00", sdf.parse("2017-09-06T00:00:00"), sdf.parse("2017-09-07T00:00:00"), sdf.parse("2017-09-06T15:00:00")},
			new object[] {"00:00", "00:00", sdf.parse("2017-09-06T00:00:00"), sdf.parse("2017-09-07T00:00:00"), sdf.parse("2017-09-06T00:00:00")},
			new object[] {"22:00", "23:00", sdf.parse("2017-09-06T22:00:00"), sdf.parse("2017-09-06T23:00:00"), sdf.parse("2017-09-06T21:15:00")},
			new object[] {"22:00", "23:00", sdf.parse("2017-09-07T22:00:00"), sdf.parse("2017-09-07T23:00:00"), sdf.parse("2017-09-06T23:15:00")},
			new object[] {"22:00", "23:00", sdf.parse("2017-09-07T22:00:00"), sdf.parse("2017-09-07T23:00:00"), sdf.parse("2017-09-07T00:15:00")}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		managementService = engineRule.ManagementService;

		defaultStartTime = processEngineConfiguration.HistoryCleanupBatchWindowStartTime;
		defaultEndTime = processEngineConfiguration.HistoryCleanupBatchWindowEndTime;
		defaultBatchSize = processEngineConfiguration.HistoryCleanupBatchSize;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		//reset configuration changes
		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = defaultStartTime;
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = defaultEndTime;
		processEngineConfiguration.HistoryCleanupBatchSize = defaultBatchSize;

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupBatchWindowForEveryDayTest outerInstance;

		  public CommandAnonymousInnerClass(HistoryCleanupBatchWindowForEveryDayTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.managementService.createJobQuery().list();
			if (jobs.Count > 0)
			{
			  assertEquals(1, jobs.Count);
			  string jobId = jobs[0].Id;
			  commandContext.JobManager.deleteJob((JobEntity) jobs[0]);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScheduleJobForBatchWindow() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScheduleJobForBatchWindow()
	  {
		ClockUtil.CurrentTime = currentDate;

		processEngineConfiguration.HistoryCleanupBatchWindowStartTime = startTime;
		processEngineConfiguration.HistoryCleanupBatchWindowEndTime = endTime;

		processEngineConfiguration.initHistoryCleanup();

		Job job = historyService.cleanUpHistoryAsync();

		assertFalse(startDateForCheck > job.Duedate); // job due date is not before start date
		assertTrue(endDateForCheck > job.Duedate);
	  }
	}

}