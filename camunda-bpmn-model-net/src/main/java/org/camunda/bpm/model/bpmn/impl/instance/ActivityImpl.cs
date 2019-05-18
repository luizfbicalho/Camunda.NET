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
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN activity element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class ActivityImpl : FlowNodeImpl, Activity
	{

	  protected internal static Attribute<bool> isForCompensationAttribute;
	  protected internal static Attribute<int> startQuantityAttribute;
	  protected internal static Attribute<int> completionQuantityAttribute;
	  protected internal static AttributeReference<SequenceFlow> defaultAttribute;
	  protected internal static ChildElement<IoSpecification> ioSpecificationChild;
	  protected internal static ChildElementCollection<Property> propertyCollection;
	  protected internal static ChildElementCollection<DataInputAssociation> dataInputAssociationCollection;
	  protected internal static ChildElementCollection<DataOutputAssociation> dataOutputAssociationCollection;
	  protected internal static ChildElementCollection<ResourceRole> resourceRoleCollection;
	  protected internal static ChildElement<LoopCharacteristics> loopCharacteristicsChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Activity), BPMN_ELEMENT_ACTIVITY).namespaceUri(BPMN20_NS).extendsType(typeof(FlowNode)).abstractType();

		isForCompensationAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_FOR_COMPENSATION).defaultValue(false).build();

		startQuantityAttribute = typeBuilder.integerAttribute(BPMN_ATTRIBUTE_START_QUANTITY).defaultValue(1).build();

		completionQuantityAttribute = typeBuilder.integerAttribute(BPMN_ATTRIBUTE_COMPLETION_QUANTITY).defaultValue(1).build();

		defaultAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_DEFAULT).idAttributeReference(typeof(SequenceFlow)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		ioSpecificationChild = sequenceBuilder.element(typeof(IoSpecification)).build();

		propertyCollection = sequenceBuilder.elementCollection(typeof(Property)).build();

		dataInputAssociationCollection = sequenceBuilder.elementCollection(typeof(DataInputAssociation)).build();

		dataOutputAssociationCollection = sequenceBuilder.elementCollection(typeof(DataOutputAssociation)).build();

		resourceRoleCollection = sequenceBuilder.elementCollection(typeof(ResourceRole)).build();

		loopCharacteristicsChild = sequenceBuilder.element(typeof(LoopCharacteristics)).build();

		typeBuilder.build();
	  }

	  public ActivityImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual bool ForCompensation
	  {
		  get
		  {
			return isForCompensationAttribute.getValue(this);
		  }
		  set
		  {
			isForCompensationAttribute.setValue(this, value);
		  }
	  }


	  public virtual int StartQuantity
	  {
		  get
		  {
			return startQuantityAttribute.getValue(this);
		  }
		  set
		  {
			startQuantityAttribute.setValue(this, value);
		  }
	  }


	  public virtual int CompletionQuantity
	  {
		  get
		  {
			return completionQuantityAttribute.getValue(this);
		  }
		  set
		  {
			completionQuantityAttribute.setValue(this, value);
		  }
	  }


	  public virtual SequenceFlow Default
	  {
		  get
		  {
			return defaultAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			defaultAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual IoSpecification IoSpecification
	  {
		  get
		  {
			return ioSpecificationChild.getChild(this);
		  }
		  set
		  {
			ioSpecificationChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<Property> Properties
	  {
		  get
		  {
			return propertyCollection.get(this);
		  }
	  }

	  public virtual ICollection<DataInputAssociation> DataInputAssociations
	  {
		  get
		  {
			return dataInputAssociationCollection.get(this);
		  }
	  }

	  public virtual ICollection<DataOutputAssociation> DataOutputAssociations
	  {
		  get
		  {
			return dataOutputAssociationCollection.get(this);
		  }
	  }

	  public virtual ICollection<ResourceRole> ResourceRoles
	  {
		  get
		  {
			return resourceRoleCollection.get(this);
		  }
	  }

	  public virtual LoopCharacteristics LoopCharacteristics
	  {
		  get
		  {
			return loopCharacteristicsChild.getChild(this);
		  }
		  set
		  {
			loopCharacteristicsChild.setChild(this, value);
		  }
	  }

	}

}