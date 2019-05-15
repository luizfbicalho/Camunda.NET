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

	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricIdentityLinkLogEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricIdentityLinkLogEventEntity;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;

	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class HistoricIdentityLinkLogManager : AbstractHistoricManager
	{

	  public virtual long findHistoricIdentityLinkLogCountByQueryCriteria(HistoricIdentityLinkLogQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectHistoricIdentityLinkCountByQueryCriteria", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricIdentityLinkLog> findHistoricIdentityLinkLogByQueryCriteria(org.camunda.bpm.engine.impl.HistoricIdentityLinkLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricIdentityLinkLog> findHistoricIdentityLinkLogByQueryCriteria(HistoricIdentityLinkLogQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectHistoricIdentityLinkByQueryCriteria", query, page);
	  }

	  public virtual void addRemovalTimeToIdentityLinkLogByRootProcessInstanceId(string rootProcessInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstanceId"] = rootProcessInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricIdentityLinkLogEventEntity), "updateIdentityLinkLogByRootProcessInstanceId", parameters);
	  }

	  public virtual void addRemovalTimeToIdentityLinkLogByProcessInstanceId(string processInstanceId, DateTime removalTime)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["removalTime"] = removalTime;

		DbEntityManager.updatePreserveOrder(typeof(HistoricIdentityLinkLogEventEntity), "updateIdentityLinkLogByProcessInstanceId", parameters);
	  }

	  public virtual void deleteHistoricIdentityLinksLogByProcessDefinitionId(string processDefId)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByProcessDefinitionId", processDefId);
		}
	  }

	  public virtual void deleteHistoricIdentityLinksLogByTaskId(string taskId)
	  {
		if (HistoryEventProduced)
		{
		  DbEntityManager.delete(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByTaskId", taskId);
		}
	  }

	  public virtual void deleteHistoricIdentityLinksLogByTaskProcessInstanceIds(IList<string> processInstanceIds)
	  {
		DbEntityManager.deletePreserveOrder(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByTaskProcessInstanceIds", processInstanceIds);
	  }

	  public virtual void deleteHistoricIdentityLinksLogByTaskCaseInstanceIds(IList<string> caseInstanceIds)
	  {
		DbEntityManager.deletePreserveOrder(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinksByTaskCaseInstanceIds", caseInstanceIds);
	  }

	  public virtual DbOperation deleteHistoricIdentityLinkLogByRemovalTime(DateTime removalTime, int minuteFrom, int minuteTo, int batchSize)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["removalTime"] = removalTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		parameters["batchSize"] = batchSize;

		return DbEntityManager.deletePreserveOrder(typeof(HistoricIdentityLinkLogEntity), "deleteHistoricIdentityLinkLogByRemovalTime", new ListQueryParameterObject(parameters, 0, batchSize));
	  }

	  protected internal virtual void configureQuery(HistoricIdentityLinkLogQueryImpl query)
	  {
		AuthorizationManager.configureHistoricIdentityLinkQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual bool HistoryEventProduced
	  {
		  get
		  {
			HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
			return historyLevel.isHistoryEventProduced(HistoryEventTypes.IDENTITY_LINK_ADD, null) || historyLevel.isHistoryEventProduced(HistoryEventTypes.IDENTITY_LINK_DELETE, null);
		  }
	  }

	}

}