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
	using LabeledEdgeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.LabeledEdgeImpl;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using BpmnLabel = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnLabel;
	using MessageVisibleKind = org.camunda.bpm.model.bpmn.instance.bpmndi.MessageVisibleKind;
	using DiagramElement = org.camunda.bpm.model.bpmn.instance.di.DiagramElement;
	using LabeledEdge = org.camunda.bpm.model.bpmn.instance.di.LabeledEdge;
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
	/// The BPMNDI BPMNEdge element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BpmnEdgeImpl : LabeledEdgeImpl, BpmnEdge
	{

	  protected internal static AttributeReference<BaseElement> bpmnElementAttribute;
	  protected internal static AttributeReference<DiagramElement> sourceElementAttribute;
	  protected internal static AttributeReference<DiagramElement> targetElementAttribute;
	  protected internal static Attribute<MessageVisibleKind> messageVisibleKindAttribute;
	  protected internal static ChildElement<BpmnLabel> bpmnLabelChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(BpmnEdge), BPMNDI_ELEMENT_BPMN_EDGE).namespaceUri(BPMNDI_NS).extendsType(typeof(LabeledEdge)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		bpmnElementAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_BPMN_ELEMENT).qNameAttributeReference(typeof(BaseElement)).build();

		sourceElementAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_SOURCE_ELEMENT).qNameAttributeReference(typeof(DiagramElement)).build();

		targetElementAttribute = typeBuilder.stringAttribute(BPMNDI_ATTRIBUTE_TARGET_ELEMENT).qNameAttributeReference(typeof(DiagramElement)).build();

		messageVisibleKindAttribute = typeBuilder.enumAttribute(BPMNDI_ATTRIBUTE_MESSAGE_VISIBLE_KIND, typeof(MessageVisibleKind)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		bpmnLabelChild = sequenceBuilder.element(typeof(BpmnLabel)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<BpmnEdge>
	  {
		  public BpmnEdge newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnEdgeImpl(instanceContext);
		  }
	  }

	  public BpmnEdgeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual DiagramElement SourceElement
	  {
		  get
		  {
			return sourceElementAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			sourceElementAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DiagramElement TargetElement
	  {
		  get
		  {
			return targetElementAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetElementAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual MessageVisibleKind MessageVisibleKind
	  {
		  get
		  {
			return messageVisibleKindAttribute.getValue(this);
		  }
		  set
		  {
			messageVisibleKindAttribute.setValue(this, value);
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