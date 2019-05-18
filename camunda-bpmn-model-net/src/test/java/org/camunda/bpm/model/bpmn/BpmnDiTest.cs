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
namespace org.camunda.bpm.model.bpmn
{
	using org.camunda.bpm.model.bpmn.instance;
	using org.camunda.bpm.model.bpmn.instance.bpmndi;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;
	using Font = org.camunda.bpm.model.bpmn.instance.dc.Font;
	using DiagramElement = org.camunda.bpm.model.bpmn.instance.di.DiagramElement;
	using Waypoint = org.camunda.bpm.model.bpmn.instance.di.Waypoint;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
	using static org.camunda.bpm.model.bpmn.BpmnTestConstants;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnDiTest
	{

	  private BpmnModelInstance modelInstance;
	  private Collaboration collaboration;
	  private Participant participant;
	  private Process process;
	  private StartEvent startEvent;
	  private ServiceTask serviceTask;
	  private ExclusiveGateway exclusiveGateway;
	  private SequenceFlow sequenceFlow;
	  private MessageFlow messageFlow;
	  private DataInputAssociation dataInputAssociation;
	  private Association association;
	  private EndEvent endEvent;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void parseModel()
	  public virtual void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(this.GetType().getResourceAsStream(this.GetType().Name + ".xml"));
		collaboration = modelInstance.getModelElementById(COLLABORATION_ID);
		participant = modelInstance.getModelElementById(PARTICIPANT_ID + 1);
		process = modelInstance.getModelElementById(PROCESS_ID + 1);
		serviceTask = modelInstance.getModelElementById(SERVICE_TASK_ID);
		exclusiveGateway = modelInstance.getModelElementById(EXCLUSIVE_GATEWAY);
		startEvent = modelInstance.getModelElementById(START_EVENT_ID + 2);
		sequenceFlow = modelInstance.getModelElementById(SEQUENCE_FLOW_ID + 3);
		messageFlow = modelInstance.getModelElementById(MESSAGE_FLOW_ID);
		dataInputAssociation = modelInstance.getModelElementById(DATA_INPUT_ASSOCIATION_ID);
		association = modelInstance.getModelElementById(ASSOCIATION_ID);
		endEvent = modelInstance.getModelElementById(END_EVENT_ID + 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnDiagram()
	  public virtual void testBpmnDiagram()
	  {
		ICollection<BpmnDiagram> diagrams = modelInstance.getModelElementsByType(typeof(BpmnDiagram));
		assertThat(diagrams).hasSize(1);
		BpmnDiagram diagram = diagrams.GetEnumerator().next();
		assertThat(diagram.BpmnPlane).NotNull;
		assertThat(diagram.BpmnPlane.BpmnElement).isEqualTo(collaboration);
		assertThat(diagram.BpmnLabelStyles).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnPane()
	  public virtual void testBpmnPane()
	  {
		DiagramElement diagramElement = collaboration.DiagramElement;
		assertThat(diagramElement).NotNull.isInstanceOf(typeof(BpmnPlane));
		BpmnPlane bpmnPlane = (BpmnPlane) diagramElement;
		assertThat(bpmnPlane.BpmnElement).isEqualTo(collaboration);
		assertThat(bpmnPlane.getChildElementsByType(typeof(DiagramElement))).NotEmpty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnLabelStyle()
	  public virtual void testBpmnLabelStyle()
	  {
		BpmnLabelStyle labelStyle = modelInstance.getModelElementsByType(typeof(BpmnLabelStyle)).GetEnumerator().next();
		Font font = labelStyle.Font;
		assertThat(font).NotNull;
		assertThat(font.Name).isEqualTo("Arial");
		assertThat(font.Size).isEqualTo(8.0);
		assertThat(font.Bold).True;
		assertThat(font.Italic).False;
		assertThat(font.StrikeThrough).False;
		assertThat(font.Underline).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnShape()
	  public virtual void testBpmnShape()
	  {
		BpmnShape shape = serviceTask.DiagramElement;
		assertThat(shape.BpmnElement).isEqualTo(serviceTask);
		assertThat(shape.BpmnLabel).Null;
		assertThat(shape.Expanded).False;
		assertThat(shape.Horizontal).False;
		assertThat(shape.MarkerVisible).False;
		assertThat(shape.MessageVisible).False;
		assertThat(shape.ParticipantBandKind).Null;
		assertThat(shape.ChoreographyActivityShape).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnLabel()
	  public virtual void testBpmnLabel()
	  {
		BpmnShape shape = startEvent.DiagramElement;
		assertThat(shape.BpmnElement).isEqualTo(startEvent);
		assertThat(shape.BpmnLabel).NotNull;

		BpmnLabel label = shape.BpmnLabel;
		assertThat(label.LabelStyle).Null;
		assertThat(label.Bounds).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBpmnEdge()
	  public virtual void testBpmnEdge()
	  {
		BpmnEdge edge = sequenceFlow.DiagramElement;
		assertThat(edge.BpmnElement).isEqualTo(sequenceFlow);
		assertThat(edge.BpmnLabel).Null;
		assertThat(edge.MessageVisibleKind).Null;
		assertThat(edge.SourceElement).isInstanceOf(typeof(BpmnShape));
		assertThat(((BpmnShape) edge.SourceElement).BpmnElement).isEqualTo(startEvent);
		assertThat(edge.TargetElement).isInstanceOf(typeof(BpmnShape));
		assertThat(((BpmnShape) edge.TargetElement).BpmnElement).isEqualTo(endEvent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDiagramElementTypes()
	  public virtual void testDiagramElementTypes()
	  {
		assertThat(collaboration.DiagramElement).isInstanceOf(typeof(BpmnPlane));
		assertThat(process.DiagramElement).Null;
		assertThat(participant.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(participant.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(startEvent.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(serviceTask.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(exclusiveGateway.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(endEvent.DiagramElement).isInstanceOf(typeof(BpmnShape));
		assertThat(sequenceFlow.DiagramElement).isInstanceOf(typeof(BpmnEdge));
		assertThat(messageFlow.DiagramElement).isInstanceOf(typeof(BpmnEdge));
		assertThat(dataInputAssociation.DiagramElement).isInstanceOf(typeof(BpmnEdge));
		assertThat(association.DiagramElement).isInstanceOf(typeof(BpmnEdge));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotRemoveBpmElementReference()
	  public virtual void shouldNotRemoveBpmElementReference()
	  {
		assertThat(startEvent.Outgoing).contains(sequenceFlow);
		assertThat(endEvent.Incoming).contains(sequenceFlow);

		BpmnEdge edge = sequenceFlow.DiagramElement;
		assertThat(edge.BpmnElement).isEqualTo(sequenceFlow);

		startEvent.Outgoing.remove(sequenceFlow);
		endEvent.Incoming.remove(sequenceFlow);

		assertThat(startEvent.Outgoing).doesNotContain(sequenceFlow);
		assertThat(endEvent.Incoming).doesNotContain(sequenceFlow);

		assertThat(edge.BpmnElement).isEqualTo(sequenceFlow);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCreateValidBpmnDi()
	  public virtual void shouldCreateValidBpmnDi()
	  {
		modelInstance = Bpmn.createProcess("process").startEvent("start").sequenceFlowId("flow").endEvent("end").done();

		process = modelInstance.getModelElementById("process");
		startEvent = modelInstance.getModelElementById("start");
		sequenceFlow = modelInstance.getModelElementById("flow");
		endEvent = modelInstance.getModelElementById("end");

		// create bpmn diagram
		BpmnDiagram bpmnDiagram = modelInstance.newInstance(typeof(BpmnDiagram));
		bpmnDiagram.Id = "diagram";
		bpmnDiagram.Name = "diagram";
		bpmnDiagram.Documentation = "bpmn diagram element";
		bpmnDiagram.Resolution = 120.0;
		modelInstance.Definitions.addChildElement(bpmnDiagram);

		// create plane for process
		BpmnPlane processPlane = modelInstance.newInstance(typeof(BpmnPlane));
		processPlane.Id = "plane";
		processPlane.BpmnElement = process;
		bpmnDiagram.BpmnPlane = processPlane;

		// create shape for start event
		BpmnShape startEventShape = modelInstance.newInstance(typeof(BpmnShape));
		startEventShape.Id = "startShape";
		startEventShape.BpmnElement = startEvent;
		processPlane.DiagramElements.Add(startEventShape);

		// create bounds for start event shape
		Bounds startEventBounds = modelInstance.newInstance(typeof(Bounds));
		startEventBounds.setHeight(36.0);
		startEventBounds.setWidth(36.0);
		startEventBounds.setX(632.0);
		startEventBounds.setY(312.0);
		startEventShape.Bounds = startEventBounds;

		// create shape for end event
		BpmnShape endEventShape = modelInstance.newInstance(typeof(BpmnShape));
		endEventShape.Id = "endShape";
		endEventShape.BpmnElement = endEvent;
		processPlane.DiagramElements.Add(endEventShape);

		// create bounds for end event shape
		Bounds endEventBounds = modelInstance.newInstance(typeof(Bounds));
		endEventBounds.setHeight(36.0);
		endEventBounds.setWidth(36.0);
		endEventBounds.setX(718.0);
		endEventBounds.setY(312.0);
		endEventShape.Bounds = endEventBounds;

		// create edge for sequence flow
		BpmnEdge flowEdge = modelInstance.newInstance(typeof(BpmnEdge));
		flowEdge.Id = "flowEdge";
		flowEdge.BpmnElement = sequenceFlow;
		flowEdge.SourceElement = startEventShape;
		flowEdge.TargetElement = endEventShape;
		processPlane.DiagramElements.Add(flowEdge);

		// create waypoints for sequence flow edge
		Waypoint startWaypoint = modelInstance.newInstance(typeof(Waypoint));
		startWaypoint.X = 668.0;
		startWaypoint.Y = 330.0;
		flowEdge.Waypoints.add(startWaypoint);

		Waypoint endWaypoint = modelInstance.newInstance(typeof(Waypoint));
		endWaypoint.X = 718.0;
		endWaypoint.Y = 330.0;
		flowEdge.Waypoints.add(endWaypoint);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel()
	  public virtual void validateModel()
	  {
		Bpmn.validateModel(modelInstance);
	  }

	}

}