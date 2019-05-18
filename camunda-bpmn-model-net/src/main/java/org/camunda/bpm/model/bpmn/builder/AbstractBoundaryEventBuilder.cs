using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;
	using Waypoint = org.camunda.bpm.model.bpmn.instance.di.Waypoint;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractBoundaryEventBuilder<B> : AbstractCatchEventBuilder<B, BoundaryEvent> where B : AbstractBoundaryEventBuilder<B>
	{

	  protected internal AbstractBoundaryEventBuilder(BpmnModelInstance modelInstance, BoundaryEvent element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Set if the boundary event cancels the attached activity.
	  /// </summary>
	  /// <param name="cancelActivity"> true if the boundary event cancels the activiy, false otherwise </param>
	  /// <returns> the builder object </returns>
	  public virtual B cancelActivity(bool? cancelActivity)
	  {
		element.CancelActivity = cancelActivity;

		return myself;
	  }

	  /// <summary>
	  /// Sets a catch all error definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B error()
	  {
		ErrorEventDefinition errorEventDefinition = createInstance(typeof(ErrorEventDefinition));
		element.EventDefinitions.add(errorEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual B error(string errorCode)
	  {
		ErrorEventDefinition errorEventDefinition = createErrorEventDefinition(errorCode);
		element.EventDefinitions.add(errorEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Creates an error event definition with an unique id
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition(string id)
	  {
		ErrorEventDefinition errorEventDefinition = createEmptyErrorEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  errorEventDefinition.Id = id;
		}

		element.EventDefinitions.add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Creates an error event definition
	  /// and returns a builder for the error event definition.
	  /// </summary>
	  /// <returns> the error event definition builder object </returns>
	  public virtual ErrorEventDefinitionBuilder errorEventDefinition()
	  {
		ErrorEventDefinition errorEventDefinition = createEmptyErrorEventDefinition();
		element.EventDefinitions.add(errorEventDefinition);
		return new ErrorEventDefinitionBuilder(modelInstance, errorEventDefinition);
	  }

	  /// <summary>
	  /// Sets a catch all escalation definition.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B escalation()
	  {
		EscalationEventDefinition escalationEventDefinition = createInstance(typeof(EscalationEventDefinition));
		element.EventDefinitions.add(escalationEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an escalation
	  /// with this code exists it will be used, otherwise a new escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual B escalation(string escalationCode)
	  {
		EscalationEventDefinition escalationEventDefinition = createEscalationEventDefinition(escalationCode);
		element.EventDefinitions.add(escalationEventDefinition);

		return myself;
	  }


	  protected internal override BpmnShape Coordinates
	  {
		  set
		  {
			BpmnShape source = findBpmnShape(element);
			Bounds shapeBounds = value.Bounds;
    
			double x = 0;
			double y = 0;
    
			if (source != null)
			{
			  Bounds sourceBounds = source.Bounds;
    
			  double sourceX = sourceBounds.getX().Value;
			  double sourceWidth = sourceBounds.getWidth().Value;
			  double sourceY = sourceBounds.getY().Value;
			  double sourceHeight = sourceBounds.getHeight().Value;
			  double targetHeight = shapeBounds.getHeight().Value;
    
			  x = sourceX + sourceWidth + SPACE / 4;
			  y = sourceY + sourceHeight - targetHeight / 2 + SPACE;
			}
    
			shapeBounds.setX(x);
			shapeBounds.setY(y);
		  }
	  }

	  protected internal override void setWaypointsWithSourceAndTarget(BpmnEdge edge, FlowNode edgeSource, FlowNode edgeTarget)
	  {
		BpmnShape source = findBpmnShape(edgeSource);
		BpmnShape target = findBpmnShape(edgeTarget);

		if (source != null && target != null)
		{
		  Bounds sourceBounds = source.Bounds;
		  Bounds targetBounds = target.Bounds;

		  double sourceX = sourceBounds.getX().Value;
		  double sourceY = sourceBounds.getY().Value;
		  double sourceWidth = sourceBounds.getWidth().Value;
		  double sourceHeight = sourceBounds.getHeight().Value;

		  double targetX = targetBounds.getX().Value;
		  double targetY = targetBounds.getY().Value;
		  double targetHeight = targetBounds.getHeight().Value;

		  Waypoint w1 = createInstance(typeof(Waypoint));
		  w1.X = sourceX + sourceWidth / 2;
		  w1.Y = sourceY + sourceHeight;

		  Waypoint w2 = createInstance(typeof(Waypoint));
		  w2.X = sourceX + sourceWidth / 2;
		  w2.Y = sourceY + sourceHeight + SPACE;

		  Waypoint w3 = createInstance(typeof(Waypoint));
		  w3.X = targetX;
		  w3.Y = targetY + targetHeight / 2;

		  edge.addChildElement(w1);
		  edge.addChildElement(w2);
		  edge.addChildElement(w3);
		}
	  }
	}

}