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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using DecisionDefinitionNotFoundException = org.camunda.bpm.engine.exception.dmn.DecisionDefinitionNotFoundException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class DecisionDefinitionCache : ResourceDefinitionCache<DecisionDefinitionEntity>
	{


	  public DecisionDefinitionCache(CacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer) : base(factory, cacheCapacity, cacheDeployer)
	  {
	  }

	  public virtual DecisionDefinitionEntity findDeployedDefinitionByKeyAndVersion(string definitionKey, int? definitionVersion)
	  {
		DecisionDefinitionEntity definition = ((DecisionDefinitionManager) Manager).findDecisionDefinitionByKeyAndVersion(definitionKey, definitionVersion);

		checkInvalidDefinitionByKeyAndVersion(definitionKey, definitionVersion, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  protected internal override AbstractResourceDefinitionManager<DecisionDefinitionEntity> Manager
	  {
		  get
		  {
			return Context.CommandContext.DecisionDefinitionManager;
		  }
	  }

	  protected internal override void checkInvalidDefinitionId(string definitionId)
	  {
		ensureNotNull("Invalid decision definition id", "decisionDefinitionId", definitionId);
	  }

	  protected internal override void checkDefinitionFound(string definitionId, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no deployed decision definition found with id '" + definitionId + "'", "decisionDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKey(string definitionKey, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key '" + definitionKey + "'", "decisionDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key '" + definitionKey + "' and tenant-id '" + tenantId + "'", "decisionDefinition", definition);
	  }

	  protected internal virtual void checkInvalidDefinitionByKeyAndVersion(string decisionDefinitionKey, int? decisionDefinitionVersion, DecisionDefinitionEntity decisionDefinition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key = '" + decisionDefinitionKey + "' and version = '" + decisionDefinitionVersion + "'", "decisionDefinition", decisionDefinition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key = '" + definitionKey + "', version = '" + definitionVersion + "' and tenant-id '" + tenantId + "'", "decisionDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key = '" + definitionKey + "', versionTag = '" + definitionVersionTag + "' and tenant-id '" + tenantId + "'", "decisionDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, DecisionDefinitionEntity definition)
	  {
		ensureNotNull(typeof(DecisionDefinitionNotFoundException), "no decision definition deployed with key = '" + definitionKey + "' in deployment = '" + deploymentId + "'", "decisionDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionWasCached(string deploymentId, string definitionId, DecisionDefinitionEntity definition)
	  {
		ensureNotNull("deployment '" + deploymentId + "' didn't put decision definition '" + definitionId + "' in the cache", "cachedDecisionDefinition", definition);
	  }
	}

}