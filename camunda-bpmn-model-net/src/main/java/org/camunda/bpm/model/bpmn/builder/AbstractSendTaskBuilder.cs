using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using Message = org.camunda.bpm.model.bpmn.instance.Message;
	using Operation = org.camunda.bpm.model.bpmn.instance.Operation;
	using SendTask = org.camunda.bpm.model.bpmn.instance.SendTask;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractSendTaskBuilder<B> : AbstractTaskBuilder<B, SendTask> where B : AbstractSendTaskBuilder<B>
	{

	  protected internal AbstractSendTaskBuilder(BpmnModelInstance modelInstance, SendTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the send task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B implementation(string implementation)
	  {
		element.Implementation = implementation;
		return myself;
	  }

	  /// <summary>
	  /// Sets the message of the send task. </summary>
	  /// <param name="message">  the message to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B message(Message message)
	  {
		element.Message = message;
		return myself;
	  }

	  /// <summary>
	  /// Sets the message with the given message name. If already a message
	  /// with this name exists it will be used, otherwise a new message is created.
	  /// </summary>
	  /// <param name="messageName"> the name of the message </param>
	  /// <returns> the builder object </returns>
	  public virtual B message(string messageName)
	  {
		Message message = findMessageForName(messageName);
		return message(message);
	  }

	  /// <summary>
	  /// Sets the operation of the send task.
	  /// </summary>
	  /// <param name="operation">  the operation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B operation(Operation operation)
	  {
		element.Operation = operation;
		return myself;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaClass(Type delegateClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return camundaClass(delegateClass.FullName);
	  }

	  /// <summary>
	  /// Sets the camunda class attribute.
	  /// </summary>
	  /// <param name="camundaClass">  the class name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaClass(string fullQualifiedClassName)
	  {
		element.CamundaClass = fullQualifiedClassName;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda delegateExpression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the delegateExpression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDelegateExpression(string camundaExpression)
	  {
		element.CamundaDelegateExpression = camundaExpression;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda expression attribute.
	  /// </summary>
	  /// <param name="camundaExpression">  the expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaExpression(string camundaExpression)
	  {
		element.CamundaExpression = camundaExpression;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda resultVariable attribute.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the name of the process variable </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute.
	  /// </summary>
	  /// <param name="camundaTopic">  the topic to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return myself;
	  }

	  /// <summary>
	  /// Set the camunda task priority attribute.
	  /// The priority is only used for service tasks which have as type value
	  /// <code>external</code>
	  /// </summary>
	  /// <param name="taskPriority"> the task priority which should used for the external tasks </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return myself;
	  }
	}

}