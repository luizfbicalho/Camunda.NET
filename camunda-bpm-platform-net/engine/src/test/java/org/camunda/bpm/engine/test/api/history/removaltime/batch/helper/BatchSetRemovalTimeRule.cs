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
namespace org.camunda.bpm.engine.test.api.history.removaltime.batch.helper
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultHistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.DefaultHistoryRemovalTimeProvider;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using GetByteArrayCommand = org.camunda.bpm.engine.test.api.resources.GetByteArrayCommand;
	using FailingExecutionListener = org.camunda.bpm.engine.test.bpmn.async.FailingExecutionListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CallActivityBuilder = org.camunda.bpm.model.bpmn.builder.CallActivityBuilder;
	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;
	using StartEventBuilder = org.camunda.bpm.model.bpmn.builder.StartEventBuilder;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class BatchSetRemovalTimeRule : TestWatcher
	{

	  protected internal ProcessEngineRule engineRule;
	  protected internal ProcessEngineTestRule engineTestRule;

	  public readonly DateTime CURRENT_DATE = new DateTime(1363608000000L);
	  public readonly DateTime REMOVAL_TIME = new DateTime(1363609000000L);

	  protected internal IList<string> batchIds = new List<string>();

	  public BatchSetRemovalTimeRule(ProcessEngineRule engineRule, ProcessEngineTestRule engineTestRule)
	  {
		this.engineRule = engineRule;
		this.engineTestRule = engineTestRule;
	  }

	  protected internal virtual void starting(Description description)
	  {
		ProcessEngineConfiguration.setHistoryRemovalTimeProvider(new DefaultHistoryRemovalTimeProvider()).setHistoryRemovalTimeStrategy(ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		ClockUtil.CurrentTime = CURRENT_DATE;

		base.starting(description);
	  }

	  protected internal virtual void finished(Description description)
	  {
		ProcessEngineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(null).initHistoryRemovalTime();

		ProcessEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		ProcessEngineConfiguration.BatchOperationsForHistoryCleanup = null;

		ProcessEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		ProcessEngineConfiguration.HistoryCleanupStrategy = null;
		ProcessEngineConfiguration.initHistoryCleanup();

		ProcessEngineConfiguration.InvocationsPerBatchJob = 1;

		ProcessEngineConfiguration.DmnEnabled = true;

		ClockUtil.reset();

		clearDatabase();

		base.finished(description);
	  }

	  public virtual void clearDatabase()
	  {
		if (batchIds.Count > 0)
		{
		  foreach (string batchId in batchIds)
		  {
			HistoricBatch historicBatch = engineRule.HistoryService.createHistoricBatchQuery().batchId(batchId).singleResult();

			if (historicBatch != null)
			{
			  engineRule.HistoryService.deleteHistoricBatch(historicBatch.Id);
			}
		  }
		}
	  }

	  // helper ////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  public virtual TestProcessBuilder process()
	  {
		return new TestProcessBuilder(this);
	  }

	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return engineRule.ProcessEngineConfiguration;
		  }
	  }

	  public virtual void updateHistoryTimeToLive(string key, int ttl)
	  {
		updateHistoryTimeToLive(ttl, key);
	  }

	  public virtual void updateHistoryTimeToLive(int ttl, params string[] keys)
	  {
		foreach (string key in keys)
		{
		  string processDefinitionId = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(key).singleResult().Id;

		  engineRule.RepositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinitionId, ttl);
		}
	  }

	  public virtual void updateHistoryTimeToLiveDmn(string key, int ttl)
	  {
		updateHistoryTimeToLiveDmn(ttl, key);
	  }

	  public virtual void updateHistoryTimeToLiveDmn(int ttl, params string[] keys)
	  {
		foreach (string key in keys)
		{
		  string decisionDefinitionId = engineRule.RepositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(key).singleResult().Id;

		  engineRule.RepositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinitionId, ttl);
		}
	  }

	  public class TestProcessBuilder
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  startEventBuilder = builder.startEvent();
		  }

		  private readonly BatchSetRemovalTimeRule outerInstance;

		  public TestProcessBuilder(BatchSetRemovalTimeRule outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }


		protected internal const string PROCESS_KEY = "process";
		protected internal const string ROOT_PROCESS_KEY = "rootProcess";

		internal ProcessBuilder builder = Bpmn.createExecutableProcess(PROCESS_KEY);
		internal StartEventBuilder startEventBuilder;
		internal ProcessBuilder rootProcessBuilder = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		internal int? ttl_Renamed;
		internal CallActivityBuilder callActivityBuilder;

		public virtual TestProcessBuilder ttl(int? ttl)
		{
		  this.ttl_Renamed = ttl;
		  return this;
		}

		public virtual TestProcessBuilder async()
		{
		  startEventBuilder.camundaAsyncBefore();

		  return this;
		}

		public virtual TestProcessBuilder ruleTask(string @ref)
		{
		  startEventBuilder.businessRuleTask().camundaDecisionRef(@ref);

		  return this;
		}

		public virtual TestProcessBuilder call()
		{
		  rootProcessBuilder = Bpmn.createExecutableProcess(ROOT_PROCESS_KEY);

		  callActivityBuilder = rootProcessBuilder.startEvent().callActivity().calledElement(PROCESS_KEY);

		  return this;
		}

		public virtual TestProcessBuilder passVars(params string[] vars)
		{
		  foreach (string variable in vars)
		  {
			callActivityBuilder.camundaIn(variable, variable);
		  }

		  callActivityBuilder.endEvent();

		  return this;
		}

		public virtual TestProcessBuilder userTask()
		{
		  startEventBuilder.userTask("userTask").name("userTask").camundaAssignee("anAssignee");

		  return this;
		}

		public virtual TestProcessBuilder scriptTask()
		{
		  startEventBuilder.scriptTask().scriptFormat("groovy").scriptText("throw new RuntimeException()");

		  return this;
		}

		public virtual TestProcessBuilder externalTask()
		{
		  startEventBuilder.serviceTask().camundaExternalTask("aTopicName");

		  return this;
		}

		public virtual TestProcessBuilder serviceTask()
		{
		  startEventBuilder.serviceTask().camundaExpression("${true}");

		  return this;
		}

		public virtual TestProcessBuilder failingCustomListener()
		{
		  startEventBuilder.userTask().camundaExecutionListenerClass("end", typeof(FailingExecutionListener));

		  return this;
		}

		public virtual TestProcessBuilder deploy()
		{
		  if (ttl_Renamed != null)
		  {
			if (rootProcessBuilder != null)
			{
			  rootProcessBuilder.camundaHistoryTimeToLive(ttl_Renamed);
			}
			else
			{
			  builder.camundaHistoryTimeToLive(ttl_Renamed);
			}
		  }

		  BpmnModelInstance process = startEventBuilder.endEvent().done();

		  outerInstance.engineTestRule.deploy(process);

		  if (rootProcessBuilder != null)
		  {
			outerInstance.engineTestRule.deploy(rootProcessBuilder.done());
		  }

		  return this;
		}

		public virtual string start()
		{
		  return startWithVariables(null);
		}

		public virtual string startWithVariables(IDictionary<string, object> variables)
		{
		  string key = null;

		  if (rootProcessBuilder != null)
		  {
			key = ROOT_PROCESS_KEY;
		  }
		  else
		  {
			key = PROCESS_KEY;
		  }

		  return outerInstance.engineRule.RuntimeService.startProcessInstanceByKey(key, variables).Id;
		}
	  }

	  public virtual void syncExec(Batch batch)
	  {
		syncExec(batch, true);
	  }

	  public virtual void syncExec(Batch batch, bool isClear)
	  {
		if (isClear)
		{
		  batchIds.Add(batch.Id);
		}

		string seedJobDefinitionId = batch.SeedJobDefinitionId;

		string jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);

		string batchJobDefinitionId = batch.BatchJobDefinitionId;

		IList<Job> jobs = engineRule.ManagementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).list();

		foreach (Job job in jobs)
		{
		  engineRule.ManagementService.executeJob(job.Id);
		}

		string monitorJobDefinitionId = batch.MonitorJobDefinitionId;

		jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(monitorJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);
	  }

	  public static DateTime addDays(DateTime date, int amount)
	  {
		DateTime c = new DateTime();
		c = new DateTime(date);
		c.AddDays(amount);
		return c;
	  }

	  public virtual ByteArrayEntity findByteArrayById(string byteArrayId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		return commandExecutor.execute(new GetByteArrayCommand(byteArrayId));
	  }

	}

}