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
//	import static org.hamcrest.Matchers.equalTo;
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


	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;
	using RequestSpecification = io.restassured.specification.RequestSpecification;

	public class GroupRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string GROUP_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/group";
	  protected internal static readonly string GROUP_COUNT_QUERY_URL = GROUP_QUERY_URL + "/count";

	  private GroupQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = setUpMockGroupQuery(MockProvider.createMockGroups());
	  }

	  private GroupQuery setUpMockGroupQuery(IList<Group> list)
	  {
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(sampleGroupQuery.list()).thenReturn(list);
		when(sampleGroupQuery.count()).thenReturn((long) list.Count);

		when(processEngine.IdentityService.createGroupQuery()).thenReturn(sampleGroupQuery);

		return sampleGroupQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {

		string queryKey = "";
		given().queryParam("name", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(GROUP_QUERY_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "name").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(GROUP_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(GROUP_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(GROUP_QUERY_URL);

		verify(mockQuery).list();
		verifyNoMoreInteractions(mockQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleGroupQuery()
	  public virtual void testSimpleGroupQuery()
	  {
		string queryName = MockProvider.EXAMPLE_GROUP_NAME;

		Response response = given().queryParam("name", queryName).then().expect().statusCode(Status.OK.StatusCode).when().get(GROUP_QUERY_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).groupName(queryName);
		inOrder.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one group returned.", 1, instances.Count);
		Assert.assertNotNull("The returned group should not be null.", instances[0]);

		string returendName = from(content).getString("[0].name");
		string returendType = from(content).getString("[0].type");

		Assert.assertEquals(MockProvider.EXAMPLE_GROUP_NAME, returendName);
		Assert.assertEquals(MockProvider.EXAMPLE_GROUP_TYPE, returendType);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteGetParameters()
	  public virtual void testCompleteGetParameters()
	  {

		IDictionary<string, string> queryParameters = CompleteStringQueryParameters;

		RequestSpecification requestSpecification = given().contentType(POST_JSON_CONTENT_TYPE);
		foreach (KeyValuePair<string, string> paramEntry in queryParameters.SetOfKeyValuePairs())
		{
		  requestSpecification.parameter(paramEntry.Key, paramEntry.Value);
		}

		requestSpecification.expect().statusCode(Status.OK.StatusCode).when().get(GROUP_QUERY_URL);

		verify(mockQuery).groupName(MockProvider.EXAMPLE_GROUP_NAME);
		verify(mockQuery).groupNameLike("%" + MockProvider.EXAMPLE_GROUP_NAME + "%");
		verify(mockQuery).groupType(MockProvider.EXAMPLE_GROUP_TYPE);
		verify(mockQuery).groupMember(MockProvider.EXAMPLE_USER_ID);
		verify(mockQuery).memberOfTenant(MockProvider.EXAMPLE_TENANT_ID);

		verify(mockQuery).list();

	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["name"] = MockProvider.EXAMPLE_GROUP_NAME;
			parameters["nameLike"] = "%" + MockProvider.EXAMPLE_GROUP_NAME + "%";
			parameters["type"] = MockProvider.EXAMPLE_GROUP_TYPE;
			parameters["member"] = MockProvider.EXAMPLE_USER_ID;
			parameters["memberOfTenant"] = MockProvider.EXAMPLE_TENANT_ID;
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(GROUP_COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(GROUP_QUERY_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }


	}

}