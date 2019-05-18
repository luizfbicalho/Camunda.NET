using System.Collections.Generic;

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
	using org.camunda.bpm.model.bpmn.instance;
	using BpmnDiagram = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnDiagram;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN definitions element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class DefinitionsImpl : BpmnModelElementInstanceImpl, Definitions
	{

	  protected internal static Attribute<string> idAttribute;
	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<string> targetNamespaceAttribute;
	  protected internal static Attribute<string> expressionLanguageAttribute;
	  protected internal static Attribute<string> typeLanguageAttribute;
	  protected internal static Attribute<string> exporterAttribute;
	  protected internal static Attribute<string> exporterVersionAttribute;

	  protected internal static ChildElementCollection<Import> importCollection;
	  protected internal static ChildElementCollection<Extension> extensionCollection;
	  protected internal static ChildElementCollection<RootElement> rootElementCollection;
	  protected internal static ChildElementCollection<BpmnDiagram> bpmnDiagramCollection;
	  protected internal static ChildElementCollection<Relationship> relationshipCollection;

	  public static void registerType(ModelBuilder bpmnModelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = bpmnModelBuilder.defineType(typeof(Definitions), BPMN_ELEMENT_DEFINITIONS).namespaceUri(BPMN20_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		idAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ID).idAttribute().build();

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		targetNamespaceAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TARGET_NAMESPACE).required().build();

		expressionLanguageAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_EXPRESSION_LANGUAGE).defaultValue(XPATH_NS).build();

		typeLanguageAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TYPE_LANGUAGE).defaultValue(XML_SCHEMA_NS).build();

		exporterAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_EXPORTER).build();

		exporterVersionAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_EXPORTER_VERSION).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		importCollection = sequenceBuilder.elementCollection(typeof(Import)).build();

		extensionCollection = sequenceBuilder.elementCollection(typeof(Extension)).build();

		rootElementCollection = sequenceBuilder.elementCollection(typeof(RootElement)).build();

		bpmnDiagramCollection = sequenceBuilder.elementCollection(typeof(BpmnDiagram)).build();

		relationshipCollection = sequenceBuilder.elementCollection(typeof(Relationship)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<Definitions>
	  {
		  public Definitions newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new DefinitionsImpl(instanceContext);
		  }
	  }

	  public DefinitionsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return idAttribute.getValue(this);
		  }
		  set
		  {
			idAttribute.setValue(this, value);
		  }
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


	  public virtual string TargetNamespace
	  {
		  get
		  {
			return targetNamespaceAttribute.getValue(this);
		  }
		  set
		  {
			targetNamespaceAttribute.setValue(this, value);
		  }
	  }


	  public virtual string ExpressionLanguage
	  {
		  get
		  {
			return expressionLanguageAttribute.getValue(this);
		  }
		  set
		  {
			expressionLanguageAttribute.setValue(this, value);
		  }
	  }


	  public virtual string TypeLanguage
	  {
		  get
		  {
			return typeLanguageAttribute.getValue(this);
		  }
		  set
		  {
			typeLanguageAttribute.setValue(this, value);
		  }
	  }


	  public virtual string Exporter
	  {
		  get
		  {
			return exporterAttribute.getValue(this);
		  }
		  set
		  {
			exporterAttribute.setValue(this, value);
		  }
	  }


	  public virtual string ExporterVersion
	  {
		  get
		  {
			return exporterVersionAttribute.getValue(this);
		  }
		  set
		  {
			exporterVersionAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Import> Imports
	  {
		  get
		  {
			return importCollection.get(this);
		  }
	  }

	  public virtual ICollection<Extension> Extensions
	  {
		  get
		  {
			return extensionCollection.get(this);
		  }
	  }

	  public virtual ICollection<RootElement> RootElements
	  {
		  get
		  {
			return rootElementCollection.get(this);
		  }
	  }

	  public virtual ICollection<BpmnDiagram> BpmDiagrams
	  {
		  get
		  {
			return bpmnDiagramCollection.get(this);
		  }
	  }

	  public virtual ICollection<Relationship> Relationships
	  {
		  get
		  {
			return relationshipCollection.get(this);
		  }
	  }

	}

}