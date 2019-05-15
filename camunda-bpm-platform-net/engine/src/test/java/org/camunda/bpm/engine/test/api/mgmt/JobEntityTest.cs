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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class JobEntityTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobEntityTest()
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
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal IList<string> jobIds = new List<string>();

	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal RuntimeService runtimeService;

	  protected internal static readonly DateTime CREATE_DATE = new DateTime(1363607000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		runtimeService = engineRule.RuntimeService;
		jobIds = new List<>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockUtil.CurrentTime = CREATE_DATE;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanup()
	  public virtual void cleanup()
	  {
		foreach (string jobId in jobIds)
		{
		  managementService.deleteJob(jobId);
		}

		if (!testRule.HistoryLevelNone)
		{
		  cleanupJobLog();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckCreateTimeOnMessage()
	  public virtual void shouldCheckCreateTimeOnMessage()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().camundaAsyncBefore().endEvent().done());

		runtimeService.startProcessInstanceByKey("process");

		// when
		Job messageJob = managementService.createJobQuery().singleResult();

		// then
		assertThat(messageJob.CreateTime, @is(CREATE_DATE));
		assertThat(messageJob.GetType().Name, @is("MessageEntity"));

		// cleanup
		jobIds.Add(messageJob.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckCreateTimeOnTimer()
	  public virtual void shouldCheckCreateTimeOnTimer()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("process").startEvent().timerWithDuration("PT5S").endEvent().done());

		runtimeService.startProcessInstanceByKey("process");

		// when
		Job timerJob = managementService.createJobQuery().singleResult();

		// then
		assertThat(timerJob.CreateTime, @is(CREATE_DATE));
		assertThat(timerJob.GetType().Name, @is("TimerEntity"));

		// cleanup
		jobIds.Add(timerJob.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckCreateTimeOnEverLivingJob()
	  public virtual void shouldCheckCreateTimeOnEverLivingJob()
	  {
		// given
		historyService.cleanUpHistoryAsync(true);

		// when
		Job everLivingJob = managementService.createJobQuery().singleResult();

		// then
		assertThat(everLivingJob.CreateTime, @is(CREATE_DATE));
		assertThat(everLivingJob.GetType().Name, @is("EverLivingJobEntity"));

		// cleanup
		jobIds.Add(everLivingJob.Id);
	  }

	  // helper ////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void cleanupJobLog()
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly JobEntityTest outerInstance;

		  public CommandAnonymousInnerClass(JobEntityTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			foreach (string jobId in outerInstance.jobIds)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}

			return null;
		  }
	  }

	}

}