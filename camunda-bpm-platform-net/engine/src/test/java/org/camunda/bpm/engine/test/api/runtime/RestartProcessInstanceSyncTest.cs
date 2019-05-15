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
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using TenantIdProviderHistoricDecisionInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderHistoricDecisionInstanceContext;
	using TenantIdProviderProcessInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderProcessInstanceContext;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using IncrementCounterListener = org.camunda.bpm.engine.test.api.runtime.util.IncrementCounterListener;
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
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class RestartProcessInstanceSyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public RestartProcessInstanceSyncTest()
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


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal TenantIdProvider defaultTenantIdProvider;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		defaultTenantIdProvider = engineRule.ProcessEngineConfiguration.TenantIdProvider;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void reset()
	  public virtual void reset()
	  {
		engineRule.ProcessEngineConfiguration.TenantIdProvider = defaultTenantIdProvider;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartSimpleProcessInstance()
	  public virtual void shouldRestartSimpleProcessInstance()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).active().singleResult();
		// process instance was deleted
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		Task restartedTask = engineRule.TaskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(task.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithTwoTasks()
	  public virtual void shouldRestartProcessInstanceWithTwoTasks()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		// the first task is completed
		Task userTask1 = taskService.createTaskQuery().processInstanceId(processInstance.Id).active().singleResult();
		taskService.complete(userTask1.Id);
		Task userTask2 = taskService.createTaskQuery().processInstanceId(processInstance.Id).active().singleResult();
		// delete process instance
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask2").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		Task restartedTask = taskService.createTaskQuery().processInstanceId(restartedProcessInstance.Id).active().singleResult();
		Assert.assertEquals(userTask2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

		ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		assertNotNull(updatedTree);
		assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("userTask2").done());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithParallelGateway()
	  public virtual void shouldRestartProcessInstanceWithParallelGateway()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").startBeforeActivity("userTask2").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		assertNotNull(updatedTree);
		assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("userTask1").activity("userTask2").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithSubProcess()
	  public virtual void shouldRestartProcessInstanceWithSubProcess()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("subProcess").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		assertNotNull(updatedTree);
		assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).beginScope("subProcess").activity("userTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithVariables()
	  public virtual void shouldRestartProcessInstanceWithVariables()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().userTask("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableExecutionListenerImpl).FullName).userTask("userTask2").endEvent().done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		// variable is set at the beginning
		runtimeService.setVariable(processInstance.Id, "var", "bar");

		// variable is changed
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).active().singleResult();
		taskService.complete(task.Id);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).singleResult();

		assertEquals(variableInstance.ExecutionId, restartedProcessInstance.Id);
		assertEquals("var", variableInstance.Name);
		assertEquals("foo", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithInitialVariables()
	  public virtual void shouldRestartProcessInstanceWithInitialVariables()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent("startEvent").userTask("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableExecutionListenerImpl).FullName).userTask("userTask2").endEvent().done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		// initial variable
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("var", "bar"));

		// variable update
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).active().singleResult();
		taskService.complete(task.Id);

		// delete process instance
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").initialSetOfVariables().processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).singleResult();

		assertEquals(variableInstance.ExecutionId, restartedProcessInstance.Id);
		assertEquals("var", variableInstance.Name);
		assertEquals("bar", variableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetLocalVariables()
	  public virtual void shouldNotSetLocalVariables()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		Execution subProcess = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).activityId("userTask").singleResult();
		runtimeService.setVariableLocal(subProcess.Id, "local", "foo");
		runtimeService.setVariable(processInstance.Id, "var", "bar");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).list();
		assertEquals(1, variables.Count);
		assertEquals("var", variables[0].Name);
		assertEquals("bar", variables[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetInitialVersionOfLocalVariables()
	  public virtual void shouldNotSetInitialVersionOfLocalVariables()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("var", "bar"));

		Execution subProcess = runtimeService.createExecutionQuery().processInstanceId(processInstance.Id).activityId("userTask").singleResult();
		runtimeService.setVariableLocal(subProcess.Id, "local", "foo");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).initialSetOfVariables().execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).list();
		assertEquals(1, variables.Count);
		assertEquals("var", variables[0].Name);
		assertEquals("bar", variables[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotSetInitialVersionOfVariables()
	  public virtual void shouldNotSetInitialVersionOfVariables()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("var", "bar"));
		runtimeService.setVariable(processInstance.Id, "bar", "foo");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).initialSetOfVariables().execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).list();
		assertEquals(1, variables.Count);
		assertEquals("var", variables[0].Name);
		assertEquals("bar", variables[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
	  public virtual void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		HistoricProcessInstanceQuery historicProcessInstanceQuery = engineRule.HistoryService.createHistoricProcessInstanceQuery().processDefinitionId(processDefinition.Id);

		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").historicProcessInstanceQuery(historicProcessInstanceQuery).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().active().singleResult();

		ActivityInstance updatedTree = runtimeService.getActivityInstance(restartedProcessInstance.Id);
		assertNotNull(updatedTree);
		assertEquals(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(processDefinition.Id).activity("userTask1").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNullProcessDefinitionId()
	  public virtual void restartProcessInstanceWithNullProcessDefinitionId()
	  {
		try
		{
		  runtimeService.restartProcessInstances(null).execute();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("processDefinitionId is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithoutProcessInstanceIds()
	  public virtual void restartProcessInstanceWithoutProcessInstanceIds()
	  {
		try
		{
		  runtimeService.restartProcessInstances("foo").startAfterActivity("bar").execute();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("processInstanceIds is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithoutInstructions()
	  public virtual void restartProcessInstanceWithoutInstructions()
	  {
		try
		{
		  runtimeService.restartProcessInstances("foo").processInstanceIds("bar").execute();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("Restart instructions cannot be empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNullProcessInstanceId()
	  public virtual void restartProcessInstanceWithNullProcessInstanceId()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		try
		{
		  runtimeService.restartProcessInstances(processDefinition.Id).startAfterActivity("bar").processInstanceIds((string) null).execute();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("Process instance ids cannot be null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartNotExistingProcessInstance()
	  public virtual void restartNotExistingProcessInstance()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		try
		{
		  runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("bar").processInstanceIds("aaa").execute();
		  fail("exception expected");
		}
		catch (BadUserRequestException e)
		{
		  Assert.assertThat(e.Message, containsString("Historic process instance cannot be found"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void restartProcessInstanceWithNotMatchingProcessDefinition()
	  public virtual void restartProcessInstanceWithNotMatchingProcessDefinition()
	  {
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process2").startEvent().userTask().endEvent().done();
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		ProcessDefinition processDefinition2 = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		runtimeService.deleteProcessInstance(processInstance.Id, null);
		try
		{
		  runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask").processInstanceIds(processInstance.Id).execute();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, containsString("Its process definition '" + processDefinition2.Id + "' does not match given process definition '" + processDefinition.Id + "'"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithoutBusinessKey()
	  public virtual void shouldRestartProcessInstanceWithoutBusinessKey()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", "businessKey", (string) null);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).withoutBusinessKey().execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		assertNull(restartedProcessInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithBusinessKey()
	  public virtual void shouldRestartProcessInstanceWithBusinessKey()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", "businessKey", (string) null);
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		assertNotNull(restartedProcessInstance.BusinessKey);
		assertEquals("businessKey", restartedProcessInstance.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithoutCaseInstanceId()
	  public virtual void shouldRestartProcessInstanceWithoutCaseInstanceId()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process", null, "caseInstanceId");
		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		assertNull(restartedProcessInstance.CaseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRestartProcessInstanceWithTenant()
	  public virtual void shouldRestartProcessInstanceWithTenant()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployForTenantAndGetDefinition("tenantId", ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).active().singleResult();
		assertNotNull(restartedProcessInstance.TenantId);
		assertEquals(processInstance.TenantId, restartedProcessInstance.TenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSkipCustomListeners()
	  public virtual void shouldSkipCustomListeners()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).activityBuilder("userTask1").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(IncrementCounterListener).FullName).done());
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		IncrementCounterListener.counter = 0;
		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").processInstanceIds(processInstance.Id).skipCustomListeners().execute();

		// then
		assertEquals(0, IncrementCounterListener.counter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSkipIoMappings()
	  public virtual void shouldSkipIoMappings()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(modify(ProcessModels.TWO_TASKS_PROCESS).activityBuilder("userTask1").camundaInputParameter("foo", "bar").done());
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").skipIoMappings().processInstanceIds(processInstance.Id).execute();

		// then
		Execution task1Execution = runtimeService.createExecutionQuery().activityId("userTask1").singleResult();
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
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity(ProcessModels.USER_TASK_ID).processInstanceIds(processInstance.Id).execute();

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
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity(ProcessModels.USER_TASK_ID).processInstanceIds(processInstance.Id).execute();

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

		ProcessInstance processInstance = runtimeService.createProcessInstanceById(processDefinition.Id).startBeforeActivity("userTask1").startBeforeActivity("userTask2").setVariable("foo", "bar").execute();

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").initialSetOfVariables().processInstanceIds(processInstance.Id).execute();

		// then
		ProcessInstance restartedProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionId(processDefinition.Id).singleResult();
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(restartedProcessInstance.Id).list();
		Assert.assertEquals(0, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotRestartActiveProcessInstance()
	  public virtual void shouldNotRestartActiveProcessInstance()
	  {
		// given
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		// then
		thrown.expect(typeof(ProcessEngineException));

		// when
		runtimeService.restartProcessInstances(processDefinition.Id).startBeforeActivity("userTask1").initialSetOfVariables().processInstanceIds(processInstance.Id).execute();
	  }

	  public class SetVariableExecutionListenerImpl : ExecutionListener
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  execution.setVariable("var", "foo");
		}
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