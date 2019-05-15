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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class EventSubscriptionQueryImpl : AbstractQuery<EventSubscriptionQuery, EventSubscription>, EventSubscriptionQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string eventSubscriptionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string eventName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string eventType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeEventSubscriptionsWithoutTenantId_Renamed = false;

	  public EventSubscriptionQueryImpl()
	  {
	  }

	  public EventSubscriptionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual EventSubscriptionQuery eventSubscriptionId(string id)
	  {
		ensureNotNull("event subscription id", id);
		this.eventSubscriptionId_Renamed = id;
		return this;
	  }

	  public virtual EventSubscriptionQuery eventName(string eventName)
	  {
		ensureNotNull("event name", eventName);
		this.eventName_Renamed = eventName;
		return this;
	  }

	  public virtual EventSubscriptionQueryImpl executionId(string executionId)
	  {
		ensureNotNull("execution id", executionId);
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual EventSubscriptionQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull("process instance id", processInstanceId);
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual EventSubscriptionQueryImpl activityId(string activityId)
	  {
		ensureNotNull("activity id", activityId);
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual EventSubscriptionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual EventSubscriptionQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual EventSubscriptionQuery includeEventSubscriptionsWithoutTenantId()
	  {
		this.includeEventSubscriptionsWithoutTenantId_Renamed = true;
		return this;
	  }

	  public virtual EventSubscriptionQueryImpl eventType(string eventType)
	  {
		ensureNotNull("event type", eventType);
		this.eventType_Renamed = eventType;
		return this;
	  }

	  public virtual EventSubscriptionQuery orderByCreated()
	  {
		return orderBy(EventSubscriptionQueryProperty_Fields.CREATED);
	  }

	  public virtual EventSubscriptionQuery orderByTenantId()
	  {
		return orderBy(EventSubscriptionQueryProperty_Fields.TENANT_ID);
	  }

	  //results //////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.EventSubscriptionManager.findEventSubscriptionCountByQueryCriteria(this);
	  }

	  public override IList<EventSubscription> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.EventSubscriptionManager.findEventSubscriptionsByQueryCriteria(this,page);
	  }

	  //getters //////////////////////////////////////////


	  public virtual string EventSubscriptionId
	  {
		  get
		  {
			return eventSubscriptionId_Renamed;
		  }
	  }
	  public virtual string EventName
	  {
		  get
		  {
			return eventName_Renamed;
		  }
	  }
	  public virtual string EventType
	  {
		  get
		  {
			return eventType_Renamed;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }

	}

}