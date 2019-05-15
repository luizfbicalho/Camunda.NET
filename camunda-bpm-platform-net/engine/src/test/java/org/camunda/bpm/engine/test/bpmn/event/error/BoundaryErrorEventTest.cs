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
namespace org.camunda.bpm.engine.test.bpmn.@event.error
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate.throwError;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.bpmn.@event.error.ThrowErrorDelegate.throwException;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public class BoundaryErrorEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		// Normally the UI will do this automatically for us
		identityService.AuthenticatedUserId = "kermit";
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		identityService.clearAuthentication();
		base.tearDown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnEmbeddedSubprocess()
	  public virtual void testCatchErrorOnEmbeddedSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("boundaryErrorOnEmbeddedSubprocess");

		// After process start, usertask in subprocess should exist
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("subprocessTask", task.Name);

		// After task completion, error end event is reached and caught
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("task after catching the error", task.Name);
	  }

	  public virtual void testThrowErrorWithoutErrorCode()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testThrowErrorWithoutErrorCode.bpmn20.xml").deploy();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("'errorCode' is mandatory on errors referenced by throwing error event definitions", re.Message);
		}
	  }

	  public virtual void testThrowErrorWithEmptyErrorCode()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testThrowErrorWithEmptyErrorCode.bpmn20.xml").deploy();
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("'errorCode' is mandatory on errors referenced by throwing error event definitions", re.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnEmbeddedSubprocessWithEmptyErrorCode()
	  public virtual void testCatchErrorOnEmbeddedSubprocessWithEmptyErrorCode()
	  {
		testCatchErrorOnEmbeddedSubprocess();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnEmbeddedSubprocessWithoutErrorCode()
	  public virtual void testCatchErrorOnEmbeddedSubprocessWithoutErrorCode()
	  {
		testCatchErrorOnEmbeddedSubprocess();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOfInnerSubprocessOnOuterSubprocess()
	  public virtual void testCatchErrorOfInnerSubprocessOnOuterSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("boundaryErrorTest");

		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("Inner subprocess task 1", tasks[0].Name);
		assertEquals("Inner subprocess task 2", tasks[1].Name);

		// Completing task 2, will cause the end error event to throw error with code 123
		taskService.complete(tasks[1].Id);
		tasks = taskService.createTaskQuery().list();
		Task taskAfterError = taskService.createTaskQuery().singleResult();
		assertEquals("task outside subprocess", taskAfterError.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorInConcurrentEmbeddedSubprocesses()
	  public virtual void testCatchErrorInConcurrentEmbeddedSubprocesses()
	  {
		assertErrorCaughtInConcurrentEmbeddedSubprocesses("boundaryEventTestConcurrentSubprocesses");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorInConcurrentEmbeddedSubprocessesThrownByScriptTask()
	  public virtual void testCatchErrorInConcurrentEmbeddedSubprocessesThrownByScriptTask()
	  {
		assertErrorCaughtInConcurrentEmbeddedSubprocesses("catchErrorInConcurrentEmbeddedSubprocessesThrownByScriptTask");
	  }

	  private void assertErrorCaughtInConcurrentEmbeddedSubprocesses(string processDefinitionKey)
	  {
		// Completing task A will lead to task D
		string procId = runtimeService.startProcessInstanceByKey(processDefinitionKey).Id;
		IList<Task> tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("task A", tasks[0].Name);
		assertEquals("task B", tasks[1].Name);
		taskService.complete(tasks[0].Id);
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("task D", task.Name);
		taskService.complete(task.Id);
		assertProcessEnded(procId);

		// Completing task B will lead to task C
		procId = runtimeService.startProcessInstanceByKey(processDefinitionKey).Id;
		tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("task A", tasks[0].Name);
		assertEquals("task B", tasks[1].Name);
		taskService.complete(tasks[1].Id);

		tasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals(2, tasks.Count);
		assertEquals("task A", tasks[0].Name);
		assertEquals("task C", tasks[1].Name);
		taskService.complete(tasks[1].Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("task A", task.Name);

		taskService.complete(task.Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("task D", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeeplyNestedErrorThrown()
	  public virtual void testDeeplyNestedErrorThrown()
	  {

		// Input = 1 -> error1 will be thrown, which will destroy ALL BUT ONE
		// subprocess, which leads to an end event, which ultimately leads to ending the process instance
		string procId = runtimeService.startProcessInstanceByKey("deeplyNestedErrorThrown").Id;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Nested task", task.Name);
		taskService.complete(task.Id, CollectionUtil.singletonMap("input", 1));
		assertProcessEnded(procId);

		// Input == 2 -> error2 will be thrown, leading to a userTask outside all subprocesses
		procId = runtimeService.startProcessInstanceByKey("deeplyNestedErrorThrown").Id;
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Nested task", task.Name);
		taskService.complete(task.Id, CollectionUtil.singletonMap("input", 2));
		task = taskService.createTaskQuery().singleResult();
		assertEquals("task after catch", task.Name);
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDeeplyNestedErrorThrownOnlyAutomaticSteps()
	  public virtual void testDeeplyNestedErrorThrownOnlyAutomaticSteps()
	  {
		// input == 1 -> error2 is thrown -> caught on subprocess2 -> end event in subprocess -> proc inst end 1
		string procId = runtimeService.startProcessInstanceByKey("deeplyNestedErrorThrown", CollectionUtil.singletonMap("input", 1)).Id;
		assertProcessEnded(procId);

		HistoricProcessInstance hip;
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  hip = historyService.createHistoricProcessInstanceQuery().processInstanceId(procId).singleResult();
		  assertEquals("processEnd1", hip.EndActivityId);
		}
		// input == 2 -> error2 is thrown -> caught on subprocess1 -> proc inst end 2
		procId = runtimeService.startProcessInstanceByKey("deeplyNestedErrorThrown", CollectionUtil.singletonMap("input", 1)).Id;
		assertProcessEnded(procId);

		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  hip = historyService.createHistoricProcessInstanceQuery().processInstanceId(procId).singleResult();
		  assertEquals("processEnd1", hip.EndActivityId);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorOnCallActivity-parent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void testCatchErrorOnCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorOnCallActivity").Id;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		// Completing the task will reach the end error event,
		// which is caught on the call activity boundary
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// Completing the task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorOnCallActivity-parent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void FAILING_testCatchErrorOnCallActivityShouldEndCalledProcessProperly()
	  {
		// given a process instance that has instantiated (called) a sub process instance
		runtimeService.startProcessInstanceByKey("catchErrorOnCallActivity").Id;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		// when an error end event is triggered in the sub process instance and catched in the super process instance
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// then the called historic process instance should have properly ended
		HistoricProcessInstance historicSubProcessInstance = historyService.createHistoricProcessInstanceQuery().processDefinitionKey("simpleSubProcess").singleResult();
		assertNotNull(historicSubProcessInstance);
		assertNull(historicSubProcessInstance.DeleteReason);
		assertEquals("theEnd", historicSubProcessInstance.EndActivityId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void testUncaughtError()
	  {
		runtimeService.startProcessInstanceByKey("simpleSubProcess");
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		try
		{
		  // Completing the task will reach the end error event,
		  // which is never caught in the process
		  taskService.complete(task.Id);
		}
		catch (BpmnError e)
		{
		  assertTextPresent("No catching boundary event found for error with errorCode 'myError', neither in same process nor in parent process", e.Message);
		}
	  }


	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testUncaughtErrorOnCallActivity-parent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void testUncaughtErrorOnCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("uncaughtErrorOnCallActivity");
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		try
		{
		  // Completing the task will reach the end error event,
		  // which is never caught in the process
		  taskService.complete(task.Id);
		}
		catch (BpmnError e)
		{
		  assertTextPresent("No catching boundary event found for error with errorCode 'myError', neither in same process nor in parent process", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnSubprocess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByCallActivityOnSubprocess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorOnSubprocess").Id;
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Task in subprocess", task.Name);

		// Completing the task will reach the end error event,
		// which is caught on the call activity boundary
		taskService.complete(task.Id);
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// Completing the task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess2ndLevel.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" }) public void testCatchErrorThrownByCallActivityOnCallActivity() throws InterruptedException
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess2ndLevel.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.subprocess.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByCallActivityOnCallActivity()
	  {
		  string procId = runtimeService.startProcessInstanceByKey("catchErrorOnCallActivity2ndLevel").Id;

		  Task task = taskService.createTaskQuery().singleResult();
		  assertEquals("Task in subprocess", task.Name);

		  taskService.complete(task.Id);

		  task = taskService.createTaskQuery().singleResult();
		  assertEquals("Escalated Task", task.Name);

		  // Completing the task will end the process instance
		  taskService.complete(task.Id);
		  assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnParallelMultiInstance()
	  public virtual void testCatchErrorOnParallelMultiInstance()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorOnParallelMi").Id;
		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(5, tasks.Count);

		// Complete two subprocesses, just to make it a bit more complex
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["throwError"] = false;
		taskService.complete(tasks[2].Id, vars);
		taskService.complete(tasks[3].Id, vars);

		// Reach the error event
		vars["throwError"] = true;
		taskService.complete(tasks[1].Id, vars);

		assertEquals(0, taskService.createTaskQuery().count());
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnSequentialMultiInstance()
	  public virtual void testCatchErrorOnSequentialMultiInstance()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorOnSequentialMi").Id;

		// complete one task
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["throwError"] = false;
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id, vars);

		// complete second task and throw error
		vars["throwError"] = true;
		task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id, vars);

		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownBySignallableActivityBehaviour()
	  public virtual void testCatchErrorThrownBySignallableActivityBehaviour()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownBySignallableActivityBehaviour").Id;
		assertNotNull("Didn't get a process id from runtime service", procId);
		ActivityInstance processActivityInstance = runtimeService.getActivityInstance(procId);
		ActivityInstance serviceTask = processActivityInstance.ChildActivityInstances[0];
		assertEquals("Expected the service task to be active after starting the process", "serviceTask", serviceTask.ActivityId);
		runtimeService.signal(serviceTask.ExecutionIds[0]);
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnServiceTask()
	  public virtual void testCatchErrorThrownByJavaDelegateOnServiceTask()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTask").Id;
		assertThatErrorHasBeenCaught(procId);

		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTask", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnServiceTaskNotCancelActivity()
	  public virtual void testCatchErrorThrownByJavaDelegateOnServiceTaskNotCancelActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTaskNotCancelActiviti").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnServiceTaskWithErrorCode()
	  public virtual void testCatchErrorThrownByJavaDelegateOnServiceTaskWithErrorCode()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnServiceTaskWithErrorCode").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcess()
	  public virtual void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcess").Id;
		assertThatErrorHasBeenCaught(procId);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcess", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcessInduction()
	  public virtual void testCatchErrorThrownByJavaDelegateOnEmbeddedSubProcessInduction()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnEmbeddedSubProcessInduction").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-parent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByJavaDelegateOnCallActivity()
	  {
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-parent").Id;
		assertThatErrorHasBeenCaught(procId);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-parent", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
	  public virtual void testUncaughtErrorThrownByJavaDelegateOnServiceTask()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnCallActivity-child");
		}
		catch (BpmnError e)
		{
		  assertTextPresent("No catching boundary event found for error with errorCode '23', neither in same process nor in parent process", e.Message);
		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwException()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess", throwError()).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalMethodOfAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchExceptionExpressionThrownByFollowUpTask()
	  public virtual void testCatchExceptionExpressionThrownByFollowUpTask()
	  {
		try
		{
		  IDictionary<string, object> vars = throwException();
		  runtimeService.startProcessInstanceByKey("testProcess", vars).Id;
		  fail("should fail and not catch the error on the first task");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		assertNull(taskService.createTaskQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchExceptionClassDelegateThrownByFollowUpTask()
	  public virtual void testCatchExceptionClassDelegateThrownByFollowUpTask()
	  {
		try
		{
		  IDictionary<string, object> vars = throwException();
		  runtimeService.startProcessInstanceByKey("testProcess", vars).Id;
		  fail("should fail");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}

		assertNull(taskService.createTaskQuery().singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchExceptionExpressionThrownByFollowUpScopeTask()
	  public virtual void testCatchExceptionExpressionThrownByFollowUpScopeTask()
	  {
		try
		{
		  IDictionary<string, object> vars = throwException();
		  runtimeService.startProcessInstanceByKey("testProcess", vars).Id;
		  fail("should fail and not catch the error on the first task");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
		assertNull(taskService.createTaskQuery().singleResult());
	  }


	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfAbstractBpmnActivityBehavior()
	  {
		string pi = runtimeService.startProcessInstanceByKey("testProcess").Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownByExecuteOfDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwException());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownByExecuteOfDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		variables.putAll(throwError());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalMethodOfDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwException());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskException", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchErrorThrownBySignalOfDelegateExpression()
	  {
		VariableMap variables = Variables.createVariables().putValue("myDelegate", new ThrowErrorDelegate());
		string pi = runtimeService.startProcessInstanceByKey("testProcess", variables).Id;

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertNull(runtimeService.getVariable(pi, "signaled"));

		Execution serviceTask = runtimeService.createExecutionQuery().processInstanceId(pi).activityId("serviceTask").singleResult();
		assertNotNull(serviceTask);

		runtimeService.setVariables(pi, throwError());
		runtimeService.signal(serviceTask.Id);

		assertTrue((bool?) runtimeService.getVariable(pi, "executed"));
		assertTrue((bool?) runtimeService.getVariable(pi, "signaled"));

		Task userTask = taskService.createTaskQuery().processInstanceId(pi).singleResult();
		assertNotNull(userTask);
		assertEquals("userTaskError", userTask.TaskDefinitionKey);

		taskService.complete(userTask.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testUncaughtErrorThrownByJavaDelegateOnCallActivity-parent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
	  public virtual void testUncaughtErrorThrownByJavaDelegateOnCallActivity()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("uncaughtErrorThrownByJavaDelegateOnCallActivity-parent");
		}
		catch (BpmnError e)
		{
		  assertTextPresent("No catching boundary event found for error with errorCode '23', neither in same process nor in parent process", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential()
	  public virtual void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["executionsBeforeError"] = 2;
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential", variables).Id;
		assertThatErrorHasBeenCaught(procId);

		variables["executionsBeforeError"] = 2;
		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskSequential", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel()
	  public virtual void testCatchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["executionsBeforeError"] = 2;
		string procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel", variables).Id;
		assertThatErrorHasBeenCaught(procId);

		variables["executionsBeforeError"] = 2;
		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("catchErrorThrownByJavaDelegateOnMultiInstanceServiceTaskParallel", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testErrorThrownByJavaDelegateNotCaughtByOtherEventType()
	  public virtual void testErrorThrownByJavaDelegateNotCaughtByOtherEventType()
	  {
		string procId = runtimeService.startProcessInstanceByKey("testErrorThrownByJavaDelegateNotCaughtByOtherEventType").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

	  private void assertThatErrorHasBeenCaught(string procId)
	  {
		// The service task will throw an error event,
		// which is caught on the service task boundary
		assertEquals("No tasks found in task list.", 1, taskService.createTaskQuery().count());
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// Completing the task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

	  private void assertThatExceptionHasBeenCaught(string procId)
	  {
		// The service task will throw an error event,
		// which is caught on the service task boundary
		assertEquals("No tasks found in task list.", 1, taskService.createTaskQuery().count());
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Exception Task", task.Name);

		// Completing the task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConcurrentExecutionsInterruptedOnDestroyScope()
	  public virtual void testConcurrentExecutionsInterruptedOnDestroyScope()
	  {

		// this test makes sure that if the first concurrent execution destroys the scope
		// (due to the interrupting boundary catch), the second concurrent execution does not
		// move forward.

		// if the test fails, it produces a constraint violation in db.

		runtimeService.startProcessInstanceByKey("process");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByExpressionOnServiceTask()
	  public virtual void testCatchErrorThrownByExpressionOnServiceTask()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["bpmnErrorBean"] = new BpmnErrorBean();
		string procId = runtimeService.startProcessInstanceByKey("testCatchErrorThrownByExpressionOnServiceTask", variables).Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByDelegateExpressionOnServiceTask()
	  public virtual void testCatchErrorThrownByDelegateExpressionOnServiceTask()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["bpmnErrorBean"] = new BpmnErrorBean();
		string procId = runtimeService.startProcessInstanceByKey("testCatchErrorThrownByDelegateExpressionOnServiceTask", variables).Id;
		assertThatErrorHasBeenCaught(procId);

		variables["exceptionType"] = true;
		procId = runtimeService.startProcessInstanceByKey("testCatchErrorThrownByDelegateExpressionOnServiceTask", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByJavaDelegateProvidedByDelegateExpressionOnServiceTask()
	  public virtual void testCatchErrorThrownByJavaDelegateProvidedByDelegateExpressionOnServiceTask()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["bpmnErrorBean"] = new BpmnErrorBean();
		string procId = runtimeService.startProcessInstanceByKey("testCatchErrorThrownByJavaDelegateProvidedByDelegateExpressionOnServiceTask", variables).Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchExceptionThrownByExpressionOnServiceTask()
	  public virtual void testCatchExceptionThrownByExpressionOnServiceTask()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["bpmnErrorBean"] = new BpmnErrorBean();
		string procId = runtimeService.startProcessInstanceByKey("testCatchExceptionThrownByExpressionOnServiceTask", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchExceptionThrownByScriptTask()
	  public virtual void testCatchExceptionThrownByScriptTask()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		string procId = runtimeService.startProcessInstanceByKey("testCatchExceptionThrownByScriptTask", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchSpecializedExceptionThrownByDelegate()
	  public virtual void testCatchSpecializedExceptionThrownByDelegate()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["bpmnErrorBean"] = new BpmnErrorBean();
		string procId = runtimeService.startProcessInstanceByKey("testCatchSpecializedExceptionThrownByDelegate", variables).Id;
		assertThatExceptionHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUncaughtRuntimeException()
	  public virtual void testUncaughtRuntimeException()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testUncaughtRuntimeException");
		  fail("error should not be caught");
		}
		catch (Exception e)
		{
		  assertEquals("This should not be caught!", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUncaughtBusinessExceptionWrongErrorCode()
	  public virtual void testUncaughtBusinessExceptionWrongErrorCode()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("testUncaughtBusinessExceptionWrongErrorCode");
		  fail("error should not be caught");
		}
		catch (Exception e)
		{
		  assertEquals("couldn't execute activity <serviceTask id=\"serviceTask\" ...>: Business Exception", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnSubprocessThrownByNonInterruptingEventSubprocess()
	  public virtual void testCatchErrorOnSubprocessThrownByNonInterruptingEventSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		EventSubscription messageSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		runtimeService.messageEventReceived("message", messageSubscription.ExecutionId);

		// should successfully have reached the task following the boundary event
		Execution taskExecution = runtimeService.createExecutionQuery().activityId("afterBoundaryTask").singleResult();
		assertNotNull(taskExecution);
		Task task = taskService.createTaskQuery().executionId(taskExecution.Id).singleResult();
		assertNotNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnSubprocessThrownByInterruptingEventSubprocess()
	  public virtual void testCatchErrorOnSubprocessThrownByInterruptingEventSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");
		EventSubscription messageSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		runtimeService.messageEventReceived("message", messageSubscription.ExecutionId);

		// should successfully have reached the task following the boundary event
		Execution taskExecution = runtimeService.createExecutionQuery().activityId("afterBoundaryTask").singleResult();
		assertNotNull(taskExecution);
		Task task = taskService.createTaskQuery().executionId(taskExecution.Id).singleResult();
		assertNotNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnSubprocessThrownByNestedEventSubprocess()
	  public virtual void testCatchErrorOnSubprocessThrownByNestedEventSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		// trigger outer event subprocess
		EventSubscription messageSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		runtimeService.messageEventReceived("outerMessage", messageSubscription.ExecutionId);

		// trigger inner event subprocess
		messageSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		runtimeService.messageEventReceived("innerMessage", messageSubscription.ExecutionId);

		// should successfully have reached the task following the boundary event
		Execution taskExecution = runtimeService.createExecutionQuery().activityId("afterBoundaryTask").singleResult();
		assertNotNull(taskExecution);
		Task task = taskService.createTaskQuery().executionId(taskExecution.Id).singleResult();
		assertNotNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorOnSubprocessSetsErrorVariables()
	  public virtual void testCatchErrorOnSubprocessSetsErrorVariables()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorVariable";
		object errorCode = "error1";

		checkErrorVariable(variableName, errorCode);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ThrowErrorProcess.bpmn", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByCallActivityOnSubprocessSetsErrorCodeVariable.bpmn" })]
	  public virtual void testCatchErrorThrownByCallActivityOnSubprocessSetsErrorVariables()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorVariable";
		//the code we gave the thrown error
		object errorCode = "error";

		checkErrorVariable(variableName, errorCode);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByMultiInstanceSubProcessSetsErrorCodeVariable.bpmn" })]
	  public virtual void testCatchErrorThrownByMultiInstanceSubProcessSetsErrorVariables()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorVariable";
		//the code we gave the thrown error
		object errorCode = "error";

		checkErrorVariable(variableName, errorCode);
	  }

	  private void checkErrorVariable(string variableName, object expectedValue)
	  {
		VariableInstance errorVariable = runtimeService.createVariableInstanceQuery().variableName(variableName).singleResult();
		assertThat(errorVariable, @is(notNullValue()));
		assertThat(errorVariable.Value, @is(expectedValue));
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchBpmnErrorThrownByJavaDelegateInCallActivityOnSubprocessSetsErrorVariables.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
	  public virtual void testCatchBpmnErrorThrownByJavaDelegateInCallActivityOnSubprocessSetsErrorVariables()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorCode";
		//the code we gave the thrown error
		object errorCode = "errorCode";
		checkErrorVariable(variableName, errorCode);
		checkErrorVariable("errorMessageVariable", "ouch!");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/error/reviewSalesLead.bpmn20.xml"})]
	  public virtual void testReviewSalesLeadProcess()
	  {

		// After starting the process, a task should be assigned to the 'initiator' (normally set by GUI)
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["details"] = "very interesting";
		variables["customerName"] = "Alfresco";
		string procId = runtimeService.startProcessInstanceByKey("reviewSaledLead", variables).Id;
		Task task = taskService.createTaskQuery().taskAssignee("kermit").singleResult();
		assertEquals("Provide new sales lead", task.Name);

		// After completing the task, the review subprocess will be active
		taskService.complete(task.Id);
		Task ratingTask = taskService.createTaskQuery().taskCandidateGroup("accountancy").singleResult();
		assertEquals("Review customer rating", ratingTask.Name);
		Task profitabilityTask = taskService.createTaskQuery().taskCandidateGroup("management").singleResult();
		assertEquals("Review profitability", profitabilityTask.Name);

		// Complete the management task by stating that not enough info was provided
		// This should throw the error event, which closes the subprocess
		variables = new Dictionary<string, object>();
		variables["notEnoughInformation"] = true;
		taskService.complete(profitabilityTask.Id, variables);

		// The 'provide additional details' task should now be active
		Task provideDetailsTask = taskService.createTaskQuery().taskAssignee("kermit").singleResult();
		assertEquals("Provide additional details", provideDetailsTask.Name);

		// Providing more details (ie. completing the task), will activate the subprocess again
		taskService.complete(provideDetailsTask.Id);
		IList<Task> reviewTasks = taskService.createTaskQuery().orderByTaskName().asc().list();
		assertEquals("Review customer rating", reviewTasks[0].Name);
		assertEquals("Review profitability", reviewTasks[1].Name);

		// Completing both tasks normally ends the process
		taskService.complete(reviewTasks[0].Id);
		variables["notEnoughInformation"] = false;
		taskService.complete(reviewTasks[1].Id, variables);
		assertProcessEnded(procId);
	  }
	}

}