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
	using Import = org.camunda.bpm.model.bpmn.instance.Import;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN import element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class ImportImpl : BpmnModelElementInstanceImpl, Import
	{

	  protected internal static Attribute<string> namespaceAttribute;
	  protected internal static Attribute<string> locationAttribute;
	  protected internal static Attribute<string> importTypeAttribute;

	  public static void registerType(ModelBuilder bpmnModelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = bpmnModelBuilder.defineType(typeof(Import), BPMN_ELEMENT_IMPORT).namespaceUri(BPMN20_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		namespaceAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAMESPACE).required().build();

		locationAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_LOCATION).required().build();

		importTypeAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPORT_TYPE).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<Import>
	  {
		  public Import newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ImportImpl(instanceContext);
		  }
	  }

	  public ImportImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual string Namespace
	  {
		  get
		  {
			return namespaceAttribute.getValue(this);
		  }
		  set
		  {
			namespaceAttribute.setValue(this, value);
		  }
	  }


	  public virtual string Location
	  {
		  get
		  {
			return locationAttribute.getValue(this);
		  }
		  set
		  {
			locationAttribute.setValue(this, value);
		  }
	  }


	  public virtual string ImportType
	  {
		  get
		  {
			return importTypeAttribute.getValue(this);
		  }
		  set
		  {
			importTypeAttribute.setValue(this, value);
		  }
	  }


	}

}