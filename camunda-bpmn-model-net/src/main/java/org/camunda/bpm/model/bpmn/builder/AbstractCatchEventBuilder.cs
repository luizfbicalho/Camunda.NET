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
	using BoundaryEvent = org.camunda.bpm.model.bpmn.instance.BoundaryEvent;
	using CatchEvent = org.camunda.bpm.model.bpmn.instance.CatchEvent;
	using CompensateEventDefinition = org.camunda.bpm.model.bpmn.instance.CompensateEventDefinition;
	using ConditionalEventDefinition = org.camunda.bpm.model.bpmn.instance.ConditionalEventDefinition;
	using EventDefinition = org.camunda.bpm.model.bpmn.instance.EventDefinition;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;
	using SignalEventDefinition = org.camunda.bpm.model.bpmn.instance.SignalEventDefinition;
	using TimeCycle = org.camunda.bpm.model.bpmn.instance.TimeCycle;
	using TimeDate = org.camunda.bpm.model.bpmn.instance.TimeDate;
	using TimeDuration = org.camunda.bpm.model.bpmn.instance.TimeDuration;
	using TimerEventDefinition = org.camunda.bpm.model.bpmn.instance.TimerEventDefinition;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractCatchEventBuilder<B, E> : AbstractEventBuilder<B, E> where B : AbstractCatchEventBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.CatchEvent
	{

	  protected internal AbstractCatchEventBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the event to be parallel multiple
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B parallelMultiple()
	  {
		element.ParallelMultiple;
		return myself;
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
	  /// Sets an event definition for the timer with a time date.
	  /// </summary>
	  /// <param name="timerDate"> the time date of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual B timerWithDate(string timerDate)
	  {
		TimeDate timeDate = createInstance(typeof(TimeDate));
		timeDate.TextContent = timerDate;

		TimerEventDefinition timerEventDefinition = createInstance(typeof(TimerEventDefinition));
		timerEventDefinition.TimeDate = timeDate;

		element.EventDefinitions.add(timerEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an event definition for the timer with a time duration.
	  /// </summary>
	  /// <param name="timerDuration"> the time duration of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual B timerWithDuration(string timerDuration)
	  {
		TimeDuration timeDuration = createInstance(typeof(TimeDuration));
		timeDuration.TextContent = timerDuration;

		TimerEventDefinition timerEventDefinition = createInstance(typeof(TimerEventDefinition));
		timerEventDefinition.TimeDuration = timeDuration;

		element.EventDefinitions.add(timerEventDefinition);

		return myself;
	  }

	  /// <summary>
	  /// Sets an event definition for the timer with a time cycle.
	  /// </summary>
	  /// <param name="timerCycle"> the time cycle of the timer </param>
	  /// <returns> the builder object </returns>
	  public virtual B timerWithCycle(string timerCycle)
	  {
		TimeCycle timeCycle = createInstance(typeof(TimeCycle));
		timeCycle.TextContent = timerCycle;

		TimerEventDefinition timerEventDefinition = createInstance(typeof(TimerEventDefinition));
		timerEventDefinition.TimeCycle = timeCycle;

		element.EventDefinitions.add(timerEventDefinition);

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

	  public virtual ConditionalEventDefinitionBuilder conditionalEventDefinition()
	  {
		return conditionalEventDefinition(null);
	  }

	  public virtual ConditionalEventDefinitionBuilder conditionalEventDefinition(string id)
	  {
		ConditionalEventDefinition eventDefinition = createInstance(typeof(ConditionalEventDefinition));
		if (!string.ReferenceEquals(id, null))
		{
		  eventDefinition.Id = id;
		}

		element.EventDefinitions.add(eventDefinition);
		return new ConditionalEventDefinitionBuilder(modelInstance, eventDefinition);
	  }

	  public virtual B condition(string condition)
	  {
		conditionalEventDefinition().condition(condition);
		return myself;
	  }

	}

}