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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionJobConfiguration = org.camunda.bpm.engine.impl.jobexecutor.ProcessEventJobHandler.EventSubscriptionJobConfiguration;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessEventJobHandler : JobHandler<EventSubscriptionJobConfiguration>
	{

	  public const string TYPE = "event";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(EventSubscriptionJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		// lookup subscription:
		string eventSubscriptionId = configuration.EventSubscriptionId;
		EventSubscriptionEntity eventSubscription = commandContext.EventSubscriptionManager.findEventSubscriptionById(eventSubscriptionId);

		// if event subscription is null, ignore
		if (eventSubscription != null)
		{
		  eventSubscription.eventReceived(null, false);
		}

	  }

	  public virtual EventSubscriptionJobConfiguration newConfiguration(string canonicalString)
	  {
		return new EventSubscriptionJobConfiguration(canonicalString);
	  }

	  public class EventSubscriptionJobConfiguration : JobHandlerConfiguration
	  {

		protected internal string eventSubscriptionId;

		public EventSubscriptionJobConfiguration(string eventSubscriptionId)
		{
		  this.eventSubscriptionId = eventSubscriptionId;
		}

		public virtual string EventSubscriptionId
		{
			get
			{
			  return eventSubscriptionId;
			}
		}

		public virtual string toCanonicalString()
		{
		  return eventSubscriptionId;
		}

	  }

	  public virtual void onDelete(EventSubscriptionJobConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}