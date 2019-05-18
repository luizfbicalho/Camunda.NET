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
namespace org.camunda.bpm.model.bpmn.impl.instance.bpmndi
{
	using LabeledShapeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.LabeledShapeImpl;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BpmnLabel = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnLabel;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using ParticipantBandKind = org.camunda.bpm.model.bpmn.instance.bpmndi.ParticipantBandKind;
	using LabeledShape = org.camunda.bpm.model.bpmn.instance.di.LabeledShape;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMNDI BPMNShape element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnShapeImpl : LabeledShapeImpl, BpmnShape
	{

	  protected internal static AttributeReference<BaseElement> bpmnElementAttribute;
	  protected internal static Attribute<bool> isHorizontalAttribute;
	  protected internal static Attribute<bool> isExpandedAttribute;
	  protected internal static Attribute<bool> isMarkerVisibleAttribute;
	  protected internal static Attribute<bool> isMessageVisibleAttribute;
	  protected internal static Attribute<ParticipantBandKind> participantBandKindAttribute;
	  protected internal static AttributeReference<BpmnShape> choreographyActivityShapeAttribute;
	  protected internal static ChildElement<BpmnLabel> bpmnLabelChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BpmnShape), BPMNDI_ELEMENT_BPMN_SHAPE).namespaceUri(BPMNDI_NS).extendsType(typeof(LabeledShape)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		bpmnElementAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_BPMN_ELEMENT).qNameAttributeReference(typeof(BaseElement)).build();

		isHorizontalAttribute = typeBuilder.booleanAttribute(BPMNDI_ATTRIBUTE_IS_HORIZONTAL).build();

		isExpandedAttribute = typeBuilder.booleanAttribute(BPMNDI_ATTRIBUTE_IS_EXPANDED).build();

		isMarkerVisibleAttribute = typeBuilder.booleanAttribute(BPMNDI_ATTRIBUTE_IS_MARKER_VISIBLE).build();

		isMessageVisibleAttribute = typeBuilder.booleanAttribute(BPMNDI_ATTRIBUTE_IS_MESSAGE_VISIBLE).build();

		participantBandKindAttribute = typeBuilder.enumAttribute(BPMNDI_ATTRIBUTE_PARTICIPANT_BAND_KIND, typeof(ParticipantBandKind)).build();

		choreographyActivityShapeAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_CHOREOGRAPHY_ACTIVITY_SHAPE).qNameAttributeReference(typeof(BpmnShape)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		bpmnLabelChild = sequenceBuilder.element(typeof(BpmnLabel)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<BpmnShape>
	  {
		  public BpmnShape newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnShapeImpl(instanceContext);
		  }
	  }

	  public BpmnShapeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual BaseElement BpmnElement
	  {
		  get
		  {
			return bpmnElementAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			bpmnElementAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual bool Horizontal
	  {
		  get
		  {
			return isHorizontalAttribute.getValue(this);
		  }
		  set
		  {
			isHorizontalAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool Expanded
	  {
		  get
		  {
			return isExpandedAttribute.getValue(this);
		  }
		  set
		  {
			isExpandedAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool MarkerVisible
	  {
		  get
		  {
			return isMarkerVisibleAttribute.getValue(this);
		  }
		  set
		  {
			isMarkerVisibleAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool MessageVisible
	  {
		  get
		  {
			return isMessageVisibleAttribute.getValue(this);
		  }
		  set
		  {
			isMessageVisibleAttribute.setValue(this, value);
		  }
	  }


	  public virtual ParticipantBandKind ParticipantBandKind
	  {
		  get
		  {
			return participantBandKindAttribute.getValue(this);
		  }
		  set
		  {
			participantBandKindAttribute.setValue(this, value);
		  }
	  }


	  public virtual BpmnShape ChoreographyActivityShape
	  {
		  get
		  {
			return choreographyActivityShapeAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			choreographyActivityShapeAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual BpmnLabel BpmnLabel
	  {
		  get
		  {
			return bpmnLabelChild.getChild(this);
		  }
		  set
		  {
			bpmnLabelChild.setChild(this, value);
		  }
	  }

	}

}