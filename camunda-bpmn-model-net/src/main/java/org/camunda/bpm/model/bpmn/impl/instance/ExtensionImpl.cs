﻿using System.Collections.Generic;

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
	using Extension = org.camunda.bpm.model.bpmn.instance.Extension;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN extension element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ExtensionImpl : BpmnModelElementInstanceImpl, Extension
	{

	  protected internal static Attribute<string> definitionAttribute;
	  protected internal static Attribute<bool> mustUnderstandAttribute;
	  protected internal static ChildElementCollection<Documentation> documentationCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Extension), BPMN_ELEMENT_EXTENSION).namespaceUri(BPMN20_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		// TODO: qname reference extension definition
		definitionAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_DEFINITION).build();

		mustUnderstandAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_MUST_UNDERSTAND).defaultValue(false).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		documentationCollection = sequenceBuilder.elementCollection(typeof(Documentation)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Extension>
	  {
		  public Extension newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ExtensionImpl(instanceContext);
		  }
	  }

	  public ExtensionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Definition
	  {
		  get
		  {
			return definitionAttribute.getValue(this);
		  }
		  set
		  {
			definitionAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool mustUnderstand()
	  {
		return mustUnderstandAttribute.getValue(this);
	  }

	  public virtual bool MustUnderstand
	  {
		  set
		  {
			mustUnderstandAttribute.setValue(this, value);
		  }
	  }

	  public virtual ICollection<Documentation> Documentations
	  {
		  get
		  {
			return documentationCollection.get(this);
		  }
	  }
	}

}