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
	using JmxManagedBpmPlatformPlugins = org.camunda.bpm.container.impl.jmx.services.JmxManagedBpmPlatformPlugins;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DiscoverBpmPlatformPluginsStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Discover BPM Platform Plugins";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		BpmPlatformPlugins plugins = BpmPlatformPlugins.load(PluginsClassloader);

		JmxManagedBpmPlatformPlugins jmxManagedPlugins = new JmxManagedBpmPlatformPlugins(plugins);
		serviceContainer.startService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_PLATFORM_PLUGINS, jmxManagedPlugins);

	  }

	  protected internal virtual ClassLoader PluginsClassloader
	  {
		  get
		  {
    
			ClassLoader pluginsClassLoader = ClassLoaderUtil.ContextClassloader;
			if (pluginsClassLoader == null)
			{
			  // if context classloader is null, use classloader which loaded the camunda-engine jar.
			  pluginsClassLoader = typeof(BpmPlatform).ClassLoader;
			}
    
			return pluginsClassLoader;
		  }
	  }

	}

}