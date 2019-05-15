using System;
using System.Collections.Generic;
using System.Text;
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
namespace org.camunda.bpm.engine.impl.cmd
{

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using TransactionLogger = org.camunda.bpm.engine.impl.cfg.TransactionLogger;
	using TransactionState = org.camunda.bpm.engine.impl.cfg.TransactionState;
	using CmmnDeployer = org.camunda.bpm.engine.impl.cmmn.deployer.CmmnDeployer;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentFailListener = org.camunda.bpm.engine.impl.persistence.deploy.DeploymentFailListener;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using DeploymentManager = org.camunda.bpm.engine.impl.persistence.entity.DeploymentManager;
	using ProcessApplicationDeploymentImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessApplicationDeploymentImpl;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using ResourceManager = org.camunda.bpm.engine.impl.persistence.entity.ResourceManager;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;
	using DeploymentBuilderImpl = org.camunda.bpm.engine.impl.repository.DeploymentBuilderImpl;
	using ProcessApplicationDeploymentBuilderImpl = org.camunda.bpm.engine.impl.repository.ProcessApplicationDeploymentBuilderImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using org.camunda.bpm.engine.repository;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Thorben Lindhauer
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class DeployCmd : Command<DeploymentWithDefinitions>
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;
	  private static readonly TransactionLogger TX_LOG = ProcessEngineLogger.TX_LOGGER;

	  private const long serialVersionUID = 1L;

	  protected internal DeploymentBuilderImpl deploymentBuilder;

	  public DeployCmd(DeploymentBuilderImpl deploymentBuilder)
	  {
		this.deploymentBuilder = deploymentBuilder;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public DeploymentWithDefinitions execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual DeploymentWithDefinitions execute(CommandContext commandContext)
	  {
		if (commandContext.ProcessEngineConfiguration.DeploymentSynchronized)
		{
		  // ensure serial processing of multiple deployments on the same node.
		  // We experienced deadlock situations with highly concurrent deployment of multiple
		  // applications on Jboss & Wildfly
		  lock (typeof(ProcessEngine))
		  {
			return doExecute(commandContext);
		  }
		}
		else
		{
		  return doExecute(commandContext);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected DeploymentWithDefinitions doExecute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual DeploymentWithDefinitions doExecute(CommandContext commandContext)
	  {
		DeploymentManager deploymentManager = commandContext.DeploymentManager;

		ISet<string> deploymentIds = getAllDeploymentIds(deploymentBuilder);
		if (deploymentIds.Count > 0)
		{
		  string[] deploymentIdArray = deploymentIds.toArray(new string[deploymentIds.Count]);
		  IList<DeploymentEntity> deployments = deploymentManager.findDeploymentsByIds(deploymentIdArray);
		  ensureDeploymentsWithIdsExists(deploymentIds, deployments);
		}

		checkCreateAndReadDeployments(commandContext, deploymentIds);

		// set deployment name if it should retrieved from an existing deployment
		string nameFromDeployment = deploymentBuilder.NameFromDeployment;
		setDeploymentName(nameFromDeployment, deploymentBuilder, commandContext);

		// get resources to re-deploy
		IList<ResourceEntity> resources = getResources(deploymentBuilder, commandContext);
		// .. and add them the builder
		addResources(resources, deploymentBuilder);

		ICollection<string> resourceNames = deploymentBuilder.ResourceNames;
		if (resourceNames == null || resourceNames.Count == 0)
		{
		  throw new NotValidException("No deployment resources contained to deploy.");
		}

		// perform deployment
		DeploymentWithDefinitions deployment = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));

		createUserOperationLog(deploymentBuilder, deployment, commandContext);

		return deployment;
	  }

	  private class CallableAnonymousInnerClass : Callable<DeploymentWithDefinitions>
	  {
		  private readonly DeployCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(DeployCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public DeploymentWithDefinitions call() throws Exception
		  public override DeploymentWithDefinitions call()
		  {
			outerInstance.acquireExclusiveLock(commandContext);
			DeploymentEntity deployment = outerInstance.initDeployment();
			IDictionary<string, ResourceEntity> resourcesToDeploy = outerInstance.resolveResourcesToDeploy(commandContext, deployment);
			IDictionary<string, ResourceEntity> resourcesToIgnore = new Dictionary<string, ResourceEntity>(deployment.Resources);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
			resourcesToIgnore.Keys.removeAll(resourcesToDeploy.Keys);

			if (resourcesToDeploy.Count > 0)
			{
			  LOG.debugCreatingNewDeployment();
			  deployment.Resources = resourcesToDeploy;
			  outerInstance.deploy(deployment);
			}
			else
			{
			  LOG.usingExistingDeployment();
			  deployment = outerInstance.getExistingDeployment(commandContext, deployment.Name);
			}

			outerInstance.scheduleProcessDefinitionActivation(commandContext, deployment);

			if (outerInstance.deploymentBuilder is ProcessApplicationDeploymentBuilder)
			{
			  // for process application deployments, job executor registration is managed by
			  // process application manager
			  ISet<string> processesToRegisterFor = outerInstance.retrieveProcessKeysFromResources(resourcesToIgnore);
			  ProcessApplicationRegistration registration = outerInstance.registerProcessApplication(commandContext, deployment, processesToRegisterFor);
			  return new ProcessApplicationDeploymentImpl(deployment, registration);
			}
			else
			{
			  outerInstance.registerWithJobExecutor(commandContext, deployment);
			}

			return deployment;
		  }
	  }

	  protected internal virtual void createUserOperationLog(DeploymentBuilderImpl deploymentBuilder, Deployment deployment, CommandContext commandContext)
	  {
		UserOperationLogManager logManager = commandContext.OperationLogManager;

		IList<PropertyChange> properties = new List<PropertyChange>();

		PropertyChange filterDuplicate = new PropertyChange("duplicateFilterEnabled", null, deploymentBuilder.DuplicateFilterEnabled);
		properties.Add(filterDuplicate);

		if (deploymentBuilder.DuplicateFilterEnabled)
		{
		  PropertyChange deployChangedOnly = new PropertyChange("deployChangedOnly", null, deploymentBuilder.DeployChangedOnly);
		  properties.Add(deployChangedOnly);
		}

		logManager.logDeploymentOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, deployment.Id, properties);
	  }

	  protected internal virtual void setDeploymentName(string deploymentId, DeploymentBuilderImpl deploymentBuilder, CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(deploymentId, null) && deploymentId.Length > 0)
		{
		  DeploymentManager deploymentManager = commandContext.DeploymentManager;
		  DeploymentEntity deployment = deploymentManager.findDeploymentById(deploymentId);
		  deploymentBuilder.Deployment.Name = deployment.Name;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity> getResources(final org.camunda.bpm.engine.impl.repository.DeploymentBuilderImpl deploymentBuilder, final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual IList<ResourceEntity> getResources(DeploymentBuilderImpl deploymentBuilder, CommandContext commandContext)
	  {
		IList<ResourceEntity> resources = new List<ResourceEntity>();

		ISet<string> deploymentIds = deploymentBuilder.Deployments;
		((IList<ResourceEntity>)resources).AddRange(getResourcesByDeploymentId(deploymentIds, commandContext));

		IDictionary<string, ISet<string>> deploymentResourcesById = deploymentBuilder.DeploymentResourcesById;
		((IList<ResourceEntity>)resources).AddRange(getResourcesById(deploymentResourcesById, commandContext));

		IDictionary<string, ISet<string>> deploymentResourcesByName = deploymentBuilder.DeploymentResourcesByName;
		((IList<ResourceEntity>)resources).AddRange(getResourcesByName(deploymentResourcesByName, commandContext));

		checkDuplicateResourceName(resources);

		return resources;
	  }

	  protected internal virtual IList<ResourceEntity> getResourcesByDeploymentId(ISet<string> deploymentIds, CommandContext commandContext)
	  {
		IList<ResourceEntity> result = new List<ResourceEntity>();

		if (deploymentIds.Count > 0)
		{

		  DeploymentManager deploymentManager = commandContext.DeploymentManager;

		  foreach (string deploymentId in deploymentIds)
		  {
			DeploymentEntity deployment = deploymentManager.findDeploymentById(deploymentId);
			IDictionary<string, ResourceEntity> resources = deployment.Resources;
			IDictionary<string, ResourceEntity>.ValueCollection values = resources.Values;
			((IList<ResourceEntity>)result).AddRange(values);
		  }
		}

		return result;
	  }

	  protected internal virtual IList<ResourceEntity> getResourcesById(IDictionary<string, ISet<string>> resourcesById, CommandContext commandContext)
	  {
		IList<ResourceEntity> result = new List<ResourceEntity>();

		ResourceManager resourceManager = commandContext.ResourceManager;

		foreach (string deploymentId in resourcesById.Keys)
		{
		  ISet<string> resourceIds = resourcesById[deploymentId];

		  string[] resourceIdArray = resourceIds.toArray(new string[resourceIds.Count]);
		  IList<ResourceEntity> resources = resourceManager.findResourceByDeploymentIdAndResourceIds(deploymentId, resourceIdArray);

		  ensureResourcesWithIdsExist(deploymentId, resourceIds, resources);

		  ((IList<ResourceEntity>)result).AddRange(resources);
		}

		return result;
	  }

	  protected internal virtual IList<ResourceEntity> getResourcesByName(IDictionary<string, ISet<string>> resourcesByName, CommandContext commandContext)
	  {
		IList<ResourceEntity> result = new List<ResourceEntity>();

		ResourceManager resourceManager = commandContext.ResourceManager;

		foreach (string deploymentId in resourcesByName.Keys)
		{
		  ISet<string> resourceNames = resourcesByName[deploymentId];

		  string[] resourceNameArray = resourceNames.toArray(new string[resourceNames.Count]);
		  IList<ResourceEntity> resources = resourceManager.findResourceByDeploymentIdAndResourceNames(deploymentId, resourceNameArray);

		  ensureResourcesWithNamesExist(deploymentId, resourceNames, resources);

		  ((IList<ResourceEntity>)result).AddRange(resources);
		}

		return result;
	  }

	  protected internal virtual void addResources(IList<ResourceEntity> resources, DeploymentBuilderImpl deploymentBuilder)
	  {
		DeploymentEntity deployment = deploymentBuilder.Deployment;
		IDictionary<string, ResourceEntity> existingResources = deployment.Resources;

		foreach (ResourceEntity resource in resources)
		{
		  string resourceName = resource.Name;

		  if (existingResources != null && existingResources.ContainsKey(resourceName))
		  {
			string message = string.Format("Cannot add resource with id '{0}' and name '{1}' from " + "deployment with id '{2}' to new deployment because the new deployment contains " + "already a resource with same name.", resource.Id, resourceName, resource.DeploymentId);

			throw new NotValidException(message);
		  }

		  MemoryStream inputStream = new MemoryStream(resource.Bytes);
		  deploymentBuilder.addInputStream(resourceName, inputStream);
		}
	  }

	  protected internal virtual void checkDuplicateResourceName(IList<ResourceEntity> resources)
	  {
		IDictionary<string, ResourceEntity> resourceMap = new Dictionary<string, ResourceEntity>();

		foreach (ResourceEntity resource in resources)
		{
		  string name = resource.Name;

		  ResourceEntity duplicate = resourceMap[name];
		  if (duplicate != null)
		  {
			string deploymentId = resource.DeploymentId;
			if (!deploymentId.Equals(duplicate.DeploymentId))
			{
			  string message = string.Format("The deployments with id '{0}' and '{1}' contain a resource with same name '{2}'.", deploymentId, duplicate.DeploymentId, name);
			  throw new NotValidException(message);
			}
		  }
		  resourceMap[name] = resource;
		}
	  }

	  protected internal virtual void ensureDeploymentsWithIdsExists(ISet<string> expected, IList<DeploymentEntity> actual)
	  {
		IDictionary<string, DeploymentEntity> deploymentMap = new Dictionary<string, DeploymentEntity>();
		foreach (DeploymentEntity deployment in actual)
		{
		  deploymentMap[deployment.Id] = deployment;
		}

		IList<string> missingDeployments = getMissingElements(expected, deploymentMap);

		if (missingDeployments.Count > 0)
		{
		  StringBuilder builder = new StringBuilder();

		  builder.Append("The following deployments are not found by id: ");

		  bool first = true;
		  foreach (string missingDeployment in missingDeployments)
		  {
			if (!first)
			{
			  builder.Append(", ");
			}
			else
			{
			  first = false;
			}
			builder.Append(missingDeployment);
		  }

		  throw new NotFoundException(builder.ToString());
		}
	  }

	  protected internal virtual void ensureResourcesWithIdsExist(string deploymentId, ISet<string> expectedIds, IList<ResourceEntity> actual)
	  {
		IDictionary<string, ResourceEntity> resources = new Dictionary<string, ResourceEntity>();
		foreach (ResourceEntity resource in actual)
		{
		  resources[resource.Id] = resource;
		}
		ensureResourcesWithKeysExist(deploymentId, expectedIds, resources, "id");
	  }

	  protected internal virtual void ensureResourcesWithNamesExist(string deploymentId, ISet<string> expectedNames, IList<ResourceEntity> actual)
	  {
		IDictionary<string, ResourceEntity> resources = new Dictionary<string, ResourceEntity>();
		foreach (ResourceEntity resource in actual)
		{
		  resources[resource.Name] = resource;
		}
		ensureResourcesWithKeysExist(deploymentId, expectedNames, resources, "name");
	  }

	  protected internal virtual void ensureResourcesWithKeysExist(string deploymentId, ISet<string> expectedKeys, IDictionary<string, ResourceEntity> actual, string valueProperty)
	  {
		IList<string> missingResources = getMissingElements(expectedKeys, actual);

		if (missingResources.Count > 0)
		{
		  StringBuilder builder = new StringBuilder();

		  builder.Append("The deployment with id '");
		  builder.Append(deploymentId);
		  builder.Append("' does not contain the following resources with ");
		  builder.Append(valueProperty);
		  builder.Append(": ");

		  bool first = true;
		  foreach (string missingResource in missingResources)
		  {
			if (!first)
			{
			  builder.Append(", ");
			}
			else
			{
			  first = false;
			}
			builder.Append(missingResource);
		  }

		  throw new NotFoundException(builder.ToString());
		}
	  }

	  protected internal virtual IList<string> getMissingElements<T1>(ISet<string> expected, IDictionary<T1> actual)
	  {
		IList<string> missingElements = new List<string>();
		foreach (string value in expected)
		{
		  if (!actual.ContainsKey(value))
		  {
			missingElements.Add(value);
		  }
		}
		return missingElements;
	  }

	  protected internal virtual ISet<string> getAllDeploymentIds(DeploymentBuilderImpl deploymentBuilder)
	  {
		ISet<string> result = new HashSet<string>();

		string nameFromDeployment = deploymentBuilder.NameFromDeployment;
		if (!string.ReferenceEquals(nameFromDeployment, null) && nameFromDeployment.Length > 0)
		{
		  result.Add(nameFromDeployment);
		}

		ISet<string> deployments = deploymentBuilder.Deployments;
		result.addAll(deployments);

		deployments = deploymentBuilder.DeploymentResourcesById.Keys;
		result.addAll(deployments);

		deployments = deploymentBuilder.DeploymentResourcesByName.Keys;
		result.addAll(deployments);

		return result;
	  }

	  protected internal virtual void checkCreateAndReadDeployments(CommandContext commandContext, ISet<string> deploymentIds)
	  {
		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkCreateDeployment();
			foreach (string deploymentId in deploymentIds)
			{
			  checker.checkReadDeployment(deploymentId);
			}
		  }
	  }

	  protected internal virtual void acquireExclusiveLock(CommandContext commandContext)
	  {
		if (Context.ProcessEngineConfiguration.DeploymentLockUsed)
		{
		  // Acquire global exclusive lock: this ensures that there can be only one
		  // transaction in the cluster which is allowed to perform deployments.
		  // This is important to ensure that duplicate filtering works correctly
		  // in a multi-node cluster. See also https://app.camunda.com/jira/browse/CAM-2128

		  // It is also important to ensure the uniqueness of a process definition key,
		  // version and tenant-id since there is no database constraint to check it.

		  commandContext.PropertyManager.acquireExclusiveLock();
		}
		else
		{
		  LOG.warnDisabledDeploymentLock();
		}
	  }

	  protected internal virtual DeploymentEntity initDeployment()
	  {
		DeploymentEntity deployment = deploymentBuilder.Deployment;
		deployment.DeploymentTime = ClockUtil.CurrentTime;
		return deployment;
	  }

	  protected internal virtual IDictionary<string, ResourceEntity> resolveResourcesToDeploy(CommandContext commandContext, DeploymentEntity deployment)
	  {
		IDictionary<string, ResourceEntity> resourcesToDeploy = new Dictionary<string, ResourceEntity>();
		IDictionary<string, ResourceEntity> containedResources = deployment.Resources;

		if (deploymentBuilder.DuplicateFilterEnabled)
		{

		  string source = deployment.Source;
		  if (string.ReferenceEquals(source, null) || source.Length == 0)
		  {
			source = ProcessApplicationDeployment_Fields.PROCESS_APPLICATION_DEPLOYMENT_SOURCE;
		  }

		  IDictionary<string, ResourceEntity> existingResources = commandContext.ResourceManager.findLatestResourcesByDeploymentName(deployment.Name, containedResources.Keys, source, deployment.TenantId);

		  foreach (ResourceEntity deployedResource in containedResources.Values)
		  {
			string resourceName = deployedResource.Name;
			ResourceEntity existingResource = existingResources[resourceName];

			if (existingResource == null || existingResource.Generated || resourcesDiffer(deployedResource, existingResource))
			{
			  // resource should be deployed

			  if (deploymentBuilder.DeployChangedOnly)
			  {
				resourcesToDeploy[resourceName] = deployedResource;
			  }
			  else
			  {
				// all resources should be deployed
				resourcesToDeploy = containedResources;
				break;
			  }
			}
		  }

		}
		else
		{
		  resourcesToDeploy = containedResources;
		}

		return resourcesToDeploy;
	  }

	  protected internal virtual bool resourcesDiffer(ResourceEntity resource, ResourceEntity existing)
	  {
		sbyte[] bytes = resource.Bytes;
		sbyte[] savedBytes = existing.Bytes;
		return !Arrays.Equals(bytes, savedBytes);
	  }

	  protected internal virtual void deploy(DeploymentEntity deployment)
	  {
		deployment.New = true;
		Context.CommandContext.DeploymentManager.insertDeployment(deployment);
	  }

	  protected internal virtual DeploymentEntity getExistingDeployment(CommandContext commandContext, string deploymentName)
	  {
		return commandContext.DeploymentManager.findLatestDeploymentByName(deploymentName);
	  }

	  protected internal virtual void scheduleProcessDefinitionActivation(CommandContext commandContext, DeploymentEntity deployment)
	  {
		if (deploymentBuilder.ProcessDefinitionsActivationDate != null)
		{
		  RepositoryService repositoryService = commandContext.ProcessEngineConfiguration.RepositoryService;

		  foreach (ProcessDefinition processDefinition in deployment.DeployedProcessDefinitions)
		  {

			// If activation date is set, we first suspend all the process definition
			repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).suspend();

			// And we schedule an activation at the provided date
			repositoryService.updateProcessDefinitionSuspensionState().byProcessDefinitionId(processDefinition.Id).executionDate(deploymentBuilder.ProcessDefinitionsActivationDate).activate();
		  }
		}
	  }

	  protected internal virtual ProcessApplicationRegistration registerProcessApplication(CommandContext commandContext, DeploymentEntity deployment, ISet<string> processKeysToRegisterFor)
	  {
		ProcessApplicationDeploymentBuilderImpl appDeploymentBuilder = (ProcessApplicationDeploymentBuilderImpl) deploymentBuilder;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationReference appReference = appDeploymentBuilder.getProcessApplicationReference();
		ProcessApplicationReference appReference = appDeploymentBuilder.ProcessApplicationReference;

		// build set of deployment ids this process app should be registered for:
		ISet<string> deploymentsToRegister = new HashSet<string>(Collections.singleton(deployment.Id));

		if (appDeploymentBuilder.ResumePreviousVersions)
		{
		  if (ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY.Equals(appDeploymentBuilder.ResumePreviousVersionsBy))
		  {
			deploymentsToRegister.addAll(resumePreviousByProcessDefinitionKey(commandContext, deployment, processKeysToRegisterFor));
		  }
		  else if (ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME.Equals(appDeploymentBuilder.ResumePreviousVersionsBy))
		  {
			deploymentsToRegister.addAll(resumePreviousByDeploymentName(commandContext, deployment));
		  }
		}

		// register process application for deployments
		return (new RegisterProcessApplicationCmd(deploymentsToRegister, appReference)).execute(commandContext);

	  }

	  /// <summary>
	  /// Searches in previous deployments for the same processes and retrieves the deployment ids.
	  /// </summary>
	  /// <param name="commandContext"> </param>
	  /// <param name="deployment">
	  ///          the current deployment </param>
	  /// <param name="processKeysToRegisterFor">
	  ///          the process keys this process application wants to register </param>
	  /// <param name="deployment">
	  ///          the set where to add further deployments this process application
	  ///          should be registered for </param>
	  /// <returns> a set of deployment ids that contain versions of the
	  ///         processKeysToRegisterFor </returns>
	  protected internal virtual ISet<string> resumePreviousByProcessDefinitionKey(CommandContext commandContext, DeploymentEntity deployment, ISet<string> processKeysToRegisterFor)
	  {
		ISet<string> processDefinitionKeys = new HashSet<string>(processKeysToRegisterFor);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends ProcessDefinition> deployedProcesses = getDeployedProcesses(deployment);
		IList<ProcessDefinition> deployedProcesses = getDeployedProcesses(deployment);
		foreach (ProcessDefinition deployedProcess in deployedProcesses)
		{
		  if (deployedProcess.Version > 1)
		  {
			processDefinitionKeys.Add(deployedProcess.Key);
		  }
		}

		return findDeploymentIdsForProcessDefinitions(commandContext, processDefinitionKeys);
	  }

	  /// <summary>
	  /// Searches for previous deployments with the same name. </summary>
	  /// <param name="commandContext"> </param>
	  /// <param name="deployment"> the current deployment </param>
	  /// <returns> a set of deployment ids </returns>
	  protected internal virtual ISet<string> resumePreviousByDeploymentName(CommandContext commandContext, DeploymentEntity deployment)
	  {
		IList<Deployment> previousDeployments = (new DeploymentQueryImpl()).deploymentName(deployment.Name).list();
		ISet<string> deploymentIds = new HashSet<string>(previousDeployments.Count);
		foreach (Deployment d in previousDeployments)
		{
		  deploymentIds.Add(d.Id);
		}
		return deploymentIds;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ProcessDefinition> getDeployedProcesses(org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity deployment)
	  protected internal virtual IList<ProcessDefinition> getDeployedProcesses(DeploymentEntity deployment)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends ProcessDefinition> deployedProcessDefinitions = deployment.getDeployedProcessDefinitions();
		IList<ProcessDefinition> deployedProcessDefinitions = deployment.DeployedProcessDefinitions;
		if (deployedProcessDefinitions == null)
		{
		  // existing deployment
		  CommandContext commandContext = Context.CommandContext;
		  ProcessDefinitionManager manager = commandContext.ProcessDefinitionManager;
		  deployedProcessDefinitions = manager.findProcessDefinitionsByDeploymentId(deployment.Id);
		}

		return deployedProcessDefinitions;
	  }

	  protected internal virtual ISet<string> retrieveProcessKeysFromResources(IDictionary<string, ResourceEntity> resources)
	  {
		ISet<string> keys = new HashSet<string>();

		foreach (ResourceEntity resource in resources.Values)
		{
		  if (isBpmnResource(resource))
		  {

			MemoryStream byteStream = new MemoryStream(resource.Bytes);
			BpmnModelInstance model = Bpmn.readModelFromStream(byteStream);
			foreach (Process process in model.Definitions.getChildElementsByType(typeof(Process)))
			{
			  keys.Add(process.Id);
			}
		  }
		  else if (isCmmnResource(resource))
		  {

			MemoryStream byteStream = new MemoryStream(resource.Bytes);
			CmmnModelInstance model = Cmmn.readModelFromStream(byteStream);
			foreach (Case cmmnCase in model.Definitions.Cases)
			{
			  keys.Add(cmmnCase.Id);
			}
		  }
		}

		return keys;
	  }

	  protected internal virtual bool isBpmnResource(ResourceEntity resourceEntity)
	  {
		return StringUtil.hasAnySuffix(resourceEntity.Name, BpmnDeployer.BPMN_RESOURCE_SUFFIXES);
	  }

	  protected internal virtual bool isCmmnResource(ResourceEntity resourceEntity)
	  {
		return StringUtil.hasAnySuffix(resourceEntity.Name, CmmnDeployer.CMMN_RESOURCE_SUFFIXES);
	  }

	  protected internal virtual ISet<string> findDeploymentIdsForProcessDefinitions(CommandContext commandContext, ISet<string> processDefinitionKeys)
	  {
		ISet<string> deploymentsToRegister = new HashSet<string>();

		if (processDefinitionKeys.Count > 0)
		{

		  string[] keys = processDefinitionKeys.toArray(new string[processDefinitionKeys.Count]);
		  ProcessDefinitionManager processDefinitionManager = commandContext.ProcessDefinitionManager;
		  IList<ProcessDefinition> previousDefinitions = processDefinitionManager.findProcessDefinitionsByKeyIn(keys);

		  foreach (ProcessDefinition definition in previousDefinitions)
		  {
			deploymentsToRegister.Add(definition.DeploymentId);
		  }
		}
		return deploymentsToRegister;
	  }

	  protected internal virtual void registerWithJobExecutor(CommandContext commandContext, DeploymentEntity deployment)
	  {
		try
		{
		  (new RegisterDeploymentCmd(deployment.Id)).execute(commandContext);

		}
		finally
		{
		  DeploymentFailListener listener = new DeploymentFailListener(deployment.Id, Context.ProcessEngineConfiguration.CommandExecutorTxRequiresNew);

		  try
		  {
			commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, listener);
		  }
		  catch (Exception)
		  {
			TX_LOG.debugTransactionOperation("Could not register transaction synchronization. Probably the TX has already been rolled back by application code.");
			listener.execute(commandContext);
		  }
		}
	  }
	}

}