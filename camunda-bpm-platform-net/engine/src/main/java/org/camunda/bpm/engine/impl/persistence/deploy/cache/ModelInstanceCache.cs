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
	using GetDeploymentResourceCmd = org.camunda.bpm.engine.impl.cmd.GetDeploymentResourceCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using ResourceDefinition = org.camunda.bpm.engine.repository.ResourceDefinition;
	using ModelInstance = org.camunda.bpm.model.xml.ModelInstance;
	using Cache = org.camunda.commons.utils.cache.Cache;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public abstract class ModelInstanceCache<InstanceType, DefinitionType> where InstanceType : org.camunda.bpm.model.xml.ModelInstance where DefinitionType : org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal Cache<string, InstanceType> instanceCache;
	  protected internal ResourceDefinitionCache<DefinitionType> definitionCache;

	  public ModelInstanceCache(CacheFactory factory, int cacheCapacity, ResourceDefinitionCache<DefinitionType> definitionCache)
	  {
		this.instanceCache = factory.createCache(cacheCapacity);
		this.definitionCache = definitionCache;
	  }

	  public virtual InstanceType findBpmnModelInstanceForDefinition(DefinitionType definitionEntity)
	  {
		InstanceType bpmnModelInstance = instanceCache.get(definitionEntity.Id);
		if (bpmnModelInstance == null)
		{
		  bpmnModelInstance = loadAndCacheBpmnModelInstance(definitionEntity);
		}
		return bpmnModelInstance;
	  }

	  public virtual InstanceType findBpmnModelInstanceForDefinition(string definitionId)
	  {
		InstanceType bpmnModelInstance = instanceCache.get(definitionId);
		if (bpmnModelInstance == null)
		{
		  DefinitionType definition = definitionCache.findDeployedDefinitionById(definitionId);
		  bpmnModelInstance = loadAndCacheBpmnModelInstance(definition);
		}
		return bpmnModelInstance;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected InstanceType loadAndCacheBpmnModelInstance(final DefinitionType definitionEntity)
	  protected internal virtual InstanceType loadAndCacheBpmnModelInstance(DefinitionType definitionEntity)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;
		Stream bpmnResourceInputStream = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, definitionEntity, commandContext));

		try
		{
		  InstanceType bpmnModelInstance = readModelFromStream(bpmnResourceInputStream);
		  instanceCache.put(definitionEntity.Id, bpmnModelInstance);
		  return bpmnModelInstance;
		}
		catch (Exception e)
		{
		  throwLoadModelException(definitionEntity.Id, e);
		}
		return default(InstanceType);
	  }

	  private class CallableAnonymousInnerClass : Callable<Stream>
	  {
		  private readonly ModelInstanceCache<InstanceType, DefinitionType> outerInstance;

		  private ResourceDefinitionEntity definitionEntity;
		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(ModelInstanceCache<InstanceType, DefinitionType> outerInstance, ResourceDefinitionEntity definitionEntity, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.definitionEntity = definitionEntity;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream call() throws Exception
		  public Stream call()
		  {
			return (new GetDeploymentResourceCmd(definitionEntity.DeploymentId, definitionEntity.ResourceName)).execute(commandContext);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void removeAllDefinitionsByDeploymentId(final String deploymentId)
	  public virtual void removeAllDefinitionsByDeploymentId(string deploymentId)
	  {
		// remove all definitions for a specific deployment
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.camunda.bpm.engine.repository.ResourceDefinition> allDefinitionsForDeployment = getAllDefinitionsForDeployment(deploymentId);
		IList<ResourceDefinition> allDefinitionsForDeployment = getAllDefinitionsForDeployment(deploymentId);
		foreach (ResourceDefinition definition in allDefinitionsForDeployment)
		{
		  try
		  {
			instanceCache.remove(definition.Id);
			definitionCache.removeDefinitionFromCache(definition.Id);

		  }
		  catch (Exception e)
		  {
			logRemoveEntryFromDeploymentCacheFailure(definition.Id, e);
		  }
		}
	  }

	  public virtual void remove(string definitionId)
	  {
		instanceCache.remove(definitionId);
	  }

	  public virtual void clear()
	  {
		instanceCache.clear();
	  }

	  public virtual Cache<string, InstanceType> Cache
	  {
		  get
		  {
			return instanceCache;
		  }
	  }

	  protected internal abstract void throwLoadModelException(string definitionId, Exception e);

	  protected internal abstract void logRemoveEntryFromDeploymentCacheFailure(string definitionId, Exception e);

	  protected internal abstract InstanceType readModelFromStream(Stream stream);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.List<? extends org.camunda.bpm.engine.repository.ResourceDefinition> getAllDefinitionsForDeployment(String deploymentId);
	  protected internal abstract IList<ResourceDefinition> getAllDefinitionsForDeployment(string deploymentId);
	}

}