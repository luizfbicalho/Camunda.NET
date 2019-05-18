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
	using ComplexGatewayBuilder = org.camunda.bpm.model.bpmn.builder.ComplexGatewayBuilder;
	using ActivationCondition = org.camunda.bpm.model.bpmn.instance.ActivationCondition;
	using ComplexGateway = org.camunda.bpm.model.bpmn.instance.ComplexGateway;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_DEFAULT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_COMPLEX_GATEWAY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN complexGateway element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ComplexGatewayImpl : GatewayImpl, ComplexGateway
	{

	  protected internal static AttributeReference<SequenceFlow> defaultAttribute;
	  protected internal static ChildElement<ActivationCondition> activationConditionChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ComplexGateway), BPMN_ELEMENT_COMPLEX_GATEWAY).namespaceUri(BPMN20_NS).extendsType(typeof(Gateway)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		defaultAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_DEFAULT).idAttributeReference(typeof(SequenceFlow)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		activationConditionChild = sequenceBuilder.element(typeof(ActivationCondition)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ComplexGateway>
	  {
		  public ComplexGateway newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ComplexGatewayImpl(instanceContext);
		  }
	  }

	  public ComplexGatewayImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override ComplexGatewayBuilder builder()
	  {
		return new ComplexGatewayBuilder((BpmnModelInstance) modelInstance, this);
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


	  public virtual ActivationCondition ActivationCondition
	  {
		  get
		  {
			return activationConditionChild.getChild(this);
		  }
		  set
		  {
			activationConditionChild.setChild(this, value);
		  }
	  }


	}

}