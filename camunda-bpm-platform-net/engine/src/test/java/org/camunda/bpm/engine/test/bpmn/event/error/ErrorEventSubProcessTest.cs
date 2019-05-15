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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Falko Menge
	/// </summary>
	public class ErrorEventSubProcessTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessTakesPrecedence()
	  public virtual void testEventSubprocessTakesPrecedence()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testErrorCodeTakesPrecedence()
	  public virtual void testErrorCodeTakesPrecedence()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;

		// The process will throw an error event,
		// which is caught and escalated by a User Task
		assertEquals("No tasks found in task list.", 1, taskService.createTaskQuery().taskDefinitionKey("taskAfterErrorCatch2").count());
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// Completing the Task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorInEmbeddedSubProcess()
	  public virtual void testCatchErrorInEmbeddedSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorInEmbeddedSubProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByScriptTaskInEmbeddedSubProcess()
	  public virtual void testCatchErrorThrownByScriptTaskInEmbeddedSubProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorThrownByScriptTaskInEmbeddedSubProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByScriptTaskInEmbeddedSubProcessWithErrorCode()
	  public virtual void testCatchErrorThrownByScriptTaskInEmbeddedSubProcessWithErrorCode()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorThrownByScriptTaskInEmbeddedSubProcessWithErrorCode").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByScriptTaskInTopLevelProcess()
	  public virtual void testCatchErrorThrownByScriptTaskInTopLevelProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorThrownByScriptTaskInTopLevelProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorThrownByScriptTaskInsideSubProcessInTopLevelProcess()
	  public virtual void testCatchErrorThrownByScriptTaskInsideSubProcessInTopLevelProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("CatchErrorThrownByScriptTaskInsideSubProcessInTopLevelProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/BoundaryErrorEventTest.testCatchErrorThrownByJavaDelegateOnCallActivity-child.bpmn20.xml" })]
	  public virtual void testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess()
	  {
		string procId = runtimeService.startProcessInstanceByKey("testThrowErrorInScriptTaskInsideCallActivitiCatchInTopLevelProcess").Id;
		assertThatErrorHasBeenCaught(procId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfAbstractBpmnActivityBehavior()
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByAbstractBpmnActivityBehavior.bpmn20.xml" })]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
	  public virtual void testCatchExceptionThrownBySignalOfDelegateExpression()
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

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorThrownByDelegateExpression.bpmn20.xml" })]
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

	  private void assertThatErrorHasBeenCaught(string procId)
	  {
		// The process will throw an error event,
		// which is caught and escalated by a User Task
		assertEquals("No tasks found in task list.", 1, taskService.createTaskQuery().count());
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("Escalated Task", task.Name);

		// Completing the Task will end the process instance
		taskService.complete(task.Id);
		assertProcessEnded(procId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchErrorEventSubprocessSetErrorVariables()
	  public virtual void testCatchErrorEventSubprocessSetErrorVariables()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorCode";
		VariableInstance errorVariable = runtimeService.createVariableInstanceQuery().variableName(variableName).singleResult();

		assertThat(errorVariable, @is(notNullValue()));
		//the code we gave the thrown error
		object errorCode = "error";
		assertThat(errorVariable.Value, @is(errorCode));

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ThrowErrorProcess.bpmn", "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchErrorFromCallActivitySetsErrorVariables.bpmn" })]
	  public virtual void testCatchErrorFromCallActivitySetsErrorVariable()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorCode";
		VariableInstance errorVariable = runtimeService.createVariableInstanceQuery().variableName(variableName).singleResult();

		assertThat(errorVariable, @is(notNullValue()));
		//the code we gave the thrown error
		object errorCode = "error";
		assertThat(errorVariable.Value, @is(errorCode));
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testCatchBpmnErrorFromJavaDelegateInsideCallActivitySetsErrorVariable.bpmn", "org/camunda/bpm/engine/test/bpmn/callactivity/subProcessWithThrownError.bpmn" })]
	  public virtual void testCatchBpmnErrorFromJavaDelegateInsideCallActivitySetsErrorVariable()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
		//the name used in "camunda:errorCodeVariable" in the BPMN
		string variableName = "errorCode";
		//the code we gave the thrown error
		object errorCode = "errorCode";
		VariableInstance errorVariable = runtimeService.createVariableInstanceQuery().variableName(variableName).singleResult();
		assertThat(errorVariable.Value, @is(errorCode));

		errorVariable = runtimeService.createVariableInstanceQuery().variableName("errorMessageVariable").singleResult();
		assertThat(errorVariable.Value, @is((object)"ouch!"));
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoop.bpmn20.xml" })]
	  public virtual void testShouldNotThrowErrorInLoop()
	  {
		runtimeService.startProcessInstanceByKey("looping-error");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("WaitState", task.Name);
		taskService.complete(task.Id);

		assertEquals("ErrorHandlingUserTask", taskService.createTaskQuery().singleResult().Name);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopWithCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/ThrowErrorToCallActivity.bpmn20.xml" })]
	  public virtual void testShouldNotThrowErrorInLoopWithCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("CallActivityErrorInLoop");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("ErrorLog", task.Name);
		taskService.complete(task.Id);

		assertEquals("ErrorHandlingUserTask", taskService.createTaskQuery().singleResult().Name);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopWithMultipleSubProcess.bpmn20.xml"})]
	  public virtual void testShouldNotThrowErrorInLoopForMultipleSubProcess()
	  {
		runtimeService.startProcessInstanceByKey("looping-error");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("LoggerTask", task.Name);
		taskService.complete(task.Id);

		assertEquals("ErrorHandlingTask", taskService.createTaskQuery().singleResult().Name);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/error/ErrorEventSubProcessTest.testThrowErrorInLoopFromCallActivityToEventSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/error/ThrowErrorToCallActivity.bpmn20.xml" })]
	  public virtual void FAILING_testShouldNotThrowErrorInLoopFromCallActivityToEventSubProcess()
	  {
		runtimeService.startProcessInstanceByKey("Process_1");

		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("userTask", task.Name);
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertEquals("ErrorLog", task.Name);
		taskService.complete(task.Id);

		// TODO: Loop exists when error thrown from call activity to event sub process
		// as they both have different process definition - CAM-6212
		assertEquals("BoundaryEventTask", taskService.createTaskQuery().singleResult().Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrownAnErrorInEventSubprocessInSubprocessDifferentTransaction()
	  public virtual void testThrownAnErrorInEventSubprocessInSubprocessDifferentTransaction()
	  {
		runtimeService.startProcessInstanceByKey("eventSubProcess");

		Task taskBefore = taskService.createTaskQuery().singleResult();
		assertNotNull(taskBefore);
		assertEquals("inside subprocess", taskBefore.Name);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		//when job is executed task is created
		managementService.executeJob(job.Id);

		Task taskDuring = taskService.createTaskQuery().taskName("inside event sub").singleResult();
		assertNotNull(taskDuring);

		taskService.complete(taskDuring.Id);

		Task taskAfter = taskService.createTaskQuery().singleResult();
		assertNotNull(taskAfter);
		assertEquals("after catch", taskAfter.Name);

		Job jobAfter = managementService.createJobQuery().singleResult();
		assertNull(jobAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrownAnErrorInEventSubprocessInSubprocess()
	  public virtual void testThrownAnErrorInEventSubprocessInSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("eventSubProcess");

		Task taskBefore = taskService.createTaskQuery().singleResult();
		assertNotNull(taskBefore);
		assertEquals("inside subprocess", taskBefore.Name);

		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);

		//when job is executed task is created
		managementService.executeJob(job.Id);

		Task taskAfter = taskService.createTaskQuery().singleResult();
		assertNotNull(taskAfter);
		assertEquals("after catch", taskAfter.Name);

		Job jobAfter = managementService.createJobQuery().singleResult();
		assertNull(jobAfter);
	  }
	}

}