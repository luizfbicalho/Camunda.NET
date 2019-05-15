using System;
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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using CallableElement = org.camunda.bpm.engine.impl.core.model.CallableElement;
	using Expression = org.camunda.bpm.engine.impl.el.Expression;
	using StartProcessVariableScope = org.camunda.bpm.engine.impl.el.StartProcessVariableScope;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscriptionJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.EventSubscriptionJobDeclaration;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// @author Danny Gräf
	/// </summary>
	[Serializable]
	public class EventSubscriptionDeclaration
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly EventType eventType;
	  protected internal readonly Expression eventName;
	  protected internal readonly CallableElement eventPayload;

	  protected internal bool async;
	  protected internal string activityId = null;
	  protected internal string eventScopeActivityId = null;
	  protected internal bool isStartEvent;

	  protected internal EventSubscriptionJobDeclaration jobDeclaration = null;

	  public EventSubscriptionDeclaration(Expression eventExpression, EventType eventType)
	  {
		this.eventName = eventExpression;
		this.eventType = eventType;
		this.eventPayload = null;
	  }

	  public EventSubscriptionDeclaration(Expression eventExpression, EventType eventType, CallableElement eventPayload)
	  {
		this.eventType = eventType;
		this.eventName = eventExpression;
		this.eventPayload = eventPayload;
	  }

	  public static IDictionary<string, EventSubscriptionDeclaration> getDeclarationsForScope(PvmScope scope)
	  {
		if (scope == null)
		{
		  return Collections.emptyMap();
		}

		return scope.Properties.get(BpmnProperties.EVENT_SUBSCRIPTION_DECLARATIONS);
	  }

	  /// <summary>
	  /// Returns the name of the event without evaluating the possible expression that it might contain.
	  /// </summary>
	  public virtual string UnresolvedEventName
	  {
		  get
		  {
			  return eventName.ExpressionText;
		  }
	  }

	  public virtual bool hasEventName()
	  {
		return !(eventName == null || "".Equals(UnresolvedEventName.Trim(), StringComparison.OrdinalIgnoreCase));
	  }

	  public virtual bool EventNameLiteralText
	  {
		  get
		  {
			return eventName.LiteralText;
		  }
	  }

	  public virtual bool Async
	  {
		  get
		  {
			return async;
		  }
		  set
		  {
			this.async = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string EventScopeActivityId
	  {
		  get
		  {
			return eventScopeActivityId;
		  }
		  set
		  {
			this.eventScopeActivityId = value;
		  }
	  }


	  public virtual bool StartEvent
	  {
		  get
		  {
			return isStartEvent;
		  }
		  set
		  {
			this.isStartEvent = value;
		  }
	  }


	  public virtual string EventType
	  {
		  get
		  {
			return eventType.name();
		  }
	  }

	  public virtual CallableElement EventPayload
	  {
		  get
		  {
			return eventPayload;
		  }
	  }

	  public virtual EventSubscriptionJobDeclaration JobDeclaration
	  {
		  set
		  {
			this.jobDeclaration = value;
		  }
	  }

	  public virtual EventSubscriptionEntity createSubscriptionForStartEvent(ProcessDefinitionEntity processDefinition)
	  {
		EventSubscriptionEntity eventSubscriptionEntity = new EventSubscriptionEntity(eventType);

		VariableScope scopeForExpression = StartProcessVariableScope.SharedInstance;
		string eventName = resolveExpressionOfEventName(scopeForExpression);
		eventSubscriptionEntity.EventName = eventName;
		eventSubscriptionEntity.ActivityId = activityId;
		eventSubscriptionEntity.Configuration = processDefinition.Id;
		eventSubscriptionEntity.TenantId = processDefinition.TenantId;

		return eventSubscriptionEntity;
	  }

	  /// <summary>
	  /// Creates and inserts a subscription entity depending on the message type of this declaration.
	  /// </summary>
	  public virtual EventSubscriptionEntity createSubscriptionForExecution(ExecutionEntity execution)
	  {
		EventSubscriptionEntity eventSubscriptionEntity = new EventSubscriptionEntity(execution, eventType);

		string eventName = resolveExpressionOfEventName(execution);
		eventSubscriptionEntity.EventName = eventName;
		if (!string.ReferenceEquals(activityId, null))
		{
		  ActivityImpl activity = execution.getProcessDefinition().findActivity(activityId);
		  eventSubscriptionEntity.Activity = activity;
		}

		eventSubscriptionEntity.insert();
		LegacyBehavior.removeLegacySubscriptionOnParent(execution, eventSubscriptionEntity);

		return eventSubscriptionEntity;
	  }

	  /// <summary>
	  /// Resolves the event name within the given scope.
	  /// </summary>
	  public virtual string resolveExpressionOfEventName(VariableScope scope)
	  {
		if (ExpressionAvailable)
		{
		  return (string) eventName.getValue(scope);
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual bool ExpressionAvailable
	  {
		  get
		  {
			return eventName != null;
		  }
	  }

	  public virtual void updateSubscription(EventSubscriptionEntity eventSubscription)
	  {
		string eventName = resolveExpressionOfEventName(eventSubscription.Execution);
		eventSubscription.EventName = eventName;
		eventSubscription.ActivityId = activityId;
	  }

	}

}