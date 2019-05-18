using System;

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
	using ParallelGatewayBuilder = org.camunda.bpm.model.bpmn.builder.ParallelGatewayBuilder;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using ParallelGateway = org.camunda.bpm.model.bpmn.instance.ParallelGateway;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_PARALLEL_GATEWAY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_ASYNC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_ATTRIBUTE_EXCLUSIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN parallelGateway element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ParallelGatewayImpl : GatewayImpl, ParallelGateway
	{

	  protected internal static Attribute<bool> camundaAsyncAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ParallelGateway), BPMN_ELEMENT_PARALLEL_GATEWAY).namespaceUri(BPMN20_NS).extendsType(typeof(Gateway)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		/// <summary>
		/// camunda extensions </summary>

		camundaAsyncAttribute = typeBuilder.booleanAttribute(CAMUNDA_ATTRIBUTE_ASYNC).@namespace(CAMUNDA_NS).defaultValue(false).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ParallelGateway>
	  {
		  public ParallelGateway newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ParallelGatewayImpl(instanceContext);
		  }
	  }

	  public override ParallelGatewayBuilder builder()
	  {
		return new ParallelGatewayBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  public virtual bool CamundaAsync
	  {
		  get
		  {
			return camundaAsyncAttribute.getValue(this);
		  }
		  set
		  {
			camundaAsyncAttribute.setValue(this, value);
		  }
	  }


	  public ParallelGatewayImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	}

}