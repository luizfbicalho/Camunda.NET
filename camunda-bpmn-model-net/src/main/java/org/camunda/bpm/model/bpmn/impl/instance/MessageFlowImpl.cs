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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using InteractionNode = org.camunda.bpm.model.bpmn.instance.InteractionNode;
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using MessageFlow = org.camunda.bpm.model.bpmn.instance.MessageFlow;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN messageFlow element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class MessageFlowImpl : BaseElementImpl, MessageFlow
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static AttributeReference<InteractionNode> sourceRefAttribute;
	  protected internal static AttributeReference<InteractionNode> targetRefAttribute;
	  protected internal static AttributeReference<Message> messageRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(MessageFlow), BPMN_ELEMENT_MESSAGE_FLOW).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		sourceRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_SOURCE_REF).required().qNameAttributeReference(typeof(InteractionNode)).build();

		targetRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TARGET_REF).required().qNameAttributeReference(typeof(InteractionNode)).build();

		messageRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_MESSAGE_REF).qNameAttributeReference(typeof(Message)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<MessageFlow>
	  {
		  public MessageFlow newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new MessageFlowImpl(instanceContext);
		  }
	  }

	  public MessageFlowImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return nameAttribute.getValue(this);
		  }
		  set
		  {
			nameAttribute.setValue(this, value);
		  }
	  }


	  public virtual InteractionNode Source
	  {
		  get
		  {
			return sourceRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			sourceRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual InteractionNode Target
	  {
		  get
		  {
			return targetRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual Message Message
	  {
		  get
		  {
			return messageRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			messageRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public override BpmnEdge DiagramElement
	  {
		  get
		  {
			return (BpmnEdge) base.DiagramElement;
		  }
	  }
	}

}