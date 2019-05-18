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
	using Activity = org.camunda.bpm.model.bpmn.instance.Activity;
	using BoundaryEvent = org.camunda.bpm.model.bpmn.instance.BoundaryEvent;
	using CompensateEventDefinition = org.camunda.bpm.model.bpmn.instance.CompensateEventDefinition;
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;

	public abstract class AbstractCompensateEventDefinitionBuilder<B> : AbstractRootElementBuilder<B, CompensateEventDefinition> where B : AbstractCompensateEventDefinitionBuilder<B>
	{

	  public AbstractCompensateEventDefinitionBuilder(BpmnModelInstance modelInstance, CompensateEventDefinition element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public override B id(string identifier)
	  {
		return base.id(identifier);
	  }

	  public virtual B activityRef(string activityId)
	  {
		Activity activity = modelInstance.getModelElementById(activityId);

		if (activity == null)
		{
		  throw new BpmnModelException("Activity with id '" + activityId + "' does not exist");
		}
		Event @event = (Event) element.ParentElement;
		if (activity.ParentElement != @event.ParentElement)
		{
		  throw new BpmnModelException("Activity with id '" + activityId + "' must be in the same scope as '" + @event.Id + "'");
		}

		element.Activity = activity;
		return myself;
	  }

	  public virtual B waitForCompletion(bool waitForCompletion)
	  {
		element.WaitForCompletion = waitForCompletion;
		return myself;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractFlowNodeBuilder> T compensateEventDefinitionDone()
	  public virtual T compensateEventDefinitionDone<T>() where T : AbstractFlowNodeBuilder
	  {
		return (T)((Event) element.ParentElement).builder();
	  }
	}

}