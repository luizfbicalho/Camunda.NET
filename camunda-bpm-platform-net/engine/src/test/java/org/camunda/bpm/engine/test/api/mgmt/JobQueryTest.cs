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
namespace org.camunda.bpm.engine.test.api.mgmt
{
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DeleteJobsCmd = org.camunda.bpm.engine.impl.cmd.DeleteJobsCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class JobQueryTest
	public class JobQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobQueryTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal RuntimeService runtimeService;
	  protected internal RepositoryService repositoryService;
	  protected internal ManagementService managementService;
	  private CommandExecutor commandExecutor;

	  private string deploymentId;
	  private string messageId;
	  private TimerEntity timerEntity;
	  private bool defaultEnsureJobDueDateSet;

	  private DateTime testStartTime;
	  private DateTime timerOneFireTime;
	  private DateTime timerTwoFireTime;
	  private DateTime timerThreeFireTime;
	  private DateTime messageDueDate;

	  private string processInstanceIdOne;
	  private string processInstanceIdTwo;
	  private string processInstanceIdThree;

	  private static readonly long ONE_HOUR = 60L * 60L * 1000L;
	  private const long ONE_SECOND = 1000L;
	  private const string EXCEPTION_MESSAGE = "java.lang.RuntimeException: This is an exception thrown from scriptTask";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public boolean ensureJobDueDateSet;
	  public bool ensureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Job DueDate is set: {0}") public static java.util.Collection<Object[]> scenarios() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {false},
			new object[] {true}
		});
	  }

	  /// <summary>
	  /// Setup will create
	  ///   - 3 process instances, each with one timer, each firing at t1/t2/t3 + 1 hour (see process)
	  ///   - 1 message
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		processEngineConfiguration = rule.ProcessEngineConfiguration;
		runtimeService = rule.RuntimeService;
		repositoryService = rule.RepositoryService;
		managementService = rule.ManagementService;
		commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;

		defaultEnsureJobDueDateSet = processEngineConfiguration.EnsureJobDueDateNotNull;
		processEngineConfiguration.EnsureJobDueDateNotNull = ensureJobDueDateSet;

		deploymentId = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/mgmt/timerOnTask.bpmn20.xml").deploy().Id;

		// Create proc inst that has timer that will fire on t1 + 1 hour
		DateTime startTime = new DateTime();
		startTime.set(DateTime.MILLISECOND, 0);

		DateTime t1 = startTime;
		ClockUtil.CurrentTime = t1;
		processInstanceIdOne = runtimeService.startProcessInstanceByKey("timerOnTask").Id;
		testStartTime = t1;
		timerOneFireTime = new DateTime(t1.Ticks + ONE_HOUR);

		// Create proc inst that has timer that will fire on t2 + 1 hour
		startTime.AddHours(1);
		DateTime t2 = startTime; // t2 = t1 + 1 hour
		ClockUtil.CurrentTime = t2;
		processInstanceIdTwo = runtimeService.startProcessInstanceByKey("timerOnTask").Id;
		timerTwoFireTime = new DateTime(t2.Ticks + ONE_HOUR);

		// Create proc inst that has timer that will fire on t3 + 1 hour
		startTime.AddHours(1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date t3 = startTime.getTime();
		DateTime t3 = startTime; // t3 = t2 + 1 hour
		ClockUtil.CurrentTime = t3;
		processInstanceIdThree = runtimeService.startProcessInstanceByKey("timerOnTask").Id;
		timerThreeFireTime = new DateTime(t3.Ticks + ONE_HOUR);

		// Message.StartTime = Message.DueDate
		startTime.AddHours(2);
		messageDueDate = startTime;

		// Create one message
		messageId = commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly JobQueryTest outerInstance;

		  public CommandAnonymousInnerClass(JobQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string execute(CommandContext commandContext)
		  {
			MessageEntity message = new MessageEntity();

			if (outerInstance.ensureJobDueDateSet)
			{
			  message.Duedate = outerInstance.messageDueDate;
			}

			commandContext.JobManager.send(message);
			return message.Id;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		repositoryService.deleteDeployment(deploymentId, true);
		commandExecutor.execute(new DeleteJobsCmd(messageId, true));
		processEngineConfiguration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNoCriteria()
	  public virtual void testQueryByNoCriteria()
	  {
		JobQuery query = managementService.createJobQuery();
		verifyQueryResults(query, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityId()
	  public virtual void testQueryByActivityId()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		JobQuery query = managementService.createJobQuery().activityId(jobDefinition.ActivityId);
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidActivityId()
	  public virtual void testQueryByInvalidActivityId()
	  {
		JobQuery query = managementService.createJobQuery().activityId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().activityId(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testByJobDefinitionId()
	  public virtual void testByJobDefinitionId()
	  {
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		JobQuery query = managementService.createJobQuery().jobDefinitionId(jobDefinition.Id);
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testByInvalidJobDefinitionId()
	  public virtual void testByInvalidJobDefinitionId()
	  {
		JobQuery query = managementService.createJobQuery().jobDefinitionId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().jobDefinitionId(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		JobQuery query = managementService.createJobQuery().processInstanceId(processInstanceIdOne);
		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessInstanceId()
	  public virtual void testQueryByInvalidProcessInstanceId()
	  {
		JobQuery query = managementService.createJobQuery().processInstanceId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().processInstanceId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionId()
	  public virtual void testQueryByExecutionId()
	  {
		Job job = managementService.createJobQuery().processInstanceId(processInstanceIdOne).singleResult();
		JobQuery query = managementService.createJobQuery().executionId(job.ExecutionId);
		assertEquals(query.singleResult().Id, job.Id);
		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidExecutionId()
	  public virtual void testQueryByInvalidExecutionId()
	  {
		JobQuery query = managementService.createJobQuery().executionId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().executionId(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionId()
	  public virtual void testQueryByProcessDefinitionId()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().list().get(0);

		JobQuery query = managementService.createJobQuery().processDefinitionId(processDefinition.Id);
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessDefinitionId()
	  public virtual void testQueryByInvalidProcessDefinitionId()
	  {
		JobQuery query = managementService.createJobQuery().processDefinitionId("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().processDefinitionId(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/JobQueryTest.testTimeCycleQueryByProcessDefinitionId.bpmn20.xml"}) public void testTimeCycleQueryByProcessDefinitionId()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/JobQueryTest.testTimeCycleQueryByProcessDefinitionId.bpmn20.xml"})]
	  public virtual void testTimeCycleQueryByProcessDefinitionId()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult().Id;

		JobQuery query = managementService.createJobQuery().processDefinitionId(processDefinitionId);

		verifyQueryResults(query, 1);

		string jobId = query.singleResult().Id;
		managementService.executeJob(jobId);

		verifyQueryResults(query, 1);

		string anotherJobId = query.singleResult().Id;
		assertFalse(jobId.Equals(anotherJobId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKey()
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		JobQuery query = managementService.createJobQuery().processDefinitionKey("timerOnTask");
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessDefinitionKey()
	  public virtual void testQueryByInvalidProcessDefinitionKey()
	  {
		JobQuery query = managementService.createJobQuery().processDefinitionKey("invalid");
		verifyQueryResults(query, 0);

		try
		{
		  managementService.createJobQuery().processDefinitionKey(null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/JobQueryTest.testTimeCycleQueryByProcessDefinitionId.bpmn20.xml"}) public void testTimeCycleQueryByProcessDefinitionKey()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/JobQueryTest.testTimeCycleQueryByProcessDefinitionId.bpmn20.xml"})]
	  public virtual void testTimeCycleQueryByProcessDefinitionKey()
	  {
		JobQuery query = managementService.createJobQuery().processDefinitionKey("process");

		verifyQueryResults(query, 1);

		string jobId = query.singleResult().Id;
		managementService.executeJob(jobId);

		verifyQueryResults(query, 1);

		string anotherJobId = query.singleResult().Id;
		assertFalse(jobId.Equals(anotherJobId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRetriesLeft()
	  public virtual void testQueryByRetriesLeft()
	  {
		JobQuery query = managementService.createJobQuery().withRetriesLeft();
		verifyQueryResults(query, 4);

		setRetries(processInstanceIdOne, 0);
		// Re-running the query should give only 3 jobs now, since one job has retries=0
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutable()
	  public virtual void testQueryByExecutable()
	  {
		long testTime = ensureJobDueDateSet? messageDueDate.Ticks : timerThreeFireTime.Ticks;
		int expectedCount = ensureJobDueDateSet? 0 : 1;

		ClockUtil.CurrentTime = new DateTime(testTime + ONE_SECOND); // all jobs should be executable at t3 + 1hour.1second
		JobQuery query = managementService.createJobQuery().executable();
		verifyQueryResults(query, 4);

		// Setting retries of one job to 0, makes it non-executable
		setRetries(processInstanceIdOne, 0);
		verifyQueryResults(query, 3);

		// Setting the clock before the start of the process instance, makes none of the jobs executable
		ClockUtil.CurrentTime = testStartTime;
		verifyQueryResults(query, expectedCount); // 1, since a message is always executable when retries > 0
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByOnlyTimers()
	  public virtual void testQueryByOnlyTimers()
	  {
		JobQuery query = managementService.createJobQuery().timers();
		verifyQueryResults(query, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByOnlyMessages()
	  public virtual void testQueryByOnlyMessages()
	  {
		JobQuery query = managementService.createJobQuery().messages();
		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidOnlyTimersUsage()
	  public virtual void testInvalidOnlyTimersUsage()
	  {
		try
		{
		  managementService.createJobQuery().timers().messages().list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot combine onlyTimers() with onlyMessages() in the same query"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDuedateLowerThen()
	  public virtual void testQueryByDuedateLowerThen()
	  {
		JobQuery query = managementService.createJobQuery().duedateLowerThen(testStartTime);
		verifyQueryResults(query, 0);

		query = managementService.createJobQuery().duedateLowerThen(new DateTime(timerOneFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, 1);

		query = managementService.createJobQuery().duedateLowerThen(new DateTime(timerTwoFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, 2);

		query = managementService.createJobQuery().duedateLowerThen(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, 3);

		if (ensureJobDueDateSet)
		{
		  query = managementService.createJobQuery().duedateLowerThen(new DateTime(messageDueDate.Ticks + ONE_SECOND));
		  verifyQueryResults(query, 4);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDuedateLowerThenOrEqual()
	  public virtual void testQueryByDuedateLowerThenOrEqual()
	  {
		JobQuery query = managementService.createJobQuery().duedateLowerThenOrEquals(testStartTime);
		verifyQueryResults(query, 0);

		query = managementService.createJobQuery().duedateLowerThenOrEquals(timerOneFireTime);
		verifyQueryResults(query, 1);

		query = managementService.createJobQuery().duedateLowerThenOrEquals(timerTwoFireTime);
		verifyQueryResults(query, 2);

		query = managementService.createJobQuery().duedateLowerThenOrEquals(timerThreeFireTime);
		verifyQueryResults(query, 3);

		if (ensureJobDueDateSet)
		{
		  query = managementService.createJobQuery().duedateLowerThenOrEquals(messageDueDate);
		  verifyQueryResults(query, 4);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDuedateHigherThen()
	  public virtual void testQueryByDuedateHigherThen()
	  {
		int startTimeExpectedCount = ensureJobDueDateSet? 4 : 3;
		int timerOneExpectedCount = ensureJobDueDateSet? 3 : 2;
		int timerTwoExpectedCount = ensureJobDueDateSet? 2 : 1;
		int timerThreeExpectedCount = ensureJobDueDateSet? 1 : 0;

		JobQuery query = managementService.createJobQuery().duedateHigherThen(testStartTime);
		verifyQueryResults(query, startTimeExpectedCount);

		query = managementService.createJobQuery().duedateHigherThen(timerOneFireTime);
		verifyQueryResults(query, timerOneExpectedCount);

		query = managementService.createJobQuery().duedateHigherThen(timerTwoFireTime);
		verifyQueryResults(query, timerTwoExpectedCount);

		query = managementService.createJobQuery().duedateHigherThen(timerThreeFireTime);
		verifyQueryResults(query, timerThreeExpectedCount);

		if (ensureJobDueDateSet)
		{
		  query = managementService.createJobQuery().duedateHigherThen(messageDueDate);
		  verifyQueryResults(query, 0);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDuedateHigherThenOrEqual()
	  public virtual void testQueryByDuedateHigherThenOrEqual()
	  {
		int startTimeExpectedCount = ensureJobDueDateSet? 4 : 3;
		int timerOneExpectedCount = ensureJobDueDateSet? 3 : 2;
		int timerTwoExpectedCount = ensureJobDueDateSet? 2 : 1;
		int timerThreeExpectedCount = ensureJobDueDateSet? 1 : 0;

		JobQuery query = managementService.createJobQuery().duedateHigherThenOrEquals(testStartTime);
		verifyQueryResults(query, startTimeExpectedCount);

		query = managementService.createJobQuery().duedateHigherThenOrEquals(timerOneFireTime);
		verifyQueryResults(query, startTimeExpectedCount);

		query = managementService.createJobQuery().duedateHigherThenOrEquals(new DateTime(timerOneFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, timerOneExpectedCount);

		query = managementService.createJobQuery().duedateHigherThenOrEquals(timerThreeFireTime);
		verifyQueryResults(query, timerTwoExpectedCount);

		query = managementService.createJobQuery().duedateHigherThenOrEquals(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, timerThreeExpectedCount);

		if (ensureJobDueDateSet)
		{
		  query = managementService.createJobQuery().duedateHigherThenOrEquals(new DateTime(messageDueDate.Ticks + ONE_SECOND));
		  verifyQueryResults(query, 0);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDuedateCombinations()
	  public virtual void testQueryByDuedateCombinations()
	  {
		JobQuery query = managementService.createJobQuery().duedateHigherThan(testStartTime).duedateLowerThan(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
		verifyQueryResults(query, 3);

		query = managementService.createJobQuery().duedateHigherThan(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND)).duedateLowerThan(testStartTime);
		verifyQueryResults(query, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCreateTimeCombinations()
	  public virtual void testQueryByCreateTimeCombinations()
	  {
		JobQuery query = managementService.createJobQuery().processInstanceId(processInstanceIdOne);
		IList<Job> jobs = query.list();
		assertEquals(1, jobs.Count);
		DateTime jobCreateTime = jobs[0].CreateTime;

		query = managementService.createJobQuery().processInstanceId(processInstanceIdOne).createdAfter(new DateTime(jobCreateTime.Ticks - 1));
		verifyQueryResults(query, 1);

		query = managementService.createJobQuery().processInstanceId(processInstanceIdOne).createdAfter(jobCreateTime);
		verifyQueryResults(query, 0);

		query = managementService.createJobQuery().processInstanceId(processInstanceIdOne).createdBefore(jobCreateTime);
		verifyQueryResults(query, 1);

		query = managementService.createJobQuery().processInstanceId(processInstanceIdOne).createdBefore(new DateTime(jobCreateTime.Ticks - 1));
		verifyQueryResults(query, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"}) public void testQueryByException()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testQueryByException()
	  {
		JobQuery query = managementService.createJobQuery().withException();
		verifyQueryResults(query, 0);

		ProcessInstance processInstance = startProcessInstanceWithFailingJob();

		query = managementService.createJobQuery().withException();
		verifyFailedJob(query, processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"}) public void testQueryByExceptionMessage()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testQueryByExceptionMessage()
	  {
		JobQuery query = managementService.createJobQuery().exceptionMessage(EXCEPTION_MESSAGE);
		verifyQueryResults(query, 0);

		ProcessInstance processInstance = startProcessInstanceWithFailingJob();

		Job job = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		query = managementService.createJobQuery().exceptionMessage(job.ExceptionMessage);
		verifyFailedJob(query, processInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"}) public void testQueryByExceptionMessageEmpty()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testQueryByExceptionMessageEmpty()
	  {
		JobQuery query = managementService.createJobQuery().exceptionMessage("");
		verifyQueryResults(query, 0);

		startProcessInstanceWithFailingJob();

		query = managementService.createJobQuery().exceptionMessage("");
		verifyQueryResults(query, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExceptionMessageNull()
	  public virtual void testQueryByExceptionMessageNull()
	  {
		try
		{
		  managementService.createJobQuery().exceptionMessage(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  assertEquals("Provided exception message is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobQueryWithExceptions() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testJobQueryWithExceptions()
	  {

		createJobWithoutExceptionMsg();

		Job job = managementService.createJobQuery().jobId(timerEntity.Id).singleResult();

		assertNotNull(job);

		IList<Job> list = managementService.createJobQuery().withException().list();
		assertEquals(list.Count, 1);

		deleteJobInDatabase();

		createJobWithoutExceptionStacktrace();

		job = managementService.createJobQuery().jobId(timerEntity.Id).singleResult();

		assertNotNull(job);

		list = managementService.createJobQuery().withException().list();
		assertEquals(list.Count, 1);

		deleteJobInDatabase();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNoRetriesLeft()
	  public virtual void testQueryByNoRetriesLeft()
	  {
		JobQuery query = managementService.createJobQuery().noRetriesLeft();
		verifyQueryResults(query, 0);

		setRetries(processInstanceIdOne, 0);
		// Re-running the query should give only one jobs now, since three job has retries>0
		verifyQueryResults(query, 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActive()
	  public virtual void testQueryByActive()
	  {
		JobQuery query = managementService.createJobQuery().active();
		verifyQueryResults(query, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBySuspended()
	  public virtual void testQueryBySuspended()
	  {
		JobQuery query = managementService.createJobQuery().suspended();
		verifyQueryResults(query, 0);

		managementService.suspendJobDefinitionByProcessDefinitionKey("timerOnTask", true);
		verifyQueryResults(query, 3);
	  }

	  //sorting //////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySorting()
	  public virtual void testQuerySorting()
	  {
		// asc
		assertEquals(4, managementService.createJobQuery().orderByJobId().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByJobDuedate().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByExecutionId().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessInstanceId().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByJobRetries().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessDefinitionId().asc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessDefinitionKey().asc().count());

		// desc
		assertEquals(4, managementService.createJobQuery().orderByJobId().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByJobDuedate().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByExecutionId().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessInstanceId().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByJobRetries().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessDefinitionId().desc().count());
		assertEquals(4, managementService.createJobQuery().orderByProcessDefinitionKey().desc().count());

		// sorting on multiple fields
		setRetries(processInstanceIdTwo, 2);
		ClockUtil.CurrentTime = new DateTime(timerThreeFireTime.Ticks + ONE_SECOND); // make sure all timers can fire

		JobQuery query = managementService.createJobQuery().timers().executable().orderByJobRetries().asc().orderByJobDuedate().desc();

		IList<Job> jobs = query.list();
		assertEquals(3, jobs.Count);

		assertEquals(2, jobs[0].Retries);
		assertEquals(3, jobs[1].Retries);
		assertEquals(3, jobs[2].Retries);

		assertEquals(processInstanceIdTwo, jobs[0].ProcessInstanceId);
		assertEquals(processInstanceIdThree, jobs[1].ProcessInstanceId);
		assertEquals(processInstanceIdOne, jobs[2].ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryInvalidSortingUsage()
	  public virtual void testQueryInvalidSortingUsage()
	  {
		try
		{
		  managementService.createJobQuery().orderByJobId().list();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("call asc() or desc() after using orderByXX()"));
		}

		try
		{
		  managementService.createJobQuery().asc();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("You should call any of the orderBy methods first before specifying a direction"));
		}
	  }

	  //helper ////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void setRetries(final String processInstanceId, final int retries)
	  private void setRetries(string processInstanceId, int retries)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		commandExecutor.execute(new CommandAnonymousInnerClass2(this, retries, job));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly JobQueryTest outerInstance;

		  private int retries;
		  private Job job;

		  public CommandAnonymousInnerClass2(JobQueryTest outerInstance, int retries, Job job)
		  {
			  this.outerInstance = outerInstance;
			  this.retries = retries;
			  this.job = job;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			JobEntity timer = commandContext.DbEntityManager.selectById(typeof(JobEntity), job.Id);
			timer.Retries = retries;
			return null;
		  }

	  }

	  private ProcessInstance startProcessInstanceWithFailingJob()
	  {
		// start a process with a failing job
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundary
		// timer event which we will execute manual for testing purposes.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);

		try
		{
		  managementService.executeJob(timerJob.Id);
		  fail("RuntimeException from within the script task expected");
		}
		catch (Exception re)
		{
		  assertThat(re.Message, containsString(EXCEPTION_MESSAGE));
		}
		return processInstance;
	  }

	  private void verifyFailedJob(JobQuery query, ProcessInstance processInstance)
	  {
		verifyQueryResults(query, 1);

		Job failedJob = query.singleResult();
		assertNotNull(failedJob);
		assertEquals(processInstance.Id, failedJob.ProcessInstanceId);
		assertNotNull(failedJob.ExceptionMessage);
		assertThat(failedJob.ExceptionMessage, containsString(EXCEPTION_MESSAGE));
	  }

	  private void verifyQueryResults(JobQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  private void verifySingleResultFails(JobQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  private void createJobWithoutExceptionMsg()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass3(this));

	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly JobQueryTest outerInstance;

		  public CommandAnonymousInnerClass3(JobQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobManager jobManager = commandContext.JobManager;

			outerInstance.timerEntity = new TimerEntity();
			outerInstance.timerEntity.LockOwner = System.Guid.randomUUID().ToString();
			outerInstance.timerEntity.Duedate = DateTime.Now;
			outerInstance.timerEntity.Retries = 0;

			StringWriter stringWriter = new StringWriter();
			System.NullReferenceException exception = new System.NullReferenceException();
			exception.printStackTrace(new PrintWriter(stringWriter));
			outerInstance.timerEntity.ExceptionStacktrace = stringWriter.ToString();

			jobManager.insert(outerInstance.timerEntity);

			assertNotNull(outerInstance.timerEntity.Id);

			return null;

		  }
	  }

	  private void createJobWithoutExceptionStacktrace()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass4(this));

	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly JobQueryTest outerInstance;

		  public CommandAnonymousInnerClass4(JobQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobManager jobManager = commandContext.JobManager;

			outerInstance.timerEntity = new TimerEntity();
			outerInstance.timerEntity.LockOwner = System.Guid.randomUUID().ToString();
			outerInstance.timerEntity.Duedate = DateTime.Now;
			outerInstance.timerEntity.Retries = 0;
			outerInstance.timerEntity.ExceptionMessage = "I'm supposed to fail";

			jobManager.insert(outerInstance.timerEntity);

			assertNotNull(outerInstance.timerEntity.Id);

			return null;

		  }
	  }

	  private void deleteJobInDatabase()
	  {
		  CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		  commandExecutor.execute(new CommandAnonymousInnerClass5(this));
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly JobQueryTest outerInstance;

		  public CommandAnonymousInnerClass5(JobQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			outerInstance.timerEntity.delete();

			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(outerInstance.timerEntity.Id);

			IList<HistoricIncident> historicIncidents = Context.ProcessEngineConfiguration.HistoryService.createHistoricIncidentQuery().list();

			foreach (HistoricIncident historicIncident in historicIncidents)
			{
			  commandContext.DbEntityManager.delete((DbEntity) historicIncident);
			}

			return null;
		  }
	  }

	}

}