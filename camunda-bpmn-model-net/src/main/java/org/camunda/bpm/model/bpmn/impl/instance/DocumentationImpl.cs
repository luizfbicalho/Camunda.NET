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
	using Documentation = org.camunda.bpm.model.bpmn.instance.Documentation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN documentation element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class DocumentationImpl : BpmnModelElementInstanceImpl, Documentation
	{

	  protected internal static Attribute<string> idAttribute;
	  protected internal static Attribute<string> textFormatAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Documentation), BPMN_ELEMENT_DOCUMENTATION).namespaceUri(BPMN20_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		idAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_ID).idAttribute().build();

		textFormatAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TEXT_FORMAT).defaultValue("text/plain").build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<Documentation>
	  {
		  public Documentation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new DocumentationImpl(instanceContext);
		  }
	  }

	  public DocumentationImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual string TextFormat
	  {
		  get
		  {
			return textFormatAttribute.getValue(this);
		  }
		  set
		  {
			textFormatAttribute.setValue(this, value);
		  }
	  }


	}

}