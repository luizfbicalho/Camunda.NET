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

	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoricDetailEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDetailEventEntity;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricDetailManager : AbstractHistoricManager
	{

	  public virtual void deleteHistoricDetailsByProcessInstanceIds(IList<string> historicProcessInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceIds"] = historicProcessInstanceIds;
		deleteHistoricDetails(parameters);
	  }

	  public virtual void deleteHistoricDetailsByTaskProcessInstanceIds(IList<string> historicProcessInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskProcessInstanceIds"] = historicProcessInstanceIds;
		deleteHistoricDetails(parameters);
	  }

	  public virtual void deleteHistoricDetailsByCaseInstanceIds(IList<string> historicCaseInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["caseInstanceIds"] = historicCaseInstanceIds;
		deleteHistoricDetails(parameters);
	  }

	  public virtual void deleteHistoricDetailsByTaskCaseInstanceIds(IList<string> historicCaseInstanceIds)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["taskCaseInstanceIds"] = historicCaseInstanceIds;
		deleteHistoricDetails(parameters);
	  }

	  public virtual void deleteHistoricDetailsByVariableInstanceId(string historicVariableInstanceId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["variableInstanceId"] = historicVariableInstanceId;
		deleteHistoricDetails(parameters);
	  }

	  public virtual void deleteHistoricDetails(IDictionary<string, object> parameters)
	  {
		DbEntityManager.deletePreserveOrder(typeof(ByteArrayEntity), "deleteHistoricDetailByteArraysByIds", parameters);
		DbEntityManager.deletePreserveOrder(typeof(HistoricDetailEventEntity), "deleteHistoricDetailsByIds", parameters);
	  }


	  public virtual long findHistoricDetailCountByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery)
	  {
		configureQuery(historicVariableUpdateQuery);
		return (long?) DbEntityManager.selectOne("selectHistoricDetailCountByQueryCriteria", historicVariableUpdateQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricDetail> findHistoricDetailsByQueryCriteria(org.camunda.bpm.engine.impl.HistoricDetailQueryImpl historicVariableUpdateQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricDetail> findHistoricDetailsByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery, Page page)
	  {
		configureQuery(historicVariableUpdateQuery);
		return DbEntityManager.selectList("selectHistoricDetailsByQueryCriteria", historicVariableUpdateQuery, page);
	  }

	  public virtual void deleteHistoricDetailsByTaskId(string taskId)
	  {
		if (HistoryEnabled)
		{
		  // delete entries in DB
		  IList<HistoricDetail> historicDetails = findHistoricDetailsByTaskId(taskId);

		  foreach (HistoricDetail historicDetail in historicDetails)
		  {
			((HistoricDetailEventEntity) historicDetail).delete();
		  }

		  //delete entries in Cache
		  IList<HistoricDetailEventEntity> cachedHistoricDetails = DbEntityManager.getCachedEntitiesByType(typeof(HistoricDetailEventEntity));
		  foreach (HistoricDetailEventEntity historicDetail in cachedHistoricDetails)
		  {
			// make sure we only delete the right ones (as we cannot make a proper query in the cache)
			if (taskId.Equals(historicDetail.TaskId))
			{
			  historicDetail.delete();
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricDetail> findHistoricDetailsByTaskId(String taskId)
	  public virtual IList<HistoricDetail> findHistoricDetailsByTaskId(string taskId)
	  {
		return DbEntityManager.selectList("selectHistoricDetailsByTaskId", taskId);
	  }

	  protected internal virtual void configureQuery(HistoricDetailQueryImpl query)
	  {
		AuthorizationManager.configureHistoricDetailQuery(query);
		TenantManager.configureQuery(query);
	  }

	  public virtual void addRemovalTimeToDetailsByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDetailEventEntity), "updateHistoricDetailsByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToDetailsByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricDetailEventEntity), "updateHistoricDetailsByProcessInstanceId", parameters);
	  }

	  public virtual DbOperation deleteHistoricDetailsByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricDetailEventEntity), "deleteHistoricDetailsByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	}

}