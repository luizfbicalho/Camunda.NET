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
	using MessageFlow = org.camunda.bpm.model.bpmn.instance.MessageFlow;
	using MessageFlowAssociation = org.camunda.bpm.model.bpmn.instance.MessageFlowAssociation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN messageFlowAssociation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class MessageFlowAssociationImpl : BaseElementImpl, MessageFlowAssociation
	{

	  protected internal static AttributeReference<MessageFlow> innerMessageFlowRefAttribute;
	  protected internal static AttributeReference<MessageFlow> outerMessageFlowRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(MessageFlowAssociation), BPMN_ELEMENT_MESSAGE_FLOW_ASSOCIATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		innerMessageFlowRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_INNER_MESSAGE_FLOW_REF).required().qNameAttributeReference(typeof(MessageFlow)).build();

		outerMessageFlowRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OUTER_MESSAGE_FLOW_REF).required().qNameAttributeReference(typeof(MessageFlow)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<MessageFlowAssociation>
	  {
		  public MessageFlowAssociation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new MessageFlowAssociationImpl(instanceContext);
		  }
	  }

	  public MessageFlowAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual MessageFlow InnerMessageFlow
	  {
		  get
		  {
			return innerMessageFlowRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			innerMessageFlowRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual MessageFlow OuterMessageFlow
	  {
		  get
		  {
			return outerMessageFlowRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			outerMessageFlowRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }

	}

}