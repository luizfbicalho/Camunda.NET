using System;
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
	using Documentation = org.camunda.bpm.model.bpmn.instance.Documentation;
	using ExtensionElements = org.camunda.bpm.model.bpmn.instance.ExtensionElements;
	using DiagramElement = org.camunda.bpm.model.bpmn.instance.di.DiagramElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;


	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN baseElement element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public abstract class BaseElementImpl : BpmnModelElementInstanceImpl, BaseElement
	{

	  protected internal static Attribute<string> idAttribute;
	  protected internal static ChildElementCollection<Documentation> documentationCollection;
	  protected internal static ChildElement<ExtensionElements> extensionElementsChild;

	  public static void registerType(ModelBuilder bpmnModelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = bpmnModelBuilder.defineType(typeof(BaseElement), BPMN_ELEMENT_BASE_ELEMENT).namespaceUri(BPMN20_NS).abstractType();

		idAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ID).idAttribute().build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		documentationCollection = sequenceBuilder.elementCollection(typeof(Documentation)).build();

		extensionElementsChild = sequenceBuilder.element(typeof(ExtensionElements)).build();

		typeBuilder.build();
	  }

	  public BaseElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return idAttribute.getValue(this);
		  }
		  set
		  {
			idAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Documentation> Documentations
	  {
		  get
		  {
			return documentationCollection.get(this);
		  }
	  }

	  public virtual ExtensionElements ExtensionElements
	  {
		  get
		  {
			return extensionElementsChild.getChild(this);
		  }
		  set
		  {
			extensionElementsChild.setChild(this, value);
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public org.camunda.bpm.model.bpmn.instance.di.DiagramElement getDiagramElement()
	  public virtual DiagramElement DiagramElement
	  {
		  get
		  {
			ICollection<Reference> incomingReferences = getIncomingReferencesByType(typeof(DiagramElement));
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
	//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> reference : incomingReferences)
			foreach (Reference<object> reference in incomingReferences)
			{
			  foreach (ModelElementInstance sourceElement in reference.findReferenceSourceElements(this))
			  {
				string referenceIdentifier = reference.getReferenceIdentifier(sourceElement);
				if (!string.ReferenceEquals(referenceIdentifier, null) && referenceIdentifier.Equals(Id))
				{
				  return (DiagramElement) sourceElement;
				}
			  }
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.Collection<org.camunda.bpm.model.xml.type.reference.Reference> getIncomingReferencesByType(Class referenceSourceTypeClass)
	  public virtual ICollection<Reference> getIncomingReferencesByType(Type referenceSourceTypeClass)
	  {
		ICollection<Reference> references = new List<Reference>();
		// we traverse all incoming references in reverse direction
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.model.xml.type.reference.Reference<?> reference : idAttribute.getIncomingReferences())
		foreach (Reference<object> reference in idAttribute.IncomingReferences)
		{

		  ModelElementType sourceElementType = reference.ReferenceSourceElementType;
		  Type sourceInstanceType = sourceElementType.InstanceType;

		  // if the referencing element (source element) is a BPMNDI element, dig deeper
		  if (referenceSourceTypeClass.IsAssignableFrom(sourceInstanceType))
		  {
		   references.Add(reference);
		  }
		}
		return references;
	  }

	}

}