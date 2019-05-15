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
namespace org.camunda.bpm.engine.impl.@event
{

	using EventSubProcessStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// @author Christopher Zell
	/// </summary>
	public class EventHandlerImpl : EventHandler
	{

	  private readonly EventType eventType;

	  public EventHandlerImpl(EventType eventType)
	  {
		this.eventType = eventType;
	  }

	  public virtual void handleIntermediateEvent(EventSubscriptionEntity eventSubscription, object payload, object localPayload, CommandContext commandContext)
	  {

		PvmExecutionImpl execution = eventSubscription.Execution;
		ActivityImpl activity = eventSubscription.Activity;

		ensureNotNull("Error while sending signal for event subscription '" + eventSubscription.Id + "': " + "no activity associated with event subscription", "activity", activity);

		if (payload is System.Collections.IDictionary)
		{
		  execution.Variables = (IDictionary<string, object>)payload;
		}

		if (localPayload is System.Collections.IDictionary)
		{
		  execution.VariablesLocal = (IDictionary<string, object>) localPayload;
		}

		if (activity.Equals(execution.getActivity()))
		{
		  execution.signal("signal", null);
		}
		else
		{
		  // hack around the fact that the start event is referenced by event subscriptions for event subprocesses
		  // and not the subprocess itself
		  if (activity.ActivityBehavior is EventSubProcessStartEventActivityBehavior)
		  {
			activity = (ActivityImpl) activity.FlowScope;
		  }

		  execution.executeEventHandlerActivity(activity);
		}
	  }

	  public virtual void handleEvent(EventSubscriptionEntity eventSubscription, object payload, object localPayload, string businessKey, CommandContext commandContext)
	  {
		handleIntermediateEvent(eventSubscription, payload, localPayload, commandContext);
	  }

	  public virtual string EventHandlerType
	  {
		  get
		  {
			return eventType.name();
		  }
	  }
	}

}