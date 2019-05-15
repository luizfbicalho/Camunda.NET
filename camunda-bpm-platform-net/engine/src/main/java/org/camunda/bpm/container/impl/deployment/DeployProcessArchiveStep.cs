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
namespace org.camunda.bpm.container.impl.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessApplicationScanningUtil = org.camunda.bpm.container.impl.deployment.scanning.ProcessApplicationScanningUtil;
	using DeployedProcessArchive = org.camunda.bpm.container.impl.deployment.util.DeployedProcessArchive;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ProcessEngines = org.camunda.bpm.engine.ProcessEngines;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessApplicationDeploymentBuilder = org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;

	/// <summary>
	/// <para>
	/// Deployment operation step responsible for deploying a process archive
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DeployProcessArchiveStep : DeploymentOperationStep
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal readonly ProcessArchiveXml processArchive;
	  protected internal URL metaFileUrl;
	  protected internal ProcessApplicationDeployment deployment;

	  public DeployProcessArchiveStep(ProcessArchiveXml parsedProcessArchive, URL url)
	  {
		processArchive = parsedProcessArchive;
		this.metaFileUrl = url;
	  }

	  public override string Name
	  {
		  get
		  {
			return "Deployment of process archive '" + processArchive.Name;
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader processApplicationClassloader = processApplication.getProcessApplicationClassloader();
		ClassLoader processApplicationClassloader = processApplication.ProcessApplicationClassloader;

		ProcessEngine processEngine = getProcessEngine(serviceContainer, processApplication.DefaultDeployToEngineName);

		// start building deployment map
		IDictionary<string, sbyte[]> deploymentMap = new Dictionary<string, sbyte[]>();

		// add all processes listed in the processes.xml
		IList<string> listedProcessResources = processArchive.ProcessResourceNames;
		foreach (string processResource in listedProcessResources)
		{
		  Stream resourceAsStream = null;
		  try
		  {
			resourceAsStream = processApplicationClassloader.getResourceAsStream(processResource);
			sbyte[] bytes = IoUtil.readInputStream(resourceAsStream, processResource);
			deploymentMap[processResource] = bytes;
		  }
		  finally
		  {
			IoUtil.closeSilently(resourceAsStream);
		  }
		}

		// scan for additional process definitions if not turned off
		if (PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS, true))
		{
		  string paResourceRoot = processArchive.Properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_RESOURCE_ROOT_PATH];
		  string[] additionalResourceSuffixes = StringUtil.Split(processArchive.Properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_ADDITIONAL_RESOURCE_SUFFIXES], org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_ADDITIONAL_RESOURCE_SUFFIXES_SEPARATOR);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  deploymentMap.putAll(findResources(processApplicationClassloader, paResourceRoot, additionalResourceSuffixes));
		}

		// perform process engine deployment
		RepositoryService repositoryService = processEngine.RepositoryService;
		ProcessApplicationDeploymentBuilder deploymentBuilder = repositoryService.createDeployment(processApplication.Reference);

		// set the name for the deployment
		string deploymentName = processArchive.Name;
		if (string.ReferenceEquals(deploymentName, null) || deploymentName.Length == 0)
		{
		  deploymentName = processApplication.Name;
		}
		deploymentBuilder.name(deploymentName);

		// set the tenant id for the deployment
		string tenantId = processArchive.TenantId;
		if (!string.ReferenceEquals(tenantId, null) && tenantId.Length > 0)
		{
		  deploymentBuilder.tenantId(tenantId);
		}

		// enable duplicate filtering
		deploymentBuilder.enableDuplicateFiltering(PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DEPLOY_CHANGED_ONLY, false));

		if (PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_RESUME_PREVIOUS_VERSIONS, true))
		{
		  enableResumingOfPreviousVersions(deploymentBuilder);
		}

		// add all resources obtained through the processes.xml and through scanning
		foreach (KeyValuePair<string, sbyte[]> deploymentResource in deploymentMap.SetOfKeyValuePairs())
		{
		  deploymentBuilder.addInputStream(deploymentResource.Key, new MemoryStream(deploymentResource.Value));
		}

		// allow the process application to add additional resources to the deployment
		processApplication.createDeployment(processArchive.Name, deploymentBuilder);

		ICollection<string> deploymentResourceNames = deploymentBuilder.ResourceNames;
		if (deploymentResourceNames.Count > 0)
		{

		  LOG.deploymentSummary(deploymentResourceNames, deploymentName);

		  // perform the process engine deployment
		  deployment = deploymentBuilder.deploy();

		  // add attachment
		  IDictionary<string, DeployedProcessArchive> processArchiveDeploymentMap = operationContext.getAttachment(Attachments.PROCESS_ARCHIVE_DEPLOYMENT_MAP);
		  if (processArchiveDeploymentMap == null)
		  {
			processArchiveDeploymentMap = new Dictionary<string, DeployedProcessArchive>();
			operationContext.addAttachment(Attachments.PROCESS_ARCHIVE_DEPLOYMENT_MAP, processArchiveDeploymentMap);
		  }
		  processArchiveDeploymentMap[processArchive.Name] = new DeployedProcessArchive(deployment);

		}
		else
		{
		  LOG.notCreatingPaDeployment(processApplication.Name);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void enableResumingOfPreviousVersions(org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder deploymentBuilder) throws IllegalArgumentException
	  protected internal virtual void enableResumingOfPreviousVersions(ProcessApplicationDeploymentBuilder deploymentBuilder)
	  {
		deploymentBuilder.resumePreviousVersions();
		string resumePreviousBy = processArchive.Properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_RESUME_PREVIOUS_BY];
		if (string.ReferenceEquals(resumePreviousBy, null))
		{
		  deploymentBuilder.resumePreviousVersionsBy(ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY);
		}
		else if (isValidValueForResumePreviousBy(resumePreviousBy))
		{
		  deploymentBuilder.resumePreviousVersionsBy(resumePreviousBy);
		}
		else
		{
		  StringBuilder b = new StringBuilder();
		  b.Append("Illegal value passed for property ").Append(org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_RESUME_PREVIOUS_BY);
		  b.Append(". Value was ").Append(resumePreviousBy);
		  b.Append(" expected ").Append(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME);
		  b.Append(" or ").Append(ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY).Append(".");
		  throw LOG.illegalValueForResumePreviousByProperty(b.ToString());
		}
	  }

	  protected internal virtual bool isValidValueForResumePreviousBy(string resumePreviousBy)
	  {
		return resumePreviousBy.Equals(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME) || resumePreviousBy.Equals(ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.Map<String, byte[]> findResources(final ClassLoader processApplicationClassloader, String paResourceRoot, String[] additionalResourceSuffixes)
	  protected internal virtual IDictionary<string, sbyte[]> findResources(ClassLoader processApplicationClassloader, string paResourceRoot, string[] additionalResourceSuffixes)
	  {
		return ProcessApplicationScanningUtil.findResources(processApplicationClassloader, paResourceRoot, metaFileUrl, additionalResourceSuffixes);
	  }

	  public override void cancelOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);

		ProcessEngine processEngine = getProcessEngine(serviceContainer, processApplication.DefaultDeployToEngineName);

		// if a registration was performed, remove it.
		if (deployment != null && deployment.ProcessApplicationRegistration != null)
		{
		  processEngine.ManagementService.unregisterProcessApplication(deployment.ProcessApplicationRegistration.DeploymentIds, true);
		}

		// delete deployment if we were able to create one AND if
		// isDeleteUponUndeploy is set.
		if (deployment != null && PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DELETE_UPON_UNDEPLOY, false))
		{
		  if (processEngine != null)
		  {
			processEngine.RepositoryService.deleteDeployment(deployment.Id, true);
		  }
		}

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.ProcessEngine getProcessEngine(final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer)
	  protected internal virtual ProcessEngine getProcessEngine(PlatformServiceContainer serviceContainer)
	  {
		return getProcessEngine(serviceContainer, ProcessEngines.NAME_DEFAULT);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.ProcessEngine getProcessEngine(final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer, String defaultDeployToProcessEngineName)
	  protected internal virtual ProcessEngine getProcessEngine(PlatformServiceContainer serviceContainer, string defaultDeployToProcessEngineName)
	  {
		string processEngineName = processArchive.ProcessEngineName;
		if (!string.ReferenceEquals(processEngineName, null))
		{
		  ProcessEngine processEngine = serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, processEngineName);
		  ensureNotNull("Cannot deploy process archive '" + processArchive.Name + "' to process engine '" + processEngineName + "' no such process engine exists", "processEngine", processEngine);
		  return processEngine;

		}
		else
		{
		  ProcessEngine processEngine = serviceContainer.getServiceValue(ServiceTypes.PROCESS_ENGINE, defaultDeployToProcessEngineName);
		  ensureNotNull("Cannot deploy process archive '" + processArchive.Name + "' to default process: no such process engine exists", "processEngine", processEngine);
		  return processEngine;
		}
	  }

	}

}