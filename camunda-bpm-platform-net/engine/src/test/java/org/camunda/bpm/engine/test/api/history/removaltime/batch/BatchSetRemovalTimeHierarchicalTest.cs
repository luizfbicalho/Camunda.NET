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
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using BatchSetRemovalTimeRule = org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule.addDays;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(HISTORY_FULL)]
	public class BatchSetRemovalTimeHierarchicalTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchSetRemovalTimeHierarchicalTest()
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
			CURRENT_DATE = testRule.CURRENT_DATE;
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule engineTestRule;
	  protected internal BatchSetRemovalTimeRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(engineTestRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal DateTime CURRENT_DATE;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

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
		testRule.process().call().passVars("temperature", "dayType").ruleTask("dish-decision").userTask().deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[2].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecision_RootDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecision_RootDecisionInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("dish-decision").list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("dish-decision").list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeToStandaloneDecision_ChildDecisionInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeToStandaloneDecision_ChildDecisionInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTime_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_DecisionInputInstance()
	  {
		// given
		testRule.process().call().passVars("temperature", "dayType").ruleTask("dish-decision").userTask().deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// assume
		assertThat(historicDecisionInputInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInputInstances[1].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInputInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_RootDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_RootDecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("dish-decision").singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// assume
		assertThat(historicDecisionInputInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("dish-decision").singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ChildDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ChildDecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("season").singleResult();

		IList<HistoricDecisionInputInstance> historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// assume
		assertThat(historicDecisionInputInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("season").singleResult();

		historicDecisionInputInstances = historicDecisionInstance.Inputs;

		// then
		assertThat(historicDecisionInputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTime_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_DecisionOutputInstance()
	  {
		// given
		testRule.process().call().passVars("temperature", "dayType").ruleTask("dish-decision").userTask().deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// assume
		assertThat(historicDecisionOutputInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_RootDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_RootDecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("dish-decision").singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// assume
		assertThat(historicDecisionOutputInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("dish-decision").singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ChildDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ChildDecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("season").singleResult();

		IList<HistoricDecisionOutputInstance> historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// assume
		assertThat(historicDecisionOutputInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("season").singleResult();

		historicDecisionOutputInstances = historicDecisionInstance.Outputs;

		// then
		assertThat(historicDecisionOutputInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ProcessInstance()
	  public virtual void shouldSetRemovalTime_ProcessInstance()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().rootProcessInstances().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ActivityInstance()
	  public virtual void shouldSetRemovalTime_ActivityInstance()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		HistoricActivityInstance historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityName("userTask").singleResult();

		// assume
		assertThat(historicActivityInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicActivityInstance = historyService.createHistoricActivityInstanceQuery().activityName("userTask").singleResult();

		// then
		assertThat(historicActivityInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_TaskInstance()
	  public virtual void shouldSetRemovalTime_TaskInstance()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		// assume
		assertThat(historicTaskInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicTaskInstance = historyService.createHistoricTaskInstanceQuery().singleResult();

		// then
		assertThat(historicTaskInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_VariableInstance()
	  public virtual void shouldSetRemovalTime_VariableInstance()
	  {
		// given
		testRule.process().call().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", "aVariableValue"));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// assume
		assertThat(historicVariableInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// then
		assertThat(historicVariableInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Detail()
	  public virtual void shouldSetRemovalTime_Detail()
	  {
		// given
		testRule.process().call().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", "aVariableValue"));

		HistoricDetail historicDetail = historyService.createHistoricDetailQuery().singleResult();

		// assume
		assertThat(historicDetail.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDetail = historyService.createHistoricDetailQuery().singleResult();

		// then
		assertThat(historicDetail.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ExternalTaskLog()
	  public virtual void shouldSetRemovalTime_ExternalTaskLog()
	  {
		// given
		testRule.process().call().externalTask().deploy().start();

		HistoricExternalTaskLog historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// assume
		assertThat(historicExternalTaskLog.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().singleResult();

		// then
		assertThat(historicExternalTaskLog.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_JobLog()
	  public virtual void shouldSetRemovalTime_JobLog()
	  {
		// given
		testRule.process().call().async().userTask().deploy().start();

		HistoricJobLog job = historyService.createHistoricJobLogQuery().processDefinitionKey("process").singleResult();

		// assume
		assertThat(job.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		job = historyService.createHistoricJobLogQuery().processDefinitionKey("process").singleResult();

		// then
		assertThat(job.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Incident()
	  public virtual void shouldSetRemovalTime_Incident()
	  {
		// given
		string rootProcessInstanceId = testRule.process().call().async().userTask().deploy().start();

		string jobId = managementService.createJobQuery().singleResult().Id;

		managementService.setJobRetries(jobId, 0);

		string leafProcessInstanceId = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(rootProcessInstanceId).singleResult().Id;

		HistoricIncident historicIncident = historyService.createHistoricIncidentQuery().processInstanceId(leafProcessInstanceId).singleResult();

		// assume
		assertThat(historicIncident.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicIncident = historyService.createHistoricIncidentQuery().processInstanceId(leafProcessInstanceId).singleResult();

		// then
		assertThat(historicIncident.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_OperationLog()
	  public virtual void shouldSetRemovalTime_OperationLog()
	  {
		// given
		string processInstanceId = testRule.process().call().async().userTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";
		runtimeService.suspendProcessInstanceById(processInstanceId);
		identityService.clearAuthentication();

		UserOperationLogEntry userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// assume
		assertThat(userOperationLog.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		userOperationLog = historyService.createUserOperationLogQuery().singleResult();

		// then
		assertThat(userOperationLog.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_IdentityLinkLog()
	  public virtual void shouldSetRemovalTime_IdentityLinkLog()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		HistoricIdentityLinkLog identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// assume
		assertThat(identityLinkLog.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		identityLinkLog = historyService.createHistoricIdentityLinkLogQuery().singleResult();

		// then
		assertThat(identityLinkLog.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_CommentByTaskId()
	  public virtual void shouldSetRemovalTime_CommentByTaskId()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		taskService.createComment(taskId, null, "aComment");

		Comment comment = taskService.getTaskComments(taskId)[0];

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(comment.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		comment = taskService.getTaskComments(taskId)[0];

		// then
		assertThat(comment.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_CommentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_CommentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().call().userTask().deploy().start();

		taskService.createComment(null, processInstanceId, "aComment");

		Comment comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(comment.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		comment = taskService.getProcessInstanceComments(processInstanceId)[0];

		// then
		assertThat(comment.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_AttachmentByTaskId()
	  public virtual void shouldSetRemovalTime_AttachmentByTaskId()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		Attachment attachment = taskService.createAttachment(null, taskId, null, null, null, "http://camunda.com");

		// assume
		assertThat(attachment.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		attachment = taskService.getTaskAttachments(taskId)[0];

		// then
		assertThat(attachment.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_AttachmentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_AttachmentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().call().userTask().deploy().start();

		Attachment attachment = taskService.createAttachment(null, null, processInstanceId, null, null, "http://camunda.com");

		// assume
		assertThat(attachment.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		attachment = taskService.getProcessInstanceAttachments(processInstanceId)[0];

		// then
		assertThat(attachment.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_AttachmentByTaskId()
	  public virtual void shouldSetRemovalTime_ByteArray_AttachmentByTaskId()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		string taskId = historyService.createHistoricTaskInstanceQuery().taskName("userTask").singleResult().Id;

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, taskId, null, null, null, new MemoryStream("".GetBytes()));

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(attachment.ContentId);

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(attachment.ContentId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_AttachmentByProcessInstanceId()
	  public virtual void shouldSetRemovalTime_ByteArray_AttachmentByProcessInstanceId()
	  {
		// given
		string processInstanceId = testRule.process().call().userTask().deploy().start();

		AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment(null, null, processInstanceId, null, null, new MemoryStream("".GetBytes()));

		string byteArrayId = attachment.ContentId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_Variable()
	  public virtual void shouldSetRemovalTime_ByteArray_Variable()
	  {
		// given
		testRule.process().call().userTask().deploy().startWithVariables(Variables.createVariables().putValue("aVariableName", Variables.fileValue("file.xml").file("<root />".GetBytes())));

		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		string byteArrayId = ((HistoricVariableInstanceEntity) historicVariableInstance).ByteArrayId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_JobLog()
	  public virtual void shouldSetRemovalTime_ByteArray_JobLog()
	  {
		// given
		testRule.process().call().async().scriptTask().deploy().start();

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

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByteArray_ExternalTaskLog()
	  public virtual void shouldSetRemovalTime_ByteArray_ExternalTaskLog()
	  {
		// given
		testRule.process().call().externalTask().deploy().start();

		string externalTaskId = externalTaskService.fetchAndLock(1, "aWorkerId").topic("aTopicName", int.MaxValue).execute()[0].Id;

		externalTaskService.handleFailure(externalTaskId, "aWorkerId", null, "errorDetails", 5, 3000L);

		HistoricExternalTaskLog externalTaskLog = historyService.createHistoricExternalTaskLogQuery().failureLog().singleResult();

		string byteArrayId = ((HistoricExternalTaskLogEntity) externalTaskLog).ErrorDetailsByteArrayId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTime_ByteArray_DecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_ByteArray_DecisionInputInstance()
	  {
		// given
		testRule.process().call().passVars("pojo").ruleTask("testDecision").userTask().deploy().startWithVariables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeInputs().singleResult();

		string byteArrayId = ((HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ByteArray_RootDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ByteArray_RootDecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("root").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("root").singleResult();

		string byteArrayId = ((HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0]).ByteArrayValueId;

		testRule.updateHistoryTimeToLiveDmn("root", 5);

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ByteArray_ChildDecisionInputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ByteArray_ChildDecisionInputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("root").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeInputs().decisionDefinitionKey("child").singleResult();

		string byteArrayId = ((HistoricDecisionInputInstanceEntity) historicDecisionInstance.Inputs[0]).ByteArrayValueId;

		testRule.updateHistoryTimeToLiveDmn("root", 5);

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void shouldSetRemovalTime_ByteArray_DecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void shouldSetRemovalTime_ByteArray_DecisionOutputInstance()
	  {
		// given
		testRule.process().call().passVars("pojo").ruleTask("testDecision").userTask().deploy().startWithVariables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().includeOutputs().singleResult();

		string byteArrayId = ((HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0]).ByteArrayValueId;

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ByteArray_RootDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ByteArray_RootDecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("root").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("root").singleResult();

		string byteArrayId = ((HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0]).ByteArrayValueId;

		testRule.updateHistoryTimeToLiveDmn("root", 5);

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ByteArray_ChildDecisionOutputInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/history/removaltime/drd.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ByteArray_ChildDecisionOutputInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("root").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().includeOutputs().decisionDefinitionKey("child").singleResult();

		string byteArrayId = ((HistoricDecisionOutputInstanceEntity) historicDecisionInstance.Outputs[0]).ByteArrayValueId;

		testRule.updateHistoryTimeToLiveDmn("root", 5);

		ByteArrayEntity byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// assume
		assertThat(byteArrayEntity.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		byteArrayEntity = testRule.findByteArrayById(byteArrayId);

		// then
		assertThat(byteArrayEntity.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

	}

}