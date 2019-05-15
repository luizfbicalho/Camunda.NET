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

	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Execution = org.camunda.bpm.engine.runtime.Execution;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// @author Michael Scholz
	/// </summary>
	public class DefaultCorrelationHandler : CorrelationHandler
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public virtual CorrelationHandlerResult correlateMessage(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {

		// first try to correlate to execution
		IList<CorrelationHandlerResult> correlations = correlateMessageToExecutions(commandContext, messageName, correlationSet);

		if (correlations.Count > 1)
		{
		  throw LOG.exceptionCorrelateMessageToSingleExecution(messageName, correlations.Count, correlationSet);

		}
		else if (correlations.Count == 1)
		{
		  return correlations[0];
		}

		// then try to correlate to process definition
		correlations = correlateStartMessages(commandContext, messageName, correlationSet);

		if (correlations.Count > 1)
		{
		  throw LOG.exceptionCorrelateMessageToSingleProcessDefinition(messageName, correlations.Count, correlationSet);

		}
		else if (correlations.Count == 1)
		{
		  return correlations[0];

		}
		else
		{
		  return null;
		}
	  }

	  public virtual IList<CorrelationHandlerResult> correlateMessages(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {
		IList<CorrelationHandlerResult> results = new List<CorrelationHandlerResult>();

		// first collect correlations to executions
		((IList<CorrelationHandlerResult>)results).AddRange(correlateMessageToExecutions(commandContext, messageName, correlationSet));
		// now collect correlations to process definition
		((IList<CorrelationHandlerResult>)results).AddRange(correlateStartMessages(commandContext, messageName, correlationSet));

		return results;
	  }

	  protected internal virtual IList<CorrelationHandlerResult> correlateMessageToExecutions(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {

		ExecutionQueryImpl query = new ExecutionQueryImpl();

		IDictionary<string, object> correlationKeys = correlationSet.CorrelationKeys;
		if (correlationKeys != null)
		{
		  foreach (KeyValuePair<string, object> correlationKey in correlationKeys.SetOfKeyValuePairs())
		  {
			query.processVariableValueEquals(correlationKey.Key, correlationKey.Value);
		  }
		}

		IDictionary<string, object> localCorrelationKeys = correlationSet.LocalCorrelationKeys;
		if (localCorrelationKeys != null)
		{
		  foreach (KeyValuePair<string, object> correlationKey in localCorrelationKeys.SetOfKeyValuePairs())
		  {
			query.variableValueEquals(correlationKey.Key, correlationKey.Value);
		  }
		}

		string businessKey = correlationSet.BusinessKey;
		if (!string.ReferenceEquals(businessKey, null))
		{
		  query.processInstanceBusinessKey(businessKey);
		}

		string processInstanceId = correlationSet.ProcessInstanceId;
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}

		if (!string.ReferenceEquals(messageName, null))
		{
		  query.messageEventSubscriptionName(messageName);
		}
		else
		{
		  query.messageEventSubscription();
		}

		if (correlationSet.isTenantIdSet)
		{
		  string tenantId = correlationSet.TenantId;
		  if (!string.ReferenceEquals(tenantId, null))
		  {
			query.tenantIdIn(tenantId);
		  }
		  else
		  {
			query.withoutTenantId();
		  }
		}

		// restrict to active executions
		query.active();

		IList<Execution> matchingExecutions = query.evaluateExpressionsAndExecuteList(commandContext, null);

		IList<CorrelationHandlerResult> result = new List<CorrelationHandlerResult>(matchingExecutions.Count);

		foreach (Execution matchingExecution in matchingExecutions)
		{
		  CorrelationHandlerResult correlationResult = CorrelationHandlerResult.matchedExecution((ExecutionEntity) matchingExecution);
		  if (!commandContext.DbEntityManager.isDeleted(correlationResult.ExecutionEntity))
		  {
			result.Add(correlationResult);
		  }
		}

		return result;
	  }

	  public virtual IList<CorrelationHandlerResult> correlateStartMessages(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {
		if (string.ReferenceEquals(messageName, null))
		{
		  // ignore empty message name
		  return Collections.emptyList();
		}

		if (string.ReferenceEquals(correlationSet.ProcessDefinitionId, null))
		{
		  return correlateStartMessageByEventSubscription(commandContext, messageName, correlationSet);

		}
		else
		{
		  CorrelationHandlerResult correlationResult = correlateStartMessageByProcessDefinitionId(commandContext, messageName, correlationSet.ProcessDefinitionId);
		  if (correlationResult != null)
		  {
			return Collections.singletonList(correlationResult);
		  }
		  else
		  {
			return Collections.emptyList();
		  }
		}
	  }

	  protected internal virtual IList<CorrelationHandlerResult> correlateStartMessageByEventSubscription(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {
		IList<CorrelationHandlerResult> results = new List<CorrelationHandlerResult>();
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

		IList<EventSubscriptionEntity> messageEventSubscriptions = findMessageStartEventSubscriptions(commandContext, messageName, correlationSet);
		foreach (EventSubscriptionEntity messageEventSubscription in messageEventSubscriptions)
		{

		  if (!string.ReferenceEquals(messageEventSubscription.Configuration, null))
		  {
			string processDefinitionId = messageEventSubscription.Configuration;
			ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
			// only an active process definition will be returned
			if (processDefinition != null && !processDefinition.Suspended)
			{
			  CorrelationHandlerResult result = CorrelationHandlerResult.matchedProcessDefinition(processDefinition, messageEventSubscription.ActivityId);
			  results.Add(result);

			}
			else
			{
			  LOG.couldNotFindProcessDefinitionForEventSubscription(messageEventSubscription, processDefinitionId);
			}
		  }
		}
		return results;
	  }

	  protected internal virtual IList<EventSubscriptionEntity> findMessageStartEventSubscriptions(CommandContext commandContext, string messageName, CorrelationSet correlationSet)
	  {
		EventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;

		if (correlationSet.isTenantIdSet)
		{
		  EventSubscriptionEntity eventSubscription = eventSubscriptionManager.findMessageStartEventSubscriptionByNameAndTenantId(messageName, correlationSet.TenantId);
		  if (eventSubscription != null)
		  {
			return Collections.singletonList(eventSubscription);
		  }
		  else
		  {
			return Collections.emptyList();
		  }

		}
		else
		{
		  return eventSubscriptionManager.findMessageStartEventSubscriptionByName(messageName);
		}
	  }

	  protected internal virtual CorrelationHandlerResult correlateStartMessageByProcessDefinitionId(CommandContext commandContext, string messageName, string processDefinitionId)
	  {
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		// only an active process definition will be returned
		if (processDefinition != null && !processDefinition.Suspended)
		{

		  string startActivityId = findStartActivityIdByMessage(processDefinition, messageName);
		  if (!string.ReferenceEquals(startActivityId, null))
		  {
			return CorrelationHandlerResult.matchedProcessDefinition(processDefinition, startActivityId);
		  }
		}
		return null;
	  }

	  protected internal virtual string findStartActivityIdByMessage(ProcessDefinitionEntity processDefinition, string messageName)
	  {
		foreach (EventSubscriptionDeclaration declaration in EventSubscriptionDeclaration.getDeclarationsForScope(processDefinition).Values)
		{
		  if (isMessageStartEventWithName(declaration, messageName))
		  {
			return declaration.ActivityId;
		  }
		}
		return null;
	  }

	  protected internal virtual bool isMessageStartEventWithName(EventSubscriptionDeclaration declaration, string messageName)
	  {
		return EventType.MESSAGE.name().Equals(declaration.EventType) && declaration.StartEvent && messageName.Equals(declaration.UnresolvedEventName);
	  }

	}

}