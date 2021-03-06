﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.test.bpmn.@event.escalation
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class EscalationEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrowEscalationEventFromEmbeddedSubprocess()
	  public virtual void testThrowEscalationEventFromEmbeddedSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testThrowEscalationEventHierarchical()
	  public virtual void testThrowEscalationEventHierarchical()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event inside the subprocess should catch the escalation event (and not the boundary event on process)
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation inside subprocess").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.nonInterruptingEscalationBoundaryEventOnCallActivity.bpmn20.xml"})]
	  public virtual void testThrowEscalationEventFromCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");
		// when throw an escalation event on called process

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event on call activity should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the called process
		assertEquals(1, taskService.createTaskQuery().taskName("task after thrown escalation").count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml")]
	  public virtual void testThrowEscalationEventNotCaught()
	  {
		runtimeService.startProcessInstanceByKey("throwEscalationProcess");
		// when throw an escalation event

		// continue the process instance, no activity should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskName("task after thrown escalation").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryEventWithEscalationCode()
	  public virtual void testBoundaryEventWithEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess with escalationCode=1

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event with escalationCode=1 should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation 1").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryEventWithoutEscalationCode()
	  public virtual void testBoundaryEventWithoutEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event without escalationCode should catch the escalation event (and all other escalation events)
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryEventWithEmptyEscalationCode()
	  public virtual void testBoundaryEventWithEmptyEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event with empty escalationCode should catch the escalation event (and all other escalation events)
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryEventWithoutEscalationRef()
	  public virtual void testBoundaryEventWithoutEscalationRef()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event without escalationRef should catch the escalation event (and all other escalation events)
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
	  public virtual void testInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the multi-instance subprocess

		// the interrupting boundary event should catch the first escalation event and cancel all instances of the subprocess
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
	  public virtual void testNonInterruptingEscalationBoundaryEventOnMultiInstanceSubprocess()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the multi-instance subprocess

		assertEquals(10, taskService.createTaskQuery().count());
		// the non-interrupting boundary event should catch every escalation event
		assertEquals(5, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(5, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

	  /// <summary>
	  /// current bug: default value of 'cancelActivity' is 'true'
	  /// </summary>
	  /// <seealso cref= https://app.camunda.com/jira/browse/CAM-4403 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testImplicitNonInterruptingEscalationBoundaryEvent()
	  public virtual void FAILING_testImplicitNonInterruptingEscalationBoundaryEvent()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the implicit non-interrupting boundary event ('cancelActivity' is not defined) should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingEscalationBoundaryEvent()
	  public virtual void testInterruptingEscalationBoundaryEvent()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		// the interrupting boundary should catch the escalation event event and cancel the subprocess
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.interruptingEscalationBoundaryEventOnCallActivity.bpmn20.xml" })]
	  public virtual void testInterruptingEscalationBoundaryEventOnCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");
		// when throw an escalation event on called process

		// the interrupting boundary event on call activity should catch the escalation event and cancel the called process
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelEscalationEndEvent()
	  public virtual void testParallelEscalationEndEvent()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation end event inside the subprocess

		assertEquals(2, taskService.createTaskQuery().count());
		// the non-interrupting boundary event should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and continue the parallel flow in subprocess
		assertEquals(1, taskService.createTaskQuery().taskName("task in subprocess").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEscalationEndEvent()
	  public virtual void testEscalationEndEvent()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation end event inside the subprocess

		// the subprocess should end and
		// the non-interrupting boundary event should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().count());
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileCatchEscalationOnCallActivity()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("catchEscalationProcess", variables).Id;
		// when throw an escalation event on called process

		// the non-interrupting boundary event on call activity should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and set the output variable of the called process to the process
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchEscalationOnCallActivity.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesTwoTimes()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("catchEscalationProcess", variables).Id;
		// when throw an escalation event on called process

		Task taskInSuperProcess = taskService.createTaskQuery().taskDefinitionKey("taskAfterCatchedEscalation").singleResult();
		assertNotNull(taskInSuperProcess);

		// (1) the variables has been passed for the first time (from sub process to super process)
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));

		// change variable "input" in sub process
		Task taskInSubProcess = taskService.createTaskQuery().taskDefinitionKey("task").singleResult();
		runtimeService.setVariable(taskInSubProcess.ProcessInstanceId, "input", 999);
		taskService.complete(taskInSubProcess.Id);

		// (2) the variables has been passed for the second time (from sub process to super process)
		assertEquals(999, runtimeService.getVariable(processInstanceId, "output"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileCatchInterruptingEscalationOnCallActivity()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("catchEscalationProcess", variables).Id;
		// when throw an escalation event on called process

		// the interrupting boundary event on call activity should catch the escalation event
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and set the output variable of the called process to the process
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testPropagateOutputVariablesWithoutCatchEscalation.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWithoutCatchEscalation()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("catchEscalationProcess", variables).Id;
		// when throw an escalation event on called process

		// then the output variable of the called process should be set to the process
		// also if the escalation is not caught by the process
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRetrieveEscalationCodeVariableOnBoundaryEvent()
	  public virtual void testRetrieveEscalationCodeVariableOnBoundaryEvent()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		// the boundary event should catch the escalation event
		Task task = taskService.createTaskQuery().taskName("task after catched escalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRetrieveEscalationCodeVariableOnBoundaryEventWithoutEscalationCode()
	  public virtual void testRetrieveEscalationCodeVariableOnBoundaryEventWithoutEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("escalationProcess");
		// when throw an escalation event inside the subprocess

		// the boundary event without escalationCode should catch the escalation event
		Task task = taskService.createTaskQuery().taskName("task after catched escalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml"})]
	  public virtual void testInterruptingRetrieveEscalationCodeInSuperProcess()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");

		// the event subprocess without escalationCode should catch the escalation event
		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterCatchedEscalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml"})]
	  public virtual void testInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");

		// the event subprocess without escalationCode should catch the escalation event
		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterCatchedEscalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }
	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testNonInterruptingRetrieveEscalationCodeInSuperProcess.bpmn20.xml"})]
	  public virtual void testNonInterruptingRetrieveEscalationCodeInSuperProcess()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");

		// the event subprocess without escalationCode should catch the escalation event
		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterCatchedEscalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.throwEscalationEvent.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode.bpmn20.xml"})]
	  public virtual void testNonInterruptingRetrieveEscalationCodeInSuperProcessWithoutEscalationCode()
	  {
		runtimeService.startProcessInstanceByKey("catchEscalationProcess");

		// the event subprocess without escalationCode should catch the escalation event
		Task task = taskService.createTaskQuery().taskDefinitionKey("taskAfterCatchedEscalation").singleResult();
		assertNotNull(task);

		// and set the escalationCode of the escalation event to the declared variable
		assertEquals("escalationCode", runtimeService.getVariable(task.ExecutionId, "escalationCodeVar"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalation.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalation()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalationTwoLevels.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalationTwoLevels()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalationThreeLevels.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalationThreeLevels()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalationInSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalationInSubProcess()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalationInSubProcessThreeLevels.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalationInSubProcessThreeLevels()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("ut2").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/escalation/testOutputVariablesWhileThrowEscalation2.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.escalationParent.bpmn20.xml"})]
	  public virtual void testPropagateOutputVariablesWhileThrowEscalation2()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["input"] = 42;
		string processInstanceId = runtimeService.startProcessInstanceByKey("EscalationParentProcess", variables).Id;

		// when throw an escalation event on called process
		string id = taskService.createTaskQuery().taskName("inside subprocess").singleResult().Id;
		taskService.complete(id);

		// then
		checkOutput(processInstanceId);
	  }

	  protected internal virtual void checkOutput(string processInstanceId)
	  {
		assertEquals(1, taskService.createTaskQuery().taskName("task after catched escalation").count());
		// and set the output variable of the called process to the process
		assertNotNull(runtimeService.getVariable(processInstanceId, "cancelReason"));
		assertEquals(42, runtimeService.getVariable(processInstanceId, "output"));
	  }
	}

}