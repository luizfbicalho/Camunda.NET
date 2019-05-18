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
	using Interface = org.camunda.bpm.model.bpmn.instance.Interface;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;
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
	/// The BPMN interface element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class InterfaceImpl : RootElementImpl, Interface
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<string> implementationRefAttribute;
	  protected internal static ChildElementCollection<Operation> operationCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Interface), BPMN_ELEMENT_INTERFACE).namespaceUri(BPMN20_NS).extendsType(typeof(RootElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).required().build();

		implementationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPLEMENTATION_REF).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		operationCollection = sequenceBuilder.elementCollection(typeof(Operation)).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Interface>
	  {
		  public Interface newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new InterfaceImpl(instanceContext);
		  }
	  }

	  public InterfaceImpl(ModelTypeInstanceContext context) : base(context)
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


	  public virtual string ImplementationRef
	  {
		  get
		  {
			return implementationRefAttribute.getValue(this);
		  }
		  set
		  {
			implementationRefAttribute.setValue(this, value);
		  }
	  }


	  public virtual ICollection<Operation> Operations
	  {
		  get
		  {
			return operationCollection.get(this);
		  }
	  }
	}

}