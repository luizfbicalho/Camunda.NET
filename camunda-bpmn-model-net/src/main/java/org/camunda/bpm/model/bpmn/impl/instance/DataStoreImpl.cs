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
	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	using DataState = org.camunda.bpm.model.bpmn.instance.DataState;
	using DataStore = org.camunda.bpm.model.bpmn.instance.DataStore;
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	/// <summary>
	/// The BPMN dataStore element
	/// 
	/// @author Falko Menge
	/// </summary>
	public class DataStoreImpl : RootElementImpl, DataStore
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<int> capacityAttribute;
	  protected internal static Attribute<bool> isUnlimitedAttribute;
	  protected internal static AttributeReference<ItemDefinition> itemSubjectRefAttribute;
	  protected internal static ChildElement<DataState> dataStateChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(DataStore), BPMN_ELEMENT_DATA_STORE).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		capacityAttribute = typeBuilder.integerAttribute(BPMN_ATTRIBUTE_CAPACITY).build();

		isUnlimitedAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_UNLIMITED).defaultValue(true).build();

		itemSubjectRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ITEM_SUBJECT_REF).qNameAttributeReference(typeof(ItemDefinition)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataStateChild = sequenceBuilder.element(typeof(DataState)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<DataStore>
	  {
		  public DataStore newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new DataStoreImpl(instanceContext);
		  }
	  }

	  public DataStoreImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual int? Capacity
	  {
		  get
		  {
			return capacityAttribute.getValue(this);
		  }
		  set
		  {
			capacityAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool? Unlimited
	  {
		  get
		  {
			return isUnlimitedAttribute.getValue(this);
		  }
		  set
		  {
			isUnlimitedAttribute.setValue(this, value);
		  }
	  }


	  public virtual ItemDefinition ItemSubject
	  {
		  get
		  {
			return itemSubjectRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			itemSubjectRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DataState DataState
	  {
		  get
		  {
			return dataStateChild.getChild(this);
		  }
		  set
		  {
			dataStateChild.setChild(this, value);
		  }
	  }


	}

}