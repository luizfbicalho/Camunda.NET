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
	using AbstractGatewayBuilder = org.camunda.bpm.model.bpmn.builder.AbstractGatewayBuilder;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using BpmnShape = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnShape;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN gateway element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public abstract class GatewayImpl : FlowNodeImpl, Gateway
	{

	  protected internal static Attribute<GatewayDirection> gatewayDirectionAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Gateway), BPMN_ELEMENT_GATEWAY).namespaceUri(BPMN20_NS).extendsType(typeof(FlowNode)).abstractType();

		gatewayDirectionAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_GATEWAY_DIRECTION, typeof(GatewayDirection)).defaultValue(GatewayDirection.Unspecified).build();

		typeBuilder.build();
	  }

	  public GatewayImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public abstract org.camunda.bpm.model.bpmn.builder.AbstractGatewayBuilder builder();
	  public override abstract AbstractGatewayBuilder builder();

	  public virtual GatewayDirection GatewayDirection
	  {
		  get
		  {
			return gatewayDirectionAttribute.getValue(this);
		  }
		  set
		  {
			gatewayDirectionAttribute.setValue(this, value);
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