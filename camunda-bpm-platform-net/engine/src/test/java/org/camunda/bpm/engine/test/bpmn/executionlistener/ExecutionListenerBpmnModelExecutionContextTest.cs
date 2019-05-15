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
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using org.camunda.bpm.model.bpmn.instance;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ExecutionListenerBpmnModelExecutionContextTest : PluggableProcessEngineTestCase
	{

	  private const string PROCESS_ID = "process";
	  private const string START_ID = "start";
	  private const string SEQUENCE_FLOW_ID = "sequenceFlow";
	  private const string CATCH_EVENT_ID = "catchEvent";
	  private const string GATEWAY_ID = "gateway";
	  private const string USER_TASK_ID = "userTask";
	  private const string END_ID = "end";
	  private const string MESSAGE_ID = "messageId";
	  private const string MESSAGE_NAME = "messageName";

	  private new string deploymentId;

	  public virtual void testProcessStartEvent()
	  {
		deployAndStartTestProcess(PROCESS_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		assertFlowElementIs(typeof(StartEvent));
		sendMessage();
		completeTask();
	  }

	  public virtual void testStartEventEndEvent()
	  {
		deployAndStartTestProcess(START_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		assertFlowElementIs(typeof(StartEvent));
		sendMessage();
		completeTask();
	  }

	  public virtual void testSequenceFlowTakeEvent()
	  {
		deployAndStartTestProcess(SEQUENCE_FLOW_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE);
		assertFlowElementIs(typeof(SequenceFlow));
		sendMessage();
		completeTask();
	  }

	  public virtual void testIntermediateCatchEventStartEvent()
	  {
		deployAndStartTestProcess(CATCH_EVENT_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		assertFlowElementIs(typeof(IntermediateCatchEvent));
		sendMessage();
		completeTask();
	  }

	  public virtual void testIntermediateCatchEventEndEvent()
	  {
		deployAndStartTestProcess(CATCH_EVENT_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		assertNotNotified();
		sendMessage();
		assertFlowElementIs(typeof(IntermediateCatchEvent));
		completeTask();
	  }

	  public virtual void testGatewayStartEvent()
	  {
		deployAndStartTestProcess(GATEWAY_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		assertNotNotified();
		sendMessage();
		assertFlowElementIs(typeof(Gateway));
		completeTask();
	  }

	  public virtual void testGatewayEndEvent()
	  {
		deployAndStartTestProcess(GATEWAY_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		assertNotNotified();
		sendMessage();
		assertFlowElementIs(typeof(ParallelGateway));
		completeTask();
	  }

	  public virtual void testUserTaskStartEvent()
	  {
		deployAndStartTestProcess(USER_TASK_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		assertNotNotified();
		sendMessage();
		assertFlowElementIs(typeof(UserTask));
		completeTask();
	  }

	  public virtual void testUserTaskEndEvent()
	  {
		deployAndStartTestProcess(USER_TASK_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		assertNotNotified();
		sendMessage();
		completeTask();
		assertFlowElementIs(typeof(UserTask));
	  }

	  public virtual void testEndEventStartEvent()
	  {
		deployAndStartTestProcess(END_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
		assertNotNotified();
		sendMessage();
		completeTask();
		assertFlowElementIs(typeof(EndEvent));
	  }

	  public virtual void testProcessEndEvent()
	  {
		deployAndStartTestProcess(PROCESS_ID, org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
		assertNotNotified();
		sendMessage();
		completeTask();
		assertFlowElementIs(typeof(EndEvent));
	  }

	  private void assertNotNotified()
	  {
		assertNull(ModelExecutionContextExecutionListener.modelInstance);
		assertNull(ModelExecutionContextExecutionListener.flowElement);
	  }

	  private void assertFlowElementIs(Type elementClass)
	  {
		BpmnModelInstance modelInstance = ModelExecutionContextExecutionListener.modelInstance;
		assertNotNull(modelInstance);

		Model model = modelInstance.Model;
		ICollection<ModelElementInstance> events = modelInstance.getModelElementsByType(model.getType(typeof(Event)));
		assertEquals(3, events.Count);
		ICollection<ModelElementInstance> gateways = modelInstance.getModelElementsByType(model.getType(typeof(Gateway)));
		assertEquals(1, gateways.Count);
		ICollection<ModelElementInstance> tasks = modelInstance.getModelElementsByType(model.getType(typeof(Task)));
		assertEquals(1, tasks.Count);

		FlowElement flowElement = ModelExecutionContextExecutionListener.flowElement;
		assertNotNull(flowElement);
		assertTrue(elementClass.IsAssignableFrom(flowElement.GetType()));
	  }

	  private void sendMessage()
	  {
		runtimeService.correlateMessage(MESSAGE_NAME);
	  }

	  private void completeTask()
	  {
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);
	  }

	  private void deployAndStartTestProcess(string elementId, string eventName)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent(START_ID).sequenceFlowId(SEQUENCE_FLOW_ID).intermediateCatchEvent(CATCH_EVENT_ID).parallelGateway(GATEWAY_ID).userTask(USER_TASK_ID).endEvent(END_ID).done();

		addMessageEventDefinition((CatchEvent) modelInstance.getModelElementById(CATCH_EVENT_ID));
		addExecutionListener((BaseElement) modelInstance.getModelElementById(elementId), eventName);
		deployAndStartProcess(modelInstance);
	  }

	  private void addMessageEventDefinition(CatchEvent catchEvent)
	  {
		BpmnModelInstance modelInstance = (BpmnModelInstance) catchEvent.ModelInstance;
		Message message = modelInstance.newInstance(typeof(Message));
		message.Id = MESSAGE_ID;
		message.Name = MESSAGE_NAME;
		modelInstance.Definitions.addChildElement(message);
		MessageEventDefinition messageEventDefinition = modelInstance.newInstance(typeof(MessageEventDefinition));
		messageEventDefinition.Message = message;
		catchEvent.EventDefinitions.add(messageEventDefinition);
	  }

	  private void addExecutionListener(BaseElement element, string eventName)
	  {
		ExtensionElements extensionElements = element.ModelInstance.newInstance(typeof(ExtensionElements));
		ModelElementInstance executionListener = extensionElements.addExtensionElement(CAMUNDA_NS, "executionListener");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		executionListener.setAttributeValueNs(CAMUNDA_NS, "class", typeof(ModelExecutionContextExecutionListener).FullName);
		executionListener.setAttributeValueNs(CAMUNDA_NS, "event", eventName);
		element.ExtensionElements = extensionElements;
	  }

	  private void deployAndStartProcess(BpmnModelInstance modelInstance)
	  {
		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).deploy().Id;
		runtimeService.startProcessInstanceByKey(PROCESS_ID);
	  }

	  public virtual void tearDown()
	  {
		ModelExecutionContextExecutionListener.clear();
		repositoryService.deleteDeployment(deploymentId, true);
	  }

	}

}