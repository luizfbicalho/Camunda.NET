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

	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class HistoricCaseActivityInstanceManager : AbstractHistoricManager
	{

	  public virtual void deleteHistoricCaseActivityInstancesByCaseInstanceIds(IList<string> historicCaseInstanceIds)
	  {
		if (HistoryEnabled)
		{
		  DbEntityManager.delete(typeof(HistoricCaseActivityInstanceEntity), "deleteHistoricCaseActivityInstancesByCaseInstanceIds", historicCaseInstanceIds);
		}
	  }

	  public virtual void insertHistoricCaseActivityInstance(HistoricCaseActivityInstanceEntity historicCaseActivityInstance)
	  {
		DbEntityManager.insert(historicCaseActivityInstance);
	  }

	  public virtual HistoricCaseActivityInstanceEntity findHistoricCaseActivityInstance(string caseActivityId, string caseInstanceId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["caseActivityId"] = caseActivityId;
		parameters["caseInstanceId"] = caseInstanceId;

		return (HistoricCaseActivityInstanceEntity) DbEntityManager.selectOne("selectHistoricCaseActivityInstance", parameters);
	  }

	  public virtual long findHistoricCaseActivityInstanceCountByQueryCriteria(HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery)
	  {
		configureHistoricCaseActivityInstanceQuery(historicCaseActivityInstanceQuery);
		return (long?) DbEntityManager.selectOne("selectHistoricCaseActivityInstanceCountByQueryCriteria", historicCaseActivityInstanceQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByQueryCriteria(org.camunda.bpm.engine.impl.HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByQueryCriteria(HistoricCaseActivityInstanceQueryImpl historicCaseActivityInstanceQuery, Page page)
	  {
		configureHistoricCaseActivityInstanceQuery(historicCaseActivityInstanceQuery);
		return DbEntityManager.selectList("selectHistoricCaseActivityInstancesByQueryCriteria", historicCaseActivityInstanceQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricCaseActivityInstance> findHistoricCaseActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbEntityManager.selectListWithRawParameter("selectHistoricCaseActivityInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricCaseActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricCaseActivityInstanceCountByNativeQuery", parameterMap).Value;
	  }

	  protected internal virtual void configureHistoricCaseActivityInstanceQuery(HistoricCaseActivityInstanceQueryImpl query)
	  {
		TenantManager.configureQuery(query);
	  }
	}

}