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
namespace org.camunda.bpm.engine.impl.runtime
{

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ConditionalEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.ConditionalEventDefinition;
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public class DefaultConditionHandler : ConditionHandler
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public virtual IList<ConditionHandlerResult> evaluateStartCondition(CommandContext commandContext, ConditionSet conditionSet)
	  {
		if (string.ReferenceEquals(conditionSet.ProcessDefinitionId, null))
		{
		  return evaluateConditionStartByEventSubscription(commandContext, conditionSet);
		}
		else
		{
		  return evaluateConditionStartByProcessDefinitionId(commandContext, conditionSet, conditionSet.ProcessDefinitionId);
		}
	  }

	  protected internal virtual IList<ConditionHandlerResult> evaluateConditionStartByEventSubscription(CommandContext commandContext, ConditionSet conditionSet)
	  {
		IList<EventSubscriptionEntity> subscriptions = findConditionalStartEventSubscriptions(commandContext, conditionSet);
		if (subscriptions.Count == 0)
		{
		  throw LOG.exceptionWhenEvaluatingConditionalStartEvent();
		}
		IList<ConditionHandlerResult> results = new List<ConditionHandlerResult>();
		foreach (EventSubscriptionEntity subscription in subscriptions)
		{

		  ProcessDefinitionEntity processDefinition = subscription.ProcessDefinition;
		  if (!processDefinition.Suspended)
		  {

			ActivityImpl activity = subscription.Activity;

			if (evaluateCondition(conditionSet, activity))
			{
			  results.Add(new ConditionHandlerResult(processDefinition, activity));
			}

		  }
		}

		return results;
	  }

	  protected internal virtual IList<EventSubscriptionEntity> findConditionalStartEventSubscriptions(CommandContext commandContext, ConditionSet conditionSet)
	  {
		EventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;

		if (conditionSet.isTenantIdSet)
		{
		  return eventSubscriptionManager.findConditionalStartEventSubscriptionByTenantId(conditionSet.TenantId);
		}
		else
		{
		  return eventSubscriptionManager.findConditionalStartEventSubscription();
		}
	  }

	  protected internal virtual IList<ConditionHandlerResult> evaluateConditionStartByProcessDefinitionId(CommandContext commandContext, ConditionSet conditionSet, string processDefinitionId)
	  {
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);

		IList<ConditionHandlerResult> results = new List<ConditionHandlerResult>();

		if (processDefinition != null && !processDefinition.Suspended)
		{
		  IList<ActivityImpl> activities = findConditionalStartEventActivities(processDefinition);
		  if (activities.Count == 0)
		  {
			throw LOG.exceptionWhenEvaluatingConditionalStartEventByProcessDefinition(processDefinitionId);
		  }
		  foreach (ActivityImpl activity in activities)
		  {
			if (evaluateCondition(conditionSet, activity))
			{
			  results.Add(new ConditionHandlerResult(processDefinition, activity));
			}
		  }
		}
		return results;
	  }

	  protected internal virtual IList<ActivityImpl> findConditionalStartEventActivities(ProcessDefinitionEntity processDefinition)
	  {
		IList<ActivityImpl> activities = new List<ActivityImpl>();
		foreach (EventSubscriptionDeclaration declaration in ConditionalEventDefinition.getDeclarationsForScope(processDefinition).Values)
		{
		  if (isConditionStartEvent(declaration))
		  {
			activities.Add(((ConditionalEventDefinition) declaration).ConditionalActivity);
		  }
		}
		return activities;
	  }

	  protected internal virtual bool isConditionStartEvent(EventSubscriptionDeclaration declaration)
	  {
		return EventType.CONDITONAL.name().Equals(declaration.EventType) && declaration.StartEvent;
	  }

	  protected internal virtual bool evaluateCondition(ConditionSet conditionSet, ActivityImpl activity)
	  {
		ExecutionEntity temporaryExecution = new ExecutionEntity();
		if (conditionSet.Variables != null)
		{
		  temporaryExecution.initializeVariableStore(conditionSet.Variables);
		}
		temporaryExecution.setProcessDefinition(activity.ProcessDefinition);

		ConditionalEventDefinition conditionalEventDefinition = activity.Properties.get(BpmnProperties.CONDITIONAL_EVENT_DEFINITION);
		if (string.ReferenceEquals(conditionalEventDefinition.VariableName, null) || conditionSet.Variables.containsKey(conditionalEventDefinition.VariableName))
		{
		  return conditionalEventDefinition.tryEvaluate(temporaryExecution);
		}
		else
		{
		  return false;
		}
	  }

	}

}