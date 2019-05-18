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
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN flowElement element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public abstract class FlowElementImpl : BaseElementImpl, FlowElement
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ChildElement<Auditing> auditingChild;
	  protected internal static ChildElement<Monitoring> monitoringChild;
	  protected internal static ElementReferenceCollection<CategoryValue, CategoryValueRef> categoryValueRefCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(FlowElement), BPMN_ELEMENT_FLOW_ELEMENT).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).abstractType();

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		auditingChild = sequenceBuilder.element(typeof(Auditing)).build();

		monitoringChild = sequenceBuilder.element(typeof(Monitoring)).build();

		categoryValueRefCollection = sequenceBuilder.elementCollection(typeof(CategoryValueRef)).qNameElementReferenceCollection(typeof(CategoryValue)).build();

		typeBuilder.build();
	  }

	  public FlowElementImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual Auditing Auditing
	  {
		  get
		  {
			return auditingChild.getChild(this);
		  }
		  set
		  {
			auditingChild.setChild(this, value);
		  }
	  }


	  public virtual Monitoring Monitoring
	  {
		  get
		  {
			return monitoringChild.getChild(this);
		  }
		  set
		  {
			monitoringChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<CategoryValue> CategoryValueRefs
	  {
		  get
		  {
			return categoryValueRefCollection.getReferenceTargetElements(this);
		  }
	  }
	}

}