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
	using org.camunda.bpm.model.bpmn.instance;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN collaboration element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CollaborationImpl : RootElementImpl, Collaboration
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<bool> isClosedAttribute;
	  protected internal static ChildElementCollection<Participant> participantCollection;
	  protected internal static ChildElementCollection<MessageFlow> messageFlowCollection;
	  protected internal static ChildElementCollection<Artifact> artifactCollection;
	  protected internal static ChildElementCollection<ConversationNode> conversationNodeCollection;
	  protected internal static ChildElementCollection<ConversationAssociation> conversationAssociationCollection;
	  protected internal static ChildElementCollection<ParticipantAssociation> participantAssociationCollection;
	  protected internal static ChildElementCollection<MessageFlowAssociation> messageFlowAssociationCollection;
	  protected internal static ChildElementCollection<CorrelationKey> correlationKeyCollection;
	  /// <summary>
	  /// TODO: choreographyRef </summary>
	  protected internal static ChildElementCollection<ConversationLink> conversationLinkCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Collaboration), BPMN_ELEMENT_COLLABORATION).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		isClosedAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_CLOSED).defaultValue(false).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		participantCollection = sequenceBuilder.elementCollection(typeof(Participant)).build();

		messageFlowCollection = sequenceBuilder.elementCollection(typeof(MessageFlow)).build();

		artifactCollection = sequenceBuilder.elementCollection(typeof(Artifact)).build();

		conversationNodeCollection = sequenceBuilder.elementCollection(typeof(ConversationNode)).build();

		conversationAssociationCollection = sequenceBuilder.elementCollection(typeof(ConversationAssociation)).build();

		participantAssociationCollection = sequenceBuilder.elementCollection(typeof(ParticipantAssociation)).build();

		messageFlowAssociationCollection = sequenceBuilder.elementCollection(typeof(MessageFlowAssociation)).build();

		correlationKeyCollection = sequenceBuilder.elementCollection(typeof(CorrelationKey)).build();

		conversationLinkCollection = sequenceBuilder.elementCollection(typeof(ConversationLink)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Collaboration>
	  {
		  public Collaboration newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CollaborationImpl(instanceContext);
		  }
	  }

	  public CollaborationImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual bool Closed
	  {
		  get
		  {
			return isClosedAttribute.getValue(this);
		  }
		  set
		  {
			isClosedAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Participant> Participants
	  {
		  get
		  {
			return participantCollection.get(this);
		  }
	  }

	  public virtual ICollection<MessageFlow> MessageFlows
	  {
		  get
		  {
			return messageFlowCollection.get(this);
		  }
	  }

	  public virtual ICollection<Artifact> Artifacts
	  {
		  get
		  {
			return artifactCollection.get(this);
		  }
	  }

	  public virtual ICollection<ConversationNode> ConversationNodes
	  {
		  get
		  {
			return conversationNodeCollection.get(this);
		  }
	  }

	  public virtual ICollection<ConversationAssociation> ConversationAssociations
	  {
		  get
		  {
			return conversationAssociationCollection.get(this);
		  }
	  }

	  public virtual ICollection<ParticipantAssociation> ParticipantAssociations
	  {
		  get
		  {
			return participantAssociationCollection.get(this);
		  }
	  }

	  public virtual ICollection<MessageFlowAssociation> MessageFlowAssociations
	  {
		  get
		  {
			return messageFlowAssociationCollection.get(this);
		  }
	  }

	  public virtual ICollection<CorrelationKey> CorrelationKeys
	  {
		  get
		  {
			return correlationKeyCollection.get(this);
		  }
	  }

	  public virtual ICollection<ConversationLink> ConversationLinks
	  {
		  get
		  {
			return conversationLinkCollection.get(this);
		  }
	  }
	}

}