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
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.END_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEND_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEQUENCE_FLOW_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SERVICE_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.START_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SUB_PROCESS_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.USER_TASK_ID;


	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;
	using Waypoint = org.camunda.bpm.model.bpmn.instance.di.Waypoint;
	using Test = org.junit.Test;

	public class CoordinatesGenerationTest
	{

	  private BpmnModelInstance instance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceStartEvent()
	  public virtual void shouldPlaceStartEvent()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).done();

		Bounds startBounds = findBpmnShape(START_EVENT_ID).Bounds;
		assertShapeCoordinates(startBounds, 100, 100);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceUserTask()
	  public virtual void shouldPlaceUserTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).userTask(USER_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceSendTask()
	  public virtual void shouldPlaceSendTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).sendTask(SEND_TASK_ID).done();

		Bounds sendTaskBounds = findBpmnShape(SEND_TASK_ID).Bounds;
		assertShapeCoordinates(sendTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceServiceTask()
	  public virtual void shouldPlaceServiceTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).serviceTask(SERVICE_TASK_ID).done();

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceReceiveTask()
	  public virtual void shouldPlaceReceiveTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).receiveTask(TASK_ID).done();

		Bounds receiveTaskBounds = findBpmnShape(TASK_ID).Bounds;
		assertShapeCoordinates(receiveTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManualTask()
	  public virtual void shouldPlaceManualTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).manualTask(TASK_ID).done();

		Bounds manualTaskBounds = findBpmnShape(TASK_ID).Bounds;
		assertShapeCoordinates(manualTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceBusinessRuleTask()
	  public virtual void shouldPlaceBusinessRuleTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).businessRuleTask(TASK_ID).done();

		Bounds businessRuleTaskBounds = findBpmnShape(TASK_ID).Bounds;
		assertShapeCoordinates(businessRuleTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceScriptTask()
	  public virtual void shouldPlaceScriptTask()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).scriptTask(TASK_ID).done();

		Bounds scriptTaskBounds = findBpmnShape(TASK_ID).Bounds;
		assertShapeCoordinates(scriptTaskBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceCatchingIntermediateEvent()
	  public virtual void shouldPlaceCatchingIntermediateEvent()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).intermediateCatchEvent("id").done();

		Bounds catchEventBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(catchEventBounds, 186, 100);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThrowingIntermediateEvent()
	  public virtual void shouldPlaceThrowingIntermediateEvent()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).intermediateThrowEvent("id").done();

		Bounds throwEventBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(throwEventBounds, 186, 100);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceEndEvent()
	  public virtual void shouldPlaceEndEvent()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).done();

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 186, 100);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceCallActivity()
	  public virtual void shouldPlaceCallActivity()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).callActivity("id").done();

		Bounds callActivityBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(callActivityBounds, 186, 78);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceExclusiveGateway()
	  public virtual void shouldPlaceExclusiveGateway()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).exclusiveGateway("id").done();

		Bounds gatewayBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(gatewayBounds, 186, 93);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceInclusiveGateway()
	  public virtual void shouldPlaceInclusiveGateway()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).inclusiveGateway("id").done();

		Bounds gatewayBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(gatewayBounds, 186, 93);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceParallelGateway()
	  public virtual void shouldPlaceParallelGateway()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).parallelGateway("id").done();

		Bounds gatewayBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(gatewayBounds, 186, 93);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceEventBasedGateway()
	  public virtual void shouldPlaceEventBasedGateway()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).eventBasedGateway().id("id").done();

		Bounds gatewayBounds = findBpmnShape("id").Bounds;
		assertShapeCoordinates(gatewayBounds, 186, 93);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceBlankSubProcess()
	  public virtual void shouldPlaceBlankSubProcess()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).subProcess(SUB_PROCESS_ID).done();

		Bounds subProcessBounds = findBpmnShape(SUB_PROCESS_ID).Bounds;
		assertShapeCoordinates(subProcessBounds, 186, 18);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 136, 118);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 186, 118);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceBoundaryEventForTask()
	  public virtual void shouldPlaceBoundaryEventForTask()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).boundaryEvent("boundary").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).endEvent().done();

		Bounds boundaryEventBounds = findBpmnShape("boundary").Bounds;
		assertShapeCoordinates(boundaryEventBounds, 218, 140);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceFollowingFlowNodeProperlyForTask()
	  public virtual void shouldPlaceFollowingFlowNodeProperlyForTask()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).boundaryEvent("boundary").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).endEvent().done();

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 266.5, 208);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 236, 176);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 266.5, 226);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBoundaryEventsForTask()
	  public virtual void shouldPlaceTwoBoundaryEventsForTask()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).boundaryEvent("boundary1").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).endEvent().moveToActivity(USER_TASK_ID).boundaryEvent("boundary2").done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 218, 140);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 254, 140);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBoundaryEventsForTask()
	  public virtual void shouldPlaceThreeBoundaryEventsForTask()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).boundaryEvent("boundary1").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).endEvent().moveToActivity(USER_TASK_ID).boundaryEvent("boundary2").moveToActivity(USER_TASK_ID).boundaryEvent("boundary3").done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 218, 140);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 254, 140);

		Bounds boundaryEvent3Bounds = findBpmnShape("boundary3").Bounds;
		assertShapeCoordinates(boundaryEvent3Bounds, 182, 140);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBoundaryEventsForTask()
	  public virtual void shouldPlaceManyBoundaryEventsForTask()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).userTask(USER_TASK_ID).boundaryEvent("boundary1").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(USER_TASK_ID).endEvent().moveToActivity(USER_TASK_ID).boundaryEvent("boundary2").moveToActivity(USER_TASK_ID).boundaryEvent("boundary3").moveToActivity(USER_TASK_ID).boundaryEvent("boundary4").done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 218, 140);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 254, 140);

		Bounds boundaryEvent3Bounds = findBpmnShape("boundary3").Bounds;
		assertShapeCoordinates(boundaryEvent3Bounds, 182, 140);

		Bounds boundaryEvent4Bounds = findBpmnShape("boundary4").Bounds;
		assertShapeCoordinates(boundaryEvent4Bounds, 218, 140);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceBoundaryEventForSubProcess()
	  public virtual void shouldPlaceBoundaryEventForSubProcess()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).boundaryEvent("boundary").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(SUB_PROCESS_ID).endEvent().done();

		Bounds boundaryEventBounds = findBpmnShape("boundary").Bounds;
		assertShapeCoordinates(boundaryEventBounds, 343, 200);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceFollowingFlowNodeForSubProcess()
	  public virtual void shouldPlaceFollowingFlowNodeForSubProcess()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).boundaryEvent("boundary").sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).moveToActivity(SUB_PROCESS_ID).endEvent().done();

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 391.5, 268);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge(SEQUENCE_FLOW_ID).Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 361, 236);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 391.5, 286);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBoundaryEventsForSubProcess()
	  public virtual void shouldPlaceTwoBoundaryEventsForSubProcess()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).boundaryEvent("boundary1").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary2").moveToActivity(SUB_PROCESS_ID).endEvent().done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 343, 200);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 379, 200);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBoundaryEventsForSubProcess()
	  public virtual void shouldPlaceThreeBoundaryEventsForSubProcess()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).boundaryEvent("boundary1").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary2").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary3").moveToActivity(SUB_PROCESS_ID).endEvent().done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 343, 200);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 379, 200);

		Bounds boundaryEvent3Bounds = findBpmnShape("boundary3").Bounds;
		assertShapeCoordinates(boundaryEvent3Bounds, 307, 200);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBoundaryEventsForSubProcess()
	  public virtual void shouldPlaceManyBoundaryEventsForSubProcess()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).boundaryEvent("boundary1").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary2").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary3").moveToActivity(SUB_PROCESS_ID).boundaryEvent("boundary4").moveToActivity(SUB_PROCESS_ID).endEvent().done();

		Bounds boundaryEvent1Bounds = findBpmnShape("boundary1").Bounds;
		assertShapeCoordinates(boundaryEvent1Bounds, 343, 200);

		Bounds boundaryEvent2Bounds = findBpmnShape("boundary2").Bounds;
		assertShapeCoordinates(boundaryEvent2Bounds, 379, 200);

		Bounds boundaryEvent3Bounds = findBpmnShape("boundary3").Bounds;
		assertShapeCoordinates(boundaryEvent3Bounds, 307, 200);

		Bounds boundaryEvent4Bounds = findBpmnShape("boundary4").Bounds;
		assertShapeCoordinates(boundaryEvent4Bounds, 343, 200);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBranchesForParallelGateway()
	  public virtual void shouldPlaceTwoBranchesForParallelGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).parallelGateway("id").sequenceFlowId("s1").userTask(USER_TASK_ID).moveToNode("id").sequenceFlowId("s2").endEvent(END_EVENT_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s2").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 226);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBranchesForParallelGateway()
	  public virtual void shouldPlaceThreeBranchesForParallelGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).parallelGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").sequenceFlowId("s1").serviceTask(SERVICE_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 334);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBranchesForParallelGateway()
	  public virtual void shouldPlaceManyBranchesForParallelGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).parallelGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").serviceTask(SERVICE_TASK_ID).moveToNode("id").sequenceFlowId("s1").sendTask(SEND_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		Bounds sendTaskBounds = findBpmnShape(SEND_TASK_ID).Bounds;
		assertShapeCoordinates(sendTaskBounds, 286, 424);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 464);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBranchesForExclusiveGateway()
	  public virtual void shouldPlaceTwoBranchesForExclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).exclusiveGateway("id").sequenceFlowId("s1").userTask(USER_TASK_ID).moveToNode("id").sequenceFlowId("s2").endEvent(END_EVENT_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s2").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 226);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBranchesForExclusiveGateway()
	  public virtual void shouldPlaceThreeBranchesForExclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).exclusiveGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").sequenceFlowId("s1").serviceTask(SERVICE_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 334);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBranchesForExclusiveGateway()
	  public virtual void shouldPlaceManyBranchesForExclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).exclusiveGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").serviceTask(SERVICE_TASK_ID).moveToNode("id").sequenceFlowId("s1").sendTask(SEND_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		Bounds sendTaskBounds = findBpmnShape(SEND_TASK_ID).Bounds;
		assertShapeCoordinates(sendTaskBounds, 286, 424);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 464);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBranchesForEventBasedGateway()
	  public virtual void shouldPlaceTwoBranchesForEventBasedGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).eventBasedGateway().id("id").sequenceFlowId("s1").userTask(USER_TASK_ID).moveToNode("id").sequenceFlowId("s2").endEvent(END_EVENT_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s2").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 226);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBranchesForEventBasedGateway()
	  public virtual void shouldPlaceThreeBranchesForEventBasedGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).eventBasedGateway().id("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").sequenceFlowId("s1").serviceTask(SERVICE_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 334);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBranchesForEventBasedGateway()
	  public virtual void shouldPlaceManyBranchesForEventBasedGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).eventBasedGateway().id("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").serviceTask(SERVICE_TASK_ID).moveToNode("id").sequenceFlowId("s1").sendTask(SEND_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		Bounds sendTaskBounds = findBpmnShape(SEND_TASK_ID).Bounds;
		assertShapeCoordinates(sendTaskBounds, 286, 424);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 464);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceTwoBranchesForInclusiveGateway()
	  public virtual void shouldPlaceTwoBranchesForInclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).inclusiveGateway("id").sequenceFlowId("s1").userTask(USER_TASK_ID).moveToNode("id").sequenceFlowId("s2").endEvent(END_EVENT_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s2").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 226);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceThreeBranchesForInclusiveGateway()
	  public virtual void shouldPlaceThreeBranchesForInclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).inclusiveGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").sequenceFlowId("s1").serviceTask(SERVICE_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 334);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceManyBranchesForInclusiveGateway()
	  public virtual void shouldPlaceManyBranchesForInclusiveGateway()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).inclusiveGateway("id").userTask(USER_TASK_ID).moveToNode("id").endEvent(END_EVENT_ID).moveToNode("id").serviceTask(SERVICE_TASK_ID).moveToNode("id").sequenceFlowId("s1").sendTask(SEND_TASK_ID).done();

		Bounds userTaskBounds = findBpmnShape(USER_TASK_ID).Bounds;
		assertShapeCoordinates(userTaskBounds, 286, 78);

		Bounds endEventBounds = findBpmnShape(END_EVENT_ID).Bounds;
		assertShapeCoordinates(endEventBounds, 286, 208);

		Bounds serviceTaskBounds = findBpmnShape(SERVICE_TASK_ID).Bounds;
		assertShapeCoordinates(serviceTaskBounds, 286, 294);

		Bounds sendTaskBounds = findBpmnShape(SEND_TASK_ID).Bounds;
		assertShapeCoordinates(sendTaskBounds, 286, 424);

		ICollection<Waypoint> sequenceFlowWaypoints = findBpmnEdge("s1").Waypoints;
		IEnumerator<Waypoint> iterator = sequenceFlowWaypoints.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Waypoint waypoint = iterator.next();
		assertWaypointCoordinates(waypoint, 211, 143);

		while (iterator.MoveNext())
		{
		  waypoint = iterator.Current;
		}

		assertWaypointCoordinates(waypoint, 286, 464);
	  }

	  public virtual void shouldPlaceStartEventWithinSubProcess()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").done();

		Bounds startEventBounds = findBpmnShape("innerStartEvent").Bounds;
		assertShapeCoordinates(startEventBounds, 236, 100);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAdjustSubProcessWidth()
	  public virtual void shouldAdjustSubProcessWidth()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").parallelGateway("innerParallelGateway").userTask("innerUserTask").endEvent("innerEndEvent").subProcessDone().done();

		Bounds subProcessBounds = findBpmnShape(SUB_PROCESS_ID).Bounds;
		assertThat(subProcessBounds.getWidth()).isEqualTo(472);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAdjustSubProcessWidthWithEmbeddedSubProcess()
	  public virtual void shouldAdjustSubProcessWidthWithEmbeddedSubProcess()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").subProcess("innerSubProcess").embeddedSubProcess().startEvent().userTask().userTask().endEvent().subProcessDone().endEvent("innerEndEvent").subProcessDone().done();

		Bounds subProcessBounds = findBpmnShape(SUB_PROCESS_ID).Bounds;
		assertThat(subProcessBounds.getWidth()).isEqualTo(794);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAdjustSubProcessHeight()
	  public virtual void shouldAdjustSubProcessHeight()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").parallelGateway("innerParallelGateway").endEvent("innerEndEvent").moveToNode("innerParallelGateway").userTask("innerUserTask").subProcessDone().done();

		Bounds subProcessBounds = findBpmnShape(SUB_PROCESS_ID).Bounds;
		assertThat(subProcessBounds.getHeight()).isEqualTo(298);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAdjustSubProcessHeightWithEmbeddedProcess()
	  public virtual void shouldAdjustSubProcessHeightWithEmbeddedProcess()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).subProcess(SUB_PROCESS_ID).embeddedSubProcess().startEvent("innerStartEvent").subProcess().embeddedSubProcess().startEvent().exclusiveGateway("id").userTask().moveToNode("id").endEvent().subProcessDone().endEvent("innerEndEvent").subProcessDone().endEvent().done();

		Bounds subProcessBounds = findBpmnShape(SUB_PROCESS_ID).Bounds;
		assertThat(subProcessBounds.getY()).isEqualTo(-32);
		assertThat(subProcessBounds.getHeight()).isEqualTo(376);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPlaceCompensation()
	  public virtual void shouldPlaceCompensation()
	  {
		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent().userTask("task").boundaryEvent("boundary").compensateEventDefinition().compensateEventDefinitionDone().compensationStart().userTask("compensate").name("compensate").compensationDone().userTask("task2").boundaryEvent("boundary2").compensateEventDefinition().compensateEventDefinitionDone().compensationStart().userTask("compensate2").name("compensate2").compensationDone().endEvent("theend").done();

		Bounds compensationBounds = findBpmnShape("compensate").Bounds;
		assertShapeCoordinates(compensationBounds, 266.5, 186);
		Bounds compensation2Bounds = findBpmnShape("compensate2").Bounds;
		assertShapeCoordinates(compensation2Bounds, 416.5, 186);
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

	  protected internal virtual BpmnEdge findBpmnEdge(string sequenceFlowId)
	  {
		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		IEnumerator<BpmnEdge> iterator = allEdges.GetEnumerator();

		while (iterator.MoveNext())
		{
		  BpmnEdge edge = iterator.Current;
		  if (edge.BpmnElement.Id.Equals(sequenceFlowId))
		  {
			return edge;
		  }
		}
		return null;
	  }

	  protected internal virtual void assertShapeCoordinates(Bounds bounds, double x, double y)
	  {
		assertThat(bounds.getX()).isEqualTo(x);
		assertThat(bounds.getY()).isEqualTo(y);
	  }

	  protected internal virtual void assertWaypointCoordinates(Waypoint waypoint, double x, double y)
	  {
		assertThat(x).isEqualTo(waypoint.X);
		assertThat(y).isEqualTo(waypoint.Y);
	  }
	}

}