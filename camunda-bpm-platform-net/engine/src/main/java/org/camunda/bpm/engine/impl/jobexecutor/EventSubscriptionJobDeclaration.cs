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
namespace org.camunda.bpm.engine.impl.jobexecutor
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using EventSubscriptionJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.ProcessEventJobHandler.EventSubscriptionJobConfiguration;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using EnsureUtil = org.camunda.commons.utils.EnsureUtil;

	/// <summary>
	/// <para>Describes and creates jobs for handling an event asynchronously.
	/// These jobs are created in the context of an <seealso cref="EventSubscriptionEntity"/> and are of type <seealso cref="MessageEntity"/>.</para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	[Serializable]
	public class EventSubscriptionJobDeclaration : JobDeclaration<EventSubscriptionEntity, MessageEntity>
	{

	  private const long serialVersionUID = 1L;

	  protected internal EventSubscriptionDeclaration eventSubscriptionDeclaration;

	  public EventSubscriptionJobDeclaration(EventSubscriptionDeclaration eventSubscriptionDeclaration) : base(ProcessEventJobHandler.TYPE)
	  {
		EnsureUtil.ensureNotNull("eventSubscriptionDeclaration", eventSubscriptionDeclaration);
		this.eventSubscriptionDeclaration = eventSubscriptionDeclaration;
	  }


	  protected internal virtual MessageEntity newJobInstance(EventSubscriptionEntity eventSubscription)
	  {

		MessageEntity message = new MessageEntity();

		// initialize job
		message.ActivityId = eventSubscription.ActivityId;
		message.ExecutionId = eventSubscription.ExecutionId;
		message.ProcessInstanceId = eventSubscription.ProcessInstanceId;

		ProcessDefinitionEntity processDefinition = eventSubscription.ProcessDefinition;

		if (processDefinition != null)
		{
		  message.ProcessDefinitionId = processDefinition.Id;
		  message.ProcessDefinitionKey = processDefinition.Key;
		}

		// TODO: support payload
		// if(payload != null) {
		//   message.setEventPayload(payload);
		// }

		return message;
	  }

	  public virtual string EventType
	  {
		  get
		  {
			return eventSubscriptionDeclaration.EventType;
		  }
	  }

	  public virtual string EventName
	  {
		  get
		  {
			return eventSubscriptionDeclaration.UnresolvedEventName;
		  }
	  }

	  public override string ActivityId
	  {
		  get
		  {
			return eventSubscriptionDeclaration.ActivityId;
		  }
	  }

	  protected internal virtual ExecutionEntity resolveExecution(EventSubscriptionEntity context)
	  {
		return context.Execution;
	  }

	  protected internal virtual JobHandlerConfiguration resolveJobHandlerConfiguration(EventSubscriptionEntity context)
	  {
		return new EventSubscriptionJobConfiguration(context.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static java.util.List<EventSubscriptionJobDeclaration> getDeclarationsForActivity(org.camunda.bpm.engine.impl.pvm.PvmActivity activity)
	  public static IList<EventSubscriptionJobDeclaration> getDeclarationsForActivity(PvmActivity activity)
	  {
		object result = activity.getProperty(BpmnParse.PROPERTYNAME_EVENT_SUBSCRIPTION_JOB_DECLARATION);
		if (result != null)
		{
		  return (IList<EventSubscriptionJobDeclaration>) result;
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	  /// <summary>
	  /// Assumes that an activity has at most one declaration of a certain eventType.
	  /// </summary>
	  public static EventSubscriptionJobDeclaration findDeclarationForSubscription(EventSubscriptionEntity eventSubscription)
	  {
		IList<EventSubscriptionJobDeclaration> declarations = getDeclarationsForActivity(eventSubscription.Activity);

		foreach (EventSubscriptionJobDeclaration declaration in declarations)
		{
		  if (declaration.EventType.Equals(eventSubscription.EventType))
		  {
			return declaration;
		  }
		}

		return null;
	  }


	}

}