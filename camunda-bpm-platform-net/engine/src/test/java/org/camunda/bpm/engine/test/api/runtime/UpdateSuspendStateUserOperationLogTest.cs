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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class UpdateSuspendStateUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public UpdateSuspendStateUserOperationLogTest()
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
			helper = new BatchSuspensionHelper(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  // do an update here
	  protected internal BatchSuspensionHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		historyService = rule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testLogCreation()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testLogCreation()
	  {


		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");
		rule.IdentityService.AuthenticatedUserId = "userId";

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspendAsync();
		rule.IdentityService.clearAuthentication();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		assertEquals(2, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);



		UserOperationLogEntry asyncEntry = entries["async"];
		assertNotNull(asyncEntry);
		assertEquals("ProcessInstance", asyncEntry.EntityType);
		assertEquals("SuspendJob", asyncEntry.OperationType);
		assertNull(asyncEntry.ProcessInstanceId);
		assertNull(asyncEntry.OrgValue);
		assertEquals("true", asyncEntry.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		assertNotNull(numInstancesEntry);
		assertEquals("ProcessInstance", numInstancesEntry.EntityType);
		assertEquals("SuspendJob", numInstancesEntry.OperationType);
		assertNull(numInstancesEntry.ProcessInstanceId);
		assertNull(numInstancesEntry.ProcessDefinitionKey);
		assertNull(numInstancesEntry.ProcessDefinitionId);
		assertNull(numInstancesEntry.OrgValue);
		assertEquals("2", numInstancesEntry.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void testNoCreationOnSyncBatchJobExecution()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void testNoCreationOnSyncBatchJobExecution()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");


		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspendAsync();
		helper.executeSeedJob(suspendprocess);

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		helper.executeJobs(suspendprocess);
		rule.IdentityService.clearAuthentication();

		// then
		assertEquals(0, rule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).count());
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

	}

}