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
	using Error = org.camunda.bpm.model.bpmn.instance.Error;
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
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
	/// The BPMN operation element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class OperationImpl : BaseElementImpl, Operation
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<string> implementationRefAttribute;
	  protected internal static ElementReference<Message, InMessageRef> inMessageRefChild;
	  protected internal static ElementReference<Message, OutMessageRef> outMessageRefChild;
	  protected internal static ElementReferenceCollection<Error, ErrorRef> errorRefCollection;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Operation), BPMN_ELEMENT_OPERATION).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_NAME).required().build();

		implementationRefAttribute = typeBuilder.stringAttribute(BPMN_ELEMENT_IMPLEMENTATION_REF).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		inMessageRefChild = sequenceBuilder.element(typeof(InMessageRef)).required().qNameElementReference(typeof(Message)).build();

		outMessageRefChild = sequenceBuilder.element(typeof(OutMessageRef)).qNameElementReference(typeof(Message)).build();

		errorRefCollection = sequenceBuilder.elementCollection(typeof(ErrorRef)).qNameElementReferenceCollection(typeof(Error)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Operation>
	  {
		  public Operation newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new OperationImpl(instanceContext);
		  }
	  }

	  public OperationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual Message InMessage
	  {
		  get
		  {
			return inMessageRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			inMessageRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual Message OutMessage
	  {
		  get
		  {
			return outMessageRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			outMessageRefChild.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual ICollection<Error> Errors
	  {
		  get
		  {
			return errorRefCollection.getReferenceTargetElements(this);
		  }
	  }
	}

}