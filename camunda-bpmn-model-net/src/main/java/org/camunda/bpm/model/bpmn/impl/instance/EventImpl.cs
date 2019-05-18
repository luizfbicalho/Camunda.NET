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
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using Property = org.camunda.bpm.model.bpmn.instance.Property;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_EVENT;

	/// <summary>
	/// The BPMN event element
	/// 
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public abstract class EventImpl : FlowNodeImpl, Event
	{

	  protected internal static ChildElementCollection<Property> propertyCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Event), BPMN_ELEMENT_EVENT).namespaceUri(BPMN20_NS).extendsType(typeof(FlowNode)).abstractType();

		SequenceBuilder sequence = typeBuilder.sequence();

		propertyCollection = sequence.elementCollection(typeof(Property)).build();

		typeBuilder.build();
	  }

	  public EventImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual ICollection<Property> Properties
	  {
		  get
		  {
			return propertyCollection.get(this);
		  }
	  }

	  public override BpmnShape DiagramElement
	  {
		  get
		  {
			return (BpmnShape) base.DiagramElement;
		  }
	  }

	}

}