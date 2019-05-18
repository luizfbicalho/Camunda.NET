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

	using org.camunda.bpm.model.bpmn.instance;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using BpmnPlane = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnPlane;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Bounds = org.camunda.bpm.model.bpmn.instance.dc.Bounds;
	using Waypoint = org.camunda.bpm.model.bpmn.instance.di.Waypoint;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractBaseElementBuilder<B, E> : AbstractBpmnModelElementBuilder<B, E> where B : AbstractBaseElementBuilder<B, E> where E : BaseElement
	{

	  public const double SPACE = 50;

	  protected internal AbstractBaseElementBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  protected internal virtual T createInstance<T>(Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		return modelInstance.newInstance(typeClass);
	  }

	  protected internal virtual T createInstance<T>(Type<T> typeClass, string identifier) where T : BaseElement
	  {
		T instance = createInstance(typeClass);
		if (!string.ReferenceEquals(identifier, null))
		{
		  instance.Id = identifier;
		  if (instance is FlowElement)
		  {
			((FlowElement) instance).Name = identifier;
		  }
		}
		return instance;
	  }

	  protected internal virtual T createChild<T>(Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		return createChild(element, typeClass);
	  }

	  protected internal virtual T createChild<T>(Type<T> typeClass, string identifier) where T : BaseElement
	  {
		return createChild(element, typeClass, identifier);
	  }

	  protected internal virtual T createChild<T>(BpmnModelElementInstance parent, Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		T instance = createInstance(typeClass);
		parent.addChildElement(instance);
		return instance;
	  }

	  protected internal virtual T createChild<T>(BpmnModelElementInstance parent, Type<T> typeClass, string identifier) where T : BaseElement
	  {
		T instance = createInstance(typeClass, identifier);
		parent.addChildElement(instance);
		return instance;
	  }

	  protected internal virtual T createSibling<T>(Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		T instance = createInstance(typeClass);
		element.ParentElement.addChildElement(instance);
		return instance;
	  }

	  protected internal virtual T createSibling<T>(Type<T> typeClass, string identifier) where T : BaseElement
	  {
		T instance = createInstance(typeClass, identifier);
		element.ParentElement.addChildElement(instance);
		return instance;
	  }

	  protected internal virtual T getCreateSingleChild<T>(Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		return getCreateSingleChild(element, typeClass);
	  }

	  protected internal virtual T getCreateSingleChild<T>(BpmnModelElementInstance parent, Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		ICollection<T> childrenOfType = parent.getChildElementsByType(typeClass);
		if (childrenOfType.Count == 0)
		{
		  return createChild(parent, typeClass);
		}
		else
		{
		  if (childrenOfType.Count > 1)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new BpmnModelException("Element " + parent + " of type " + parent.ElementType.TypeName + " has more than one child element of type " + typeClass.FullName);
		  }
		  else
		  {
			return childrenOfType.GetEnumerator().next();
		  }
		}
	  }

	  protected internal virtual T getCreateSingleExtensionElement<T>(Type<T> typeClass) where T : BpmnModelElementInstance
	  {
		ExtensionElements extensionElements = getCreateSingleChild(typeof(ExtensionElements));
		return getCreateSingleChild(extensionElements, typeClass);
	  }

	  protected internal virtual Message findMessageForName(string messageName)
	  {
		ICollection<Message> messages = modelInstance.getModelElementsByType(typeof(Message));
		foreach (Message message in messages)
		{
		  if (messageName.Equals(message.Name))
		  {
			// return already existing message for message name
			return message;
		  }
		}

		// create new message for non existing message name
		Definitions definitions = modelInstance.Definitions;
		Message message = createChild(definitions, typeof(Message));
		message.Name = messageName;

		return message;
	  }

	  protected internal virtual MessageEventDefinition createMessageEventDefinition(string messageName)
	  {
		Message message = findMessageForName(messageName);
		MessageEventDefinition messageEventDefinition = createInstance(typeof(MessageEventDefinition));
		messageEventDefinition.Message = message;
		return messageEventDefinition;
	  }

	  protected internal virtual MessageEventDefinition createEmptyMessageEventDefinition()
	  {
		return createInstance(typeof(MessageEventDefinition));
	  }

	  protected internal virtual Signal findSignalForName(string signalName)
	  {
		ICollection<Signal> signals = modelInstance.getModelElementsByType(typeof(Signal));
		foreach (Signal signal in signals)
		{
		  if (signalName.Equals(signal.Name))
		  {
			// return already existing signal for signal name
			return signal;
		  }
		}

		// create new signal for non existing signal name
		Definitions definitions = modelInstance.Definitions;
		Signal signal = createChild(definitions, typeof(Signal));
		signal.Name = signalName;

		return signal;
	  }

	  protected internal virtual SignalEventDefinition createSignalEventDefinition(string signalName)
	  {
		Signal signal = findSignalForName(signalName);
		SignalEventDefinition signalEventDefinition = createInstance(typeof(SignalEventDefinition));
		signalEventDefinition.Signal = signal;
		return signalEventDefinition;
	  }

	  protected internal virtual ErrorEventDefinition findErrorDefinitionForCode(string errorCode)
	  {
		ICollection<ErrorEventDefinition> definitions = modelInstance.getModelElementsByType(typeof(ErrorEventDefinition));
		foreach (ErrorEventDefinition definition in definitions)
		{
		  Error error = definition.Error;
		  if (error != null && error.ErrorCode.Equals(errorCode))
		  {
			  return definition;
		  }
		}
		return null;
	  }

	  protected internal virtual Error findErrorForNameAndCode(string errorCode)
	  {
		ICollection<Error> errors = modelInstance.getModelElementsByType(typeof(Error));
		foreach (Error error in errors)
		{
		  if (errorCode.Equals(error.ErrorCode))
		  {
			// return already existing error
			return error;
		  }
		}

		// create new error
		Definitions definitions = modelInstance.Definitions;
		Error error = createChild(definitions, typeof(Error));
		error.ErrorCode = errorCode;

		return error;
	  }

	  protected internal virtual ErrorEventDefinition createEmptyErrorEventDefinition()
	  {
		ErrorEventDefinition errorEventDefinition = createInstance(typeof(ErrorEventDefinition));
		return errorEventDefinition;
	  }

	  protected internal virtual ErrorEventDefinition createErrorEventDefinition(string errorCode)
	  {
		Error error = findErrorForNameAndCode(errorCode);
		ErrorEventDefinition errorEventDefinition = createInstance(typeof(ErrorEventDefinition));
		errorEventDefinition.Error = error;
		return errorEventDefinition;
	  }

	  protected internal virtual Escalation findEscalationForCode(string escalationCode)
	  {
		ICollection<Escalation> escalations = modelInstance.getModelElementsByType(typeof(Escalation));
		foreach (Escalation escalation in escalations)
		{
		  if (escalationCode.Equals(escalation.EscalationCode))
		  {
			  // return already existing escalation
			  return escalation;
		  }
		}

		Definitions definitions = modelInstance.Definitions;
		Escalation escalation = createChild(definitions, typeof(Escalation));
		escalation.EscalationCode = escalationCode;
		return escalation;
	  }

	  protected internal virtual EscalationEventDefinition createEscalationEventDefinition(string escalationCode)
	  {
		Escalation escalation = findEscalationForCode(escalationCode);
		EscalationEventDefinition escalationEventDefinition = createInstance(typeof(EscalationEventDefinition));
		escalationEventDefinition.Escalation = escalation;
		return escalationEventDefinition;
	  }

	  protected internal virtual CompensateEventDefinition createCompensateEventDefinition()
	  {
		CompensateEventDefinition compensateEventDefinition = createInstance(typeof(CompensateEventDefinition));
		return compensateEventDefinition;
	  }


	  /// <summary>
	  /// Sets the identifier of the element.
	  /// </summary>
	  /// <param name="identifier">  the identifier to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B id(string identifier)
	  {
		element.Id = identifier;
		return myself;
	  }

	  /// <summary>
	  /// Add an extension element to the element.
	  /// </summary>
	  /// <param name="extensionElement">  the extension element to add </param>
	  /// <returns> the builder object </returns>
	  public virtual B addExtensionElement(BpmnModelElementInstance extensionElement)
	  {
		ExtensionElements extensionElements = getCreateSingleChild(typeof(ExtensionElements));
		extensionElements.addChildElement(extensionElement);
		return myself;
	  }

	  public virtual BpmnShape createBpmnShape(FlowNode node)
	  {
		BpmnPlane bpmnPlane = findBpmnPlane();
		if (bpmnPlane != null)
		{
		  BpmnShape bpmnShape = createInstance(typeof(BpmnShape));
		  bpmnShape.BpmnElement = node;
		  Bounds nodeBounds = createInstance(typeof(Bounds));

		  if (node is SubProcess)
		  {
			bpmnShape.Expanded = true;
			nodeBounds.setWidth(350);
			nodeBounds.setHeight(200);
		  }
		  else if (node is Activity)
		  {
			nodeBounds.setWidth(100);
			nodeBounds.setHeight(80);
		  }
		  else if (node is Event)
		  {
			nodeBounds.setWidth(36);
			nodeBounds.setHeight(36);
		  }
		  else if (node is Gateway)
		  {
			nodeBounds.setWidth(50);
			nodeBounds.setHeight(50);
			if (node is ExclusiveGateway)
			{
			  bpmnShape.MarkerVisible = true;
			}
		  }

		  nodeBounds.setX(0);
		  nodeBounds.setY(0);

		  bpmnShape.addChildElement(nodeBounds);
		  bpmnPlane.addChildElement(bpmnShape);

		  return bpmnShape;
		}
		return null;
	  }

	  protected internal virtual BpmnShape Coordinates
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
			  x = sourceX + sourceWidth + SPACE;
    
			  if (element is FlowNode)
			  {
    
				FlowNode flowNode = (FlowNode) element;
				ICollection<SequenceFlow> outgoing = flowNode.Outgoing;
    
				if (outgoing.Count == 0)
				{
				  double sourceY = sourceBounds.getY().Value;
				  double sourceHeight = sourceBounds.getHeight().Value;
				  double targetHeight = shapeBounds.getHeight().Value;
				  y = sourceY + sourceHeight / 2 - targetHeight / 2;
				}
				else
				{
				  SequenceFlow[] sequenceFlows = outgoing.toArray(new SequenceFlow[outgoing.Count]);
				  SequenceFlow last = sequenceFlows[outgoing.Count - 1];
    
				  BpmnShape targetShape = findBpmnShape(last.Target);
				  if (targetShape != null)
				  {
					Bounds targetBounds = targetShape.Bounds;
					double lastY = targetBounds.getY().Value;
					double lastHeight = targetBounds.getHeight().Value;
					y = lastY + lastHeight + SPACE;
				  }
    
				}
			  }
			}
    
			shapeBounds.setX(x);
			shapeBounds.setY(y);
		  }
	  }

	  /// @deprecated use <seealso cref="#createEdge(BaseElement)"/> instead 
	  [Obsolete("use <seealso cref="#createEdge(BaseElement)"/> instead")]
	  public virtual BpmnEdge createBpmnEdge(SequenceFlow sequenceFlow)
	  {
		return createEdge(sequenceFlow);
	  }

	  public virtual BpmnEdge createEdge(BaseElement baseElement)
	  {
		BpmnPlane bpmnPlane = findBpmnPlane();
		if (bpmnPlane != null)
		{


		   BpmnEdge edge = createInstance(typeof(BpmnEdge));
		   edge.BpmnElement = baseElement;
		   Waypoints = edge;

		   bpmnPlane.addChildElement(edge);
		   return edge;
		}
		return null;

	  }

	  protected internal virtual BpmnEdge Waypoints
	  {
		  set
		  {
			BaseElement bpmnElement = value.BpmnElement;
    
			FlowNode edgeSource;
			FlowNode edgeTarget;
			if (bpmnElement is SequenceFlow)
			{
    
			  SequenceFlow sequenceFlow = (SequenceFlow) bpmnElement;
    
			  edgeSource = sequenceFlow.Source;
			  edgeTarget = sequenceFlow.Target;
    
			}
			else if (bpmnElement is Association)
			{
			  Association association = (Association) bpmnElement;
    
			  edgeSource = (FlowNode) association.Source;
			  edgeTarget = (FlowNode) association.Target;
			}
			else
			{
			  throw new Exception("Bpmn element type not supported");
			}
    
			setWaypointsWithSourceAndTarget(value, edgeSource, edgeTarget);
		  }
	  }

	  protected internal virtual void setWaypointsWithSourceAndTarget(BpmnEdge edge, FlowNode edgeSource, FlowNode edgeTarget)
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

		  if (edgeSource.Outgoing.Count == 1)
		  {
			w1.X = sourceX + sourceWidth;
			w1.Y = sourceY + sourceHeight / 2;

			edge.addChildElement(w1);
		  }
		  else
		  {
			w1.X = sourceX + sourceWidth / 2;
			w1.Y = sourceY + sourceHeight;

			edge.addChildElement(w1);

			Waypoint w2 = createInstance(typeof(Waypoint));
			w2.X = sourceX + sourceWidth / 2;
			w2.Y = targetY + targetHeight / 2;

			edge.addChildElement(w2);
		  }

		  Waypoint w3 = createInstance(typeof(Waypoint));
		  w3.X = targetX;
		  w3.Y = targetY + targetHeight / 2;

		  edge.addChildElement(w3);
		}
	  }

	  protected internal virtual BpmnPlane findBpmnPlane()
	  {
		ICollection<BpmnPlane> planes = modelInstance.getModelElementsByType(typeof(BpmnPlane));
		return planes.GetEnumerator().next();
	  }

	  protected internal virtual BpmnShape findBpmnShape(BaseElement node)
	  {
		ICollection<BpmnShape> allShapes = modelInstance.getModelElementsByType(typeof(BpmnShape));

		IEnumerator<BpmnShape> iterator = allShapes.GetEnumerator();
		while (iterator.MoveNext())
		{
		  BpmnShape shape = iterator.Current;
		  if (shape.BpmnElement.Equals(node))
		  {
			return shape;
		  }
		}
		return null;
	  }

	  protected internal virtual BpmnEdge findBpmnEdge(BaseElement sequenceFlow)
	  {
		ICollection<BpmnEdge> allEdges = modelInstance.getModelElementsByType(typeof(BpmnEdge));
		IEnumerator<BpmnEdge> iterator = allEdges.GetEnumerator();

		while (iterator.MoveNext())
		{
		  BpmnEdge edge = iterator.Current;
		  if (edge.BpmnElement.Equals(sequenceFlow))
		  {
			return edge;
		  }
		}
		return null;
	  }

	  protected internal virtual void resizeSubProcess(BpmnShape innerShape)
	  {

		BaseElement innerElement = innerShape.BpmnElement;
		Bounds innerShapeBounds = innerShape.Bounds;

		ModelElementInstance parent = innerElement.ParentElement;

		while (parent is SubProcess)
		{

		  BpmnShape subProcessShape = findBpmnShape((SubProcess) parent);

		  if (subProcessShape != null)
		  {

			Bounds subProcessBounds = subProcessShape.Bounds;
			double innerX = innerShapeBounds.getX().Value;
			double innerWidth = innerShapeBounds.getWidth().Value;
			double innerY = innerShapeBounds.getY().Value;
			double innerHeight = innerShapeBounds.getHeight().Value;

			double subProcessY = subProcessBounds.getY().Value;
			double subProcessHeight = subProcessBounds.getHeight().Value;
			double subProcessX = subProcessBounds.getX().Value;
			double subProcessWidth = subProcessBounds.getWidth().Value;

			double tmpWidth = innerX + innerWidth + SPACE;
			double tmpHeight = innerY + innerHeight + SPACE;

			if (innerY == subProcessY)
			{
			  subProcessBounds.setY(subProcessY - SPACE);
			  subProcessBounds.setHeight(subProcessHeight + SPACE);
			}

			if (tmpWidth >= subProcessX + subProcessWidth)
			{
			  double newWidth = tmpWidth - subProcessX;
			  subProcessBounds.setWidth(newWidth);
			}

			if (tmpHeight >= subProcessY + subProcessHeight)
			{
			  double newHeight = tmpHeight - subProcessY;
			  subProcessBounds.setHeight(newHeight);
			}

			innerElement = (SubProcess) parent;
			innerShapeBounds = subProcessBounds;
			parent = innerElement.ParentElement;
		  }
		  else
		  {
			break;
		  }
		}
	  }
	}

}