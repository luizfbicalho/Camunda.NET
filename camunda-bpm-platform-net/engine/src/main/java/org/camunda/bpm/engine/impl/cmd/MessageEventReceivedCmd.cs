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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNumberOfElements;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class MessageEventReceivedCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string executionId;
	  protected internal readonly IDictionary<string, object> processVariables;
	  protected internal readonly IDictionary<string, object> processVariablesLocal;
	  protected internal readonly string messageName;
	  protected internal bool exclusive = false;

	  public MessageEventReceivedCmd(string messageName, string executionId, IDictionary<string, object> processVariables) : this(messageName, executionId, processVariables, null)
	  {
	  }

	  public MessageEventReceivedCmd(string messageName, string executionId, IDictionary<string, object> processVariables, IDictionary<string, object> processVariablesLocal)
	  {
		this.executionId = executionId;
		this.messageName = messageName;
		this.processVariables = processVariables;
		this.processVariablesLocal = processVariablesLocal;
	  }

	  public MessageEventReceivedCmd(string messageName, string executionId, IDictionary<string, object> processVariables, IDictionary<string, object> processVariablesLocal, bool exclusive) : this(messageName, executionId, processVariables, processVariablesLocal)
	  {
		this.exclusive = exclusive;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("executionId", executionId);

		EventSubscriptionManager eventSubscriptionManager = commandContext.EventSubscriptionManager;
		IList<EventSubscriptionEntity> eventSubscriptions = null;
		if (!string.ReferenceEquals(messageName, null))
		{
		  eventSubscriptions = eventSubscriptionManager.findEventSubscriptionsByNameAndExecution(EventType.MESSAGE.name(), messageName, executionId, exclusive);
		}
		else
		{
		  eventSubscriptions = eventSubscriptionManager.findEventSubscriptionsByExecutionAndType(executionId, EventType.MESSAGE.name(), exclusive);
		}

		ensureNotEmpty("Execution with id '" + executionId + "' does not have a subscription to a message event with name '" + messageName + "'", "eventSubscriptions", eventSubscriptions);
		ensureNumberOfElements("More than one matching message subscription found for execution " + executionId, "eventSubscriptions", eventSubscriptions, 1);

		// there can be only one:
		EventSubscriptionEntity eventSubscriptionEntity = eventSubscriptions[0];

		// check authorization
		string processInstanceId = eventSubscriptionEntity.ProcessInstanceId;
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstanceById(processInstanceId);
		}

		eventSubscriptionEntity.eventReceived(processVariables, processVariablesLocal, null, false);

		return null;
	  }


	}

}