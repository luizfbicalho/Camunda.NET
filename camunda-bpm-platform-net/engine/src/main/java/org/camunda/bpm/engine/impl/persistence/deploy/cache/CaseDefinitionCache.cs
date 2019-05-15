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
	using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class CaseDefinitionCache : ResourceDefinitionCache<CaseDefinitionEntity>
	{

	  public CaseDefinitionCache(CacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer) : base(factory, cacheCapacity, cacheDeployer)
	  {
	  }

	  public virtual CaseDefinitionEntity getCaseDefinitionById(string caseDefinitionId)
	  {
		checkInvalidDefinitionId(caseDefinitionId);
		CaseDefinitionEntity caseDefinition = getDefinition(caseDefinitionId);
		if (caseDefinition == null)
		{
		  caseDefinition = findDeployedDefinitionById(caseDefinitionId);

		}
		return caseDefinition;
	  }

	  protected internal override AbstractResourceDefinitionManager<CaseDefinitionEntity> Manager
	  {
		  get
		  {
			return Context.CommandContext.CaseDefinitionManager;
		  }
	  }

	  protected internal override void checkInvalidDefinitionId(string definitionId)
	  {
		ensureNotNull("Invalid case definition id", "caseDefinitionId", definitionId);
	  }

	  protected internal override void checkDefinitionFound(string definitionId, CaseDefinitionEntity definition)
	  {
		ensureNotNull(typeof(CaseDefinitionNotFoundException), "no deployed case definition found with id '" + definitionId + "'", "caseDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKey(string definitionKey, CaseDefinitionEntity definition)
	  {
		ensureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key '" + definitionKey + "'", "caseDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, CaseDefinitionEntity definition)
	  {
		ensureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key '" + definitionKey + "' and tenant-id '" + tenantId + "'", "caseDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, CaseDefinitionEntity definition)
	  {
		ensureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key = '" + definitionKey + "', version = '" + definitionVersion + "'" + " and tenant-id = '" + tenantId + "'", "caseDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId, CaseDefinitionEntity definition)
	  {
		throw new System.NotSupportedException("Version tag is not implemented in case definition.");
	  }

	  protected internal override void checkInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, CaseDefinitionEntity definition)
	  {
		ensureNotNull(typeof(CaseDefinitionNotFoundException), "no case definition deployed with key = '" + definitionKey + "' in deployment = '" + deploymentId + "'", "caseDefinition", definition);
	  }

	  protected internal override void checkInvalidDefinitionWasCached(string deploymentId, string definitionId, CaseDefinitionEntity definition)
	  {
		ensureNotNull("deployment '" + deploymentId + "' didn't put case definition '" + definitionId + "' in the cache", "cachedCaseDefinition", definition);
	  }
	}

}