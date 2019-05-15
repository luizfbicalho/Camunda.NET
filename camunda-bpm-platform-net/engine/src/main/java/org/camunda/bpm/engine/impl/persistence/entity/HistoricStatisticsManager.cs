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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;


	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricStatisticsManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricActivityStatistics> getHistoricStatisticsGroupedByActivity(org.camunda.bpm.engine.impl.HistoricActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricActivityStatistics> getHistoricStatisticsGroupedByActivity(HistoricActivityStatisticsQueryImpl query, Page page)
	  {
		if (ensureHistoryReadOnProcessDefinition(query))
		{
		  return DbEntityManager.selectList("selectHistoricActivityStatistics", query, page);
		}
		else
		{
		  return new List<HistoricActivityStatistics>();
		}
	  }

	  public virtual long getHistoricStatisticsCountGroupedByActivity(HistoricActivityStatisticsQueryImpl query)
	  {
		if (ensureHistoryReadOnProcessDefinition(query))
		{
		  return (long?) DbEntityManager.selectOne("selectHistoricActivityStatisticsCount", query).Value;
		}
		else
		{
		  return 0;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.history.HistoricCaseActivityStatistics> getHistoricStatisticsGroupedByCaseActivity(org.camunda.bpm.engine.impl.HistoricCaseActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<HistoricCaseActivityStatistics> getHistoricStatisticsGroupedByCaseActivity(HistoricCaseActivityStatisticsQueryImpl query, Page page)
	  {
		return DbEntityManager.selectList("selectHistoricCaseActivityStatistics", query, page);
	  }

	  public virtual long getHistoricStatisticsCountGroupedByCaseActivity(HistoricCaseActivityStatisticsQueryImpl query)
	  {
		return (long?) DbEntityManager.selectOne("selectHistoricCaseActivityStatisticsCount", query).Value;
	  }

	  protected internal virtual bool ensureHistoryReadOnProcessDefinition(HistoricActivityStatisticsQueryImpl query)
	  {
		CommandContext commandContext = CommandContext;

		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
		{
		  string processDefinitionId = query.ProcessDefinitionId;
		  ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);

		  if (definition == null)
		  {
			return false;
		  }

		  return AuthorizationManager.isAuthorized(READ_HISTORY, PROCESS_DEFINITION, definition.Key);
		}

		return true;
	  }

	}

}