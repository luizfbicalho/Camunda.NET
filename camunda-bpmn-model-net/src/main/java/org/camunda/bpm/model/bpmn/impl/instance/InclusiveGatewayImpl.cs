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
	using InclusiveGatewayBuilder = org.camunda.bpm.model.bpmn.builder.InclusiveGatewayBuilder;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using InclusiveGateway = org.camunda.bpm.model.bpmn.instance.InclusiveGateway;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN inclusiveGateway element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class InclusiveGatewayImpl : GatewayImpl, InclusiveGateway
	{

	  protected internal static AttributeReference<SequenceFlow> defaultAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(InclusiveGateway), BPMN_ELEMENT_INCLUSIVE_GATEWAY).namespaceUri(BPMN20_NS).extendsType(typeof(Gateway)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		defaultAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_DEFAULT).idAttributeReference(typeof(SequenceFlow)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<InclusiveGateway>
	  {
		  public InclusiveGateway newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new InclusiveGatewayImpl(instanceContext);
		  }
	  }

	  public InclusiveGatewayImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override InclusiveGatewayBuilder builder()
	  {
		return new InclusiveGatewayBuilder((BpmnModelInstance) modelInstance, this);
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

	}

}