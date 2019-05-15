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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class AcquireJobsCmdTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/jobexecutor/oneJobProcess.bpmn20.xml"})]
	  public virtual void testJobsNotVisisbleToAcquisitionIfInstanceSuspended()
	  {

		ProcessDefinition pd = repositoryService.createProcessDefinitionQuery().singleResult();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey(pd.Key);

		// now there is one job:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		makeSureJobDue(job);

		// the acquirejobs command sees the job:
		AcquiredJobs acquiredJobs = executeAcquireJobsCommand();
		assertEquals(1, acquiredJobs.size());

		// suspend the process instance:
		runtimeService.suspendProcessInstanceById(pi.Id);

		// now, the acquirejobs command does not see the job:
		acquiredJobs = executeAcquireJobsCommand();
		assertEquals(0, acquiredJobs.size());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/standalone/jobexecutor/oneJobProcess.bpmn20.xml"})]
	  public virtual void testJobsNotVisisbleToAcquisitionIfDefinitionSuspended()
	  {

		ProcessDefinition pd = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceByKey(pd.Key);
		// now there is one job:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		makeSureJobDue(job);

		// the acquirejobs command sees the job:
		AcquiredJobs acquiredJobs = executeAcquireJobsCommand();
		assertEquals(1, acquiredJobs.size());

		// suspend the process instance:
		repositoryService.suspendProcessDefinitionById(pd.Id);

		// now, the acquirejobs command does not see the job:
		acquiredJobs = executeAcquireJobsCommand();
		assertEquals(0, acquiredJobs.size());
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void makeSureJobDue(final org.camunda.bpm.engine.runtime.Job job)
	  protected internal virtual void makeSureJobDue(Job job)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, job));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly AcquireJobsCmdTest outerInstance;

		  private Job job;

		  public CommandAnonymousInnerClass(AcquireJobsCmdTest outerInstance, Job job)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			DateTime currentTime = ClockUtil.CurrentTime;
			commandContext.JobManager.findJobById(job.Id).Duedate = new DateTime(currentTime.Ticks - 10000);
			return null;
		  }

	  }

	  private AcquiredJobs executeAcquireJobsCommand()
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new AcquireJobsCmd(processEngineConfiguration.JobExecutor));
	  }

	}

}