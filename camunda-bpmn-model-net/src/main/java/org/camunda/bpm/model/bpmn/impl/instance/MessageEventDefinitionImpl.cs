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
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;
	using AttributeReference = org.camunda.bpm.model.xml.type.reference.AttributeReference;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;

	/// 
	/// <summary>
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class MessageEventDefinitionImpl : EventDefinitionImpl, MessageEventDefinition
	{

	  protected internal static AttributeReference<Message> messageRefAttribute;
	  protected internal static ElementReference<Operation, OperationRef> operationRefChild;

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
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(MessageEventDefinition), BPMN_ELEMENT_MESSAGE_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		messageRefAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_MESSAGE_REF).qNameAttributeReference(typeof(Message)).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		operationRefChild = sequenceBuilder.element(typeof(OperationRef)).qNameElementReference(typeof(Operation)).build();

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

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder.ModelTypeInstanceProvider<MessageEventDefinition>
	  {
		  public MessageEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new MessageEventDefinitionImpl(instanceContext);
		  }
	  }

	  public MessageEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
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
			return operationRefChild.getReferenceTargetElement(this);
		  }
		  set
		  {
			operationRefChild.setReferenceTargetElement(this, value);
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