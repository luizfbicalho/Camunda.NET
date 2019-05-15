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

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoricActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricActivityInstanceEventEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricActivityInstanceManager : AbstractHistoricManager
	{

	  public virtual void deleteHistoricActivityInstancesByProcessInstanceIds(IList<string> historicProcessInstanceIds)
	  {
		DbEntityManager.deletePreserveOrder(typeof(HistoricActivityInstanceEntity), "deleteHistoricActivityInstancesByProcessInstanceIds", historicProcessInstanceIds);
	  }

	  public virtual void insertHistoricActivityInstance(HistoricActivityInstanceEntity historicActivityInstance)
	  {
		DbEntityManager.insert(historicActivityInstance);
	  }

	  public virtual HistoricActivityInstanceEntity findHistoricActivityInstance(string activityId, string processInstanceId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["activityId"] = activityId;
		parameters["processInstanceId"] = processInstanceId;

		return (HistoricActivityInstanceEntity) DbEntityManager.selectOne("selectHistoricActivityInstance", parameters);
	  }

	  public virtual long findHistoricActivityInstanceCountByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery)
	  {
		configureQuery(historicActivityInstanceQuery);
		return (long?) DbEntityManager.selectOne("selectHistoricActivityInstanceCountByQueryCriteria", historicActivityInstanceQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricActivityInstance> findHistoricActivityInstancesByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, Page page)
	  {
		configureQuery(historicActivityInstanceQuery);
		return DbEntityManager.selectList("selectHistoricActivityInstancesByQueryCriteria", historicActivityInstanceQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricActivityInstance> findHistoricActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricActivityInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricActivityInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureQuery(HistoricActivityInstanceQueryImpl query)
	  {
		AuthorizationManager.configureHistoricActivityInstanceQuery(query);
		TenantManager.configureQuery(query);
	  }

	  public virtual void addRemovalTimeToActivityInstancesByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricActivityInstanceEventEntity), "updateHistoricActivityInstancesByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToActivityInstancesByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricActivityInstanceEventEntity), "updateHistoricActivityInstancesByProcessInstanceId", parameters);
	  }

	  public virtual DbOperation deleteHistoricActivityInstancesByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricActivityInstanceEntity), "deleteHistoricActivityInstancesByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}

}