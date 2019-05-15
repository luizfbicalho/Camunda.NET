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
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class MessageCorrelationByLocalVariablesTest
	{
		private bool InstanceFieldsInitialized = false;

		public MessageCorrelationByLocalVariablesTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
		}


	  public const string TEST_MESSAGE_NAME = "TEST_MSG";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testHelper = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReceiveTaskMessageCorrelation()
	  public virtual void testReceiveTaskMessageCorrelation()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().receiveTask("MessageReceiver_1").message(TEST_MESSAGE_NAME).camundaInputParameter("localVar", "${loopVar}").camundaInputParameter("constVar", "someValue").userTask("UserTask_1").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 3);
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated by local variables
		string messageName = TEST_MESSAGE_NAME;
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		int correlationKey = 1;
		correlationKeys["localVar"] = correlationKey;
		correlationKeys["constVar"] = "someValue";

		MessageCorrelationResult messageCorrelationResult = engineRule.RuntimeService.createMessageCorrelation(messageName).localVariablesEqual(correlationKeys).setVariables(Variables.createVariables().putValue("newVar", "newValue")).correlateWithResult();

		//then one message is correlated, two other continue waiting
		checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

		//uncorrelated executions
		IList<Execution> uncorrelatedExecutions = engineRule.RuntimeService.createExecutionQuery().activityId("MessageReceiver_1").list();
		assertEquals(2, uncorrelatedExecutions.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntermediateCatchEventMessageCorrelation()
	  public virtual void testIntermediateCatchEventMessageCorrelation()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().intermediateCatchEvent("MessageReceiver_1").message(TEST_MESSAGE_NAME).camundaInputParameter("localVar", "${loopVar}").userTask("UserTask_1").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 3);
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated by local variables
		string messageName = TEST_MESSAGE_NAME;
		int correlationKey = 1;

		MessageCorrelationResult messageCorrelationResult = engineRule.RuntimeService.createMessageCorrelation(messageName).localVariableEquals("localVar", correlationKey).setVariables(Variables.createVariables().putValue("newVar", "newValue")).correlateWithResult();

		//then one message is correlated, two others continue waiting
		checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

		//uncorrelated executions
		IList<Execution> uncorrelatedExecutions = engineRule.RuntimeService.createExecutionQuery().activityId("MessageReceiver_1").list();
		assertEquals(2, uncorrelatedExecutions.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessageBoundaryEventMessageCorrelation()
	  public virtual void testMessageBoundaryEventMessageCorrelation()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().userTask("UserTask_1").camundaInputParameter("localVar", "${loopVar}").camundaInputParameter("constVar", "someValue").boundaryEvent("MessageReceiver_1").message(TEST_MESSAGE_NAME).userTask("UserTask_2").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 3);
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated by local variables
		string messageName = TEST_MESSAGE_NAME;
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		int correlationKey = 1;
		correlationKeys["localVar"] = correlationKey;
		correlationKeys["constVar"] = "someValue";
		IDictionary<string, object> messagePayload = new Dictionary<string, object>();
		messagePayload["newVar"] = "newValue";

		MessageCorrelationResult messageCorrelationResult = engineRule.RuntimeService.createMessageCorrelation(messageName).localVariablesEqual(correlationKeys).setVariables(messagePayload).correlateWithResult();

		//then one message is correlated, two others continue waiting
		checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "UserTask_1");

		//uncorrelated executions
		IList<Execution> uncorrelatedExecutions = engineRule.RuntimeService.createExecutionQuery().activityId("UserTask_1").list();
		assertEquals(2, uncorrelatedExecutions.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBothInstanceAndLocalVariableMessageCorrelation()
	  public virtual void testBothInstanceAndLocalVariableMessageCorrelation()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().receiveTask("MessageReceiver_1").message(TEST_MESSAGE_NAME).userTask("UserTask_1").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		model = modify(model).activityBuilder("MessageReceiver_1").camundaInputParameter("localVar", "${loopVar}").camundaInputParameter("constVar", "someValue").done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 3);
		variables["processInstanceVar"] = "processInstanceVarValue";
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//second process instance with another process instance variable value
		variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 3);
		variables["processInstanceVar"] = "anotherProcessInstanceVarValue";
		engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated by local variables
		string messageName = TEST_MESSAGE_NAME;
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		int correlationKey = 1;
		correlationKeys["localVar"] = correlationKey;
		correlationKeys["constVar"] = "someValue";
		IDictionary<string, object> processInstanceKeys = new Dictionary<string, object>();
		string processInstanceVarValue = "processInstanceVarValue";
		processInstanceKeys["processInstanceVar"] = processInstanceVarValue;
		IDictionary<string, object> messagePayload = new Dictionary<string, object>();
		messagePayload["newVar"] = "newValue";

		MessageCorrelationResult messageCorrelationResult = engineRule.RuntimeService.createMessageCorrelation(messageName).processInstanceVariablesEqual(processInstanceKeys).localVariablesEqual(correlationKeys).setVariables(messagePayload).correlateWithResult();

		//then exactly one message is correlated = one receive task is passed by, two + three others continue waiting
		checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

		//uncorrelated executions
		IList<Execution> uncorrelatedExecutions = engineRule.RuntimeService.createExecutionQuery().activityId("MessageReceiver_1").list();
		assertEquals(5, uncorrelatedExecutions.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReceiveTaskMessageCorrelationFail()
	  public virtual void testReceiveTaskMessageCorrelationFail()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().receiveTask("MessageReceiver_1").message(TEST_MESSAGE_NAME).camundaInputParameter("localVar", "${loopVar}").camundaInputParameter("constVar", "someValue").userTask("UserTask_1").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 1);
		engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated by local variables
		string messageName = TEST_MESSAGE_NAME;
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		int correlationKey = 1;
		correlationKeys["localVar"] = correlationKey;
		correlationKeys["constVar"] = "someValue";

		// declare expected exception
		thrown.expect(typeof(MismatchingMessageCorrelationException));
		thrown.expectMessage(string.Format("Cannot correlate a message with name '{0}' to a single execution", TEST_MESSAGE_NAME));

		engineRule.RuntimeService.createMessageCorrelation(messageName).localVariablesEqual(correlationKeys).setVariables(Variables.createVariables().putValue("newVar", "newValue")).correlateWithResult();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReceiveTaskMessageCorrelationAll()
	  public virtual void testReceiveTaskMessageCorrelationAll()
	  {
		//given
		BpmnModelInstance model = Bpmn.createExecutableProcess("Process_1").startEvent().subProcess("SubProcess_1").embeddedSubProcess().startEvent().receiveTask("MessageReceiver_1").message(TEST_MESSAGE_NAME).camundaInputParameter("localVar", "${loopVar}").camundaInputParameter("constVar", "someValue").userTask("UserTask_1").endEvent().subProcessDone().multiInstance().camundaCollection("${vars}").camundaElementVariable("loopVar").multiInstanceDone().endEvent().done();

		testHelper.deploy(model);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["vars"] = Arrays.asList(1, 2, 1);
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey("Process_1", variables);

		//when correlated ALL by local variables
		string messageName = TEST_MESSAGE_NAME;
		IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
		int correlationKey = 1;
		correlationKeys["localVar"] = correlationKey;
		correlationKeys["constVar"] = "someValue";

		IList<MessageCorrelationResult> messageCorrelationResults = engineRule.RuntimeService.createMessageCorrelation(messageName).localVariablesEqual(correlationKeys).setVariables(Variables.createVariables().putValue("newVar", "newValue")).correlateAllWithResult();

		//then two messages correlated, one message task is still waiting
		foreach (MessageCorrelationResult result in messageCorrelationResults)
		{
		  checkExecutionMessageCorrelationResult(result, processInstance, "MessageReceiver_1");
		}

		//uncorrelated executions
		IList<Execution> uncorrelatedExecutions = engineRule.RuntimeService.createExecutionQuery().activityId("MessageReceiver_1").list();
		assertEquals(1, uncorrelatedExecutions.Count);

	  }

	  protected internal virtual void checkExecutionMessageCorrelationResult(MessageCorrelationResult result, ProcessInstance processInstance, string activityId)
	  {
		assertNotNull(result);
		assertEquals(MessageCorrelationResultType.Execution, result.ResultType);
		assertEquals(processInstance.Id, result.Execution.ProcessInstanceId);
		ExecutionEntity entity = (ExecutionEntity) result.Execution;
		assertEquals(activityId, entity.ActivityId);
	  }

	}

}