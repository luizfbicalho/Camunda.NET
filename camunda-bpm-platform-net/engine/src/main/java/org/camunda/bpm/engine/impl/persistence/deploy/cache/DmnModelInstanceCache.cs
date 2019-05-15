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
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionDefinitionQueryImpl = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionQueryImpl;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Dmn = org.camunda.bpm.model.dmn.Dmn;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class DmnModelInstanceCache : ModelInstanceCache<DmnModelInstance, DecisionDefinitionEntity>
	{

	  public DmnModelInstanceCache(CacheFactory factory, int cacheCapacity, ResourceDefinitionCache<DecisionDefinitionEntity> definitionCache) : base(factory, cacheCapacity, definitionCache)
	  {
	  }

	  protected internal override void throwLoadModelException(string definitionId, Exception e)
	  {
		throw LOG.loadModelException("DMN", "decision", definitionId, e);
	  }

	  protected internal override DmnModelInstance readModelFromStream(Stream cmmnResourceInputStream)
	  {
		return Dmn.readModelFromStream(cmmnResourceInputStream);
	  }

	  protected internal override void logRemoveEntryFromDeploymentCacheFailure(string definitionId, Exception e)
	  {
		LOG.removeEntryFromDeploymentCacheFailure("decision", definitionId, e);
	  }

	  protected internal override IList<DecisionDefinition> getAllDefinitionsForDeployment(string deploymentId)
	  {
		return (new DecisionDefinitionQueryImpl()).deploymentId(deploymentId).list();
	  }
	}

}