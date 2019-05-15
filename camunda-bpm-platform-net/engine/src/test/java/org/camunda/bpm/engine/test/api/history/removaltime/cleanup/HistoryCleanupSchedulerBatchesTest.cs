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
	public class HistoryCleanupSchedulerBatchesTest : AbstractHistoryCleanupSchedulerTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupSchedulerBatchesTest()
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
			PROCESS = Bpmn.createExecutableProcess(PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().userTask().endEvent().done();
			CALLING_PROCESS = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).startEvent().callActivity().calledElement(PROCESS_KEY).multiInstance().cardinality("5").multiInstanceDone().endEvent().done();
		}


	  public ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return outerInstance.configure(configuration, HistoryEventTypes.BATCH_START, HistoryEventTypes.BATCH_END);
		  }
	  }

	  public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

	  protected internal readonly string PROCESS_KEY = "process";
	  protected internal BpmnModelInstance PROCESS;

	  protected internal readonly string CALLING_PROCESS_KEY = "callingProcess";
	  protected internal BpmnModelInstance CALLING_PROCESS;

	  protected internal readonly new DateTime END_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldScheduleToNow()
	  public virtual void shouldScheduleToNow()
	  {
		// given
		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.HistoryCleanupBatchSize = 5;
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		testRule.deploy(CALLING_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY).Id;

		for (int i = 0; i < 5; i++)
		{
		  string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list().get(0).Id;
		  runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		  ClockUtil.CurrentTime = END_DATE;

		  string jobId = managementService.createJobQuery().singleResult().Id;
		  managementService.executeJob(jobId);
		  jobIds.Add(jobId);

		  IList<Job> jobs = managementService.createJobQuery().list();
		  foreach (Job job in jobs)
		  {
			managementService.executeJob(job.Id);
			jobIds.Add(job.Id);
		  }
		}

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
		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.HistoryCleanupBatchSize = 6;
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		testRule.deploy(CALLING_PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY).Id;

		for (int i = 0; i < 5; i++)
		{
			string processInstanceId = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list().get(0).Id;
		  runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		  ClockUtil.CurrentTime = END_DATE;

		  string jobId = managementService.createJobQuery().singleResult().Id;
		  managementService.executeJob(jobId);
		  jobIds.Add(jobId);

		  IList<Job> jobs = managementService.createJobQuery().list();
		  foreach (Job job in jobs)
		  {
			managementService.executeJob(job.Id);
			jobIds.Add(job.Id);
		  }
		}

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