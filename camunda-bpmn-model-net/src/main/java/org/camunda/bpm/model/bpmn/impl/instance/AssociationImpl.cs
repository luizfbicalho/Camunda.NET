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
	using Artifact = org.camunda.bpm.model.bpmn.instance.Artifact;
	using Association = org.camunda.bpm.model.bpmn.instance.Association;
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AssociationImpl : ArtifactImpl, Association
	{

	  protected internal static AttributeReference<BaseElement> sourceRefAttribute;
	  protected internal static AttributeReference<BaseElement> targetRefAttribute;
	  protected internal static Attribute<AssociationDirection> associationDirectionAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Association), BPMN_ELEMENT_ASSOCIATION).namespaceUri(BPMN20_NS).extendsType(typeof(Artifact)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		sourceRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_SOURCE_REF).required().qNameAttributeReference(typeof(BaseElement)).build();

		targetRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TARGET_REF).required().qNameAttributeReference(typeof(BaseElement)).build();

		associationDirectionAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_ASSOCIATION_DIRECTION, typeof(AssociationDirection)).defaultValue(AssociationDirection.None).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Association>
	  {
		  public Association newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new AssociationImpl(instanceContext);
		  }
	  }

	  public AssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual BaseElement Source
	  {
		  get
		  {
			return sourceRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			sourceRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual BaseElement Target
	  {
		  get
		  {
			return targetRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual AssociationDirection AssociationDirection
	  {
		  get
		  {
			return associationDirectionAttribute.getValue(this);
		  }
		  set
		  {
			associationDirectionAttribute.setValue(this, value);
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