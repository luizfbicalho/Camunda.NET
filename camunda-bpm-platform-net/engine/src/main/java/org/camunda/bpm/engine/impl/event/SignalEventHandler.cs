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
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class SignalEventHandler : EventHandlerImpl
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public SignalEventHandler() : base(EventType.SIGNAL)
	  {
	  }

	  protected internal virtual void handleStartEvent(EventSubscriptionEntity eventSubscription, IDictionary<string, object> payload, string businessKey, CommandContext commandContext)
	  {
		string processDefinitionId = eventSubscription.Configuration;
		ensureNotNull("Configuration of signal start event subscription '" + eventSubscription.Id + "' contains no process definition id.", processDefinitionId);

		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
		if (processDefinition == null || processDefinition.Suspended)
		{
		  // ignore event subscription
		  LOG.debugIgnoringEventSubscription(eventSubscription, processDefinitionId);
		}
		else
		{
		  ActivityImpl signalStartEvent = processDefinition.findActivity(eventSubscription.ActivityId);
		  PvmProcessInstance processInstance = processDefinition.createProcessInstance(businessKey, signalStartEvent);
		  processInstance.start(payload);
		}
	  }

	  public override void handleEvent(EventSubscriptionEntity eventSubscription, object payload, object payloadLocal, string businessKey, CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(eventSubscription.ExecutionId, null))
		{
		  handleIntermediateEvent(eventSubscription, payload, payloadLocal, commandContext);
		}
		else
		{
		  handleStartEvent(eventSubscription, (IDictionary<string, object>) payload, businessKey, commandContext);
		}
	  }

	}

}