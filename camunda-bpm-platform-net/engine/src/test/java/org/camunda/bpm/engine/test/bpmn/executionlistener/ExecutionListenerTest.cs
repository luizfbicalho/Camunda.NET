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
namespace org.camunda.bpm.engine.test.bpmn.executionlistener
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT;

	using AssertionFailedError = junit.framework.AssertionFailedError;
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;

	using SetVariableDelegate = org.camunda.bpm.engine.test.bpmn.@event.conditional.SetVariableDelegate;
	using CurrentActivity = org.camunda.bpm.engine.test.bpmn.executionlistener.CurrentActivityExecutionListener.CurrentActivity;
	using RecordedEvent = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener.RecordedEvent;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsCollectionWithSize.hasSize;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class ExecutionListenerTest
	{
		private bool InstanceFieldsInitialized = false;

		public ExecutionListenerTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(processEngineRule);
			ruleChain = RuleChain.outerRule(processEngineRule).around(testHelper);
		}


	  protected internal const string ERROR_CODE = "208";
	  protected internal const string PROCESS_KEY = "Process";
	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void clearRecorderListener()
	  public virtual void clearRecorderListener()
	  {
		RecorderExecutionListener.clear();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = processEngineRule.RuntimeService;
		taskService = processEngineRule.TaskService;
		historyService = processEngineRule.HistoryService;
		managementService = processEngineRule.ManagementService;
		repositoryService = processEngineRule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void resetListener()
	  public virtual void resetListener()
	  {
		ThrowBPMNErrorDelegate.reset();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void assertProcessEnded(final String processInstanceId)
	  public virtual void assertProcessEnded(string processInstanceId)
	  {
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		if (processInstance != null)
		{
		  throw new AssertionFailedError("Expected finished process instance '" + processInstanceId + "' but it was still in the db");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersProcess.bpmn20.xml"}) public void testExecutionListenersOnAllPossibleElements()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersProcess.bpmn20.xml"})]
	  public virtual void testExecutionListenersOnAllPossibleElements()
	  {

		// Process start executionListener will have executionListener class that sets 2 variables
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("executionListenersProcess", "businessKey123");

		string varSetInExecutionListener = (string) runtimeService.getVariable(processInstance.Id, "variableSetInExecutionListener");
		assertNotNull(varSetInExecutionListener);
		assertEquals("firstValue", varSetInExecutionListener);

		// Check if business key was available in execution listener
		string businessKey = (string) runtimeService.getVariable(processInstance.Id, "businessKeyInExecution");
		assertNotNull(businessKey);
		assertEquals("businessKey123", businessKey);

		// Transition take executionListener will set 2 variables
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		varSetInExecutionListener = (string) runtimeService.getVariable(processInstance.Id, "variableSetInExecutionListener");

		assertNotNull(varSetInExecutionListener);
		assertEquals("secondValue", varSetInExecutionListener);

		ExampleExecutionListenerPojo myPojo = new ExampleExecutionListenerPojo();
		runtimeService.setVariable(processInstance.Id, "myPojo", myPojo);

		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		// First usertask uses a method-expression as executionListener: ${myPojo.myMethod(execution.eventName)}
		ExampleExecutionListenerPojo pojoVariable = (ExampleExecutionListenerPojo) runtimeService.getVariable(processInstance.Id, "myPojo");
		assertNotNull(pojoVariable.ReceivedEventName);
		assertEquals("end", pojoVariable.ReceivedEventName);

		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertNotNull(task);
		taskService.complete(task.Id);

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersStartEndEvent.bpmn20.xml"}) public void testExecutionListenersOnStartEndEvents()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersStartEndEvent.bpmn20.xml"})]
	  public virtual void testExecutionListenersOnStartEndEvents()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("executionListenersProcess");
		assertProcessEnded(processInstance.Id);

		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(4, recordedEvents.Count);

		assertEquals("theStart", recordedEvents[0].ActivityId);
		assertEquals("Start Event", recordedEvents[0].ActivityName);
		assertEquals("Start Event Listener", recordedEvents[0].Parameter);
		assertEquals("end", recordedEvents[0].EventName);
		assertThat(recordedEvents[0].Canceled, @is(false));

		assertEquals("noneEvent", recordedEvents[1].ActivityId);
		assertEquals("None Event", recordedEvents[1].ActivityName);
		assertEquals("Intermediate Catch Event Listener", recordedEvents[1].Parameter);
		assertEquals("end", recordedEvents[1].EventName);
		assertThat(recordedEvents[1].Canceled, @is(false));

		assertEquals("signalEvent", recordedEvents[2].ActivityId);
		assertEquals("Signal Event", recordedEvents[2].ActivityName);
		assertEquals("Intermediate Throw Event Listener", recordedEvents[2].Parameter);
		assertEquals("start", recordedEvents[2].EventName);
		assertThat(recordedEvents[2].Canceled, @is(false));

		assertEquals("theEnd", recordedEvents[3].ActivityId);
		assertEquals("End Event", recordedEvents[3].ActivityName);
		assertEquals("End Event Listener", recordedEvents[3].Parameter);
		assertEquals("start", recordedEvents[3].EventName);
		assertThat(recordedEvents[3].Canceled, @is(false));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersFieldInjectionProcess.bpmn20.xml"}) public void testExecutionListenerFieldInjection()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersFieldInjectionProcess.bpmn20.xml"})]
	  public virtual void testExecutionListenerFieldInjection()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["myVar"] = "listening!";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("executionListenersProcess", variables);

		object varSetByListener = runtimeService.getVariable(processInstance.Id, "var");
		assertNotNull(varSetByListener);
		assertTrue(varSetByListener is string);

		// Result is a concatenation of fixed injected field and injected expression
		assertEquals("Yes, I am listening!", varSetByListener);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersCurrentActivity.bpmn20.xml"}) public void testExecutionListenerCurrentActivity()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenersCurrentActivity.bpmn20.xml"})]
	  public virtual void testExecutionListenerCurrentActivity()
	  {

		CurrentActivityExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("executionListenersProcess");
		assertProcessEnded(processInstance.Id);

		IList<CurrentActivity> currentActivities = CurrentActivityExecutionListener.CurrentActivities;
		assertEquals(3, currentActivities.Count);

		assertEquals("theStart", currentActivities[0].ActivityId);
		assertEquals("Start Event", currentActivities[0].ActivityName);

		assertEquals("noneEvent", currentActivities[1].ActivityId);
		assertEquals("None Event", currentActivities[1].ActivityName);

		assertEquals("theEnd", currentActivities[2].ActivityId);
		assertEquals("End Event", currentActivities[2].ActivityName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testOnBoundaryEvents.bpmn20.xml"}) public void testOnBoundaryEvents()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testOnBoundaryEvents.bpmn20.xml"})]
	  public virtual void testOnBoundaryEvents()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Job firstTimer = managementService.createJobQuery().timers().singleResult();

		managementService.executeJob(firstTimer.Id);

		Job secondTimer = managementService.createJobQuery().timers().singleResult();

		managementService.executeJob(secondTimer.Id);

		assertProcessEnded(processInstance.Id);

		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);

		assertEquals("timer1", recordedEvents[0].ActivityId);
		assertEquals("start boundary listener", recordedEvents[0].Parameter);
		assertEquals("start", recordedEvents[0].EventName);
		assertThat(recordedEvents[0].Canceled, @is(false));

		assertEquals("timer2", recordedEvents[1].ActivityId);
		assertEquals("end boundary listener", recordedEvents[1].Parameter);
		assertEquals("end", recordedEvents[1].EventName);
		assertThat(recordedEvents[1].Canceled, @is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testScriptListener()
	  public virtual void testScriptListener()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertTrue(processInstance.Ended);


		if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		  long count = query.count();
		  assertEquals(5, count);

		  HistoricVariableInstance variableInstance = null;
		  string[] variableNames = new string[]{"start-start", "start-end", "start-take", "end-start", "end-end"};
		  foreach (string variableName in variableNames)
		  {
			variableInstance = query.variableName(variableName).singleResult();
			assertNotNull("Unable ot find variable with name '" + variableName + "'", variableInstance);
			assertTrue("Variable '" + variableName + "' should be set to true", (bool?) variableInstance.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testScriptResourceListener.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/executionlistener/executionListener.groovy" }) public void testScriptResourceListener()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testScriptResourceListener.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/executionlistener/executionListener.groovy" })]
	  public virtual void testScriptResourceListener()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertTrue(processInstance.Ended);

		if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();
		  long count = query.count();
		  assertEquals(5, count);

		  HistoricVariableInstance variableInstance = null;
		  string[] variableNames = new string[]{"start-start", "start-end", "start-take", "end-start", "end-end"};
		  foreach (string variableName in variableNames)
		  {
			variableInstance = query.variableName(variableName).singleResult();
			assertNotNull("Unable ot find variable with name '" + variableName + "'", variableInstance);
			assertTrue("Variable '" + variableName + "' should be set to true", (bool?) variableInstance.Value);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testExecutionListenerOnTerminateEndEvent()
	  public virtual void testExecutionListenerOnTerminateEndEvent()
	  {
		RecorderExecutionListener.clear();

		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;

		assertEquals(2, recordedEvents.Count);

		assertEquals("start", recordedEvents[0].EventName);
		assertEquals("end", recordedEvents[1].EventName);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testOnCancellingBoundaryEvent.bpmn"}) public void testOnCancellingBoundaryEvents()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testOnCancellingBoundaryEvent.bpmn"})]
	  public virtual void testOnCancellingBoundaryEvents()
	  {
		RecorderExecutionListener.clear();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		Job timer = managementService.createJobQuery().timers().singleResult();

		managementService.executeJob(timer.Id);

		assertProcessEnded(processInstance.Id);

		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertThat(recordedEvents, hasSize(1));

		assertEquals("UserTask_1", recordedEvents[0].ActivityId);
		assertEquals("end", recordedEvents[0].EventName);
		assertThat(recordedEvents[0].Canceled, @is(true));
	  }

	  private const string MESSAGE = "cancelMessage";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().parallelGateway("fork").userTask("userTask1").serviceTask("sendTask").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SendMessageDelegate).FullName).camundaExpression("${true}").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).moveToLastGateway().userTask("userTask2").boundaryEvent("boundaryEvent").message(MESSAGE).endEvent("endBoundaryEvent").moveToNode("userTask2").endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskExecutionListenerCall()
	  public virtual void testServiceTaskExecutionListenerCall()
	  {
		testHelper.deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.complete(task.Id);

		assertEquals(0, taskService.createTaskQuery().list().size());
		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(1, recordedEvents.Count);
		assertEquals("endEvent", recordedEvents[0].ActivityId);
	  }

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_SERVICE_TASK_WITH_TWO_EXECUTION_START_LISTENER = modify(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER).activityBuilder("sendTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskTwoExecutionListenerCall()
	  public virtual void testServiceTaskTwoExecutionListenerCall()
	  {
		testHelper.deploy(PROCESS_SERVICE_TASK_WITH_TWO_EXECUTION_START_LISTENER);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.complete(task.Id);

		assertEquals(0, taskService.createTaskQuery().list().size());
		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);
		assertEquals("sendTask", recordedEvents[0].ActivityId);
		assertEquals("endEvent", recordedEvents[1].ActivityId);
	  }

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS = modify(Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask").serviceTask("sendTask").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SendMessageDelegate).FullName).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).camundaExpression("${true}").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done()).addSubProcessTo(PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent("startSubProcess").interrupting(false).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).message(MESSAGE).userTask("subProcessTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).endEvent("endSubProcess").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskExecutionListenerCallAndSubProcess()
	  public virtual void testServiceTaskExecutionListenerCallAndSubProcess()
	  {
		testHelper.deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask").singleResult();
		taskService.complete(task.Id);

		assertEquals(1, taskService.createTaskQuery().list().size());

		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(4, recordedEvents.Count);
		assertEquals("startSubProcess", recordedEvents[0].ActivityId);
		assertEquals("subProcessTask", recordedEvents[1].ActivityId);
		assertEquals("sendTask", recordedEvents[2].ActivityId);
		assertEquals("endEvent", recordedEvents[3].ActivityId);
	  }

	  public class SendMessageDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  RuntimeService runtimeService = execution.ProcessEngineServices.RuntimeService;
		  runtimeService.correlateMessage(MESSAGE);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEndExecutionListenerIsCalledOnlyOnce()
	  public virtual void testEndExecutionListenerIsCalledOnlyOnce()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("conditionalProcessKey").startEvent().userTask().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetVariableDelegate).FullName).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(RecorderExecutionListener).FullName).endEvent().done();

		modelInstance = modify(modelInstance).addSubProcessTo("conditionalProcessKey").triggerByEvent().embeddedSubProcess().startEvent().interrupting(true).conditionalEventDefinition().condition("${variable == 1}").conditionalEventDefinitionDone().endEvent().done();

		testHelper.deploy(modelInstance);

		// given
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("conditionalProcessKey");
		TaskQuery taskQuery = taskService.createTaskQuery().processInstanceId(procInst.Id);

		//when task is completed
		taskService.complete(taskQuery.singleResult().Id);

		//then end listener sets variable and triggers conditional event
		//end listener should called only once
		assertEquals(1, RecorderExecutionListener.RecordedEvents.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testMultiInstanceCancelation.bpmn20.xml") public void testMultiInstanceCancelationDoesNotAffectEndListener()
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testMultiInstanceCancelation.bpmn20.xml")]
	  public virtual void testMultiInstanceCancelationDoesNotAffectEndListener()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("MultiInstanceCancelation");
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[2].Id);

		// when
		taskService.complete(tasks[3].Id);

		// then
		assertProcessEnded(processInstance.Id);
		if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstance endVariable = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("finished").singleResult();
		  assertNotNull(endVariable);
		  assertNotNull(endVariable.Value);
		  assertTrue(Convert.ToBoolean(endVariable.Value.ToString()));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testMultiInstanceCancelation.bpmn20.xml") public void testProcessInstanceCancelationNoticedInEndListener()
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/executionlistener/ExecutionListenerTest.testMultiInstanceCancelation.bpmn20.xml")]
	  public virtual void testProcessInstanceCancelationNoticedInEndListener()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("MultiInstanceCancelation");
		IList<Task> tasks = taskService.createTaskQuery().processInstanceId(processInstance.Id).list();
		taskService.complete(tasks[0].Id);
		taskService.complete(tasks[2].Id);

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, "myReason");

		// then
		assertProcessEnded(processInstance.Id);
		if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >= HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstance endVariable = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("canceled").singleResult();
		  assertNotNull(endVariable);
		  assertNotNull(endVariable.Value);
		  assertTrue(Convert.ToBoolean(endVariable.Value.ToString()));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerServiceTaskWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerServiceTaskWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInServiceTaskAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerAndSubprocessWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInSubprocessAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerAndEventSubprocessWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInEventSubprocessAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerAndServiceTaskWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerAndServiceTaskWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInServiceTaskAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerAndSubprocessWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInSubprocessAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerAndEventSubprocessWithCatch()
	  {
		// given
		BpmnModelInstance model = createModelWithCatchInEventSubprocessAndListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInTakeListenerAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInTakeListenerAndEventSubprocessWithCatch()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
		BpmnModelInstance model = processBuilder.startEvent().userTask("userTask1").sequenceFlowId("flow1").userTask("afterListener").endEvent().done();

		CamundaExecutionListener listener = model.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		listener.CamundaClass = typeof(ThrowBPMNErrorDelegate).FullName;
		model.getModelElementById<SequenceFlow>("flow1").builder().addExtensionElement(listener);

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOfStartEventAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerOfStartEventAndEventSubprocessWithCatch()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = processBuilder.startEvent().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).userTask("afterListener").endEvent().done();

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(model);
		// when the listeners are invoked
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOfStartEventAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerOfStartEventAndSubprocessWithCatch()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").embeddedSubProcess().startEvent("inSub").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).userTask("afterListener").endEvent().subProcessDone().boundaryEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent("endEvent").moveToActivity("sub").endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerOfLastEventAndEventProcessWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerOfLastEventAndEventProcessWithCatch()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = processBuilder.startEvent().userTask("userTask1").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ThrowBPMNErrorDelegate).FullName).done();

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		Task afterCatch = taskService.createTaskQuery().singleResult();
		assertNotNull(afterCatch);
		assertEquals("afterCatch", afterCatch.Name);
		assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);

		// and completing this task ends the process instance
		taskService.complete(afterCatch.Id);

		assertEquals(0, runtimeService.createExecutionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerOfLastEventAndServiceTaskWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerOfLastEventAndServiceTaskWithCatch()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ThrowBPMNErrorDelegate).FullName).boundaryEvent().error(ERROR_CODE).userTask("afterCatch").endEvent().done();

		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOfLastEventAndServiceTaskWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerOfLastEventAndServiceTaskWithCatch()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).boundaryEvent().error(ERROR_CODE).userTask("afterCatch").endEvent().done();

		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerOfLastEventAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerOfLastEventAndSubprocessWithCatch()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").embeddedSubProcess().startEvent("inSub").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ThrowBPMNErrorDelegate).FullName).boundaryEvent().error(ERROR_CODE).userTask("afterCatch").moveToActivity("sub").userTask("afterSub").endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOfLastEventAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartListenerOfLastEventAndSubprocessWithCatch()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").embeddedSubProcess().startEvent("inSub").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).boundaryEvent().error(ERROR_CODE).userTask("afterCatch").moveToActivity("sub").userTask("afterSub").endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerServiceTaskAndEndListener()
	  public virtual void testThrowBpmnErrorInStartListenerServiceTaskAndEndListener()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(SetsVariableDelegate).FullName).camundaExpression("${true}").boundaryEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent("endEvent").moveToActivity("throw").userTask("afterService").endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
		// end listener is called
		assertEquals("bar", runtimeService.createVariableInstanceQuery().variableName("foo").singleResult().Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOfStartEventAndCallActivity()
	  public virtual void testThrowBpmnErrorInStartListenerOfStartEventAndCallActivity()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance subprocess = Bpmn.createExecutableProcess("subprocess").startEvent().userTask("userTask1").serviceTask("throw").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).camundaExpression("${true}").userTask("afterService").done();
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
		BpmnModelInstance parent = processBuilder.startEvent().callActivity().calledElement("subprocess").userTask("afterCallActivity").done();

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(parent, subprocess);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerInConcurrentExecutionAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInEndListenerInConcurrentExecutionAndEventSubprocessWithCatch()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = processBuilder.startEvent().parallelGateway("fork").userTask("userTask1").serviceTask("throw").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).camundaExpression("${true}").userTask("afterService").endEvent().moveToLastGateway().userTask("userTask2").done();
		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		// when the listeners are invoked
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartExpressionListenerAndEventSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInStartExpressionListenerAndEventSubprocessWithCatch()
	  {
		// given
		processEngineRule.ProcessEngineConfiguration.Beans["myListener"] = new ThrowBPMNErrorDelegate();

		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
		BpmnModelInstance model = processBuilder.startEvent().userTask("userTask1").serviceTask("throw").camundaExecutionListenerExpression(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, "${myListener.notify(execution)}").camundaExpression("${true}").userTask("afterService").endEvent().done();
		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		// when listeners are invoked
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.complete(task.Id);

		// then
		verifyErrorGotCaught();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testThrowBpmnErrorInEndScriptListenerAndSubprocessWithCatch()
	  public virtual void testThrowBpmnErrorInEndScriptListenerAndSubprocessWithCatch()
	  {
		// when the listeners are invoked
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		// then
		assertEquals(1, taskService.createTaskQuery().list().size());
		assertEquals("afterCatch", taskService.createTaskQuery().singleResult().Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowUncaughtBpmnErrorFromEndListenerShouldNotTriggerListenerAgain()
	  public virtual void testThrowUncaughtBpmnErrorFromEndListenerShouldNotTriggerListenerAgain()
	  {

		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ThrowBPMNErrorDelegate).FullName).endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when the listeners are invoked
		taskService.complete(task.Id);

		// then

		// the process has ended, because the error was not caught
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// the listener was only called once
		assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowUncaughtBpmnErrorFromStartListenerShouldNotTriggerListenerAgain()
	  public virtual void testThrowUncaughtBpmnErrorFromStartListenerShouldNotTriggerListenerAgain()
	  {

		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExpression("${true}").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).endEvent().done();

		testHelper.deploy(model);

		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();

		// when the listeners are invoked
		taskService.complete(task.Id);

		// then

		// the process has ended, because the error was not caught
		assertEquals(0, runtimeService.createExecutionQuery().count());

		// the listener was only called once
		assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInEndListenerMessageCorrelationShouldNotTriggerPropagation()
	  public virtual void testThrowBpmnErrorInEndListenerMessageCorrelationShouldNotTriggerPropagation()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").embeddedSubProcess().startEvent("inSub").userTask("taskWithListener").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(ThrowBPMNErrorDelegate).FullName).boundaryEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent().subProcessDone().boundaryEvent("message").message("foo").userTask("afterMessage").endEvent("endEvent").moveToActivity("sub").endEvent().done();

		DeploymentWithDefinitions deployment = testHelper.deploy(model);
		runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.complete(task.Id);
		// assert
		assertEquals(1, taskService.createTaskQuery().list().size());
		assertEquals("taskWithListener", taskService.createTaskQuery().singleResult().Name);

		try
		{
		  // when the listeners are invoked
		  runtimeService.correlateMessage("foo");
		  fail("Expected exception");
		}
		catch (Exception e)
		{
		  // then
		  assertTrue(e.Message.contains("business error"));
		  assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
		}

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInStartListenerOnModificationShouldNotTriggerPropagation()
	  public virtual void testThrowBpmnErrorInStartListenerOnModificationShouldNotTriggerPropagation()
	  {
		// expect
		thrown.expect(typeof(BpmnError));
		thrown.expectMessage("business error");

		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(ThrowBPMNErrorDelegate).FullName).embeddedSubProcess().startEvent("inSub").serviceTask("throw").camundaExpression("${true}").boundaryEvent("errorEvent1").error(ERROR_CODE).subProcessDone().boundaryEvent("errorEvent2").error(ERROR_CODE).userTask("afterCatch").endEvent("endEvent").moveToActivity("sub").userTask("afterSub").endEvent().done();
		DeploymentWithDefinitions deployment = testHelper.deploy(model);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(PROCESS_KEY);

		// when the listeners are invoked
		runtimeService.createModification(definition.Id).startBeforeActivity("throw").processInstanceIds(processInstance.Id).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInProcessStartListenerShouldNotTriggerPropagation()
	  public virtual void testThrowBpmnErrorInProcessStartListenerShouldNotTriggerPropagation()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
		BpmnModelInstance model = processBuilder.startEvent().userTask("afterThrow").endEvent().done();

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		CamundaExecutionListener listener = model.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		listener.CamundaClass = typeof(ThrowBPMNErrorDelegate).FullName;
		model.getModelElementById<org.camunda.bpm.model.bpmn.instance.Process>(PROCESS_KEY).builder().addExtensionElement(listener);

		DeploymentWithDefinitions deployment = testHelper.deploy(model);

		try
		{
		  // when listeners are invoked
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		  fail("Exception expected");
		}
		catch (Exception e)
		{
		  // then
		  assertTrue(e.Message.contains("business error"));
		  assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
		}

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowBpmnErrorInProcessEndListenerShouldNotTriggerPropagation()
	  public virtual void testThrowBpmnErrorInProcessEndListenerShouldNotTriggerPropagation()
	  {
		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
		BpmnModelInstance model = processBuilder.startEvent().endEvent().done();

		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();

		CamundaExecutionListener listener = model.newInstance(typeof(CamundaExecutionListener));
		listener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		listener.CamundaClass = typeof(ThrowBPMNErrorDelegate).FullName;
		model.getModelElementById<org.camunda.bpm.model.bpmn.instance.Process>(PROCESS_KEY).builder().addExtensionElement(listener);

		DeploymentWithDefinitions deployment = testHelper.deploy(model);

		try
		{
		  // when listeners are invoked
		  runtimeService.startProcessInstanceByKey(PROCESS_KEY);
		  fail("Exception expected");
		}
		catch (Exception e)
		{
		  assertTrue(e.Message.contains("business error"));
		  assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
		}

		// cleanup
		repositoryService.deleteDeployment(deployment.Id, true, true);
	  }

	  protected internal virtual BpmnModelInstance createModelWithCatchInServiceTaskAndListener(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").serviceTask("throw").camundaExecutionListenerClass(eventName, typeof(ThrowBPMNErrorDelegate).FullName).camundaExpression("${true}").boundaryEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent("endEvent").moveToActivity("throw").userTask("afterService").endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createModelWithCatchInSubprocessAndListener(string eventName)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return Bpmn.createExecutableProcess(PROCESS_KEY).startEvent().userTask("userTask1").subProcess("sub").embeddedSubProcess().startEvent("inSub").serviceTask("throw").camundaExecutionListenerClass(eventName, typeof(ThrowBPMNErrorDelegate).FullName).camundaExpression("${true}").userTask("afterService").endEvent().subProcessDone().boundaryEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent("endEvent").moveToActivity("sub").userTask("afterSub").endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createModelWithCatchInEventSubprocessAndListener(string eventName)
	  {
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess(PROCESS_KEY);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = processBuilder.startEvent().userTask("userTask1").serviceTask("throw").camundaExecutionListenerClass(eventName, typeof(ThrowBPMNErrorDelegate).FullName).camundaExpression("${true}").userTask("afterService").endEvent().done();
		processBuilder.eventSubProcess().startEvent("errorEvent").error(ERROR_CODE).userTask("afterCatch").endEvent();
		return model;
	  }

	  protected internal virtual void verifyErrorGotCaught()
	  {
		assertEquals(1, taskService.createTaskQuery().list().size());
		assertEquals("afterCatch", taskService.createTaskQuery().singleResult().Name);
		assertEquals(1, ThrowBPMNErrorDelegate.INVOCATIONS);
	  }

	  public class ThrowBPMNErrorDelegate : ExecutionListener
	  {

		public static int INVOCATIONS = 0;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  INVOCATIONS++;
		  throw new BpmnError(ERROR_CODE, "business error");
		}

		public static void reset()
		{
		  INVOCATIONS = 0;
		}
	  }

	  public class SetsVariableDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  execution.setVariable("foo", "bar");
		}
	  }
	}

}