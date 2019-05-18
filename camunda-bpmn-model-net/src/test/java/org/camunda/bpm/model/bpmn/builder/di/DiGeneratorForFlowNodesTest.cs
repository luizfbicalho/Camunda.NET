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
namespace org.camunda.bpm.model.bpmn.builder.di
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.BOUNDARY_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.CALL_ACTIVITY_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.CATCH_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.CONDITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.END_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEND_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SERVICE_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.START_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SUB_PROCESS_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TEST_CONDITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TRANSACTION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.USER_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using BpmnDiagram = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnDiagram;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using After = org.junit.After;
	using Test = org.junit.Test;

	public class DiGeneratorForFlowNodesTest
	{

	  private BpmnModelInstance instance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void validateModel()
	  {
		if (instance != null)
		{
		  Bpmn.validateModel(instance);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGeneratePlaneForProcess()
	  public virtual void shouldGeneratePlaneForProcess()
	  {

		// when
		instance = Bpmn.createExecutableProcess("process").done();

		// then
		ICollection<BpmnDiagram> bpmnDiagrams = instance.getModelElementsByType(typeof(BpmnDiagram));
		assertEquals(1, bpmnDiagrams.Count);

		BpmnDiagram diagram = bpmnDiagrams.GetEnumerator().next();
		assertNotNull(diagram.Id);

		assertNotNull(diagram.BpmnPlane);
		assertEquals(diagram.BpmnPlane.BpmnElement, instance.getModelElementById("process"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForStartEvent()
	  public virtual void shouldGenerateShapeForStartEvent()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertEventShapeProperties(START_EVENT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForUserTask()
	  public virtual void shouldGenerateShapeForUserTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(USER_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForSendTask()
	  public virtual void shouldGenerateShapeForSendTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).sendTask(SEND_TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(SEND_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForServiceTask()
	  public virtual void shouldGenerateShapeForServiceTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).serviceTask(SERVICE_TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(SERVICE_TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForReceiveTask()
	  public virtual void shouldGenerateShapeForReceiveTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).receiveTask(TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForManualTask()
	  public virtual void shouldGenerateShapeForManualTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).manualTask(TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForBusinessRuleTask()
	  public virtual void shouldGenerateShapeForBusinessRuleTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).businessRuleTask(TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForScriptTask()
	  public virtual void shouldGenerateShapeForScriptTask()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).scriptTask(TASK_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertTaskShapeProperties(TASK_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForCatchingIntermediateEvent()
	  public virtual void shouldGenerateShapeForCatchingIntermediateEvent()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).intermediateCatchEvent(CATCH_ID).endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertEventShapeProperties(CATCH_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForBoundaryIntermediateEvent()
	  public virtual void shouldGenerateShapeForBoundaryIntermediateEvent()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).boundaryEvent(BOUNDARY_ID).conditionalEventDefinition(CONDITION_ID).condition(TEST_CONDITION).conditionalEventDefinitionDone().endEvent().done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(5, allShapes.Count);

		assertEventShapeProperties(BOUNDARY_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForThrowingIntermediateEvent()
	  public virtual void shouldGenerateShapeForThrowingIntermediateEvent()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).intermediateThrowEvent("inter").endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertEventShapeProperties("inter");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForEndEvent()
	  public virtual void shouldGenerateShapeForEndEvent()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(2, allShapes.Count);

		assertEventShapeProperties(END_EVENT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForBlankSubProcess()
	  public virtual void shouldGenerateShapeForBlankSubProcess()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		BpmnShape bpmnShapeSubProcess = findBpmnShape(SUB_PROCESS_ID);
		assertNotNull(bpmnShapeSubProcess);
		assertSubProcessSize(bpmnShapeSubProcess);
		assertTrue(bpmnShapeSubProcess.Expanded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapesForNestedFlowNodes()
	  public virtual void shouldGenerateShapesForNestedFlowNodes()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").userTask("innerUserTask").endEvent("innerEndEvent").subProcessDone().endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(6, allShapes.Count);

		assertEventShapeProperties("innerStartEvent");
		assertTaskShapeProperties("innerUserTask");
		assertEventShapeProperties("innerEndEvent");

		BpmnShape bpmnShapeSubProcess = findBpmnShape(SUB_PROCESS_ID);
		assertNotNull(bpmnShapeSubProcess);
		assertTrue(bpmnShapeSubProcess.Expanded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForEventSubProcess()
	  public virtual void shouldGenerateShapeForEventSubProcess()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).endEvent(END_EVENT_ID).subProcess(SUB_PROCESS_ID).triggerByEvent().embeddedSubProcess().startEvent("innerStartEvent").endEvent("innerEndEvent").subProcessDone().done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(5, allShapes.Count);

		assertEventShapeProperties("innerStartEvent");
		assertEventShapeProperties("innerEndEvent");

		BpmnShape bpmnShapeEventSubProcess = findBpmnShape(SUB_PROCESS_ID);
		assertNotNull(bpmnShapeEventSubProcess);
		assertTrue(bpmnShapeEventSubProcess.Expanded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForCallActivity()
	  public virtual void shouldGenerateShapeForCallActivity()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).callActivity(CALL_ACTIVITY_ID).endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertTaskShapeProperties(CALL_ACTIVITY_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForTransaction()
	  public virtual void shouldGenerateShapeForTransaction()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).transaction(TRANSACTION_ID).embeddedSubProcess().startEvent("innerStartEvent").userTask("innerUserTask").endEvent("innerEndEvent").transactionDone().endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(6, allShapes.Count);

		assertEventShapeProperties("innerStartEvent");
		assertTaskShapeProperties("innerUserTask");
		assertEventShapeProperties("innerEndEvent");

		BpmnShape bpmnShapeSubProcess = findBpmnShape(TRANSACTION_ID);
		assertNotNull(bpmnShapeSubProcess);
		assertTrue(bpmnShapeSubProcess.Expanded);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForParallelGateway()
	  public virtual void shouldGenerateShapeForParallelGateway()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).parallelGateway("and").endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertGatewayShapeProperties("and");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForInclusiveGateway()
	  public virtual void shouldGenerateShapeForInclusiveGateway()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).inclusiveGateway("inclusive").endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertGatewayShapeProperties("inclusive");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForEventBasedGateway()
	  public virtual void shouldGenerateShapeForEventBasedGateway()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).eventBasedGateway().id("eventBased").endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertGatewayShapeProperties("eventBased");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateShapeForExclusiveGateway()
	  public virtual void shouldGenerateShapeForExclusiveGateway()
	  {

		// given
		ProcessBuilder processBuilder = Bpmn.createExecutableProcess();

		// when
		instance = processBuilder.startEvent(START_EVENT_ID).exclusiveGateway("or").endEvent(END_EVENT_ID).done();

		// then
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));
		assertEquals(3, allShapes.Count);

		assertGatewayShapeProperties("or");
		BpmnShape bpmnShape = findBpmnShape("or");
		assertTrue(bpmnShape.MarkerVisible);
	  }

	  protected internal virtual void assertTaskShapeProperties(string id)
	  {
		BpmnShape bpmnShapeTask = findBpmnShape(id);
		assertNotNull(bpmnShapeTask);
		assertActivitySize(bpmnShapeTask);
	  }

	  protected internal virtual void assertEventShapeProperties(string id)
	  {
		BpmnShape bpmnShapeEvent = findBpmnShape(id);
		assertNotNull(bpmnShapeEvent);
		assertEventSize(bpmnShapeEvent);
	  }

	  protected internal virtual void assertGatewayShapeProperties(string id)
	  {
		BpmnShape bpmnShapeGateway = findBpmnShape(id);
		assertNotNull(bpmnShapeGateway);
		assertGatewaySize(bpmnShapeGateway);
	  }

	  protected internal virtual BpmnShape findBpmnShape(string id)
	  {
		ICollection<BpmnShape> allShapes = instance.getModelElementsByType(typeof(BpmnShape));

		IEnumerator<BpmnShape> iterator = allShapes.GetEnumerator();
		while (iterator.MoveNext())
		{
		  BpmnShape shape = iterator.Current;
		  if (shape.BpmnElement.Id.Equals(id))
		  {
			return shape;
		  }
		}
		return null;
	  }

	  protected internal virtual void assertEventSize(BpmnShape shape)
	  {
		assertSize(shape, 36, 36);
	  }

	  protected internal virtual void assertGatewaySize(BpmnShape shape)
	  {
		assertSize(shape, 50, 50);
	  }

	  protected internal virtual void assertSubProcessSize(BpmnShape shape)
	  {
		assertSize(shape, 200, 350);
	  }

	  protected internal virtual void assertActivitySize(BpmnShape shape)
	  {
		assertSize(shape, 80, 100);
	  }

	  protected internal virtual void assertSize(BpmnShape shape, int height, int width)
	  {
		assertThat(shape.Bounds.Height).isEqualTo(height);
		assertThat(shape.Bounds.Width).isEqualTo(width);
	  }

	}

}