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
namespace org.camunda.bpm.engine.test.api.runtime
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class RestartProcessInstanceUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public RestartProcessInstanceUserOperationLogTest()
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
			helper = new BatchRestartHelper(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchRestartHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal BpmnModelInstance instance;
	  protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockUtil.CurrentTime = START_DATE;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createBpmnModelInstance()
	  public virtual void createBpmnModelInstance()
	  {
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").userTask("user1").sequenceFlowId("seq").userTask("user2").endEvent("end").done();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLogCreationAsync()
	  public virtual void testLogCreationAsync()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		rule.IdentityService.AuthenticatedUserId = "userId";

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process1");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process1");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startAfterActivity("user1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();
		rule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().operationType("RestartProcessInstance").list();
		Assert.assertEquals(2, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);


		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals("ProcessInstance", asyncEntry.EntityType);
		Assert.assertEquals("RestartProcessInstance", asyncEntry.OperationType);
		Assert.assertEquals(processDefinition.Id, asyncEntry.ProcessDefinitionId);
		Assert.assertEquals(processDefinition.Key, asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("true", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstancesEntry);
		Assert.assertEquals("ProcessInstance", numInstancesEntry.EntityType);
		Assert.assertEquals("RestartProcessInstance", numInstancesEntry.OperationType);
		Assert.assertEquals(processDefinition.Id, numInstancesEntry.ProcessDefinitionId);
		Assert.assertEquals(processDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstancesEntry.ProcessInstanceId);
		Assert.assertNull(numInstancesEntry.OrgValue);
		Assert.assertEquals("2", numInstancesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstancesEntry.Category);

		Assert.assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLogCreationSync()
	  public virtual void testLogCreationSync()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		rule.IdentityService.AuthenticatedUserId = "userId";

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process1");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process1");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startAfterActivity("user1").processInstanceIds(processInstance1.Id, processInstance2.Id).execute();
		rule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().operationType("RestartProcessInstance").list();
		Assert.assertEquals(2, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);


		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals("ProcessInstance", asyncEntry.EntityType);
		Assert.assertEquals("RestartProcessInstance", asyncEntry.OperationType);
		Assert.assertEquals(processDefinition.Id, asyncEntry.ProcessDefinitionId);
		Assert.assertEquals(processDefinition.Key, asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("false", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstancesEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstancesEntry);
		Assert.assertEquals("ProcessInstance", numInstancesEntry.EntityType);
		Assert.assertEquals("RestartProcessInstance", numInstancesEntry.OperationType);
		Assert.assertEquals(processDefinition.Id, numInstancesEntry.ProcessDefinitionId);
		Assert.assertEquals(processDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstancesEntry.ProcessInstanceId);
		Assert.assertNull(numInstancesEntry.OrgValue);
		Assert.assertEquals("2", numInstancesEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstancesEntry.Category);

		Assert.assertEquals(asyncEntry.OperationId, numInstancesEntry.OperationId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnSyncBatchJobExecution()
	  public virtual void testNoCreationOnSyncBatchJobExecution()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("user1").processInstanceIds(processInstance.Id).executeAsync();

		helper.executeSeedJob(batch);

		// when
		rule.IdentityService.AuthenticatedUserId = "userId";
		helper.executeJobs(batch);
		rule.IdentityService.clearAuthentication();

		// then
		Assert.assertEquals(0, rule.HistoryService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoCreationOnJobExecutorBatchJobExecution()
	  public virtual void testNoCreationOnJobExecutorBatchJobExecution()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");
		runtimeService.restartProcessInstances(processDefinition.Id).startAfterActivity("user1").processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		// when
		testRule.waitForJobExecutorToProcessAllJobs(5000L);

		// then
		Assert.assertEquals(0, rule.HistoryService.createUserOperationLogQuery().count());
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