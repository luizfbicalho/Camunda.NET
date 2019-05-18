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
	using ConversationAssociation = org.camunda.bpm.model.bpmn.instance.ConversationAssociation;
	using ConversationNode = org.camunda.bpm.model.bpmn.instance.ConversationNode;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN conversationAssociation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ConversationAssociationImpl : BaseElementImpl, ConversationAssociation
	{

	  protected internal static AttributeReference<ConversationNode> innerConversationNodeRefAttribute;
	  protected internal static AttributeReference<ConversationNode> outerConversationNodeRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ConversationAssociation), BPMN_ELEMENT_CONVERSATION_ASSOCIATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		innerConversationNodeRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_INNER_CONVERSATION_NODE_REF).required().qNameAttributeReference(typeof(ConversationNode)).build();

		outerConversationNodeRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OUTER_CONVERSATION_NODE_REF).required().qNameAttributeReference(typeof(ConversationNode)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ConversationAssociation>
	  {
		  public ConversationAssociation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ConversationAssociationImpl(instanceContext);
		  }
	  }

	  public ConversationAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ConversationNode InnerConversationNode
	  {
		  get
		  {
			return innerConversationNodeRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			innerConversationNodeRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ConversationNode OuterConversationNode
	  {
		  get
		  {
			return outerConversationNodeRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			outerConversationNodeRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }

	}

}