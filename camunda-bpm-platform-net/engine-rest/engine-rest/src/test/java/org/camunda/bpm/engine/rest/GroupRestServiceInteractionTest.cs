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
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP_MEMBERSHIP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using GroupDto = org.camunda.bpm.engine.rest.dto.identity.GroupDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class GroupRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/group";
	  protected internal static readonly string GROUP_URL = SERVICE_URL + "/{id}";
	  protected internal static readonly string GROUP_MEMBERS_URL = GROUP_URL + "/members";
	  protected internal static readonly string GROUP_MEMBER_URL = GROUP_MEMBERS_URL + "/{userId}";
	  protected internal static readonly string GROUP_CREATE_URL = TEST_RESOURCE_ROOT_PATH + "/group/create";

	  protected internal IdentityService identityServiceMock;
	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal ProcessEngineConfiguration processEngineConfigurationMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupGroupData()
	  public virtual void setupGroupData()
	  {

		identityServiceMock = mock(typeof(IdentityService));
		authorizationServiceMock = mock(typeof(AuthorizationService));
		processEngineConfigurationMock = mock(typeof(ProcessEngineConfiguration));

		// mock identity service
		when(processEngine.IdentityService).thenReturn(identityServiceMock);
		// authorization service
		when(processEngine.AuthorizationService).thenReturn(authorizationServiceMock);
		// process engine configuration
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleGroup()
	  public virtual void testGetSingleGroup()
	  {
		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_GROUP_ID)).body("name", equalTo(MockProvider.EXAMPLE_GROUP_NAME)).when().get(GROUP_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserRestServiceOptions()
	  public virtual void testUserRestServiceOptions()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + GroupRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserRestServiceOptionsWithAuthorizationDisabled()
	  public virtual void testUserRestServiceOptionsWithAuthorizationDisabled()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + GroupRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupResourceOptionsUnauthenticated()
	  public virtual void testGroupResourceOptionsUnauthenticated()
	  {
		string fullGroupUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID;

		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullGroupUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullGroupUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullGroupUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(GROUP_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupResourceOptionsUnauthorized()
	  public virtual void testGroupResourceOptionsUnauthorized()
	  {
		string fullGroupUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, GROUP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(false);

		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullGroupUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(GROUP_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP, MockProvider.EXAMPLE_GROUP_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, GROUP, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupResourceOptionsAuthorized()
	  public virtual void testGroupResourceOptionsAuthorized()
	  {
		string fullGroupUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, GROUP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(false);

		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullGroupUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullGroupUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2]", nullValue()).when().options(GROUP_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP, MockProvider.EXAMPLE_GROUP_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, GROUP, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupResourceOptionsWithAuthorizationDisabled()
	  public virtual void testGroupResourceOptionsWithAuthorizationDisabled()
	  {
		string fullGroupUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullGroupUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullGroupUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullGroupUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(GROUP_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupMembersResourceOptions()
	  public virtual void testGroupMembersResourceOptions()
	  {
		string fullMembersUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID + "/members";

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(GROUP_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupMembersResourceOptionsAuthorized()
	  public virtual void testGroupMembersResourceOptionsAuthorized()
	  {
		string fullMembersUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID + "/members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(true);

		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(GROUP_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupMembersResourceOptionsUnauthorized()
	  public virtual void testGroupMembersResourceOptionsUnauthorized()
	  {
		string fullMembersUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID + "/members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID)).thenReturn(false);

		Group sampleGroup = MockProvider.createMockGroup();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(sampleGroup);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(GROUP_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, GROUP_MEMBERSHIP, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupMembersResourceOptionsWithAuthorizationDisabled()
	  public virtual void testGroupMembersResourceOptionsWithAuthorizationDisabled()
	  {
		string fullMembersUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/group/" + MockProvider.EXAMPLE_GROUP_ID + "/members";

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(GROUP_MEMBERS_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingGroup()
	  public virtual void testGetNonExistingGroup()
	  {
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(anyString())).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingGroup").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Group with id aNonExistingGroup does not exist")).when().get(GROUP_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroup()
	  public virtual void testDeleteGroup()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(GROUP_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingGroup()
	  public virtual void testDeleteNonExistingGroup()
	  {
		given().pathParam("id", "non-existing").expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(GROUP_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupThrowsAuthorizationException()
	  public virtual void testDeleteGroupThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).deleteGroup(MockProvider.EXAMPLE_GROUP_ID);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(GROUP_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateExistingGroup()
	  public virtual void testUpdateExistingGroup()
	  {
		Group initialGroup = MockProvider.createMockGroup();
		Group groupUpdate = MockProvider.createMockGroupUpdate();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(initialGroup);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).body(GroupDto.fromGroup(groupUpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(GROUP_URL);

		// initial group was updated
		verify(initialGroup).Name = groupUpdate.Name;

		// and then saved
		verify(identityServiceMock).saveGroup(initialGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateNonExistingGroup()
	  public virtual void testUpdateNonExistingGroup()
	  {
		Group groupUpdate = MockProvider.createMockGroupUpdate();
		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId("aNonExistingGroup")).thenReturn(sampleGroupQuery);
		// this time the query returns null
		when(sampleGroupQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingGroup").body(GroupDto.fromGroup(groupUpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Group with id aNonExistingGroup does not exist")).when().put(GROUP_URL);

		verify(identityServiceMock, never()).saveGroup(any(typeof(Group)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateGroupThrowsAuthorizationException()
	  public virtual void testUpdateGroupThrowsAuthorizationException()
	  {
		Group initialGroup = MockProvider.createMockGroup();
		Group groupUpdate = MockProvider.createMockGroupUpdate();

		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));
		when(identityServiceMock.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.groupId(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.singleResult()).thenReturn(initialGroup);

		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveGroup(any(typeof(Group)));

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).body(GroupDto.fromGroup(groupUpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(GROUP_URL);

		// initial group was updated
		verify(initialGroup).Name = groupUpdate.Name;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupCreate()
	  public virtual void testGroupCreate()
	  {
		Group newGroup = MockProvider.createMockGroup();
		when(identityServiceMock.newGroup(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(newGroup);

		given().body(GroupDto.fromGroup(newGroup)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(GROUP_CREATE_URL);

		verify(identityServiceMock).newGroup(MockProvider.EXAMPLE_GROUP_ID);
		verify(newGroup).Name = MockProvider.EXAMPLE_GROUP_NAME;
		verify(identityServiceMock).saveGroup(newGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupCreateExistingFails()
	  public virtual void testGroupCreateExistingFails()
	  {
		Group newGroup = MockProvider.createMockGroup();
		when(identityServiceMock.newGroup(MockProvider.EXAMPLE_GROUP_ID)).thenReturn(newGroup);
		doThrow(new ProcessEngineException("")).when(identityServiceMock).saveGroup(newGroup);

		given().body(GroupDto.fromGroup(newGroup)).contentType(ContentType.JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).when().post(GROUP_CREATE_URL);

		verify(identityServiceMock).newGroup(MockProvider.EXAMPLE_GROUP_ID);
		verify(identityServiceMock).saveGroup(newGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupCreateThrowsAuthorizationException()
	  public virtual void testGroupCreateThrowsAuthorizationException()
	  {
		Group newGroup = MockProvider.createMockGroup();
		string message = "exception expected";
		when(identityServiceMock.newGroup(newGroup.Id)).thenThrow(new AuthorizationException(message));

		given().body(GroupDto.fromGroup(newGroup)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(GROUP_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveGroupThrowsAuthorizationException()
	  public virtual void testSaveGroupThrowsAuthorizationException()
	  {
		Group newGroup = MockProvider.createMockGroup();

		string message = "exception expected";
		when(identityServiceMock.newGroup(newGroup.Id)).thenReturn(newGroup);
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveGroup(newGroup);

		given().body(GroupDto.fromGroup(newGroup)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(GROUP_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateGroupMember()
	  public virtual void testCreateGroupMember()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(GROUP_MEMBER_URL);

		verify(identityServiceMock).createMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateGroupMemberThrowsAuthorizationException()
	  public virtual void testCreateGroupMemberThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).createMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(GROUP_MEMBER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupMember()
	  public virtual void testDeleteGroupMember()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(GROUP_MEMBER_URL);

		verify(identityServiceMock).deleteMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteGroupMemberThrowsAuthorizationException()
	  public virtual void testDeleteGroupMemberThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).deleteMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(GROUP_MEMBER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyGroupCreateFails()
	  public virtual void testReadOnlyGroupCreateFails()
	  {
		Group newGroup = MockProvider.createMockGroup();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().body(GroupDto.fromGroup(newGroup)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().post(GROUP_CREATE_URL);

		verify(identityServiceMock, never()).newGroup(MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyGroupUpdateFails()
	  public virtual void testReadOnlyGroupUpdateFails()
	  {
		Group groupUdpdate = MockProvider.createMockGroup();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).body(GroupDto.fromGroup(groupUdpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(GROUP_URL);

		verify(identityServiceMock, never()).saveGroup(groupUdpdate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyGroupDeleteFails()
	  public virtual void testReadOnlyGroupDeleteFails()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(GROUP_URL);

		verify(identityServiceMock, never()).deleteGroup(MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyCreateGroupMemberFails()
	  public virtual void testReadOnlyCreateGroupMemberFails()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(GROUP_MEMBER_URL);

		verify(identityServiceMock, never()).createMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyGroupMemberDeleteFails()
	  public virtual void testReadOnlyGroupMemberDeleteFails()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_GROUP_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(GROUP_MEMBER_URL);

		verify(identityServiceMock, never()).deleteMembership(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

	  protected internal virtual void verifyNoAuthorizationCheckPerformed()
	  {
		verify(identityServiceMock, times(0)).CurrentAuthentication;
		verify(authorizationServiceMock, times(0)).isUserAuthorized(anyString(), anyListOf(typeof(string)), any(typeof(Permission)), any(typeof(Resource)));
	  }

	}

}