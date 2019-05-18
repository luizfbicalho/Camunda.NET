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
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_THROW_EVENT;

	/// <summary>
	/// The BPMN throwEvent element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class ThrowEventImpl : EventImpl, ThrowEvent
	{

	  protected internal static ChildElementCollection<DataInput> dataInputCollection;
	  protected internal static ChildElementCollection<DataInputAssociation> dataInputAssociationCollection;
	  protected internal static ChildElement<InputSet> inputSetChild;
	  protected internal static ChildElementCollection<EventDefinition> eventDefinitionCollection;
	  protected internal static ElementReferenceCollection<EventDefinition, EventDefinitionRef> eventDefinitionRefCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ThrowEvent), BPMN_ELEMENT_THROW_EVENT).namespaceUri(BPMN20_NS).extendsType(typeof(Event)).abstractType();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataInputCollection = sequenceBuilder.elementCollection(typeof(DataInput)).build();

		dataInputAssociationCollection = sequenceBuilder.elementCollection(typeof(DataInputAssociation)).build();

		inputSetChild = sequenceBuilder.element(typeof(InputSet)).build();

		eventDefinitionCollection = sequenceBuilder.elementCollection(typeof(EventDefinition)).build();

		eventDefinitionRefCollection = sequenceBuilder.elementCollection(typeof(EventDefinitionRef)).qNameElementReferenceCollection(typeof(EventDefinition)).build();

		typeBuilder.build();
	  }


	  public ThrowEventImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual ICollection<DataInput> DataInputs
	  {
		  get
		  {
			return dataInputCollection.get(this);
		  }
	  }

	  public virtual ICollection<DataInputAssociation> DataInputAssociations
	  {
		  get
		  {
			return dataInputAssociationCollection.get(this);
		  }
	  }

	  public virtual InputSet InputSet
	  {
		  get
		  {
			return inputSetChild.getChild(this);
		  }
		  set
		  {
			inputSetChild.setChild(this, value);
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