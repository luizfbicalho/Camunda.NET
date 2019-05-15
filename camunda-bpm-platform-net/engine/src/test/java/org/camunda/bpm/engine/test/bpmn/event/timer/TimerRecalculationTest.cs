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
namespace org.camunda.bpm.engine.test.bpmn.@event.timer
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using HistoryCleanupJobHandler = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandler;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using After = org.junit.After;


	/// <summary>
	/// Test timer recalculation
	/// 
	/// @author Tobias Metzke
	/// </summary>

	public class TimerRecalculationTest : PluggableProcessEngineTestCase
	{

	  private ISet<string> jobIds = new HashSet<string>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		clearMeterLog();

		foreach (string jobId in jobIds)
		{
		  clearJobLog(jobId);
		  clearJob(jobId);
		}

		jobIds = new HashSet<>();
	  }

	  public virtual void testUnknownId()
	  {
		try
		{
		  // when
		  managementService.recalculateJobDuedate("unknownID", false);
		  fail("The recalculation with an unknown job ID should not be possible");
		}
		catch (ProcessEngineException pe)
		{
		  // then
		  assertTextPresent("No job found with id '" + "unknownID", pe.Message);
		}
	  }

	  public virtual void testEmptyId()
	  {
		try
		{
		  // when
		  managementService.recalculateJobDuedate("", false);
		  fail("The recalculation with an unknown job ID should not be possible");
		}
		catch (ProcessEngineException pe)
		{
		  // then
		  assertTextPresent("The job id is mandatory: jobId is empty", pe.Message);
		}
	  }

	  public virtual void testNullId()
	  {
		try
		{
		  // when
		  managementService.recalculateJobDuedate(null, false);
		  fail("The recalculation with an unknown job ID should not be possible");
		}
		catch (ProcessEngineException pe)
		{
		  // then
		  assertTextPresent("The job id is mandatory: jobId is null", pe.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFinishedJob()
	  public virtual void testFinishedJob()
	  {
		// given
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		variables1["dueDate"] = DateTime.Now;

		ProcessInstance pi1 = runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables1);
		assertEquals(1, managementService.createJobQuery().processInstanceId(pi1.Id).count());

		JobQuery jobQuery = managementService.createJobQuery().executable();
		assertEquals(1L, jobQuery.count());

		// job duedate can be recalculated, job still exists in runtime
		string jobId = jobQuery.singleResult().Id;
		managementService.recalculateJobDuedate(jobId, false);
		// run the job, finish the process
		managementService.executeJob(jobId);
		assertEquals(0L, managementService.createJobQuery().processInstanceId(pi1.Id).count());
		assertProcessEnded(pi1.ProcessInstanceId);

		try
		{
		  // when
		  managementService.recalculateJobDuedate(jobId, false);
		  fail("The recalculation of a finished job should not be possible");
		}
		catch (ProcessEngineException pe)
		{
		  // then
		  assertTextPresent("No job found with id '" + jobId, pe.Message);
		}
	  }

	  public virtual void testEverLivingJob()
	  {
		// given
		Job job = historyService.cleanUpHistoryAsync(true);
		jobIds.Add(job.Id);

		// when & then
		tryRecalculateUnsupported(job, HistoryCleanupJobHandler.TYPE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageJob()
	  public virtual void testMessageJob()
	  {
		// given
		runtimeService.startProcessInstanceByKey("asyncService");
		Job job = managementService.createJobQuery().singleResult();
		jobIds.Add(job.Id);

		// when & then
		tryRecalculateUnsupported(job, AsyncContinuationJobHandler.TYPE);
	  }


	  // helper /////////////////////////////////////////////////////////////////

	  protected internal virtual void tryRecalculateUnsupported(Job job, string type)
	  {
		try
		{
		  // when
		  managementService.recalculateJobDuedate(job.Id, false);
		  fail("The recalculation with an unsupported type should not be possible");
		}
		catch (ProcessEngineException pe)
		{
		  // then
		  assertTextPresent("Only timer jobs can be recalculated, but the job with id '" + job.Id + "' is of type '" + type, pe.Message);
		}
	  }


	  protected internal virtual void clearMeterLog()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly TimerRecalculationTest outerInstance;

		  public CommandAnonymousInnerClass(TimerRecalculationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJobLog(final String jobId)
	  protected internal virtual void clearJobLog(string jobId)
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass2(this, jobId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly TimerRecalculationTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass2(TimerRecalculationTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJob(final String jobId)
	  protected internal virtual void clearJob(string jobId)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this, jobId));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private readonly TimerRecalculationTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass3(TimerRecalculationTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			JobEntity job = commandContext.JobManager.findJobById(jobId);
			if (job != null)
			{
			  commandContext.JobManager.delete(job);
			}
			return null;
		  }
	  }
	}

}