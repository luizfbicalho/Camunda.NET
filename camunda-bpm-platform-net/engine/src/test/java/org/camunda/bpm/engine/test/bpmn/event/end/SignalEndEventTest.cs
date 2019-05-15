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

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Kristin Polenz
	/// </summary>
	public class SignalEndEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCatchSignalEndEventInEmbeddedSubprocess() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCatchSignalEndEventInEmbeddedSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("catchSignalEndEventInEmbeddedSubprocess");
		assertNotNull(processInstance);

		// After process start, usertask in subprocess should exist
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("subprocessTask", task.Name);

		// After task completion, signal end event is reached and caught
		taskService.complete(task.Id);

		task = taskService.createTaskQuery().singleResult();
		assertEquals("task after catching the signal", task.Name);

		taskService.complete(task.Id);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/bpmn/event/end/SignalEndEventTest.catchSignalEndEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/end/SignalEndEventTest.processWithSignalEndEvent.bpmn20.xml" }) public void testCatchSignalEndEventInCallActivity() throws Exception
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/bpmn/event/end/SignalEndEventTest.catchSignalEndEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/end/SignalEndEventTest.processWithSignalEndEvent.bpmn20.xml" })]
	  public virtual void testCatchSignalEndEventInCallActivity()
	  {
		// first, start process to wait of the signal event
		ProcessInstance processInstanceCatchEvent = runtimeService.startProcessInstanceByKey("catchSignalEndEvent");
		assertNotNull(processInstanceCatchEvent);

		// now we have a subscription for the signal event:
		assertEquals(1, runtimeService.createEventSubscriptionQuery().count());
		assertEquals("alert", runtimeService.createEventSubscriptionQuery().singleResult().EventName);

		// start process which throw the signal end event
		ProcessInstance processInstanceEndEvent = runtimeService.startProcessInstanceByKey("processWithSignalEndEvent");
		assertNotNull(processInstanceEndEvent);
		assertProcessEnded(processInstanceEndEvent.Id);

		// user task of process catchSignalEndEvent
		assertEquals(1, taskService.createTaskQuery().count());
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("taskAfterSignalCatch", task.TaskDefinitionKey);

		// complete user task
		taskService.complete(task.Id);

		assertProcessEnded(processInstanceCatchEvent.Id);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/testPropagateOutputVariablesWhileThrowSignal.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEndEventTest.parent.bpmn20.xml" })]
	  public virtual void testPropagateOutputVariablesWhileThrowSignal()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("SignalParentProcess", variables).Id;

		// when
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/testPropagateOutputVariablesWhileThrowSignal2.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEndEventTest.parent.bpmn20.xml" })]
	  public virtual void testPropagateOutputVariablesWhileThrowSignal2()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("SignalParentProcess", variables).Id;

		// when
		string id = taskService.createTaskQuery().taskName("inside subprocess").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  protected internal virtual void checkOutput(string processInstanceId)
	  {
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched signal").count());
		// and set the output variable of the called process to the process
		assertNotNull(runtimeService.getVariable(processInstanceId, "cancelReason"));
		assertEquals(42, runtimeService.getVariable(processInstanceId, "input"));
	  }
	}

}