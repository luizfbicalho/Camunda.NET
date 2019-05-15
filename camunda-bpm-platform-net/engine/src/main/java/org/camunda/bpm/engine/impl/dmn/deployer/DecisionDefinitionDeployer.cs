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
namespace org.camunda.bpm.engine.impl.dmn.deployer
{

	using DmnDecision = org.camunda.bpm.dmn.engine.DmnDecision;
	using DmnTransformer = org.camunda.bpm.dmn.engine.impl.spi.transform.DmnTransformer;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;

	/// <summary>
	/// <seealso cref="Deployer"/> responsible to parse DMN 1.1 XML files and create the proper
	/// <seealso cref="DecisionDefinitionEntity"/>s. Since it uses the result of the
	/// <seealso cref="DecisionRequirementsDefinitionDeployer"/> to avoid duplicated parsing, the DecisionRequirementsDefinitionDeployer must
	/// process the deployment before this cacheDeployer.
	/// </summary>
	public class DecisionDefinitionDeployer : AbstractDefinitionDeployer<DecisionDefinitionEntity>
	{

	  protected internal static readonly DecisionLogger LOG = ProcessEngineLogger.DECISION_LOGGER;

	  public static readonly string[] DMN_RESOURCE_SUFFIXES = new string[] {"dmn11.xml", "dmn"};

	  protected internal DmnTransformer transformer;

	  protected internal override string[] ResourcesSuffixes
	  {
		  get
		  {
			return DMN_RESOURCE_SUFFIXES;
		  }
	  }

	  protected internal override IList<DecisionDefinitionEntity> transformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Properties properties)
	  {
		IList<DecisionDefinitionEntity> decisions = new List<DecisionDefinitionEntity>();

		// get the decisions from the deployed drd instead of parse the DMN again
		DecisionRequirementsDefinitionEntity deployedDrd = findDeployedDrdForResource(deployment, resource.Name);

		if (deployedDrd == null)
		{
		  throw LOG.exceptionNoDrdForResource(resource.Name);
		}

		ICollection<DmnDecision> decisionsOfDrd = deployedDrd.Decisions;
		foreach (DmnDecision decisionOfDrd in decisionsOfDrd)
		{

		  DecisionDefinitionEntity decisionEntity = (DecisionDefinitionEntity) decisionOfDrd;
		  if (DecisionRequirementsDefinitionDeployer.isDecisionRequirementsDefinitionPersistable(deployedDrd))
		  {
			decisionEntity.DecisionRequirementsDefinitionId = deployedDrd.Id;
			decisionEntity.DecisionRequirementsDefinitionKey = deployedDrd.Key;
		  }

		  decisions.Add(decisionEntity);
		}

		if (!DecisionRequirementsDefinitionDeployer.isDecisionRequirementsDefinitionPersistable(deployedDrd))
		{
		  deployment.removeArtifact(deployedDrd);
		}

		return decisions;
	  }

	  protected internal virtual DecisionRequirementsDefinitionEntity findDeployedDrdForResource(DeploymentEntity deployment, string resourceName)
	  {
		IList<DecisionRequirementsDefinitionEntity> deployedDrds = deployment.getDeployedArtifacts(typeof(DecisionRequirementsDefinitionEntity));
		if (deployedDrds != null)
		{

		  foreach (DecisionRequirementsDefinitionEntity deployedDrd in deployedDrds)
		  {
			if (deployedDrd.ResourceName.Equals(resourceName))
			{
			  return deployedDrd;
			}
		  }
		}
		return null;
	  }

	  protected internal override DecisionDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return DecisionDefinitionManager.findDecisionDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }

	  protected internal override DecisionDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return DecisionDefinitionManager.findLatestDecisionDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  protected internal override void persistDefinition(DecisionDefinitionEntity definition)
	  {
		DecisionDefinitionManager.insertDecisionDefinition(definition);
	  }

	  protected internal override void addDefinitionToDeploymentCache(DeploymentCache deploymentCache, DecisionDefinitionEntity definition)
	  {
		deploymentCache.addDecisionDefinition(definition);
	  }

	  // context ///////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual DecisionDefinitionManager DecisionDefinitionManager
	  {
		  get
		  {
			return CommandContext.DecisionDefinitionManager;
		  }
	  }

	  // getters/setters ///////////////////////////////////////////////////////////////////////////////////

	  public virtual DmnTransformer Transformer
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