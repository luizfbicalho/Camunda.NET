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
	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using JmxManagedBpmPlatformPlugins = org.camunda.bpm.container.impl.jmx.services.JmxManagedBpmPlatformPlugins;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class NotifyPostProcessApplicationUndeployedStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "NotifyPostProcessApplicationUndeployedStep";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);
		AbstractProcessApplication processApplication = operationContext.getAttachment(Attachments.PROCESS_APPLICATION);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.jmx.services.JmxManagedBpmPlatformPlugins plugins = serviceContainer.getService(org.camunda.bpm.container.impl.spi.ServiceTypes.BPM_PLATFORM, org.camunda.bpm.container.impl.RuntimeContainerDelegateImpl.SERVICE_NAME_PLATFORM_PLUGINS);
		JmxManagedBpmPlatformPlugins plugins = serviceContainer.getService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_PLATFORM_PLUGINS);

		if (plugins != null)
		{
		  foreach (BpmPlatformPlugin plugin in plugins.Value.Plugins)
		  {
			plugin.postProcessApplicationUndeploy(processApplication);
		  }
		}
	  }

	}

}