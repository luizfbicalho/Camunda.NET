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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using Cache = org.camunda.commons.utils.cache.Cache;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public abstract class ResourceDefinitionCache<T> where T : org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity
	{

	  protected internal Cache<string, T> cache;
	  protected internal CacheDeployer cacheDeployer;

	  public ResourceDefinitionCache(CacheFactory factory, int cacheCapacity, CacheDeployer cacheDeployer)
	  {
		this.cache = factory.createCache(cacheCapacity);
		this.cacheDeployer = cacheDeployer;
	  }

	  public virtual T findDefinitionFromCache(string definitionId)
	  {
		return cache.get(definitionId);
	  }

	  public virtual T findDeployedDefinitionById(string definitionId)
	  {
		checkInvalidDefinitionId(definitionId);
		T definition = Manager.getCachedResourceDefinitionEntity(definitionId);
		if (definition == null)
		{
		  definition = Manager.findLatestDefinitionById(definitionId);
		}

		checkDefinitionFound(definitionId, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  /// <returns> the latest version of the definition with the given key (from any tenant) </returns>
	  /// <exception cref="ProcessEngineException"> if more than one tenant has a definition with the given key </exception>
	  public virtual T findDeployedLatestDefinitionByKey(string definitionKey)
	  {
		T definition = Manager.findLatestDefinitionByKey(definitionKey);
		checkInvalidDefinitionByKey(definitionKey, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  public virtual T findDeployedLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		T definition = Manager.findLatestDefinitionByKeyAndTenantId(definitionKey, tenantId);
		checkInvalidDefinitionByKeyAndTenantId(definitionKey, tenantId, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public T findDeployedDefinitionByKeyVersionAndTenantId(final String definitionKey, final System.Nullable<int> definitionVersion, final String tenantId)
	  public virtual T findDeployedDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;
		T definition = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, definitionKey, definitionVersion, tenantId));
		checkInvalidDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  private class CallableAnonymousInnerClass : Callable<T>
	  {
		  private readonly ResourceDefinitionCache<T> outerInstance;

		  private string definitionKey;
		  private int? definitionVersion;
		  private string tenantId;

		  public CallableAnonymousInnerClass(ResourceDefinitionCache<T> outerInstance, string definitionKey, int? definitionVersion, string tenantId)
		  {
			  this.outerInstance = outerInstance;
			  this.definitionKey = definitionKey;
			  this.definitionVersion = definitionVersion;
			  this.tenantId = tenantId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T call() throws Exception
		  public T call()
		  {
			return outerInstance.Manager.findDefinitionByKeyVersionAndTenantId(definitionKey, definitionVersion, tenantId);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public T findDeployedDefinitionByKeyVersionTagAndTenantId(final String definitionKey, final String definitionVersionTag, final String tenantId)
	  public virtual T findDeployedDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;
		T definition = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, definitionKey, definitionVersionTag, tenantId));
		checkInvalidDefinitionByKeyVersionTagAndTenantId(definitionKey, definitionVersionTag, tenantId, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  private class CallableAnonymousInnerClass2 : Callable<T>
	  {
		  private readonly ResourceDefinitionCache<T> outerInstance;

		  private string definitionKey;
		  private string definitionVersionTag;
		  private string tenantId;

		  public CallableAnonymousInnerClass2(ResourceDefinitionCache<T> outerInstance, string definitionKey, string definitionVersionTag, string tenantId)
		  {
			  this.outerInstance = outerInstance;
			  this.definitionKey = definitionKey;
			  this.definitionVersionTag = definitionVersionTag;
			  this.tenantId = tenantId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T call() throws Exception
		  public T call()
		  {
			return outerInstance.Manager.findDefinitionByKeyVersionTagAndTenantId(definitionKey, definitionVersionTag, tenantId);
		  }
	  }

	  public virtual T findDeployedDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		T definition = Manager.findDefinitionByDeploymentAndKey(deploymentId, definitionKey);
		checkInvalidDefinitionByDeploymentAndKey(deploymentId, definitionKey, definition);
		definition = resolveDefinition(definition);
		return definition;
	  }

	  public virtual T resolveDefinition(T definition)
	  {
		string definitionId = definition.Id;
		string deploymentId = definition.DeploymentId;
		T cachedDefinition = cache.get(definitionId);
		if (cachedDefinition == null)
		{
		  lock (this)
		  {
			cachedDefinition = cache.get(definitionId);
			if (cachedDefinition == null)
			{
			  DeploymentEntity deployment = Context.CommandContext.DeploymentManager.findDeploymentById(deploymentId);
			  deployment.New = false;
			  cacheDeployer.deployOnlyGivenResourcesOfDeployment(deployment, definition.ResourceName, definition.DiagramResourceName);
			  cachedDefinition = cache.get(definitionId);
			}
		  }
		  checkInvalidDefinitionWasCached(deploymentId, definitionId, cachedDefinition);
		}
		if (cachedDefinition != null)
		{
		  cachedDefinition.updateModifiableFieldsFromEntity(definition);
		}
		return cachedDefinition;
	  }

	  public virtual void addDefinition(T definition)
	  {
		cache.put(definition.Id, definition);
	  }

	  public virtual T getDefinition(string id)
	  {
		return cache.get(id);
	  }

	  public virtual void removeDefinitionFromCache(string id)
	  {
		cache.remove(id);
	  }

	  public virtual void clear()
	  {
		cache.clear();
	  }

	  public virtual Cache<string, T> Cache
	  {
		  get
		  {
			return cache;
		  }
	  }

	  protected internal abstract AbstractResourceDefinitionManager<T> Manager {get;}

	  protected internal abstract void checkInvalidDefinitionId(string definitionId);

	  protected internal abstract void checkDefinitionFound(string definitionId, T definition);

	  protected internal abstract void checkInvalidDefinitionByKey(string definitionKey, T definition);

	  protected internal abstract void checkInvalidDefinitionByKeyAndTenantId(string definitionKey, string tenantId, T definition);

	  protected internal abstract void checkInvalidDefinitionByKeyVersionAndTenantId(string definitionKey, int? definitionVersion, string tenantId, T definition);

	  protected internal abstract void checkInvalidDefinitionByKeyVersionTagAndTenantId(string definitionKey, string definitionVersionTag, string tenantId, T definition);

	  protected internal abstract void checkInvalidDefinitionByDeploymentAndKey(string deploymentId, string definitionKey, T definition);

	  protected internal abstract void checkInvalidDefinitionWasCached(string deploymentId, string definitionId, T definition);

	}
}