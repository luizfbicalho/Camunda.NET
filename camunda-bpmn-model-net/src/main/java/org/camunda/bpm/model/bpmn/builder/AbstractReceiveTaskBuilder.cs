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
	using ReceiveTask = org.camunda.bpm.model.bpmn.instance.ReceiveTask;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractReceiveTaskBuilder<B> : AbstractTaskBuilder<B, ReceiveTask> where B : AbstractReceiveTaskBuilder<B>
	{

	  protected internal AbstractReceiveTaskBuilder(BpmnModelInstance modelInstance, ReceiveTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the receive task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B implementation(string implementation)
	  {
		element.Implementation = implementation;
		return myself;
	  }

	  /// <summary>
	  /// Sets the receive task instantiate attribute to true.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B instantiate()
	  {
		element.Instantiate = true;
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

	}

}