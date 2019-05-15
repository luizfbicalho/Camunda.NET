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
namespace org.camunda.bpm.engine.impl.@event
{
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ConditionalEventBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ConditionalEventBehavior;
	using VariableEvent = org.camunda.bpm.engine.impl.core.variable.@event.VariableEvent;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ConditionalEventHandler : EventHandler
	{

	  public virtual string EventHandlerType
	  {
		  get
		  {
			return EventType.CONDITONAL.name();
		  }
	  }

	  public virtual void handleEvent(EventSubscriptionEntity eventSubscription, object payload, object localPayload, string businessKey, CommandContext commandContext)
	  {
		VariableEvent variableEvent;
		if (payload == null || payload is VariableEvent)
		{
		  variableEvent = (VariableEvent) payload;
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("Payload have to be " + typeof(VariableEvent).FullName + ", to evaluate condition.");
		}

		ActivityImpl activity = eventSubscription.Activity;
		ActivityBehavior activityBehavior = activity.ActivityBehavior;
		if (activityBehavior is ConditionalEventBehavior)
		{
		  ConditionalEventBehavior conditionalBehavior = (ConditionalEventBehavior) activityBehavior;
		  conditionalBehavior.leaveOnSatisfiedCondition(eventSubscription, variableEvent);
		}
		else
		{
		  throw new ProcessEngineException("Conditional Event has not correct behavior: " + activityBehavior);
		}
	  }

	}

}