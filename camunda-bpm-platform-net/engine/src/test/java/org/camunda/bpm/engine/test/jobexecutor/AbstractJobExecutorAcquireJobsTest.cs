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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using Page = org.camunda.bpm.engine.impl.Page;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;

	public abstract class AbstractJobExecutorAcquireJobsTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

	  protected internal ManagementService managementService;
	  protected internal RuntimeService runtimeService;

	  protected internal ProcessEngineConfigurationImpl configuration;

	  private bool jobExecutorAcquireByDueDate;
	  private bool jobExecutorAcquireByPriority;
	  private bool jobExecutorPreferTimerJobs;
	  private bool jobEnsureDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		managementService = rule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void saveProcessEngineConfiguration()
	  public virtual void saveProcessEngineConfiguration()
	  {
		configuration = (ProcessEngineConfigurationImpl) rule.ProcessEngine.ProcessEngineConfiguration;
		jobExecutorAcquireByDueDate = configuration.JobExecutorAcquireByDueDate;
		jobExecutorAcquireByPriority = configuration.JobExecutorAcquireByPriority;
		jobExecutorPreferTimerJobs = configuration.JobExecutorPreferTimerJobs;
		jobEnsureDueDateSet = configuration.EnsureJobDueDateNotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockTestUtil.setClockToDateWithoutMilliseconds();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreProcessEngineConfiguration()
	  public virtual void restoreProcessEngineConfiguration()
	  {
		configuration.JobExecutorAcquireByDueDate = jobExecutorAcquireByDueDate;
		configuration.JobExecutorAcquireByPriority = jobExecutorAcquireByPriority;
		configuration.JobExecutorPreferTimerJobs = jobExecutorPreferTimerJobs;
		configuration.EnsureJobDueDateNotNull = jobEnsureDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

	  protected internal virtual IList<JobEntity> findAcquirableJobs()
	  {
		return configuration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<IList<JobEntity>>
	  {
		  private readonly AbstractJobExecutorAcquireJobsTest outerInstance;

		  public CommandAnonymousInnerClass(AbstractJobExecutorAcquireJobsTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public IList<JobEntity> execute(CommandContext commandContext)
		  {
			return commandContext.JobManager.findNextJobsToExecute(new Page(0, 100));
		  }
	  }

	  protected internal virtual string startProcess(string processDefinitionKey, string activity)
	  {
		return runtimeService.createProcessInstanceByKey(processDefinitionKey).startBeforeActivity(activity).execute().Id;
	  }

	  protected internal virtual void startProcess(string processDefinitionKey, string activity, int times)
	  {
		for (int i = 0; i < times; i++)
		{
		  startProcess(processDefinitionKey, activity);
		}
	  }

	}

}