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
namespace org.camunda.bpm.model.bpmn.builder
{

	using Activity = org.camunda.bpm.model.bpmn.instance.Activity;
	using BoundaryEvent = org.camunda.bpm.model.bpmn.instance.BoundaryEvent;
	using MultiInstanceLoopCharacteristics = org.camunda.bpm.model.bpmn.instance.MultiInstanceLoopCharacteristics;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using CamundaInputOutput = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputOutput;
	using CamundaInputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaInputParameter;
	using CamundaOutputParameter = org.camunda.bpm.model.bpmn.instance.camunda.CamundaOutputParameter;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractActivityBuilder<B, E> : AbstractFlowNodeBuilder<B, E> where B : AbstractActivityBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.Activity
	{

	  protected internal AbstractActivityBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public virtual BoundaryEventBuilder boundaryEvent()
	  {
		return boundaryEvent(null);
	  }

	  public virtual BoundaryEventBuilder boundaryEvent(string id)
	  {
		BoundaryEvent boundaryEvent = createSibling(typeof(BoundaryEvent), id);
		boundaryEvent.AttachedTo = element;

		BpmnShape boundaryEventBpmnShape = createBpmnShape(boundaryEvent);
		BoundaryEventCoordinates = boundaryEventBpmnShape;

		return boundaryEvent.builder();
	  }

	  public virtual MultiInstanceLoopCharacteristicsBuilder multiInstance()
	  {
		MultiInstanceLoopCharacteristics miCharacteristics = createChild(typeof(MultiInstanceLoopCharacteristics));

		return miCharacteristics.builder();
	  }

	  /// <summary>
	  /// Creates a new camunda input parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the input parameter </param>
	  /// <param name="value"> the value of the input parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInputParameter(string name, string value)
	  {
		CamundaInputOutput camundaInputOutput = getCreateSingleExtensionElement(typeof(CamundaInputOutput));

		CamundaInputParameter camundaInputParameter = createChild(camundaInputOutput, typeof(CamundaInputParameter));
		camundaInputParameter.CamundaName = name;
		camundaInputParameter.TextContent = value;

		return myself;
	  }

	  /// <summary>
	  /// Creates a new camunda output parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the output parameter </param>
	  /// <param name="value"> the value of the output parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaOutputParameter(string name, string value)
	  {
		CamundaInputOutput camundaInputOutput = getCreateSingleExtensionElement(typeof(CamundaInputOutput));

		CamundaOutputParameter camundaOutputParameter = createChild(camundaInputOutput, typeof(CamundaOutputParameter));
		camundaOutputParameter.CamundaName = name;
		camundaOutputParameter.TextContent = value;

		return myself;
	  }

	  protected internal virtual double calculateXCoordinate(Bounds boundaryEventBounds)
	  {
		BpmnShape attachedToElement = findBpmnShape(element);

		double x = 0;

		if (attachedToElement != null)
		{

		  Bounds attachedToBounds = attachedToElement.Bounds;

		  ICollection<BoundaryEvent> boundaryEvents = element.ParentElement.getChildElementsByType(typeof(BoundaryEvent));
		  ICollection<BoundaryEvent> attachedBoundaryEvents = new List<BoundaryEvent>();

		  IEnumerator<BoundaryEvent> iterator = boundaryEvents.GetEnumerator();
		  while (iterator.MoveNext())
		  {
			BoundaryEvent tmp = iterator.Current;
			if (tmp.AttachedTo.Equals(element))
			{
			  attachedBoundaryEvents.Add(tmp);
			}
		  }

		  double attachedToX = attachedToBounds.getX().Value;
		  double attachedToWidth = attachedToBounds.getWidth().Value;
		  double boundaryWidth = boundaryEventBounds.getWidth().Value;

		  switch (attachedBoundaryEvents.Count)
		  {
			case 2:
			{
			  x = attachedToX + attachedToWidth / 2 + boundaryWidth / 2;
			  break;
			}
			case 3:
			{
			  x = attachedToX + attachedToWidth / 2 - 1.5 * boundaryWidth;
			  break;
			}
			default:
			{
			  x = attachedToX + attachedToWidth / 2 - boundaryWidth / 2;
			  break;
			}
		  }

		}

		return x;
	  }

	  protected internal virtual BpmnShape BoundaryEventCoordinates
	  {
		  set
		  {
			BpmnShape activity = findBpmnShape(element);
			Bounds boundaryBounds = value.Bounds;
    
			double x = 0;
			double y = 0;
    
			if (activity != null)
			{
			  Bounds activityBounds = activity.Bounds;
			  double activityY = activityBounds.getY().Value;
			  double activityHeight = activityBounds.getHeight().Value;
			  double boundaryHeight = boundaryBounds.getHeight().Value;
			  x = calculateXCoordinate(boundaryBounds);
			  y = activityY + activityHeight - boundaryHeight / 2;
			}
    
			boundaryBounds.setX(x);
			boundaryBounds.setY(y);
		  }
	  }

	}

}