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
namespace org.camunda.bpm.engine.impl.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CmmnDeployer = org.camunda.bpm.engine.impl.cmmn.deployer.CmmnDeployer;
	using DecisionDefinitionDeployer = org.camunda.bpm.engine.impl.dmn.deployer.DecisionDefinitionDeployer;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Dmn = org.camunda.bpm.model.dmn.Dmn;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeploymentBuilderImpl : DeploymentBuilder
	{

	  private const long serialVersionUID = 1L;

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  [NonSerialized]
	  protected internal RepositoryServiceImpl repositoryService;
	  protected internal DeploymentEntity deployment = new DeploymentEntity();
	  protected internal bool isDuplicateFilterEnabled = false;
	  protected internal bool deployChangedOnly = false;
	  protected internal DateTime processDefinitionsActivationDate;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string nameFromDeployment_Renamed;
	  protected internal ISet<string> deployments = new HashSet<string>();
	  protected internal IDictionary<string, ISet<string>> deploymentResourcesById = new Dictionary<string, ISet<string>>();
	  protected internal IDictionary<string, ISet<string>> deploymentResourcesByName = new Dictionary<string, ISet<string>>();

	  public DeploymentBuilderImpl(RepositoryServiceImpl repositoryService)
	  {
		this.repositoryService = repositoryService;
	  }

	  public virtual DeploymentBuilder addInputStream(string resourceName, Stream inputStream)
	  {
		ensureNotNull("inputStream for resource '" + resourceName + "' is null", "inputStream", inputStream);
		sbyte[] bytes = IoUtil.readInputStream(inputStream, resourceName);

		return addBytes(resourceName, bytes);
	  }

	  public virtual DeploymentBuilder addClasspathResource(string resource)
	  {
		Stream inputStream = ReflectUtil.getResourceAsStream(resource);
		ensureNotNull("resource '" + resource + "' not found", "inputStream", inputStream);
		return addInputStream(resource, inputStream);
	  }

	  public virtual DeploymentBuilder addString(string resourceName, string text)
	  {
		ensureNotNull("text", text);

		sbyte[] bytes = (repositoryService != null && repositoryService.DeploymentCharset != null) ? text.GetBytes(repositoryService.DeploymentCharset) : text.GetBytes();

		return addBytes(resourceName, bytes);
	  }

	  public virtual DeploymentBuilder addModelInstance(string resourceName, CmmnModelInstance modelInstance)
	  {
		ensureNotNull("modelInstance", modelInstance);

		validateResouceName(resourceName, CmmnDeployer.CMMN_RESOURCE_SUFFIXES);

		MemoryStream outputStream = new MemoryStream();
		Cmmn.writeModelToStream(outputStream, modelInstance);

		return addBytes(resourceName, outputStream.toByteArray());
	  }

	  public virtual DeploymentBuilder addModelInstance(string resourceName, BpmnModelInstance modelInstance)
	  {
		ensureNotNull("modelInstance", modelInstance);

		validateResouceName(resourceName, BpmnDeployer.BPMN_RESOURCE_SUFFIXES);

		MemoryStream outputStream = new MemoryStream();
		Bpmn.writeModelToStream(outputStream, modelInstance);

		return addBytes(resourceName, outputStream.toByteArray());
	  }

	  public virtual DeploymentBuilder addModelInstance(string resourceName, DmnModelInstance modelInstance)
	  {
		ensureNotNull("modelInstance", modelInstance);

		validateResouceName(resourceName, DecisionDefinitionDeployer.DMN_RESOURCE_SUFFIXES);

		MemoryStream outputStream = new MemoryStream();
		Dmn.writeModelToStream(outputStream, modelInstance);

		return addBytes(resourceName, outputStream.toByteArray());
	  }

	  private void validateResouceName(string resourceName, string[] resourceSuffixes)
	  {
		if (!StringUtil.hasAnySuffix(resourceName, resourceSuffixes))
		{
		  LOG.warnDeploymentResourceHasWrongName(resourceName, resourceSuffixes);
		}
	  }

	  protected internal virtual DeploymentBuilder addBytes(string resourceName, sbyte[] bytes)
	  {
		ResourceEntity resource = new ResourceEntity();
		resource.Bytes = bytes;
		resource.Name = resourceName;
		deployment.addResource(resource);

		return this;
	  }

	  public virtual DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream)
	  {
		try
		{
		  ZipEntry entry = zipInputStream.NextEntry;
		  while (entry != null)
		  {
			if (!entry.Directory)
			{
			  string entryName = entry.Name;
			  addInputStream(entryName, zipInputStream);
			}
			entry = zipInputStream.NextEntry;
		  }
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("problem reading zip input stream", e);
		}
		return this;
	  }

	  public virtual DeploymentBuilder addDeploymentResources(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		deployments.Add(deploymentId);
		return this;
	  }

	  public virtual DeploymentBuilder addDeploymentResourceById(string deploymentId, string resourceId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		ensureNotNull(typeof(NotValidException), "resourceId", resourceId);

		CollectionUtil.addToMapOfSets(deploymentResourcesById, deploymentId, resourceId);

		return this;
	  }

	  public virtual DeploymentBuilder addDeploymentResourcesById(string deploymentId, IList<string> resourceIds)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);

		ensureNotNull(typeof(NotValidException), "resourceIds", resourceIds);
		ensureNotEmpty(typeof(NotValidException), "resourceIds", resourceIds);
		ensureNotContainsNull(typeof(NotValidException), "resourceIds", resourceIds);

		CollectionUtil.addCollectionToMapOfSets(deploymentResourcesById, deploymentId, resourceIds);

		return this;
	  }

	  public virtual DeploymentBuilder addDeploymentResourceByName(string deploymentId, string resourceName)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		ensureNotNull(typeof(NotValidException), "resourceName", resourceName);

		CollectionUtil.addToMapOfSets(deploymentResourcesByName, deploymentId, resourceName);

		return this;
	  }

	  public virtual DeploymentBuilder addDeploymentResourcesByName(string deploymentId, IList<string> resourceNames)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);

		ensureNotNull(typeof(NotValidException), "resourceNames", resourceNames);
		ensureNotEmpty(typeof(NotValidException), "resourceNames", resourceNames);
		ensureNotContainsNull(typeof(NotValidException), "resourceNames", resourceNames);

		CollectionUtil.addCollectionToMapOfSets(deploymentResourcesByName, deploymentId, resourceNames);

		return this;
	  }

	  public virtual DeploymentBuilder name(string name)
	  {
		if (!string.ReferenceEquals(nameFromDeployment_Renamed, null) && nameFromDeployment_Renamed.Length > 0)
		{
		  string message = string.Format("Cannot set the deployment name to '{0}', because the property 'nameForDeployment' has been already set to '{1}'.", name, nameFromDeployment_Renamed);
		  throw new NotValidException(message);
		}
		deployment.Name = name;
		return this;
	  }

	  public virtual DeploymentBuilder nameFromDeployment(string deploymentId)
	  {
		string name = deployment.Name;
		if (!string.ReferenceEquals(name, null) && name.Length > 0)
		{
		  string message = string.Format("Cannot set the given deployment id '{0}' to get the name from it, because the deployment name has been already set to '{1}'.", deploymentId, name);
		  throw new NotValidException(message);
		}
		nameFromDeployment_Renamed = deploymentId;
		return this;
	  }

	  public virtual DeploymentBuilder enableDuplicateFiltering()
	  {
		return enableDuplicateFiltering(false);
	  }

	  public virtual DeploymentBuilder enableDuplicateFiltering(bool deployChangedOnly)
	  {
		this.isDuplicateFilterEnabled = true;
		this.deployChangedOnly = deployChangedOnly;
		return this;
	  }

	  public virtual DeploymentBuilder activateProcessDefinitionsOn(DateTime date)
	  {
		this.processDefinitionsActivationDate = date;
		return this;
	  }

	  public virtual DeploymentBuilder source(string source)
	  {
		deployment.Source = source;
		return this;
	  }

	  public virtual DeploymentBuilder tenantId(string tenantId)
	  {
		deployment.TenantId = tenantId;
		return this;
	  }

	  public virtual Deployment deploy()
	  {
		return deployWithResult();
	  }

	  public virtual DeploymentWithDefinitions deployWithResult()
	  {
		return repositoryService.deployWithResult(this);
	  }


	  public virtual ICollection<string> ResourceNames
	  {
		  get
		  {
			if (deployment.Resources == null)
			{
			  return System.Linq.Enumerable.Empty<string>();
			}
			else
			{
			  return deployment.Resources.Keys;
			}
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual DeploymentEntity Deployment
	  {
		  get
		  {
			return deployment;
		  }
	  }

	  public virtual bool DuplicateFilterEnabled
	  {
		  get
		  {
			return isDuplicateFilterEnabled;
		  }
	  }

	  public virtual bool DeployChangedOnly
	  {
		  get
		  {
			return deployChangedOnly;
		  }
	  }

	  public virtual DateTime ProcessDefinitionsActivationDate
	  {
		  get
		  {
			return processDefinitionsActivationDate;
		  }
	  }

	  public virtual string NameFromDeployment
	  {
		  get
		  {
			return nameFromDeployment_Renamed;
		  }
	  }

	  public virtual ISet<string> Deployments
	  {
		  get
		  {
			return deployments;
		  }
	  }

	  public virtual IDictionary<string, ISet<string>> DeploymentResourcesById
	  {
		  get
		  {
			return deploymentResourcesById;
		  }
	  }

	  public virtual IDictionary<string, ISet<string>> DeploymentResourcesByName
	  {
		  get
		  {
			return deploymentResourcesByName;
		  }
	  }

	}

}