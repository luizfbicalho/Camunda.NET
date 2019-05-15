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
namespace org.camunda.bpm.container.impl.parser
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using TestCase = junit.framework.TestCase;

	using BpmPlatformXmlParser = org.camunda.bpm.container.impl.metadata.BpmPlatformXmlParser;
	using BpmPlatformXml = org.camunda.bpm.container.impl.metadata.spi.BpmPlatformXml;
	using JobAcquisitionXml = org.camunda.bpm.container.impl.metadata.spi.JobAcquisitionXml;
	using JobExecutorXml = org.camunda.bpm.container.impl.metadata.spi.JobExecutorXml;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;

	/// <summary>
	/// <para>The testcases for the <seealso cref="BpmPlatformXmlParser"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmPlatformXmlParserTest : TestCase
	{

	  private BpmPlatformXmlParser parser;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		parser = new BpmPlatformXmlParser();
		base.setUp();
	  }

	  protected internal virtual URL getStreamUrl(string filename)
	  {
		return typeof(BpmPlatformXmlParserTest).getResource(filename);
	  }

	  public virtual void testParseBpmPlatformXmlNoEngine()
	  {

		BpmPlatformXml bpmPlatformXml = parser.createParse().sourceUrl(getStreamUrl("bpmplatform_xml_no_engine.xml")).execute().BpmPlatformXml;

		assertNotNull(bpmPlatformXml);
		assertNotNull(bpmPlatformXml.JobExecutor);
		assertEquals(0, bpmPlatformXml.ProcessEngines.Count);

		JobExecutorXml jobExecutorXml = bpmPlatformXml.JobExecutor;
		assertEquals(1, jobExecutorXml.JobAcquisitions.Count);

		JobAcquisitionXml jobAcquisitionXml = jobExecutorXml.JobAcquisitions[0];
		assertEquals("default", jobAcquisitionXml.Name);
		assertEquals("org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor", jobAcquisitionXml.JobExecutorClassName);

		assertEquals(2, jobAcquisitionXml.Properties.Count);

	  }

	  public virtual void testParseBpmPlatformXmlOneEngine()
	  {

		BpmPlatformXml bpmPlatformXml = parser.createParse().sourceUrl(getStreamUrl("bpmplatform_xml_one_engine.xml")).execute().BpmPlatformXml;

		assertNotNull(bpmPlatformXml);
		assertNotNull(bpmPlatformXml.JobExecutor);
		assertEquals(1, bpmPlatformXml.ProcessEngines.Count);

		JobExecutorXml jobExecutorXml = bpmPlatformXml.JobExecutor;
		assertEquals(1, jobExecutorXml.JobAcquisitions.Count);
		assertThat(jobExecutorXml.Properties.Count, @is(2));

		JobAcquisitionXml jobAcquisitionXml = jobExecutorXml.JobAcquisitions[0];
		assertEquals("default", jobAcquisitionXml.Name);
		assertEquals("org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor", jobAcquisitionXml.JobExecutorClassName);

		assertEquals(2, jobAcquisitionXml.Properties.Count);

		ProcessEngineXml engineXml = bpmPlatformXml.ProcessEngines[0];
		assertEquals("engine1", engineXml.Name);
		assertEquals("default", engineXml.JobAcquisitionName);

		IDictionary<string, string> properties = engineXml.Properties;
		assertNotNull(properties);
		assertEquals(0, properties.Count);

		IList<ProcessEnginePluginXml> plugins = engineXml.Plugins;
		assertNotNull(plugins);
		assertEquals(0, plugins.Count);

	  }

	  public virtual void testParseBpmPlatformXmlEnginePlugin()
	  {

		BpmPlatformXml bpmPlatformXml = parser.createParse().sourceUrl(getStreamUrl("bpmplatform_xml_engine_plugin.xml")).execute().BpmPlatformXml;

		assertNotNull(bpmPlatformXml);
		assertEquals(1, bpmPlatformXml.ProcessEngines.Count);

		ProcessEngineXml engineXml = bpmPlatformXml.ProcessEngines[0];
		assertEquals("engine1", engineXml.Name);
		assertEquals("default", engineXml.JobAcquisitionName);

		IList<ProcessEnginePluginXml> plugins = engineXml.Plugins;
		assertEquals(1, plugins.Count);

		ProcessEnginePluginXml plugin1 = plugins[0];
		assertNotNull(plugin1);

		assertEquals("org.camunda.bpm.MyAwesomePlugin", plugin1.PluginClass);

		IDictionary<string, string> properties = plugin1.Properties;
		assertNotNull(properties);
		assertEquals(2, properties.Count);

		string val1 = properties["prop1"];
		assertNotNull(val1);
		assertEquals("val1", val1);

		string val2 = properties["prop2"];
		assertNotNull(val2);
		assertEquals("val2", val2);

	  }

	  public virtual void testParseBpmPlatformXmlMultipleEnginePlugins()
	  {

		BpmPlatformXml bpmPlatformXml = parser.createParse().sourceUrl(getStreamUrl("bpmplatform_xml_multiple_engine_plugins.xml")).execute().BpmPlatformXml;

		assertNotNull(bpmPlatformXml);
		assertEquals(1, bpmPlatformXml.ProcessEngines.Count);

		ProcessEngineXml engineXml = bpmPlatformXml.ProcessEngines[0];
		assertEquals("engine1", engineXml.Name);
		assertEquals("default", engineXml.JobAcquisitionName);

		IList<ProcessEnginePluginXml> plugins = engineXml.Plugins;
		assertEquals(2, plugins.Count);

	  }

	  public virtual void testParseProcessesXmlAntStyleProperties()
	  {

		BpmPlatformXml platformXml = parser.createParse().sourceUrl(getStreamUrl("bpmplatform_xml_ant_style_properties.xml")).execute().BpmPlatformXml;

		assertNotNull(platformXml);

		ProcessEngineXml engineXml = platformXml.ProcessEngines[0];

		assertEquals(1, engineXml.Plugins.Count);
		ProcessEnginePluginXml pluginXml = engineXml.Plugins[0];

		IDictionary<string, string> properties = pluginXml.Properties;
		assertEquals(2, properties.Count);

		// these two system properties are guaranteed to be set
		assertEquals(System.getProperty("java.version"), properties["prop1"]);
		assertEquals("prefix-" + System.getProperty("os.name"), properties["prop2"]);
	  }

	}

}