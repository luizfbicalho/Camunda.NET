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
	using ReceiveTaskBuilder = org.camunda.bpm.model.bpmn.builder.ReceiveTaskBuilder;
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
	using ReceiveTask = org.camunda.bpm.model.bpmn.instance.ReceiveTask;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN receiveTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ReceiveTaskImpl : TaskImpl, ReceiveTask
	{

	  protected internal static Attribute<string> implementationAttribute;
	  protected internal static Attribute<bool> instantiateAttribute;
	  protected internal static AttributeReference<Message> messageRefAttribute;
	  protected internal static AttributeReference<Operation> operationRefAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ReceiveTask), BPMN_ELEMENT_RECEIVE_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Task)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		implementationAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPLEMENTATION).defaultValue("##WebService").build();

		instantiateAttribute = typeBuilder.booleanAttribute(BPMN_ATTRIBUTE_INSTANTIATE).defaultValue(false).build();

		messageRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_MESSAGE_REF).qNameAttributeReference(typeof(Message)).build();

		operationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OPERATION_REF).qNameAttributeReference(typeof(Operation)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ReceiveTask>
	  {
		  public ReceiveTask newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ReceiveTaskImpl(instanceContext);
		  }
	  }

	  public ReceiveTaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override ReceiveTaskBuilder builder()
	  {
		return new ReceiveTaskBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual string Implementation
	  {
		  get
		  {
			return implementationAttribute.getValue(this);
		  }
		  set
		  {
			implementationAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool instantiate()
	  {
		return instantiateAttribute.getValue(this);
	  }

	  public virtual bool Instantiate
	  {
		  set
		  {
			instantiateAttribute.setValue(this, value);
		  }
	  }

	  public virtual Message Message
	  {
		  get
		  {
			return messageRefAttribute.getReferenceTargetElement(this);
		  }
		  set
		  {
			messageRefAttribute.setReferenceTargetElement(this, value);
		  }
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

	}

}