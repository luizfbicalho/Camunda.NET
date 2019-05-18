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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using CorrelationProperty = org.camunda.bpm.model.bpmn.instance.CorrelationProperty;
	using CorrelationPropertyBinding = org.camunda.bpm.model.bpmn.instance.CorrelationPropertyBinding;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN correlationPropertyBinding element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CorrelationPropertyBindingImpl : BaseElementImpl, CorrelationPropertyBinding
	{

	  protected internal static AttributeReference<CorrelationProperty> correlationPropertyRefAttribute;
	  protected internal static ChildElement<DataPath> dataPathChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CorrelationPropertyBinding), BPMN_ELEMENT_CORRELATION_PROPERTY_BINDING).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		correlationPropertyRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_CORRELATION_PROPERTY_REF).required().qNameAttributeReference(typeof(CorrelationProperty)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		dataPathChild = sequenceBuilder.element(typeof(DataPath)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CorrelationPropertyBinding>
	  {
		  public CorrelationPropertyBinding newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CorrelationPropertyBindingImpl(instanceContext);
		  }
	  }

	  public CorrelationPropertyBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual CorrelationProperty CorrelationProperty
	  {
		  get
		  {
			return correlationPropertyRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			correlationPropertyRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DataPath DataPath
	  {
		  get
		  {
			return dataPathChild.getChild(this);
		  }
		  set
		  {
			dataPathChild.setChild(this, value);
		  }
	  }

	}

}