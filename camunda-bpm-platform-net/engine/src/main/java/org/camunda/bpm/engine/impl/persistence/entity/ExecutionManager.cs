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

	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ExecutionManager : AbstractManager
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public virtual void insertExecution(ExecutionEntity execution)
	  {
		DbEntityManager.insert(execution);
		createDefaultAuthorizations(execution);
	  }

	  public virtual void deleteExecution(ExecutionEntity execution)
	  {
		DbEntityManager.delete(execution);
		if (execution.ProcessInstanceExecution)
		{
		  deleteAuthorizations(Resources.PROCESS_INSTANCE, execution.ProcessInstanceId);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteProcessInstancesByProcessDefinition(String processDefinitionId, String deleteReason, boolean cascade, boolean skipCustomListeners, boolean skipIoMappings)
	  public virtual void deleteProcessInstancesByProcessDefinition(string processDefinitionId, string deleteReason, bool cascade, bool skipCustomListeners, bool skipIoMappings)
	  {
		IList<string> processInstanceIds = DbEntityManager.selectList("selectProcessInstanceIdsByProcessDefinitionId", processDefinitionId);

		foreach (string processInstanceId in processInstanceIds)
		{
		  deleteProcessInstance(processInstanceId, deleteReason, cascade, skipCustomListeners, false, skipIoMappings, false);
		}

		if (cascade)
		{
		  HistoricProcessInstanceManager.deleteHistoricProcessInstanceByProcessDefinitionId(processDefinitionId);
		}
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason)
	  {
		deleteProcessInstance(processInstanceId, deleteReason, false, false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners)
	  {
		deleteProcessInstance(processInstanceId, deleteReason, cascade, skipCustomListeners, false, false, false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses)
	  {
		ExecutionEntity execution = findExecutionById(processInstanceId);

		if (execution == null)
		{
		  throw LOG.requestedProcessInstanceNotFoundException(processInstanceId);
		}

		TaskManager.deleteTasksByProcessInstanceId(processInstanceId, deleteReason, cascade, skipCustomListeners);

		// delete the execution BEFORE we delete the history, otherwise we will produce orphan HistoricVariableInstance instances
		execution.deleteCascade(deleteReason, skipCustomListeners, skipIoMappings, externallyTerminated, skipSubprocesses);

		if (cascade)
		{
		  HistoricProcessInstanceManager.deleteHistoricProcessInstanceByIds(Arrays.asList(processInstanceId));
		}
	  }

	  public virtual ExecutionEntity findSubProcessInstanceBySuperExecutionId(string superExecutionId)
	  {
		return (ExecutionEntity) DbEntityManager.selectOne("selectSubProcessInstanceBySuperExecutionId", superExecutionId);
	  }

	  public virtual ExecutionEntity findSubProcessInstanceBySuperCaseExecutionId(string superCaseExecutionId)
	  {
		return (ExecutionEntity) DbEntityManager.selectOne("selectSubProcessInstanceBySuperCaseExecutionId", superCaseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findChildExecutionsByParentExecutionId(String parentExecutionId)
	  public virtual IList<ExecutionEntity> findChildExecutionsByParentExecutionId(string parentExecutionId)
	  {
		return DbEntityManager.selectList("selectExecutionsByParentExecutionId", parentExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findExecutionsByProcessInstanceId(String processInstanceId)
	  public virtual IList<ExecutionEntity> findExecutionsByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectExecutionsByProcessInstanceId", processInstanceId);
	  }

	  public virtual ExecutionEntity findExecutionById(string executionId)
	  {
		return DbEntityManager.selectById(typeof(ExecutionEntity), executionId);
	  }

	  public virtual long findExecutionCountByQueryCriteria(ExecutionQueryImpl executionQuery)
	  {
		configureQuery(executionQuery);
		return (long?) DbEntityManager.selectOne("selectExecutionCountByQueryCriteria", executionQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findExecutionsByQueryCriteria(org.camunda.bpm.engine.impl.ExecutionQueryImpl executionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<ExecutionEntity> findExecutionsByQueryCriteria(ExecutionQueryImpl executionQuery, Page page)
	  {
		configureQuery(executionQuery);
		return DbEntityManager.selectList("selectExecutionsByQueryCriteria", executionQuery, page);
	  }

	  public virtual long findProcessInstanceCountByQueryCriteria(ProcessInstanceQueryImpl processInstanceQuery)
	  {
		configureQuery(processInstanceQuery);
		return (long?) DbEntityManager.selectOne("selectProcessInstanceCountByQueryCriteria", processInstanceQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> findProcessInstancesByQueryCriteria(org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<ProcessInstance> findProcessInstancesByQueryCriteria(ProcessInstanceQueryImpl processInstanceQuery, Page page)
	  {
		configureQuery(processInstanceQuery);
		return DbEntityManager.selectList("selectProcessInstanceByQueryCriteria", processInstanceQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findProcessInstancesIdsByQueryCriteria(org.camunda.bpm.engine.impl.ProcessInstanceQueryImpl processInstanceQuery)
	  public virtual IList<string> findProcessInstancesIdsByQueryCriteria(ProcessInstanceQueryImpl processInstanceQuery)
	  {
		configureQuery(processInstanceQuery);
		return DbEntityManager.selectList("selectProcessInstanceIdsByQueryCriteria", processInstanceQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findEventScopeExecutionsByActivityId(String activityRef, String parentExecutionId)
	  public virtual IList<ExecutionEntity> findEventScopeExecutionsByActivityId(string activityRef, string parentExecutionId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["activityId"] = activityRef;
		parameters["parentExecutionId"] = parentExecutionId;
		return DbEntityManager.selectList("selectExecutionsByParentExecutionId", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.Execution> findExecutionsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Execution> findExecutionsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.ProcessInstance> findProcessInstanceByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<ProcessInstance> findProcessInstanceByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findExecutionCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectExecutionCountByNativeQuery", parameterMap).Value;
	  }

	  public virtual void updateExecutionSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateExecutionSuspensionStateByProcessInstanceId(string processInstanceId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateExecutionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateExecutionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string tenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isTenantIdSet"] = true;
		parameters["tenantId"] = tenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExecutionEntity), "updateExecutionSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  // helper ///////////////////////////////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(ExecutionEntity execution)
	  {
		if (execution.ProcessInstanceExecution && AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newProcessInstance(execution);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureQuery<T1>(AbstractQuery<T1> query)
	  {
		AuthorizationManager.configureExecutionQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	}

}