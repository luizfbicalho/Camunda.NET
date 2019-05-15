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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class ExternalTaskUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public ExternalTaskUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  private static string PROCESS_DEFINITION_KEY = "oneExternalTaskProcess";
	  private static string PROCESS_DEFINITION_KEY_2 = "twoExternalTaskWithPriorityProcess";

	  protected internal RuntimeService runtimeService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		externalTaskService = rule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeAllRunningAndHistoricBatches()
	  public virtual void removeAllRunningAndHistoricBatches()
	  {
		HistoryService historyService = rule.HistoryService;
		ManagementService managementService = rule.ManagementService;
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
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testSetRetriesLogCreationForOneExternalTaskId()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesLogCreationForOneExternalTaskId()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
		rule.IdentityService.AuthenticatedUserId = "userId";

		// when
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();
		externalTaskService.setRetries(externalTask.Id, 5);
		rule.IdentityService.clearAuthentication();
		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(1, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry retriesEntry = entries["retries"];
		Assert.assertNotNull(retriesEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, retriesEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", retriesEntry.OperationType);
		Assert.assertEquals(externalTask.Id, retriesEntry.ExternalTaskId);
		Assert.assertEquals(externalTask.ProcessInstanceId, retriesEntry.ProcessInstanceId);
		Assert.assertEquals(externalTask.ProcessDefinitionId, retriesEntry.ProcessDefinitionId);
		Assert.assertEquals(externalTask.ProcessDefinitionKey, retriesEntry.ProcessDefinitionKey);
		Assert.assertNull(retriesEntry.OrgValue);
		Assert.assertEquals("5", retriesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, retriesEntry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testSetRetriesLogCreationSync()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesLogCreationSync()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		IList<ExternalTask> list = externalTaskService.createExternalTaskQuery().list();
		IList<string> externalTaskIds = new List<string>();

		foreach (ExternalTask task in list)
		{
		  externalTaskIds.Add(task.Id);
		}

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		externalTaskService.setRetries(externalTaskIds, 5);
		rule.IdentityService.clearAuthentication();
		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, asyncEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", asyncEntry.OperationType);
		Assert.assertNull(asyncEntry.ExternalTaskId);
		Assert.assertNull(asyncEntry.ProcessDefinitionId);
		Assert.assertNull(asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("false", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstancesEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, numInstancesEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", numInstancesEntry.OperationType);
		Assert.assertNull(numInstancesEntry.ExternalTaskId);
		Assert.assertNull(numInstancesEntry.ProcessDefinitionId);
		Assert.assertNull(numInstancesEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstancesEntry.ProcessInstanceId);
		Assert.assertNull(numInstancesEntry.OrgValue);
		Assert.assertEquals("2", numInstancesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstancesEntry.Category);

		UserOperationLogEntry retriesEntry = entries["retries"];
		Assert.assertNotNull(retriesEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, retriesEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", retriesEntry.OperationType);
		Assert.assertNull(retriesEntry.ExternalTaskId);
		Assert.assertNull(retriesEntry.ProcessDefinitionId);
		Assert.assertNull(retriesEntry.ProcessDefinitionKey);
		Assert.assertNull(retriesEntry.ProcessInstanceId);
		Assert.assertNull(retriesEntry.OrgValue);
		Assert.assertEquals("5", retriesEntry.NewValue);
		Assert.assertEquals(asyncEntry.OperationId, retriesEntry.OperationId);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, retriesEntry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testSetRetriesLogCreationAsync()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testSetRetriesLogCreationAsync()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		externalTaskService.setRetriesAsync(null, externalTaskService.createExternalTaskQuery(), 5);
		rule.IdentityService.clearAuthentication();
		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, asyncEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", asyncEntry.OperationType);
		Assert.assertNull(asyncEntry.ExternalTaskId);
		Assert.assertNull(asyncEntry.ProcessDefinitionId);
		Assert.assertNull(asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("true", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstancesEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, numInstancesEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", numInstancesEntry.OperationType);
		Assert.assertNull(numInstancesEntry.ExternalTaskId);
		Assert.assertNull(numInstancesEntry.ProcessDefinitionId);
		Assert.assertNull(numInstancesEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstancesEntry.ProcessInstanceId);
		Assert.assertNull(numInstancesEntry.OrgValue);
		Assert.assertEquals("2", numInstancesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstancesEntry.Category);

		UserOperationLogEntry retriesEntry = entries["retries"];
		Assert.assertNotNull(retriesEntry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, retriesEntry.EntityType);
		Assert.assertEquals("SetExternalTaskRetries", retriesEntry.OperationType);
		Assert.assertNull(retriesEntry.ExternalTaskId);
		Assert.assertNull(retriesEntry.ProcessDefinitionId);
		Assert.assertNull(retriesEntry.ProcessDefinitionKey);
		Assert.assertNull(retriesEntry.ProcessInstanceId);
		Assert.assertNull(retriesEntry.OrgValue);
		Assert.assertEquals("5", retriesEntry.NewValue);
		Assert.assertEquals(asyncEntry.OperationId, retriesEntry.OperationId);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, retriesEntry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml") public void testSetPriorityLogCreation()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testSetPriorityLogCreation()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_2, Collections.singletonMap<string, object>("priority", 14));
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().priorityHigherThanOrEquals(1).singleResult();

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		externalTaskService.setPriority(externalTask.Id, 78L);
		rule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(1, opLogEntries.Count);

		UserOperationLogEntry entry = opLogEntries[0];
		Assert.assertNotNull(entry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, entry.EntityType);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, entry.OperationType);
		Assert.assertEquals(externalTask.Id, entry.ExternalTaskId);
		Assert.assertEquals(externalTask.ProcessInstanceId, entry.ProcessInstanceId);
		Assert.assertEquals(externalTask.ProcessDefinitionId, entry.ProcessDefinitionId);
		Assert.assertEquals(externalTask.ProcessDefinitionKey, entry.ProcessDefinitionKey);
		Assert.assertEquals("priority", entry.Property);
		Assert.assertEquals("14", entry.OrgValue);
		Assert.assertEquals("78", entry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testUnlockLogCreation()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testUnlockLogCreation()
	  {
		// given
		runtimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY);
		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();
		externalTaskService.fetchAndLock(1, "aWorker").topic(externalTask.TopicName, 3000L).execute();

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		externalTaskService.unlock(externalTask.Id);
		rule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(1, opLogEntries.Count);

		UserOperationLogEntry entry = opLogEntries[0];
		Assert.assertNotNull(entry);
		Assert.assertEquals(EntityTypes.EXTERNAL_TASK, entry.EntityType);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UNLOCK, entry.OperationType);
		Assert.assertEquals(externalTask.Id, entry.ExternalTaskId);
		Assert.assertEquals(externalTask.ProcessInstanceId, entry.ProcessInstanceId);
		Assert.assertEquals(externalTask.ProcessDefinitionId, entry.ProcessDefinitionId);
		Assert.assertEquals(externalTask.ProcessDefinitionKey, entry.ProcessDefinitionKey);
		Assert.assertNull(entry.Property);
		Assert.assertNull(entry.OrgValue);
		Assert.assertNull(entry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
	  }

	  protected internal virtual IDictionary<string, UserOperationLogEntry> asMap(IList<UserOperationLogEntry> logEntries)
	  {
		IDictionary<string, UserOperationLogEntry> map = new Dictionary<string, UserOperationLogEntry>();

		foreach (UserOperationLogEntry entry in logEntries)
		{

		  UserOperationLogEntry previousValue = map[entry.Property] = entry;
		  if (previousValue != null)
		  {
			Assert.fail("expected only entry for every property");
		  }
		}

		return map;
	  }
	}

}