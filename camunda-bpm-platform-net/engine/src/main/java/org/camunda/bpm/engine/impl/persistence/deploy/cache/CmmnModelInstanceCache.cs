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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using DecisionDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionQueryImpl;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class CmmnModelInstanceCache : ModelInstanceCache<CmmnModelInstance, CaseDefinitionEntity>
	{

	  public CmmnModelInstanceCache(CacheFactory factory, int cacheCapacity, ResourceDefinitionCache<CaseDefinitionEntity> definitionCache) : base(factory, cacheCapacity, definitionCache)
	  {
	  }

	  protected internal override void throwLoadModelException(string definitionId, Exception e)
	  {
		throw LOG.loadModelException("CMMN", "case", definitionId, e);
	  }

	  protected internal override CmmnModelInstance readModelFromStream(Stream cmmnResourceInputStream)
	  {
		return Cmmn.readModelFromStream(cmmnResourceInputStream);
	  }

	  protected internal override void logRemoveEntryFromDeploymentCacheFailure(string definitionId, Exception e)
	  {
		LOG.removeEntryFromDeploymentCacheFailure("case", definitionId, e);
	  }

	  protected internal override IList<DecisionDefinition> getAllDefinitionsForDeployment(string deploymentId)
	  {
		return (new DecisionDefinitionQueryImpl()).deploymentId(deploymentId).list();
	  }
	}

}