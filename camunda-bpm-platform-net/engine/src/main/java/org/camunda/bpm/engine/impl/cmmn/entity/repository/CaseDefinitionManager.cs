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
namespace org.camunda.bpm.engine.impl.cmmn.entity.repository
{
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using AbstractResourceDefinitionManager = org.camunda.bpm.engine.impl.persistence.AbstractResourceDefinitionManager;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionManager : AbstractManager, AbstractResourceDefinitionManager<CaseDefinitionEntity>
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public virtual void insertCaseDefinition(CaseDefinitionEntity caseDefinition)
	  {
		DbEntityManager.insert(caseDefinition);
	  }

	  public virtual void deleteCaseDefinitionsByDeploymentId(string deploymentId)
	  {
		DbEntityManager.delete(typeof(CaseDefinitionEntity), "deleteCaseDefinitionsByDeploymentId", deploymentId);
	  }

	  public virtual CaseDefinitionEntity findCaseDefinitionById(string caseDefinitionId)
	  {
		return DbEntityManager.selectById(typeof(CaseDefinitionEntity), caseDefinitionId);
	  }

	  /// <returns> the latest version of the case definition with the given key (from any tenant)
	  /// </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a case definition with the given key
	  /// </exception>
	  /// <seealso cref= #findLatestCaseDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual CaseDefinitionEntity findLatestCaseDefinitionByKey(string caseDefinitionKey)
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<CaseDefinitionEntity> caseDefinitions = getDbEntityManager().selectList("selectLatestCaseDefinitionByKey", configureParameterizedQuery(caseDefinitionKey));
		IList<CaseDefinitionEntity> caseDefinitions = DbEntityManager.selectList("selectLatestCaseDefinitionByKey", configureParameterizedQuery(caseDefinitionKey));

		if (caseDefinitions.Count == 0)
		{
		  return null;

		}
		else if (caseDefinitions.Count == 1)
		{
		  return caseDefinitions.GetEnumerator().next();

		}
		else
		{
		  throw LOG.multipleTenantsForCaseDefinitionKeyException(caseDefinitionKey);
		}
	  }

	  /// <returns> the latest version of the case definition with the given key and tenant id
	  /// </returns>
	  /// <seealso cref= #findLatestCaseDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual CaseDefinitionEntity findLatestCaseDefinitionByKeyAndTenantId(string caseDefinitionKey, string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["caseDefinitionKey"] = caseDefinitionKey;
		parameters["tenantId"] = tenantId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  return (CaseDefinitionEntity) DbEntityManager.selectOne("selectLatestCaseDefinitionByKeyWithoutTenantId", parameters);
		}
		else
		{
		  return (CaseDefinitionEntity) DbEntityManager.selectOne("selectLatestCaseDefinitionByKeyAndTenantId", parameters);
		}
	  }

	  public virtual CaseDefinitionEntity findCaseDefinitionByKeyVersionAndTenantId(string caseDefinitionKey, int? caseDefinitionVersion, string tenantId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["caseDefinitionVersion"] = caseDefinitionVersion;
		parameters["caseDefinitionKey"] = caseDefinitionKey;
		parameters["tenantId"] = tenantId;
		return (CaseDefinitionEntity) DbEntityManager.selectOne("selectCaseDefinitionByKeyVersionAndTenantId", parameters);
	  }

	  public virtual CaseDefinitionEntity findCaseDefinitionByDeploymentAndKey(string deploymentId, string caseDefinitionKey)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["caseDefinitionKey"] = caseDefinitionKey;
		return (CaseDefinitionEntity) DbEntityManager.selectOne("selectCaseDefinitionByDeploymentAndKey", parameters);
	  }

	  public virtual string findPreviousCaseDefinitionId(string caseDefinitionKey, int? version, string tenantId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["key"] = caseDefinitionKey;
		@params["version"] = version;
		@params["tenantId"] = tenantId;
		return (string) DbEntityManager.selectOne("selectPreviousCaseDefinitionId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.CaseDefinition> findCaseDefinitionsByQueryCriteria(CaseDefinitionQueryImpl caseDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CaseDefinition> findCaseDefinitionsByQueryCriteria(CaseDefinitionQueryImpl caseDefinitionQuery, Page page)
	  {
		configureCaseDefinitionQuery(caseDefinitionQuery);
		return DbEntityManager.selectList("selectCaseDefinitionsByQueryCriteria", caseDefinitionQuery, page);
	  }

	  public virtual long findCaseDefinitionCountByQueryCriteria(CaseDefinitionQueryImpl caseDefinitionQuery)
	  {
		configureCaseDefinitionQuery(caseDefinitionQuery);
		return (long?) DbEntityManager.selectOne("selectCaseDefinitionCountByQueryCriteria", caseDefinitionQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.CaseDefinition> findCaseDefinitionByDeploymentId(String deploymentId)
	  public virtual IList<CaseDefinition> findCaseDefinitionByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectCaseDefinitionByDeploymentId", deploymentId);
	  }

	  protected internal virtual void configureCaseDefinitionQuery(CaseDefinitionQueryImpl query)
	  {
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  public virtual CaseDefinitionEntity findLatestDefinitionByKey(string key)
	  {
		return findLatestCaseDefinitionByKey(key);
	  }

	  public virtual CaseDefinitionEntity findLatestDefinitionById(string id)
	  {
		return findCaseDefinitionById(id);
	  }

	  public virtual CaseDefinitionEntity getCachedResourceDefinitionEntity(string definitionId)
	  {
		return DbEntityManager.getCachedEntity(typeof(CaseDefinitionEntity), definitionId);
	  }

	  public virtual CaseDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return findLatestCaseDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  public virtual CaseDefinitionEntity findDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId)
	  {
		throw new System.NotSupportedException("Currently finding case definition by version tag and tenant is not implemented.");
	  }

	  public virtual CaseDefinitionEntity findDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
	  {
		return findCaseDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
	  }

	  public virtual CaseDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return findCaseDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }
	}

}