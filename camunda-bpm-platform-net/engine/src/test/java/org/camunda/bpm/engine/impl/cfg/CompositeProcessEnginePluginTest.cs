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
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	public class CompositeProcessEnginePluginTest
	{

	  private static readonly ProcessEnginePlugin PLUGIN_A = processEnginePlugin("PluginA");
	  private static readonly ProcessEnginePlugin PLUGIN_B = processEnginePlugin("PluginB");
	  private static readonly InOrder ORDER = inOrder(PLUGIN_A, PLUGIN_B);

	  private static readonly ProcessEngineConfigurationImpl CONFIGURATION = mock(typeof(ProcessEngineConfigurationImpl));
	  private static readonly ProcessEngine ENGINE = mock(typeof(ProcessEngine));
	  private InOrder inOrder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addPlugin() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void addPlugin()
	  {
		CompositeProcessEnginePlugin composite = new CompositeProcessEnginePlugin(PLUGIN_A);

		assertThat(composite.Plugins.Count, @is(1));
		assertThat(composite.Plugins[0], @is(PLUGIN_A));

		composite.addProcessEnginePlugin(PLUGIN_B);
		assertThat(composite.Plugins.Count, @is(2));
		assertThat(composite.Plugins[1], @is(PLUGIN_B));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void addPlugins() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void addPlugins()
	  {
		CompositeProcessEnginePlugin composite = new CompositeProcessEnginePlugin(PLUGIN_A);
		composite.addProcessEnginePlugins(Arrays.asList(PLUGIN_B));

		assertThat(composite.Plugins.Count, @is(2));
		assertThat(composite.Plugins[0], @is(PLUGIN_A));
		assertThat(composite.Plugins[1], @is(PLUGIN_B));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void allPluginsOnPreInit() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void allPluginsOnPreInit()
	  {
		(new CompositeProcessEnginePlugin(PLUGIN_A, PLUGIN_B)).preInit(CONFIGURATION);

		ORDER.verify(PLUGIN_A).preInit(CONFIGURATION);
		ORDER.verify(PLUGIN_B).preInit(CONFIGURATION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void allPluginsOnPostInit() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void allPluginsOnPostInit()
	  {
		(new CompositeProcessEnginePlugin(PLUGIN_A, PLUGIN_B)).postInit(CONFIGURATION);

		ORDER.verify(PLUGIN_A).postInit(CONFIGURATION);
		ORDER.verify(PLUGIN_B).postInit(CONFIGURATION);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void allPluginsOnPostProcessEngineBuild() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void allPluginsOnPostProcessEngineBuild()
	  {
		(new CompositeProcessEnginePlugin(PLUGIN_A, PLUGIN_B)).postProcessEngineBuild(ENGINE);

		ORDER.verify(PLUGIN_A).postProcessEngineBuild(ENGINE);
		ORDER.verify(PLUGIN_B).postProcessEngineBuild(ENGINE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void verifyToString() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void verifyToString()
	  {
		assertThat((new CompositeProcessEnginePlugin(PLUGIN_A, PLUGIN_B)).ToString(), @is("CompositeProcessEnginePlugin[PluginA, PluginB]"));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static ProcessEnginePlugin processEnginePlugin(final String name)
	  private static ProcessEnginePlugin processEnginePlugin(string name)
	  {
		ProcessEnginePlugin plugin = Mockito.mock(typeof(ProcessEnginePlugin));
		when(plugin.ToString()).thenReturn(name);

		return plugin;
	  }
	}

}