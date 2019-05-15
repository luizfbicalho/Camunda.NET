using System;
using System.Collections.Generic;
using System.IO;

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

	using DmnTransformer = org.camunda.bpm.dmn.engine.impl.spi.transform.DmnTransformer;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using DecisionRequirementsDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionManager;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;

	/// <summary>
	/// <seealso cref="Deployer"/> responsible to parse DMN 1.1 XML files and create the proper
	/// <seealso cref="DecisionRequirementsDefinitionEntity"/>s.
	/// </summary>
	public class DecisionRequirementsDefinitionDeployer : AbstractDefinitionDeployer<DecisionRequirementsDefinitionEntity>
	{

	  protected internal static readonly DecisionLogger LOG = ProcessEngineLogger.DECISION_LOGGER;

	  protected internal DmnTransformer transformer;

	  protected internal override string[] ResourcesSuffixes
	  {
		  get
		  {
			// since the DecisionDefinitionDeployer uses the result of this cacheDeployer, make sure that
			// it process the same DMN resources
			return DecisionDefinitionDeployer.DMN_RESOURCE_SUFFIXES;
		  }
	  }

	  protected internal override IList<DecisionRequirementsDefinitionEntity> transformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Properties properties)
	  {
		sbyte[] bytes = resource.Bytes;
		MemoryStream inputStream = new MemoryStream(bytes);

		try
		{
		  DecisionRequirementsDefinitionEntity drd = transformer.createTransform().modelInstance(inputStream).transformDecisionRequirementsGraph();

		  return Collections.singletonList(drd);

		}
		catch (Exception e)
		{
		  throw LOG.exceptionParseDmnResource(resource.Name, e);
		}
	  }

	  protected internal override DecisionRequirementsDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return DecisionRequirementsDefinitionManager.findDecisionRequirementsDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }

	  protected internal override DecisionRequirementsDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return DecisionRequirementsDefinitionManager.findLatestDecisionRequirementsDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  protected internal override void persistDefinition(DecisionRequirementsDefinitionEntity definition)
	  {
		if (isDecisionRequirementsDefinitionPersistable(definition))
		{
		  DecisionRequirementsDefinitionManager.insertDecisionRequirementsDefinition(definition);
		}
	  }

	  protected internal override void addDefinitionToDeploymentCache(DeploymentCache deploymentCache, DecisionRequirementsDefinitionEntity definition)
	  {
		if (isDecisionRequirementsDefinitionPersistable(definition))
		{
		  deploymentCache.addDecisionRequirementsDefinition(definition);
		}
	  }

	  protected internal override void ensureNoDuplicateDefinitionKeys(IList<DecisionRequirementsDefinitionEntity> definitions)
	  {
		// ignore decision requirements definitions which will not be persistent
		List<DecisionRequirementsDefinitionEntity> persistableDefinitions = new List<DecisionRequirementsDefinitionEntity>();
		foreach (DecisionRequirementsDefinitionEntity definition in definitions)
		{
		  if (isDecisionRequirementsDefinitionPersistable(definition))
		  {
			persistableDefinitions.Add(definition);
		  }
		}

		base.ensureNoDuplicateDefinitionKeys(persistableDefinitions);
	  }

	  public static bool isDecisionRequirementsDefinitionPersistable(DecisionRequirementsDefinitionEntity definition)
	  {
		// persist no decision requirements definition for a single decision
		return definition.Decisions.size() > 1;
	  }

	  protected internal override void updateDefinitionByPersistedDefinition(DeploymentEntity deployment, DecisionRequirementsDefinitionEntity definition, DecisionRequirementsDefinitionEntity persistedDefinition)
	  {
		// cannot update the definition if it is not persistent
		if (persistedDefinition != null)
		{
		  base.updateDefinitionByPersistedDefinition(deployment, definition, persistedDefinition);
		}
	  }

	  //context ///////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual DecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
	  {
		  get
		  {
			return CommandContext.DecisionRequirementsDefinitionManager;
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