using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.rest
{
	using ClientResponse = com.sun.jersey.api.client.ClientResponse;
	using WebResource = com.sun.jersey.api.client.WebResource;

	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using JSONArray = org.codehaus.jettison.json.JSONArray;
	using JSONException = org.codehaus.jettison.json.JSONException;
	using JSONObject = org.codehaus.jettison.json.JSONObject;
	using Assert = org.junit.Assert;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	public class RestIT : AbstractWebappIntegrationTest
	{

	  private const string ENGINE_DEFAULT_PATH = "engine/default";

	  private static readonly string PROCESS_DEFINITION_PATH = ENGINE_DEFAULT_PATH + "/process-definition";

	  private static readonly string JOB_DEFINITION_PATH = ENGINE_DEFAULT_PATH + "/job-definition";

	  private static readonly string TASK_PATH = ENGINE_DEFAULT_PATH + "/task";

	  private static readonly string FILTER_PATH = ENGINE_DEFAULT_PATH + "/filter";

	  private static readonly string HISTORIC_DETAIL_PATH = ENGINE_DEFAULT_PATH + "/history/detail";

	  private static readonly string PROCESS_INSTANCE_PATH = ENGINE_DEFAULT_PATH + "/process-instance";


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger log = Logger.getLogger(typeof(RestIT).FullName);

	  protected internal override string ApplicationContextPath
	  {
		  get
		  {
			return "engine-rest/";
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setup() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static void setup()
	  {
		// just wait some seconds before starting because of Wildfly / Cargo race conditions
		Thread.Sleep(5 * 1000);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScenario() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScenario()
	  {

		// FIXME: cannot do this on JBoss AS7, see https://app.camunda.com/jira/browse/CAM-787

		// get list of process engines
		// log.info("Checking " + APP_BASE_PATH + ENGINES_PATH);
		// WebResource resource = client.resource(APP_BASE_PATH + ENGINES_PATH);
		// ClientResponse response = resource.accept(MediaType.APPLICATION_JSON).get(ClientResponse.class);
		//
		// Assert.assertEquals(200, response.getStatus());
		//
		// JSONArray enginesJson = response.getEntity(JSONArray.class);
		// Assert.assertEquals(1, enginesJson.length());
		//
		// JSONObject engineJson = enginesJson.getJSONObject(0);
		// Assert.assertEquals("default", engineJson.getString("name"));
		//
		// response.close();

		// get process definitions for default engine
		log.info("Checking " + APP_BASE_PATH + PROCESS_DEFINITION_PATH);
		WebResource resource = client.resource(APP_BASE_PATH + PROCESS_DEFINITION_PATH);
		ClientResponse response = resource.accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));

		assertEquals(200, response.Status);

		JSONArray definitionsJson = response.getEntity(typeof(JSONArray));
		// invoice example
		assertEquals(2, definitionsJson.length());

		JSONObject definitionJson = definitionsJson.getJSONObject(0);

		assertEquals("invoice", definitionJson.getString("key"));
		assertEquals("http://www.omg.org/spec/BPMN/20100524/MODEL", definitionJson.getString("category"));
		assertEquals("Invoice Receipt", definitionJson.getString("name"));
		Assert.assertTrue(definitionJson.isNull("description"));
		Assert.assertTrue(definitionJson.getString("resource").contains("invoice.v1.bpmn"));
		assertFalse(definitionJson.getBoolean("suspended"));

		definitionJson = definitionsJson.getJSONObject(1);

		assertEquals("invoice", definitionJson.getString("key"));
		assertEquals("http://www.omg.org/spec/BPMN/20100524/MODEL", definitionJson.getString("category"));
		assertEquals("Invoice Receipt", definitionJson.getString("name"));
		Assert.assertTrue(definitionJson.isNull("description"));
		Assert.assertTrue(definitionJson.getString("resource").contains("invoice.v2.bpmn"));
		assertFalse(definitionJson.getBoolean("suspended"));

		response.close();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void assertJodaTimePresent()
	  public virtual void assertJodaTimePresent()
	  {
		log.info("Checking " + APP_BASE_PATH + TASK_PATH);

		WebResource resource = client.resource(APP_BASE_PATH + TASK_PATH);
		resource.queryParam("dueAfter", "2000-01-01T00-00-00");
		ClientResponse response = resource.accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));

		assertEquals(200, response.Status);

		JSONArray definitionsJson = response.getEntity(typeof(JSONArray));
		assertEquals(6, definitionsJson.length());

		response.close();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedJobDefinitionSuspension()
	  public virtual void testDelayedJobDefinitionSuspension()
	  {
		log.info("Checking " + APP_BASE_PATH + JOB_DEFINITION_PATH + "/suspended");

		WebResource resource = client.resource(APP_BASE_PATH + JOB_DEFINITION_PATH + "/suspended");

		IDictionary<string, object> requestBody = new Dictionary<string, object>();
		requestBody["processDefinitionKey"] = "jobExampleProcess";
		requestBody["suspended"] = true;
		requestBody["includeJobs"] = true;
		requestBody["executionDate"] = "2014-08-25T13:55:45";

		ClientResponse response = resource.accept(MediaType.APPLICATION_JSON).type(MediaType.APPLICATION_JSON).put(typeof(ClientResponse), requestBody);

		assertEquals(204, response.Status);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskQueryContentType()
	  public virtual void testTaskQueryContentType()
	  {
		string resourcePath = APP_BASE_PATH + TASK_PATH;
		log.info("Checking " + resourcePath);
		assertMediaTypesOfResource(resourcePath, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSingleTaskContentType() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSingleTaskContentType()
	  {
		// get id of first task
		string taskId = FirstTask.getString("id");

		string resourcePath = APP_BASE_PATH + TASK_PATH + "/" + taskId;
		log.info("Checking " + resourcePath);
		assertMediaTypesOfResource(resourcePath, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskFilterResultContentType() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTaskFilterResultContentType()
	  {
		// create filter for first task, so single result will not throw an exception
		JSONObject firstTask = FirstTask;
		IDictionary<string, object> query = new Dictionary<string, object>();
		query["taskDefinitionKey"] = firstTask.getString("taskDefinitionKey");
		query["processInstanceId"] = firstTask.getString("processInstanceId");
		IDictionary<string, object> filter = new Dictionary<string, object>();
		filter["resourceType"] = "Task";
		filter["name"] = "IT Test Filter";
		filter["query"] = query;

		ClientResponse response = client.resource(APP_BASE_PATH + FILTER_PATH + "/create").accept(MediaType.APPLICATION_JSON).entity(filter, MediaType.APPLICATION_JSON_TYPE).post(typeof(ClientResponse));
		assertEquals(200, response.Status);
		string filterId = response.getEntity(typeof(JSONObject)).getString("id");
		response.close();

		string resourcePath = APP_BASE_PATH + FILTER_PATH + "/" + filterId + "/list";
		log.info("Checking " + resourcePath);
		assertMediaTypesOfResource(resourcePath, true);

		resourcePath = APP_BASE_PATH + FILTER_PATH + "/" + filterId + "/singleResult";
		log.info("Checking " + resourcePath);
		assertMediaTypesOfResource(resourcePath, true);

		// delete test filter
		response = client.resource(APP_BASE_PATH + FILTER_PATH + "/" + filterId).delete(typeof(ClientResponse));
		assertEquals(204, response.Status);
		response.close();
	  }

	  /// <summary>
	  /// Tests that a feature implemented via Jackson-2 annotations works:
	  /// polymorphic serialization of historic details
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPolymorphicSerialization() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPolymorphicSerialization()
	  {
		JSONObject historicVariableUpdate = FirstHistoricVariableUpdates;

		// variable update specific property
		assertTrue(historicVariableUpdate.has("variableName"));

	  }

	  /// <summary>
	  /// Uses Jackson's object mapper directly
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceQuery()
	  public virtual void testProcessInstanceQuery()
	  {
		WebResource resource = client.resource(APP_BASE_PATH + PROCESS_INSTANCE_PATH);
		ClientResponse response = resource.queryParam("variables", "invoiceNumber_eq_GPFE-23232323").accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));

		JSONArray instancesJson = response.getEntity(typeof(JSONArray));
		response.close();

		assertEquals(200, response.Status);
		// invoice example instance
		assertEquals(2, instancesJson.length());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testComplexObjectJacksonSerialization() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testComplexObjectJacksonSerialization()
	  {
		WebResource resource = client.resource(APP_BASE_PATH + PROCESS_DEFINITION_PATH + "/statistics");
		ClientResponse response = resource.queryParam("incidents", "true").accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));

		JSONArray definitionStatistics = response.getEntity(typeof(JSONArray));
		response.close();

		assertEquals(200, response.Status);
		// invoice example instance
		assertEquals(2, definitionStatistics.length());

		// check that definition is also serialized
		for (int i = 0; i < definitionStatistics.length(); i++)
		{
		  JSONObject definitionStatistic = definitionStatistics.getJSONObject(i);
		  assertEquals("org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionStatisticsResultDto", definitionStatistic.getString("@class"));
		  assertEquals(0, definitionStatistic.getJSONArray("incidents").length());
		  JSONObject definition = definitionStatistic.getJSONObject("definition");
		  assertEquals("Invoice Receipt", definition.getString("name"));
		  assertFalse(definition.getBoolean("suspended"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOptionsRequest()
	  public virtual void testOptionsRequest()
	  {
		//since WAS 9 contains patched cxf, which does not support OPTIONS request, we have to test this
		string resourcePath = APP_BASE_PATH + FILTER_PATH;
		log.info("Send OPTIONS request to " + resourcePath);

		// given
		WebResource resource = client.resource(resourcePath);

		// when
		ClientResponse response = resource.options(typeof(ClientResponse));

		// then
		assertNotNull(response);
		assertEquals(200, response.Status);
		JSONObject entity = response.getEntity(typeof(JSONObject));
		assertNotNull(entity.has("links"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyBodyFilterIsActive() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testEmptyBodyFilterIsActive()
	  {
		ClientResponse response = client.resource(APP_BASE_PATH + FILTER_PATH + "/create").accept(MediaType.APPLICATION_JSON).entity(null, MediaType.APPLICATION_JSON_TYPE).post(typeof(ClientResponse));

		assertEquals(400, response.Status);
		response.close();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.codehaus.jettison.json.JSONObject getFirstTask() throws org.codehaus.jettison.json.JSONException
	  protected internal virtual JSONObject FirstTask
	  {
		  get
		  {
			ClientResponse response = client.resource(APP_BASE_PATH + TASK_PATH).accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));
			JSONArray tasks = response.getEntity(typeof(JSONArray));
			JSONObject firstTask = tasks.getJSONObject(0);
			response.close();
			return firstTask;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.codehaus.jettison.json.JSONObject getFirstHistoricVariableUpdates() throws org.codehaus.jettison.json.JSONException
	  protected internal virtual JSONObject FirstHistoricVariableUpdates
	  {
		  get
		  {
			ClientResponse response = client.resource(APP_BASE_PATH + HISTORIC_DETAIL_PATH).queryParam("variableUpdates", "true").accept(MediaType.APPLICATION_JSON).get(typeof(ClientResponse));
    
			JSONArray updates = response.getEntity(typeof(JSONArray));
			JSONObject firstUpdate = updates.getJSONObject(0);
			response.close();
			return firstUpdate;
		  }
	  }

	  protected internal virtual void assertMediaTypesOfResource(string resourcePath, bool postSupported)
	  {
		WebResource resource = client.resource(resourcePath);
		assertMediaTypes(resource, postSupported, MediaType.APPLICATION_JSON_TYPE);
		assertMediaTypes(resource, postSupported, MediaType.APPLICATION_JSON_TYPE, MediaType.WILDCARD);
		assertMediaTypes(resource, postSupported, MediaType.APPLICATION_JSON_TYPE, MediaType.APPLICATION_JSON);
		assertMediaTypes(resource, postSupported, Hal.APPLICATION_HAL_JSON_TYPE, Hal.APPLICATION_HAL_JSON);
		assertMediaTypes(resource, postSupported, Hal.APPLICATION_HAL_JSON_TYPE, Hal.APPLICATION_HAL_JSON, MediaType.APPLICATION_JSON + "; q=0.5");
		assertMediaTypes(resource, postSupported, MediaType.APPLICATION_JSON_TYPE, Hal.APPLICATION_HAL_JSON + "; q=0.5", MediaType.APPLICATION_JSON);
		assertMediaTypes(resource, postSupported, MediaType.APPLICATION_JSON_TYPE, Hal.APPLICATION_HAL_JSON + "; q=0.5 ", MediaType.APPLICATION_JSON + "; q=0.6");
		assertMediaTypes(resource, postSupported, Hal.APPLICATION_HAL_JSON_TYPE, Hal.APPLICATION_HAL_JSON + "; q=0.6", MediaType.APPLICATION_JSON + "; q=0.5");
	  }

	  protected internal virtual void assertMediaTypes(WebResource resource, bool postSupported, MediaType expectedMediaType, params string[] acceptMediaTypes)
	  {
		// test GET request
		ClientResponse response = resource.accept(acceptMediaTypes).get(typeof(ClientResponse));
		assertMediaType(response, expectedMediaType);
		response.close();

		if (postSupported)
		{
		  // test POST request
		  response = resource.accept(acceptMediaTypes).entity(Collections.EMPTY_MAP, MediaType.APPLICATION_JSON_TYPE).post(typeof(ClientResponse));
		  assertMediaType(response, expectedMediaType);
		  response.close();
		}
	  }

	  protected internal virtual void assertMediaType(ClientResponse response, MediaType expected)
	  {
		MediaType actual = response.Type;
		assertEquals(200, response.Status);
		// use startsWith cause sometimes server also returns quality parameters (e.g. websphere/wink)
		assertTrue("Expected: " + expected + " Actual: " + actual, actual.ToString().StartsWith(expected.ToString(), StringComparison.Ordinal));
	  }

	}

}