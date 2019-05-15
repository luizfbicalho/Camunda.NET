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
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;



	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Saeid Mirzaei
	/// @author Christopher Zell
	/// </summary>
	public class ProcessDefinitionManager : AbstractManager, AbstractResourceDefinitionManager<ProcessDefinitionEntity>
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  // insert ///////////////////////////////////////////////////////////

	  public virtual void insertProcessDefinition(ProcessDefinitionEntity processDefinition)
	  {
		DbEntityManager.insert(processDefinition);
		createDefaultAuthorizations(processDefinition);
	  }

	  // select ///////////////////////////////////////////////////////////

	  /// <returns> the latest version of the process definition with the given key (from any tenant)
	  /// </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a process definition with the given key
	  /// </exception>
	  /// <seealso cref= #findLatestProcessDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual ProcessDefinitionEntity findLatestProcessDefinitionByKey(string processDefinitionKey)
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<ProcessDefinitionEntity> processDefinitions = getDbEntityManager().selectList("selectLatestProcessDefinitionByKey", configureParameterizedQuery(processDefinitionKey));
		IList<ProcessDefinitionEntity> processDefinitions = DbEntityManager.selectList("selectLatestProcessDefinitionByKey", configureParameterizedQuery(processDefinitionKey));

		if (processDefinitions.Count == 0)
		{
		  return null;

		}
		else if (processDefinitions.Count == 1)
		{
		  return processDefinitions.GetEnumerator().next();

		}
		else
		{
		  throw LOG.multipleTenantsForProcessDefinitionKeyException(processDefinitionKey);
		}
	  }

	  /// <returns> the latest version of the process definition with the given key and tenant id
	  /// </returns>
	  /// <seealso cref= #findLatestProcessDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual ProcessDefinitionEntity findLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["tenantId"] = tenantId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  return (ProcessDefinitionEntity) DbEntityManager.selectOne("selectLatestProcessDefinitionByKeyWithoutTenantId", parameters);
		}
		else
		{
		  return (ProcessDefinitionEntity) DbEntityManager.selectOne("selectLatestProcessDefinitionByKeyAndTenantId", parameters);
		}
	  }

	  public virtual ProcessDefinitionEntity findLatestProcessDefinitionById(string processDefinitionId)
	  {
		return DbEntityManager.selectById(typeof(ProcessDefinitionEntity), processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByQueryCriteria(org.camunda.bpm.engine.impl.ProcessDefinitionQueryImpl processDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<ProcessDefinition> findProcessDefinitionsByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery, Page page)
	  {
		configureProcessDefinitionQuery(processDefinitionQuery);
		return DbEntityManager.selectList("selectProcessDefinitionsByQueryCriteria", processDefinitionQuery, page);
	  }

	  public virtual long findProcessDefinitionCountByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery)
	  {
		configureProcessDefinitionQuery(processDefinitionQuery);
		return (long?) DbEntityManager.selectOne("selectProcessDefinitionCountByQueryCriteria", processDefinitionQuery).Value;
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByDeploymentAndKey(string deploymentId, string processDefinitionKey)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["processDefinitionKey"] = processDefinitionKey;
		return (ProcessDefinitionEntity) DbEntityManager.selectOne("selectProcessDefinitionByDeploymentAndKey", parameters);
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByKeyVersionAndTenantId(string processDefinitionKey, int? processDefinitionVersion, string tenantId)
	  {
		return findProcessDefinitionByKeyVersionOrVersionTag(processDefinitionKey, processDefinitionVersion, null, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByKeyVersionTagAndTenantId(string processDefinitionKey, string processDefinitionVersionTag, string tenantId)
	  {
		return findProcessDefinitionByKeyVersionOrVersionTag(processDefinitionKey, null, processDefinitionVersionTag, tenantId);
	  }

	  protected internal virtual ProcessDefinitionEntity findProcessDefinitionByKeyVersionOrVersionTag(string processDefinitionKey, int? processDefinitionVersion, string processDefinitionVersionTag, string tenantId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		if (processDefinitionVersion != null)
		{
		  parameters["processDefinitionVersion"] = processDefinitionVersion;
		}
		else if (!string.ReferenceEquals(processDefinitionVersionTag, null))
		{
		  parameters["processDefinitionVersionTag"] = processDefinitionVersionTag;
		}
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["tenantId"] = tenantId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<ProcessDefinitionEntity> results = getDbEntityManager().selectList("selectProcessDefinitionByKeyVersionAndTenantId", parameters);
		IList<ProcessDefinitionEntity> results = DbEntityManager.selectList("selectProcessDefinitionByKeyVersionAndTenantId", parameters);
		if (results.Count == 1)
		{
		  return results[0];
		}
		else if (results.Count > 1)
		{
		  if (processDefinitionVersion != null)
		  {
			throw LOG.toManyProcessDefinitionsException(results.Count, processDefinitionKey, "version", processDefinitionVersion.ToString(), tenantId);
		  }
		  else if (!string.ReferenceEquals(processDefinitionVersionTag, null))
		  {
			throw LOG.toManyProcessDefinitionsException(results.Count, processDefinitionKey, "versionTag", processDefinitionVersionTag, tenantId);
		  }
		}
		return null;
	  }

	  public virtual IList<ProcessDefinition> findProcessDefinitionsByKey(string processDefinitionKey)
	  {
		ProcessDefinitionQueryImpl processDefinitionQuery = (new ProcessDefinitionQueryImpl()).processDefinitionKey(processDefinitionKey);
		return findProcessDefinitionsByQueryCriteria(processDefinitionQuery, null);
	  }

	  public virtual IList<ProcessDefinition> findProcessDefinitionsStartableByUser(string user)
	  {
		return (new ProcessDefinitionQueryImpl()).startableByUser(user).list();
	  }

	  public virtual string findPreviousProcessDefinitionId(string processDefinitionKey, int? version, string tenantId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["key"] = processDefinitionKey;
		@params["version"] = version;
		@params["tenantId"] = tenantId;
		return (string) DbEntityManager.selectOne("selectPreviousProcessDefinitionId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByDeploymentId(String deploymentId)
	  public virtual IList<ProcessDefinition> findProcessDefinitionsByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectProcessDefinitionByDeploymentId", deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findProcessDefinitionsByKeyIn(String... keys)
	  public virtual IList<ProcessDefinition> findProcessDefinitionsByKeyIn(params string[] keys)
	  {
		return DbEntityManager.selectList("selectProcessDefinitionByKeyIn", keys);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findDefinitionsByKeyAndTenantId(String processDefinitionKey, String tenantId, boolean isTenantIdSet)
	  public virtual IList<ProcessDefinition> findDefinitionsByKeyAndTenantId(string processDefinitionKey, string tenantId, bool isTenantIdSet)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isTenantIdSet"] = isTenantIdSet;
		parameters["tenantId"] = tenantId;

		return DbEntityManager.selectList("selectProcessDefinitions", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> findDefinitionsByIds(java.util.Set<String> processDefinitionIds)
	  public virtual IList<ProcessDefinition> findDefinitionsByIds(ISet<string> processDefinitionIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionIds"] = processDefinitionIds;
		parameters["isTenantIdSet"] = false;

		return DbEntityManager.selectList("selectProcessDefinitions", parameters);
	  }

	  // update ///////////////////////////////////////////////////////////

	  public virtual void updateProcessDefinitionSuspensionStateById(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateProcessDefinitionSuspensionStateByKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateProcessDefinitionSuspensionStateByKeyAndTenantId(string processDefinitionKey, string tenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isTenantIdSet"] = true;
		parameters["tenantId"] = tenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ProcessDefinitionEntity), "updateProcessDefinitionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  // delete  ///////////////////////////////////////////////////////////

	  /// <summary>
	  /// Cascades the deletion of the process definition to the process instances.
	  /// Skips the custom listeners if the flag was set to true.
	  /// </summary>
	  /// <param name="processDefinitionId"> the process definition id </param>
	  /// <param name="skipCustomListeners"> true if the custom listeners should be skipped at process instance deletion </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked </param>
	  protected internal virtual void cascadeDeleteProcessInstancesForProcessDefinition(string processDefinitionId, bool skipCustomListeners, bool skipIoMappings)
	  {
		ProcessInstanceManager.deleteProcessInstancesByProcessDefinition(processDefinitionId, "deleted process definition", true, skipCustomListeners, skipIoMappings);
	  }

	  /// <summary>
	  /// Cascades the deletion of a process definition to the history, deletes the history.
	  /// </summary>
	  /// <param name="processDefinitionId"> the process definition id </param>
	  protected internal virtual void cascadeDeleteHistoryForProcessDefinition(string processDefinitionId)
	  {
		 // remove historic incidents which are not referenced to a process instance
		HistoricIncidentManager.deleteHistoricIncidentsByProcessDefinitionId(processDefinitionId);

		 // remove historic identity links which are not reference to a process instance
		HistoricIdentityLinkManager.deleteHistoricIdentityLinksLogByProcessDefinitionId(processDefinitionId);

		 // remove historic job log entries not related to a process instance
		HistoricJobLogManager.deleteHistoricJobLogsByProcessDefinitionId(processDefinitionId);
	  }

	  /// <summary>
	  /// Deletes the timer start events for the given process definition.
	  /// </summary>
	  /// <param name="processDefinition"> the process definition </param>
	  protected internal virtual void deleteTimerStartEventsForProcessDefinition(ProcessDefinition processDefinition)
	  {
		IList<JobEntity> timerStartJobs = JobManager.findJobsByConfiguration(TimerStartEventJobHandler.TYPE, processDefinition.Key, processDefinition.TenantId);

		ProcessDefinitionEntity latestVersion = ProcessDefinitionManager.findLatestProcessDefinitionByKeyAndTenantId(processDefinition.Key, processDefinition.TenantId);

		// delete timer start event jobs only if this is the latest version of the process definition.
		if (latestVersion != null && latestVersion.Id.Equals(processDefinition.Id))
		{
		  foreach (Job job in timerStartJobs)
		  {
			((JobEntity)job).delete();
		  }
		}
	  }

	  /// <summary>
	  /// Deletes the subscriptions for the process definition, which is
	  /// identified by the given process definition id.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition </param>
	  public virtual void deleteSubscriptionsForProcessDefinition(string processDefinitionId)
	  {
		IList<EventSubscriptionEntity> eventSubscriptionsToRemove = new List<EventSubscriptionEntity>();
		// remove message event subscriptions:
		IList<EventSubscriptionEntity> messageEventSubscriptions = EventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.MESSAGE.name(), processDefinitionId);
		((IList<EventSubscriptionEntity>)eventSubscriptionsToRemove).AddRange(messageEventSubscriptions);

		// remove signal event subscriptions:
		IList<EventSubscriptionEntity> signalEventSubscriptions = EventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.SIGNAL.name(), processDefinitionId);
		((IList<EventSubscriptionEntity>)eventSubscriptionsToRemove).AddRange(signalEventSubscriptions);

		// remove conditional event subscriptions:
		IList<EventSubscriptionEntity> conditionalEventSubscriptions = EventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.CONDITONAL.name(), processDefinitionId);
		((IList<EventSubscriptionEntity>)eventSubscriptionsToRemove).AddRange(conditionalEventSubscriptions);

		foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptionsToRemove)
		{
		  eventSubscriptionEntity.delete();
		}
	  }

	 /// <summary>
	 /// Deletes the given process definition from the database and cache.
	 /// If cascadeToHistory and cascadeToInstances is set to true it deletes
	 /// the history and the process instances.
	 /// 
	 /// *Note*: If more than one process definition, from one deployment, is deleted in
	 /// a single transaction and the cascadeToHistory and cascadeToInstances flag was set to true it
	 /// can cause a dirty deployment cache. The process instances of ALL process definitions must be deleted,
	 /// before every process definition can be deleted! In such cases the cascadeToInstances flag
	 /// have to set to false!
	 /// 
	 /// On deletion of all process instances, the task listeners will be deleted as well.
	 /// Deletion of tasks and listeners needs the redeployment of deployments.
	 /// It can cause to problems if is done sequential with the deletion of process definition
	 /// in a single transaction.
	 /// 
	 /// *For example*:
	 /// Deployment contains two process definition. First process definition
	 /// and instances will be removed, also cleared from the cache.
	 /// Second process definition will be removed and his instances.
	 /// Deletion of instances will cause redeployment this deploys again
	 /// first into the cache. Only the second will be removed from cache and
	 /// first remains in the cache after the deletion process.
	 /// </summary>
	 /// <param name="processDefinition"> the process definition which should be deleted </param>
	 /// <param name="processDefinitionId"> the id of the process definition </param>
	 /// <param name="cascadeToHistory"> if true the history will deleted as well </param>
	 /// <param name="cascadeToInstances"> if true the process instances are deleted as well </param>
	 /// <param name="skipCustomListeners"> if true skips the custom listeners on deletion of instances </param>
	 /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked </param>
	  public virtual void deleteProcessDefinition(ProcessDefinition processDefinition, string processDefinitionId, bool cascadeToHistory, bool cascadeToInstances, bool skipCustomListeners, bool skipIoMappings)
	  {

		if (cascadeToHistory)
		{
		  cascadeDeleteHistoryForProcessDefinition(processDefinitionId);
		  if (cascadeToInstances)
		  {
			cascadeDeleteProcessInstancesForProcessDefinition(processDefinitionId, skipCustomListeners, skipIoMappings);
		  }
		}
		else
		{
		  ProcessInstanceQueryImpl procInstQuery = (new ProcessInstanceQueryImpl()).processDefinitionId(processDefinitionId);
		  long processInstanceCount = ProcessInstanceManager.findProcessInstanceCountByQueryCriteria(procInstQuery);
		  if (processInstanceCount != 0)
		  {
			throw LOG.deleteProcessDefinitionWithProcessInstancesException(processDefinitionId, processInstanceCount);
		  }
		}

		// remove related authorization parameters in IdentityLink table
		IdentityLinkManager.deleteIdentityLinksByProcDef(processDefinitionId);

		// remove timer start events:
		deleteTimerStartEventsForProcessDefinition(processDefinition);

		//delete process definition from database
		DbEntityManager.delete(typeof(ProcessDefinitionEntity), "deleteProcessDefinitionsById", processDefinitionId);

		// remove process definition from cache:
		Context.ProcessEngineConfiguration.DeploymentCache.removeProcessDefinition(processDefinitionId);

		deleteSubscriptionsForProcessDefinition(processDefinitionId);

		// delete job definitions
		JobDefinitionManager.deleteJobDefinitionsByProcessDefinitionId(processDefinition.Id);

	  }


	  // helper ///////////////////////////////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(ProcessDefinition processDefinition)
	  {
		if (AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newProcessDefinition(processDefinition);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureProcessDefinitionQuery(ProcessDefinitionQueryImpl query)
	  {
		AuthorizationManager.configureProcessDefinitionQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  public virtual ProcessDefinitionEntity findLatestDefinitionByKey(string key)
	  {
		return findLatestProcessDefinitionByKey(key);
	  }

	  public virtual ProcessDefinitionEntity findLatestDefinitionById(string id)
	  {
		return findLatestProcessDefinitionById(id);
	  }

	  public virtual ProcessDefinitionEntity getCachedResourceDefinitionEntity(string definitionId)
	  {
		return DbEntityManager.getCachedEntity(typeof(ProcessDefinitionEntity), definitionId);
	  }

	  public virtual ProcessDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return findLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
	  {
		return findProcessDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId)
	  {
		return findProcessDefinitionByKeyVersionTagAndTenantId(definitionKey, definitionVersionTag, tenantId);
	  }

	  public virtual ProcessDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return findProcessDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }
	}

}