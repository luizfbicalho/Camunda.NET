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
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN resourceRole element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ResourceRoleImpl : BaseElementImpl, ResourceRole
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReference<Resource, ResourceRef> resourceRefChild;
	  protected internal static ChildElementCollection<ResourceParameterBinding> resourceParameterBindingCollection;
	  protected internal static ChildElement<ResourceAssignmentExpression> resourceAssignmentExpressionChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ResourceRole), BPMN_ELEMENT_RESOURCE_ROLE).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		resourceRefChild = sequenceBuilder.element(typeof(ResourceRef)).qNameElementReference(typeof(Resource)).build();

		resourceParameterBindingCollection = sequenceBuilder.elementCollection(typeof(ResourceParameterBinding)).build();

		resourceAssignmentExpressionChild = sequenceBuilder.element(typeof(ResourceAssignmentExpression)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ResourceRole>
	  {
		  public ResourceRole newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ResourceRoleImpl(instanceContext);
		  }
	  }

	  public ResourceRoleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual Resource Resource
	  {
		  get
		  {
			return resourceRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			resourceRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<ResourceParameterBinding> ResourceParameterBinding
	  {
		  get
		  {
			return resourceParameterBindingCollection.get(this);
		  }
	  }

	  public virtual ResourceAssignmentExpression ResourceAssignmentExpression
	  {
		  get
		  {
			return resourceAssignmentExpressionChild.getChild(this);
		  }
	  }
	}

}