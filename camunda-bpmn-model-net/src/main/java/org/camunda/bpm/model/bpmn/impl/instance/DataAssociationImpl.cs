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
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_DATA_ASSOCIATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN dataAssociation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class DataAssociationImpl : BaseElementImpl, DataAssociation
	{

	  protected internal static ElementReferenceCollection<ItemAwareElement, SourceRef> sourceRefCollection;
	  protected internal static ElementReference<ItemAwareElement, TargetRef> targetRefChild;
	  protected internal static ChildElement<Transformation> transformationChild;
	  protected internal static ChildElementCollection<Assignment> assignmentCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(DataAssociation), BPMN_ELEMENT_DATA_ASSOCIATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		sourceRefCollection = sequenceBuilder.elementCollection(typeof(SourceRef)).idElementReferenceCollection(typeof(ItemAwareElement)).build();

		targetRefChild = sequenceBuilder.element(typeof(TargetRef)).required().idElementReference(typeof(ItemAwareElement)).build();

		transformationChild = sequenceBuilder.element(typeof(Transformation)).build();

		assignmentCollection = sequenceBuilder.elementCollection(typeof(Assignment)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<DataAssociation>
	  {
		  public DataAssociation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new DataAssociationImpl(instanceContext);
		  }
	  }

	  public DataAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ICollection<ItemAwareElement> Sources
	  {
		  get
		  {
			return sourceRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual ItemAwareElement Target
	  {
		  get
		  {
			return targetRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual FormalExpression getTransformation()
	  {
		return transformationChild.getChild(this);
	  }

	  public virtual void setTransformation(Transformation transformation)
	  {
		transformationChild.setChild(this, transformation);
	  }

	  public virtual ICollection<Assignment> Assignments
	  {
		  get
		  {
			return assignmentCollection.get(this);
		  }
	  }

	  public override BpmnEdge DiagramElement
	  {
		  get
		  {
			return (BpmnEdge) base.DiagramElement;
		  }
	  }
	}

}