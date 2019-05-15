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
namespace org.camunda.bpm.application.impl.deployment.parser
{

	using ProcessesXmlParser = org.camunda.bpm.application.impl.metadata.ProcessesXmlParser;
	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;

	using TestCase = junit.framework.TestCase;

	/// <summary>
	/// <para>The testcases for the <seealso cref="ProcessesXmlParser"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessesXmlParserTest : TestCase
	{

	  private ProcessesXmlParser parser;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		parser = new ProcessesXmlParser();
		base.setUp();
	  }

	  protected internal virtual URL getStreamUrl(string filename)
	  {
		return typeof(ProcessesXmlParserTest).getResource(filename);
	  }

	  public virtual void testParseProcessesXmlOneEngine()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_one_engine.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(1, processesXml.ProcessEngines.Count);
		assertEquals(0, processesXml.ProcessArchives.Count);

		ProcessEngineXml engineXml = processesXml.ProcessEngines[0];
		assertEquals("default", engineXml.Name);
		assertEquals("default", engineXml.JobAcquisitionName);
		assertEquals("configuration", engineXml.ConfigurationClass);
		assertEquals("datasource", engineXml.Datasource);

		IDictionary<string, string> properties = engineXml.Properties;
		assertNotNull(properties);
		assertEquals(2, properties.Count);

		assertEquals("value1", properties["prop1"]);
		assertEquals("value2", properties["prop2"]);

	  }

	  public virtual void testParseProcessesXmlTwoEngines()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_two_engines.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(2, processesXml.ProcessEngines.Count);
		assertEquals(0, processesXml.ProcessArchives.Count);

		ProcessEngineXml engineXml1 = processesXml.ProcessEngines[0];
		assertEquals("engine1", engineXml1.Name);
		assertEquals("configuration", engineXml1.ConfigurationClass);
		assertEquals("datasource", engineXml1.Datasource);

		IDictionary<string, string> properties1 = engineXml1.Properties;
		assertNotNull(properties1);
		assertEquals(2, properties1.Count);

		assertEquals("value1", properties1["prop1"]);
		assertEquals("value2", properties1["prop2"]);

		ProcessEngineXml engineXml2 = processesXml.ProcessEngines[1];
		assertEquals("engine2", engineXml2.Name);
		assertEquals("configuration", engineXml2.ConfigurationClass);
		assertEquals("datasource", engineXml2.Datasource);

		// the second engine has no properties
		IDictionary<string, string> properties2 = engineXml2.Properties;
		assertNotNull(properties2);
		assertEquals(0, properties2.Count);

	  }

	  public virtual void testParseProcessesXmlOneArchive()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_one_archive.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(0, processesXml.ProcessEngines.Count);
		assertEquals(1, processesXml.ProcessArchives.Count);

		ProcessArchiveXml archiveXml1 = processesXml.ProcessArchives[0];
		assertEquals("pa1", archiveXml1.Name);
		assertEquals("default", archiveXml1.ProcessEngineName);

		IList<string> resourceNames = archiveXml1.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames[0]);
		assertEquals("process2.bpmn", resourceNames[1]);

		IDictionary<string, string> properties1 = archiveXml1.Properties;
		assertNotNull(properties1);
		assertEquals(2, properties1.Count);

		assertEquals("value1", properties1["prop1"]);
		assertEquals("value2", properties1["prop2"]);

	  }

	  public virtual void testParseProcessesXmlTwoArchives()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_two_archives.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(0, processesXml.ProcessEngines.Count);
		assertEquals(2, processesXml.ProcessArchives.Count);


		ProcessArchiveXml archiveXml1 = processesXml.ProcessArchives[0];
		assertEquals("pa1", archiveXml1.Name);
		assertEquals("default", archiveXml1.ProcessEngineName);

		IList<string> resourceNames = archiveXml1.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames[0]);
		assertEquals("process2.bpmn", resourceNames[1]);

		IDictionary<string, string> properties1 = archiveXml1.Properties;
		assertNotNull(properties1);
		assertEquals(2, properties1.Count);

		assertEquals("value1", properties1["prop1"]);
		assertEquals("value2", properties1["prop2"]);

		ProcessArchiveXml archiveXml2 = processesXml.ProcessArchives[1];
		assertEquals("pa2", archiveXml2.Name);
		assertEquals("default", archiveXml2.ProcessEngineName);

		IList<string> resourceNames2 = archiveXml2.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames2[0]);
		assertEquals("process2.bpmn", resourceNames2[1]);

		IDictionary<string, string> properties2 = archiveXml2.Properties;
		assertNotNull(properties2);
		assertEquals(0, properties2.Count);

	  }

	  public virtual void testParseProcessesXmlTwoArchivesAndTwoEngines()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_two_archives_two_engines.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(2, processesXml.ProcessEngines.Count);
		assertEquals(2, processesXml.ProcessArchives.Count);

		// validate archives

		ProcessArchiveXml archiveXml1 = processesXml.ProcessArchives[0];
		assertEquals("pa1", archiveXml1.Name);
		assertEquals("default", archiveXml1.ProcessEngineName);

		IList<string> resourceNames = archiveXml1.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames[0]);
		assertEquals("process2.bpmn", resourceNames[1]);

		IDictionary<string, string> properties1 = archiveXml1.Properties;
		assertNotNull(properties1);
		assertEquals(2, properties1.Count);

		assertEquals("value1", properties1["prop1"]);
		assertEquals("value2", properties1["prop2"]);

		ProcessArchiveXml archiveXml2 = processesXml.ProcessArchives[1];
		assertEquals("pa2", archiveXml2.Name);
		assertEquals("default", archiveXml2.ProcessEngineName);

		IList<string> resourceNames2 = archiveXml2.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames2[0]);
		assertEquals("process2.bpmn", resourceNames2[1]);

		IDictionary<string, string> properties2 = archiveXml2.Properties;
		assertNotNull(properties2);
		assertEquals(0, properties2.Count);

		// validate engines

		ProcessEngineXml engineXml1 = processesXml.ProcessEngines[0];
		assertEquals("engine1", engineXml1.Name);
		assertEquals("configuration", engineXml1.ConfigurationClass);
		assertEquals("datasource", engineXml1.Datasource);

		properties1 = engineXml1.Properties;
		assertNotNull(properties1);
		assertEquals(2, properties1.Count);

		assertEquals("value1", properties1["prop1"]);
		assertEquals("value2", properties1["prop2"]);

		ProcessEngineXml engineXml2 = processesXml.ProcessEngines[1];
		assertEquals("engine2", engineXml2.Name);
		assertEquals("configuration", engineXml2.ConfigurationClass);
		assertEquals("datasource", engineXml2.Datasource);

		// the second engine has no properties
		properties2 = engineXml2.Properties;
		assertNotNull(properties2);
		assertEquals(0, properties2.Count);

	  }

	  public virtual void testParseProcessesXmlEngineNoName()
	  {

		// this test is to make sure that XML Schema Validation works.
		try
		{
		  parser.createParse().sourceUrl(getStreamUrl("process_xml_engine_no_name.xml")).execute();

		  fail("exception expected");

		}
		catch (ProcessEngineException)
		{
		  // expected
		}

	  }

	  public virtual void FAILING_testParseProcessesXmlClassLineBreak()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_one_archive_with_line_break.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		ProcessArchiveXml archiveXml1 = processesXml.ProcessArchives[0];
		IList<string> resourceNames = archiveXml1.ProcessResourceNames;
		assertEquals(2, resourceNames.Count);
		assertEquals("process1.bpmn", resourceNames[0]);

	  }

	  public virtual void testParseProcessesXmlNsPrefix()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_ns_prefix.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);

		assertEquals(1, processesXml.ProcessEngines.Count);
		assertEquals(1, processesXml.ProcessArchives.Count);

	  }

	  public virtual void testParseProcessesXmlTenantId()
	  {

		ProcessesXml processesXml = parser.createParse().sourceUrl(getStreamUrl("process_xml_tenant_id.xml")).execute().ProcessesXml;

		assertNotNull(processesXml);
		assertEquals(2, processesXml.ProcessArchives.Count);

		ProcessArchiveXml archiveXmlWithoutTenantId = processesXml.ProcessArchives[0];
		assertNull(archiveXmlWithoutTenantId.TenantId);

		ProcessArchiveXml archiveXmlWithTenantId = processesXml.ProcessArchives[1];
		assertEquals("tenant1", archiveXmlWithTenantId.TenantId);
	  }

	}

}