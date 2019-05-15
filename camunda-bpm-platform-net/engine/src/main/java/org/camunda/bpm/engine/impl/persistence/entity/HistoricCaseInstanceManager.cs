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

	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using HistoricCaseInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseInstanceEventEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class HistoricCaseInstanceManager : AbstractHistoricManager
	{

	  public virtual HistoricCaseInstanceEntity findHistoricCaseInstance(string caseInstanceId)
	  {
		if (HistoryEnabled)
		{
		  return DbEntityManager.selectById(typeof(HistoricCaseInstanceEntity), caseInstanceId);
		}
		return null;
	  }

	  public virtual HistoricCaseInstanceEventEntity findHistoricCaseInstanceEvent(string eventId)
	  {
		if (HistoryEnabled)
		{
		  return DbEntityManager.selectById(typeof(HistoricCaseInstanceEventEntity), eventId);
		}
		return null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricCaseInstanceByCaseDefinitionId(String caseDefinitionId)
	  public virtual void deleteHistoricCaseInstanceByCaseDefinitionId(string caseDefinitionId)
	  {
		if (HistoryEnabled)
		{
		  IList<string> historicCaseInstanceIds = DbEntityManager.selectList("selectHistoricCaseInstanceIdsByCaseDefinitionId", caseDefinitionId);

		  if (historicCaseInstanceIds != null && historicCaseInstanceIds.Count > 0)
		  {
			deleteHistoricCaseInstancesByIds(historicCaseInstanceIds);
		  }
		}
	  }

	  public virtual void deleteHistoricCaseInstancesByIds(IList<string> historicCaseInstanceIds)
	  {
		if (HistoryEnabled)
		{
		  HistoricDetailManager.deleteHistoricDetailsByCaseInstanceIds(historicCaseInstanceIds);

		  HistoricVariableInstanceManager.deleteHistoricVariableInstancesByCaseInstanceIds(historicCaseInstanceIds);

		  HistoricCaseActivityInstanceManager.deleteHistoricCaseActivityInstancesByCaseInstanceIds(historicCaseInstanceIds);

		  HistoricTaskInstanceManager.deleteHistoricTaskInstancesByCaseInstanceIds(historicCaseInstanceIds);

		  DbEntityManager.delete(typeof(HistoricCaseInstanceEntity), "deleteHistoricCaseInstancesByIds", historicCaseInstanceIds);
		}
	  }

	  public virtual long findHistoricCaseInstanceCountByQueryCriteria(HistoricCaseInstanceQueryImpl historicCaseInstanceQuery)
	  {
		if (HistoryEnabled)
		{
		  configureHistoricCaseInstanceQuery(historicCaseInstanceQuery);
		  return (long?) DbEntityManager.selectOne("selectHistoricCaseInstanceCountByQueryCriteria", historicCaseInstanceQuery).Value;
		}
		return 0;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricCaseInstance> findHistoricCaseInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricCaseInstanceQueryImpl historicCaseInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricCaseInstance> findHistoricCaseInstancesByQueryCriteria(HistoricCaseInstanceQueryImpl historicCaseInstanceQuery, Page page)
	  {
		if (HistoryEnabled)
		{
		  configureHistoricCaseInstanceQuery(historicCaseInstanceQuery);
		  return DbEntityManager.selectList("selectHistoricCaseInstancesByQueryCriteria", historicCaseInstanceQuery, page);
		}
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricCaseInstance> findHistoricCaseInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricCaseInstance> findHistoricCaseInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricCaseInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricCaseInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricCaseInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureHistoricCaseInstanceQuery(HistoricCaseInstanceQueryImpl query)
	  {
		TenantManager.configureQuery(query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findHistoricCaseInstanceIdsForCleanup(int batchSize, int minuteFrom, int minuteTo)
	  public virtual IList<string> findHistoricCaseInstanceIdsForCleanup(int batchSize, int minuteFrom, int minuteTo)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["currentTimestamp"] = ClockUtil.CurrentTime;
		if (minuteTo - minuteFrom + 1 < 60)
		{
		  parameters["minuteFrom"] = minuteFrom;
		  parameters["minuteTo"] = minuteTo;
		}
		ListQueryParameterObject parameterObject = new ListQueryParameterObject(parameters, 0, batchSize);
		return DbEntityManager.selectList("selectHistoricCaseInstanceIdsForCleanup", parameterObject);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult> findCleanableHistoricCaseInstancesReportByCriteria(org.camunda.bpm.engine.impl.CleanableHistoricCaseInstanceReportImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CleanableHistoricCaseInstanceReportResult> findCleanableHistoricCaseInstancesReportByCriteria(CleanableHistoricCaseInstanceReportImpl query, Page page)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		TenantManager.configureQuery(query);
		return DbEntityManager.selectList("selectFinishedCaseInstancesReportEntities", query, page);
	  }

	  public virtual long findCleanableHistoricCaseInstancesReportCountByCriteria(CleanableHistoricCaseInstanceReportImpl query)
	  {
		query.CurrentTimestamp = ClockUtil.CurrentTime;
		TenantManager.configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectFinishedCaseInstancesReportEntitiesCount", query).Value;
	  }

	}

}