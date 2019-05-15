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
namespace org.camunda.bpm.application.impl.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;

	using TestCase = junit.framework.TestCase;

	/// <summary>
	/// <para>Testcase verifying the default properties in the empty processes.xml</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EmptyProcessesXmlTest : TestCase
	{

	  public virtual void testDefaultValues()
	  {

		ProcessesXml emptyProcessesXml = ProcessesXml.EMPTY_PROCESSES_XML;
		assertNotNull(emptyProcessesXml);

		assertNotNull(emptyProcessesXml.ProcessEngines);
		assertEquals(0, emptyProcessesXml.ProcessEngines.Count);

		assertNotNull(emptyProcessesXml.ProcessArchives);
		assertEquals(1, emptyProcessesXml.ProcessArchives.Count);

		ProcessArchiveXml processArchiveXml = emptyProcessesXml.ProcessArchives[0];

		assertNull(processArchiveXml.Name);
		assertNull(processArchiveXml.ProcessEngineName);

		assertNotNull(processArchiveXml.ProcessResourceNames);
		assertEquals(0, processArchiveXml.ProcessResourceNames.Count);

		IDictionary<string, string> properties = processArchiveXml.Properties;

		assertNotNull(properties);
		assertEquals(4, properties.Count);

		string isDeleteUponUndeploy = properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DELETE_UPON_UNDEPLOY];
		assertNotNull(isDeleteUponUndeploy);
		assertEquals(false.ToString(), isDeleteUponUndeploy);

		string isScanForProcessDefinitions = properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS];
		assertNotNull(isScanForProcessDefinitions);
		assertEquals(true.ToString(), isScanForProcessDefinitions);

		string isDeployChangedOnly = properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_IS_DEPLOY_CHANGED_ONLY];
		assertNotNull(isDeployChangedOnly);
		assertEquals(false.ToString(), isDeployChangedOnly);

		string resumePreviousBy = properties[org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml_Fields.PROP_RESUME_PREVIOUS_BY];
		assertThat(resumePreviousBy, @is(notNullValue()));
		assertThat(resumePreviousBy, @is(ResumePreviousBy.RESUME_BY_PROCESS_DEFINITION_KEY));
	  }

	}

}