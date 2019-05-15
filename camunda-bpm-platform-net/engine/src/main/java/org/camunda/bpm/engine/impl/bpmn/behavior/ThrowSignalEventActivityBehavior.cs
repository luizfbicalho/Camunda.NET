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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// Defines activity behavior for signal end event and intermediate throw signal event.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ThrowSignalEventActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal readonly EventSubscriptionDeclaration signalDefinition;

	  public ThrowSignalEventActivityBehavior(EventSubscriptionDeclaration signalDefinition)
	  {
		this.signalDefinition = signalDefinition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		string businessKey = signalDefinition.EventPayload.getBusinessKey(execution);
		VariableMap variableMap = signalDefinition.EventPayload.getInputVariables(execution);

		string eventName = signalDefinition.resolveExpressionOfEventName(execution);
		// trigger all event subscriptions for the signal (start and intermediate)
		IList<EventSubscriptionEntity> signalEventSubscriptions = findSignalEventSubscriptions(eventName, execution.TenantId);

		foreach (EventSubscriptionEntity signalEventSubscription in signalEventSubscriptions)
		{
		  if (isActiveEventSubscription(signalEventSubscription))
		  {
			signalEventSubscription.eventReceived(variableMap, null, businessKey, signalDefinition.Async);
		  }
		}
		leave(execution);
	  }

	  protected internal virtual IList<EventSubscriptionEntity> findSignalEventSubscriptions(string signalName, string tenantId)
	  {
		EventSubscriptionManager eventSubscriptionManager = Context.CommandContext.EventSubscriptionManager;

		if (!string.ReferenceEquals(tenantId, null))
		{
		  return eventSubscriptionManager.findSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(signalName, tenantId);

		}
		else
		{
		  // find event subscriptions without tenant id
		  return eventSubscriptionManager.findSignalEventSubscriptionsByEventNameAndTenantId(signalName, null);
		}
	  }

	  protected internal virtual bool isActiveEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
	  {
		return isStartEventSubscription(signalEventSubscriptionEntity) || isActiveIntermediateEventSubscription(signalEventSubscriptionEntity);
	  }

	  protected internal virtual bool isStartEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
	  {
		return string.ReferenceEquals(signalEventSubscriptionEntity.ExecutionId, null);
	  }

	  protected internal virtual bool isActiveIntermediateEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
	  {
		ExecutionEntity execution = signalEventSubscriptionEntity.Execution;
		return execution != null && !execution.Ended && !execution.Canceled;
	  }

	}

}