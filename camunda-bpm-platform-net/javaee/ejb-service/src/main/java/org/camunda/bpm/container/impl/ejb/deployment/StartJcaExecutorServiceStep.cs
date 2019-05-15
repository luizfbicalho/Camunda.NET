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
namespace org.camunda.bpm.container.impl.ejb.deployment
{

	using Attachments = org.camunda.bpm.container.impl.deployment.Attachments;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;

	/// <summary>
	/// <para>Deployment operation responsible registering a service which represents a Proxy to the
	/// JCA-Backed <seealso cref="ExecutorService"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StartJcaExecutorServiceStep : DeploymentOperationStep
	{

	  protected internal ExecutorService executorService;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOGGER = Logger.getLogger(typeof(StartJcaExecutorServiceStep).FullName);

	  public StartJcaExecutorServiceStep(ExecutorService executorService)
	  {
		this.executorService = executorService;
	  }

	  public override string Name
	  {
		  get
		  {
			return "Start JCA Executor Service";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {
		BpmPlatformXml bpmPlatformXml = operationContext.getAttachment(Attachments.BPM_PLATFORM_XML);
		checkConfiguration(bpmPlatformXml.JobExecutor);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		serviceContainer.startService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_EXECUTOR, new JcaExecutorServiceDelegate(executorService));

	  }

	  /// <summary>
	  /// Checks the validation to see if properties are present, which will be ignored
	  /// in this environment so we can log a warning.
	  /// </summary>
	  private void checkConfiguration(JobExecutorXml jobExecutorXml)
	  {
		IDictionary<string, string> properties = jobExecutorXml.Properties;
		foreach (KeyValuePair<string, string> entry in properties.SetOfKeyValuePairs())
		{
		  LOGGER.warning("Property " + entry.Key + " with value " + entry.Value + " from bpm-platform.xml will be ignored for JobExecutor.");
		}
	  }

	}

}