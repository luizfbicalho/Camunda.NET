using System;
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
namespace org.camunda.bpm.engine.impl
{

	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;

	/// <summary>
	/// <seealso cref="Deployer"/> responsible to parse resource files and create the proper entities.
	/// This class is extended by specific resource deployers.
	/// 
	/// Note: Implementations must be thread-safe. In particular they should not keep deployment-specific state.
	/// </summary>
	public abstract class AbstractDefinitionDeployer<DefinitionEntity> : Deployer where DefinitionEntity : org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity
	{

	  public static readonly string[] DIAGRAM_SUFFIXES = new string[] {"png", "jpg", "gif", "svg"};

	  private readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal IdGenerator idGenerator;

	  public virtual IdGenerator IdGenerator
	  {
		  get
		  {
			return idGenerator;
		  }
		  set
		  {
			this.idGenerator = value;
		  }
	  }


	  public virtual void deploy(DeploymentEntity deployment)
	  {
		LOG.debugProcessingDeployment(deployment.Name);
		Properties properties = new Properties();
		IList<DefinitionEntity> definitions = parseDefinitionResources(deployment, properties);
		ensureNoDuplicateDefinitionKeys(definitions);
		postProcessDefinitions(deployment, definitions, properties);
	  }

	  protected internal virtual IList<DefinitionEntity> parseDefinitionResources(DeploymentEntity deployment, Properties properties)
	  {
		IList<DefinitionEntity> definitions = new List<DefinitionEntity>();
		foreach (ResourceEntity resource in deployment.Resources.Values)
		{
		  LOG.debugProcessingResource(resource.Name);
		  if (isResourceHandled(resource))
		  {
			((IList<DefinitionEntity>)definitions).AddRange(transformResource(deployment, resource, properties));
		  }
		}
		return definitions;
	  }

	  protected internal virtual bool isResourceHandled(ResourceEntity resource)
	  {
		string resourceName = resource.Name;

		foreach (string suffix in ResourcesSuffixes)
		{
		  if (resourceName.EndsWith(suffix, StringComparison.Ordinal))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <returns> the list of resource suffixes for this cacheDeployer </returns>
	  protected internal abstract string[] ResourcesSuffixes {get;}

	  protected internal virtual ICollection<DefinitionEntity> transformResource(DeploymentEntity deployment, ResourceEntity resource, Properties properties)
	  {
		string resourceName = resource.Name;
		IList<DefinitionEntity> definitions = transformDefinitions(deployment, resource, properties);

		foreach (DefinitionEntity definition in definitions)
		{
		  definition.ResourceName = resourceName;

		  string diagramResourceName = getDiagramResourceForDefinition(deployment, resourceName, definition, deployment.Resources);
		  if (!string.ReferenceEquals(diagramResourceName, null))
		  {
			definition.DiagramResourceName = diagramResourceName;
		  }
		}

		return definitions;
	  }


	  /// <summary>
	  /// Transform the resource entity into definition entities.
	  /// </summary>
	  /// <param name="deployment"> the deployment the resources belongs to </param>
	  /// <param name="resource"> the resource to transform </param>
	  /// <returns> a list of transformed definition entities </returns>
	  protected internal abstract IList<DefinitionEntity> transformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Properties properties);

	  /// <summary>
	  /// Returns the default name of the image resource for a certain definition.
	  /// 
	  /// It will first look for an image resource which matches the definition
	  /// specifically, before resorting to an image resource which matches the file
	  /// containing the definition.
	  /// 
	  /// Example: if the deployment contains a BPMN 2.0 xml resource called
	  /// 'abc.bpmn20.xml' containing only one process with key 'myProcess', then
	  /// this method will look for an image resources called 'abc.myProcess.png'
	  /// (or .jpg, or .gif, etc.) or 'abc.png' if the previous one wasn't found.
	  /// 
	  /// Example 2: if the deployment contains a BPMN 2.0 xml resource called
	  /// 'abc.bpmn20.xml' containing three processes (with keys a, b and c),
	  /// then this method will first look for an image resource called 'abc.a.png'
	  /// before looking for 'abc.png' (likewise for b and c).
	  /// Note that if abc.a.png, abc.b.png and abc.c.png don't exist, all
	  /// processes will have the same image: abc.png.
	  /// </summary>
	  /// <returns> null if no matching image resource is found. </returns>
	  protected internal virtual string getDiagramResourceForDefinition(DeploymentEntity deployment, string resourceName, DefinitionEntity definition, IDictionary<string, ResourceEntity> resources)
	  {
		foreach (string diagramSuffix in DiagramSuffixes)
		{
		  string definitionDiagramResource = getDefinitionDiagramResourceName(resourceName, definition, diagramSuffix);
		  string diagramForFileResource = getGeneralDiagramResourceName(resourceName, definition, diagramSuffix);
		  if (resources.ContainsKey(definitionDiagramResource))
		  {
			return definitionDiagramResource;
		  }
		  else if (resources.ContainsKey(diagramForFileResource))
		  {
			return diagramForFileResource;
		  }
		}
		// no matching diagram found
		return null;
	  }

	  protected internal virtual string getDefinitionDiagramResourceName(string resourceName, DefinitionEntity definition, string diagramSuffix)
	  {
		string fileResourceBase = stripDefinitionFileSuffix(resourceName);
		string definitionKey = definition.Key;

		return fileResourceBase + definitionKey + "." + diagramSuffix;
	  }

	  protected internal virtual string getGeneralDiagramResourceName(string resourceName, DefinitionEntity definition, string diagramSuffix)
	  {
		string fileResourceBase = stripDefinitionFileSuffix(resourceName);

		return fileResourceBase + diagramSuffix;
	  }

	  protected internal virtual string stripDefinitionFileSuffix(string resourceName)
	  {
		foreach (string suffix in ResourcesSuffixes)
		{
		  if (resourceName.EndsWith(suffix, StringComparison.Ordinal))
		  {
			return resourceName.Substring(0, resourceName.Length - suffix.Length);
		  }
		}
		return resourceName;
	  }

	  protected internal virtual string[] DiagramSuffixes
	  {
		  get
		  {
			return DIAGRAM_SUFFIXES;
		  }
	  }

	  protected internal virtual void ensureNoDuplicateDefinitionKeys(IList<DefinitionEntity> definitions)
	  {
		ISet<string> keys = new HashSet<string>();

		foreach (DefinitionEntity definition in definitions)
		{

		  string key = definition.Key;

		  if (keys.Contains(key))
		  {
			throw new ProcessEngineException("The deployment contains definitions with the same key '" + key + "' (id attribute), this is not allowed");
		  }

		  keys.Add(key);
		}
	  }

	  protected internal virtual void postProcessDefinitions(DeploymentEntity deployment, IList<DefinitionEntity> definitions, Properties properties)
	  {
		if (deployment.New)
		{
		  // if the deployment is new persist the new definitions
		  persistDefinitions(deployment, definitions, properties);
		}
		else
		{
		  // if the current deployment is not a new one,
		  // then load the already existing definitions
		  loadDefinitions(deployment, definitions, properties);
		}
	  }

	  protected internal virtual void persistDefinitions(DeploymentEntity deployment, IList<DefinitionEntity> definitions, Properties properties)
	  {
		foreach (DefinitionEntity definition in definitions)
		{
		  string definitionKey = definition.Key;
		  string tenantId = deployment.TenantId;

		  DefinitionEntity latestDefinition = findLatestDefinitionByKeyAndTenantId(definitionKey, tenantId);

		  updateDefinitionByLatestDefinition(deployment, definition, latestDefinition);

		  persistDefinition(definition);
		  registerDefinition(deployment, definition, properties);
		}
	  }

	  protected internal virtual void updateDefinitionByLatestDefinition(DeploymentEntity deployment, DefinitionEntity definition, DefinitionEntity latestDefinition)
	  {
		definition.Version = getNextVersion(deployment, definition, latestDefinition);
		definition.Id = generateDefinitionId(deployment, definition, latestDefinition);
		definition.DeploymentId = deployment.Id;
		definition.TenantId = deployment.TenantId;
	  }

	  protected internal virtual void loadDefinitions(DeploymentEntity deployment, IList<DefinitionEntity> definitions, Properties properties)
	  {
		foreach (DefinitionEntity definition in definitions)
		{
		  string deploymentId = deployment.Id;
		  string definitionKey = definition.Key;

		  DefinitionEntity persistedDefinition = findDefinitionByDeploymentAndKey(deploymentId, definitionKey);
		  handlePersistedDefinition(definition, persistedDefinition, deployment, properties);
		}
	  }

	  protected internal virtual void handlePersistedDefinition(DefinitionEntity definition, DefinitionEntity persistedDefinition, DeploymentEntity deployment, Properties properties)
	  {
		persistedDefinitionLoaded(deployment, definition, persistedDefinition);
		updateDefinitionByPersistedDefinition(deployment, definition, persistedDefinition);
		registerDefinition(deployment, definition, properties);
	  }

	  protected internal virtual void updateDefinitionByPersistedDefinition(DeploymentEntity deployment, DefinitionEntity definition, DefinitionEntity persistedDefinition)
	  {
		definition.Version = persistedDefinition.Version;
		definition.Id = persistedDefinition.Id;
		definition.DeploymentId = deployment.Id;
		definition.TenantId = persistedDefinition.TenantId;
	  }

	  /// <summary>
	  /// Called when a previous version of a definition was loaded from the persistent store.
	  /// </summary>
	  /// <param name="deployment"> the deployment of the definition </param>
	  /// <param name="definition"> the definition entity </param>
	  /// <param name="persistedDefinition"> the loaded definition entity </param>
	  protected internal virtual void persistedDefinitionLoaded(DeploymentEntity deployment, DefinitionEntity definition, DefinitionEntity persistedDefinition)
	  {
		// do nothing;
	  }

	  /// <summary>
	  /// Find a definition entity by deployment id and definition key. </summary>
	  /// <param name="deploymentId"> the deployment id </param>
	  /// <param name="definitionKey"> the definition key </param>
	  /// <returns> the corresponding definition entity or null if non is found </returns>
	  protected internal abstract DefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey);

	  /// <summary>
	  /// Find the last deployed definition entity by definition key and tenant id.
	  /// </summary>
	  /// <returns> the corresponding definition entity or null if non is found </returns>
	  protected internal abstract DefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId);

	  /// <summary>
	  /// Persist definition entity into the database. </summary>
	  /// <param name="definition"> the definition entity </param>
	  protected internal abstract void persistDefinition(DefinitionEntity definition);

	  protected internal virtual void registerDefinition(DeploymentEntity deployment, DefinitionEntity definition, Properties properties)
	  {
		DeploymentCache deploymentCache = DeploymentCache;

		// Add to cache
		addDefinitionToDeploymentCache(deploymentCache, definition);

		definitionAddedToDeploymentCache(deployment, definition, properties);

		// Add to deployment for further usage
		deployment.addDeployedArtifact(definition);
	  }

	  /// <summary>
	  /// Add a definition to the deployment cache
	  /// </summary>
	  /// <param name="deploymentCache"> the deployment cache </param>
	  /// <param name="definition"> the definition to add </param>
	  protected internal abstract void addDefinitionToDeploymentCache(DeploymentCache deploymentCache, DefinitionEntity definition);

	  /// <summary>
	  /// Called after a definition was added to the deployment cache.
	  /// </summary>
	  /// <param name="deployment"> the deployment of the definition </param>
	  /// <param name="definition"> the definition entity </param>
	  protected internal virtual void definitionAddedToDeploymentCache(DeploymentEntity deployment, DefinitionEntity definition, Properties properties)
	  {
		// do nothing
	  }

	  /// <summary>
	  /// per default we increment the latest definition version by one - but you
	  /// might want to hook in some own logic here, e.g. to align definition
	  /// versions with deployment / build versions.
	  /// </summary>
	  protected internal virtual int getNextVersion(DeploymentEntity deployment, DefinitionEntity newDefinition, DefinitionEntity latestDefinition)
	  {
		int result = 1;
		if (latestDefinition != null)
		{
		  int latestVersion = latestDefinition.Version;
		  result = latestVersion + 1;
		}
		return result;
	  }

	  /// <summary>
	  /// create an id for the definition. The default is to ask the <seealso cref="IdGenerator"/>
	  /// and add the definition key and version if that does not exceed 64 characters.
	  /// You might want to hook in your own implementation here.
	  /// </summary>
	  protected internal virtual string generateDefinitionId(DeploymentEntity deployment, DefinitionEntity newDefinition, DefinitionEntity latestDefinition)
	  {
		string nextId = idGenerator.NextId;

		string definitionKey = newDefinition.Key;
		int definitionVersion = newDefinition.Version;

		string definitionId = definitionKey + ":" + definitionVersion + ":" + nextId;

		// ACT-115: maximum id length is 64 characters
		if (definitionId.Length > 64)
		{
		  definitionId = nextId;
		}
		return definitionId;
	  }

	  protected internal virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration;
		  }
	  }

	  protected internal virtual CommandContext CommandContext
	  {
		  get
		  {
			return Context.CommandContext;
		  }
	  }

	  protected internal virtual DeploymentCache DeploymentCache
	  {
		  get
		  {
			return ProcessEngineConfiguration.DeploymentCache;
		  }
	  }

	}

}