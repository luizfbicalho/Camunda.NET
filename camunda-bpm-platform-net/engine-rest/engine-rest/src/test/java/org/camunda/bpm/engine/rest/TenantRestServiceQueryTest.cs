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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using RequestSpecification = io.restassured.specification.RequestSpecification;

	public class TenantRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/tenant";
	  protected internal static readonly string COUNT_QUERY_URL = QUERY_URL + "/count";

	  private TenantQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		IList<Tenant> tenants = Collections.singletonList(MockProvider.createMockTenant());
		mockQuery = setUpMockQuery(tenants);
	  }

	  private TenantQuery setUpMockQuery(IList<Tenant> tenants)
	  {
		TenantQuery query = mock(typeof(TenantQuery));
		when(query.list()).thenReturn(tenants);
		when(query.count()).thenReturn((long) tenants.Count);

		when(processEngine.IdentityService.createTenantQuery()).thenReturn(query);

		return query;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void emptyQuery()
	  public virtual void emptyQuery()
	  {
		string queryKey = "";

		given().queryParam("name", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void noParametersQuery()
	  public virtual void noParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		verify(mockQuery).list();
		verifyNoMoreInteractions(mockQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantRetrieval()
	  public virtual void tenantRetrieval()
	  {
		string name = MockProvider.EXAMPLE_TENANT_NAME;

		Response response = given().queryParam("name", name).then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).tenantName(name);
		inOrder.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertThat(instances.Count, @is(1));

		string returnedId = from(content).getString("[0].id");
		string returnedName = from(content).getString("[0].name");

		assertThat(returnedId, @is(MockProvider.EXAMPLE_TENANT_ID));
		assertThat(returnedName, @is(MockProvider.EXAMPLE_TENANT_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeGetParameters()
	  public virtual void completeGetParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteStringQueryParameters;

		RequestSpecification requestSpecification = given().contentType(POST_JSON_CONTENT_TYPE);
		foreach (KeyValuePair<string, string> paramEntry in queryParameters.SetOfKeyValuePairs())
		{
		  requestSpecification.parameter(paramEntry.Key, paramEntry.Value);
		}

		requestSpecification.expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		verify(mockQuery).tenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockQuery).tenantName(MockProvider.EXAMPLE_TENANT_NAME);
		verify(mockQuery).tenantNameLike("%" + MockProvider.EXAMPLE_TENANT_NAME + "%");
		verify(mockQuery).userMember(MockProvider.EXAMPLE_USER_ID);
		verify(mockQuery).groupMember(MockProvider.EXAMPLE_GROUP_ID);

		verify(mockQuery).list();
	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["id"] = MockProvider.EXAMPLE_TENANT_ID;
			parameters["name"] = MockProvider.EXAMPLE_TENANT_NAME;
			parameters["nameLike"] = "%" + MockProvider.EXAMPLE_TENANT_NAME + "%";
			parameters["userMember"] = MockProvider.EXAMPLE_USER_ID;
			parameters["groupMember"] = MockProvider.EXAMPLE_GROUP_ID;
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryByUserIncludingGroups()
	  public virtual void queryByUserIncludingGroups()
	  {

		given().queryParam("userMember", MockProvider.EXAMPLE_USER_ID).queryParam("includingGroupsOfUser", true).then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		verify(mockQuery).userMember(MockProvider.EXAMPLE_USER_ID);
		verify(mockQuery).includingGroupsOfUser(true);

		verify(mockQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryCount()
	  public virtual void queryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryPagination()
	  public virtual void queryPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sortByParameterOnly()
	  public virtual void sortByParameterOnly()
	  {
		given().queryParam("sortBy", "name").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sortOrderParameterOnly()
	  public virtual void sortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sortById()
	  public virtual void sortById()
	  {
		given().queryParam("sortBy", "id").queryParam("sortOrder", "asc").then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		InOrder inOrder = Mockito.inOrder(mockQuery);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sortByName()
	  public virtual void sortByName()
	  {
		given().queryParam("sortBy", "name").queryParam("sortOrder", "desc").then().expect().statusCode(Status.OK.StatusCode).when().get(QUERY_URL);

		InOrder inOrder = Mockito.inOrder(mockQuery);
		inOrder.verify(mockQuery).orderByTenantName();
		inOrder.verify(mockQuery).desc();
	  }

	}

}