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
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using LinkEventDefinition = org.camunda.bpm.model.bpmn.instance.LinkEventDefinition;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN linkEventDefinition element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class LinkEventDefinitionImpl : EventDefinitionImpl, LinkEventDefinition
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReferenceCollection<LinkEventDefinition, Source> sourceCollection;
	  protected internal static ElementReference<LinkEventDefinition, Target> targetChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(LinkEventDefinition), BPMN_ELEMENT_LINK_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).required().build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		sourceCollection = sequenceBuilder.elementCollection(typeof(Source)).qNameElementReferenceCollection(typeof(LinkEventDefinition)).build();

		targetChild = sequenceBuilder.element(typeof(Target)).qNameElementReference(typeof(LinkEventDefinition)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<LinkEventDefinition>
	  {
		  public LinkEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new LinkEventDefinitionImpl(instanceContext);
		  }
	  }

	  public LinkEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual ICollection<LinkEventDefinition> Sources
	  {
		  get
		  {
			return sourceCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual LinkEventDefinition Target
	  {
		  get
		  {
			return targetChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			targetChild.setReferenceTargetElement(this, value);
		  }
	  }


	}

}