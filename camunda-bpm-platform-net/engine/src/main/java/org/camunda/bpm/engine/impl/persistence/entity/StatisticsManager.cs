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
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using BatchStatisticsQueryImpl = org.camunda.bpm.engine.impl.batch.BatchStatisticsQueryImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using HistoricDecisionInstanceStatistics = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatistics;
	using DeploymentStatistics = org.camunda.bpm.engine.management.DeploymentStatistics;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.DECISION_REQUIREMENTS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	public class StatisticsManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.management.ProcessDefinitionStatistics> getStatisticsGroupedByProcessDefinitionVersion(org.camunda.bpm.engine.impl.ProcessDefinitionStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<ProcessDefinitionStatistics> getStatisticsGroupedByProcessDefinitionVersion(ProcessDefinitionStatisticsQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectProcessDefinitionStatistics", query, page);
	  }

	  public virtual long getStatisticsCountGroupedByProcessDefinitionVersion(ProcessDefinitionStatisticsQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectProcessDefinitionStatisticsCount", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.management.ActivityStatistics> getStatisticsGroupedByActivity(org.camunda.bpm.engine.impl.ActivityStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<ActivityStatistics> getStatisticsGroupedByActivity(ActivityStatisticsQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectActivityStatistics", query, page);
	  }

	  public virtual long getStatisticsCountGroupedByActivity(ActivityStatisticsQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectActivityStatisticsCount", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.management.DeploymentStatistics> getStatisticsGroupedByDeployment(org.camunda.bpm.engine.impl.DeploymentStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<DeploymentStatistics> getStatisticsGroupedByDeployment(DeploymentStatisticsQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectDeploymentStatistics", query, page);
	  }

	  public virtual long getStatisticsCountGroupedByDeployment(DeploymentStatisticsQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectDeploymentStatisticsCount", query).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.batch.BatchStatistics> getStatisticsGroupedByBatch(org.camunda.bpm.engine.impl.batch.BatchStatisticsQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<BatchStatistics> getStatisticsGroupedByBatch(BatchStatisticsQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectBatchStatistics", query, page);
	  }

	  public virtual long getStatisticsCountGroupedByBatch(BatchStatisticsQueryImpl query)
	  {
		configureQuery(query);
		return (long?) DbEntityManager.selectOne("selectBatchStatisticsCount", query).Value;
	  }

	  protected internal virtual void configureQuery(DeploymentStatisticsQueryImpl query)
	  {
		AuthorizationManager.configureDeploymentStatisticsQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual void configureQuery(ProcessDefinitionStatisticsQueryImpl query)
	  {
		AuthorizationManager.configureProcessDefinitionStatisticsQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual void configureQuery(ActivityStatisticsQueryImpl query)
	  {
		checkReadProcessDefinition(query);
		AuthorizationManager.configureActivityStatisticsQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual void configureQuery(BatchStatisticsQueryImpl batchQuery)
	  {
		AuthorizationManager.configureBatchStatisticsQuery(batchQuery);
		TenantManager.configureQuery(batchQuery);
	  }

	  protected internal virtual void checkReadProcessDefinition(ActivityStatisticsQueryImpl query)
	  {
		CommandContext commandContext = CommandContext;
		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
		{
		  string processDefinitionId = query.ProcessDefinitionId;
		  ProcessDefinitionEntity definition = ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
		  ensureNotNull("no deployed process definition found with id '" + processDefinitionId + "'", "processDefinition", definition);
		  AuthorizationManager.checkAuthorization(READ, PROCESS_DEFINITION, definition.Key);
		}
	  }

	  public virtual long getStatisticsCountGroupedByDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl decisionRequirementsDefinitionStatisticsQuery)
	  {
		configureQuery(decisionRequirementsDefinitionStatisticsQuery);
		return (long?) DbEntityManager.selectOne("selectDecisionDefinitionStatisticsCount", decisionRequirementsDefinitionStatisticsQuery).Value;
	  }

	  protected internal virtual void configureQuery(HistoricDecisionInstanceStatisticsQueryImpl decisionRequirementsDefinitionStatisticsQuery)
	  {
		checkReadDecisionRequirementsDefinition(decisionRequirementsDefinitionStatisticsQuery);
		TenantManager.configureQuery(decisionRequirementsDefinitionStatisticsQuery);
	  }

	  protected internal virtual void checkReadDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl query)
	  {
		CommandContext commandContext = CommandContext;
		if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
		{
		  string decisionRequirementsDefinitionId = query.DecisionRequirementsDefinitionId;
		  DecisionRequirementsDefinition definition = DecisionRequirementsDefinitionManager.findDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);
		  ensureNotNull("no deployed decision requirements definition found with id '" + decisionRequirementsDefinitionId + "'", "decisionRequirementsDefinition", definition);
		  AuthorizationManager.checkAuthorization(READ, DECISION_REQUIREMENTS_DEFINITION, definition.Key);
		}
	  }

	  public virtual IList<HistoricDecisionInstanceStatistics> getStatisticsGroupedByDecisionRequirementsDefinition(HistoricDecisionInstanceStatisticsQueryImpl query, Page page)
	  {
		configureQuery(query);
		return DbEntityManager.selectList("selectDecisionDefinitionStatistics", query, page);
	  }
	}

}