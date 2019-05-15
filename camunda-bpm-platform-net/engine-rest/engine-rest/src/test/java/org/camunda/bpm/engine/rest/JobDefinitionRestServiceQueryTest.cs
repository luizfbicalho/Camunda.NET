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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using Response = io.restassured.response.Response;

	public class JobDefinitionRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string JOB_DEFINITION_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/job-definition";
	  protected internal static readonly string JOB_DEFINITION_COUNT_QUERY_URL = JOB_DEFINITION_QUERY_URL + "/count";
	  private JobDefinitionQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockDefinitionQuery(MockProvider.createMockJobDefinitions());
	  }

	  private JobDefinitionQuery setUpMockDefinitionQuery(IList<JobDefinition> mockedJobDefinitions)
	  {
		JobDefinitionQuery query = mock(typeof(JobDefinitionQuery));

		when(query.list()).thenReturn(mockedJobDefinitions);
		when(query.count()).thenReturn((long) mockedJobDefinitions.Count);
		when(processEngine.ManagementService.createJobDefinitionQuery()).thenReturn(query);

		return query;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryJobDefinitionId = "";
		given().queryParam("jobDefinitionId", queryJobDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQueryAsPost()
	  public virtual void testEmptyQueryAsPost()
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["jobDefinitionId"] = "";

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("jobDefinitionId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "jobDefinitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		// asc
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobConfiguration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobConfiguration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		// desc
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobType();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("jobConfiguration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByJobConfiguration();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = OrderingBuilder.create().orderBy("jobType").desc().orderBy("jobConfiguration").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		inOrder.verify(mockedQuery).orderByJobType();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByJobConfiguration();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

	  /// <summary>
	  /// If parameter "firstResult" is missing, we expect 0 as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

	  /// <summary>
	  /// If parameter "maxResults" is missing, we expect Integer.MAX_VALUE as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionRetrieval()
	  public virtual void testJobDefinitionRetrieval()
	  {
		string queryJobDefinitionId = "aJobDefId";
		Response response = given().queryParam("jobDefinitionId", queryJobDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).jobDefinitionId(queryJobDefinitionId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> jobDefinitions = from(content).getList("");

		assertThat(jobDefinitions).hasSize(1);
		assertThat(jobDefinitions[0]).NotNull;

		string returnedId = from(content).getString("[0].id");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedJobType = from(content).getString("[0].jobType");
		string returnedJobConfiguration = from(content).getString("[0].jobConfiguration");
		bool returnedSuspensionState = from(content).getBoolean("[0].suspended");
		long? returnedJobPriority = from(content).getObject("[0].overridingJobPriority", typeof(Long));

		assertThat(returnedId).isEqualTo(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		assertThat(returnedProcessDefinitionId).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		assertThat(returnedProcessDefinitionKey).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		assertThat(returnedActivityId).isEqualTo(MockProvider.EXAMPLE_ACTIVITY_ID);
		assertThat(returnedJobType).isEqualTo(MockProvider.EXAMPLE_JOB_TYPE);
		assertThat(returnedJobConfiguration).isEqualTo(MockProvider.EXAMPLE_JOB_CONFIG);
		assertThat(returnedSuspensionState).isEqualTo(MockProvider.EXAMPLE_JOB_DEFINITION_IS_SUSPENDED);
		assertThat(returnedJobPriority).isEqualTo(MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionRetrievalAsPost()
	  public virtual void testJobDefinitionRetrievalAsPost()
	  {
		string queryJobDefinitionId = "aJobDefId";
		IDictionary<string, string> queryParameter = new Dictionary<string, string>();
		queryParameter["jobDefinitionId"] = queryJobDefinitionId;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameter).then().expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).jobDefinitionId(queryJobDefinitionId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> jobDefinitions = from(content).getList("");

		assertThat(jobDefinitions).hasSize(1);
		assertThat(jobDefinitions[0]).NotNull;

		string returnedId = from(content).getString("[0].id");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedJobType = from(content).getString("[0].jobType");
		string returnedJobConfiguration = from(content).getString("[0].jobConfiguration");
		bool returnedSuspensionState = from(content).getBoolean("[0].suspended");

		assertThat(returnedId).isEqualTo(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		assertThat(returnedProcessDefinitionId).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		assertThat(returnedProcessDefinitionKey).isEqualTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		assertThat(returnedActivityId).isEqualTo(MockProvider.EXAMPLE_ACTIVITY_ID);
		assertThat(returnedJobType).isEqualTo(MockProvider.EXAMPLE_JOB_TYPE);
		assertThat(returnedJobConfiguration).isEqualTo(MockProvider.EXAMPLE_JOB_CONFIG);
		assertThat(returnedSuspensionState).isEqualTo(MockProvider.EXAMPLE_JOB_DEFINITION_IS_SUSPENDED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParameters()
	  public virtual void testMultipleParameters()
	  {
		IDictionary<string, string> queryParameters = new Dictionary<string, string>();

		queryParameters["jobDefinitionId"] = "aJobDefId";
		queryParameters["processDefinitionId"] = "aProcDefId";
		queryParameters["processDefinitionKey"] = "aProcDefKey";
		queryParameters["activityIdIn"] = "aActId,anotherActId";
		queryParameters["jobType"] = "aJobType";
		queryParameters["jobConfiguration"] = "aJobConfig";
		queryParameters["suspended"] = "true";
		queryParameters["active"] = "true";
		queryParameters["withOverridingJobPriority"] = "true";

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).jobDefinitionId(queryParameters["jobDefinitionId"]);
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["processDefinitionKey"]);
		verify(mockedQuery).activityIdIn("aActId", "anotherActId");
		verify(mockedQuery).jobType(queryParameters["jobType"]);
		verify(mockedQuery).jobConfiguration(queryParameters["jobConfiguration"]);
		verify(mockedQuery).active();
		verify(mockedQuery).suspended();
		verify(mockedQuery).withOverridingJobPriority();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParametersAsPost()
	  public virtual void testMultipleParametersAsPost()
	  {
		string aJobDefId = "aJobDefId";
		string aProcDefId = "aProcDefId";
		string aProcDefKey = "aProcDefKey";
		string aActId = "aActId";
		string anotherActId = "anotherActId";
		string aJobType = "aJobType";
		string aJobConfig = "aJobConfig";

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();

		queryParameters["jobDefinitionId"] = aJobDefId;
		queryParameters["processDefinitionId"] = aProcDefId;
		queryParameters["processDefinitionKey"] = aProcDefKey;
		queryParameters["jobType"] = aJobType;
		queryParameters["jobConfiguration"] = aJobConfig;
		queryParameters["suspended"] = "true";
		queryParameters["active"] = "true";
		queryParameters["withOverridingJobPriority"] = "true";

		IList<string> activityIdIn = new List<string>();
		activityIdIn.Add(aActId);
		activityIdIn.Add(anotherActId);
		queryParameters["activityIdIn"] = activityIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).jobDefinitionId(aJobDefId);
		verify(mockedQuery).processDefinitionId(aProcDefId);
		verify(mockedQuery).processDefinitionKey(aProcDefKey);
		verify(mockedQuery).activityIdIn(aActId, anotherActId);
		verify(mockedQuery).jobType(aJobType);
		verify(mockedQuery).jobConfiguration(aJobConfig);
		verify(mockedQuery).active();
		verify(mockedQuery).suspended();
		verify(mockedQuery).withOverridingJobPriority();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(JOB_DEFINITION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountAsPost()
	  public virtual void testQueryCountAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(JOB_DEFINITION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockDefinitionQuery(createMockJobDefinitionsTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(1);

		string returnedTenantId = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeJobDefinitionsWithoutTenantIdParameter()
	  public virtual void testIncludeJobDefinitionsWithoutTenantIdParameter()
	  {
		IList<JobDefinition> jobDefinitions = Arrays.asList(MockProvider.mockJobDefinition().tenantId(null).build(), MockProvider.mockJobDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockedQuery = setUpMockDefinitionQuery(jobDefinitions);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID).queryParam("includeJobDefinitionsWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockedQuery).includeJobDefinitionsWithoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockDefinitionQuery(createMockJobDefinitionsTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdPostParameter()
	  public virtual void testWithoutTenantIdPostParameter()
	  {
		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(1);

		string returnedTenantId = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeJobDefinitionsWithoutTenantIdPostParameter()
	  public virtual void testIncludeJobDefinitionsWithoutTenantIdPostParameter()
	  {
		IList<JobDefinition> jobDefinitions = Arrays.asList(MockProvider.mockJobDefinition().tenantId(null).build(), MockProvider.mockJobDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockedQuery = setUpMockDefinitionQuery(jobDefinitions);

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = new string[] {MockProvider.EXAMPLE_TENANT_ID};
		queryParameters["includeJobDefinitionsWithoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockedQuery).includeJobDefinitionsWithoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

	  private IList<JobDefinition> createMockJobDefinitionsTwoTenants()
	  {
		return Arrays.asList(MockProvider.mockJobDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockJobDefinition().tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
	  }

	}

}