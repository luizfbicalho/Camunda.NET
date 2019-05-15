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
namespace org.camunda.bpm.engine.test.api.history.removaltime.batch
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder;
	using SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricDecisionInstancesBuilder;
	using SetRemovalTimeToHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricProcessInstancesBuilder;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using BatchSetRemovalTimeRule = org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_NONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule.addDays;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.contains;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>

	[RequiredHistoryLevel(HISTORY_FULL)]
	public class BatchSetRemovalTimeTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchSetRemovalTimeTest()
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
			REMOVAL_TIME = testRule.REMOVAL_TIME;
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule engineTestRule;
	  protected internal BatchSetRemovalTimeRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(engineTestRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal DateTime CURRENT_DATE;
	  protected internal DateTime REMOVAL_TIME;

	  protected internal RuntimeService runtimeService;
	  private DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void assignServices()
	  public virtual void assignServices()
	  {
		runtimeService = engineRule.RuntimeService;
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSetRemovalTime_DmnDisabled()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSetRemovalTime_DmnDisabled()
	  {
		// given
		testRule.ProcessEngineConfiguration.DmnEnabled = false;

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
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSetRemovalTimeInHierarchy_DmnDisabled()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSetRemovalTimeInHierarchy_DmnDisabled()
	  {
		// given
		testRule.ProcessEngineConfiguration.DmnEnabled = false;

		testRule.process().call().passVars("temperature", "dayType").ruleTask("dish-decision").deploy().startWithVariables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend"));

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSetRemovalTimeForStandaloneDecision_DmnDisabled()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSetRemovalTimeForStandaloneDecision_DmnDisabled()
	  {
		// given
		testRule.ProcessEngineConfiguration.DmnEnabled = false;

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
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_MultipleInvocationsPerBatchJob()
	  public virtual void shouldSetRemovalTime_MultipleInvocationsPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;

		testRule.process().userTask().deploy().start();
		testRule.process().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_MultipleInvocationsPerBatchJob()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_MultipleInvocationsPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
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
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_MultipleInvocationsPerBatchJob()
	  public virtual void shouldSetRemovalTimeForBatch_MultipleInvocationsPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;

		string processInstanceIdOne = testRule.process().userTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdOne), "");

		string processInstanceIdTwo = testRule.process().userTask().deploy().start();
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdTwo), "");

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicBatches[1].RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batchOne.Id, true);
		managementService.deleteBatch(batchTwo.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_SingleInvocationPerBatchJob()
	  public virtual void shouldSetRemovalTime_SingleInvocationPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 1;

		testRule.process().userTask().deploy().start();
		testRule.process().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_SingleInvocationPerBatchJob()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_SingleInvocationPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 1;

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
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
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_SingleInvocationPerBatchJob()
	  public virtual void shouldSetRemovalTimeForBatch_SingleInvocationPerBatchJob()
	  {
		// given
		testRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 1;

		string processInstanceIdOne = testRule.process().userTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdOne), "");

		string processInstanceIdTwo = testRule.process().userTask().deploy().start();
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdTwo), "");

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicBatches[1].RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batchOne.Id, true);
		managementService.deleteBatch(batchTwo.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTime_BaseTimeNone()
	  public virtual void shouldNotSetRemovalTime_BaseTimeNone()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		testRule.process().ttl(5).serviceTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTime_BaseTimeNone()
	  public virtual void shouldClearRemovalTime_BaseTimeNone()
	  {
		// given
		testRule.process().ttl(5).serviceTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSetRemovalTimeForStandaloneDecision_BaseTimeNone()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSetRemovalTimeForStandaloneDecision_BaseTimeNone()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldClearRemovalTimeForStandaloneDecision_BaseTimeNone()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldClearRemovalTimeForStandaloneDecision_BaseTimeNone()
	  {
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).NotNull;
		assertThat(historicDecisionInstances[1].RemovalTime).NotNull;
		assertThat(historicDecisionInstances[2].RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTimeInHierarchy_BaseTimeNone()
	  public virtual void shouldNotSetRemovalTimeInHierarchy_BaseTimeNone()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		testRule.process().ttl(5).call().serviceTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTimeInHierarchy_BaseTimeNone()
	  public virtual void shouldClearRemovalTimeInHierarchy_BaseTimeNone()
	  {
		// given
		testRule.process().ttl(5).call().serviceTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).NotNull;
		assertThat(historicProcessInstances[1].RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldNotSetRemovalTimeForStandaloneDecisionInHierarchy_BaseTimeNone()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldNotSetRemovalTimeForStandaloneDecisionInHierarchy_BaseTimeNone()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldClearRemovalTimeForStandaloneDecisionInHierarchy_BaseTimeNone()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldClearRemovalTimeForStandaloneDecisionInHierarchy_BaseTimeNone()
	  {
		// given
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE).initHistoryRemovalTime();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTimeForBatch_BaseTimeNone()
	  public virtual void shouldNotSetRemovalTimeForBatch_BaseTimeNone()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;
		configuration.HistoryCleanupStrategy = "endTimeBased";
		configuration.HistoryRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_NONE;

		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		string processInstanceIdOne = testRule.process().serviceTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdOne), "");
		testRule.syncExec(batchOne);

		string processInstanceIdTwo = testRule.process().serviceTask().deploy().start();
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdTwo), "");
		testRule.syncExec(batchTwo);

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTimeForBatch_BaseTimeNone()
	  public virtual void shouldClearRemovalTimeForBatch_BaseTimeNone()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;

		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		string processInstanceIdOne = testRule.process().serviceTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdOne), "");
		testRule.syncExec(batchOne);

		string processInstanceIdTwo = testRule.process().serviceTask().deploy().start();
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceIdTwo), "");
		testRule.syncExec(batchTwo);

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).NotNull;
		assertThat(historicBatches[1].RemovalTime).NotNull;

		configuration.HistoryRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_NONE;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_BaseTimeStart()
	  public virtual void shouldSetRemovalTime_BaseTimeStart()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().userTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("process", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(BatchSetRemovalTimeRule.addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_BaseTimeStart()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_BaseTimeStart()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_BaseTimeStart()
	  public virtual void shouldSetRemovalTimeForBatch_BaseTimeStart()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;
		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeInHierarchy_BaseTimeStart()
	  public virtual void shouldSetRemovalTimeInHierarchy_BaseTimeStart()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().call().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeInHierarchyForStandaloneDecision_BaseTimeStart()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeInHierarchyForStandaloneDecision_BaseTimeStart()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTime_BaseTimeEnd()
	  public virtual void shouldNotSetRemovalTime_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.process().ttl(5).userTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTime_BaseTimeEnd()
	  public virtual void shouldClearRemovalTime_BaseTimeEnd()
	  {
		// given
		testRule.process().ttl(5).userTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTimeForBatch_BaseTimeEnd()
	  public virtual void shouldNotSetRemovalTimeForBatch_BaseTimeEnd()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;

		configuration.HistoryRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_END;

		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).Null;

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTimeForBatch_BaseTimeEnd()
	  public virtual void shouldClearRemovalTimeForBatch_BaseTimeEnd()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;
		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).NotNull;

		configuration.HistoryRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_END;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).Null;

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetRemovalTimeInHierarchy_BaseTimeEnd()
	  public virtual void shouldNotSetRemovalTimeInHierarchy_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.process().call().ttl(5).userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldClearRemovalTimeInHierarchy_BaseTimeEnd()
	  public virtual void shouldClearRemovalTimeInHierarchy_BaseTimeEnd()
	  {
		// given
		testRule.process().call().ttl(5).userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).NotNull;
		assertThat(historicProcessInstances[1].RemovalTime).NotNull;

		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_BaseTimeEnd()
	  public virtual void shouldSetRemovalTime_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.process().serviceTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLive("process", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(BatchSetRemovalTimeRule.addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_BaseTimeEnd()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_BaseTimeEnd()
	  public virtual void shouldSetRemovalTimeForBatch_BaseTimeEnd()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;

		configuration.HistoryRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_END;

		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		ClockUtil.CurrentTime = addDays(CURRENT_DATE, 1);

		testRule.syncExec(batch);

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5 + 1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeInHierarchy_BaseTimeEnd()
	  public virtual void shouldSetRemovalTimeInHierarchy_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		testRule.process().call().serviceTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		testRule.updateHistoryTimeToLive("rootProcess", 5);

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeInHierarchyForStandaloneDecision_BaseTimeEnd()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeInHierarchyForStandaloneDecision_BaseTimeEnd()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END).initHistoryRemovalTime();

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Null()
	  public virtual void shouldSetRemovalTime_Null()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().ttl(5).userTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(BatchSetRemovalTimeRule.addDays(CURRENT_DATE, 5));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_Null()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_Null()
	  {
		// given
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_Null()
	  public virtual void shouldSetRemovalTimeForBatch_Null()
	  {
		// given
		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;

		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).NotNull;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().clearedRemovalTime().byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).Null;

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeInHierarchy_Null()
	  public virtual void shouldSetRemovalTimeInHierarchy_Null()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().call().ttl(5).userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeInHierarchyForStandaloneDecision_Null()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeInHierarchyForStandaloneDecision_Null()
	  {
		// given
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_Absolute()
	  public virtual void shouldSetRemovalTime_Absolute()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().ttl(5).userTask().deploy().start();

		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// assume
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(BatchSetRemovalTimeRule.addDays(CURRENT_DATE, 5));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicProcessInstance = historyService.createHistoricProcessInstanceQuery().singleResult();

		// then
		assertThat(historicProcessInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_Absolute()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_Absolute()
	  {
		// given
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly().singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_Absolute()
	  public virtual void shouldSetRemovalTimeForBatch_Absolute()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batch.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeInHierarchy_Absolute()
	  public virtual void shouldSetRemovalTimeInHierarchy_Absolute()
	  {
		// given
		testRule.ProcessEngineConfiguration.setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START).initHistoryRemovalTime();

		testRule.process().call().ttl(5).userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().rootProcessInstances();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeInHierarchyForStandaloneDecision_Absolute()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeInHierarchyForStandaloneDecision_Absolute()
	  {
		// given
		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().rootDecisionInstancesOnly();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).hierarchical().executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeInHierarchy_ByChildInstance()
	  public virtual void shouldSetRemovalTimeInHierarchy_ByChildInstance()
	  {
		// given
		string rootProcessInstance = testRule.process().call().ttl(5).userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(rootProcessInstance);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).hierarchical().executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(REMOVAL_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeInHierarchyForStandaloneDecision_ByChildInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeInHierarchyForStandaloneDecision_ByChildInstance()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn("dish-decision", 5);

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionInstanceId(historicDecisionInstance.Id);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).hierarchical().executeAsync());

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[2].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ByIds()
	  public virtual void shouldSetRemovalTime_ByIds()
	  {
		// given
		testRule.process().call().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;
		assertThat(historicProcessInstances[1].RemovalTime).Null;

		testRule.updateHistoryTimeToLive(5, "process", "rootProcess");

		IList<string> ids = new List<string>();
		foreach (HistoricProcessInstance historicProcessInstance in historicProcessInstances)
		{
		  ids.Add(historicProcessInstance.Id);
		}

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byIds(ids.ToArray()).executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestException_NotExistingIds()
	  public virtual void shouldThrowBadUserRequestException_NotExistingIds()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicProcessInstances is empty");

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byIds("aNotExistingId", "anotherNotExistingId").executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ByIds()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ByIds()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;
		assertThat(historicDecisionInstances[2].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn(5, "dish-decision", "season", "guestCount");

		IList<string> ids = new List<string>();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  ids.Add(historicDecisionInstance.Id);
		}

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byIds(ids.ToArray()).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[2].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestExceptionForStandaloneDecision_NotExistingIds()
	  public virtual void shouldThrowBadUserRequestExceptionForStandaloneDecision_NotExistingIds()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicDecisionInstances is empty");

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byIds("aNotExistingId", "anotherNotExistingId").executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_ByIds()
	  public virtual void shouldSetRemovalTimeForBatch_ByIds()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();

		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;

		IList<string> ids = new List<string>();
		foreach (HistoricBatch historicBatch in historicBatches)
		{
		  ids.Add(historicBatch.Id);
		}

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byIds(ids.ToArray()).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).isEqualTo(REMOVAL_TIME);
		assertThat(historicBatches[1].RemovalTime).isEqualTo(REMOVAL_TIME);

		// clear database
		managementService.deleteBatch(batchOne.Id, true);
		managementService.deleteBatch(batchTwo.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestExceptionForBatch_NotExistingIds()
	  public virtual void shouldThrowBadUserRequestExceptionForBatch_NotExistingIds()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicBatches is empty");

		// when
		historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byIds("aNotExistingId", "anotherNotExistingId").executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestException()
	  public virtual void shouldThrowBadUserRequestException()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicProcessInstances is empty");

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestExceptionForStandaloneDecision()
	  public virtual void shouldThrowBadUserRequestExceptionForStandaloneDecision()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicDecisionInstances is empty");

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowBadUserRequestExceptionForBatch()
	  public virtual void shouldThrowBadUserRequestExceptionForBatch()
	  {
		// given

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("historicBatches is empty");

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(REMOVAL_TIME).byQuery(query).executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProduceHistory()
	  public virtual void shouldProduceHistory()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// then
		assertThat(historicBatch.Type).isEqualTo("process-set-removal-time");
		assertThat(historicBatch.StartTime).isEqualTo(CURRENT_DATE);
		assertThat(historicBatch.EndTime).isEqualTo(CURRENT_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldProduceHistoryForStandaloneDecision()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldProduceHistoryForStandaloneDecision()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).executeAsync());

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();

		// then
		assertThat(historicBatch.Type).isEqualTo("decision-set-removal-time");
		assertThat(historicBatch.StartTime).isEqualTo(CURRENT_DATE);
		assertThat(historicBatch.EndTime).isEqualTo(CURRENT_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldProduceHistoryForBatch()
	  public virtual void shouldProduceHistoryForBatch()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		testRule.syncExec(batch);

		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).executeAsync());

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type("batch-set-removal-time").singleResult();

		// then
		assertThat(historicBatch.StartTime).isEqualTo(CURRENT_DATE);
		assertThat(historicBatch.EndTime).isEqualTo(CURRENT_DATE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoRemovalTimeSettingDefined()
	  public virtual void shouldThrowExceptionIfNoRemovalTimeSettingDefined()
	  {
		// given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		SetRemovalTimeToHistoricProcessInstancesBuilder batchBuilder = historyService.setRemovalTimeToHistoricProcessInstances().byQuery(query);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("removalTime is null");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoRemovalTimeSettingDefinedForStandaloneDecision()
	  public virtual void shouldThrowExceptionIfNoRemovalTimeSettingDefinedForStandaloneDecision()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery();

		SetRemovalTimeToHistoricDecisionInstancesBuilder batchBuilder = historyService.setRemovalTimeToHistoricDecisionInstances().byQuery(query);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("removalTime is null");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoRemovalTimeSettingDefinedForBatch()
	  public virtual void shouldThrowExceptionIfNoRemovalTimeSettingDefinedForBatch()
	  {
		// given
		HistoricBatchQuery query = historyService.createHistoricBatchQuery();

		SetRemovalTimeToHistoricBatchesBuilder batchBuilder = historyService.setRemovalTimeToHistoricBatches().byQuery(query);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("removalTime is null");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoQueryAndNoIdsDefined()
	  public virtual void shouldThrowExceptionIfNoQueryAndNoIdsDefined()
	  {
		// given
		SetRemovalTimeToHistoricProcessInstancesBuilder batchBuilder = historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(DateTime.Now);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Either query nor ids provided.");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoQueryAndNoIdsDefinedForStandaloneDecision()
	  public virtual void shouldThrowExceptionIfNoQueryAndNoIdsDefinedForStandaloneDecision()
	  {
		// given
		SetRemovalTimeToHistoricDecisionInstancesBuilder batchBuilder = historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(DateTime.Now);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Either query nor ids provided.");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionIfNoQueryAndNoIdsDefinedForBatch()
	  public virtual void shouldThrowExceptionIfNoQueryAndNoIdsDefinedForBatch()
	  {
		// given
		SetRemovalTimeToHistoricBatchesBuilder batchBuilder = historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(DateTime.Now);

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Either query nor ids provided.");

		// when
		batchBuilder.executeAsync();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_BothQueryAndIdsDefined()
	  public virtual void shouldSetRemovalTime_BothQueryAndIdsDefined()
	  {
		// given
		string rootProcessInstanceId = testRule.process().call().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLive(5, "rootProcess", "process");

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(rootProcessInstanceId);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(query).byIds(rootProcessInstanceId).executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicProcessInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_BothQueryAndIdsDefined()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_BothQueryAndIdsDefined()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKeyIn("season", "dish-decision").list();

		// assume
		assertThat(historicDecisionInstances[0].RemovalTime).Null;
		assertThat(historicDecisionInstances[1].RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn(5, "dish-decision", "season");

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("dish-decision");

		string id = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult().Id;

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(query).byIds(id).executeAsync());

		historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKeyIn("season", "dish-decision").list();

		// then
		assertThat(historicDecisionInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicDecisionInstances[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_BothQueryAndIdsDefined()
	  public virtual void shouldSetRemovalTimeForBatch_BothQueryAndIdsDefined()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");
		Batch batchTwo = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		IList<HistoricBatch> historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// assume
		assertThat(historicBatches[0].RemovalTime).Null;
		assertThat(historicBatches[1].RemovalTime).Null;

		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;
		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).batchId(batchOne.Id);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(query).byIds(batchTwo.Id).executeAsync());

		historicBatches = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		// then
		assertThat(historicBatches[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
		assertThat(historicBatches[1].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		// clear database
		managementService.deleteBatch(batchOne.Id, true);
		managementService.deleteBatch(batchTwo.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTime_ExistingAndNotExistingId()
	  public virtual void shouldSetRemovalTime_ExistingAndNotExistingId()
	  {
		// given
		string processInstanceId = testRule.process().userTask().deploy().start();

		IList<HistoricProcessInstance> historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// assume
		assertThat(historicProcessInstances[0].RemovalTime).Null;

		testRule.updateHistoryTimeToLive(5, "process");

		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().superProcessInstanceId(processInstanceId);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byIds("notExistingId", processInstanceId).executeAsync());

		historicProcessInstances = historyService.createHistoricProcessInstanceQuery().list();

		// then
		assertThat(historicProcessInstances[0].RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldSetRemovalTimeForStandaloneDecision_ExistingAndNotExistingId()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldSetRemovalTimeForStandaloneDecision_ExistingAndNotExistingId()
	  {
		// given
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();

		HistoricDecisionInstance historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKeyIn("season").singleResult();

		// assume
		assertThat(historicDecisionInstance.RemovalTime).Null;

		testRule.updateHistoryTimeToLiveDmn(5, "season");

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("dish-decision");

		string id = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey("season").singleResult().Id;

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byIds("notExistingId", id).executeAsync());

		historicDecisionInstance = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKeyIn("season").singleResult();

		// then
		assertThat(historicDecisionInstance.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSetRemovalTimeForBatch_ExistingAndNotExistingId()
	  public virtual void shouldSetRemovalTimeForBatch_ExistingAndNotExistingId()
	  {
		// given
		string processInstanceId = testRule.process().serviceTask().deploy().start();
		Batch batchOne = historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "");

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// assume
		assertThat(historicBatch.RemovalTime).Null;

		ProcessEngineConfigurationImpl configuration = testRule.ProcessEngineConfiguration;
		configuration.BatchOperationHistoryTimeToLive = "P5D";
		configuration.initHistoryCleanup();

		HistoricBatchQuery query = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).batchId(batchOne.Id);

		// when
		testRule.syncExec(historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byIds("notExistingId", batchOne.Id).executeAsync());

		historicBatch = historyService.createHistoricBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).singleResult();

		// then
		assertThat(historicBatch.RemovalTime).isEqualTo(addDays(CURRENT_DATE, 5));

		// clear database
		managementService.deleteBatch(batchOne.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestException_SelectMultipleModes_ModeCleared()
	  public virtual void ThrowBadUserRequestException_SelectMultipleModes_ModeCleared()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyService.setRemovalTimeToHistoricProcessInstances();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.clearedRemovalTime();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestException_SelectMultipleModes_ModeAbsolute()
	  public virtual void ThrowBadUserRequestException_SelectMultipleModes_ModeAbsolute()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder builder = historyService.setRemovalTimeToHistoricProcessInstances();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.absoluteRemovalTime(DateTime.Now);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestExceptionForStandaloneDecision_SelectMultipleModes_ModeCleared()
	  public virtual void ThrowBadUserRequestExceptionForStandaloneDecision_SelectMultipleModes_ModeCleared()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyService.setRemovalTimeToHistoricDecisionInstances();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.clearedRemovalTime();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestExceptionForStandaloneDecision_SelectMultipleModes_ModeAbsolute()
	  public virtual void ThrowBadUserRequestExceptionForStandaloneDecision_SelectMultipleModes_ModeAbsolute()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder builder = historyService.setRemovalTimeToHistoricDecisionInstances();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.absoluteRemovalTime(DateTime.Now);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestExceptionForBatch_SelectMultipleModes_ModeCleared()
	  public virtual void ThrowBadUserRequestExceptionForBatch_SelectMultipleModes_ModeCleared()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyService.setRemovalTimeToHistoricBatches();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.clearedRemovalTime();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ThrowBadUserRequestExceptionForBatch_SelectMultipleModes_ModeAbsolute()
	  public virtual void ThrowBadUserRequestExceptionForBatch_SelectMultipleModes_ModeAbsolute()
	  {
		// given
		SetRemovalTimeSelectModeForHistoricBatchesBuilder builder = historyService.setRemovalTimeToHistoricBatches();
		builder.calculatedRemovalTime();

		// then
		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("The removal time modes are mutually exclusive: mode is not null");

		// when
		builder.absoluteRemovalTime(DateTime.Now);
	  }

	}

}