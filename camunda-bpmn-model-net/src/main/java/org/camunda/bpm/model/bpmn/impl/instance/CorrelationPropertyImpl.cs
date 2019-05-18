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
	using CorrelationProperty = org.camunda.bpm.model.bpmn.instance.CorrelationProperty;
	using CorrelationPropertyRetrievalExpression = org.camunda.bpm.model.bpmn.instance.CorrelationPropertyRetrievalExpression;
	using ItemDefinition = org.camunda.bpm.model.bpmn.instance.ItemDefinition;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN correlationProperty element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CorrelationPropertyImpl : RootElementImpl, CorrelationProperty
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static AttributeReference<ItemDefinition> typeAttribute;
	  protected internal static ChildElementCollection<CorrelationPropertyRetrievalExpression> correlationPropertyRetrievalExpressionCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder;
		typeBuilder = modelBuilder.defineType(typeof(CorrelationProperty), BPMN_ELEMENT_CORRELATION_PROPERTY).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		typeAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_TYPE).qNameAttributeReference(typeof(ItemDefinition)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		correlationPropertyRetrievalExpressionCollection = sequenceBuilder.elementCollection(typeof(CorrelationPropertyRetrievalExpression)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CorrelationProperty>
	  {
		  public CorrelationProperty newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CorrelationPropertyImpl(instanceContext);
		  }
	  }

	  public CorrelationPropertyImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual ItemDefinition Type
	  {
		  get
		  {
			return typeAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			typeAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<CorrelationPropertyRetrievalExpression> CorrelationPropertyRetrievalExpressions
	  {
		  get
		  {
			return correlationPropertyRetrievalExpressionCollection.get(this);
		  }
	  }
	}

}