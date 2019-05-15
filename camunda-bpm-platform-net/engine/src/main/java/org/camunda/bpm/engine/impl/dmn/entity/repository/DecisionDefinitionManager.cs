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
namespace org.camunda.bpm.engine.impl.dmn.entity.repository
{

	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using AbstractResourceDefinitionManager = org.camunda.bpm.engine.impl.persistence.AbstractResourceDefinitionManager;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	public class DecisionDefinitionManager : AbstractManager, AbstractResourceDefinitionManager<DecisionDefinitionEntity>
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public virtual void insertDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		DbEntityManager.insert(decisionDefinition);
		createDefaultAuthorizations(decisionDefinition);
	  }

	  public virtual void deleteDecisionDefinitionsByDeploymentId(string deploymentId)
	  {
		DbEntityManager.delete(typeof(DecisionDefinitionEntity), "deleteDecisionDefinitionsByDeploymentId", deploymentId);
	  }

	  public virtual DecisionDefinitionEntity findDecisionDefinitionById(string decisionDefinitionId)
	  {
		return DbEntityManager.selectById(typeof(DecisionDefinitionEntity), decisionDefinitionId);
	  }

	  /// <returns> the latest version of the decision definition with the given key (from any tenant)
	  /// </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a decision definition with the given key
	  /// </exception>
	  /// <seealso cref= #findLatestDecisionDefinitionByKeyAndTenantId(String, String) </seealso>
	  public virtual DecisionDefinitionEntity findLatestDecisionDefinitionByKey(string decisionDefinitionKey)
	  {
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<DecisionDefinitionEntity> decisionDefinitions = getDbEntityManager().selectList("selectLatestDecisionDefinitionByKey", configureParameterizedQuery(decisionDefinitionKey));
		IList<DecisionDefinitionEntity> decisionDefinitions = DbEntityManager.selectList("selectLatestDecisionDefinitionByKey", configureParameterizedQuery(decisionDefinitionKey));

		if (decisionDefinitions.Count == 0)
		{
		  return null;

		}
		else if (decisionDefinitions.Count == 1)
		{
		  return decisionDefinitions.GetEnumerator().next();

		}
		else
		{
		  throw LOG.multipleTenantsForDecisionDefinitionKeyException(decisionDefinitionKey);
		}
	  }

	  /// <returns> the latest version of the decision definition with the given key and tenant id
	  /// </returns>
	  /// <seealso cref= #findLatestDecisionDefinitionByKey(String) </seealso>
	  public virtual DecisionDefinitionEntity findLatestDecisionDefinitionByKeyAndTenantId(string decisionDefinitionKey, string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["decisionDefinitionKey"] = decisionDefinitionKey;
		parameters["tenantId"] = tenantId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectLatestDecisionDefinitionByKeyWithoutTenantId", parameters);
		}
		else
		{
		  return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectLatestDecisionDefinitionByKeyAndTenantId", parameters);
		}
	  }

	  public virtual DecisionDefinitionEntity findDecisionDefinitionByKeyAndVersion(string decisionDefinitionKey, int? decisionDefinitionVersion)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["decisionDefinitionVersion"] = decisionDefinitionVersion;
		parameters["decisionDefinitionKey"] = decisionDefinitionKey;
		return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectDecisionDefinitionByKeyAndVersion", configureParameterizedQuery(parameters));
	  }

	  public virtual DecisionDefinitionEntity findDecisionDefinitionByKeyVersionAndTenantId(string decisionDefinitionKey, int? decisionDefinitionVersion, string tenantId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["decisionDefinitionVersion"] = decisionDefinitionVersion;
		parameters["decisionDefinitionKey"] = decisionDefinitionKey;
		parameters["tenantId"] = tenantId;
		if (string.ReferenceEquals(tenantId, null))
		{
		  return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectDecisionDefinitionByKeyVersionWithoutTenantId", parameters);
		}
		else
		{
		  return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectDecisionDefinitionByKeyVersionAndTenantId", parameters);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public DecisionDefinitionEntity findDecisionDefinitionByKeyVersionTagAndTenantId(String decisionDefinitionKey, String decisionDefinitionVersionTag, String tenantId)
	  public virtual DecisionDefinitionEntity findDecisionDefinitionByKeyVersionTagAndTenantId(string decisionDefinitionKey, string decisionDefinitionVersionTag, string tenantId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["decisionDefinitionVersionTag"] = decisionDefinitionVersionTag;
		parameters["decisionDefinitionKey"] = decisionDefinitionKey;
		parameters["tenantId"] = tenantId;

		ListQueryParameterObject parameterObject = new ListQueryParameterObject();
		parameterObject.Parameter = parameters;

		IList<DecisionDefinitionEntity> decisionDefinitions = DbEntityManager.selectList("selectDecisionDefinitionByKeyVersionTag", parameterObject);

		if (decisionDefinitions.Count == 1)
		{
		  return decisionDefinitions[0];
		}
		else if (decisionDefinitions.Count == 0)
		{
		  return null;
		}
		else
		{
		  throw LOG.multipleDefinitionsForVersionTagException(decisionDefinitionKey, decisionDefinitionVersionTag);
		}
	  }

	  public virtual DecisionDefinitionEntity findDecisionDefinitionByDeploymentAndKey(string deploymentId, string decisionDefinitionKey)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["decisionDefinitionKey"] = decisionDefinitionKey;
		return (DecisionDefinitionEntity) DbEntityManager.selectOne("selectDecisionDefinitionByDeploymentAndKey", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.DecisionDefinition> findDecisionDefinitionsByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<DecisionDefinition> findDecisionDefinitionsByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery, Page page)
	  {
		configureDecisionDefinitionQuery(decisionDefinitionQuery);
		return DbEntityManager.selectList("selectDecisionDefinitionsByQueryCriteria", decisionDefinitionQuery, page);
	  }

	  public virtual long findDecisionDefinitionCountByQueryCriteria(DecisionDefinitionQueryImpl decisionDefinitionQuery)
	  {
		configureDecisionDefinitionQuery(decisionDefinitionQuery);
		return (long?) DbEntityManager.selectOne("selectDecisionDefinitionCountByQueryCriteria", decisionDefinitionQuery).Value;
	  }

	  public virtual string findPreviousDecisionDefinitionId(string decisionDefinitionKey, int? version, string tenantId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["key"] = decisionDefinitionKey;
		@params["version"] = version;
		@params["tenantId"] = tenantId;
		return (string) DbEntityManager.selectOne("selectPreviousDecisionDefinitionId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.DecisionDefinition> findDecisionDefinitionByDeploymentId(String deploymentId)
	  public virtual IList<DecisionDefinition> findDecisionDefinitionByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectDecisionDefinitionByDeploymentId", deploymentId);
	  }



	  protected internal virtual void createDefaultAuthorizations(DecisionDefinition decisionDefinition)
	  {
		if (AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newDecisionDefinition(decisionDefinition);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureDecisionDefinitionQuery(DecisionDefinitionQueryImpl query)
	  {
		AuthorizationManager.configureDecisionDefinitionQuery(query);
		TenantManager.configureQuery(query);
	  }

	  protected internal virtual ListQueryParameterObject configureParameterizedQuery(object parameter)
	  {
		return TenantManager.configureQuery(parameter);
	  }

	  public virtual DecisionDefinitionEntity findLatestDefinitionById(string id)
	  {
		return findDecisionDefinitionById(id);
	  }

	  public virtual DecisionDefinitionEntity findLatestDefinitionByKey(string key)
	  {
		return findLatestDecisionDefinitionByKey(key);
	  }

	  public virtual DecisionDefinitionEntity getCachedResourceDefinitionEntity(string definitionId)
	  {
		return DbEntityManager.getCachedEntity(typeof(DecisionDefinitionEntity), definitionId);
	  }

	  public virtual DecisionDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return findLatestDecisionDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  public virtual DecisionDefinitionEntity findDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
	  {
		return findDecisionDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
	  }

	  public virtual DecisionDefinitionEntity findDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId)
	  {
		return findDecisionDefinitionByKeyVersionTagAndTenantId(definitionKey, definitionVersionTag, tenantId);
	  }

	  public virtual DecisionDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return findDecisionDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }
	}

}