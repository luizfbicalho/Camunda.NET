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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class SignalEventReceivedCmd : Command<Void>
	{

	  protected internal static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal readonly SignalEventReceivedBuilderImpl builder;

	  public SignalEventReceivedCmd(SignalEventReceivedBuilderImpl builder)
	  {
		this.builder = builder;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual Void execute(CommandContext commandContext)
	  {

		string signalName = builder.SignalName;
		string executionId = builder.ExecutionId;

		if (string.ReferenceEquals(executionId, null))
		{
		  sendSignal(commandContext, signalName);

		}
		else
		{
		  sendSignalToExecution(commandContext, signalName, executionId);
		}
		return null;
	  }

	  protected internal virtual void sendSignal(CommandContext commandContext, string signalName)
	  {

		IList<EventSubscriptionEntity> signalEventSubscriptions = findSignalEventSubscriptions(commandContext, signalName);

		IList<EventSubscriptionEntity> catchSignalEventSubscription = filterIntermediateSubscriptions(signalEventSubscriptions);
		IList<EventSubscriptionEntity> startSignalEventSubscriptions = filterStartSubscriptions(signalEventSubscriptions);
		IDictionary<string, ProcessDefinitionEntity> processDefinitions = getProcessDefinitionsOfSubscriptions(startSignalEventSubscriptions);

		checkAuthorizationOfCatchSignals(commandContext, catchSignalEventSubscription);
		checkAuthorizationOfStartSignals(commandContext, startSignalEventSubscriptions, processDefinitions);

		notifyExecutions(catchSignalEventSubscription);
		startProcessInstances(startSignalEventSubscriptions, processDefinitions);
	  }

	  protected internal virtual IList<EventSubscriptionEntity> findSignalEventSubscriptions(CommandContext commandContext, string signalName)
	  {
		EventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;

		if (builder.TenantIdSet)
		{
		  return eventSubscriptionManager.findSignalEventSubscriptionsByEventNameAndTenantId(signalName, builder.TenantId);

		}
		else
		{
		  return eventSubscriptionManager.findSignalEventSubscriptionsByEventName(signalName);
		}
	  }

	  protected internal virtual IDictionary<string, ProcessDefinitionEntity> getProcessDefinitionsOfSubscriptions(IList<EventSubscriptionEntity> startSignalEventSubscriptions)
	  {
		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;

		IDictionary<string, ProcessDefinitionEntity> processDefinitions = new Dictionary<string, ProcessDefinitionEntity>();

		foreach (EventSubscriptionEntity eventSubscription in startSignalEventSubscriptions)
		{

		  string processDefinitionId = eventSubscription.Configuration;
		  ensureNotNull("Configuration of signal start event subscription '" + eventSubscription.Id + "' contains no process definition id.", processDefinitionId);

		  ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null && !processDefinition.Suspended)
		  {
			processDefinitions[eventSubscription.Id] = processDefinition;
		  }
		}

		return processDefinitions;
	  }

	  protected internal virtual void sendSignalToExecution(CommandContext commandContext, string signalName, string executionId)
	  {

		ExecutionManager executionManager = commandContext.ExecutionManager;
		ExecutionEntity execution = executionManager.findExecutionById(executionId);
		ensureNotNull("Cannot find execution with id '" + executionId + "'", "execution", execution);

		EventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;
		IList<EventSubscriptionEntity> signalEvents = eventSubscriptionManager.findSignalEventSubscriptionsByNameAndExecution(signalName, executionId);
		ensureNotEmpty("Execution '" + executionId + "' has not subscribed to a signal event with name '" + signalName + "'.", signalEvents);

		checkAuthorizationOfCatchSignals(commandContext, signalEvents);
		notifyExecutions(signalEvents);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void checkAuthorizationOfCatchSignals(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, java.util.List<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity> catchSignalEventSubscription)
	  protected internal virtual void checkAuthorizationOfCatchSignals(CommandContext commandContext, IList<EventSubscriptionEntity> catchSignalEventSubscription)
	  {
		// check authorization for each fetched signal event
		foreach (EventSubscriptionEntity @event in catchSignalEventSubscription)
		{
		  string processInstanceId = @event.ProcessInstanceId;
		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkUpdateProcessInstanceById(processInstanceId);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void checkAuthorizationOfStartSignals(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, java.util.List<org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity> startSignalEventSubscriptions, java.util.Map<String, org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity> processDefinitions)
	  private void checkAuthorizationOfStartSignals(CommandContext commandContext, IList<EventSubscriptionEntity> startSignalEventSubscriptions, IDictionary<string, ProcessDefinitionEntity> processDefinitions)
	  {
		// check authorization for process definition
		foreach (EventSubscriptionEntity signalStartEventSubscription in startSignalEventSubscriptions)
		{
		  ProcessDefinitionEntity processDefinition = processDefinitions[signalStartEventSubscription.Id];
		  if (processDefinition != null)
		  {

			foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
			{
			  checker.checkCreateProcessInstance(processDefinition);
			}

		  }
		}
	  }

	  private void notifyExecutions(IList<EventSubscriptionEntity> catchSignalEventSubscription)
	  {

		foreach (EventSubscriptionEntity signalEventSubscriptionEntity in catchSignalEventSubscription)
		{
		  if (isActiveEventSubscription(signalEventSubscriptionEntity))
		  {
			signalEventSubscriptionEntity.eventReceived(builder.getVariables(), false);
		  }
		}
	  }

	  private bool isActiveEventSubscription(EventSubscriptionEntity signalEventSubscriptionEntity)
	  {
		ExecutionEntity execution = signalEventSubscriptionEntity.Execution;
		return !execution.Ended && !execution.Canceled;
	  }

	  private void startProcessInstances(IList<EventSubscriptionEntity> startSignalEventSubscriptions, IDictionary<string, ProcessDefinitionEntity> processDefinitions)
	  {
		foreach (EventSubscriptionEntity signalStartEventSubscription in startSignalEventSubscriptions)
		{
		  ProcessDefinitionEntity processDefinition = processDefinitions[signalStartEventSubscription.Id];
		  if (processDefinition != null)
		  {

			ActivityImpl signalStartEvent = processDefinition.findActivity(signalStartEventSubscription.ActivityId);
			PvmProcessInstance processInstance = processDefinition.createProcessInstanceForInitial(signalStartEvent);
			processInstance.start(builder.getVariables());
		  }
		}
	  }

	  protected internal virtual IList<EventSubscriptionEntity> filterIntermediateSubscriptions(IList<EventSubscriptionEntity> subscriptions)
	  {
		IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();

		foreach (EventSubscriptionEntity subscription in subscriptions)
		{
		  if (!string.ReferenceEquals(subscription.ExecutionId, null))
		  {
			result.Add(subscription);
		  }
		}

		return result;
	  }

	  protected internal virtual IList<EventSubscriptionEntity> filterStartSubscriptions(IList<EventSubscriptionEntity> subscriptions)
	  {
		IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();

		foreach (EventSubscriptionEntity subscription in subscriptions)
		{
		  if (string.ReferenceEquals(subscription.ExecutionId, null))
		  {
			result.Add(subscription);
		  }
		}

		return result;
	  }

	}

}