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
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ItemDefinitionImpl : RootElementImpl, ItemDefinition
	{

	  protected internal static Attribute<string> structureRefAttribute;
	  protected internal static Attribute<bool> isCollectionAttribute;
	  protected internal static Attribute<ItemKind> itemKindAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ItemDefinition),BPMN_ELEMENT_ITEM_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		structureRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_STRUCTURE_REF).build();

		isCollectionAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_IS_COLLECTION).defaultValue(false).build();

		itemKindAttribute = typeBuilder.enumAttribute(BPMN_ATTRIBUTE_ITEM_KIND, typeof(ItemKind)).defaultValue(ItemKind.Information).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ItemDefinition>
	  {
		  public ItemDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ItemDefinitionImpl(instanceContext);
		  }
	  }

	  public ItemDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual string StructureRef
	  {
		  get
		  {
			return structureRefAttribute.getValue(this);
		  }
		  set
		  {
			structureRefAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool Collection
	  {
		  get
		  {
			return isCollectionAttribute.getValue(this);
		  }
		  set
		  {
			isCollectionAttribute.setValue(this, value);
		  }
	  }


	  public virtual ItemKind ItemKind
	  {
		  get
		  {
			return itemKindAttribute.getValue(this);
		  }
		  set
		  {
			itemKindAttribute.setValue(this, value);
		  }
	  }

	}

}