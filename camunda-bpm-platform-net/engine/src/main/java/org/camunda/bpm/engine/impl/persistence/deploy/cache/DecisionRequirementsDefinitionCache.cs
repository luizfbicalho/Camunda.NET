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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class DecisionRequirementsDefinitionCache : ResourceDefinitionCache<DecisionRequirementsDefinitionEntity>
	{

	  public DecisionRequirementsDefinitionCache(CacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer) : base(factory, cacheCapacity, cacheDeployer)
	  {
	  }

	  protected internal override AbstractResourceDefinitionManager<DecisionRequirementsDefinitionEntity> Manager
	  {
		  get
		  {
			return Context.CommandContext.DecisionRequirementsDefinitionManager;
		  }
	  }

	  protected internal override void checkInvalidDefinitionId(string definitionId)
	  {
		ensureNotNull("Invalid decision requirements definition id", "decisionRequirementsDefinitionId", definitionId);
	  }

	  protected internal override void checkDefinitionFound(string definitionId, DecisionRequirementsDefinitionEntity definition)
	  {
		ensureNotNull("no deployed decision requirements definition found with id '" + definitionId + "'", "decisionRequirementsDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKey(string definitionKey, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void checkInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void checkInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, DecisionRequirementsDefinitionEntity definition)
	  {
		// not needed
	  }

	  protected internal override void checkInvalidDefinitionWasCached(string deploymentId, string definitionId, DecisionRequirementsDefinitionEntity definition)
	  {
		ensureNotNull("deployment '" + deploymentId + "' didn't put decision requirements definition '" + definitionId + "' in the cache", "cachedDecisionRequirementsDefinition", definition);
	  }
	}

}