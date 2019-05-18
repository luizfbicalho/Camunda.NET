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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using CorrelationKey = org.camunda.bpm.model.bpmn.instance.CorrelationKey;
	using CorrelationPropertyBinding = org.camunda.bpm.model.bpmn.instance.CorrelationPropertyBinding;
	using CorrelationSubscription = org.camunda.bpm.model.bpmn.instance.CorrelationSubscription;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN correlationSubscription element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CorrelationSubscriptionImpl : BaseElementImpl, CorrelationSubscription
	{

	  protected internal static AttributeReference<CorrelationKey> correlationKeyAttribute;
	  protected internal static ChildElementCollection<CorrelationPropertyBinding> correlationPropertyBindingCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(CorrelationSubscription), BPMN_ELEMENT_CORRELATION_SUBSCRIPTION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		correlationKeyAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_CORRELATION_KEY_REF).required().qNameAttributeReference(typeof(CorrelationKey)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		correlationPropertyBindingCollection = sequenceBuilder.elementCollection(typeof(CorrelationPropertyBinding)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<CorrelationSubscription>
	  {
		  public CorrelationSubscription newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CorrelationSubscriptionImpl(instanceContext);
		  }
	  }

	  public CorrelationSubscriptionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual CorrelationKey CorrelationKey
	  {
		  get
		  {
			return correlationKeyAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			correlationKeyAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<CorrelationPropertyBinding> CorrelationPropertyBindings
	  {
		  get
		  {
			return correlationPropertyBindingCollection.get(this);
		  }
	  }
	}

}