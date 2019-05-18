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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using CallConversation = org.camunda.bpm.model.bpmn.instance.CallConversation;
	using ConversationNode = org.camunda.bpm.model.bpmn.instance.ConversationNode;
	using GlobalConversation = org.camunda.bpm.model.bpmn.instance.GlobalConversation;
	using ParticipantAssociation = org.camunda.bpm.model.bpmn.instance.ParticipantAssociation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN callConversation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CallConversationImpl : ConversationNodeImpl, CallConversation
	{

	  protected internal static AttributeReference<GlobalConversation> calledCollaborationRefAttribute;
	  protected internal static ChildElementCollection<ParticipantAssociation> participantAssociationCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CallConversation), BPMN_ELEMENT_CALL_CONVERSATION).namespaceUri(BPMN20_NS).extendsType(typeof(ConversationNode)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		calledCollaborationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_CALLED_COLLABORATION_REF).qNameAttributeReference(typeof(GlobalConversation)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		participantAssociationCollection = sequenceBuilder.elementCollection(typeof(ParticipantAssociation)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CallConversation>
	  {
		  public CallConversation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CallConversationImpl(instanceContext);
		  }
	  }

	  public CallConversationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual GlobalConversation CalledCollaboration
	  {
		  get
		  {
			return calledCollaborationRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			calledCollaborationRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<ParticipantAssociation> ParticipantAssociations
	  {
		  get
		  {
			return participantAssociationCollection.get(this);
		  }
	  }
	}

}