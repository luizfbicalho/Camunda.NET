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
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN catchEvent element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class CatchEventImpl : EventImpl, CatchEvent
	{

	  protected internal static Attribute<bool> parallelMultipleAttribute;
	  protected internal static ChildElementCollection<DataOutput> dataOutputCollection;
	  protected internal static ChildElementCollection<DataOutputAssociation> dataOutputAssociationCollection;
	  protected internal static ChildElement<OutputSet> outputSetChild;
	  protected internal static ChildElementCollection<EventDefinition> eventDefinitionCollection;
	  protected internal static ElementReferenceCollection<EventDefinition, EventDefinitionRef> eventDefinitionRefCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CatchEvent), BPMN_ELEMENT_CATCH_EVENT).namespaceUri(BPMN20_NS).extendsType(typeof(Event)).abstractType();

		parallelMultipleAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_PARALLEL_MULTIPLE).defaultValue(false).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataOutputCollection = sequenceBuilder.elementCollection(typeof(DataOutput)).build();

		dataOutputAssociationCollection = sequenceBuilder.elementCollection(typeof(DataOutputAssociation)).build();

		outputSetChild = sequenceBuilder.element(typeof(OutputSet)).build();

		eventDefinitionCollection = sequenceBuilder.elementCollection(typeof(EventDefinition)).build();

		eventDefinitionRefCollection = sequenceBuilder.elementCollection(typeof(EventDefinitionRef)).qNameElementReferenceCollection(typeof(EventDefinition)).build();

		typeBuilder.build();
	  }


	  public CatchEventImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual bool ParallelMultiple
	  {
		  get
		  {
			return parallelMultipleAttribute.getValue(this);
		  }
		  set
		  {
			parallelMultipleAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<DataOutput> DataOutputs
	  {
		  get
		  {
			return dataOutputCollection.get(this);
		  }
	  }

	  public virtual ICollection<DataOutputAssociation> DataOutputAssociations
	  {
		  get
		  {
			return dataOutputAssociationCollection.get(this);
		  }
	  }

	  public virtual OutputSet OutputSet
	  {
		  get
		  {
			return outputSetChild.getChild(this);
		  }
		  set
		  {
			outputSetChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<EventDefinition> EventDefinitions
	  {
		  get
		  {
			return eventDefinitionCollection.get(this);
		  }
	  }

	  public virtual ICollection<EventDefinition> EventDefinitionRefs
	  {
		  get
		  {
			return eventDefinitionRefCollection.getReferenceTargetElements(this);
		  }
	  }
	}

}