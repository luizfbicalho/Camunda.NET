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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class SetBusinessKeyTest
	{
		private bool InstanceFieldsInitialized = false;

		public SetBusinessKeyTest()
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

	  protected internal const string PROCESS_KEY = "process";

	  protected internal static readonly BpmnModelInstance SYNC_SERVICE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent("startEvent").serviceTask().camundaClass(typeof(SetBusinessKeyDelegate)).userTask("userTask2").endEvent("endEvent").done();

	  protected internal static readonly BpmnModelInstance ASYNC_SERVICE_TASK_PROCESS = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent("startEvent").serviceTask().camundaAsyncBefore().camundaClass(typeof(SetBusinessKeyDelegate)).userTask("userTask2").endEvent("endEvent").done();

	  protected internal const string BUSINESS_KEY_VARIABLE = "businessKeyVar";
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
		historyService = rule.HistoryService;
		managementService = rule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInSyncServiceTask()
	  public virtual void testNewKeyInSyncServiceTask()
	  {
		// given
		testRule.deploy(SYNC_SERVICE_TASK_PROCESS);

		// when
		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInAsyncServiceTask()
	  public virtual void testNewKeyInAsyncServiceTask()
	  {
		// given
		testRule.deploy(ASYNC_SERVICE_TASK_PROCESS);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// when
		executeJob();

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInStartExecListener()
	  public virtual void testNewKeyInStartExecListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
		BpmnModelInstance process = createModelExecutionListener(listener);
		testRule.deploy(process);

		// when
		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInEndExecListener()
	  public virtual void testNewKeyInEndExecListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
		BpmnModelInstance process = createModelExecutionListener(listener);
		testRule.deploy(process);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		completeTask("userTask1");

		// assume
		assertNotNull(taskService.createTaskQuery().taskDefinitionKey("userTask2").singleResult());

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInStartTaskListener()
	  public virtual void testNewKeyInStartTaskListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		BpmnModelInstance process = createModelTaskListener(listener);
		testRule.deploy(process);

		// when
		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNewKeyInEndTaskListener()
	  public virtual void testNewKeyInEndTaskListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		BpmnModelInstance process = createModelTaskListener(listener);
		testRule.deploy(process);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		completeTask("userTask1");

		// assume
		assertNotNull(taskService.createTaskQuery().taskDefinitionKey("userTask2").singleResult());

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyInSyncServiceTask()
	  public virtual void testUpdateKeyInSyncServiceTask()
	  {
		// given
		testRule.deploy(SYNC_SERVICE_TASK_PROCESS);

		// when
		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyInAsyncServiceTask()
	  public virtual void testUpdateKeyInAsyncServiceTask()
	  {
		// given
		testRule.deploy(ASYNC_SERVICE_TASK_PROCESS);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// when
		executeJob();

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyInStartExecListener()
	  public virtual void testUpdateKeyInStartExecListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
		BpmnModelInstance process = createModelExecutionListener(listener);
		testRule.deploy(process);

		// when
		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyInEndExecListener()
	  public virtual void testUpdateKeyInEndExecListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
		BpmnModelInstance process = createModelExecutionListener(listener);
		testRule.deploy(process);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		completeTask("userTask1");

		// assume
		assertNotNull(taskService.createTaskQuery().taskDefinitionKey("userTask2").singleResult());

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyInEndTaskListener()
	  public virtual void testUpdateKeyInEndTaskListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE;
		BpmnModelInstance process = createModelTaskListener(listener);
		testRule.deploy(process);

		string newBusinessKeyValue = "newBusinessKey";
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// when
		completeTask("userTask1");

		// assume
		assertNotNull(taskService.createTaskQuery().taskDefinitionKey("userTask2").singleResult());

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateKeyNullValueInStartTaskListener()
	  public virtual void testUpdateKeyNullValueInStartTaskListener()
	  {
		// given
		string listener = org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE;
		BpmnModelInstance process = createModelTaskListener(listener);
		testRule.deploy(process);

		// when
		string newBusinessKeyValue = null;
		runtimeService.startProcessInstanceByKey(PROCESS_KEY, "aBusinessKey", Variables.createVariables().putValue(BUSINESS_KEY_VARIABLE, newBusinessKeyValue));

		// then
		checkBusinessKeyChanged(newBusinessKeyValue);
	  }

	  protected internal virtual void checkBusinessKeyChanged(string newBusinessKeyValue)
	  {
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey(PROCESS_KEY).singleResult();
		assertNotNull(processInstance);
		assertEquals(newBusinessKeyValue, processInstance.BusinessKey);

		HistoricProcessInstance historicInstance = historyService.createHistoricProcessInstanceQuery().singleResult();
		assertNotNull(historicInstance);
		assertEquals(newBusinessKeyValue, historicInstance.BusinessKey);
	  }

	  protected internal virtual BpmnModelInstance createModelExecutionListener(string listener)
	  {
		return Bpmn.createExecutableProcess(PROCESS_KEY).startEvent("startEvent").userTask("userTask1").name("User task").camundaExecutionListenerExpression(listener, "${execution.setProcessBusinessKey(execution.getVariable(\"" + BUSINESS_KEY_VARIABLE + "\"))}").userTask("userTask2").endEvent("endEvent").done();
	  }

	  protected internal virtual BpmnModelInstance createModelTaskListener(string listener)
	  {
		return Bpmn.createExecutableProcess(PROCESS_KEY).startEvent("startEvent").userTask("userTask1").name("User task").camundaTaskListenerClass(listener, typeof(SetBusinessKeyListener)).userTask("userTask2").endEvent("endEvent").done();
	  }

	  protected internal virtual void executeJob()
	  {
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		managementService.executeJob(job.Id);
	  }

	  protected internal virtual void completeTask(string key)
	  {
		Task task = taskService.createTaskQuery().taskDefinitionKey(key).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);
	  }

	  public class SetBusinessKeyDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  string newKeyValue = (string) execution.getVariable(BUSINESS_KEY_VARIABLE);
		  execution.ProcessBusinessKey = newKeyValue;
		}

	  }

	  public class SetBusinessKeyListener : TaskListener
	  {

		public virtual void notify(DelegateTask delegateTask)
		{
		  DelegateExecution execution = delegateTask.Execution;
		  string newKeyValue = (string) execution.getVariable(BUSINESS_KEY_VARIABLE);
		  execution.ProcessBusinessKey = newKeyValue;
		}

	  }
	}

}