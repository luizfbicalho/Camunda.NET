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

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricIncidentEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricIncidentEventEntity;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricIncidentManager : AbstractHistoricManager
	{

	  public virtual long findHistoricIncidentCountByQueryCriteria(HistoricIncidentQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectHistoricIncidentCountByQueryCriteria", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricIncident> findHistoricIncidentByQueryCriteria(org.camunda.bpm.engine.impl.HistoricIncidentQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricIncident> findHistoricIncidentByQueryCriteria(HistoricIncidentQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectHistoricIncidentByQueryCriteria", query, page);
	  }

	  public virtual void addRemovalTimeToIncidentsByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricIncidentEventEntity), "updateHistoricIncidentsByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToIncidentsByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricIncidentEventEntity), "updateHistoricIncidentsByProcessInstanceId", parameters);
	  }

	  public virtual void deleteHistoricIncidentsByProcessInstanceIds(IList<string> processInstanceIds)
	  {
		DbEntityManager.deletePreserveOrder(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByProcessInstanceIds", processInstanceIds);
	  }

	  public virtual void deleteHistoricIncidentsByProcessDefinitionId(string processDefinitionId)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByProcessDefinitionId", processDefinitionId);
		}
	  }

	  public virtual void deleteHistoricIncidentsByJobDefinitionId(string jobDefinitionId)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByJobDefinitionId", jobDefinitionId);
		}
	  }

	  public virtual void deleteHistoricIncidentsByBatchId(IList<string> historicBatchIds)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByBatchIds", historicBatchIds);
		}
	  }

	  protected internal virtual void configureQuery(HistoricIncidentQueryImpl query)
	  {
		AuthorizationManager.configureHistoricIncidentQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual bool HistoryEventProduced
	  {
		  get
		  {
			HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
			return historyLevel.isHistoryEventProduced(HistoryEventTypes.INCIDENT_CREATE, null) || historyLevel.isHistoryEventProduced(HistoryEventTypes.INCIDENT_DELETE, null) || historyLevel.isHistoryEventProduced(HistoryEventTypes.INCIDENT_MIGRATE, null) || historyLevel.isHistoryEventProduced(HistoryEventTypes.INCIDENT_RESOLVE, null);
		  }
	  }

	  public virtual DbOperation deleteHistoricIncidentsByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricIncidentEntity), "deleteHistoricIncidentsByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  public virtual void addRemovalTimeToHistoricIncidentsByBatchId(string batchId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["batchId"] = batchId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricIncidentEntity), "updateHistoricIncidentsByBatchId", parameters);
	  }

	}

}