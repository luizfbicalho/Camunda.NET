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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.builder.AbstractBaseElementBuilder.SPACE;

	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using SubProcess = org.camunda.bpm.model.bpmn.instance.SubProcess;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class EmbeddedSubProcessBuilder : AbstractEmbeddedSubProcessBuilder<EmbeddedSubProcessBuilder, AbstractSubProcessBuilder<JavaToDotNetGenericWildcard>>
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected EmbeddedSubProcessBuilder(AbstractSubProcessBuilder subProcessBuilder)
	  protected internal EmbeddedSubProcessBuilder(AbstractSubProcessBuilder subProcessBuilder) : base(subProcessBuilder, typeof(EmbeddedSubProcessBuilder))
	  {
	  }

	  public virtual StartEventBuilder startEvent()
	  {
		return startEvent(null);
	  }

	  public virtual StartEventBuilder startEvent(string id)
	  {
		StartEvent start = subProcessBuilder.createChild(typeof(StartEvent), id);

		BpmnShape startShape = subProcessBuilder.createBpmnShape(start);
		BpmnShape subProcessShape = subProcessBuilder.findBpmnShape(subProcessBuilder.Element);

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


	  public virtual EventSubProcessBuilder eventSubProcess()
	  {
		return eventSubProcess(null);
	  }

	  public virtual EventSubProcessBuilder eventSubProcess(string id)
	  {
		// Create a subprocess, triggered by an event, and add it to modelInstance
		SubProcess subProcess = subProcessBuilder.createChild(typeof(SubProcess), id);
		subProcess.TriggeredByEvent = true;

		// Create Bpmn shape so subprocess will be drawn
		BpmnShape targetBpmnShape = subProcessBuilder.createBpmnShape(subProcess);
		//find the lowest shape in the process
		// place event sub process underneath
		Coordinates = targetBpmnShape;

		subProcessBuilder.resizeSubProcess(targetBpmnShape);

		// Return the eventSubProcessBuilder
		EventSubProcessBuilder eventSubProcessBuilder = new EventSubProcessBuilder(subProcessBuilder.modelInstance, subProcess);
		return eventSubProcessBuilder;

	  }

	  protected internal virtual BpmnShape Coordinates
	  {
		  set
		  {
    
			SubProcess eventSubProcess = (SubProcess) value.BpmnElement;
			SubProcess parentSubProcess = (SubProcess) eventSubProcess.ParentElement;
			BpmnShape parentBpmnShape = subProcessBuilder.findBpmnShape(parentSubProcess);
    
    
			Bounds targetBounds = value.Bounds;
			Bounds parentBounds = parentBpmnShape.Bounds;
    
			// these should just be offsets maybe
			double? ycoord = parentBounds.getHeight() + parentBounds.getY();
    
    
			double? xcoord = (parentBounds.getWidth() / 2) - (targetBounds.getWidth() / 2) + parentBounds.getX();
			if (xcoord - parentBounds.getX() < 50.0)
			{
			  xcoord = 50.0 + parentBounds.getX();
			}
    
			// move target
			targetBounds.setY(ycoord.Value);
			targetBounds.setX(xcoord.Value);
    
			// parent expands automatically
    
			// nodes surrounding the parent subprocess will not be moved
			// they may end up inside the subprocess (but only graphically)
		  }
	  }
	}

}