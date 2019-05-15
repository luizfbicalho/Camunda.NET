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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using SchemaLogEntryEntity = org.camunda.bpm.engine.impl.persistence.entity.SchemaLogEntryEntity;
	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using Mockito = org.mockito.Mockito;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogRestServiceQueryTest : AbstractRestServiceTest
	{

	  /// 
	  private static readonly string SCHEMA_LOG_URL = TEST_RESOURCE_ROOT_PATH + SchemaLogRestService_Fields.PATH;

	  private const string SCHEMA_LOG_ENTRY_MOCK_ID = "schema-log-entry-mock-id";
	  private const string SCHEMA_LOG_ENTRY_MOCK_VERSION = "schema-log-entry-mock-version";
	  private static readonly DateTime SCHEMA_LOG_ENTRY_MOCK_TIMESTAMP = DateTime.Now;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  private SchemaLogQuery mockedQuery;

	  private IList<SchemaLogEntry> mockedSchemaLogEntries;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		mockedQuery = Mockito.mock(typeof(SchemaLogQuery));

		mockedSchemaLogEntries = createMockedSchemaLogEntries();
		when(mockedQuery.list()).thenReturn(mockedSchemaLogEntries);

		when(processEngine.ManagementService.createSchemaLogQuery()).thenReturn(mockedQuery);
	  }

	  private IList<SchemaLogEntry> createMockedSchemaLogEntries()
	  {
		IList<SchemaLogEntry> entries = new List<SchemaLogEntry>();
		SchemaLogEntryEntity entry = new SchemaLogEntryEntity();

		entry.Id = SCHEMA_LOG_ENTRY_MOCK_ID;
		entry.Version = SCHEMA_LOG_ENTRY_MOCK_VERSION;
		entry.Timestamp = SCHEMA_LOG_ENTRY_MOCK_TIMESTAMP;

		entries.Add(entry);
		return entries;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSchemaLog()
	  public virtual void testGetSchemaLog()
	  {
		given().queryParam("version", SCHEMA_LOG_ENTRY_MOCK_VERSION).queryParam("sortBy", "timestamp").queryParam("sortOrder", "asc").expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("schemaLogEntries[0].version", @is(SCHEMA_LOG_ENTRY_MOCK_VERSION)).body("schemaLogEntries[0].timestamp", notNullValue()).when().get(SCHEMA_LOG_URL);

		verify(mockedQuery).version(SCHEMA_LOG_ENTRY_MOCK_VERSION);
		verify(mockedQuery).orderByTimestamp();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSchemaLogAsPost()
	  public virtual void testGetSchemaLogAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["version"] = SCHEMA_LOG_ENTRY_MOCK_VERSION;
		@params["sortBy"] = "timestamp";
		@params["sortOrder"] = "asc";

		given().contentType(MediaType.APPLICATION_JSON).body(@params).expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("schemaLogEntries[0].version", @is(SCHEMA_LOG_ENTRY_MOCK_VERSION)).body("schemaLogEntries[0].timestamp", notNullValue()).when().post(SCHEMA_LOG_URL);

		verify(mockedQuery).version(SCHEMA_LOG_ENTRY_MOCK_VERSION);
		verify(mockedQuery).orderByTimestamp();
		verify(mockedQuery).list();
	  }
	}

}