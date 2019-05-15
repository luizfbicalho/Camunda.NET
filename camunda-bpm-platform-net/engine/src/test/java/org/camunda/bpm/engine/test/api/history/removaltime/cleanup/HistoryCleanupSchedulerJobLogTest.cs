﻿using System;

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
namespace org.camunda.bpm.engine.test.api.history.removaltime.cleanup
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addDays;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addSeconds;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandlerConfiguration.START_DELAY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class HistoryCleanupSchedulerJobLogTest : AbstractHistoryCleanupSchedulerTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupSchedulerJobLogTest()
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
			PROCESS = Bpmn.createExecutableProcess(PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().scriptTask().camundaAsyncBefore().scriptFormat("groovy").scriptText("if(execution.getIncidents().size() == 0) throw new RuntimeException(\"I'm supposed to fail!\")").userTask("userTask").endEvent().done();
		}


	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return outerInstance.configure(configuration, HistoryEventTypes.JOB_FAIL);
		  }
	  }

	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		engineConfiguration = engineRule.ProcessEngineConfiguration;
		initEngineConfiguration(engineConfiguration);

		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;

		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
	  }

	  protected internal readonly string PROCESS_KEY = "process";
	  protected internal BpmnModelInstance PROCESS;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldScheduleToNow()
	  public virtual void shouldScheduleToNow()
	  {
		// given
		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 5);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  try
		  {
			managementService.executeJob(jobId);
		  }
		  catch (Exception)
		  {
		  }
		}

		managementService.executeJob(jobId);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		engineConfiguration.HistoryCleanupBatchSize = 5;
		engineConfiguration.initHistoryCleanup();

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		Job job = historyService.findHistoryCleanupJobs()[0];

		// then
		assertThat(job.Duedate, @is(removalTime));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldScheduleToLater()
	  public virtual void shouldScheduleToLater()
	  {
		// given
		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 5);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  try
		  {
			managementService.executeJob(jobId);
		  }
		  catch (Exception)
		  {
		  }
		}

		managementService.executeJob(jobId);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		engineConfiguration.HistoryCleanupBatchSize = 6;
		engineConfiguration.initHistoryCleanup();

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		Job job = historyService.findHistoryCleanupJobs()[0];

		// then
		assertThat(job.Duedate, @is(addSeconds(removalTime, START_DELAY)));
	  }

	}

}