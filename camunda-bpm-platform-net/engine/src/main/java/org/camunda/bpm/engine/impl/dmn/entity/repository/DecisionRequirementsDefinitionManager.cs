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
	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;
	using AbstractResourceDefinitionManager = org.camunda.bpm.engine.impl.persistence.AbstractResourceDefinitionManager;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class DecisionRequirementsDefinitionManager : AbstractManager, AbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity>
	{

	  public virtual void insertDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
	  {
		DbEntityManager.insert(decisionRequirementsDefinition);
		createDefaultAuthorizations(decisionRequirementsDefinition);
	  }

	  public virtual void deleteDecisionRequirementsDefinitionsByDeploymentId(string deploymentId)
	  {
		DbEntityManager.delete(typeof(DecisionDefinitionEntity), "deleteDecisionRequirementsDefinitionsByDeploymentId", deploymentId);
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDecisionRequirementsDefinitionById(string decisionRequirementsDefinitionId)
	  {
		return DbEntityManager.selectById(typeof(DecisionRequirementsDefinitionEntity), decisionRequirementsDefinitionId);
	  }

	  public virtual string findPreviousDecisionRequirementsDefinitionId(string decisionRequirementsDefinitionKey, int? version, string tenantId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["key"] = decisionRequirementsDefinitionKey;
		@params["version"] = version;
		@params["tenantId"] = tenantId;
		return (string) DbEntityManager.selectOne("selectPreviousDecisionRequirementsDefinitionId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.DecisionRequirementsDefinition> findDecisionRequirementsDefinitionByDeploymentId(String deploymentId)
	  public virtual IList<DecisionRequirementsDefinition> findDecisionRequirementsDefinitionByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectDecisionRequirementsDefinitionByDeploymentId", deploymentId);
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDecisionRequirementsDefinitionByDeploymentAndKey(string deploymentId, string decisionRequirementsDefinitionKey)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["decisionRequirementsDefinitionKey"] = decisionRequirementsDefinitionKey;
		return (DecisionRequirementsDefinitionEntity) DbEntityManager.selectOne("selectDecisionRequirementsDefinitionByDeploymentAndKey", parameters);
	  }

	  /// <returns> the latest version of the decision requirements definition with the given key and tenant id </returns>
	  public virtual DecisionRequirementsDefinitionEntity findLatestDecisionRequirementsDefinitionByKeyAndTenantId(string decisionRequirementsDefinitionKey, string tenantId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["decisionRequirementsDefinitionKey"] = decisionRequirementsDefinitionKey;
		parameters["tenantId"] = tenantId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  return (DecisionRequirementsDefinitionEntity) DbEntityManager.selectOne("selectLatestDecisionRequirementsDefinitionByKeyWithoutTenantId", parameters);
		}
		else
		{
		  return (DecisionRequirementsDefinitionEntity) DbEntityManager.selectOne("selectLatestDecisionRequirementsDefinitionByKeyAndTenantId", parameters);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.repository.DecisionRequirementsDefinition> findDecisionRequirementsDefinitionsByQueryCriteria(DecisionRequirementsDefinitionQueryImpl query, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<DecisionRequirementsDefinition> findDecisionRequirementsDefinitionsByQueryCriteria(DecisionRequirementsDefinitionQueryImpl query, Page page)
	  {
		configureDecisionRequirementsDefinitionQuery(query);
		return DbEntityManager.selectList("selectDecisionRequirementsDefinitionsByQueryCriteria", query, page);
	  }

	  public virtual long findDecisionRequirementsDefinitionCountByQueryCriteria(DecisionRequirementsDefinitionQueryImpl query)
	  {
		configureDecisionRequirementsDefinitionQuery(query);
		return (long?) DbEntityManager.selectOne("selectDecisionRequirementsDefinitionCountByQueryCriteria", query).Value;
	  }

	  protected internal virtual void createDefaultAuthorizations(DecisionRequirementsDefinition decisionRequirementsDefinition)
	  {
		if (AuthorizationEnabled)
		{
		  ResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
		  AuthorizationEntity[] authorizations = provider.newDecisionRequirementsDefinition(decisionRequirementsDefinition);
		  saveDefaultAuthorizations(authorizations);
		}
	  }

	  protected internal virtual void configureDecisionRequirementsDefinitionQuery(DecisionRequirementsDefinitionQueryImpl query)
	  {
		AuthorizationManager.configureDecisionRequirementsDefinitionQuery(query);
		TenantManager.configureQuery(query);
	  }


	  public virtual DecisionRequirementsDefinitionEntity findLatestDefinitionByKey(string key)
	  {
		return null;
	  }

	  public virtual DecisionRequirementsDefinitionEntity findLatestDefinitionById(string id)
	  {
		return DbEntityManager.selectById(typeof(DecisionRequirementsDefinitionEntity), id);
	  }

	  public virtual DecisionRequirementsDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return null;
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
	  {
		return null;
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId)
	  {
		return null;
	  }

	  public virtual DecisionRequirementsDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return null;
	  }

	  public virtual DecisionRequirementsDefinitionEntity getCachedResourceDefinitionEntity(string definitionId)
	  {
		return DbEntityManager.getCachedEntity(typeof(DecisionRequirementsDefinitionEntity), definitionId);
	  }
	}

}