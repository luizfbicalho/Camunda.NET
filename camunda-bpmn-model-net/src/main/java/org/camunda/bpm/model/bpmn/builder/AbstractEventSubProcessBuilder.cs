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


	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;

	public class AbstractEventSubProcessBuilder <B> : AbstractFlowElementBuilder<B, SubProcess> where B : AbstractEventSubProcessBuilder<B>
	{

	  protected internal AbstractEventSubProcessBuilder(BpmnModelInstance modelInstance, SubProcess element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public virtual StartEventBuilder startEvent()
	  {
		return startEvent(null);
	  }

	  public virtual StartEventBuilder startEvent(string id)
	  {
		StartEvent start = createChild(typeof(StartEvent), id);

		BpmnShape startShape = createBpmnShape(start);
		BpmnShape subProcessShape = findBpmnShape(Element);

		if (subProcessShape != null)
		{
		  Bounds subProcessBounds = subProcessShape.Bounds;
		  Bounds startBounds = startShape.Bounds;

		  double subProcessX = subProcessBounds.getX().Value;
		  double subProcessY = subProcessBounds.getY().Value;
		  double subProcessHeight = subProcessBounds.getHeight().Value;
		  double startHeight = startBounds.getHeight().Value;

		  startBounds.setX(subProcessX + SPACE);
		  startBounds.setY(subProcessY + subProcessHeight / 2 - startHeight / 2);
		}

		return start.builder();
	  }
	}



}