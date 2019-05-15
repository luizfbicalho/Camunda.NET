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

	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using TransactionListener = org.camunda.bpm.engine.impl.cfg.TransactionListener;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using TopicFetchInstruction = org.camunda.bpm.engine.impl.externaltask.TopicFetchInstruction;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskManager : AbstractManager
	{

	  public static QueryOrderingProperty EXT_TASK_PRIORITY_ORDERING_PROPERTY = new QueryOrderingProperty(org.camunda.bpm.engine.impl.ExternalTaskQueryProperty_Fields.PRIORITY, Direction.DESCENDING);

	  public virtual ExternalTaskEntity findExternalTaskById(string id)
	  {
		return DbEntityManager.selectById(typeof(ExternalTaskEntity), id);
	  }

	  public virtual void insert(ExternalTaskEntity externalTask)
	  {
		DbEntityManager.insert(externalTask);
		fireExternalTaskAvailableEvent();
	  }

	  public virtual void delete(ExternalTaskEntity externalTask)
	  {
		DbEntityManager.delete(externalTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExternalTaskEntity> findExternalTasksByExecutionId(String id)
	  public virtual IList<ExternalTaskEntity> findExternalTasksByExecutionId(string id)
	  {
		return DbEntityManager.selectList("selectExternalTasksByExecutionId", id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExternalTaskEntity> findExternalTasksByProcessInstanceId(String processInstanceId)
	  public virtual IList<ExternalTaskEntity> findExternalTasksByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectExternalTasksByProcessInstanceId", processInstanceId);
	  }

	  public virtual IList<ExternalTaskEntity> selectExternalTasksForTopics(ICollection<TopicFetchInstruction> queryFilters, int maxResults, bool usePriority)
	  {
		if (queryFilters.Count == 0)
		{
		  return new List<ExternalTaskEntity>();
		}

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["topics"] = queryFilters;
		parameters["now"] = ClockUtil.CurrentTime;
		parameters["applyOrdering"] = usePriority;
		IList<QueryOrderingProperty> orderingProperties = new List<QueryOrderingProperty>();
		orderingProperties.Add(EXT_TASK_PRIORITY_ORDERING_PROPERTY);
		parameters["orderingProperties"] = orderingProperties;

		ListQueryParameterObject parameter = new ListQueryParameterObject(parameters, 0, maxResults);
		configureQuery(parameter);

		DbEntityManager manager = DbEntityManager;
		return manager.selectList("selectExternalTasksForTopics", parameter);
	  }

	  public virtual IList<ExternalTask> findExternalTasksByQueryCriteria(ExternalTaskQueryImpl externalTaskQuery)
	  {
		configureQuery(externalTaskQuery);
		return DbEntityManager.selectList("selectExternalTaskByQueryCriteria", externalTaskQuery);
	  }

	  public virtual IList<string> findExternalTaskIdsByQueryCriteria(ExternalTaskQueryImpl externalTaskQuery)
	  {
		configureQuery(externalTaskQuery);
		return DbEntityManager.selectList("selectExternalTaskIdsByQueryCriteria", externalTaskQuery);
	  }

	  public virtual long findExternalTaskCountByQueryCriteria(ExternalTaskQueryImpl externalTaskQuery)
	  {
		configureQuery(externalTaskQuery);
		return (long?) DbEntityManager.selectOne("selectExternalTaskCountByQueryCriteria", externalTaskQuery).Value;
	  }

	  protected internal virtual void updateExternalTaskSuspensionState(string processInstanceId, string processDefinitionId, string processDefinitionKey, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = false;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExternalTaskEntity), "updateExternalTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  public virtual void updateExternalTaskSuspensionStateByProcessInstanceId(string processInstanceId, SuspensionState suspensionState)
	  {
		updateExternalTaskSuspensionState(processInstanceId, null, null, suspensionState);
	  }

	  public virtual void updateExternalTaskSuspensionStateByProcessDefinitionId(string processDefinitionId, SuspensionState suspensionState)
	  {
		updateExternalTaskSuspensionState(null, processDefinitionId, null, suspensionState);
	  }

	  public virtual void updateExternalTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey, SuspensionState suspensionState)
	  {
		updateExternalTaskSuspensionState(null, null, processDefinitionKey, suspensionState);
	  }

	  public virtual void updateExternalTaskSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, SuspensionState suspensionState)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["isProcessDefinitionTenantIdSet"] = true;
		parameters["processDefinitionTenantId"] = processDefinitionTenantId;
		parameters["suspensionState"] = suspensionState.StateCode;
		DbEntityManager.update(typeof(ExternalTaskEntity), "updateExternalTaskSuspensionStateByParameters", configureParameterizedQuery(parameters));
	  }

	  protected internal virtual void configureQuery(ExternalTaskQueryImpl query)
	  {
		AuthorizationManager.configureExternalTaskQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual void configureQuery(ListQueryParameterObject parameter)
	  {
		AuthorizationManager.configureExternalTaskFetch(parameter);
		TenantManager.configureQuery(parameter);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  public virtual void fireExternalTaskAvailableEvent()
	  {
		Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, new TransactionListenerAnonymousInnerClass(this));
	  }

	  private class TransactionListenerAnonymousInnerClass : TransactionListener
	  {
		  private readonly ExternalTaskManager outerInstance;

		  public TransactionListenerAnonymousInnerClass(ExternalTaskManager outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void execute(CommandContext commandContext)
		  {
			ProcessEngineImpl.EXT_TASK_CONDITIONS.signalAll();
		  }
	  }
	}



}