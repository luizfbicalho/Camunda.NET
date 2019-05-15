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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;


	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;


	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	public class HistoricVariableInstanceManager : AbstractHistoricManager
	{

	  public virtual void deleteHistoricVariableInstanceByVariableInstanceId(string historicVariableInstanceId)
	  {
		if (HistoryEnabled)
		{
		  HistoricVariableInstanceEntity historicVariableInstance = findHistoricVariableInstanceByVariableInstanceId(historicVariableInstanceId);
		  if (historicVariableInstance != null)
		  {
			historicVariableInstance.delete();
		  }
		}
	  }

	  public virtual void deleteHistoricVariableInstanceByProcessInstanceIds(IList<string> historicProcessInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceIds"] = historicProcessInstanceIds;
		deleteHistoricVariableInstances(parameters);
	  }

	  public virtual void deleteHistoricVariableInstancesByTaskProcessInstanceIds(IList<string> historicProcessInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskProcessInstanceIds"] = historicProcessInstanceIds;
		deleteHistoricVariableInstances(parameters);
	  }

	  public virtual void deleteHistoricVariableInstanceByCaseInstanceId(string historicCaseInstanceId)
	  {
		deleteHistoricVariableInstancesByProcessCaseInstanceId(null, historicCaseInstanceId);
	  }

	  public virtual void deleteHistoricVariableInstancesByCaseInstanceIds(IList<string> historicCaseInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["caseInstanceIds"] = historicCaseInstanceIds;
		deleteHistoricVariableInstances(parameters);
	  }

	  protected internal virtual void deleteHistoricVariableInstances(IDictionary<string, object> parameters)
	  {
		DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteHistoricVariableInstanceByteArraysByIds", parameters);
		DbEntityManager.deletePreserveOrder(typeof(HistoricVariableInstanceEntity), "deleteHistoricVariableInstanceByIds", parameters);
	  }

	  protected internal virtual void deleteHistoricVariableInstancesByProcessCaseInstanceId(string historicProcessInstanceId, string historicCaseInstanceId)
	  {
		ensureOnlyOneNotNull("Only the process instance or case instance id should be set", historicProcessInstanceId, historicCaseInstanceId);
		if (HistoryEnabled)
		{

		  // delete entries in DB
		  IList<HistoricVariableInstance> historicVariableInstances;
		  if (!string.ReferenceEquals(historicProcessInstanceId, null))
		  {
			historicVariableInstances = findHistoricVariableInstancesByProcessInstanceId(historicProcessInstanceId);
		  }
		  else
		  {
			historicVariableInstances = findHistoricVariableInstancesByCaseInstanceId(historicCaseInstanceId);
		  }

		  foreach (HistoricVariableInstance historicVariableInstance in historicVariableInstances)
		  {
			((HistoricVariableInstanceEntity) historicVariableInstance).delete();
		  }

		  // delete entries in Cache
		  IList<HistoricVariableInstanceEntity> cachedHistoricVariableInstances = DbEntityManager.getCachedEntitiesByType(typeof(HistoricVariableInstanceEntity));
		  foreach (HistoricVariableInstanceEntity historicVariableInstance in cachedHistoricVariableInstances)
		  {
			// make sure we only delete the right ones (as we cannot make a proper query in the cache)
			if ((!string.ReferenceEquals(historicProcessInstanceId, null) && historicProcessInstanceId.Equals(historicVariableInstance.ProcessInstanceId)) || (!string.ReferenceEquals(historicCaseInstanceId, null) && historicCaseInstanceId.Equals(historicVariableInstance.CaseInstanceId)))
			{
			  historicVariableInstance.delete();
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByProcessInstanceId(String processInstanceId)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByProcessInstanceId(string processInstanceId)
	  {
		return DbEntityManager.selectList("selectHistoricVariablesByProcessInstanceId", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByCaseInstanceId(String caseInstanceId)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByCaseInstanceId(string caseInstanceId)
	  {
		return DbEntityManager.selectList("selectHistoricVariablesByCaseInstanceId", caseInstanceId);
	  }

	  public virtual long findHistoricVariableInstanceCountByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery)
	  {
		configureQuery(historicProcessVariableQuery);
		return (long?) DbEntityManager.selectOne("selectHistoricVariableInstanceCountByQueryCriteria", historicProcessVariableQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricVariableInstanceQueryImpl historicProcessVariableQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery, Page page)
	  {
		configureQuery(historicProcessVariableQuery);
		return DbEntityManager.selectList("selectHistoricVariableInstanceByQueryCriteria", historicProcessVariableQuery, page);
	  }

	  public virtual HistoricVariableInstanceEntity findHistoricVariableInstanceByVariableInstanceId(string variableInstanceId)
	  {
		return (HistoricVariableInstanceEntity) DbEntityManager.selectOne("selectHistoricVariableInstanceByVariableInstanceId", variableInstanceId);
	  }

	  public virtual void deleteHistoricVariableInstancesByTaskId(string taskId)
	  {
		if (HistoryEnabled)
		{
		  HistoricVariableInstanceQuery historicProcessVariableQuery = (new HistoricVariableInstanceQueryImpl()).taskIdIn(taskId);
		  IList<HistoricVariableInstance> historicProcessVariables = historicProcessVariableQuery.list();
		  foreach (HistoricVariableInstance historicProcessVariable in historicProcessVariables)
		  {
			((HistoricVariableInstanceEntity) historicProcessVariable).delete();
		  }
		}
	  }

	  public virtual void addRemovalTimeToVariableInstancesByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricVariableInstanceEntity), "updateHistoricVariableInstancesByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToVariableInstancesByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricVariableInstanceEntity), "updateHistoricVariableInstancesByProcessInstanceId", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricVariableInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricVariableInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricVariableInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureQuery(HistoricVariableInstanceQueryImpl query)
	  {
		AuthorizationManager.configureHistoricVariableInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

	  public virtual DbOperation deleteHistoricVariableInstancesByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricVariableInstanceEntity), "deleteHistoricVariableInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }
	}

}