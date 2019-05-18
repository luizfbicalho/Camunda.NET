using System;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_EXTENSION_ELEMENTS;

	using ExtensionElements = org.camunda.bpm.model.bpmn.instance.ExtensionElements;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;

	/// <summary>
	/// The BPMN extensionElements element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class ExtensionElementsImpl : BpmnModelElementInstanceImpl, ExtensionElements
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {

		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ExtensionElements), BPMN_ELEMENT_EXTENSION_ELEMENTS).namespaceUri(BPMN20_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<ExtensionElements>
	  {
		  public ExtensionElements newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ExtensionElementsImpl(instanceContext);
		  }
	  }

	  public ExtensionElementsImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual ICollection<ModelElementInstance> Elements
	  {
		  get
		  {
			return ModelUtil.getModelElementCollection(DomElement.ChildElements, modelInstance);
		  }
	  }

	  public virtual Query<ModelElementInstance> ElementsQuery
	  {
		  get
		  {
			return new QueryImpl<ModelElementInstance>(Elements);
		  }
	  }

	  public virtual ModelElementInstance addExtensionElement(string namespaceUri, string localName)
	  {
		ModelElementType extensionElementType = modelInstance.registerGenericType(namespaceUri, localName);
		ModelElementInstance extensionElement = extensionElementType.newInstance(modelInstance);
		addChildElement(extensionElement);
		return extensionElement;
	  }

	  public virtual T addExtensionElement<T>(Type<T> extensionElementClass) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ModelElementInstance extensionElement = modelInstance.newInstance(extensionElementClass);
		addChildElement(extensionElement);
		return extensionElementClass.cast(extensionElement);
	  }

	  public override void addChildElement(ModelElementInstance extensionElement)
	  {
		DomElement.appendChild(extensionElement.DomElement);
	  }

	}

}