﻿/*
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
	using Expression = org.camunda.bpm.model.bpmn.instance.Expression;
	using ResourceParameter = org.camunda.bpm.model.bpmn.instance.ResourceParameter;
	using ResourceParameterBinding = org.camunda.bpm.model.bpmn.instance.ResourceParameterBinding;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN resourceParameterBinding element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ResourceParameterBindingImpl : BaseElementImpl, ResourceParameterBinding
	{

	  protected internal static AttributeReference<ResourceParameter> parameterRefAttribute;
	  protected internal static ChildElement<Expression> expressionChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ResourceParameterBinding), BPMN_ELEMENT_RESOURCE_PARAMETER_BINDING).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		parameterRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_PARAMETER_REF).required().qNameAttributeReference(typeof(ResourceParameter)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		expressionChild = sequenceBuilder.element(typeof(Expression)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ResourceParameterBinding>
	  {
		  public ResourceParameterBinding newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ResourceParameterBindingImpl(instanceContext);
		  }
	  }

	  public ResourceParameterBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual ResourceParameter Parameter
	  {
		  get
		  {
			return parameterRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			parameterRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual Expression Expression
	  {
		  get
		  {
			return expressionChild.getChild(this);
		  }
		  set
		  {
			expressionChild.setChild(this, value);
		  }
	  }

	}

}