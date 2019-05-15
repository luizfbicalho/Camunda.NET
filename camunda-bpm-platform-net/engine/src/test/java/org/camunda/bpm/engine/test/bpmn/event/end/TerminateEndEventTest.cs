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
namespace org.camunda.bpm.engine.test.bpmn.@event.end
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using RecorderExecutionListener = org.camunda.bpm.engine.test.bpmn.executionlistener.RecorderExecutionListener;

	/// <summary>
	/// @author Nico Rehwaldt
	/// </summary>
	public class TerminateEndEventTest : PluggableProcessEngineTestCase
	{

	  public static int serviceTaskInvokedCount = 0;

	  public class CountDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  serviceTaskInvokedCount++;

		  // leave only 3 out of n subprocesses
		  execution.setVariableLocal("terminate", serviceTaskInvokedCount > 3);
		}
	  }

	  public static int serviceTaskInvokedCount2 = 0;

	  public class CountDelegate2 : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  serviceTaskInvokedCount2++;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessTerminate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testProcessTerminate()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long executionEntities = runtimeService.createExecutionQuery().processInstanceId(pi.Id).count();
		assertEquals(3, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preTerminateTask").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateWithSubProcess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateWithSubProcess()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the process and
		long executionEntities = runtimeService.createExecutionQuery().processInstanceId(pi.Id).count();
		assertEquals(4, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preTerminateEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateWithCallActivity.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessNoTerminate.bpmn" }) public void testTerminateWithCallActivity() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateWithCallActivity.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessNoTerminate.bpmn" })]
	  public virtual void testTerminateWithCallActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long executionEntities = runtimeService.createExecutionQuery().processInstanceId(pi.Id).count();
		assertEquals(4, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preTerminateEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateInSubProcess()
	  {
		serviceTaskInvokedCount = 0;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the subprocess and continue the parent
		long executionEntities = runtimeService.createExecutionQuery().processInstanceId(pi.Id).count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

	  /// <summary>
	  /// CAM-4067
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessShouldNotInvokeProcessEndListeners()
	  public virtual void testTerminateInSubProcessShouldNotInvokeProcessEndListeners()
	  {
		RecorderExecutionListener.clear();

		// when process instance is started and terminate end event in subprocess executed
		runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// then the outer task still exists
		Task outerTask = taskService.createTaskQuery().singleResult();
		assertNotNull(outerTask);
		assertEquals("outerTask", outerTask.TaskDefinitionKey);

		// and the process end listener was not invoked
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

	  }

	  /// <summary>
	  /// CAM-4067
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessConcurrentShouldNotInvokeProcessEndListeners()
	  public virtual void testTerminateInSubProcessConcurrentShouldNotInvokeProcessEndListeners()
	  {
		RecorderExecutionListener.clear();

		// when process instance is started and terminate end event in subprocess executed
		runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// then the outer task still exists
		Task outerTask = taskService.createTaskQuery().singleResult();
		assertNotNull(outerTask);
		assertEquals("outerTask", outerTask.TaskDefinitionKey);

		// and the process end listener was not invoked
		assertTrue(RecorderExecutionListener.RecordedEvents.Count == 0);

	  }

	  /// <summary>
	  /// CAM-4067
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcess.bpmn") public void testTerminateInSubProcessShouldNotEndProcessInstanceInHistory() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcess.bpmn")]
	  public virtual void testTerminateInSubProcessShouldNotEndProcessInstanceInHistory()
	  {
		// when process instance is started and terminate end event in subprocess executed
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// then the historic process instance should not appear ended
		assertProcessNotEnded(pi.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().singleResult();

		  assertNotNull(hpi);
		  assertNull(hpi.EndTime);
		  assertNull(hpi.DurationInMillis);
		  assertNull(hpi.DeleteReason);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessConcurrent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateInSubProcessConcurrent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

	  /// <summary>
	  /// CAM-4067
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcessConcurrent.bpmn") public void testTerminateInSubProcessConcurrentShouldNotEndProcessInstanceInHistory() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInSubProcessConcurrent.bpmn")]
	  public virtual void testTerminateInSubProcessConcurrentShouldNotEndProcessInstanceInHistory()
	  {
		// when process instance is started and terminate end event in subprocess executed
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// then the historic process instance should not appear ended
		assertProcessNotEnded(pi.Id);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  HistoricProcessInstance hpi = historyService.createHistoricProcessInstanceQuery().singleResult();

		  assertNotNull(hpi);
		  assertNull(hpi.EndTime);
		  assertNull(hpi.DurationInMillis);
		  assertNull(hpi.DeleteReason);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessConcurrentMultiInstance() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateInSubProcessConcurrentMultiInstance()
	  {
		serviceTaskInvokedCount = 0;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(12, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		long executionEntities2 = runtimeService.createExecutionQuery().count();
		assertEquals(10, executionEntities2);

		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task t in tasks)
		{
		  taskService.complete(t.Id);
		}

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessMultiInstance() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateInSubProcessMultiInstance()
	  {
		serviceTaskInvokedCount = 0;

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTerminateInSubProcessSequentialConcurrentMultiInstance() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTerminateInSubProcessSequentialConcurrentMultiInstance()
	  {
		serviceTaskInvokedCount = 0;
		serviceTaskInvokedCount2 = 0;

		// Starting multi instance with 5 instances; terminating 2, finishing 3
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		long remainingExecutions = runtimeService.createExecutionQuery().count();

		// outer execution still available
		assertEquals(1, remainingExecutions);

		// three finished
		assertEquals(3, serviceTaskInvokedCount2);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		// last task remaining
		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivity.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" }) public void testTerminateInCallActivity() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivity.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" })]
	  public virtual void testTerminateInCallActivity()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the called process and continue the parent
		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityMulitInstance.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" }) public void testTerminateInCallActivityMulitInstance() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityMulitInstance.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessTerminate.bpmn" })]
	  public virtual void testTerminateInCallActivityMulitInstance()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the called process and continue the parent
		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrent.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" }) public void testTerminateInCallActivityConcurrent() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrent.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" })]
	  public virtual void testTerminateInCallActivityConcurrent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the called process and continue the parent
		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrentMulitInstance.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" }) public void testTerminateInCallActivityConcurrentMulitInstance() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.testTerminateInCallActivityConcurrentMulitInstance.bpmn", "org/camunda/bpm/engine/test/bpmn/event/end/TerminateEndEventTest.subProcessConcurrentTerminate.bpmn" })]
	  public virtual void testTerminateInCallActivityConcurrentMulitInstance()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("terminateEndEventExample");

		// should terminate the called process and continue the parent
		long executionEntities = runtimeService.createExecutionQuery().count();
		assertEquals(1, executionEntities);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).taskDefinitionKey("preNormalEnd").singleResult();
		taskService.complete(task.Id);

		assertProcessEnded(pi.Id);
	  }
	}
}