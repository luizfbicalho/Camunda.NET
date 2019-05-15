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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using ProcessEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.ProcessEventJobHandler;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EnsureUtil = org.camunda.commons.utils.EnsureUtil;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class EventSubscriptionManager : AbstractManager
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  /// <summary>
	  /// keep track of subscriptions created in the current command </summary>
	  protected internal IList<EventSubscriptionEntity> createdSignalSubscriptions = new List<EventSubscriptionEntity>();

	  public virtual void insert(EventSubscriptionEntity persistentObject)
	  {
		base.insert(persistentObject);
		if (persistentObject.isSubscriptionForEventType(EventType.SIGNAL))
		{
		  createdSignalSubscriptions.Add(persistentObject);
		}
	  }

	  public virtual void deleteEventSubscription(EventSubscriptionEntity persistentObject)
	  {
		DbEntityManager.delete(persistentObject);
		if (persistentObject.isSubscriptionForEventType(EventType.SIGNAL))
		{
		  createdSignalSubscriptions.Remove(persistentObject);
		}

		// if the event subscription has been triggered asynchronously but not yet executed
		IList<JobEntity> asyncJobs = JobManager.findJobsByConfiguration(ProcessEventJobHandler.TYPE, persistentObject.Id, persistentObject.TenantId);
		foreach (JobEntity asyncJob in asyncJobs)
		{
		  asyncJob.delete();
		}
	  }

	  public virtual void deleteAndFlushEventSubscription(EventSubscriptionEntity persistentObject)
	  {
		deleteEventSubscription(persistentObject);
		DbEntityManager.flushEntity(persistentObject);
	  }

	  public virtual EventSubscriptionEntity findEventSubscriptionById(string id)
	  {
		return (EventSubscriptionEntity) DbEntityManager.selectOne("selectEventSubscription", id);
	  }

	  public virtual long findEventSubscriptionCountByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl)
	  {
		configureQuery(eventSubscriptionQueryImpl);
		return (long?) DbEntityManager.selectOne("selectEventSubscriptionCountByQueryCriteria", eventSubscriptionQueryImpl).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.EventSubscription> findEventSubscriptionsByQueryCriteria(org.camunda.bpm.engine.impl.EventSubscriptionQueryImpl eventSubscriptionQueryImpl, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<EventSubscription> findEventSubscriptionsByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl, Page page)
	  {
		configureQuery(eventSubscriptionQueryImpl);
		return DbEntityManager.selectList("selectEventSubscriptionByQueryCriteria", eventSubscriptionQueryImpl, page);
	  }

	  /// <summary>
	  /// Find all signal event subscriptions with the given event name for any tenant.
	  /// </summary>
	  /// <seealso cref= #findSignalEventSubscriptionsByEventNameAndTenantId(String, String) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByEventName(String eventName)
	  public virtual IList<EventSubscriptionEntity> findSignalEventSubscriptionsByEventName(string eventName)
	  {
		const string query = "selectSignalEventSubscriptionsByEventName";
		ISet<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(DbEntityManager.selectList(query, configureParameterizedQuery(eventName)));

		// add events created in this command (not visible yet in query)
		foreach (EventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (eventName.Equals(entity.EventName))
		  {
			eventSubscriptions.Add(entity);
		  }
		}
		return new List<EventSubscriptionEntity>(eventSubscriptions);
	  }

	  /// <summary>
	  /// Find all signal event subscriptions with the given event name and tenant.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByEventNameAndTenantId(String eventName, String tenantId)
	  public virtual IList<EventSubscriptionEntity> findSignalEventSubscriptionsByEventNameAndTenantId(string eventName, string tenantId)
	  {
		const string query = "selectSignalEventSubscriptionsByEventNameAndTenantId";

		IDictionary<string, object> parameter = new Dictionary<string, object>();
		parameter["eventName"] = eventName;
		parameter["tenantId"] = tenantId;
		ISet<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(DbEntityManager.selectList(query, parameter));

		// add events created in this command (not visible yet in query)
		foreach (EventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (eventName.Equals(entity.EventName) && hasTenantId(entity, tenantId))
		  {
			eventSubscriptions.Add(entity);
		  }
		}
		return new List<EventSubscriptionEntity>(eventSubscriptions);
	  }

	  /// <summary>
	  /// Find all signal event subscriptions with the given event name which belongs to the given tenant or no tenant.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(String eventName, String tenantId)
	  public virtual IList<EventSubscriptionEntity> findSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId(string eventName, string tenantId)
	  {
		const string query = "selectSignalEventSubscriptionsByEventNameAndTenantIdIncludeWithoutTenantId";

		IDictionary<string, object> parameter = new Dictionary<string, object>();
		parameter["eventName"] = eventName;
		parameter["tenantId"] = tenantId;
		ISet<EventSubscriptionEntity> eventSubscriptions = new HashSet<EventSubscriptionEntity>(DbEntityManager.selectList(query, parameter));

		// add events created in this command (not visible yet in query)
		foreach (EventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (eventName.Equals(entity.EventName) && (string.ReferenceEquals(entity.TenantId, null) || hasTenantId(entity, tenantId)))
		  {
			eventSubscriptions.Add(entity);
		  }
		}
		return new List<EventSubscriptionEntity>(eventSubscriptions);
	  }

	  protected internal virtual bool hasTenantId(EventSubscriptionEntity entity, string tenantId)
	  {
		if (string.ReferenceEquals(tenantId, null))
		{
		  return string.ReferenceEquals(entity.TenantId, null);
		}
		else
		{
		  return tenantId.Equals(entity.TenantId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByExecution(String executionId)
	  public virtual IList<EventSubscriptionEntity> findSignalEventSubscriptionsByExecution(string executionId)
	  {
		const string query = "selectSignalEventSubscriptionsByExecution";
		ISet<EventSubscriptionEntity> selectList = new HashSet<EventSubscriptionEntity>(DbEntityManager.selectList(query, executionId));

		// add events created in this command (not visible yet in query)
		foreach (EventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (executionId.Equals(entity.ExecutionId))
		  {
			selectList.Add(entity);
		  }
		}
		return new List<EventSubscriptionEntity>(selectList);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findSignalEventSubscriptionsByNameAndExecution(String name, String executionId)
	  public virtual IList<EventSubscriptionEntity> findSignalEventSubscriptionsByNameAndExecution(string name, string executionId)
	  {
		const string query = "selectSignalEventSubscriptionsByNameAndExecution";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["executionId"] = executionId;
		@params["eventName"] = name;
		ISet<EventSubscriptionEntity> selectList = new HashSet<EventSubscriptionEntity>(DbEntityManager.selectList(query, @params));

		// add events created in this command (not visible yet in query)
		foreach (EventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (executionId.Equals(entity.ExecutionId) && name.Equals(entity.EventName))
		  {
			selectList.Add(entity);
		  }
		}
		return new List<EventSubscriptionEntity>(selectList);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByExecutionAndType(String executionId, String type, boolean lockResult)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByExecutionAndType(string executionId, string type, bool lockResult)
	  {
		const string query = "selectEventSubscriptionsByExecutionAndType";
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["executionId"] = executionId;
		@params["eventType"] = type;
		@params["lockResult"] = lockResult;
		return DbEntityManager.selectList(query, @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByExecution(String executionId)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByExecution(string executionId)
	  {
		const string query = "selectEventSubscriptionsByExecution";
		return DbEntityManager.selectList(query, executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptions(String executionId, String type, String activityId)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptions(string executionId, string type, string activityId)
	  {
		const string query = "selectEventSubscriptionsByExecutionTypeAndActivity";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["executionId"] = executionId;
		@params["eventType"] = type;
		@params["activityId"] = activityId;
		return DbEntityManager.selectList(query, @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByConfiguration(String type, String configuration)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByConfiguration(string type, string configuration)
	  {
		const string query = "selectEventSubscriptionsByConfiguration";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventType"] = type;
		@params["configuration"] = configuration;
		return DbEntityManager.selectList(query, @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByNameAndTenantId(String type, String eventName, String tenantId)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByNameAndTenantId(string type, string eventName, string tenantId)
	  {
		const string query = "selectEventSubscriptionsByNameAndTenantId";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventType"] = type;
		@params["eventName"] = eventName;
		@params["tenantId"] = tenantId;
		return DbEntityManager.selectList(query, @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByNameAndExecution(String type, String eventName, String executionId, boolean lockResult)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByNameAndExecution(string type, string eventName, string executionId, bool lockResult)
	  {
		// first check cache in case entity is already loaded
		ExecutionEntity cachedExecution = DbEntityManager.getCachedEntity(typeof(ExecutionEntity), executionId);
		if (cachedExecution != null && !lockResult)
		{
		  IList<EventSubscriptionEntity> eventSubscriptions = cachedExecution.EventSubscriptions;
		  IList<EventSubscriptionEntity> result = new List<EventSubscriptionEntity>();
		  foreach (EventSubscriptionEntity subscription in eventSubscriptions)
		  {
			if (matchesSubscription(subscription, type, eventName))
			{
			  result.Add(subscription);
			}
		  }
		  return result;
		}
		else
		{
		  const string query = "selectEventSubscriptionsByNameAndExecution";
		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["eventType"] = type;
		  @params["eventName"] = eventName;
		  @params["executionId"] = executionId;
		  @params["lockResult"] = lockResult;
		  return DbEntityManager.selectList(query, @params);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByProcessInstanceId(String processInstanceId)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectEventSubscriptionsByProcessInstanceId", processInstanceId);
	  }

	  /// <returns> the message start event subscriptions with the given message name (from any tenant)
	  /// </returns>
	  /// <seealso cref= #findMessageStartEventSubscriptionByNameAndTenantId(String, String) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findMessageStartEventSubscriptionByName(String messageName)
	  public virtual IList<EventSubscriptionEntity> findMessageStartEventSubscriptionByName(string messageName)
	  {
		return DbEntityManager.selectList("selectMessageStartEventSubscriptionByName", configureParameterizedQuery(messageName));
	  }

	  /// <returns> the message start event subscription with the given message name and tenant id
	  /// </returns>
	  /// <seealso cref= #findMessageStartEventSubscriptionByName(String) </seealso>
	  public virtual EventSubscriptionEntity findMessageStartEventSubscriptionByNameAndTenantId(string messageName, string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["messageName"] = messageName;
		parameters["tenantId"] = tenantId;

		return (EventSubscriptionEntity) DbEntityManager.selectOne("selectMessageStartEventSubscriptionByNameAndTenantId", parameters);
	  }

	  /// <param name="tenantId"> </param>
	  /// <returns> the conditional start event subscriptions with the given tenant id
	  ///  </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findConditionalStartEventSubscriptionByTenantId(String tenantId)
	  public virtual IList<EventSubscriptionEntity> findConditionalStartEventSubscriptionByTenantId(string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["tenantId"] = tenantId;

		configureParameterizedQuery(parameters);
		return DbEntityManager.selectList("selectConditionalStartEventSubscriptionByTenantId", parameters);
	  }

	  /// <returns> the conditional start event subscriptions (from any tenant)
	  ///  </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findConditionalStartEventSubscription()
	  public virtual IList<EventSubscriptionEntity> findConditionalStartEventSubscription()
	  {
		ListQueryParameterObject parameter = new ListQueryParameterObject();

		configurParameterObject(parameter);
		return DbEntityManager.selectList("selectConditionalStartEventSubscription", parameter);
	  }

	  protected internal virtual void configurParameterObject(ListQueryParameterObject parameter)
	  {
		AuthorizationManager.configureConditionalEventSubscriptionQuery(parameter);
		TenantManager.configureQuery(parameter);
	  }

	  protected internal virtual void configureQuery(EventSubscriptionQueryImpl query)
	  {
		AuthorizationManager.configureEventSubscriptionQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  protected internal virtual bool matchesSubscription(EventSubscriptionEntity subscription, string type, string eventName)
	  {
		EnsureUtil.ensureNotNull("event type", type);
		string subscriptionEventName = subscription.EventName;

		return type.Equals(subscription.EventType) && ((string.ReferenceEquals(eventName, null) && string.ReferenceEquals(subscriptionEventName, null)) || (!string.ReferenceEquals(eventName, null) && eventName.Equals(subscriptionEventName)));
	  }

	}

}