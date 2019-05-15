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
namespace org.camunda.bpm.engine.impl.cmmn.deployer
{

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseDefinitionManager = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionManager;
	using CmmnTransformer = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformer;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;

	/// <summary>
	/// <seealso cref="Deployer"/> responsible to parse CMMN 1.0 XML files and create the
	/// proper <seealso cref="CaseDefinitionEntity"/>s.
	/// 
	/// @author Roman Smirnov
	/// @author Simon Zambrovski
	/// 
	/// </summary>
	public class CmmnDeployer : AbstractDefinitionDeployer<CaseDefinitionEntity>
	{

	  public static readonly string[] CMMN_RESOURCE_SUFFIXES = new string[] {"cmmn11.xml", "cmmn10.xml", "cmmn"};

	  protected internal ExpressionManager expressionManager;
	  protected internal CmmnTransformer transformer;

	  protected internal override string[] ResourcesSuffixes
	  {
		  get
		  {
			return CMMN_RESOURCE_SUFFIXES;
		  }
	  }

	  protected internal override IList<CaseDefinitionEntity> transformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Properties properties)
	  {
		return transformer.createTransform().deployment(deployment).resource(resource).transform();
	  }

	  protected internal override CaseDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return CaseDefinitionManager.findCaseDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }

	  protected internal override CaseDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return CaseDefinitionManager.findLatestCaseDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  protected internal override void persistDefinition(CaseDefinitionEntity definition)
	  {
		CaseDefinitionManager.insertCaseDefinition(definition);
	  }

	  protected internal override void addDefinitionToDeploymentCache(DeploymentCache deploymentCache, CaseDefinitionEntity definition)
	  {
		deploymentCache.addCaseDefinition(definition);
	  }

	  // context ///////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual CaseDefinitionManager CaseDefinitionManager
	  {
		  get
		  {
			return CommandContext.CaseDefinitionManager;
		  }
	  }

	  // getters/setters ///////////////////////////////////////////////////////////////////////////////////

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual CmmnTransformer Transformer
	  {
		  get
		  {
			return transformer;
		  }
		  set
		  {
			this.transformer = value;
		  }
	  }


	}

}