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
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ProcessBuilder : AbstractProcessBuilder<ProcessBuilder>
	{

	  public ProcessBuilder(BpmnModelInstance modelInstance, Process process) : base(modelInstance, process, typeof(ProcessBuilder))
	  {
	  }

	  public virtual StartEventBuilder startEvent()
	  {
		return startEvent(null);
	  }

	  public virtual StartEventBuilder startEvent(string id)
	  {
		StartEvent start = createChild(typeof(StartEvent), id);
		BpmnShape bpmnShape = createBpmnShape(start);
		Coordinates = bpmnShape;
		return start.builder();
	  }

	  public virtual EventSubProcessBuilder eventSubProcess()
	  {
		return eventSubProcess(null);
	  }

	  public virtual EventSubProcessBuilder eventSubProcess(string id)
	  {
		// Create a subprocess, triggered by an event, and add it to modelInstance
		SubProcess subProcess = createChild(typeof(SubProcess), id);
		subProcess.TriggeredByEvent = true;

		// Create Bpmn shape so subprocess will be drawn
		BpmnShape targetBpmnShape = createBpmnShape(subProcess);
		//find the lowest shape in the process
		// place event sub process underneath
		EventSubProcessCoordinates = targetBpmnShape;

		resizeSubProcess(targetBpmnShape);

		// Return the eventSubProcessBuilder
		EventSubProcessBuilder eventSubProcessBuilder = new EventSubProcessBuilder(modelInstance, subProcess);
		return eventSubProcessBuilder;

	  }

	  protected internal override BpmnShape Coordinates
	  {
		  set
		  {
			Bounds bounds = value.Bounds;
			bounds.setX(100);
			bounds.setY(100);
		  }
	  }

	  protected internal virtual BpmnShape EventSubProcessCoordinates
	  {
		  set
		  {
			SubProcess eventSubProcess = (SubProcess) value.BpmnElement;
			Bounds targetBounds = value.Bounds;
			double lowestheight = 0;
    
			// find the lowest element in the model
			ICollection<BpmnShape> allShapes = modelInstance.getModelElementsByType(typeof(BpmnShape));
			foreach (BpmnShape shape in allShapes)
			{
			  Bounds bounds = shape.Bounds;
			  double bottom = bounds.getY() + bounds.getHeight();
			  if (bottom > lowestheight)
			  {
				lowestheight = bottom;
			  }
			}
    
			double? ycoord = lowestheight + 50.0;
			double? xcoord = 100.0;
    
			// move target
			targetBounds.setY(ycoord.Value);
			targetBounds.setX(xcoord.Value);
		  }
	  }
	}

}