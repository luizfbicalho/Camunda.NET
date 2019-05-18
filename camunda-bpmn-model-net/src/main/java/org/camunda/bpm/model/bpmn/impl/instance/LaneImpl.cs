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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using Lane = org.camunda.bpm.model.bpmn.instance.Lane;
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
	/// The BPMN lane element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class LaneImpl : BaseElementImpl, Lane
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static AttributeReference<PartitionElement> partitionElementRefAttribute;
	  protected internal static ChildElement<PartitionElement> partitionElementChild;
	  protected internal static ElementReferenceCollection<FlowNode, FlowNodeRef> flowNodeRefCollection;
	  protected internal static ChildElement<ChildLaneSet> childLaneSetChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Lane), BPMN_ELEMENT_LANE).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		partitionElementRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_PARTITION_ELEMENT_REF).qNameAttributeReference(typeof(PartitionElement)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		partitionElementChild = sequenceBuilder.element(typeof(PartitionElement)).build();

		flowNodeRefCollection = sequenceBuilder.elementCollection(typeof(FlowNodeRef)).idElementReferenceCollection(typeof(FlowNode)).build();

		childLaneSetChild = sequenceBuilder.element(typeof(ChildLaneSet)).build();



		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Lane>
	  {
		  public Lane newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new LaneImpl(instanceContext);
		  }
	  }

	  public LaneImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual PartitionElement PartitionElement
	  {
		  get
		  {
			return partitionElementRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			partitionElementRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual PartitionElement PartitionElementChild
	  {
		  get
		  {
			return partitionElementChild.getChild(this);
		  }
		  set
		  {
			partitionElementChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<FlowNode> FlowNodeRefs
	  {
		  get
		  {
			return flowNodeRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ChildLaneSet ChildLaneSet
	  {
		  get
		  {
			return childLaneSetChild.getChild(this);
		  }
		  set
		  {
			childLaneSetChild.setChild(this, value);
		  }
	  }

	}

}