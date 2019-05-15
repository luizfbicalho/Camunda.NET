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
namespace org.camunda.bpm.engine.test.bpmn.@event.compensate
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ModifiableBpmnModelInstance = org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance;
	using VariableEvent = org.camunda.bpm.engine.test.bpmn.@event.compensate.ReadLocalVariableListener.VariableEvent;
	using BookFlightService = org.camunda.bpm.engine.test.bpmn.@event.compensate.helper.BookFlightService;
	using CancelFlightService = org.camunda.bpm.engine.test.bpmn.@event.compensate.helper.CancelFlightService;
	using GetVariablesDelegate = org.camunda.bpm.engine.test.bpmn.@event.compensate.helper.GetVariablesDelegate;
	using SetVariablesDelegate = org.camunda.bpm.engine.test.bpmn.@event.compensate.helper.SetVariablesDelegate;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CompensateEventTest : PluggableProcessEngineTestCase
	{

	  public virtual void testCompensateOrder()
	  {
		//given two process models, only differ in order of the activities
		const string PROCESS_MODEL_WITH_REF_BEFORE = "org/camunda/bpm/engine/test/bpmn/event/compensate/compensation_reference-before.bpmn";
		const string PROCESS_MODEL_WITH_REF_AFTER = "org/camunda/bpm/engine/test/bpmn/event/compensate/compensation_reference-after.bpmn";

		//when model with ref before is deployed
		org.camunda.bpm.engine.repository.Deployment deployment1 = repositoryService.createDeployment().addClasspathResource(PROCESS_MODEL_WITH_REF_BEFORE).deploy();
		//then no problem will occure

		//when model with ref after is deployed
		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().addClasspathResource(PROCESS_MODEL_WITH_REF_AFTER).deploy();
		//then also no problem should occure

		//clean up
		repositoryService.deleteDeployment(deployment1.Id);
		repositoryService.deleteDeployment(deployment2.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateSubprocess()
	  public virtual void testCompensateSubprocess()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateSubprocessInsideSubprocess()
	  public virtual void testCompensateSubprocessInsideSubprocess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTask("Book Hotel");
		completeTask("Book Flight");

		// throw compensation event
		completeTask("throw compensation");

		// execute compensation handlers
		completeTask("Cancel Hotel");
		completeTask("Cancel Flight");

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateParallelSubprocess()
	  public virtual void testCompensateParallelSubprocess()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));

		Task singleResult = taskService.createTaskQuery().singleResult();
		taskService.complete(singleResult.Id);

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateParallelSubprocessCompHandlerWaitstate()
	  public virtual void testCompensateParallelSubprocessCompHandlerWaitstate()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		IList<Task> compensationHandlerTasks = taskService.createTaskQuery().taskDefinitionKey("undoBookHotel").list();
		assertEquals(5, compensationHandlerTasks.Count);

		ActivityInstance rootActivityInstance = runtimeService.getActivityInstance(processInstance.Id);
		IList<ActivityInstance> compensationHandlerInstances = getInstancesForActivityId(rootActivityInstance, "undoBookHotel");
		assertEquals(5, compensationHandlerInstances.Count);

		foreach (Task task in compensationHandlerTasks)
		{
		  taskService.complete(task.Id);
		}

		Task singleResult = taskService.createTaskQuery().singleResult();
		taskService.complete(singleResult.Id);

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensateParallelSubprocessCompHandlerWaitstate.bpmn20.xml")]
	  public virtual void testDeleteParallelSubprocessCompHandlerWaitstate()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		// five inner tasks
		IList<Task> compensationHandlerTasks = taskService.createTaskQuery().taskDefinitionKey("undoBookHotel").list();
		assertEquals(5, compensationHandlerTasks.Count);

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, "");

		// then the process has been removed
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateMiSubprocess()
	  public virtual void testCompensateMiSubprocess()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateScope()
	  public virtual void testCompensateScope()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookFlight"));

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

	  }

	  // See: https://app.camunda.com/jira/browse/CAM-1410
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateActivityRef()
	  public virtual void testCompensateActivityRef()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		assertEquals(5, runtimeService.getVariable(processInstance.Id, "undoBookHotel"));
		assertNull(runtimeService.getVariable(processInstance.Id, "undoBookFlight"));

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

	  }

	  /// <summary>
	  /// CAM-3628
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateSubprocessWithBoundaryEvent()
	  public virtual void testCompensateSubprocessWithBoundaryEvent()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("compensateProcess");

		Task compensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationTask);
		assertEquals("undoSubprocess", compensationTask.TaskDefinitionKey);

		taskService.complete(compensationTask.Id);
		runtimeService.signal(instance.Id);
		assertProcessEnded(instance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateActivityInSubprocess()
	  public virtual void testCompensateActivityInSubprocess()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("compensateProcess");

		Task scopeTask = taskService.createTaskQuery().singleResult();
		taskService.complete(scopeTask.Id);

		// process has not yet thrown compensation
		// when throw compensation
		runtimeService.signal(instance.Id);
		// then
		Task compensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationTask);
		assertEquals("undoScopeTask", compensationTask.TaskDefinitionKey);

		taskService.complete(compensationTask.Id);
		runtimeService.signal(instance.Id);
		assertProcessEnded(instance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateActivityInConcurrentSubprocess()
	  public virtual void testCompensateActivityInConcurrentSubprocess()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("compensateProcess");

		Task scopeTask = taskService.createTaskQuery().taskDefinitionKey("scopeTask").singleResult();
		taskService.complete(scopeTask.Id);

		Task outerTask = taskService.createTaskQuery().taskDefinitionKey("outerTask").singleResult();
		taskService.complete(outerTask.Id);

		// process has not yet thrown compensation
		// when throw compensation
		runtimeService.signal(instance.Id);

		// then
		Task compensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(compensationTask);
		assertEquals("undoScopeTask", compensationTask.TaskDefinitionKey);

		taskService.complete(compensationTask.Id);
		runtimeService.signal(instance.Id);
		assertProcessEnded(instance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateConcurrentMiActivity()
	  public virtual void testCompensateConcurrentMiActivity()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		// complete 4 of 5 user tasks
		completeTasks("Book Hotel", 4);

		// throw compensation event
		completeTaskWithVariable("Request Vacation", "accept", false);

		// should not compensate activity before multi instance activity is completed
		assertEquals(0, taskService.createTaskQuery().taskName("Cancel Hotel").count());

		// complete last open task and end process instance
		completeTask("Book Hotel");
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateConcurrentMiSubprocess()
	  public virtual void testCompensateConcurrentMiSubprocess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		// complete 4 of 5 user tasks
		completeTasks("Book Hotel", 4);

		// throw compensation event
		completeTaskWithVariable("Request Vacation", "accept", false);

		// should not compensate activity before multi instance activity is completed
		assertEquals(0, taskService.createTaskQuery().taskName("Cancel Hotel").count());

		// complete last open task and end process instance
		completeTask("Book Hotel");

		runtimeService.signal(processInstanceId);
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateActivityRefMiActivity()
	  public virtual void testCompensateActivityRefMiActivity()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTasks("Book Hotel", 5);

		// throw compensation event for activity
		completeTaskWithVariable("Request Vacation", "accept", false);

		// execute compensation handlers for each execution of the subprocess
		assertEquals(5, taskService.createTaskQuery().count());
		completeTasks("Cancel Hotel", 5);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateActivityRefMiSubprocess()
	  public virtual void testCompensateActivityRefMiSubprocess()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTasks("Book Hotel", 5);

		// throw compensation event for activity
		completeTaskWithVariable("Request Vacation", "accept", false);

		// execute compensation handlers for each execution of the subprocess
		assertEquals(5, taskService.createTaskQuery().count());
		completeTasks("Cancel Hotel", 5);

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCallActivityCompensationHandler.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensationHandler.bpmn20.xml" })]
	  public virtual void testCallActivityCompensationHandler()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		}

		runtimeService.signal(processInstance.Id);
		assertProcessEnded(processInstance.Id);

		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(6, historyService.createHistoricProcessInstanceQuery().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateMiSubprocessVariableSnapshots()
	  public virtual void testCompensateMiSubprocessVariableSnapshots()
	  {
		// see referenced java delegates in the process definition.

		IList<string> hotels = Arrays.asList("Rupert", "Vogsphere", "Milliways", "Taunton", "Ysolldins");

		SetVariablesDelegate.Values = hotels;

		// SetVariablesDelegate take the first element of static list and set the value as local variable
		// GetVariablesDelegate read the variable and add the value to static list

		runtimeService.startProcessInstanceByKey("compensateProcess");

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertTrue(GetVariablesDelegate.values.containsAll(hotels));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateMiSubprocessWithCompensationEventSubprocessVariableSnapshots()
	  public virtual void testCompensateMiSubprocessWithCompensationEventSubprocessVariableSnapshots()
	  {
		// see referenced java delegates in the process definition.

		IList<string> hotels = Arrays.asList("Rupert", "Vogsphere", "Milliways", "Taunton", "Ysolldins");

		SetVariablesDelegate.Values = hotels;

		// SetVariablesDelegate take the first element of static list and set the value as local variable
		// GetVariablesDelegate read the variable and add the value to static list

		runtimeService.startProcessInstanceByKey("compensateProcess");

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertTrue(GetVariablesDelegate.values.containsAll(hotels));
	  }

	  /// <summary>
	  /// enable test case when bug is fixed
	  /// </summary>
	  /// <seealso cref= https://app.camunda.com/jira/browse/CAM-4268 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testCompensateMiSubprocessVariableSnapshotOfElementVariable()
	  public virtual void FAILING_testCompensateMiSubprocessVariableSnapshotOfElementVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		// multi instance collection
		IList<string> flights = Arrays.asList("STS-14", "STS-28");
		variables["flights"] = flights;

		// see referenced java delegates in the process definition
		// java delegates read element variable (flight) and add the variable value
		// to a static list
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess", variables);

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(flights.Count, historyService.createHistoricActivityInstanceQuery().activityId("undoBookFlight").count());
		}

		// java delegates should be invoked for each element in collection
		assertEquals(flights, BookFlightService.bookedFlights);
		assertEquals(flights, CancelFlightService.canceledFlights);

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationTriggeredByEventSubProcessActivityRef.bpmn20.xml" })]
	  public virtual void testCompensateActivityRefTriggeredByEventSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstanceQuery historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookHotel");

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals("undoBookHotel", historicVariableInstanceQuery.list().get(0).VariableName);
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);

		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookFlight").count());
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationTriggeredByEventSubProcessInSubProcessActivityRef.bpmn20.xml" })]
	  public virtual void testCompensateActivityRefTriggeredByEventSubprocessInSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstanceQuery historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookHotel");

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals("undoBookHotel", historicVariableInstanceQuery.list().get(0).VariableName);
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);

		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookFlight").count());
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationInEventSubProcessActivityRef.bpmn20.xml" })]
	  public virtual void testCompensateActivityRefInEventSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstanceQuery historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("undoBookSecondHotel");

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals("undoBookSecondHotel", historicVariableInstanceQuery.list().get(0).VariableName);
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);

		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookFlight").count());

		  assertEquals(0, historyService.createHistoricVariableInstanceQuery().processInstanceId(processInstance.Id).variableName("undoBookHotel").count());
		}
	  }

	  /// <summary>
	  /// enable test case when bug is fixed
	  /// </summary>
	  /// <seealso cref= https://app.camunda.com/jira/browse/CAM-4304 </seealso>
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationInEventSubProcess.bpmn20.xml" })]
	  public virtual void testCompensateInEventSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");
		assertProcessEnded(processInstance.Id);

		HistoricVariableInstanceQuery historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("undoBookSecondHotel");

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals("undoBookSecondHotel", historicVariableInstanceQuery.list().get(0).VariableName);
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);

		  historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("undoBookFlight");

		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);

		  historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("undoBookHotel");

		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals(5, historicVariableInstanceQuery.list().get(0).Value);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListeners()
	  public virtual void testExecutionListeners()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["start"] = 0;
		variables["end"] = 0;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess", variables);

		int started = (int?) runtimeService.getVariable(processInstance.Id, "start").Value;
		assertEquals(5, started);

		int ended = (int?) runtimeService.getVariable(processInstance.Id, "end").Value;
		assertEquals(5, ended);

		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  long finishedCount = historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").finished().count();
		  assertEquals(5, finishedCount);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testActivityInstanceTreeWithoutEventScope()
	  public virtual void testActivityInstanceTreeWithoutEventScope()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = instance.Id;

		// when
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		// then
		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentExecutionsAndPendingCompensation()
	  public virtual void testConcurrentExecutionsAndPendingCompensation()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("process");
		string processInstanceId = instance.Id;
		string taskId = taskService.createTaskQuery().taskDefinitionKey("innerTask").singleResult().Id;

		// when (1)
		taskService.complete(taskId);

		// then (1)
		ExecutionTree executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree(null).scope().child("task1").concurrent().noScope().up().child("task2").concurrent().noScope().up().child("subProcess").eventScope().scope().up().done());

		ActivityInstance tree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task1").activity("task2").done());

		// when (2)
		taskId = taskService.createTaskQuery().taskDefinitionKey("task1").singleResult().Id;
		taskService.complete(taskId);

		// then (2)
		executionTree = ExecutionTree.forExecution(processInstanceId, processEngine);
		assertThat(executionTree).matches(describeExecutionTree("task2").scope().child("subProcess").eventScope().scope().up().done());

		tree = runtimeService.getActivityInstance(processInstanceId);
		assertThat(tree).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("task2").done());

		// when (3)
		taskId = taskService.createTaskQuery().taskDefinitionKey("task2").singleResult().Id;
		taskService.complete(taskId);

		// then (3)
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEndEventWithScope()
	  public virtual void testCompensationEndEventWithScope()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookFlight").count());
		}

		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEndEventWithActivityRef()
	  public virtual void testCompensationEndEventWithActivityRef()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HISTORY_NONE))
		{
		  assertEquals(5, historyService.createHistoricActivityInstanceQuery().activityId("undoBookHotel").count());
		  assertEquals(0, historyService.createHistoricActivityInstanceQuery().activityId("undoBookFlight").count());
		}

		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.activityWithCompensationEndEvent.bpmn20.xml")]
	  public virtual void testActivityInstanceTreeForCompensationEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("end").activity("undoBookHotel").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.compensationMiActivity.bpmn20.xml")]
	  public virtual void testActivityInstanceTreeForMiActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("end").beginMiBody("bookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensateParallelSubprocessCompHandlerWaitstate.bpmn20.xml")]
	  public virtual void testActivityInstanceTreeForParallelMiActivityInSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("parallelTask").activity("throwCompensate").beginScope("scope").beginMiBody("bookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.compensationMiSubprocess.bpmn20.xml")]
	  public virtual void testActivityInstanceTreeForMiSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		completeTasks("Book Hotel", 5);
		// throw compensation event
		completeTask("throwCompensation");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("throwingCompensation").beginMiBody("scope").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").activity("undoBookHotel").done());
	  }

	  /// <summary>
	  /// CAM-4903
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testActivityInstanceTreeForMiSubProcessDefaultHandler()
	  public virtual void FAILING_testActivityInstanceTreeForMiSubProcessDefaultHandler()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		completeTasks("Book Hotel", 5);
		// throw compensation event
		completeTask("throwCompensation");

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("throwingCompensation").beginMiBody("scope").beginScope("scope").activity("undoBookHotel").endScope().beginScope("scope").activity("undoBookHotel").endScope().beginScope("scope").activity("undoBookHotel").endScope().beginScope("scope").activity("undoBookHotel").endScope().beginScope("scope").activity("undoBookHotel").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.activityWithCompensationEndEvent.bpmn20.xml")]
	  public virtual void testCancelProcessInstanceWithActiveCompensation()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("compensateProcess");

		// when
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		assertProcessEnded(processInstance.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationEventSubProcess.bpmn20.xml" })]
	  public virtual void testCompensationEventSubProcessWithScope()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("bookingProcess").Id;

		completeTask("Book Flight");
		completeTask("Book Hotel");

		// throw compensation event for current scope (without activityRef)
		completeTaskWithVariable("Validate Booking", "valid", false);

		// first - compensate book flight
		assertEquals(1, taskService.createTaskQuery().count());
		completeTask("Cancel Flight");
		// second - compensate book hotel
		assertEquals(1, taskService.createTaskQuery().count());
		completeTask("Cancel Hotel");
		// third - additional compensation handler
		completeTask("Update Customer Record");

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEventSubProcessWithActivityRef()
	  public virtual void testCompensationEventSubProcessWithActivityRef()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("bookingProcess").Id;

		completeTask("Book Hotel");
		completeTask("Book Flight");

		// throw compensation event for specific scope (with activityRef = subprocess)
		completeTaskWithVariable("Validate Booking", "valid", false);

		// compensate the activity within this scope
		assertEquals(1, taskService.createTaskQuery().count());
		completeTask("Cancel Hotel");

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testCompensationEventSubProcess.bpmn20.xml" })]
	  public virtual void testActivityInstanceTreeForCompensationEventSubProcess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("bookingProcess");

		completeTask("Book Flight");
		completeTask("Book Hotel");

		// throw compensation event
		completeTaskWithVariable("Validate Booking", "valid", false);

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("throwCompensation").beginScope("booking-subprocess").activity("cancelFlight").beginScope("compensationSubProcess").activity("compensateFlight").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateMiSubprocessWithCompensationEventSubProcess()
	  public virtual void testCompensateMiSubprocessWithCompensationEventSubProcess()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		// multi instance collection
		variables["flights"] = Arrays.asList("STS-14", "STS-28");

		string processInstanceId = runtimeService.startProcessInstanceByKey("bookingProcess", variables).Id;

		completeTask("Book Flight");
		completeTask("Book Hotel");

		completeTask("Book Flight");
		completeTask("Book Hotel");

		// throw compensation event
		completeTaskWithVariable("Validate Booking", "valid", false);

		// execute compensation handlers for each execution of the subprocess
		completeTasks("Cancel Flight", 2);
		completeTasks("Cancel Hotel", 2);
		completeTasks("Update Customer Record", 2);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensateParallelMiSubprocessWithCompensationEventSubProcess()
	  public virtual void testCompensateParallelMiSubprocessWithCompensationEventSubProcess()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		// multi instance collection
		variables["flights"] = Arrays.asList("STS-14", "STS-28");

		string processInstanceId = runtimeService.startProcessInstanceByKey("bookingProcess", variables).Id;

		completeTasks("Book Flight", 2);
		completeTasks("Book Hotel", 2);

		// throw compensation event
		completeTaskWithVariable("Validate Booking", "valid", false);

		// execute compensation handlers for each execution of the subprocess
		completeTasks("Cancel Flight", 2);
		completeTasks("Cancel Hotel", 2);
		completeTasks("Update Customer Record", 2);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEventSubprocessWithoutBoundaryEvents()
	  public virtual void testCompensationEventSubprocessWithoutBoundaryEvents()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTask("Book Hotel");
		completeTask("Book Flight");

		// throw compensation event
		completeTask("throw compensation");

		// execute compensation handlers
		completeTask("Cancel Flight");
		completeTask("Cancel Hotel");

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEventSubprocessReThrowCompensationEvent()
	  public virtual void testCompensationEventSubprocessReThrowCompensationEvent()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTask("Book Hotel");
		completeTask("Book Flight");

		// throw compensation event
		completeTask("throw compensation");

		// execute compensation handler and re-throw compensation event
		completeTask("Cancel Hotel");
		// execute compensation handler at subprocess
		completeTask("Cancel Flight");

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompensationEventSubprocessConsumeCompensationEvent()
	  public virtual void testCompensationEventSubprocessConsumeCompensationEvent()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("compensateProcess").Id;

		completeTask("Book Hotel");
		completeTask("Book Flight");

		// throw compensation event
		completeTask("throw compensation");

		// execute compensation handler and consume compensation event
		completeTask("Cancel Hotel");
		// compensation handler at subprocess (Cancel Flight) should not be executed
		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSubprocessCompensationHandler()
	  public virtual void testSubprocessCompensationHandler()
	  {

		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessCompensationHandler");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// then the compensation handler has been activated
		// and the user task in the sub process can be successfully completed
		Task subProcessTask = taskService.createTaskQuery().singleResult();
		assertNotNull(subProcessTask);
		assertEquals("subProcessTask", subProcessTask.TaskDefinitionKey);

		taskService.complete(subProcessTask.Id);

		// and the task following compensation can be successfully completed
		Task afterCompensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(afterCompensationTask);
		assertEquals("beforeEnd", afterCompensationTask.TaskDefinitionKey);

		taskService.complete(afterCompensationTask.Id);

		// and the process has successfully ended
		assertProcessEnded(processInstance.Id);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testSubprocessCompensationHandler.bpmn20.xml")]
	  public virtual void testSubprocessCompensationHandlerActivityInstanceTree()
	  {

		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessCompensationHandler");

		// when throwing compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// then the activity instance tree is correct
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("throwCompensate").beginScope("compensationHandler").activity("subProcessTask").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testSubprocessCompensationHandler.bpmn20.xml")]
	  public virtual void testSubprocessCompensationHandlerDeleteProcessInstance()
	  {

		// given a process instance in compensation
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessCompensationHandler");
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// when deleting the process instance
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then the process instance is ended
		assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// CAM-4387
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testSubprocessCompensationHandlerWithEventSubprocess()
	  public virtual void FAILING_testSubprocessCompensationHandlerWithEventSubprocess()
	  {
		// given a process instance in compensation
		runtimeService.startProcessInstanceByKey("subProcessCompensationHandlerWithEventSubprocess");
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// when the event subprocess is triggered that is defined as part of the compensation handler
		runtimeService.correlateMessage("Message");

		// then activity instance tree is correct
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("eventSubProcessTask", task.TaskDefinitionKey);
	  }

	  /// <summary>
	  /// CAM-4387
	  /// </summary>
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.testSubprocessCompensationHandlerWithEventSubprocess.bpmn20.xml")]
	  public virtual void FAILING_testSubprocessCompensationHandlerWithEventSubprocessActivityInstanceTree()
	  {
		// given a process instance in compensation
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("subProcessCompensationHandlerWithEventSubprocess");
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// when the event subprocess is triggered that is defined as part of the compensation handler
		runtimeService.correlateMessage("Message");

		// then the event subprocess has been triggered
		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("throwCompensate").beginScope("compensationHandler").beginScope("eventSubProcess").activity("eventSubProcessTask").done());
	  }

	  /// <summary>
	  /// CAM-4387
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testReceiveTaskCompensationHandler()
	  public virtual void FAILING_testReceiveTaskCompensationHandler()
	  {
		// given a process instance
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("receiveTaskCompensationHandler");

		// when triggering compensation
		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();
		taskService.complete(beforeCompensationTask.Id);

		// then there is a message event subscription for the receive task compensation handler
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertNotNull(eventSubscription);
		assertEquals(EventType.MESSAGE, eventSubscription.EventType);

		// and triggering the message completes compensation
		runtimeService.correlateMessage("Message");

		Task afterCompensationTask = taskService.createTaskQuery().singleResult();
		assertNotNull(afterCompensationTask);
		assertEquals("beforeEnd", afterCompensationTask.TaskDefinitionKey);

		taskService.complete(afterCompensationTask.Id);

		// and the process has successfully ended
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentScopeCompensation()
	  public virtual void testConcurrentScopeCompensation()
	  {
		// given a process instance with two concurrent tasks, one of which is waiting
		// before throwing compensation
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("concurrentScopeCompensation");
		Task beforeCompensationTask = taskService.createTaskQuery().taskDefinitionKey("beforeCompensationTask").singleResult();
		Task concurrentTask = taskService.createTaskQuery().taskDefinitionKey("concurrentTask").singleResult();

		// when throwing compensation such that two subprocesses are compensated
		taskService.complete(beforeCompensationTask.Id);

		// then both compensation handlers have been executed
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricVariableInstanceQuery historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("compensateScope1Task");

		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals(1, historicVariableInstanceQuery.list().get(0).Value);

		  historicVariableInstanceQuery = historyService.createHistoricVariableInstanceQuery().variableName("compensateScope2Task");

		  assertEquals(1, historicVariableInstanceQuery.count());
		  assertEquals(1, historicVariableInstanceQuery.list().get(0).Value);
		}

		// and after completing the concurrent task, the process instance ends successfully
		taskService.complete(concurrentTask.Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLocalVariablesInEndExecutionListener()
	  public virtual void testLocalVariablesInEndExecutionListener()
	  {
		// given
		SetLocalVariableListener setListener = new SetLocalVariableListener("foo", "bar");
		ReadLocalVariableListener readListener = new ReadLocalVariableListener("foo");

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", Variables.createVariables().putValue("setListener", setListener).putValue("readListener", readListener));

		Task beforeCompensationTask = taskService.createTaskQuery().singleResult();

		// when executing the compensation handler
		taskService.complete(beforeCompensationTask.Id);

		// then the variable listener has been invoked and was able to read the variable on the end event
		readListener = (ReadLocalVariableListener) runtimeService.getVariable(processInstance.Id, "readListener");

		Assert.assertEquals(1, readListener.VariableEvents.Count);

		VariableEvent @event = readListener.VariableEvents[0];
		Assert.assertEquals("foo", @event.VariableName);
		Assert.assertEquals("bar", @event.VariableValue);
	  }

	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void FAILING_testDeleteInstanceWithEventScopeExecution()
	  {
		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("foo").startEvent("start").subProcess("subProcess").embeddedSubProcess().startEvent("subProcessStart").endEvent("subProcessEnd").subProcessDone().userTask("userTask").done();

		modelInstance = ModifiableBpmnModelInstance.modify(modelInstance).addSubProcessTo("subProcess").id("eventSubProcess").triggerByEvent().embeddedSubProcess().startEvent().compensateEventDefinition().compensateEventDefinitionDone().endEvent().done();

		deployment(modelInstance);

		long dayInMillis = 1000 * 60 * 60 * 24;
		DateTime date1 = new DateTime(10 * dayInMillis);
		ClockUtil.CurrentTime = date1;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("foo");

		// when
		DateTime date2 = new DateTime(date1.Ticks + dayInMillis);
		ClockUtil.CurrentTime = date2;
		runtimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		IList<HistoricActivityInstance> historicActivityInstance = historyService.createHistoricActivityInstanceQuery().orderByActivityId().asc().list();
		assertEquals(5, historicActivityInstance.Count);

		assertEquals("start", historicActivityInstance[0].ActivityId);
		assertEquals(date1, historicActivityInstance[0].EndTime);
		assertEquals("subProcess", historicActivityInstance[1].ActivityId);
		assertEquals(date1, historicActivityInstance[1].EndTime);
		assertEquals("subProcessEnd", historicActivityInstance[2].ActivityId);
		assertEquals(date1, historicActivityInstance[2].EndTime);
		assertEquals("subProcessStart", historicActivityInstance[3].ActivityId);
		assertEquals(date1, historicActivityInstance[3].EndTime);
		assertEquals("userTask", historicActivityInstance[4].ActivityId);
		assertEquals(date2, historicActivityInstance[4].EndTime);


	  }

	  private void completeTask(string taskName)
	  {
		completeTasks(taskName, 1);
	  }

	  private void completeTasks(string taskName, int times)
	  {
		IList<Task> tasks = taskService.createTaskQuery().taskName(taskName).list();

		assertTrue("Actual there are " + tasks.Count + " open tasks with name '" + taskName + "'. Expected at least " + times, times <= tasks.Count);

		IEnumerator<Task> taskIterator = tasks.GetEnumerator();
		for (int i = 0; i < times; i++)
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  Task task = taskIterator.next();
		  taskService.complete(task.Id);
		}
	  }

	  private void completeTaskWithVariable(string taskName, string variable, object value)
	  {
		Task task = taskService.createTaskQuery().taskName(taskName).singleResult();
		assertNotNull("No open task with name '" + taskName + "'", task);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		if (!string.ReferenceEquals(variable, null))
		{
		  variables[variable] = value;
		}

		taskService.complete(task.Id, variables);
	  }

	}

}