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
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN conversationNode element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class ConversationNodeImpl : BaseElementImpl, ConversationNode
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReferenceCollection<Participant, ParticipantRef> participantRefCollection;
	  protected internal static ElementReferenceCollection<MessageFlow, MessageFlowRef> messageFlowRefCollection;
	  protected internal static ChildElementCollection<CorrelationKey> correlationKeyCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ConversationNode), BPMN_ELEMENT_CONVERSATION_NODE).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).abstractType();

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		participantRefCollection = sequenceBuilder.elementCollection(typeof(ParticipantRef)).qNameElementReferenceCollection(typeof(Participant)).build();

		messageFlowRefCollection = sequenceBuilder.elementCollection(typeof(MessageFlowRef)).qNameElementReferenceCollection(typeof(MessageFlow)).build();

		correlationKeyCollection = sequenceBuilder.elementCollection(typeof(CorrelationKey)).build();

		typeBuilder.build();
	  }

	  public ConversationNodeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual ICollection<Participant> Participants
	  {
		  get
		  {
			return participantRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<MessageFlow> MessageFlows
	  {
		  get
		  {
			return messageFlowRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<CorrelationKey> CorrelationKeys
	  {
		  get
		  {
			return correlationKeyCollection.get(this);
		  }
	  }
	}

}