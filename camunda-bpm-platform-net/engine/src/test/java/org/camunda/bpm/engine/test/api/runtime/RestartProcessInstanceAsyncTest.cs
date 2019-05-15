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
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using SetVariableExecutionListenerImpl = org.camunda.bpm.engine.test.api.runtime.RestartProcessInstanceSyncTest.SetVariableExecutionListenerImpl;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using IncrementCounterListener = org.camunda.bpm.engine.test.api.runtime.util.IncrementCounterListener;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class RestartProcessInstanceAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public RestartProcessInstanceAsyncTest()
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
			helper = new BatchRestartHelper(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchRestartHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal TenantIdProvider defaultTenantIdProvider;
	  protected internal bool defaultEnsureJobDueDateSet;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;

		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		defaultTenantIdProvider = processEngineConfiguration.TenantIdProvider;
		defaultEnsureJobDueDateSet = processEngineConfiguration.EnsureJobDueDateNotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void reset()
	  public virtual void reset()
	  {
		helper.removeAllRunningAndHistoricBatches();
		processEngineConfiguration.TenantIdProvider = defaultTenantIdProvider;
		processEngineConfiguration.EnsureJobDueDateNotNull = defaultEnsureJobDueDateSet;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createBatchRestart()
	  public virtual void createBatchRestart()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		IList<string> processInstanceIds = Arrays.asList(processInstance1.Id, processInstance2.Id);

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startAfterActivity("userTask2").processInstanceIds(processInstanceIds).executeAsync();

		// then
		assertBatchCreated(batch, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNullProcessDefinitionId()
	  public virtual void restartProcessInstanceWithNullProcessDefinitionId()
	  {
		try
		{
		  runtimeService.restartProcessInstances(null).executeAsync();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("processDefinitionId is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithoutInstructions()
	  public virtual void restartProcessInstanceWithoutInstructions()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		try
		{
		  Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).processInstanceIds(processInstance.Id).executeAsync();
		  helper.completeBatch(batch);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("instructions is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithoutProcessInstanceIds()
	  public virtual void restartProcessInstanceWithoutProcessInstanceIds()
	  {
		try
		{
		  runtimeService.restartProcessInstances("foo").startAfterActivity("bar").executeAsync();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("processInstanceIds is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNullProcessInstanceId()
	  public virtual void restartProcessInstanceWithNullProcessInstanceId()
	  {
		try
		{
		  runtimeService.restartProcessInstances("foo").startAfterActivity("bar").processInstanceIds((string) null).executeAsync();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("processInstanceIds contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartNotExistingProcessInstance()
	  public virtual void restartNotExistingProcessInstance()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("bar").processInstanceIds("aaa").executeAsync();
		helper.executeSeedJob(batch);
		try
		{
		  helper.executeJobs(batch);
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("Historic process instance cannot be found"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstance()
	  public virtual void shouldRestartProcessInstance()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		Task task1 = taskService.createTaskQuery().processInstanceId(processInstance1.Id).active().singleResult();
		Task task2 = taskService.createTaskQuery().processInstanceId(processInstance2.Id).active().singleResult();

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id,processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().active().list();
		ProcessInstance restartedProcessInstance = restartedProcessInstances[0];
		Task restartedTask = engineRule.TaskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(task1.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

		restartedProcessInstance = restartedProcessInstances[1];
		restartedTask = engineRule.TaskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(task2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithParallelGateway()
	  public virtual void shouldRestartProcessInstanceWithParallelGateway()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").startBeforeActivity("userTask2").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().active().list();
		foreach (ProcessInstance restartedProcessInstance in restartedProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		  assertNotNull(updatedTree);
		  assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("userTask1").activity("userTask2").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithSubProcess()
	  public virtual void shouldRestartProcessInstanceWithSubProcess()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("subProcess").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().active().list();
		foreach (ProcessInstance restartedProcessInstance in restartedProcessInstances)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		  assertNotNull(updatedTree);
		  assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).beginScope("subProcess").activity("userTask").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithInitialVariables()
	  public virtual void shouldRestartProcessInstanceWithInitialVariables()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().userTask("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableExecutionListenerImpl).FullName).userTask("userTask2").endEvent().done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		 // initial variables
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("var", "bar"));
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("var", "bar"));

		// variables update
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionId(processDefinition.Id).active().list();
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// delete process instances
		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").initialSetOfVariables().processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		VariableInstance variableInstance1 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstances[0].Id).singleResult();
		VariableInstance variableInstance2 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstances[1].Id).singleResult();

		assertEquals(variableInstance1.ExecutionId, restartedProcessInstances[0].Id);
		assertEquals(variableInstance2.ExecutionId, restartedProcessInstances[1].Id);
		assertEquals("var", variableInstance1.Name);
		assertEquals("bar", variableInstance1.Value);
		assertEquals("var", variableInstance2.Name);
		assertEquals("bar", variableInstance2.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithVariables()
	  public virtual void shouldRestartProcessInstanceWithVariables()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().userTask("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableExecutionListenerImpl).FullName).userTask("userTask2").endEvent().done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		// variables are set at the beginning
		runtimeService.setVariable(processInstance1.Id, "var", "bar");
		runtimeService.setVariable(processInstance2.Id, "var", "bb");

		// variables are changed
		IList<Task> tasks = taskService.createTaskQuery().processDefinitionId(processDefinition.Id).active().list();
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[1].Id);

		// process instances are deleted
		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().active().list();
		ProcessInstance restartedProcessInstance = restartedProcessInstances[0];
		VariableInstance variableInstance1 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).singleResult();
		assertEquals(variableInstance1.ExecutionId, restartedProcessInstance.Id);

		restartedProcessInstance = restartedProcessInstances[1];
		VariableInstance variableInstance2 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).singleResult();
		assertEquals(variableInstance2.ExecutionId, restartedProcessInstance.Id);
		assertTrue(variableInstance1.Name.Equals(variableInstance2.Name));
		assertEquals("var", variableInstance1.Name);
		assertTrue(variableInstance1.Value.Equals(variableInstance2.Value));
		assertEquals("foo", variableInstance2.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetLocalVariables()
	  public virtual void shouldNotSetLocalVariables()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		Execution subProcess1 = runtimeService.createExecutionQuery().processInstanceId(processInstance1.Id).activityId("userTask").singleResult();
		Execution subProcess2 = runtimeService.createExecutionQuery().processInstanceId(processInstance2.Id).activityId("userTask").singleResult();
		runtimeService.setVariableLocal(subProcess1.Id, "local", "foo");
		runtimeService.setVariableLocal(subProcess2.Id, "local", "foo");

		runtimeService.setVariable(processInstance1.Id, "var", "bar");
		runtimeService.setVariable(processInstance2.Id, "var", "bar");


		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");


		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);
		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		IList<VariableInstance> variables1 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstances[0].Id).list();
		assertEquals(1, variables1.Count);
		assertEquals("var", variables1[0].Name);
		assertEquals("bar", variables1[0].Value);
		IList<VariableInstance> variables2 = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstances[1].Id).list();
		assertEquals(1, variables1.Count);
		assertEquals("var", variables2[0].Name);
		assertEquals("bar", variables2[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
	  public virtual void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		Task task1 = taskService.createTaskQuery().processInstanceId(processInstance1.Id).active().singleResult();

		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");
		Task task2 = taskService.createTaskQuery().processInstanceId(processInstance2.Id).active().singleResult();

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		HistoricProcessInstanceQuery historicProcessInstanceQuery = engineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionId(processDefinition.Id);

		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").historicProcessInstanceQuery(historicProcessInstanceQuery).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().active().list();
		ProcessInstance restartedProcessInstance = restartedProcessInstances[0];
		Task restartedTask = taskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(task1.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

		restartedProcessInstance = restartedProcessInstances[1];
		restartedTask = taskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(task2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
	  {
		// given
		int processInstanceCount = 2;
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		HistoricProcessInstanceQuery processInstanceQuery = engineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionId(processDefinition.Id);

		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).historicProcessInstanceQuery(processInstanceQuery).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).list();
		assertEquals(restartedProcessInstances.Count, processInstanceCount);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobPollingForCompletion()
	  public virtual void testMonitorJobPollingForCompletion()
	  {
		processEngineConfiguration.EnsureJobDueDateNotNull = false;

		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		// when the seed job creates the monitor job
		DateTime createDate = ClockTestUtil.setClockToDateWithoutMilliseconds();
		helper.executeSeedJob(batch);

		// then the monitor job has a no due date set
		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
		assertNull(monitorJob.Duedate);

		// when the monitor job is executed
		helper.executeMonitorJob(batch);

		// then the monitor job has a due date of the default batch poll time
		monitorJob = helper.getMonitorJob(batch);
		DateTime dueDate = helper.addSeconds(createDate, 30);
		assertEquals(dueDate, monitorJob.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobPollingForCompletionDueDateSet()
	  public virtual void testMonitorJobPollingForCompletionDueDateSet()
	  {
		DateTime testDate = new DateTime(1457326800000L);
		ClockUtil.CurrentTime = testDate;
		processEngineConfiguration.EnsureJobDueDateNotNull = true;

		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		// when the seed job creates the monitor job
		DateTime createDate = testDate;
		helper.executeSeedJob(batch);

		// then the monitor job has the create date as due date set
		Job monitorJob = helper.getMonitorJob(batch);
		assertNotNull(monitorJob);
		assertEquals(testDate, monitorJob.Duedate);

		// when the monitor job is executed
		helper.executeMonitorJob(batch);

		// then the monitor job has a due date of the default batch poll time
		monitorJob = helper.getMonitorJob(batch);
		DateTime dueDate = helper.addSeconds(createDate, 30);
		assertEquals(dueDate, monitorJob.Duedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMonitorJobRemovesBatchAfterCompletion()
	  public virtual void testMonitorJobRemovesBatchAfterCompletion()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);
		helper.executeJobs(batch);

		helper.executeMonitorJob(batch);

		// then the batch was completed and removed
		assertEquals(0, engineRule.ManagementService.createBatchQuery().count());

		// and the seed jobs was removed
		assertEquals(0, engineRule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithCascade()
	  public virtual void testBatchDeletionWithCascade()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);

		engineRule.ManagementService.deleteBatch(batch.Id, true);

		// then the batch was deleted
		assertEquals(0, engineRule.ManagementService.createBatchQuery().count());

		// and the seed and execution job definition were deleted
		assertEquals(0, engineRule.ManagementService.createJobDefinitionQuery().count());

		// and the seed job and execution jobs were deleted
		assertEquals(0, engineRule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchDeletionWithoutCascade()
	  public virtual void testBatchDeletionWithoutCascade()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);

		engineRule.ManagementService.deleteBatch(batch.Id, false);

		// then the batch was deleted
		assertEquals(0, engineRule.ManagementService.createBatchQuery().count());

		// and the seed and execution job definition were deleted
		assertEquals(0, engineRule.ManagementService.createJobDefinitionQuery().count());

		// and the seed job and execution jobs were deleted
		assertEquals(0, engineRule.ManagementService.createJobQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedSeedJobDeletionWithCascade()
	  public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		// create incident
		Job seedJob = helper.getSeedJob(batch);
		engineRule.ManagementService.setJobRetries(seedJob.Id, 0);

		engineRule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = engineRule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedExecutionJobDeletionWithCascade()
	  public virtual void testBatchWithFailedExecutionJobDeletionWithCascade()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);

		// create incidents
		IList<Job> executionJobs = helper.getExecutionJobs(batch);
		foreach (Job executionJob in executionJobs)
		{
		  engineRule.ManagementService.setJobRetries(executionJob.Id, 0);
		}

		engineRule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = engineRule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchWithFailedMonitorJobDeletionWithCascade()
	  public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.executeSeedJob(batch);

		// create incident
		Job monitorJob = helper.getMonitorJob(batch);
		engineRule.ManagementService.setJobRetries(monitorJob.Id, 0);

		engineRule.ManagementService.deleteBatch(batch.Id, true);

		// then the no historic incidents exists
		long historicIncidents = engineRule.HistoryService.createHistoricIncidentQuery().count();
		assertEquals(0, historicIncidents);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  public virtual void testJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
	  {
		// given
		ProcessEngineConfigurationImpl processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		processEngineConfiguration.AuthorizationEnabled = true;
		ProcessDefinition processDefinition = testRule.deployForTenantAndGetDefinition("tenantId", ProcessModels.TWO_TASKS_PROCESS);

		try
		{
		  ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		  ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		  IList<string> list = Arrays.asList(processInstance1.Id, processInstance2.Id);

		  runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		  runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		  // when
		  Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startTransition("flow1").processInstanceIds(list).executeAsync();
		  helper.executeSeedJob(batch);

		  testRule.waitForJobExecutorToProcessAllJobs();

		  IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).list();
		  // then all process instances were restarted
		  foreach (ProcessInstance restartedProcessInstance in restartedProcessInstances)
		  {
			ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
			assertNotNull(updatedTree);
			assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
			assertEquals("tenantId", restartedProcessInstance.TenantId);

			assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("userTask2").done());
		  }

		}
		finally
		{
		  processEngineConfiguration.AuthorizationEnabled = false;
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNotMatchingProcessDefinition()
	  public virtual void restartProcessInstanceWithNotMatchingProcessDefinition()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.deleteProcessInstance(processInstance.Id, null);
		BpmnModelInstance instance2 = Bpmn.createExecutableProcess().done();
		ProcessDefinition processDefinition2 = testRule.deployAndGetDefinition(instance2);

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition2.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).executeAsync();

		try
		{
		  helper.completeBatch(batch);
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  Assert.assertThat(e.Message, containsString("Its process definition '" + processDefinition.Id + "' does not match given process definition '" + processDefinition2.Id + "'"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithoutBusinessKey()
	  public virtual void shouldRestartProcessInstanceWithoutBusinessKey()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process", "businessKey1", (string) null);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process", "businessKey2", (string) null);

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).withoutBusinessKey().executeAsync();

		helper.completeBatch(batch);
		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		ProcessInstance restartedProcessInstance1 = restartedProcessInstances[0];
		ProcessInstance restartedProcessInstance2 = restartedProcessInstances[1];
		assertNull(restartedProcessInstance1.BusinessKey);
		assertNull(restartedProcessInstance2.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithBusinessKey()
	  public virtual void shouldRestartProcessInstanceWithBusinessKey()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process", "businessKey1", (string) null);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process", "businessKey2", (string) null);

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);
		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		ProcessInstance restartedProcessInstance1 = restartedProcessInstances[0];
		ProcessInstance restartedProcessInstance2 = restartedProcessInstances[1];
		assertNotNull(restartedProcessInstance1.BusinessKey);
		assertNotNull(restartedProcessInstance2.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithoutCaseInstanceId()
	  public virtual void shouldRestartProcessInstanceWithoutCaseInstanceId()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process", null, "caseInstanceId1");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process", null, "caseInstanceId2");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);
		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		ProcessInstance restartedProcessInstance1 = restartedProcessInstances[0];
		ProcessInstance restartedProcessInstance2 = restartedProcessInstances[1];
		assertNull(restartedProcessInstance1.CaseInstanceId);
		assertNull(restartedProcessInstance2.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithTenant()
	  public virtual void shouldRestartProcessInstanceWithTenant()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployForTenantAndGetDefinition("tenantId", ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");


		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);
		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().list();
		assertNotNull(restartedProcessInstances[0].TenantId);
		assertNotNull(restartedProcessInstances[1].TenantId);
		assertEquals("tenantId", restartedProcessInstances[0].TenantId);
		assertEquals("tenantId", restartedProcessInstances[1].TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSkipCustomListeners()
	  public virtual void shouldSkipCustomListeners()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).activityBuilder("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(IncrementCounterListener).FullName).done());
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		IncrementCounterListener.counter = 0;
		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance1.Id, processInstance2.Id).skipCustomListeners().executeAsync();

		helper.completeBatch(batch);
		// then
		assertEquals(0, IncrementCounterListener.counter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSkipIoMappings()
	  public virtual void shouldSkipIoMappings()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).activityBuilder("userTask1").camundaInputParameter("foo", "bar").done());
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("Process");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").skipIoMappings().processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).list();
		Execution task1Execution = runtimeService.createExecutionQuery().processInstanceId(restartedProcessInstances[0].Id).activityId("userTask1").singleResult();
		assertNotNull(task1Execution);
		assertNull(runtimeService.getVariable(task1Execution.Id, "foo"));

		task1Execution = runtimeService.createExecutionQuery().processInstanceId(restartedProcessInstances[1].Id).activityId("userTask1").singleResult();
		assertNotNull(task1Execution);
		assertNull(runtimeService.getVariable(task1Execution.Id, "foo"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRetainTenantIdOfSharedProcessDefinition()
	  public virtual void shouldRetainTenantIdOfSharedProcessDefinition()
	  {
		// given
		engineRule.ProcessEngineConfiguration.TenantIdProvider = new TestTenantIdProvider();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity(ProcessModels.USER_TASK_ID).processInstanceIds(processInstance.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processDefinition.Id).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSkipTenantIdProviderOnRestart()
	  public virtual void shouldSkipTenantIdProviderOnRestart()
	  {
		// given
		engineRule.ProcessEngineConfiguration.TenantIdProvider = new TestTenantIdProvider();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);
		assertEquals(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// set tenant id provider to fail to verify it is not called during instantiation
		engineRule.ProcessEngineConfiguration.TenantIdProvider = new FailingTenantIdProvider();

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity(ProcessModels.USER_TASK_ID).processInstanceIds(processInstance.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		ProcessInstance restartedInstance = runtimeService.createProcessInstanceQuery().active().processDefinitionId(processDefinition.Id).singleResult();

		assertNotNull(restartedInstance);
		assertEquals(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetInitialVariablesIfThereIsNoUniqueStartActivity()
	  public virtual void shouldNotSetInitialVariablesIfThereIsNoUniqueStartActivity()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance1 = runtimeService.createProcessInstanceById(processDefinition.Id).startBeforeActivity("userTask2").startBeforeActivity("userTask1").execute();

		ProcessInstance processInstance2 = runtimeService.createProcessInstanceById(processDefinition.Id).startBeforeActivity("userTask1").startBeforeActivity("userTask2").setVariable("foo", "bar").execute();

		runtimeService.deleteProcessInstance(processInstance1.Id, "test");
		runtimeService.deleteProcessInstance(processInstance2.Id, "test");

		// when
		Batch batch = runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").initialSetOfVariables().processInstanceIds(processInstance1.Id, processInstance2.Id).executeAsync();

		helper.completeBatch(batch);

		// then
		IList<ProcessInstance> restartedProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).list();
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstances[0].Id, restartedProcessInstances[1].Id).list();
		Assert.assertEquals(0, variables.Count);
	  }

	  protected internal virtual void assertBatchCreated(Batch batch, int processInstanceCount)
	  {
		assertNotNull(batch);
		assertNotNull(batch.Id);
		assertEquals("instance-restart", batch.Type);
		assertEquals(processInstanceCount, batch.TotalJobs);
	  }

	  public class TestTenantIdProvider : FailingTenantIdProvider
	  {

		internal const string TENANT_ID = "testTenantId";

		public override string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return TENANT_ID;
		}

	  }

	  public class FailingTenantIdProvider : TenantIdProvider
	  {

		public virtual string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  throw new System.NotSupportedException();
		}

		public virtual string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  throw new System.NotSupportedException();
		}

		public virtual string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  throw new System.NotSupportedException();
		}
	  }

	}

}