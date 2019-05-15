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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
	using static org.camunda.bpm.engine.test.api.runtime.migration.models.ConditionalModels;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;


	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class TransientVariableTest
	{
		private bool InstanceFieldsInitialized = false;

		public TransientVariableTest()
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


	  private const int OUTPUT_VALUE = 2;
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		this.runtimeService = engineRule.RuntimeService;
		this.historyService = engineRule.HistoryService;
		this.taskService = engineRule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTransientTypedVariablesUsingVariableMap() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createTransientTypedVariablesUsingVariableMap()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaClass(typeof(ReadTransientVariablesOfAllTypesDelegate).FullName).userTask("user").endEvent().done();

		testRule.deploy(instance);

		// when
		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValueTyped("a", Variables.stringValue("bar", true)).putValueTyped("b", Variables.booleanValue(true, true)).putValueTyped("c", Variables.byteArrayValue("test".GetBytes(), true)).putValueTyped("d", Variables.dateValue(DateTime.Now, true)).putValueTyped("e", Variables.doubleValue(20.0, true)).putValueTyped("f", Variables.integerValue(10, true)).putValueTyped("g", Variables.longValue((long) 10, true)).putValueTyped("h", Variables.shortValue((short) 10, true)).putValueTyped("i", Variables.objectValue(new int?(100), true).create()).putValueTyped("j", Variables.untypedValue(null, true)).putValueTyped("k", Variables.untypedValue(Variables.booleanValue(true), true)).putValueTyped("l", Variables.fileValue(new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt").toURI()), true)));

		// then
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, historicVariableInstances.Count);
		assertEquals(0, variableInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTransientVariablesUsingVariableMap() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createTransientVariablesUsingVariableMap()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaClass(typeof(ReadTransientVariablesOfAllTypesDelegate).FullName).userTask("user").endEvent().done();

		testRule.deploy(instance);

		// when
		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("a", Variables.stringValue("bar", true)).putValue("b", Variables.booleanValue(true, true)).putValue("c", Variables.byteArrayValue("test".GetBytes(), true)).putValue("d", Variables.dateValue(DateTime.Now, true)).putValue("e", Variables.doubleValue(20.0, true)).putValue("f", Variables.integerValue(10, true)).putValue("g", Variables.longValue((long) 10, true)).putValue("h", Variables.shortValue((short) 10, true)).putValue("i", Variables.objectValue(new int?(100), true).create()).putValue("j", Variables.untypedValue(null, true)).putValue("k", Variables.untypedValue(Variables.booleanValue(true), true)).putValue("l", Variables.fileValue(new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt").toURI()), true)));

		// then
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, historicVariableInstances.Count);
		assertEquals(0, variableInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTransientVariablesUsingFluentBuilder()
	  public virtual void createTransientVariablesUsingFluentBuilder()
	  {
		// given
		BpmnModelInstance simpleInstanceWithListener = Bpmn.createExecutableProcess("Process").startEvent().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ReadTransientVariableExecutionListener)).userTask().endEvent().done();
		testRule.deploy(simpleInstanceWithListener);

		// when
		runtimeService.createProcessInstanceByKey("Process").setVariables(Variables.createVariables().putValue(VARIABLE_NAME, Variables.stringValue("dlsd", true))).execute();

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
		assertEquals(0, historicVariableInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createVariablesUsingVariableMap()
	  public virtual void createVariablesUsingVariableMap()
	  {
		// given
		BpmnModelInstance simpleInstanceWithListener = Bpmn.createExecutableProcess("Process").startEvent().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ReadTransientVariableExecutionListener)).userTask().endEvent().done();
		testRule.deploy(simpleInstanceWithListener);

		// when
		VariableMap variables = Variables.createVariables();
		variables.put(VARIABLE_NAME, Variables.untypedValue(true, true));
		runtimeService.startProcessInstanceByKey("Process", variables);

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
		assertEquals(0, historicVariableInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void triggerConditionalEventWithTransientVariable()
	  public virtual void triggerConditionalEventWithTransientVariable()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess(CONDITIONAL_PROCESS_KEY).startEvent().serviceTask().camundaClass(typeof(SetVariableTransientDelegate).FullName).intermediateCatchEvent(CONDITION_ID).conditionalEventDefinition().condition(VAR_CONDITION).conditionalEventDefinitionDone().endEvent().done();

		testRule.deploy(instance);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CONDITIONAL_PROCESS_KEY);

		// then
		assertEquals(true, processInstance.Ended);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelProcessWithSetVariableTransientAfterReachingEventBasedGW()
	  public virtual void testParallelProcessWithSetVariableTransientAfterReachingEventBasedGW()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_PROCESS_KEY).startEvent().parallelGateway().id("parallel").userTask("taskBeforeGw").eventBasedGateway().id("evenBased").intermediateCatchEvent().conditionalEventDefinition().condition(VAR_CONDITION).camundaVariableEvents(Arrays.asList("create", "update")).conditionalEventDefinitionDone().userTask().name("taskAfter").endEvent().moveToNode("parallel").userTask("taskBefore").serviceTask().camundaClass(typeof(SetVariableTransientDelegate).FullName).endEvent().done();

		testRule.deploy(modelInstance);

		//given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey(CONDITIONAL_PROCESS_KEY);
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);
		Task taskBeforeEGW = taskService.createTaskQuery().taskDefinitionKey("taskBeforeGw").singleResult();
		Task taskBeforeServiceTask = taskService.createTaskQuery().taskDefinitionKey("taskBefore").singleResult();

		//when task before event based gateway is completed and after that task before service task
		taskService.complete(taskBeforeEGW.Id);
		taskService.complete(taskBeforeServiceTask.Id);

		//then event based gateway is reached and executions stays there
		//variable is set after reaching event based gateway
		//after setting variable the conditional event is triggered and evaluated to true
		Task task = taskQuery.singleResult();
		assertNotNull(task);
		assertEquals("taskAfter", task.Name);
		//completing this task ends process instance
		taskService.complete(task.Id);
		assertNull(taskQuery.singleResult());
		assertNull(runtimeService.createProcessInstanceQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableTransientInRunningProcessInstance()
	  public virtual void setVariableTransientInRunningProcessInstance()
	  {
		// given
		testRule.deploy(ProcessModels.ONE_TASK_PROCESS);

		// when
		runtimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);
		Execution execution = runtimeService.createExecutionQuery().singleResult();
		runtimeService.setVariable(execution.Id, "foo", Variables.stringValue("bar", true));

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableTransientForCase()
	  public virtual void setVariableTransientForCase()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn");

		// when
		engineRule.CaseService.withCaseDefinitionByKey("oneTaskCase").setVariable("foo", Variables.stringValue("bar", true)).create();

		// then
		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(0, variables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransientVariableOvewritesPersistedVariableInSameScope()
	  public virtual void testTransientVariableOvewritesPersistedVariableInSameScope()
	  {
		testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", "bar"));
		Execution execution = runtimeService.createExecutionQuery().singleResult();

		try
		{
		  runtimeService.setVariable(execution.Id, "foo", Variables.stringValue("xyz", true));
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot set transient variable with name foo"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSameNamesDifferentScopes()
	  public virtual void testSameNamesDifferentScopes()
	  {
		testRule.deploy(ProcessModels.SUBPROCESS_PROCESS);
		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValue("foo", Variables.stringValue("bar")));
		Execution execution = runtimeService.createExecutionQuery().activityId(USER_TASK_ID).singleResult();

		try
		{
		  runtimeService.setVariable(execution.Id, "foo", Variables.stringValue("xyz", true));
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Cannot set transient variable with name foo"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormFieldsWithCustomTransientFlags()
	  public virtual void testFormFieldsWithCustomTransientFlags()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/form/TransientVariableTest.taskFormFieldsWithTransientFlags.bpmn20.xml");
		runtimeService.startProcessInstanceByKey("testProcess");
		Task task = taskService.createTaskQuery().singleResult();

		// when
		IDictionary<string, object> formValues = new Dictionary<string, object>();
		formValues["stringField"] = Variables.stringValue("foobar", true);
		formValues["longField"] = 9L;
		engineRule.FormService.submitTaskForm(task.Id, formValues);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().list();
		assertEquals(1, variables.Count);
		assertEquals(variables[0].Value, 9L);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartProcessInstanceWithFormsUsingTransientVariables()
	  public virtual void testStartProcessInstanceWithFormsUsingTransientVariables()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/form/TransientVariableTest.startFormFieldsWithTransientFlags.bpmn20.xml");
		ProcessDefinition processDefinition = engineRule.RepositoryService.createProcessDefinitionQuery().singleResult();

		// when
		IDictionary<string, object> formValues = new Dictionary<string, object>();
		formValues["stringField"] = Variables.stringValue("foobar", true);
		formValues["longField"] = 9L;
		engineRule.FormService.submitStartForm(processDefinition.Id, formValues);

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().list();
		assertEquals(1, variables.Count);
		assertEquals(variables[0].Value, 9L);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSignalWithTransientVariables()
	  public virtual void testSignalWithTransientVariables()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().intermediateCatchEvent("signalCatch").signal("signal").scriptTask("scriptTask").scriptFormat("javascript").camundaResultVariable("abc").scriptText("execution.setVariable('abc', foo);").endEvent().done();

		testRule.deploy(instance);
		runtimeService.startProcessInstanceByKey("Process");

		// when
		runtimeService.signalEventReceived("signal", Variables.createVariables().putValue("foo", Variables.stringValue("bar", true)));

		// then
		IList<HistoricVariableInstance> variables = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, variables.Count);
		assertEquals("abc", variables[0].Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartMessageCorrelationWithTransientVariable()
	  public virtual void testStartMessageCorrelationWithTransientVariable()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent().message("message").scriptTask("scriptTask").scriptFormat("javascript").camundaResultVariable("abc").scriptText("execution.setVariable('abc', foo);").endEvent().done();

		testRule.deploy(instance);

		// when
		runtimeService.createMessageCorrelation("message").setVariable("foo", Variables.stringValue("bar", true)).correlate();

		// then
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variableInstances.Count);
		IList<HistoricVariableInstance> historicInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, historicInstances.Count);
		assertEquals("abc", historicInstances[0].Name);
		assertEquals("bar", historicInstances[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageCorrelationWithTransientVariable()
	  public virtual void testMessageCorrelationWithTransientVariable()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("process").startEvent().intermediateCatchEvent().message("message").scriptTask("scriptTask").scriptFormat("javascript").camundaResultVariable("abc").scriptText("execution.setVariable('abc', blob);").endEvent().done();

		testRule.deploy(instance);
		runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValueTyped("foo", Variables.stringValue("foo", false)));

		// when
		VariableMap correlationKeys = Variables.createVariables().putValueTyped("foo", Variables.stringValue("foo", true));
		VariableMap variables = Variables.createVariables().putValueTyped("blob", Variables.stringValue("blob", true));
		runtimeService.correlateMessage("message", correlationKeys, variables);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertNull(variableInstance);
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableName("abc").singleResult();
		assertNotNull(historicVariableInstance);
		assertEquals("blob", historicVariableInstance.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelExecutions()
	  public virtual void testParallelExecutions()
	  {
		// given
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().parallelGateway().scriptTask().scriptFormat("javascript").camundaResultVariable("abc").scriptText("execution.setVariableLocal('abc', foo);").endEvent().moveToLastGateway().scriptTask().scriptFormat("javascript").camundaResultVariable("abc").scriptText("execution.setVariableLocal('abc', foo);").endEvent().done();

		testRule.deploy(instance);

		// when
		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValueTyped("foo", Variables.stringValue("bar", true)));

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variables.Count);

		IList<HistoricVariableInstance> historicVariables = historyService.createHistoricVariableInstanceQuery().variableName("abc").list();
		assertEquals(2, historicVariables.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExclusiveGateway()
	  public virtual void testExclusiveGateway()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/bpmn/gateway/ExclusiveGatewayTest.testDivergingExclusiveGateway.bpmn20.xml");

		// when
		runtimeService.startProcessInstanceByKey("exclusiveGwDiverging", Variables.createVariables().putValueTyped("input", Variables.integerValue(1, true)));

		// then
		IList<VariableInstance> variables = runtimeService.createVariableInstanceQuery().list();
		assertEquals(0, variables.Count);
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("theTask1", task.TaskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeTransientVariable() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testChangeTransientVariable()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaClass(typeof(ChangeVariableTransientDelegate).FullName).userTask("user").endEvent().done();

		testRule.deploy(instance);

		string output = "transientVariableOutput";
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[output] = false;

		// when
		runtimeService.startProcessInstanceByKey("Process", variables);

		// then
		IList<HistoricVariableInstance> historicVariableInstances = historyService.createHistoricVariableInstanceQuery().list();
		IList<VariableInstance> variableInstances = runtimeService.createVariableInstanceQuery().list();
		assertEquals(1, historicVariableInstances.Count);
		assertEquals(1, variableInstances.Count);
		assertEquals(output, variableInstances[0].Name);
		assertEquals(OUTPUT_VALUE, variableInstances[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSwitchTransientToNonVariable() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSwitchTransientToNonVariable()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaClass(typeof(SwitchTransientVariableDelegate).FullName).userTask("user").endEvent().done();

		testRule.deploy(instance);


		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["transient1"] = true;
		variables["transient2"] = false;

		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot set transient variable with name variable to non-transient variable and vice versa.");

		// when
		runtimeService.startProcessInstanceByKey("Process", variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSwitchNonToTransientVariable() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSwitchNonToTransientVariable()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance instance = Bpmn.createExecutableProcess("Process").startEvent().serviceTask().camundaClass(typeof(SwitchTransientVariableDelegate).FullName).userTask("user").endEvent().done();

		testRule.deploy(instance);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["transient1"] = false;
		variables["transient2"] = true;

		// expect
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot set transient variable with name variable to non-transient variable and vice versa.");

		// when
		runtimeService.startProcessInstanceByKey("Process", variables);
	  }

	  public class SetVariableTransientDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  execution.setVariable(VARIABLE_NAME, Variables.integerValue(1, true));
		}
	  }

	  public class ReadTransientVariablesOfAllTypesDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  for (char i = 'a'; i < 'm'; i++)
		  {
			object value = execution.getVariable("" + i);
			// variable 'j' is a transient null
			if (i != 'j')
			{
			  assertNotNull(value);
			}
			else
			{
				assertNull(value);
			}
		  }
		}
	  }

	  public class ReadTransientVariableExecutionListener : ExecutionListener
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  object variable = execution.getVariable(VARIABLE_NAME);
		  assertNotNull(variable);
		}
	  }

	  public class ChangeVariableTransientDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  execution.setVariable(VARIABLE_NAME, Variables.integerValue(1, true));
		  execution.setVariable(VARIABLE_NAME, Variables.integerValue(OUTPUT_VALUE, true));
		  execution.setVariable("transientVariableOutput", execution.getVariable(VARIABLE_NAME));
		}
	  }

	  public class SwitchTransientVariableDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  bool? transient1 = (bool?) execution.getVariable("transient1");
		  bool? transient2 = (bool?) execution.getVariable("transient2");
		  execution.setVariable(VARIABLE_NAME, Variables.integerValue(1, transient1));
		  execution.setVariable(VARIABLE_NAME, Variables.integerValue(2, transient2));
		}
	  }

	}

}