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
	using CompensateEventDefinition = org.camunda.bpm.model.bpmn.instance.CompensateEventDefinition;
	using EscalationEventDefinition = org.camunda.bpm.model.bpmn.instance.EscalationEventDefinition;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;
	using SignalEventDefinition = org.camunda.bpm.model.bpmn.instance.SignalEventDefinition;
	using ThrowEvent = org.camunda.bpm.model.bpmn.instance.ThrowEvent;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractThrowEventBuilder<B, E> : AbstractEventBuilder<B, E> where B : AbstractThrowEventBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.ThrowEvent
	{

	  protected internal AbstractThrowEventBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets an event definition for the given message name. If already a message
	  /// with this name exists it will be used, otherwise a new message is created.
	  /// </summary>
	  /// <param name="messageName"> the name of the message </param>
	  /// <returns> the builder object </returns>
	  public virtual B message(string messageName)
	  {
		MessageEventDefinition messageEventDefinition = createMessageEventDefinition(messageName);
		element.EventDefinitions.add(messageEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Creates an empty message event definition with an unique id
	  /// and returns a builder for the message event definition.
	  /// </summary>
	  /// <returns> the message event definition builder object </returns>
	  public virtual MessageEventDefinitionBuilder messageEventDefinition()
	  {
		return messageEventDefinition(null);
	  }

	  /// <summary>
	  /// Creates an empty message event definition with the given id
	  /// and returns a builder for the message event definition.
	  /// </summary>
	  /// <param name="id"> the id of the message event definition </param>
	  /// <returns> the message event definition builder object </returns>
	  public virtual MessageEventDefinitionBuilder messageEventDefinition(string id)
	  {
		MessageEventDefinition messageEventDefinition = createEmptyMessageEventDefinition();
		if (!string.ReferenceEquals(id, null))
		{
		  messageEventDefinition.Id = id;
		}

		element.EventDefinitions.add(messageEventDefinition);
		return new MessageEventDefinitionBuilder(modelInstance, messageEventDefinition);
	  }

	  /// <summary>
	  /// Sets an event definition for the given signal name. If already a signal
	  /// with this name exists it will be used, otherwise a new signal is created.
	  /// </summary>
	  /// <param name="signalName"> the name of the signal </param>
	  /// <returns> the builder object </returns>
	  public virtual B signal(string signalName)
	  {
		SignalEventDefinition signalEventDefinition = createSignalEventDefinition(signalName);
		element.EventDefinitions.add(signalEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an event definition for the given Signal name. If a signal with this
	  /// name already exists it will be used, otherwise a new signal is created.
	  /// It returns a builder for the Signal Event Definition.
	  /// </summary>
	  /// <param name="signalName"> the name of the signal </param>
	  /// <returns> the signal event definition builder object </returns>
	  public virtual SignalEventDefinitionBuilder signalEventDefinition(string signalName)
	  {
		SignalEventDefinition signalEventDefinition = createSignalEventDefinition(signalName);
		element.EventDefinitions.add(signalEventDefinition);

		return new SignalEventDefinitionBuilder(modelInstance, signalEventDefinition);
	  }

	  /// <summary>
	  /// Sets an escalation definition for the given escalation code. If already an
	  /// escalation with this code exists it will be used, otherwise a new
	  /// escalation is created.
	  /// </summary>
	  /// <param name="escalationCode"> the code of the escalation </param>
	  /// <returns> the builder object </returns>
	  public virtual B escalation(string escalationCode)
	  {
		EscalationEventDefinition escalationEventDefinition = createEscalationEventDefinition(escalationCode);
		element.EventDefinitions.add(escalationEventDefinition);

		return myself;
	  }

	  public virtual CompensateEventDefinitionBuilder compensateEventDefinition()
	  {
		return compensateEventDefinition(null);
	  }

	  public virtual CompensateEventDefinitionBuilder compensateEventDefinition(string id)
	  {
		CompensateEventDefinition eventDefinition = createInstance(typeof(CompensateEventDefinition));
		if (!string.ReferenceEquals(id, null))
		{
		  eventDefinition.Id = id;
		}

		element.EventDefinitions.add(eventDefinition);
		return new CompensateEventDefinitionBuilder(modelInstance, eventDefinition);
	  }
	}

}