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


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using AuthorizationServiceImpl = org.camunda.bpm.engine.impl.AuthorizationServiceImpl;
	using IdentityServiceImpl = org.camunda.bpm.engine.impl.IdentityServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultPermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultPermissionProvider;
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

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationRestServiceQueryTest : AbstractRestServiceTest
	{

	  protected internal static readonly string SERVICE_PATH = TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH;
	  protected internal static readonly string SERVICE_COUNT_PATH = TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH + "/count";

	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal IdentityService identityServiceMock;
	  protected internal ProcessEngineConfigurationImpl processEngineConfigurationMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		authorizationServiceMock = mock(typeof(AuthorizationServiceImpl));
		identityServiceMock = mock(typeof(IdentityServiceImpl));
		processEngineConfigurationMock = mock(typeof(ProcessEngineConfigurationImpl));

		when(processEngine.AuthorizationService).thenReturn(authorizationServiceMock);
		when(processEngine.IdentityService).thenReturn(identityServiceMock);
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationMock);
		when(processEngineConfigurationMock.PermissionProvider).thenReturn(new DefaultPermissionProvider());
	  }

	  private AuthorizationQuery setUpMockQuery(IList<Authorization> list)
	  {
		AuthorizationQuery query = mock(typeof(AuthorizationQuery));
		when(query.list()).thenReturn(list);
		when(query.count()).thenReturn((long) list.Count);

		when(processEngine.AuthorizationService.createAuthorizationQuery()).thenReturn(query);

		return query;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {

		UpMockQuery = MockProvider.createMockAuthorizations();

		string queryKey = "";
		given().queryParam("name", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(SERVICE_PATH);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "resourceType").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {

		AuthorizationQuery mockQuery = setUpMockQuery(MockProvider.createMockAuthorizations());

		expect().statusCode(Status.OK.StatusCode).when().get(SERVICE_PATH);

		verify(mockQuery).list();
		verifyNoMoreInteractions(mockQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleAuthorizationQuery()
	  public virtual void testSimpleAuthorizationQuery()
	  {

		IList<Authorization> mockAuthorizations = MockProvider.createMockGlobalAuthorizations();
		AuthorizationQuery mockQuery = setUpMockQuery(mockAuthorizations);

		Response response = given().queryParam("type", org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL).then().expect().statusCode(Status.OK.StatusCode).when().get(SERVICE_PATH);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).authorizationType(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		inOrder.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one authorization returned.", 1, instances.Count);
		Assert.assertNotNull("The returned authorization should not be null.", instances[0]);

		Authorization mockAuthorization = mockAuthorizations[0];

		Assert.assertEquals(mockAuthorization.Id, from(content).getString("[0].id"));
		Assert.assertEquals(mockAuthorization.AuthorizationType, from(content).getInt("[0].type"));
		Assert.assertEquals(Permissions.READ.Name, from(content).getString("[0].permissions[0]"));
		Assert.assertEquals(Permissions.UPDATE.Name, from(content).getString("[0].permissions[1]"));
		Assert.assertEquals(mockAuthorization.UserId, from(content).getString("[0].userId"));
		Assert.assertEquals(mockAuthorization.GroupId, from(content).getString("[0].groupId"));
		Assert.assertEquals(mockAuthorization.ResourceType, from(content).getInt("[0].resourceType"));
		Assert.assertEquals(mockAuthorization.ResourceId, from(content).getString("[0].resourceId"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteGetParameters()
	  public virtual void testCompleteGetParameters()
	  {

		IList<Authorization> mockAuthorizations = MockProvider.createMockGlobalAuthorizations();
		AuthorizationQuery mockQuery = setUpMockQuery(mockAuthorizations);

		IDictionary<string, string> queryParameters = CompleteStringQueryParameters;

		RequestSpecification requestSpecification = given().contentType(POST_JSON_CONTENT_TYPE);
		foreach (KeyValuePair<string, string> paramEntry in queryParameters.SetOfKeyValuePairs())
		{
		  requestSpecification.parameter(paramEntry.Key, paramEntry.Value);
		}

		requestSpecification.expect().statusCode(Status.OK.StatusCode).when().get(SERVICE_PATH);

		verify(mockQuery).authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID);
		verify(mockQuery).authorizationType(MockProvider.EXAMPLE_AUTHORIZATION_TYPE);
		verify(mockQuery).userIdIn(new string[]{MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_ID2});
		verify(mockQuery).groupIdIn(new string[]{MockProvider.EXAMPLE_GROUP_ID, MockProvider.EXAMPLE_GROUP_ID2});
		verify(mockQuery).resourceType(MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		verify(mockQuery).resourceId(MockProvider.EXAMPLE_RESOURCE_ID);

		verify(mockQuery).list();

	  }


	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["id"] = MockProvider.EXAMPLE_AUTHORIZATION_ID;
			parameters["type"] = MockProvider.EXAMPLE_AUTHORIZATION_TYPE_STRING;
			parameters["userIdIn"] = MockProvider.EXAMPLE_USER_ID + "," + MockProvider.EXAMPLE_USER_ID2;
			parameters["groupIdIn"] = MockProvider.EXAMPLE_GROUP_ID + "," + MockProvider.EXAMPLE_GROUP_ID2;
			parameters["resourceType"] = MockProvider.EXAMPLE_RESOURCE_TYPE_ID_STRING;
			parameters["resourceId"] = MockProvider.EXAMPLE_RESOURCE_ID;
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {

		AuthorizationQuery mockQuery = setUpMockQuery(MockProvider.createMockAuthorizations());

		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(3)).when().get(SERVICE_COUNT_PATH);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		AuthorizationQuery mockQuery = setUpMockQuery(MockProvider.createMockAuthorizations());

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(SERVICE_PATH);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }


	}

}