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
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using ElementReferenceCollection = org.camunda.bpm.model.xml.type.reference.ElementReferenceCollection;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN callableElement element
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public class CallableElementImpl : RootElementImpl, CallableElement
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static ElementReferenceCollection<Interface, SupportedInterfaceRef> supportedInterfaceRefCollection;
	  protected internal static ChildElement<IoSpecification> ioSpecificationChild;
	  protected internal static ChildElementCollection<IoBinding> ioBindingCollection;

	  public static void registerType(ModelBuilder bpmnModelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = bpmnModelBuilder.defineType(typeof(CallableElement), BPMN_ELEMENT_CALLABLE_ELEMENT).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		supportedInterfaceRefCollection = sequenceBuilder.elementCollection(typeof(SupportedInterfaceRef)).qNameElementReferenceCollection(typeof(Interface)).build();

		ioSpecificationChild = sequenceBuilder.element(typeof(IoSpecification)).build();

		ioBindingCollection = sequenceBuilder.elementCollection(typeof(IoBinding)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ModelElementInstance>
	  {
		  public ModelElementInstance newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CallableElementImpl(instanceContext);
		  }
	  }

	  public CallableElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual ICollection<Interface> SupportedInterfaces
	  {
		  get
		  {
			return supportedInterfaceRefCollection.getReferenceTargetElements(this);
		  }
	  }

	  public virtual IoSpecification IoSpecification
	  {
		  get
		  {
			return ioSpecificationChild.getChild(this);
		  }
		  set
		  {
			ioSpecificationChild.setChild(this, value);
		  }
	  }


	  public virtual ICollection<IoBinding> IoBindings
	  {
		  get
		  {
			return ioBindingCollection.get(this);
		  }
	  }

	}

}