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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultHistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.DefaultHistoryRemovalTimeProvider;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using GetByteArrayCommand = org.camunda.bpm.engine.test.api.resources.GetByteArrayCommand;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using AfterClass = org.junit.AfterClass;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addDays;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.commons.lang3.time.DateUtils.addMinutes;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHandler.MAX_BATCH_SIZE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.nullValue;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(HISTORY_FULL)]
	public class HistoryCleanupRemovalTimeTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupRemovalTimeTest()
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
			PROCESS = Bpmn.createExecutableProcess(PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().userTask("userTask").name("userTask").endEvent().done();
			CALLED_PROCESS_INCIDENT = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().scriptTask().camundaAsyncBefore().scriptFormat("groovy").scriptText("if(execution.getIncidents().size() == 0) throw new RuntimeException(\"I'm supposed to fail!\")").userTask("userTask").endEvent().done();
			CALLING_PROCESS = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement(PROCESS_KEY).endEvent().done();
			CALLING_PROCESS_WO_TTL = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).startEvent().callActivity().calledElement(PROCESS_KEY).endEvent().done();
			CALLING_PROCESS_CALLS_DMN = Bpmn.createExecutableProcess(CALLING_PROCESS_CALLS_DMN_KEY).camundaHistoryTimeToLive(5).startEvent().businessRuleTask().camundaAsyncAfter().camundaDecisionRef("dish-decision").endEvent().done();
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

	  protected internal static ProcessEngineConfigurationImpl engineConfiguration;

	  protected internal ISet<string> jobIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		formService = engineRule.FormService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		managementService = engineRule.ManagementService;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		externalTaskService = engineRule.ExternalTaskService;
		decisionService = engineRule.DecisionService;

		engineConfiguration = engineRule.ProcessEngineConfiguration;

		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).setHistoryRemovalTimeProvider(new DefaultHistoryRemovalTimeProvider()).initHistoryRemovalTime();

		engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		engineConfiguration.HistoryCleanupBatchSize = MAX_BATCH_SIZE;
		engineConfiguration.HistoryCleanupBatchWindowStartTime = null;
		engineConfiguration.HistoryCleanupDegreeOfParallelism = 1;

		engineConfiguration.BatchOperationHistoryTimeToLive = null;
		engineConfiguration.BatchOperationsForHistoryCleanup = null;

		engineConfiguration.HistoryTimeToLive = null;

		engineConfiguration.initHistoryCleanup();

		jobIds = new HashSet<>();
	  }

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
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDownAfterAll()
	  public static void tearDownAfterAll()
	  {
		if (engineConfiguration != null)
		{
		  engineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(null).initHistoryRemovalTime();

		  engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		  engineConfiguration.HistoryCleanupBatchSize = MAX_BATCH_SIZE;
		  engineConfiguration.HistoryCleanupBatchWindowStartTime = null;
		  engineConfiguration.HistoryCleanupDegreeOfParallelism = 1;

		  engineConfiguration.BatchOperationHistoryTimeToLive = null;
		  engineConfiguration.BatchOperationsForHistoryCleanup = null;

		  engineConfiguration.initHistoryCleanup();
		}

		ClockUtil.reset();
	  }

	  protected internal readonly string PROCESS_KEY = "process";
	  protected internal BpmnModelInstance PROCESS;


	  protected internal BpmnModelInstance CALLED_PROCESS_INCIDENT;

	  protected internal readonly string CALLING_PROCESS_KEY = "callingProcess";
	  protected internal BpmnModelInstance CALLING_PROCESS;

	  protected internal BpmnModelInstance CALLING_PROCESS_WO_TTL;

	  protected internal readonly string CALLING_PROCESS_CALLS_DMN_KEY = "callingProcessCallsDmn";
	  protected internal BpmnModelInstance CALLING_PROCESS_CALLS_DMN;

	  protected internal readonly DateTime END_DATE = new DateTime(1363608000000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldCleanupDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldCleanupDecisionInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldCleanupStandaloneDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldCleanupStandaloneDecisionInstance()
	  {
		// given
		ClockUtil.CurrentTime = END_DATE;

		DecisionDefinition decisionDefinition = repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey("dish-decision").singleResult();
		repositoryService.updateDecisionDefinitionHistoryTimeToLive(decisionDefinition.Id, 5);


		// when
		decisionService.evaluateDecisionTableByKey("dish-decision", Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().includeOutputs().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		// when
		runHistoryCleanup();

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().includeOutputs().list();

		// then
		assertThat(historicDecisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldReportMetricsForDecisionInstanceCleanup()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldReportMetricsForDecisionInstanceCleanup()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		long removedDecisionInstancesSum = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES).sum();

		// then
		assertThat(removedDecisionInstancesSum, @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldCleanupDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldCleanupDecisionInputInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().list();

		// then
		assertThat(historicDecisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldCleanupDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldCleanupDecisionOutputInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeOutputs().list();

		// assume
		assertThat(historicDecisionInstances.Count, @is(3));

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeOutputs().list();

		// then
		assertThat(historicDecisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupProcessInstance()
	  public virtual void shouldCleanupProcessInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// assume
		assertThat(historicProcessInstances.Count, @is(1));

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(historicProcessInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotCleanupProcessInstanceWithoutTTL()
	  public virtual void shouldNotCleanupProcessInstanceWithoutTTL()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_WO_TTL);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// assume
		assertThat(historicProcessInstances.Count, @is(1));

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(historicProcessInstances.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupProcessInstanceWithoutTTLWithConfigDefault()
	  public virtual void shouldCleanupProcessInstanceWithoutTTLWithConfigDefault()
	  {
		// given
		engineConfiguration.HistoryTimeToLive = "5";

		testRule.deploy(CALLING_PROCESS_WO_TTL);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// assume
		assertThat(historicProcessInstances.Count, @is(1));

		DateTime removalTime = addDays(END_DATE, 5);
		ClockUtil.CurrentTime = removalTime;

		// when
		runHistoryCleanup();

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(historicProcessInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReportMetricsForProcessInstanceCleanup()
	  public virtual void shouldReportMetricsForProcessInstanceCleanup()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		long removedProcessInstancesSum = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES).sum();

		// then
		assertThat(removedProcessInstancesSum, @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupActivityInstance()
	  public virtual void shouldCleanupActivityInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// assume
		assertThat(historicActivityInstances.Count, @is(6));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicActivityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// then
		assertThat(historicActivityInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupTaskInstance()
	  public virtual void shouldCleanupTaskInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		taskService.complete(taskId);

		IList<HistoricTaskInstance> historicTaskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// assume
		assertThat(historicTaskInstances.Count, @is(1));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicTaskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// then
		assertThat(historicTaskInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupVariableInstance()
	  public virtual void shouldCleanupVariableInstance()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		taskService.complete(taskId);

		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// assume
		assertThat(historicVariableInstances.Count, @is(1));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// then
		assertThat(historicVariableInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupDetail()
	  public virtual void shouldCleanupDetail()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY, Variables.createVariables().putValue("aVariableName", Variables.stringValue("aVariableValue")));

		runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().variableUpdates().list();

		// assume
		assertThat(historicDetails.Count, @is(2));

		ClockUtil.CurrentTime = END_DATE;

		string taskId = historyService.createHistoricTaskInstanceQuery().singleResult().Id;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicDetails = historyService.createHistoricDetailQuery().variableUpdates().list();

		// then
		assertThat(historicDetails.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupIncident()
	  public virtual void shouldCleanupIncident()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS_INCIDENT);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		// assume
		assertThat(historicIncidents.Count, @is(2));

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicIncidents = historyService.createHistoricIncidentQuery().list();

		// then
		assertThat(historicIncidents.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupExternalTaskLog()
	  public virtual void shouldCleanupExternalTaskLog()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		runtimeService.startProcessInstanceByKey("callingProcess");

		LockedExternalTask externalTask = externalTaskService.fetchAndLock(1, "aWorkerId").topic("anExternalTaskTopic", 3000).execute()[0];

		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// assume
		assertThat(externalTaskLogs.Count, @is(1));

		ClockUtil.CurrentTime = END_DATE;

		externalTaskService.complete(externalTask.Id, "aWorkerId");

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupJobLog()
	  public virtual void shouldCleanupJobLog()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		ClockUtil.CurrentTime = END_DATE;

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.executeJob(jobId);

		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// assume
		assertThat(jobLogs.Count, @is(2));

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(jobLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupUserOperationLog()
	  public virtual void shouldCleanupUserOperationLog()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		identityService.AuthenticatedUserId = "aUserId";
		managementService.setJobRetries(jobId, 65);
		identityService.clearAuthentication();

		IList<UserOperationLogEntry> userOperationLogs = historyService.createUserOperationLogQuery().list();

		// assume
		assertThat(userOperationLogs.Count, @is(1));

		managementService.executeJob(jobId);

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		userOperationLogs = historyService.createUserOperationLogQuery().list();

		// then
		assertThat(userOperationLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupIdentityLink()
	  public virtual void shouldCleanupIdentityLink()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addCandidateUser(taskId, "aUserId");

		IList<HistoricIdentityLinkLog> historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// assume
		assertThat(historicIdentityLinkLogs.Count, @is(1));

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// then
		assertThat(historicIdentityLinkLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupComment()
	  public virtual void shouldCleanupComment()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		taskService.createComment(null, processInstanceId, "aMessage");

		IList<Comment> comments = taskService.getProcessInstanceComments(processInstanceId);

		// assume
		assertThat(comments.Count, @is(1));

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		comments = taskService.getProcessInstanceComments(processInstanceId);

		// then
		assertThat(comments.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupAttachment()
	  public virtual void shouldCleanupAttachment()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

		taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com").Id;

		IList<Attachment> attachments = taskService.getProcessInstanceAttachments(processInstanceId);

		// assume
		assertThat(attachments.Count, @is(1));

		string taskId = taskService.createTaskQuery().singleResult().Id;

		ClockUtil.CurrentTime = END_DATE;

		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		attachments = taskService.getProcessInstanceAttachments(processInstanceId);

		// then
		assertThat(attachments.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupByteArray()
	  public virtual void shouldCleanupByteArray()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS_INCIDENT);

		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  managementService.executeJob(jobId);
		}
		catch (Exception)
		{
		}

		HistoricJobLogEventEntity jobLog = (HistoricJobLogEventEntity) historyService.createHistoricJobLogQuery().failureLog().singleResult();

		ByteArrayEntity byteArray = findByteArrayById(jobLog.ExceptionByteArrayId);

		// assume
		assertThat(byteArray, notNullValue());

		managementService.setJobRetries(jobId, 0);

		managementService.executeJob(jobId);

		ClockUtil.CurrentTime = END_DATE;

		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		byteArray = findByteArrayById(jobLog.ExceptionByteArrayId);

		// then
		assertThat(byteArray, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCleanupBatch()
	  public virtual void shouldCleanupBatch()
	  {
		// given
		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		string batchId = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason").Id;

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

		// assume
		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().list();

		assertThat(historicBatches.Count, @is(1));

		// assume
		IList<HistoricJobLog> historicJobLogs = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).list();

		assertThat(historicJobLogs.Count, @is(6));

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		runHistoryCleanup();

		historicBatches = historyService.createHistoricBatchQuery().list();
		historicJobLogs = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batchId).list();

		// then
		assertThat(historicBatches.Count, @is(0));
		assertThat(historicJobLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReportMetricsForBatchCleanup()
	  public virtual void shouldReportMetricsForBatchCleanup()
	  {
		// given
		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		testRule.deploy(CALLING_PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

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

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().list();

		// assume
		assertThat(historicBatches.Count, @is(1));

		// when
		runHistoryCleanup();

		long removedBatchesSum = managementService.createMetricsQuery().name(Metrics.HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS).sum();

		// then
		assertThat(removedBatchesSum, @is(1L));
	  }

	  // parallelism test cases ////////////////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldDistributeWorkForDecisions()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldDistributeWorkForDecisions()
	  {
		// given
		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string jobId = managementService.createJobQuery().singleResult().Id;
			managementService.executeJob(jobId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(decisionInstances.Count, @is(45));

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(decisionInstances.Count, @is(30));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(decisionInstances.Count, @is(15));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(decisionInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForProcessInstances()
	  public virtual void shouldDistributeWorkForProcessInstances()
	  {
		// given
		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(PROCESS_KEY);

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricProcessInstance> processInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(processInstances.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		processInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(processInstances.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		processInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(processInstances.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		processInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(processInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForActivityInstances()
	  public virtual void shouldDistributeWorkForActivityInstances()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string taskId = taskService.createTaskQuery().singleResult().Id;

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricActivityInstance> activityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(activityInstances.Count, @is(90));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		activityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// then
		assertThat(activityInstances.Count, @is(60));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		activityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// then
		assertThat(activityInstances.Count, @is(30));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		activityInstances = historyService.createHistoricActivityInstanceQuery().list();

		// then
		assertThat(activityInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForTaskInstances()
	  public virtual void shouldDistributeWorkForTaskInstances()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string taskId = taskService.createTaskQuery().singleResult().Id;

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricTaskInstance> taskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(taskInstances.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		taskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// then
		assertThat(taskInstances.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		taskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// then
		assertThat(taskInstances.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		taskInstances = historyService.createHistoricTaskInstanceQuery().list();

		// then
		assertThat(taskInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForVariableInstances()
	  public virtual void shouldDistributeWorkForVariableInstances()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string taskId = taskService.createTaskQuery().singleResult().Id;

			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricVariableInstance> variableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(variableInstances.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		variableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// then
		assertThat(variableInstances.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		variableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// then
		assertThat(variableInstances.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		variableInstances = historyService.createHistoricVariableInstanceQuery().list();

		// then
		assertThat(variableInstances.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForDetails()
	  public virtual void shouldDistributeWorkForDetails()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			runtimeService.setVariable(processInstance.Id, "aVariableName", Variables.stringValue("anotherVariableValue"));

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(historicDetails.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		historicDetails = historyService.createHistoricDetailQuery().list();

		// then
		assertThat(historicDetails.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		historicDetails = historyService.createHistoricDetailQuery().list();

		// then
		assertThat(historicDetails.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		historicDetails = historyService.createHistoricDetailQuery().list();

		// then
		assertThat(historicDetails.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForIncidents()
	  public virtual void shouldDistributeWorkForIncidents()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS_INCIDENT);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string jobId = managementService.createJobQuery().singleResult().Id;

			managementService.setJobRetries(jobId, 0);

			try
			{
			  managementService.executeJob(jobId);
			}
			catch (Exception)
			{
			}

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricIncident> historicIncidents = historyService.createHistoricIncidentQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(historicIncidents.Count, @is(30));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		historicIncidents = historyService.createHistoricIncidentQuery().list();

		// then
		assertThat(historicIncidents.Count, @is(20));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		historicIncidents = historyService.createHistoricIncidentQuery().list();

		// then
		assertThat(historicIncidents.Count, @is(10));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		historicIncidents = historyService.createHistoricIncidentQuery().list();

		// then
		assertThat(historicIncidents.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForExternalTaskLogs()
	  public virtual void shouldDistributeWorkForExternalTaskLogs()
	  {
		// given
		testRule.deploy(Bpmn.createExecutableProcess("calledProcess").startEvent().serviceTask().camundaExternalTask("anExternalTaskTopic").endEvent().done());

		testRule.deploy(Bpmn.createExecutableProcess("callingProcess").camundaHistoryTimeToLive(5).startEvent().callActivity().calledElement("calledProcess").endEvent().done());

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey("callingProcess");

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			LockedExternalTask externalTask = externalTaskService.fetchAndLock(1, "aWorkerId").topic("anExternalTaskTopic", 3000).execute()[0];

			externalTaskService.complete(externalTask.Id, "aWorkerId");
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricExternalTaskLog> externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(externalTaskLogs.Count, @is(30));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs.Count, @is(20));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs.Count, @is(10));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		externalTaskLogs = historyService.createHistoricExternalTaskLogQuery().list();

		// then
		assertThat(externalTaskLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForJobLogs()
	  public virtual void shouldDistributeWorkForJobLogs()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string jobId = managementService.createJobQuery().singleResult().Id;

			managementService.executeJob(jobId);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(jobLogs.Count, @is(30));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(jobLogs.Count, @is(20));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(jobLogs.Count, @is(10));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		jobLogs = historyService.createHistoricJobLogQuery().processDefinitionKey(PROCESS_KEY).list();

		// then
		assertThat(jobLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForUserOperationLogs()
	  public virtual void shouldDistributeWorkForUserOperationLogs()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().camundaAsyncBefore().userTask("userTask").name("userTask").endEvent().done());

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string jobId = managementService.createJobQuery().singleResult().Id;

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			identityService.AuthenticatedUserId = "aUserId";
			managementService.setJobRetries(jobId, 65);
			identityService.clearAuthentication();

			managementService.executeJob(jobId);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<UserOperationLogEntry> userOperationLogs = historyService.createUserOperationLogQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(userOperationLogs.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		userOperationLogs = historyService.createUserOperationLogQuery().list();

		// then
		assertThat(userOperationLogs.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		userOperationLogs = historyService.createUserOperationLogQuery().list();

		// then
		assertThat(userOperationLogs.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		userOperationLogs = historyService.createUserOperationLogQuery().list();

		// then
		assertThat(userOperationLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForIdentityLinkLogs()
	  public virtual void shouldDistributeWorkForIdentityLinkLogs()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			string taskId = taskService.createTaskQuery().singleResult().Id;

			taskService.addCandidateUser(taskId, "aUserId");

			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<HistoricIdentityLinkLog> historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(historicIdentityLinkLogs.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// then
		assertThat(historicIdentityLinkLogs.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// then
		assertThat(historicIdentityLinkLogs.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		historicIdentityLinkLogs = historyService.createHistoricIdentityLinkLogQuery().list();

		// then
		assertThat(historicIdentityLinkLogs.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForComment()
	  public virtual void shouldDistributeWorkForComment()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

			processInstanceIds.Add(processInstanceId);

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			taskService.createComment(null, processInstanceId, "aMessage");

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<Comment> comments = getCommentsBy(processInstanceIds);

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(comments.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		comments = getCommentsBy(processInstanceIds);

		// then
		assertThat(comments.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		comments = getCommentsBy(processInstanceIds);

		// then
		assertThat(comments.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		comments = getCommentsBy(processInstanceIds);

		// then
		assertThat(comments.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForAttachment()
	  public virtual void shouldDistributeWorkForAttachment()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(PROCESS);

		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string processInstanceId = runtimeService.createProcessInstanceQuery().activityIdIn("userTask").singleResult().Id;

			processInstanceIds.Add(processInstanceId);

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com").Id;

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		IList<Attachment> attachments = getAttachmentsBy(processInstanceIds);

		// assume
		assertThat(jobs.Count, @is(3));
		assertThat(attachments.Count, @is(15));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		attachments = getAttachmentsBy(processInstanceIds);

		// then
		assertThat(attachments.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		attachments = getAttachmentsBy(processInstanceIds);

		// then
		assertThat(attachments.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		attachments = getAttachmentsBy(processInstanceIds);

		// then
		assertThat(attachments.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForByteArray()
	  public virtual void shouldDistributeWorkForByteArray()
	  {
		// given
		testRule.deploy(CALLING_PROCESS);

		testRule.deploy(CALLED_PROCESS_INCIDENT);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

			string jobId = managementService.createJobQuery().singleResult().Id;

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			try
			{
			  managementService.executeJob(jobId);
			}
			catch (Exception)
			{
			}

			managementService.setJobRetries(jobId, 0);

			managementService.executeJob(jobId);

			string taskId = taskService.createTaskQuery().singleResult().Id;
			taskService.complete(taskId);
		  }
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		// assume
		assertThat(jobs.Count, @is(3));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		IList<ByteArrayEntity> byteArrays = findByteArrays();

		// then
		assertThat(byteArrays.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		byteArrays = findByteArrays();

		// then
		assertThat(byteArrays.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		byteArrays = findByteArrays();

		// then
		assertThat(byteArrays.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldDistributeWorkForBatches()
	  public virtual void shouldDistributeWorkForBatches()
	  {
		// given
		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		testRule.deploy(CALLING_PROCESS);

		for (int i = 0; i < 60; i++)
		{
		  if (i % 4 == 0)
		  {
			string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

			ClockUtil.CurrentTime = addMinutes(END_DATE, i);

			runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

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
		}

		// assume
		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().list();

		assertThat(historicBatches.Count, @is(15));

		ClockUtil.CurrentTime = addDays(END_DATE, 6);

		engineConfiguration.HistoryCleanupDegreeOfParallelism = 3;
		engineConfiguration.initHistoryCleanup();

		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();

		// assume
		assertThat(jobs.Count, @is(3));

		Job jobOne = jobs[0];
		jobIds.Add(jobOne.Id);

		// when
		managementService.executeJob(jobOne.Id);

		historicBatches = historyService.createHistoricBatchQuery().list();

		// then
		assertThat(historicBatches.Count, @is(10));

		Job jobTwo = jobs[1];
		jobIds.Add(jobTwo.Id);

		// when
		managementService.executeJob(jobTwo.Id);

		historicBatches = historyService.createHistoricBatchQuery().list();

		// then
		assertThat(historicBatches.Count, @is(5));

		Job jobThree = jobs[2];
		jobIds.Add(jobThree.Id);

		// when
		managementService.executeJob(jobThree.Id);

		historicBatches = historyService.createHistoricBatchQuery().list();

		// then
		assertThat(historicBatches.Count, @is(0));
	  }

	  // report tests //////////////////////////////////////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSeeCleanableButNoFinishedProcessInstancesInReport()
	  public virtual void shouldSeeCleanableButNoFinishedProcessInstancesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.deploy(PROCESS);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricProcessInstanceReportResult report = historyService.createCleanableHistoricProcessInstanceReport().compact().singleResult();

		// then
		assertThat(report.CleanableProcessInstanceCount, @is(5L));
		assertThat(report.FinishedProcessInstanceCount, @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSeeFinishedButNoCleanableProcessInstancesInReport()
	  public virtual void shouldSeeFinishedButNoCleanableProcessInstancesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.deploy(PROCESS);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		  string taskId = taskService.createTaskQuery().singleResult().Id;
		  taskService.complete(taskId);
		}

		// when
		CleanableHistoricProcessInstanceReportResult report = historyService.createCleanableHistoricProcessInstanceReport().compact().singleResult();

		// then
		assertThat(report.FinishedProcessInstanceCount, @is(5L));
		assertThat(report.CleanableProcessInstanceCount, @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSeeCleanableProcessInstancesReport()
	  public virtual void shouldNotSeeCleanableProcessInstancesReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.deploy(PROCESS);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricProcessInstanceReportResult report = historyService.createCleanableHistoricProcessInstanceReport().compact().singleResult();

		// then
		assertThat(report, nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSeeCleanableDecisionInstancesInReport()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSeeCleanableDecisionInstancesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricDecisionInstanceReportResult report = historyService.createCleanableHistoricDecisionInstanceReport().decisionDefinitionKeyIn("dish-decision").compact().singleResult();

		// then
		assertThat(report.CleanableDecisionInstanceCount, @is(5L));
		assertThat(report.FinishedDecisionInstanceCount, @is(5L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSeeCleanableDecisionInstancesInReport()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSeeCleanableDecisionInstancesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.deploy(CALLING_PROCESS_CALLS_DMN);

		ClockUtil.CurrentTime = END_DATE;

		for (int i = 0; i < 5; i++)
		{
		  runtimeService.startProcessInstanceByKey(CALLING_PROCESS_CALLS_DMN_KEY, Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));
		}

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricDecisionInstanceReportResult report = historyService.createCleanableHistoricDecisionInstanceReport().decisionDefinitionKeyIn("dish-decision").compact().singleResult();

		// then
		assertThat(report.CleanableDecisionInstanceCount, @is(0L));
		assertThat(report.FinishedDecisionInstanceCount, @is(5L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSeeCleanableBatchesInReport()
	  public virtual void shouldSeeCleanableBatchesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		ClockUtil.CurrentTime = END_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricBatchReportResult report = historyService.createCleanableHistoricBatchReport().singleResult();

		// then
		assertThat(report.CleanableBatchesCount, @is(1L));
		assertThat(report.FinishedBatchesCount, @is(0L));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSeeCleanableBatchesInReport()
	  public virtual void shouldNotSeeCleanableBatchesInReport()
	  {
		// given
		engineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		engineConfiguration.BatchOperationHistoryTimeToLive = "P5D";
		engineConfiguration.initHistoryCleanup();

		testRule.deploy(PROCESS);

		string processInstanceId = runtimeService.startProcessInstanceByKey(PROCESS_KEY).Id;

		ClockUtil.CurrentTime = END_DATE;

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		ClockUtil.CurrentTime = addDays(END_DATE, 5);

		// when
		CleanableHistoricBatchReportResult report = historyService.createCleanableHistoricBatchReport().singleResult();

		// then
		assertThat(report.CleanableBatchesCount, @is(0L));
		assertThat(report.FinishedBatchesCount, @is(0L));

		// cleanup
		managementService.deleteBatch(batch.Id, true);
	  }

	  // helper /////////////////////////////////////////////////////////////////

	  protected internal virtual IList<Job> runHistoryCleanup()
	  {
		historyService.cleanUpHistoryAsync(true);

		IList<Job> jobs = historyService.findHistoryCleanupJobs();
		foreach (Job job in jobs)
		{
		  jobIds.Add(job.Id);
		  managementService.executeJob(job.Id);
		}

		return jobs;
	  }

	  protected internal virtual IList<Attachment> getAttachmentsBy(IList<string> processInstanceIds)
	  {
		IList<Attachment> attachments = new List<Attachment>();
		foreach (string processInstanceId in processInstanceIds)
		{
		  ((IList<Attachment>)attachments).AddRange(taskService.getProcessInstanceAttachments(processInstanceId));
		}

		return attachments;
	  }

	  protected internal virtual IList<Comment> getCommentsBy(IList<string> processInstanceIds)
	  {
		IList<Comment> comments = new List<Comment>();
		foreach (string processInstanceId in processInstanceIds)
		{
		  ((IList<Comment>)comments).AddRange(taskService.getProcessInstanceComments(processInstanceId));
		}

		return comments;
	  }

	  protected internal virtual ByteArrayEntity findByteArrayById(string byteArrayId)
	  {
		return engineConfiguration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayId));
	  }

	  protected internal virtual IList<ByteArrayEntity> findByteArrays()
	  {
		IList<HistoricJobLog> jobLogs = historyService.createHistoricJobLogQuery().failureLog().list();

		IList<ByteArrayEntity> byteArrays = new List<ByteArrayEntity>();
		foreach (HistoricJobLog jobLog in jobLogs)
		{
		  byteArrays.Add(findByteArrayById(((HistoricJobLogEventEntity) jobLog).ExceptionByteArrayId));
		}

		return byteArrays;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJobLog(final String jobId)
	  protected internal virtual void clearJobLog(string jobId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this, jobId));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly HistoryCleanupRemovalTimeTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass(HistoryCleanupRemovalTimeTest outerInstance, string jobId)
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
		engineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, jobId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly HistoryCleanupRemovalTimeTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass2(HistoryCleanupRemovalTimeTest outerInstance, string jobId)
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

	  protected internal virtual void clearMeterLog()
	  {
		engineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private readonly HistoryCleanupRemovalTimeTest outerInstance;

		  public CommandAnonymousInnerClass3(HistoryCleanupRemovalTimeTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.MeterLogManager.deleteAll();

			return null;
		  }
	  }

	}

}