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
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// <summary>
	/// The BPMN ioBinding element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class IoBindingImpl : BaseElementImpl, IoBinding
	{

	  protected internal static AttributeReference<Operation> operationRefAttribute;
	  protected internal static AttributeReference<DataInput> inputDataRefAttribute;
	  protected internal static AttributeReference<DataOutput> outputDataRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(IoBinding), BPMN_ELEMENT_IO_BINDING).namespaceUri(BPMN20_NS).extendsType(typeof(BaseElement)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		operationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OPERATION_REF).required().qNameAttributeReference(typeof(Operation)).build();

		inputDataRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_INPUT_DATA_REF).required().idAttributeReference(typeof(DataInput)).build();

		outputDataRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OUTPUT_DATA_REF).required().idAttributeReference(typeof(DataOutput)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<IoBinding>
	  {
		  public IoBinding newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new IoBindingImpl(instanceContext);
		  }
	  }

	  public IoBindingImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual Operation Operation
	  {
		  get
		  {
			return operationRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			operationRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DataInput InputData
	  {
		  get
		  {
			return inputDataRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			inputDataRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }


	  public virtual DataOutput OutputData
	  {
		  get
		  {
			return outputDataRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			outputDataRefAttribute.setReferenceTargetElement(this, value);
		  }
	  }

	}

}