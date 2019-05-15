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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BatchHistoricDecisionInstanceDeletionUserOperationTest
	{
		private bool InstanceFieldsInitialized = false;

		public BatchHistoricDecisionInstanceDeletionUserOperationTest()
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


	  protected internal static string DECISION = "decision";

	  public const string USER_ID = "userId";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal DecisionService decisionService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal IdentityService identityService;

	  protected internal IList<string> decisionInstanceIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		historyService = engineRule.HistoryService;
		decisionService = engineRule.DecisionService;
		managementService = engineRule.ManagementService;
		identityService = engineRule.IdentityService;
		decisionInstanceIds = new List<string>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void evaluateDecisionInstances()
	  public virtual void evaluateDecisionInstances()
	  {
		testRule.deploy("org/camunda/bpm/engine/test/api/dmn/Example.dmn");

		VariableMap variables = Variables.createVariables().putValue("status", "silver").putValue("sum", 723);

		for (int i = 0; i < 10; i++)
		{
		  decisionService.evaluateDecisionByKey(DECISION).variables(variables).evaluate();
		}

		IList<HistoricDecisionInstance> decisionInstances = historyService.createHistoricDecisionInstanceQuery().list();
		foreach (HistoricDecisionInstance decisionInstance in decisionInstances)
		{
		  decisionInstanceIds.Add(decisionInstance.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		foreach (Batch batch in managementService.createBatchQuery().list())
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		// remove history of completed batches
		foreach (HistoricBatch historicBatch in historyService.createHistoricBatchQuery().list())
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearAuthentication()
	  public virtual void clearAuthentication()
	  {
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreationByIds()
	  public virtual void testCreationByIds()
	  {
		// when
		identityService.AuthenticatedUserId = USER_ID;
		historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, "a-delete-reason");
		identityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = engineRule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry asyncEntry = entries["async"];
		assertNotNull(asyncEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, asyncEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, asyncEntry.OperationType);
		assertNull(asyncEntry.ProcessDefinitionId);
		assertNull(asyncEntry.ProcessDefinitionKey);
		assertNull(asyncEntry.ProcessInstanceId);
		assertNull(asyncEntry.OrgValue);
		assertEquals("true", asyncEntry.NewValue);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		assertNotNull(numInstancesEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, numInstancesEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, numInstancesEntry.OperationType);
		assertNull(numInstancesEntry.ProcessDefinitionId);
		assertNull(numInstancesEntry.ProcessDefinitionKey);
		assertNull(numInstancesEntry.ProcessInstanceId);
		assertNull(numInstancesEntry.OrgValue);
		assertEquals("10", numInstancesEntry.NewValue);

		UserOperationLogEntry deleteReasonEntry = entries["deleteReason"];
		assertNotNull(deleteReasonEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, deleteReasonEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, deleteReasonEntry.OperationType);
		assertNull(deleteReasonEntry.ProcessDefinitionId);
		assertNull(deleteReasonEntry.ProcessDefinitionKey);
		assertNull(deleteReasonEntry.ProcessInstanceId);
		assertNull(deleteReasonEntry.OrgValue);
		assertEquals("a-delete-reason", deleteReasonEntry.NewValue);

		assertEquals(numInstancesEntry.OperationId, asyncEntry.OperationId);
		assertEquals(asyncEntry.OperationId, deleteReasonEntry.OperationId);
		assertEquals(numInstancesEntry.OperationId, deleteReasonEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreationByQuery()
	  public virtual void testCreationByQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		historyService.deleteHistoricDecisionInstancesAsync(query, "a-delete-reason");
		identityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = engineRule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry asyncEntry = entries["async"];
		assertNotNull(asyncEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, asyncEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, asyncEntry.OperationType);
		assertNull(asyncEntry.ProcessDefinitionId);
		assertNull(asyncEntry.ProcessDefinitionKey);
		assertNull(asyncEntry.ProcessInstanceId);
		assertNull(asyncEntry.OrgValue);
		assertEquals("true", asyncEntry.NewValue);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		assertNotNull(numInstancesEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, numInstancesEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, numInstancesEntry.OperationType);
		assertNull(numInstancesEntry.ProcessDefinitionId);
		assertNull(numInstancesEntry.ProcessDefinitionKey);
		assertNull(numInstancesEntry.ProcessInstanceId);
		assertNull(numInstancesEntry.OrgValue);
		assertEquals("10", numInstancesEntry.NewValue);

		UserOperationLogEntry deleteReasonEntry = entries["deleteReason"];
		assertNotNull(deleteReasonEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, deleteReasonEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, deleteReasonEntry.OperationType);
		assertNull(deleteReasonEntry.ProcessDefinitionId);
		assertNull(deleteReasonEntry.ProcessDefinitionKey);
		assertNull(deleteReasonEntry.ProcessInstanceId);
		assertNull(deleteReasonEntry.OrgValue);
		assertEquals("a-delete-reason", deleteReasonEntry.NewValue);

		assertEquals(deleteReasonEntry.OperationId, asyncEntry.OperationId);
		assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
		assertEquals(numInstancesEntry.OperationId, deleteReasonEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreationByIdsAndQuery()
	  public virtual void testCreationByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, "a-delete-reason");
		identityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = engineRule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry asyncEntry = entries["async"];
		assertNotNull(asyncEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, asyncEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, asyncEntry.OperationType);
		assertNull(asyncEntry.ProcessDefinitionId);
		assertNull(asyncEntry.ProcessDefinitionKey);
		assertNull(asyncEntry.ProcessInstanceId);
		assertNull(asyncEntry.OrgValue);
		assertEquals("true", asyncEntry.NewValue);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		assertNotNull(numInstancesEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, numInstancesEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, numInstancesEntry.OperationType);
		assertNull(numInstancesEntry.ProcessDefinitionId);
		assertNull(numInstancesEntry.ProcessDefinitionKey);
		assertNull(numInstancesEntry.ProcessInstanceId);
		assertNull(numInstancesEntry.OrgValue);
		assertEquals("10", numInstancesEntry.NewValue);

		UserOperationLogEntry deleteReasonEntry = entries["deleteReason"];
		assertNotNull(deleteReasonEntry);
		assertEquals(EntityTypes.DECISION_INSTANCE, deleteReasonEntry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, deleteReasonEntry.OperationType);
		assertNull(deleteReasonEntry.ProcessDefinitionId);
		assertNull(deleteReasonEntry.ProcessDefinitionKey);
		assertNull(deleteReasonEntry.ProcessInstanceId);
		assertNull(deleteReasonEntry.OrgValue);
		assertEquals("a-delete-reason", deleteReasonEntry.NewValue);

		assertEquals(deleteReasonEntry.OperationId, asyncEntry.OperationId);
		assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
		assertEquals(numInstancesEntry.OperationId, deleteReasonEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnSyncBatchJobExecution()
	  public virtual void testNoCreationOnSyncBatchJobExecution()
	  {
		// given
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		// when
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
		executeJobs(batch);
		engineRule.IdentityService.clearAuthentication();

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.DECISION_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnSyncBatchJobExecutionByIds()
	  public virtual void testNoCreationOnSyncBatchJobExecutionByIds()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(query, null);

		// when
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
		executeJobs(batch);
		engineRule.IdentityService.clearAuthentication();

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.DECISION_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnSyncBatchJobExecutionByIdsAndQuery()
	  public virtual void testNoCreationOnSyncBatchJobExecutionByIdsAndQuery()
	  {
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		Batch batch = historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		// when
		engineRule.IdentityService.AuthenticatedUserId = USER_ID;
		executeJobs(batch);
		engineRule.IdentityService.clearAuthentication();

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.DECISION_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnJobExecutorBatchJobExecutionByIds()
	  public virtual void testNoCreationOnJobExecutorBatchJobExecutionByIds()
	  {
		// given
		// given
		historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, null);

		// when
		testRule.waitForJobExecutorToProcessAllJobs(5000L);

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnJobExecutorBatchJobExecutionByQuery()
	  public virtual void testNoCreationOnJobExecutorBatchJobExecutionByQuery()
	  {
		// given
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		historyService.deleteHistoricDecisionInstancesAsync(query, null);

		// when
		testRule.waitForJobExecutorToProcessAllJobs(5000L);

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnJobExecutorBatchJobExecutionByIdsAndQuery()
	  public virtual void testNoCreationOnJobExecutorBatchJobExecutionByIdsAndQuery()
	  {
		// given
		// given
		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionDefinitionKey(DECISION);
		historyService.deleteHistoricDecisionInstancesAsync(decisionInstanceIds, query, null);

		// when
		testRule.waitForJobExecutorToProcessAllJobs(5000L);

		// then
		assertEquals(0, engineRule.HistoryService.createUserOperationLogQuery().count());
	  }

	  protected internal virtual IDictionary<string, UserOperationLogEntry> asMap(IList<UserOperationLogEntry> logEntries)
	  {
		IDictionary<string, UserOperationLogEntry> map = new Dictionary<string, UserOperationLogEntry>();

		foreach (UserOperationLogEntry entry in logEntries)
		{

		  UserOperationLogEntry previousValue = map[entry.Property] = entry;
		  if (previousValue != null)
		  {
			fail("expected only entry for every property");
		  }
		}

		return map;
	  }

	  protected internal virtual void executeJobs(Batch batch)
	  {
		Job job = managementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult();

		// seed job
		managementService.executeJob(job.Id);

		foreach (Job pending in managementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list())
		{
		  managementService.executeJob(pending.Id);
		}
	  }

	}

}