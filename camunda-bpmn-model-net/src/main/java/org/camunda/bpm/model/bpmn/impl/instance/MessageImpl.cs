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
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN message event
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class MessageImpl : RootElementImpl, Message
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static AttributeReference<ItemDefinition> itemRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Message), BPMN_ELEMENT_MESSAGE).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		itemRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ITEM_REF).qNameAttributeReference(typeof(ItemDefinition)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<Message>
	  {
		  public Message newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new MessageImpl(instanceContext);
		  }
	  }

	  public MessageImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual ItemDefinition Item
	  {
		  get
		  {
			return itemRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			itemRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	}

}