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
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using BatchWindowConfiguration = org.camunda.bpm.engine.impl.cfg.BatchWindowConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoryCleanupJobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandlerConfiguration;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryCleanupOnEngineBootstrapTest
	{

	  private const string ENGINE_NAME = "engineWithHistoryCleanupBatchWindow";

	  private static SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConsecutiveEngineBootstrapHistoryCleanupJobReconfiguration()
	  public virtual void testConsecutiveEngineBootstrapHistoryCleanupJobReconfiguration()
	  {

		// given
		// create history cleanup job
		ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/batchwindow.camunda.cfg.xml").buildProcessEngine().close();

		// when
		// suspend history cleanup job
		ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/no-batchwindow.camunda.cfg.xml").buildProcessEngine().close();

		// then
		// reconfigure history cleanup job
		ProcessEngineConfiguration processEngineConfiguration = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/batchwindow.camunda.cfg.xml");
		processEngineConfiguration.ProcessEngineName = ENGINE_NAME;
		ProcessEngine processEngine = processEngineConfiguration.buildProcessEngine();

		assertNotNull(ProcessEngines.getProcessEngine(ENGINE_NAME));

		closeProcessEngine(processEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDecreaseNumberOfHistoryCleanupJobs()
	  public virtual void testDecreaseNumberOfHistoryCleanupJobs()
	  {
		// given
		// create history cleanup job
		ProcessEngine engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-parallelism-default.camunda.cfg.xml").buildProcessEngine();

		// assume
		ManagementService managementService = engine.ManagementService;
		assertEquals(4, managementService.createJobQuery().list().size());

		engine.close();

		// when
		engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-parallelism-less.camunda.cfg.xml").buildProcessEngine();

		// then
		// reconfigure history cleanup job
		managementService = engine.ManagementService;
		assertEquals(1, managementService.createJobQuery().list().size());

		Job job = managementService.createJobQuery().singleResult();
		assertEquals(0, getHistoryCleanupJobHandlerConfiguration(job).MinuteFrom);
		assertEquals(59, getHistoryCleanupJobHandlerConfiguration(job).MinuteTo);

		closeProcessEngine(engine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncreaseNumberOfHistoryCleanupJobs()
	  public virtual void testIncreaseNumberOfHistoryCleanupJobs()
	  {
		// given
		// create history cleanup job
		ProcessEngine engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-parallelism-default.camunda.cfg.xml").buildProcessEngine();

		// assume
		ManagementService managementService = engine.ManagementService;
		assertEquals(4, managementService.createJobQuery().count());

		engine.close();

		// when
		engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-parallelism-more.camunda.cfg.xml").buildProcessEngine();

		// then
		// reconfigure history cleanup job
		managementService = engine.ManagementService;
		IList<Job> jobs = managementService.createJobQuery().list();
		assertEquals(8, jobs.Count);

		foreach (Job job in jobs)
		{
		  int minuteTo = getHistoryCleanupJobHandlerConfiguration(job).MinuteTo;
		  int minuteFrom = getHistoryCleanupJobHandlerConfiguration(job).MinuteFrom;

		  if (minuteFrom == 0)
		  {
			assertEquals(6, minuteTo);
		  }
		  else if (minuteFrom == 7)
		  {
			assertEquals(13, minuteTo);
		  }
		  else if (minuteFrom == 14)
		  {
			assertEquals(20, minuteTo);
		  }
		  else if (minuteFrom == 21)
		  {
			assertEquals(27, minuteTo);
		  }
		  else if (minuteFrom == 28)
		  {
			assertEquals(34, minuteTo);
		  }
		  else if (minuteFrom == 35)
		  {
			assertEquals(41, minuteTo);
		  }
		  else if (minuteFrom == 42)
		  {
			assertEquals(48, minuteTo);
		  }
		  else if (minuteFrom == 49)
		  {
			assertEquals(59, minuteTo);
		  }
		  else
		  {
			fail("unexpected minute from " + minuteFrom);
		  }
		}

		closeProcessEngine(engine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWindowXmlConfigParsingException() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBatchWindowXmlConfigParsingException()
	  {
		thrown.expect(typeof(Exception));
		thrown.expectMessage("startTime");

		ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-batch-window-map-wrong-values.camunda.cfg.xml").buildProcessEngine();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWindowMapInXmlConfig() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBatchWindowMapInXmlConfig()
	  {
		// given
		//we're on Monday
		ClockUtil.CurrentTime = sdf.parse("2018-05-14T22:00:00");

		//when
		//we configure batch window only for Wednesday and start the server
		ProcessEngine engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-batch-window-map.camunda.cfg.xml").buildProcessEngine();

		//then
		//history cleanup is scheduled for Wednesday
		IList<Job> historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-16T23:00:00"), historyCleanupJobs[0].Duedate);
		assertEquals(false, historyCleanupJobs[0].Suspended);

		engine.close();

		//when
		//we reconfigure batch window with default values
		engine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/history/history-cleanup-batch-window-default.camunda.cfg.xml").buildProcessEngine();

		//then
		//history cleanup is scheduled for today
		historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-14T23:00:00"), historyCleanupJobs[0].Duedate);
		assertEquals(false, historyCleanupJobs[0].Suspended);

		closeProcessEngine(engine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryCleanupJobScheduled() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHistoryCleanupJobScheduled()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl standaloneInMemProcessEngineConfiguration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl)org.camunda.bpm.engine.ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		ProcessEngineConfigurationImpl standaloneInMemProcessEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		standaloneInMemProcessEngineConfiguration.HistoryCleanupBatchWindowStartTime = "23:00";
		standaloneInMemProcessEngineConfiguration.HistoryCleanupBatchWindowEndTime = "01:00";
		standaloneInMemProcessEngineConfiguration.JdbcUrl = "jdbc:h2:mem:camunda" + this.GetType().Name + "testHistoryCleanupJobScheduled";

		ProcessEngine engine = standaloneInMemProcessEngineConfiguration.buildProcessEngine();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.runtime.Job> historyCleanupJobs = engine.getHistoryService().findHistoryCleanupJobs();
		IList<Job> historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl) engine.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) engine.ProcessEngineConfiguration;
		foreach (Job historyCleanupJob in historyCleanupJobs)
		{
		  assertEquals(processEngineConfiguration.BatchWindowManager.getCurrentOrNextBatchWindow(ClockUtil.CurrentTime, processEngineConfiguration).Start, historyCleanupJob.Duedate);
		}

		closeProcessEngine(engine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWindowOneDayOfWeek() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBatchWindowOneDayOfWeek()
	  {
		ClockUtil.CurrentTime = sdf.parse("2018-05-14T22:00:00"); //monday
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl configuration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl)org.camunda.bpm.engine.ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		//we have batch window only once per week - Monday afternoon
		configuration.HistoryCleanupBatchWindows[DayOfWeek.Monday] = new BatchWindowConfiguration("18:00", "20:00");
		configuration.JdbcUrl = "jdbc:h2:mem:camunda" + this.GetType().Name + "testBatchWindowOneDayOfWeek";

		//when
		//we're on Monday evening
		//and we bootstrap the engine
		ProcessEngine engine = configuration.buildProcessEngine();

		//then
		//job is scheduled for next week Monday
		IList<Job> historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-21T18:00:00"), historyCleanupJobs[0].Duedate); //monday next week
		assertEquals(false, historyCleanupJobs[0].Suspended);

		//when
		//we're on Monday evening next week, right aftre the end of batch window
		ClockUtil.CurrentTime = sdf.parse("2018-05-21T20:00:01"); //monday
		//we force history job to be rescheduled
		engine.ManagementService.executeJob(historyCleanupJobs[0].Id);

		//then
		//job is scheduled for next week Monday
		historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-28T18:00:00"), historyCleanupJobs[0].Duedate); //monday next week
		assertEquals(false, historyCleanupJobs[0].Suspended);

		closeProcessEngine(engine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWindow24Hours() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBatchWindow24Hours()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl configuration = (org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl)org.camunda.bpm.engine.ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();
		//we have batch window for 24 hours
		configuration.HistoryCleanupBatchWindows[DayOfWeek.Monday] = new BatchWindowConfiguration("06:00", "06:00");
		configuration.JdbcUrl = "jdbc:h2:mem:camunda" + this.GetType().Name + "testBatchWindow24Hours";

		//when
		//we're on Monday early morning
		ClockUtil.CurrentTime = sdf.parse("2018-05-14T05:00:00"); //monday
		//and we bootstrap the engine
		ProcessEngine engine = configuration.buildProcessEngine();

		//then
		//job is scheduled for Monday 06 AM
		IList<Job> historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-14T06:00:00"), historyCleanupJobs[0].Duedate);
		assertEquals(false, historyCleanupJobs[0].Suspended);

		//when
		//we're on Monday afternoon
		ClockUtil.CurrentTime = sdf.parse("2018-05-14T15:00:00");
		//we force history job to be rescheduled
		engine.ManagementService.executeJob(historyCleanupJobs[0].Id);

		//then
		//job is still within current batch window
		historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertTrue(sdf.parse("2018-05-15T06:00:00").after(historyCleanupJobs[0].Duedate));
		assertEquals(false, historyCleanupJobs[0].Suspended);

		//when
		//we're on Tuesday early morning close to the end of batch window
		ClockUtil.CurrentTime = sdf.parse("2018-05-15T05:59:00");
		//we force history job to be rescheduled
		engine.ManagementService.executeJob(historyCleanupJobs[0].Id);

		//then
		//job is still within current batch window
		historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertTrue(sdf.parse("2018-05-15T06:00:00").after(historyCleanupJobs[0].Duedate));
		assertEquals(false, historyCleanupJobs[0].Suspended);

		//when
		//we're on Tuesday early morning shortly after the end of batch window
		ClockUtil.CurrentTime = sdf.parse("2018-05-15T06:01:00");
		//we force history job to be rescheduled
		engine.ManagementService.executeJob(historyCleanupJobs[0].Id);

		//then
		//job is rescheduled till next Monday
		historyCleanupJobs = engine.HistoryService.findHistoryCleanupJobs();
		assertFalse(historyCleanupJobs.Count == 0);
		assertEquals(1, historyCleanupJobs.Count);
		assertEquals(sdf.parse("2018-05-21T06:00:00"), historyCleanupJobs[0].Duedate);
		assertEquals(false, historyCleanupJobs[0].Suspended);

		closeProcessEngine(engine);
	  }

	  protected internal virtual HistoryCleanupJobHandlerConfiguration getHistoryCleanupJobHandlerConfiguration(Job job)
	  {
		return HistoryCleanupJobHandlerConfiguration.fromJson(JsonUtil.asObject(((JobEntity) job).JobHandlerConfigurationRaw));
	  }

	  protected internal virtual void closeProcessEngine(ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.HistoryService historyService = processEngine.getHistoryService();
		HistoryService historyService = processEngine.HistoryService;
		configuration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, historyService));

		processEngine.close();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupOnEngineBootstrapTest outerInstance;

		  private HistoryService historyService;

		  public CommandAnonymousInnerClass(HistoryCleanupOnEngineBootstrapTest outerInstance, HistoryService historyService)
		  {
			  this.outerInstance = outerInstance;
			  this.historyService = historyService;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = historyService.findHistoryCleanupJobs();
			foreach (Job job in jobs)
			{
			  commandContext.JobManager.deleteJob((JobEntity) job);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			}

			//cleanup "detached" historic job logs
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricJobLog> list = historyService.createHistoricJobLogQuery().list();
			IList<HistoricJobLog> list = historyService.createHistoricJobLogQuery().list();
			foreach (HistoricJobLog jobLog in list)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobLog.JobId);
			}

			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

	}

}