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
	using EventBasedGatewayBuilder = org.camunda.bpm.model.bpmn.builder.EventBasedGatewayBuilder;
	using EventBasedGateway = org.camunda.bpm.model.bpmn.instance.EventBasedGateway;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN eventBasedGateway element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class EventBasedGatewayImpl : GatewayImpl, EventBasedGateway
	{

	  protected internal static Attribute<bool> instantiateAttribute;
	  protected internal static Attribute<EventBasedGatewayType> eventGatewayTypeAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(EventBasedGateway), BPMN_ELEMENT_EVENT_BASED_GATEWAY).namespaceUri(BPMN20_NS).extendsType(typeof(Gateway)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		instantiateAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_INSTANTIATE).defaultValue(false).build();

		eventGatewayTypeAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_EVENT_GATEWAY_TYPE, typeof(EventBasedGatewayType)).defaultValue(EventBasedGatewayType.Exclusive).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<EventBasedGateway>
	  {
		  public EventBasedGateway newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new EventBasedGatewayImpl(instanceContext);
		  }
	  }

	  public EventBasedGatewayImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override EventBasedGatewayBuilder builder()
	  {
		return new EventBasedGatewayBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual bool Instantiate
	  {
		  get
		  {
			return instantiateAttribute.getValue(this);
		  }
		  set
		  {
			instantiateAttribute.setValue(this, value);
		  }
	  }


	  public virtual EventBasedGatewayType EventGatewayType
	  {
		  get
		  {
			return eventGatewayTypeAttribute.getValue(this);
		  }
		  set
		  {
			eventGatewayTypeAttribute.setValue(this, value);
		  }
	  }


	  public override bool CamundaAsyncAfter
	  {
		  get
		  {
			throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
		  }
		  set
		  {
			throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
		  }
	  }


	}

}