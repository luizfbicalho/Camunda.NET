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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TaskManager : AbstractManager
	{

	  public virtual void insertTask(TaskEntity task)
	  {
		DbEntityManager.insert(task);
		createDefaultAuthorizations(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void deleteTasksByProcessInstanceId(String processInstanceId, String deleteReason, boolean cascade, boolean skipCustomListeners)
	  public virtual void deleteTasksByProcessInstanceId(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners)
	  {
		IList<TaskEntity> tasks = (System.Collections.IList) DbEntityManager.createTaskQuery().processInstanceId(processInstanceId).list();

		string reason = (string.ReferenceEquals(deleteReason, null) || deleteReason.Length == 0) ? TaskEntity.DELETE_REASON_DELETED : deleteReason;

		foreach (TaskEntity task in tasks)
		{
		  task.delete(reason, cascade, skipCustomListeners);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void deleteTasksByCaseInstanceId(String caseInstanceId, String deleteReason, boolean cascade)
	  public virtual void deleteTasksByCaseInstanceId(string caseInstanceId, string deleteReason, bool cascade)
	  {
		IList<TaskEntity> tasks = (System.Collections.IList) DbEntityManager.createTaskQuery().caseInstanceId(caseInstanceId).list();

		  string reason = (string.ReferenceEquals(deleteReason, null) || deleteReason.Length == 0) ? TaskEntity.DELETE_REASON_DELETED : deleteReason;

		  foreach (TaskEntity task in tasks)
		  {
			task.delete(reason, cascade, false);
		  }
	  }

	  public virtual void deleteTask(TaskEntity task, string deleteReason, bool cascade, bool skipCustomListeners)
	  {
		if (!task.Deleted)
		{
		  task.Deleted = true;

		  CommandContext commandContext = Context.CommandContext;
		  string taskId = task.Id;

		  IList<Task> subTasks = findTasksByParentTaskId(taskId);
		  foreach (Task subTask in subTasks)
		  {
			((TaskEntity) subTask).delete(deleteReason, cascade, skipCustomListeners);
		  }

		  task.deleteIdentityLinks(false);

		  commandContext.VariableInstanceManager.deleteVariableInstanceByTask(task);

		  if (cascade)
		  {
			commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstanceById(taskId);
		  }
		  else
		  {
			commandContext.HistoricTaskInstanceManager.markTaskInstanceEnded(taskId, deleteReason);
			if (TaskEntity.DELETE_REASON_COMPLETED.Equals(deleteReason))
			{
			  task.createHistoricTaskDetails(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE);
			}
			else
			{
			  task.createHistoricTaskDetails(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE);
			}
		  }

		  deleteAuthorizations(Resources.TASK, taskId);
		  DbEntityManager.delete(task);
		}
	  }


	  public virtual TaskEntity findTaskById(string id)
	  {
		ensureNotNull("Invalid task id", "id", id);
		return DbEntityManager.selectById(typeof(TaskEntity), id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TaskEntity> findTasksByExecutionId(String executionId)
	  public virtual IList<TaskEntity> findTasksByExecutionId(string executionId)
	  {
		return DbEntityManager.selectList("selectTasksByExecutionId", executionId);
	  }

	  public virtual TaskEntity findTaskByCaseExecutionId(string caseExecutionId)
	  {
		return (TaskEntity) DbEntityManager.selectOne("selectTaskByCaseExecutionId", caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TaskEntity> findTasksByProcessInstanceId(String processInstanceId)
	  public virtual IList<TaskEntity> findTasksByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectTasksByProcessInstanceId", processInstanceId);
	  }


	  [Obsolete]
	  public virtual IList<Task> findTasksByQueryCriteria(TaskQueryImpl taskQuery, Page page)
	  {
		taskQuery.FirstResult = page.FirstResult;
		taskQuery.MaxResults = page.MaxResults;
		return findTasksByQueryCriteria(taskQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Task> findTasksByQueryCriteria(org.camunda.bpm.engine.impl.TaskQueryImpl taskQuery)
	  public virtual IList<Task> findTasksByQueryCriteria(TaskQueryImpl taskQuery)
	  {
		configureQuery(taskQuery);
		return DbEntityManager.selectList("selectTaskByQueryCriteria", taskQuery);
	  }

	  public virtual long findTaskCountByQueryCriteria(TaskQueryImpl taskQuery)
	  {
		configureQuery(taskQuery);
		return (long?) DbEntityManager.selectOne("selectTaskCountByQueryCriteria", taskQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Task> findTasksByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Task> findTasksByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectTaskByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findTaskCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectTaskCountByNativeQuery", parameterMap).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.task.Task> findTasksByParentTaskId(String parentTaskId)
	  public virtual IList<Task> findTasksByParentTaskId(string parentTaskId)
	  {
		return DbEntityManager.selectList("selectTasksByParentTaskId", parentTaskId);
	  }

	  public virtual void updateTaskSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateTaskSuspensionStateByProcessInstanceId(string processInstanceId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateTaskSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = true;
		parameters["processDefinitionTenantId"] = processDefinitionTenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateTaskSuspensionStateByCaseExecutionId(string caseExecutionId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["caseExecutionId"] = caseExecutionId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));

	  }

	  // helper ///////////////////////////////////////////////////////////

	  protected internal virtual void createDefaultAuthorizations(TaskEntity task)
	  {
		if (AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newTask(task);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureQuery(TaskQueryImpl query)
	  {
		AuthorizationManager.configureTaskQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	}

}