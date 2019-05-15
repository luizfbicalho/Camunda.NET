using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.history.removaltime.batch
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using HistoricDecisionInputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInputInstanceEntity;
	using HistoricDecisionOutputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionOutputInstanceEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Task = org.camunda.bpm.engine.task.Task;
	using BatchSetRemovalTimeRule = org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule;
	using TestProcessBuilder = org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule.TestProcessBuilder;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(HISTORY_FULL)]
	public class BatchSetRemovalTimeNonHierarchicalTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchSetRemovalTimeNonHierarchicalTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			engineTestRule = new ProcessEngineTestRule(engineRule);
			testRule = new BatchSetRemovalTimeRule(engineRule, engineTestRule);
			ruleChain = RuleChain.outerRule(engineRule).around(engineTestRule).around(testRule);
			REMOVAL_TIME = testRule.REMOVAL_TIME;
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule engineTestRule;
	  protected internal BatchSetRemovalTimeRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(engineTestRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal DateTime REMOVAL_TIME;

	  protected internal readonly DateTime CREATE_TIME = new DateTime(1363608000000L);

	  protected internal RuntimeService runtimeService;
	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void assignServices()
	  public virtual void assignServices()
	  {
		runtimeService = engineRule.RuntimeService;
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
		externalTaskService = engineRule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTime_DecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_DecisionInstance()
	  {
		// given
		testRule.process().ruleTask("dish-decision").deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicDecisionInstances[2].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecision_DecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecision_DecisionInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicDecisionInstances[2].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTime_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_DecisionInputInstance()
	  {
		// given
		testRule.process().ruleTask("dish-decision").deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInputInstances[1].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicDecisionInputInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecision_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecision_DecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").includeInputs().singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").includeInputs().singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTime_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_DecisionOutputInstance()
	  {
		// given
		testRule.process().ruleTask("dish-decision").deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecision_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecision_DecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("season").singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("season").singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ProcessInstance()
	  public virtual void shouldSetRemovalTime_ProcessInstance()
	  {
		// given
		testRule.process().userTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).Null;

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ActivityInstance()
	  public virtual void shouldSetRemovalTime_ActivityInstance()
	  {
		// given
		testRule.process().userTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityName("userTask").singleResult();

		// then
		assertThat(historicActivityInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_TaskInstance()
	  public virtual void shouldSetRemovalTime_TaskInstance()
	  {
		// given
		testRule.process().userTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		// then
		assertThat(historicTaskInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_VariableInstance()
	  public virtual void shouldSetRemovalTime_VariableInstance()
	  {
		// given
		testRule.process().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", "aVariableValue"));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// then
		assertThat(historicVariableInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Detail()
	  public virtual void shouldSetRemovalTime_Detail()
	  {
		// given
		testRule.process().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", "aVariableValue"));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();

		// then
		assertThat(historicDetail.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ExternalTaskLog()
	  public virtual void shouldSetRemovalTime_ExternalTaskLog()
	  {
		// given
		testRule.process().externalTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		HistoricExternalTaskLog historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// assume
		assertThat(historicExternalTaskLog.RemovalTime).Null;

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// then
		assertThat(historicExternalTaskLog.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-10172
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ExternalTaskLog_WithPreservedCreateTime()
	  public virtual void shouldSetRemovalTime_ExternalTaskLog_WithPreservedCreateTime()
	  {
		// given
		ClockUtil.CurrentTime = CREATE_TIME;

		testRule.process().externalTask().deploy().start();

		HistoricExternalTaskLog historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// assume
		assertThat(historicExternalTaskLog.Timestamp).isEqualTo(CREATE_TIME);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// then
		assertThat(historicExternalTaskLog.Timestamp).isEqualTo(CREATE_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_JobLog()
	  public virtual void shouldSetRemovalTime_JobLog()
	  {
		// given
		string processInstanceId = testRule.process().async().userTask().deploy().start();

		HistoricJobLog job = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId).singleResult();

		// assume
		assertThat(job.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		job = historyService.createHistoricJobLogQuery().processInstanceId(processInstanceId).singleResult();

		// then
		assertThat(job.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Incident()
	  public virtual void shouldSetRemovalTime_Incident()
	  {
		// given
		testRule.process().async().userTask().deploy().start();

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// assume
		assertThat(historicIncident.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-10172
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Incident_WithPreservedCreateTime()
	  public virtual void shouldSetRemovalTime_Incident_WithPreservedCreateTime()
	  {
		// given
		ClockUtil.CurrentTime = CREATE_TIME;

		testRule.process().async().userTask().deploy().start();

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// assume
		assertThat(historicIncident.CreateTime).isEqualTo(CREATE_TIME);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.CreateTime).isEqualTo(CREATE_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_OperationLog()
	  public virtual void shouldSetRemovalTime_OperationLog()
	  {
		// given
		string processInstanceId = testRule.process().async().userTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";
		runtimeService.suspendProcessInstanceById(processInstanceId);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// then
		assertThat(userOperationLog.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-10172
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_OperationLog_WithPreservedTimestamp()
	  public virtual void shouldSetRemovalTime_OperationLog_WithPreservedTimestamp()
	  {
		// given
		ClockUtil.CurrentTime = CREATE_TIME;

		string processInstanceId = testRule.process().async().userTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";
		runtimeService.suspendProcessInstanceById(processInstanceId);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.Timestamp).isEqualTo(CREATE_TIME);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// then
		assertThat(userOperationLog.Timestamp).isEqualTo(CREATE_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_IdentityLinkLog()
	  public virtual void shouldSetRemovalTime_IdentityLinkLog()
	  {
		// given
		testRule.process().userTask().deploy().start();

		HistoricIdentityLinkLog identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(identityLinkLog.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// then
		assertThat(identityLinkLog.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

	  /// <summary>
	  /// See https://app.camunda.com/jira/browse/CAM-10172
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_IdentityLinkLog_WithPreservedTime()
	  public virtual void shouldSetRemovalTime_IdentityLinkLog_WithPreservedTime()
	  {
		// given
		ClockUtil.CurrentTime = CREATE_TIME;

		testRule.process().userTask().deploy().start();

		HistoricIdentityLinkLog identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(identityLinkLog.Time).isEqualTo(CREATE_TIME);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// then
		assertThat(identityLinkLog.Time).isEqualTo(CREATE_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetUnaffectedRemovalTime_IdentityLinkLog()
	  public virtual void shouldNotSetUnaffectedRemovalTime_IdentityLinkLog()
	  {
		// given
		BatchSetRemovalTimeRule.TestProcessBuilder testProcessBuilder = testRule.process().userTask().deploy();

		string instance1 = testProcessBuilder.start();
		string instance2 = testProcessBuilder.start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query.processInstanceId(instance1)).executeAsync());

		Task task2 = taskService.createTaskQuery().processInstanceId(instance2).singleResult();

		HistoricIdentityLinkLog identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().taskId(task2.Id).singleResult();

		// then
		assertThat(identityLinkLog.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_CommentByTaskId()
	  public virtual void shouldSetRemovalTime_CommentByTaskId()
	  {
		// given
		testRule.process().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		taskService.createComment(taskId, null, "aComment");

		Comment comment = taskService.getTaskComments(taskId)[0];

		// assume
		assertThat(comment.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		comment = taskService.getTaskComments(taskId)[0];

		// then
		assertThat(comment.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_CommentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_CommentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		taskService.createComment(null, processInstanceId, "aComment");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// assume
		assertThat(comment.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// then
		assertThat(comment.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_AttachmentByTaskId()
	  public virtual void shouldSetRemovalTime_AttachmentByTaskId()
	  {
		// given
		testRule.process().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		Attachment attachment = taskService.createAttachment(null, taskId, null, null, null, "http://camunda.com");

		// assume
		assertThat(attachment.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		attachment = taskService.getTaskAttachments(taskId)[0];

		// then
		assertThat(attachment.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_AttachmentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_AttachmentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		Attachment attachment = taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com");

		// assume
		assertThat(attachment.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		attachment = taskService.getProcessInstanceAttachments(processInstanceId)[0];

		// then
		assertThat(attachment.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_AttachmentByTaskId()
	  public virtual void shouldSetRemovalTime_ByteArray_AttachmentByTaskId()
	  {
		// given
		testRule.process().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, taskId, null, null, null, new MemoryStream("".GetBytes()));

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(attachment.ContentId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(attachment.ContentId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_AttachmentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_ByteArray_AttachmentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, null, processInstanceId, null, null, new MemoryStream("".GetBytes()));

		string byteArrayId = attachment.ContentId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_Variable()
	  public virtual void shouldSetRemovalTime_ByteArray_Variable()
	  {
		// given
		testRule.process().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", Variables.fileValue("file.xml").file("<root />".GetBytes())));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		string byteArrayId = ((HistoricVariableInstanceEntity) historicVariableInstance).ByteArrayId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_JobLog()
	  public virtual void shouldSetRemovalTime_ByteArray_JobLog()
	  {
		// given
		testRule.process().async().scriptTask().deploy().start();

		string jobId = managementService.createJobQuery().singleResult().Id;

		try
		{
		  managementService.executeJob(jobId);

		}
		catch (Exception)
		{
		}

		HistoricJobLog historicJobLog = historyService.createHistoricJobLogQuery().failureLog().singleResult();

		string byteArrayId = ((HistoricJobLogEventEntity) historicJobLog).ExceptionByteArrayId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_ExternalTaskLog()
	  public virtual void shouldSetRemovalTime_ByteArray_ExternalTaskLog()
	  {
		// given
		testRule.process().externalTask().deploy().start();

		string externalTaskId = externalTaskService.fetchAndLock(1, "aWorkerId").topic("aTopicName", int.MaxValue).execute()[0].Id;

		externalTaskService.handleFailure(externalTaskId, "aWorkerId", null, "errorDetails", 5, 3000L);

		HistoricExternalTaskLog externalTaskLog = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		string byteArrayId = ((HistoricExternalTaskLogEntity) externalTaskLog).ErrorDetailsByteArrayId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTime_ByteArray_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_ByteArray_DecisionInputInstance()
	  {
		// given
		testRule.process().ruleTask("testDecision").deploy().startWithVariables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		string byteArrayId = ((HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecisions_ByteArray_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecisions_ByteArray_DecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("testDecision").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		string byteArrayId = ((HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTime_ByteArray_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_ByteArray_DecisionOutputInstance()
	  {
		// given
		testRule.process().ruleTask("testDecision").deploy().startWithVariables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		string byteArrayId = ((HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecisions_ByteArray_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecisions_ByteArray_DecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("testDecision").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		string byteArrayId = ((HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeToBatch()
	  public virtual void shouldSetRemovalTimeToBatch()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeToBatch_JobLog()
	  public virtual void shouldSetRemovalTimeToBatch_JobLog()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		HistoricJobLog historicJobLog = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batch.Id).singleResult();

		// assume
		assertThat(historicJobLog.RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicJobLog = historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batch.Id).singleResult();

		// then
		assertThat(historicJobLog.RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeToBatch_JobLogByteArray()
	  public virtual void shouldSetRemovalTimeToBatch_JobLogByteArray()
	  {
		// given
		string processInstance = testRule.process().failingCustomListener().deploy().start();
		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstance), "aDeleteReason");

		try
		{
		  testRule.syncExec(batch);
		}
		catch (Exception e)
		{
		  // assume
		  assertThat(e).hasMessage("I'm supposed to fail!");
		}

		HistoricJobLogEventEntity historicJobLog = (HistoricJobLogEventEntity) historyService.createHistoricJobLogQuery().jobDefinitionConfiguration(batch.Id).failureLog().singleResult();

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(historicJobLog.ExceptionByteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		byteArrayEntity = testRule.findByteArrayById(historicJobLog.ExceptionByteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batch.Id, true);
		runtimeService.deleteProcessInstance(processInstance, "", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeToBatch_Incident()
	  public virtual void shouldSetRemovalTimeToBatch_Incident()
	  {
		// given
		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList("aProcessInstanceId"), "aDeleteReason");

		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// assume
		assertThat(historicIncident.RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicIncident = historyService.createHistoricIncidentQuery().singleResult();

		// then
		assertThat(historicIncident.RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

	}

}