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
namespace org.camunda.bpm.container.impl.deployment.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.deployment.Attachments.PROCESS_APPLICATION;

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using JmxManagedJobExecutor = org.camunda.bpm.container.impl.jmx.services.JmxManagedJobExecutor;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using RuntimeContainerJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.RuntimeContainerJobExecutor;

	/// <summary>
	/// <para>Deployment operation step responsible for starting a JobEexecutor</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartJobAcquisitionStep : DeploymentOperationStep
	{

	  protected internal static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  protected internal readonly JobAcquisitionXml jobAcquisitionXml;

	  public StartJobAcquisitionStep(JobAcquisitionXml jobAcquisitionXml)
	  {
		this.jobAcquisitionXml = jobAcquisitionXml;

	  }

	  public override string Name
	  {
		  get
		  {
			return "Start job acquisition '" + jobAcquisitionXml.Name + "'";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(PROCESS_APPLICATION);

		ClassLoader configurationClassloader = null;

		if (processApplication != null)
		{
		  configurationClassloader = processApplication.ProcessApplicationClassloader;
		}
		else
		{
		  configurationClassloader = typeof(ProcessEngineConfiguration).ClassLoader;
		}

		string configurationClassName = jobAcquisitionXml.JobExecutorClassName;

		if (string.ReferenceEquals(configurationClassName, null) || configurationClassName.Length == 0)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  configurationClassName = typeof(RuntimeContainerJobExecutor).FullName;
		}

		// create & instantiate the job executor class
		Type jobExecutorClass = loadJobExecutorClass(configurationClassloader, configurationClassName);
		JobExecutor jobExecutor = instantiateJobExecutor(jobExecutorClass);

		// apply properties
		IDictionary<string, string> properties = jobAcquisitionXml.Properties;
		PropertyHelper.applyProperties(jobExecutor, properties);

		// construct service for job executor
		JmxManagedJobExecutor jmxManagedJobExecutor = new JmxManagedJobExecutor(jobExecutor);

		// deploy the job executor service into the container
		serviceContainer.startService(ServiceTypes.JOB_EXECUTOR, jobAcquisitionXml.Name, jmxManagedJobExecutor);
	  }


	  protected internal virtual JobExecutor instantiateJobExecutor(Type configurationClass)
	  {
		try
		{
		  return System.Activator.CreateInstance(configurationClass);
		}
		catch (Exception e)
		{
		  throw LOG.couldNotInstantiateJobExecutorClass(e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected Class loadJobExecutorClass(ClassLoader processApplicationClassloader, String jobExecutorClassname)
	  protected internal virtual Type loadJobExecutorClass(ClassLoader processApplicationClassloader, string jobExecutorClassname)
	  {
		try
		{
		  return (Type) processApplicationClassloader.loadClass(jobExecutorClassname);
		}
		catch (ClassNotFoundException e)
		{
		  throw LOG.couldNotLoadJobExecutorClass(e);
		}
	  }

	}

}