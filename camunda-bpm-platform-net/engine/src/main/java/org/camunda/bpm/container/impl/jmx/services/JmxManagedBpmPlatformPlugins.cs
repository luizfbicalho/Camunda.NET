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
namespace org.camunda.bpm.container.impl.jmx.services
{

	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JmxManagedBpmPlatformPlugins : PlatformService<BpmPlatformPlugins>, JmxManagedBpmPlatformPluginsMBean
	{

	  protected internal BpmPlatformPlugins plugins;

	  public JmxManagedBpmPlatformPlugins(BpmPlatformPlugins plugins)
	  {
		this.plugins = plugins;
	  }

	  public virtual void start(PlatformServiceContainer mBeanServiceContainer)
	  {
		// no callbacks or initialization in the plugins
	  }

	  public virtual void stop(PlatformServiceContainer mBeanServiceContainer)
	  {
		// no callbacks or initialization in the plugins
	  }

	  public virtual BpmPlatformPlugins Value
	  {
		  get
		  {
			return plugins;
		  }
	  }

	  public virtual string[] PluginNames
	  {
		  get
		  {
			// expose names of discovered plugins in JMX
			IList<BpmPlatformPlugin> pluginList = plugins.Plugins;
			string[] names = new string[pluginList.Count];
			for (int i = 0; i < names.Length; i++)
			{
			  BpmPlatformPlugin bpmPlatformPlugin = pluginList[i];
			  if (bpmPlatformPlugin != null)
			  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				names[i] = bpmPlatformPlugin.GetType().FullName;
			  }
			}
			return names;
		  }
	  }

	}

}