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
namespace org.camunda.bpm.engine.impl.cfg
{


	/// <summary>
	/// <seealso cref="ProcessEnginePlugin"/> that provides composite behavior. When registered on an engine configuration,
	/// all plugins added to this composite will be triggered on preInit/postInit/postProcessEngineBuild.
	/// <para>
	/// Use to encapsulate common behavior (like engine configuration).
	/// </para>
	/// </summary>
	public class CompositeProcessEnginePlugin : AbstractProcessEnginePlugin
	{

	  protected internal readonly IList<ProcessEnginePlugin> plugins;

	  /// <summary>
	  /// New instance with empty list.
	  /// </summary>
	  public CompositeProcessEnginePlugin()
	  {
		this.plugins = new List<ProcessEnginePlugin>();
	  }


	  /// <summary>
	  /// New instance with vararg. </summary>
	  /// <param name="plugin"> first plugin </param>
	  /// <param name="additionalPlugins"> additional vararg plugins </param>
	  public CompositeProcessEnginePlugin(ProcessEnginePlugin plugin, params ProcessEnginePlugin[] additionalPlugins) : this()
	  {
		addProcessEnginePlugin(plugin, additionalPlugins);
	  }

	  /// <summary>
	  /// New instance with initial plugins.
	  /// </summary>
	  /// <param name="plugins"> the initial plugins. Must not be null. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeProcessEnginePlugin(final java.util.List<ProcessEnginePlugin> plugins)
	  public CompositeProcessEnginePlugin(IList<ProcessEnginePlugin> plugins) : this()
	  {
		addProcessEnginePlugins(plugins);
	  }

	  /// <summary>
	  /// Add one (or more) plugins.
	  /// </summary>
	  /// <param name="plugin"> first plugin </param>
	  /// <param name="additionalPlugins"> additional vararg plugins </param>
	  /// <returns> self for fluent usage </returns>
	  public virtual CompositeProcessEnginePlugin addProcessEnginePlugin(ProcessEnginePlugin plugin, params ProcessEnginePlugin[] additionalPlugins)
	  {
		return this.addProcessEnginePlugins(toList(plugin, additionalPlugins));
	  }

	  /// <summary>
	  /// Add collection of plugins.
	  /// 
	  /// If collection is not sortable, order of plugin execution can not be guaranteed.
	  /// </summary>
	  /// <param name="plugins"> plugins to add </param>
	  /// <returns> self for fluent usage </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public CompositeProcessEnginePlugin addProcessEnginePlugins(final java.util.Collection<ProcessEnginePlugin> plugins)
	  public virtual CompositeProcessEnginePlugin addProcessEnginePlugins(ICollection<ProcessEnginePlugin> plugins)
	  {
		((IList<ProcessEnginePlugin>)this.plugins).AddRange(plugins);

		return this;
	  }

	  public override void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		foreach (ProcessEnginePlugin plugin in plugins)
		{
		  plugin.preInit(processEngineConfiguration);
		}
	  }

	  public override void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		foreach (ProcessEnginePlugin plugin in plugins)
		{
		  plugin.postInit(processEngineConfiguration);
		}
	  }

	  public override void postProcessEngineBuild(ProcessEngine processEngine)
	  {
		foreach (ProcessEnginePlugin plugin in plugins)
		{
		  plugin.postProcessEngineBuild(processEngine);
		}
	  }

	  /// <summary>
	  /// Get all plugins.
	  /// </summary>
	  /// <returns> the configured plugins </returns>
	  public virtual IList<ProcessEnginePlugin> Plugins
	  {
		  get
		  {
			return plugins;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + plugins;
	  }


	  private static IList<ProcessEnginePlugin> toList(ProcessEnginePlugin plugin, params ProcessEnginePlugin[] additionalPlugins)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<ProcessEnginePlugin> plugins = new java.util.ArrayList<ProcessEnginePlugin>();
		IList<ProcessEnginePlugin> plugins = new List<ProcessEnginePlugin>();
		plugins.Add(plugin);
		if (additionalPlugins != null && additionalPlugins.Length > 0)
		{
		  ((IList<ProcessEnginePlugin>)plugins).AddRange(Arrays.asList(additionalPlugins));
		}
		return plugins;
	  }
	}

}