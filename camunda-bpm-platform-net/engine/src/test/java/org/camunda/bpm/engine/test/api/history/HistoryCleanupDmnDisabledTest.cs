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
	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Meter = org.camunda.bpm.engine.impl.metrics.Meter;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoryCleanupDmnDisabledTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupDmnDisabledTest()
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


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.DmnEnabled = false;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  private RuntimeService runtimeService;
	  private HistoryService historyService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;
	  private ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createProcessEngine()
	  public virtual void createProcessEngine()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

		clearMetrics();

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly HistoryCleanupDmnDisabledTest outerInstance;

		  public CommandAnonymousInnerClass(HistoryCleanupDmnDisabledTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.engineRule.ManagementService.createJobQuery().list();
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

	  protected internal virtual void clearMetrics()
	  {
		ICollection<Meter> meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
		foreach (Meter meter in meters)
		{
		  meter.AndClear;
		}
		managementService.deleteMetrics(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void historyCleanupWithDisabledDmn()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void historyCleanupWithDisabledDmn()
	  {

		prepareHistoricProcesses("oneTaskProcess");

		ClockUtil.CurrentTime = DateTime.Now;
		//when
		string jobId = historyService.cleanUpHistoryAsync(true).Id;

		engineRule.ManagementService.executeJob(jobId);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("oneTaskProcess").count());
	  }

	  private void prepareHistoricProcesses(string businessKey)
	  {
		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, -6);

		IList<string> processInstanceIds = new List<string>();

		for (int i = 0; i < 5; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(businessKey);
		  processInstanceIds.Add(processInstance.Id);
		}
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);

		ClockUtil.CurrentTime = oldCurrentTime;

	  }

	}

}