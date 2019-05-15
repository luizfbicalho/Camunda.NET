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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionManager : AbstractManager
	{

	  public virtual void insertCaseExecution(CaseExecutionEntity caseExecution)
	  {
		DbEntityManager.insert(caseExecution);
	  }

	  public virtual void deleteCaseExecution(CaseExecutionEntity caseExecution)
	  {
		DbEntityManager.delete(caseExecution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteCaseInstancesByCaseDefinition(String caseDefinitionId, String deleteReason, boolean cascade)
	  public virtual void deleteCaseInstancesByCaseDefinition(string caseDefinitionId, string deleteReason, bool cascade)
	  {
		IList<string> caseInstanceIds = DbEntityManager.selectList("selectCaseInstanceIdsByCaseDefinitionId", caseDefinitionId);

		foreach (string caseInstanceId in caseInstanceIds)
		{
		  deleteCaseInstance(caseInstanceId, deleteReason, cascade);
		}

		if (cascade)
		{
		  Context.CommandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstanceByCaseDefinitionId(caseDefinitionId);
		}

	  }

	  public virtual void deleteCaseInstance(string caseInstanceId, string deleteReason)
	  {
		deleteCaseInstance(caseInstanceId, deleteReason, false);
	  }

	  public virtual void deleteCaseInstance(string caseInstanceId, string deleteReason, bool cascade)
	  {
		CaseExecutionEntity execution = findCaseExecutionById(caseInstanceId);

		if (execution == null)
		{
		  throw new BadUserRequestException("No case instance found for id '" + caseInstanceId + "'");
		}

		CommandContext commandContext = Context.CommandContext;
		commandContext.TaskManager.deleteTasksByCaseInstanceId(caseInstanceId, deleteReason, cascade);

		execution.deleteCascade();

		if (cascade)
		{
		  Context.CommandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstancesByIds(Arrays.asList(caseInstanceId));
		}
	  }

	  public virtual CaseExecutionEntity findCaseExecutionById(string caseExecutionId)
	  {
		return DbEntityManager.selectById(typeof(CaseExecutionEntity), caseExecutionId);
	  }

	  public virtual CaseExecutionEntity findSubCaseInstanceBySuperCaseExecutionId(string superCaseExecutionId)
	  {
		return (CaseExecutionEntity) DbEntityManager.selectOne("selectSubCaseInstanceBySuperCaseExecutionId", superCaseExecutionId);
	  }

	  public virtual CaseExecutionEntity findSubCaseInstanceBySuperExecutionId(string superExecutionId)
	  {
		return (CaseExecutionEntity) DbEntityManager.selectOne("selectSubCaseInstanceBySuperExecutionId", superExecutionId);
	  }

	  public virtual long findCaseExecutionCountByQueryCriteria(CaseExecutionQueryImpl caseExecutionQuery)
	  {
		configureTenantCheck(caseExecutionQuery);
		return (long?) DbEntityManager.selectOne("selectCaseExecutionCountByQueryCriteria", caseExecutionQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.CaseExecution> findCaseExecutionsByQueryCriteria(CaseExecutionQueryImpl caseExecutionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CaseExecution> findCaseExecutionsByQueryCriteria(CaseExecutionQueryImpl caseExecutionQuery, Page page)
	  {
		configureTenantCheck(caseExecutionQuery);
		return DbEntityManager.selectList("selectCaseExecutionsByQueryCriteria", caseExecutionQuery, page);
	  }

	  public virtual long findCaseInstanceCountByQueryCriteria(CaseInstanceQueryImpl caseInstanceQuery)
	  {
		configureTenantCheck(caseInstanceQuery);
		return (long?) DbEntityManager.selectOne("selectCaseInstanceCountByQueryCriteria", caseInstanceQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.runtime.CaseInstance> findCaseInstanceByQueryCriteria(CaseInstanceQueryImpl caseInstanceQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CaseInstance> findCaseInstanceByQueryCriteria(CaseInstanceQueryImpl caseInstanceQuery, Page page)
	  {
		configureTenantCheck(caseInstanceQuery);
		return DbEntityManager.selectList("selectCaseInstanceByQueryCriteria", caseInstanceQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<CaseExecutionEntity> findChildCaseExecutionsByParentCaseExecutionId(String parentCaseExecutionId)
	  public virtual IList<CaseExecutionEntity> findChildCaseExecutionsByParentCaseExecutionId(string parentCaseExecutionId)
	  {
		return DbEntityManager.selectList("selectCaseExecutionsByParentCaseExecutionId", parentCaseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<CaseExecutionEntity> findChildCaseExecutionsByCaseInstanceId(String caseInstanceId)
	  public virtual IList<CaseExecutionEntity> findChildCaseExecutionsByCaseInstanceId(string caseInstanceId)
	  {
		return DbEntityManager.selectList("selectCaseExecutionsByCaseInstanceId", caseInstanceId);
	  }

	  protected internal virtual void configureTenantCheck<T1>(AbstractQuery<T1> query)
	  {
		TenantManager.configureQuery(query);
	  }

	}

}