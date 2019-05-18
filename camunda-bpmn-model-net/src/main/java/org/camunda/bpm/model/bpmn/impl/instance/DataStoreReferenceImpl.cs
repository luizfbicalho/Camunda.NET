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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_DATA_STORE_REFERENCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_ITEM_SUBJECT_REF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ATTRIBUTE_DATA_STORE_REF;

	using org.camunda.bpm.model.bpmn.instance;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	/// <summary>
	/// @author Giulio Piccinin
	/// </summary>
	public class DataStoreReferenceImpl : FlowElementImpl, DataStoreReference
	{

		protected internal static AttributeReference<ItemDefinition> itemSubjectRefAttribute;
		protected internal static AttributeReference<DataStore> dataStoreRefAttribute;
		protected internal static ChildElement<DataState> dataStateChild;

		public static void registerType(ModelBuilder modelBuilder)
		{
			ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(DataStoreReference), BPMN_ELEMENT_DATA_STORE_REFERENCE).namespaceUri(BPMN20_NS).extendsType(typeof(FlowElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

			itemSubjectRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ITEM_SUBJECT_REF).qNameAttributeReference(typeof(ItemDefinition)).build();

			dataStoreRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_DATA_STORE_REF).idAttributeReference(typeof(DataStore)).build();

			SequenceBuilder sequenceBuilder = typeBuilder.sequence();

			dataStateChild = sequenceBuilder.element(typeof(DataState)).build();

			typeBuilder.build();
		}

		private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<DataStoreReference>
		{
			public override DataStoreReference newInstance(ModelTypeInstanceContext instanceContext)
			{
				return new DataStoreReferenceImpl(instanceContext);
			}
		}

		public DataStoreReferenceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
		{
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


		public virtual DataStore DataStore
		{
			get
			{
				return dataStoreRefAttribute.getReferenceTargetElement(this);
			}
			set
			{
				dataStoreRefAttribute.setReferenceTargetElement(this, value);
			}
		}

	}
}