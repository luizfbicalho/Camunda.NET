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
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
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


	  public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

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
	  public static readonly BpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER = Bpmn.createExecutableProcess("Process").startEvent().parallelGateway("fork").userTask("userTask1").serviceTask("sendTask").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SendMessageDelegate).FullName).camundaExpression("${true}").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).moveToLastGateway().userTask("userTask2").boundaryEvent("boundaryEvent").message(MESSAGE).endEvent("endBoundaryEvent").moveToNode("userTask2").endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskExecutionListenerCall()
	  public virtual void testServiceTaskExecutionListenerCall()
	  {
		testHelper.deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER);
		runtimeService.startProcessInstanceByKey("Process");
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
		runtimeService.startProcessInstanceByKey("Process");
		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.complete(task.Id);

		assertEquals(0, taskService.createTaskQuery().list().size());
		IList<RecordedEvent> recordedEvents = RecorderExecutionListener.RecordedEvents;
		assertEquals(2, recordedEvents.Count);
		assertEquals("sendTask", recordedEvents[0].ActivityId);
		assertEquals("endEvent", recordedEvents[1].ActivityId);
	  }

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS = modify(Bpmn.createExecutableProcess("Process").startEvent().userTask("userTask").serviceTask("sendTask").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(SendMessageDelegate).FullName).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).camundaExpression("${true}").endEvent("endEvent").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).done()).addSubProcessTo("Process").triggerByEvent().embeddedSubProcess().startEvent("startSubProcess").interrupting(false).camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).message(MESSAGE).userTask("subProcessTask").camundaExecutionListenerClass(RecorderExecutionListener.EVENTNAME_START, typeof(RecorderExecutionListener).FullName).endEvent("endSubProcess").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testServiceTaskExecutionListenerCallAndSubProcess()
	  public virtual void testServiceTaskExecutionListenerCallAndSubProcess()
	  {
		testHelper.deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS);
		runtimeService.startProcessInstanceByKey("Process");
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
	}

}