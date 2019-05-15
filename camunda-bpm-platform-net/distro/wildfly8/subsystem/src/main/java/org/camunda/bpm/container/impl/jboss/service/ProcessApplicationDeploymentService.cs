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
namespace org.camunda.bpm.container.impl.jboss.service
{

	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using Tccl = org.camunda.bpm.container.impl.jboss.util.Tccl;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessApplicationDeploymentBuilder = org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;
	using ComponentView = org.jboss.@as.ee.component.ComponentView;
	using ManagedReference = org.jboss.@as.naming.ManagedReference;
	using Module = org.jboss.modules.Module;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;
	using InjectedValue = org.jboss.msc.value.InjectedValue;

	/// <summary>
	/// <para>Service responsible for performing a deployment to the process engine and managing
	/// the resulting <seealso cref="ProcessApplicationRegistration"/> with the process engine.</para>
	/// 
	/// <para>We construct one of these per Process Archive of a Process Application.</para>
	/// 
	/// <para>We need a dependency on the componentView service of the ProcessApplication
	/// component and the process engine to which the deployment should be performed.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentService : Service<ProcessApplicationDeploymentService>
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(ProcessApplicationDeploymentService).FullName);

	  protected internal InjectedValue<ExecutorService> executorInjector = new InjectedValue<ExecutorService>();

	  protected internal InjectedValue<ProcessEngine> processEngineInjector = new InjectedValue<ProcessEngine>();

	  protected internal InjectedValue<ProcessApplicationInterface> noViewProcessApplication = new InjectedValue<ProcessApplicationInterface>();
	  // for view-exposing ProcessApplicationComponents
	  protected internal InjectedValue<ComponentView> paComponentViewInjector = new InjectedValue<ComponentView>();

	  /// <summary>
	  /// the map of deployment resources obtained  through scanning </summary>
	  protected internal readonly IDictionary<string, sbyte[]> deploymentMap;
	  /// <summary>
	  /// deployment metadata that is passed in </summary>
	  protected internal readonly ProcessArchiveXml processArchive;

	  /// <summary>
	  /// the deployment we create here </summary>
	  protected internal ProcessApplicationDeployment deployment;

	  protected internal Module module;

	  public ProcessApplicationDeploymentService(IDictionary<string, sbyte[]> deploymentMap, ProcessArchiveXml processArchive, Module module)
	  {
		this.deploymentMap = deploymentMap;
		this.processArchive = processArchive;
		this.module = module;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(final org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void start(StartContext context)
	  {
		context.asynchronous();
		executorInjector.Value.submit(() =>
		{
	try
	{
	  performDeployment();
	  context.complete();
	}
	catch (StartException e)
	{
	  context.failed(e);
	}
	catch (Exception e)
	{
	  context.failed(new StartException(e));
	}
		});
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void stop(final org.jboss.msc.service.StopContext context)
	  public virtual void stop(StopContext context)
	  {
		context.asynchronous();
		executorInjector.Value.submit(() =>
		{
	try
	{
	  performUndeployment();
	}
	finally
	{
	  context.complete();
	}
		});
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performDeployment() throws org.jboss.msc.service.StartException
	  protected internal virtual void performDeployment()
	  {

		ManagedReference reference = null;
		try
		{

		  // get process engine
		  ProcessEngine processEngine = processEngineInjector.Value;

		  // get the process application component
		  ProcessApplicationInterface processApplication = null;
		  ComponentView componentView = paComponentViewInjector.OptionalValue;
		  if (componentView != null)
		  {
			reference = componentView.createInstance();
			processApplication = (ProcessApplicationInterface) reference.Instance;
		  }
		  else
		  {
			processApplication = noViewProcessApplication.Value;
		  }

		  // get the application name
		  string processApplicationName = processApplication.Name;

		  // build the deployment
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RepositoryService repositoryService = processEngine.getRepositoryService();
		  RepositoryService repositoryService = processEngine.RepositoryService;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder deploymentBuilder = repositoryService.createDeployment(processApplication.getReference());
		  ProcessApplicationDeploymentBuilder deploymentBuilder = repositoryService.createDeployment(processApplication.Reference);

		  // enable duplicate filtering
		  deploymentBuilder.enableDuplicateFiltering(PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DEPLOY_CHANGED_ONLY, false));

		  // enable resuming of previous versions:
		  if (PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_RESUME_PREVIOUS_VERSIONS, true))
		  {
			enableResumingOfPreviousVersions(deploymentBuilder);
		  }

		  // set the name for the deployment
		  string deploymentName = processArchive.Name;
		  if (string.ReferenceEquals(deploymentName, null) || deploymentName.Length == 0)
		  {
			deploymentName = processApplicationName;
		  }
		  deploymentBuilder.name(deploymentName);

		  // set the tenant id for the deployment
		  string tenantId = processArchive.TenantId;
		  if (!string.ReferenceEquals(tenantId, null) && tenantId.Length > 0)
		  {
			deploymentBuilder.tenantId(tenantId);
		  }

		  // add deployment resources
		  foreach (KeyValuePair<string, sbyte[]> resource in deploymentMap.SetOfKeyValuePairs())
		  {
			deploymentBuilder.addInputStream(resource.Key, new MemoryStream(resource.Value));
		  }

		  // let the process application component add resources to the deployment.
		  processApplication.createDeployment(processArchive.Name, deploymentBuilder);

		  ICollection<string> resourceNames = deploymentBuilder.ResourceNames;
		  if (resourceNames.Count > 0)
		  {
			logDeploymentSummary(resourceNames, deploymentName, processApplicationName);
			// perform the actual deployment
			deployment = Tccl.runUnderClassloader(new OperationAnonymousInnerClass(this, deploymentBuilder)
		   , module.ClassLoader);

		  }
		  else
		  {
			LOGGER.info("Not creating a deployment for process archive '" + processArchive.Name + "': no resources provided.");

		  }

		}
		catch (Exception e)
		{
		  throw new StartException("Could not register process application with shared process engine ",e);

		}
		finally
		{
		  if (reference != null)
		  {
			reference.release();
		  }
		}
	  }

	  private class OperationAnonymousInnerClass : Tccl.Operation<ProcessApplicationDeployment>
	  {
		  private readonly ProcessApplicationDeploymentService outerInstance;

		  private ProcessApplicationDeploymentBuilder deploymentBuilder;

		  public OperationAnonymousInnerClass(ProcessApplicationDeploymentService outerInstance, ProcessApplicationDeploymentBuilder deploymentBuilder)
		  {
			  this.outerInstance = outerInstance;
			  this.deploymentBuilder = deploymentBuilder;
		  }


		  public ProcessApplicationDeployment run()
		  {
			return deploymentBuilder.deploy();
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
		  throw new System.ArgumentException(b.ToString());
		}
	  }

	  protected internal virtual bool isValidValueForResumePreviousBy(string resumePreviousBy)
	  {
		return resumePreviousBy.Equals(ResumePreviousBy.RESUME_BY_DEPLOYMENT_NAME) || resumePreviousBy.Equals(ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY);
	  }

	  /// <param name="deploymentMap2"> </param>
	  /// <param name="deploymentName"> </param>
	  protected internal virtual void logDeploymentSummary(ICollection<string> resourceNames, string deploymentName, string processApplicationName)
	  {
		// log a summary of the deployment
		StringBuilder builder = new StringBuilder();
		builder.Append("Deployment summary for process archive '" + deploymentName + "' of process application '" + processApplicationName + "': \n");
		builder.Append("\n");
		foreach (string resourceName in resourceNames)
		{
		  builder.Append("        " + resourceName);
		  builder.Append("\n");
		}
		LOGGER.log(Level.INFO, builder.ToString());
	  }

	  protected internal virtual void performUndeployment()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine processEngine = processEngineInjector.getValue();
		ProcessEngine processEngine = processEngineInjector.Value;

		try
		{
		  if (deployment != null)
		  {
			// always unregister
			ISet<string> deploymentIds = deployment.ProcessApplicationRegistration.DeploymentIds;
			processEngine.ManagementService.unregisterProcessApplication(deploymentIds, true);
		  }
		}
		catch (Exception)
		{
		  LOGGER.log(Level.SEVERE, "Exception while unregistering process application with the process engine.");

		}

		// delete the deployment only if requested in metadata
		if (deployment != null && PropertyHelper.getBooleanProperty(processArchive.Properties, org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DELETE_UPON_UNDEPLOY, false))
		{
		  try
		  {
			LOGGER.info("Deleting cascade deployment with name '" + deployment.Name + "/" + deployment.Id + "'.");
			// always cascade & skip custom listeners
			processEngine.RepositoryService.deleteDeployment(deployment.Id, true, true);

		  }
		  catch (Exception e)
		  {
			LOGGER.log(Level.WARNING, "Exception while deleting process engine deployment", e);

		  }

		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ProcessApplicationDeploymentService getValue() throws IllegalStateException, IllegalArgumentException
	  public virtual ProcessApplicationDeploymentService Value
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual InjectedValue<ProcessEngine> ProcessEngineInjector
	  {
		  get
		  {
			return processEngineInjector;
		  }
	  }

	  public virtual InjectedValue<ProcessApplicationInterface> NoViewProcessApplication
	  {
		  get
		  {
			return noViewProcessApplication;
		  }
	  }

	  public virtual InjectedValue<ComponentView> PaComponentViewInjector
	  {
		  get
		  {
			return paComponentViewInjector;
		  }
	  }

	  public virtual ProcessApplicationDeployment Deployment
	  {
		  get
		  {
			return deployment;
		  }
	  }

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineInjector.Value.Name;
		  }
	  }

	  public virtual InjectedValue<ExecutorService> ExecutorInjector
	  {
		  get
		  {
			return executorInjector;
		  }
	  }


	}

}