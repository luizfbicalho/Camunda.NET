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
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class DeploymentRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string DEPLOYMENT_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/deployment";
	  protected internal static readonly string DEPLOYMENT_COUNT_QUERY_URL = DEPLOYMENT_QUERY_URL + "/count";
	  private DeploymentQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockDeploymentQuery(MockProvider.createMockDeployments());
	  }

	  private DeploymentQuery setUpMockDeploymentQuery(IList<Deployment> mockedDeployments)
	  {
		DeploymentQuery sampleDeploymentQuery = mock(typeof(DeploymentQuery));
		when(sampleDeploymentQuery.list()).thenReturn(mockedDeployments);
		when(sampleDeploymentQuery.count()).thenReturn((long) mockedDeployments.Count);
		when(processEngine.RepositoryService.createDeploymentQuery()).thenReturn(sampleDeploymentQuery);
		return sampleDeploymentQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifyFailingSorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST, typeof(InvalidRequestException).Name, "Cannot set query parameter 'sortBy' to value 'anInvalidSortByOption'");
		executeAndVerifyFailingSorting("name", "anInvalidSortOrderOption", Status.BAD_REQUEST, typeof(InvalidRequestException).Name, "Cannot set query parameter 'sortOrder' to value 'anInvalidSortOrderOption'");
	  }

	  protected internal virtual void executeAndVerifySuccessfulSorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(DEPLOYMENT_QUERY_URL);
	  }

	  protected internal virtual void executeAndVerifyFailingSorting(string sortBy, string sortOrder, Status expectedStatus, string expectedErrorType, string expectedErrorMessage)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).contentType(ContentType.JSON).body("type", equalTo(expectedErrorType)).body("message", equalTo(expectedErrorMessage)).when().get(DEPLOYMENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "name").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(DEPLOYMENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(DEPLOYMENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentRetrieval()
	  public virtual void testDeploymentRetrieval()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);

		string queryKey = "Name";
		Response response = given().queryParam("nameLike", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		// assert query invocation
		inOrder.verify(mockedQuery).deploymentNameLike(queryKey);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> deployments = from(content).getList("");
		Assert.assertEquals("There should be one deployment returned.", 1, deployments.Count);
		Assert.assertNotNull("There should be one deployment returned", deployments[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedName = from(content).getString("[0].name");
		string returnedSource = from(content).getString("[0].source");
		string returnedDeploymentTime = from(content).getString("[0].deploymentTime");

		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_NAME, returnedName);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_SOURCE, returnedSource);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_TIME, returnedDeploymentTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		// assert query invocation
		verify(mockedQuery).deploymentName(queryParameters["name"]);
		verify(mockedQuery).deploymentNameLike(queryParameters["nameLike"]);
		verify(mockedQuery).deploymentId(queryParameters["id"]);
		verify(mockedQuery).deploymentSource(queryParameters["source"]);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutSourceParameter()
	  public virtual void testWithoutSourceParameter()
	  {

		given().queryParam("withoutSource", true).expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		// assert query invocation
		verify(mockedQuery).deploymentSource(null);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSourceAndWithoutSource()
	  public virtual void testSourceAndWithoutSource()
	  {
		given().queryParam("withoutSource", true).queryParam("source", "source").expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("The query parameters \"withoutSource\" and \"source\" cannot be used in combination.")).when().get(DEPLOYMENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentBefore()
	  public virtual void testDeploymentBefore()
	  {
		given().queryParam("before", MockProvider.EXAMPLE_DEPLOYMENT_TIME_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).deploymentBefore(any(typeof(DateTime)));
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentAfter()
	  public virtual void testDeploymentAfter()
	  {
		given().queryParam("after", MockProvider.EXAMPLE_DEPLOYMENT_TIME_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).deploymentAfter(any(typeof(DateTime)));
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentTenantIdList()
	  public virtual void testDeploymentTenantIdList()
	  {
		IList<Deployment> deployments = Arrays.asList(MockProvider.createMockDeployment(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockDeployment(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
		mockedQuery = setUpMockDeploymentQuery(deployments);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentWithoutTenantId()
	  public virtual void testDeploymentWithoutTenantId()
	  {
		Deployment mockDeployment = MockProvider.createMockDeployment(null);
		mockedQuery = setUpMockDeploymentQuery(Collections.singletonList(mockDeployment));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentTenantIdIncludeDefinitionsWithoutTenantid()
	  public virtual void testDeploymentTenantIdIncludeDefinitionsWithoutTenantid()
	  {
		IList<Deployment> mockDeployments = Arrays.asList(MockProvider.createMockDeployment(null), MockProvider.createMockDeployment(MockProvider.EXAMPLE_TENANT_ID));
		mockedQuery = setUpMockDeploymentQuery(mockDeployments);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID).queryParam("includeDeploymentsWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockedQuery).includeDeploymentsWithoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

	  private IDictionary<string, string> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["id"] = "depId";
			parameters["name"] = "name";
			parameters["nameLike"] = "nameLike";
			parameters["source"] = "source";
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("id", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("id", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("deploymentTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("deploymentTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("name", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("name", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeploymentName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySuccessfulSorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

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
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

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
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(DEPLOYMENT_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(DEPLOYMENT_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

	}

}