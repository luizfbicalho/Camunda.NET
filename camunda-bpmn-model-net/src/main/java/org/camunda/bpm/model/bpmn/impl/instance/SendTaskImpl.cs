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
	using SendTaskBuilder = org.camunda.bpm.model.bpmn.builder.SendTaskBuilder;
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
	using SendTask = org.camunda.bpm.model.bpmn.instance.SendTask;
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
	/// The BPMN sendTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class SendTaskImpl : TaskImpl, SendTask
	{

	  protected internal static Attribute<string> implementationAttribute;
	  protected internal static AttributeReference<Message> messageRefAttribute;
	  protected internal static AttributeReference<Operation> operationRefAttribute;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<string> camundaClassAttribute;
	  protected internal static Attribute<string> camundaDelegateExpressionAttribute;
	  protected internal static Attribute<string> camundaExpressionAttribute;
	  protected internal static Attribute<string> camundaResultVariableAttribute;
	  protected internal static Attribute<string> camundaTopicAttribute;
	  protected internal static Attribute<string> camundaTypeAttribute;
	  protected internal static Attribute<string> camundaTaskPriorityAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(SendTask), BPMN_ELEMENT_SEND_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Task)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		implementationAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_IMPLEMENTATION).defaultValue("##WebService").build();

		messageRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_MESSAGE_REF).qNameAttributeReference(typeof(Message)).build();

		operationRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_OPERATION_REF).qNameAttributeReference(typeof(Operation)).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaClassAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_CLASS).@namespace(CAMUNDA_NS).build();

		camundaDelegateExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaExpressionAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_EXPRESSION).@namespace(CAMUNDA_NS).build();

		camundaResultVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESULT_VARIABLE).@namespace(CAMUNDA_NS).build();

		camundaTopicAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TOPIC).@namespace(CAMUNDA_NS).build();

		camundaTypeAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TYPE).@namespace(CAMUNDA_NS).build();

		camundaTaskPriorityAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_TASK_PRIORITY).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<SendTask>
	  {
		  public SendTask newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new SendTaskImpl(instanceContext);
		  }
	  }

	  public SendTaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override SendTaskBuilder builder()
	  {
		return new SendTaskBuilder((BpmnModelInstance) modelInstance, this);
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


	  /// <summary>
	  /// camunda extensions </summary>

	  public virtual string CamundaClass
	  {
		  get
		  {
			return camundaClassAttribute.getValue(this);
		  }
		  set
		  {
			camundaClassAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaDelegateExpression
	  {
		  get
		  {
			return camundaDelegateExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaDelegateExpressionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaExpression
	  {
		  get
		  {
			return camundaExpressionAttribute.getValue(this);
		  }
		  set
		  {
			camundaExpressionAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaResultVariable
	  {
		  get
		  {
			return camundaResultVariableAttribute.getValue(this);
		  }
		  set
		  {
			camundaResultVariableAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTopic
	  {
		  get
		  {
			return camundaTopicAttribute.getValue(this);
		  }
		  set
		  {
			camundaTopicAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaType
	  {
		  get
		  {
			return camundaTypeAttribute.getValue(this);
		  }
		  set
		  {
			camundaTypeAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaTaskPriority
	  {
		  get
		  {
			return camundaTaskPriorityAttribute.getValue(this);
		  }
		  set
		  {
			camundaTaskPriorityAttribute.setValue(this, value);
		  }
	  }

	}

}