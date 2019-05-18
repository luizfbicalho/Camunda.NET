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
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN participant element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ParticipantImpl : BaseElementImpl, Participant
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static AttributeReference<Process> processRefAttribute;
	  protected internal static ElementReferenceCollection<Interface, InterfaceRef> interfaceRefCollection;
	  protected internal static ElementReferenceCollection<EndPoint, EndPointRef> endPointRefCollection;
	  protected internal static ChildElement<ParticipantMultiplicity> participantMultiplicityChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Participant), BPMN_ELEMENT_PARTICIPANT).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		processRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_PROCESS_REF).qNameAttributeReference(typeof(Process)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		interfaceRefCollection = sequenceBuilder.elementCollection(typeof(InterfaceRef)).qNameElementReferenceCollection(typeof(Interface)).build();

		endPointRefCollection = sequenceBuilder.elementCollection(typeof(EndPointRef)).qNameElementReferenceCollection(typeof(EndPoint)).build();

		participantMultiplicityChild = sequenceBuilder.element(typeof(ParticipantMultiplicity)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Participant>
	  {
		  public Participant newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ParticipantImpl(instanceContext);
		  }
	  }

	  public ParticipantImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual Process Process
	  {
		  get
		  {
			return processRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			processRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<Interface> Interfaces
	  {
		  get
		  {
			return interfaceRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ICollection<EndPoint> EndPoints
	  {
		  get
		  {
			return endPointRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ParticipantMultiplicity ParticipantMultiplicity
	  {
		  get
		  {
			return participantMultiplicityChild.getChild(this);
		  }
		  set
		  {
			participantMultiplicityChild.setChild(this, value);
		  }
	  }

	}

}