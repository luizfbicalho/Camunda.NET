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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
	using DiagramElement = org.camunda.bpm.engine.repository.DiagramElement;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using org.camunda.bpm.model.bpmn.builder;
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelInstance = org.camunda.bpm.model.xml.ModelInstance;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;

	public class ModifiableBpmnModelInstance : BpmnModelInstance
	{

	  protected internal BpmnModelInstance modelInstance;

	  public ModifiableBpmnModelInstance(BpmnModelInstance modelInstance)
	  {
		this.modelInstance = modelInstance;
	  }

	  /// <summary>
	  /// Copies the argument; following modifications are not applied to the original model instance
	  /// </summary>
	  public static ModifiableBpmnModelInstance modify(BpmnModelInstance modelInstance)
	  {
		return new ModifiableBpmnModelInstance(modelInstance.clone());
	  }

	  /// <summary>
	  /// wraps the argument; following modifications are applied to the original model instance
	  /// </summary>
	  public static ModifiableBpmnModelInstance wrap(BpmnModelInstance modelInstance)
	  {
		return new ModifiableBpmnModelInstance(modelInstance);
	  }

	  public virtual Definitions Definitions
	  {
		  get
		  {
			return modelInstance.Definitions;
		  }
		  set
		  {
			modelInstance.Definitions = value;
		  }
	  }


	  public override BpmnModelInstance clone()
	  {
		return modelInstance.clone();
	  }

	  public virtual DomDocument Document
	  {
		  get
		  {
			return modelInstance.Document;
		  }
	  }

	  public virtual ModelElementInstance DocumentElement
	  {
		  get
		  {
			return modelInstance.DocumentElement;
		  }
		  set
		  {
			modelInstance.DocumentElement = value;
		  }
	  }


	  public virtual T newInstance<T>(Type type) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  type = typeof(T);
		return modelInstance.newInstance(type);
	  }

	  public override T newInstance<T>(Type aClass, string s) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  aClass = typeof(T);
		return modelInstance.newInstance(aClass, s);
	  }

	  public virtual T newInstance<T>(ModelElementType type) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return modelInstance.newInstance(type);
	  }

	  public override T newInstance<T>(ModelElementType modelElementType, string s) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return modelInstance.newInstance(modelElementType, s);
	  }

	  public virtual Model Model
	  {
		  get
		  {
			return modelInstance.Model;
		  }
	  }

	  public virtual T getModelElementById<T>(string id) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return modelInstance.getModelElementById(id);
	  }

	  public virtual ICollection<ModelElementInstance> getModelElementsByType(ModelElementType referencingType)
	  {
		return modelInstance.getModelElementsByType(referencingType);
	  }

	  public virtual ICollection<T> getModelElementsByType<T>(Type referencingClass) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
			  referencingClass = typeof(T);
		return modelInstance.getModelElementsByType(referencingClass);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends AbstractBaseElementBuilder> T getBuilderForElementById(String id, Class<T> builderClass)
	  public virtual T getBuilderForElementById<T>(string id, Type builderClass) where T : AbstractBaseElementBuilder
	  {
			  builderClass = typeof(T);
		BaseElement modelElementById = modelInstance.getModelElementById(id);
		return (T) modelElementById.builder();
	  }

	  public virtual AbstractActivityBuilder activityBuilder(string activityId)
	  {
		return getBuilderForElementById(activityId, typeof(AbstractActivityBuilder));
	  }

	  public virtual AbstractFlowNodeBuilder flowNodeBuilder(string flowNodeId)
	  {
		return getBuilderForElementById(flowNodeId, typeof(AbstractFlowNodeBuilder));
	  }

	  public virtual UserTaskBuilder userTaskBuilder(string userTaskId)
	  {
		return getBuilderForElementById(userTaskId, typeof(UserTaskBuilder));
	  }

	  public virtual ServiceTaskBuilder serviceTaskBuilder(string serviceTaskId)
	  {
		return getBuilderForElementById(serviceTaskId, typeof(ServiceTaskBuilder));
	  }

	  public virtual CallActivityBuilder callActivityBuilder(string callActivityId)
	  {
		return getBuilderForElementById(callActivityId, typeof(CallActivityBuilder));
	  }

	  public virtual IntermediateCatchEventBuilder intermediateCatchEventBuilder(string eventId)
	  {
		return getBuilderForElementById(eventId, typeof(IntermediateCatchEventBuilder));
	  }

	  public virtual StartEventBuilder startEventBuilder(string eventId)
	  {
		return getBuilderForElementById(eventId, typeof(StartEventBuilder));
	  }

	  public virtual EndEventBuilder endEventBuilder(string eventId)
	  {
		return getBuilderForElementById(eventId, typeof(EndEventBuilder));
	  }

	  public virtual ModifiableBpmnModelInstance changeElementId(string oldId, string newId)
	  {
		BaseElement element = getModelElementById(oldId);
		element.Id = newId;
		return this;
	  }

	  public virtual ModifiableBpmnModelInstance changeElementName(string elementId, string newName)
	  {
		FlowElement flowElement = getModelElementById(elementId);
		flowElement.Name = newName;
		return this;
	  }

	  public virtual ModifiableBpmnModelInstance removeChildren(string elementId)
	  {
		BaseElement element = getModelElementById(elementId);

		ICollection<BaseElement> children = element.getChildElementsByType(typeof(BaseElement));
		foreach (BaseElement child in children)
		{
		  element.removeChildElement(child);
		}

		return this;
	  }

	  public virtual ModifiableBpmnModelInstance renameMessage(string oldMessageName, string newMessageName)
	  {
		ICollection<Message> messages = modelInstance.getModelElementsByType(typeof(Message));

		foreach (Message message in messages)
		{
		  if (message.Name.Equals(oldMessageName))
		  {
			message.Name = newMessageName;
		  }
		}

		return this;
	  }

	  public virtual ModifiableBpmnModelInstance addDocumentation(string content)
	  {
		ICollection<Process> processes = modelInstance.getModelElementsByType(typeof(Process));
		Documentation documentation = modelInstance.newInstance(typeof(Documentation));
		documentation.TextContent = content;
		foreach (Process process in processes)
		{
		  process.addChildElement(documentation);
		}
		return this;
	  }

	  public virtual ModifiableBpmnModelInstance renameSignal(string oldSignalName, string newSignalName)
	  {
		ICollection<Signal> signals = modelInstance.getModelElementsByType(typeof(Signal));

		foreach (Signal signal in signals)
		{
		  if (signal.Name.Equals(oldSignalName))
		  {
			signal.Name = newSignalName;
		  }
		}

		return this;
	  }

	  public virtual ModifiableBpmnModelInstance swapElementIds(string firstElementId, string secondElementId)
	  {
		BaseElement firstElement = getModelElementById(firstElementId);
		BaseElement secondElement = getModelElementById(secondElementId);

		secondElement.Id = "___TEMP___ID___";
		firstElement.Id = secondElementId;
		secondElement.Id = firstElementId;

		return this;
	  }

	  public virtual SubProcessBuilder addSubProcessTo(string parentId)
	  {
		SubProcess eventSubProcess = modelInstance.newInstance(typeof(SubProcess));

		BpmnModelElementInstance parent = getModelElementById(parentId);
		parent.addChildElement(eventSubProcess);

		return eventSubProcess.builder();
	  }

	  public virtual ModifiableBpmnModelInstance removeFlowNode(string flowNodeId)
	  {
		FlowNode flowNode = getModelElementById(flowNodeId);
		ModelElementInstance scope = flowNode.ParentElement;

		foreach (SequenceFlow outgoingFlow in flowNode.Outgoing)
		{
		  removeBpmnEdge(outgoingFlow);
		  scope.removeChildElement(outgoingFlow);
		}
		foreach (SequenceFlow incomingFlow in flowNode.Incoming)
		{
		  removeBpmnEdge(incomingFlow);
		  scope.removeChildElement(incomingFlow);
		}
		ICollection<Association> associations = scope.getChildElementsByType(typeof(Association));
		foreach (Association association in associations)
		{
		  if (flowNode.Equals(association.Source) || flowNode.Equals(association.Target))
		  {
			removeBpmnEdge(association);
			scope.removeChildElement(association);
		  }
		}

		removeBpmnShape(flowNode);
		scope.removeChildElement(flowNode);

		return this;
	  }

	  protected internal virtual void removeBpmnEdge(BaseElement element)
	  {
		ICollection<BpmnEdge> edges = modelInstance.getModelElementsByType(typeof(BpmnEdge));
		foreach (BpmnEdge edge in edges)
		{
		  if (edge.BpmnElement.Equals(element))
		  {
			ModelElementInstance bpmnPlane = edge.ParentElement;
			bpmnPlane.removeChildElement(edge);
			break;
		  }
		}
	  }

	  protected internal virtual void removeBpmnShape(FlowNode flowNode)
	  {
		ICollection<BpmnShape> bpmnShapes = modelInstance.getModelElementsByType(typeof(BpmnShape));
		foreach (BpmnShape shape in bpmnShapes)
		{
		  if (shape.BpmnElement.Equals(flowNode))
		  {
			ModelElementInstance bpmnPlane = shape.ParentElement;
			bpmnPlane.removeChildElement(shape);
			break;
		  }
		}
	  }

	  public virtual ModifiableBpmnModelInstance asyncBeforeInnerMiActivity(string activityId)
	  {
		Activity activity = modelInstance.getModelElementById(activityId);

		MultiInstanceLoopCharacteristics miCharacteristics = (MultiInstanceLoopCharacteristics) activity.getUniqueChildElementByType(typeof(MultiInstanceLoopCharacteristics));
		miCharacteristics.CamundaAsyncBefore = true;

		return this;
	  }

	  public virtual ModifiableBpmnModelInstance asyncAfterInnerMiActivity(string activityId)
	  {
		Activity activity = modelInstance.getModelElementById(activityId);

		MultiInstanceLoopCharacteristics miCharacteristics = (MultiInstanceLoopCharacteristics) activity.getUniqueChildElementByType(typeof(MultiInstanceLoopCharacteristics));
		miCharacteristics.CamundaAsyncAfter = true;

		return this;
	  }

	  public virtual ValidationResults validate<T1>(ICollection<T1> validators)
	  {
		return null;
	  }

	}

}