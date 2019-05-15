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
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchSetRemovalTimeRule = org.camunda.bpm.engine.test.api.history.removaltime.batch.helper.BatchSetRemovalTimeRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Java6Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(HISTORY_FULL)]
	public class BatchSetRemovalTimeUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchSetRemovalTimeUserOperationLogTest()
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
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule engineTestRule;
	  protected internal BatchSetRemovalTimeRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(engineTestRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void assignServices()
	  public virtual void assignServices()
	  {
		runtimeService = engineRule.RuntimeService;
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearAuth()
	  public virtual void clearAuth()
	  {
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearDatabase()
	  public virtual void clearDatabase()
	  {
		IList<Batch> batches = managementService.createBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_HISTORIC_PROCESS_INSTANCE_DELETION).list();

		if (batches.Count > 0)
		{
		  foreach (Batch batch in batches)
		  {
			managementService.deleteBatch(batch.Id, true);
		  }
		}

		string batchId = managementService.createBatchQuery().singleResult().Id;
		managementService.deleteBatch(batchId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances()
	  public virtual void shouldWriteUserOperationLogForProcessInstances()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().list();

		// then
		assertProperties(userOperationLogEntries, "mode", "removalTime", "hierarchical", "nrOfInstances", "async");
		assertOperationType(userOperationLogEntries, "SetRemovalTime");
		assertCategory(userOperationLogEntries, "Operator");
		assertEntityType(userOperationLogEntries, "ProcessInstance");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_ModeCalculatedRemovalTime()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_ModeCalculatedRemovalTime()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().calculatedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("CALCULATED_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_ModeAbsoluteRemovalTime()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_ModeAbsoluteRemovalTime()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(DateTime.Now).byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("ABSOLUTE_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_RemovalTime()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_RemovalTime()
	  {
		// given
		DateTime removalTime = DateTime.Now;

		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().absoluteRemovalTime(removalTime).byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(fromMillis(userOperationLogEntry.NewValue)).isEqualToIgnoringMillis(removalTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_RemovalTimeNull()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_RemovalTimeNull()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_NrOfInstances()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_NrOfInstances()
	  {
		// given
		testRule.process().serviceTask().deploy().start();
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("nrOfInstances").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_AsyncTrue()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_AsyncTrue()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("async").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("true");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_HierarchicalTrue()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_HierarchicalTrue()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(historicProcessInstanceQuery).hierarchical().executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("hierarchical").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("true");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForProcessInstances_HierarchicalFalse()
	  public virtual void shouldWriteUserOperationLogForProcessInstances_HierarchicalFalse()
	  {
		// given
		testRule.process().serviceTask().deploy().start();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricProcessInstanceQuery historicProcessInstanceQuery = historyService.createHistoricProcessInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricProcessInstances().clearedRemovalTime().byQuery(historicProcessInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("hierarchical").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("false");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().list();

		// then
		assertProperties(userOperationLogEntries, "mode", "removalTime", "hierarchical", "nrOfInstances", "async");
		assertOperationType(userOperationLogEntries, "SetRemovalTime");
		assertCategory(userOperationLogEntries, "Operator");
		assertEntityType(userOperationLogEntries, "DecisionInstance");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_ModeCalculatedRemovalTime()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_ModeCalculatedRemovalTime()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().calculatedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("CALCULATED_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_ModeAbsoluteRemovalTime()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_ModeAbsoluteRemovalTime()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(DateTime.Now).byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("ABSOLUTE_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_RemovalTime()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_RemovalTime()
	  {
		// given
		DateTime removalTime = DateTime.Now;

		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().absoluteRemovalTime(removalTime).byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(fromMillis(userOperationLogEntry.NewValue)).isEqualToIgnoringMillis(removalTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_RemovalTimeNull()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_RemovalTimeNull()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_NrOfInstances()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_NrOfInstances()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("nrOfInstances").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("3");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_AsyncTrue()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_AsyncTrue()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("async").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("true");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_HierarchicalTrue()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_HierarchicalTrue()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(historicDecisionInstanceQuery).hierarchical().executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("hierarchical").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("true");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" }) public void shouldWriteUserOperationLogForDecisionInstances_HierarchicalFalse()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml" })]
	  public virtual void shouldWriteUserOperationLogForDecisionInstances_HierarchicalFalse()
	  {
		// given
		evaluate();

		identityService.AuthenticatedUserId = "aUserId";

		HistoricDecisionInstanceQuery historicDecisionInstanceQuery = historyService.createHistoricDecisionInstanceQuery();

		// when
		historyService.setRemovalTimeToHistoricDecisionInstances().clearedRemovalTime().byQuery(historicDecisionInstanceQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("hierarchical").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("false");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches()
	  public virtual void shouldWriteUserOperationLogForBatches()
	  {
		// given
		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(historicBatchQuery).executeAsync();

		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().list();

		// then
		assertProperties(userOperationLogEntries, "mode", "removalTime", "nrOfInstances", "async");
		assertOperationType(userOperationLogEntries, "SetRemovalTime");
		assertEntityType(userOperationLogEntries, "Batch");
		assertCategory(userOperationLogEntries, "Operator");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_ModeCalculatedRemovalTime()
	  public virtual void shouldWriteUserOperationLogForBatches_ModeCalculatedRemovalTime()
	  {
		// given
		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().calculatedRemovalTime().byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("CALCULATED_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_ModeAbsoluteRemovalTime()
	  public virtual void shouldWriteUserOperationLogForBatches_ModeAbsoluteRemovalTime()
	  {
		// given
		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(DateTime.Now).byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("mode").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("ABSOLUTE_REMOVAL_TIME");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_RemovalTime()
	  public virtual void shouldWriteUserOperationLogForBatches_RemovalTime()
	  {
		// given
		DateTime removalTime = DateTime.Now;

		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().absoluteRemovalTime(removalTime).byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(fromMillis(userOperationLogEntry.NewValue)).isEqualToIgnoringMillis(removalTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_RemovalTimeNull()
	  public virtual void shouldWriteUserOperationLogForBatches_RemovalTimeNull()
	  {
		// given
		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().clearedRemovalTime().byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("removalTime").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_NrOfInstances()
	  public virtual void shouldWriteUserOperationLogForBatches_NrOfInstances()
	  {
		// given
		createBatch(2);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().clearedRemovalTime().byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("nrOfInstances").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteUserOperationLogForBatches_AsyncTrue()
	  public virtual void shouldWriteUserOperationLogForBatches_AsyncTrue()
	  {
		// given
		createBatch(1);

		identityService.AuthenticatedUserId = "aUserId";

		HistoricBatchQuery historicBatchQuery = historyService.createHistoricBatchQuery();

		// when
		historyService.setRemovalTimeToHistoricBatches().clearedRemovalTime().byQuery(historicBatchQuery).executeAsync();

		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().property("async").singleResult();

		// then
		assertThat(userOperationLogEntry.OrgValue).Null;
		assertThat(userOperationLogEntry.NewValue).isEqualTo("true");
	  }

	  // helper ////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual void assertProperties(IList<UserOperationLogEntry> userOperationLogEntries, params string[] expectedProperties)
	  {
		assertThat(userOperationLogEntries.Count).isEqualTo(expectedProperties.Length);

		assertThat(userOperationLogEntries).extracting("property").containsExactlyInAnyOrder(expectedProperties);
	  }

	  protected internal virtual void assertEntityType(IList<UserOperationLogEntry> userOperationLogEntries, string entityType)
	  {
		foreach (UserOperationLogEntry userOperationLogEntry in userOperationLogEntries)
		{
		  assertThat(userOperationLogEntry.EntityType).isEqualTo(entityType);
		}
	  }

	  protected internal virtual void assertOperationType(IList<UserOperationLogEntry> userOperationLogEntries, string operationType)
	  {
		foreach (UserOperationLogEntry userOperationLogEntry in userOperationLogEntries)
		{
		  assertThat(userOperationLogEntry.OperationType).isEqualTo(operationType);
		}
	  }

	  protected internal virtual void assertCategory(IList<UserOperationLogEntry> userOperationLogEntries, string category)
	  {
		foreach (UserOperationLogEntry userOperationLogEntry in userOperationLogEntries)
		{
		  assertThat(userOperationLogEntry.Category).isEqualTo(category);
		}
	  }

	  protected internal virtual DateTime fromMillis(string milliseconds)
	  {
		DateTime calendar = new DateTime();
		calendar.TimeInMillis = Convert.ToInt64(milliseconds);

		return calendar;
	  }

	  protected internal virtual void evaluate()
	  {
		decisionService.evaluateDecisionByKey("dish-decision").variables(Variables.createVariables().putValue("temperature", 32).putValue("dayType", "Weekend")).evaluate();
	  }

	  protected internal virtual void createBatch(int times)
	  {
		for (int i = 0; i < times; i++)
		{
		  string processInstanceId = testRule.process().serviceTask().deploy().start();
		  historyService.deleteHistoricProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");
		}
	  }

	}

}