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
	using Participant = org.camunda.bpm.model.bpmn.instance.Participant;
	using ParticipantAssociation = org.camunda.bpm.model.bpmn.instance.ParticipantAssociation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_PARTICIPANT_ASSOCIATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN participantAssociation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ParticipantAssociationImpl : BaseElementImpl, ParticipantAssociation
	{

	  protected internal static ElementReference<Participant, InnerParticipantRef> innerParticipantRefChild;
	  protected internal static ElementReference<Participant, OuterParticipantRef> outerParticipantRefChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ParticipantAssociation), BPMN_ELEMENT_PARTICIPANT_ASSOCIATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		innerParticipantRefChild = sequenceBuilder.element(typeof(InnerParticipantRef)).required().qNameElementReference(typeof(Participant)).build();

		outerParticipantRefChild = sequenceBuilder.element(typeof(OuterParticipantRef)).required().qNameElementReference(typeof(Participant)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ParticipantAssociation>
	  {
		  public ParticipantAssociation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ParticipantAssociationImpl(instanceContext);
		  }
	  }

	  public ParticipantAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual Participant InnerParticipant
	  {
		  get
		  {
			return innerParticipantRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
		   innerParticipantRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual Participant OuterParticipant
	  {
		  get
		  {
			return outerParticipantRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			 outerParticipantRefChild.setReferenceTargetElement(this, value);
		  }
	  }

	}

}