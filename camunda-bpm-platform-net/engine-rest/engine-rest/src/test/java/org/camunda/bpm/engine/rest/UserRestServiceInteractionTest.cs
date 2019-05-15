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
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;
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
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using UserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.UserCredentialsDto;
	using UserDto = org.camunda.bpm.engine.rest.dto.identity.UserDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;
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
	public class UserRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/user";
	  protected internal static readonly string USER_URL = SERVICE_URL + "/{id}";
	  protected internal static readonly string USER_CREATE_URL = SERVICE_URL + "/create";
	  protected internal static readonly string USER_PROFILE_URL = USER_URL + "/profile";
	  protected internal static readonly string USER_CREDENTIALS_URL = USER_URL + "/credentials";
	  protected internal static readonly string USER_UNLOCK = USER_URL + "/unlock";

	  protected internal IdentityService identityServiceMock;
	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal ProcessEngineConfiguration processEngineConfigurationMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupUserData()
	  public virtual void setupUserData()
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
//ORIGINAL LINE: @Test public void testGetSingleUserProfile()
	  public virtual void testGetSingleUserProfile()
	  {
		User sampleUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(sampleUser);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_USER_ID)).body("firstName", equalTo(MockProvider.EXAMPLE_USER_FIRST_NAME)).body("lastName", equalTo(MockProvider.EXAMPLE_USER_LAST_NAME)).body("email", equalTo(MockProvider.EXAMPLE_USER_EMAIL)).when().get(USER_PROFILE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserRestServiceOptions()
	  public virtual void testUserRestServiceOptions()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + UserRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserRestServiceOptionsWithAuthorizationDisabled()
	  public virtual void testUserRestServiceOptionsWithAuthorizationDisabled()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + UserRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserResourceOptionsUnauthenticated()
	  public virtual void testUserResourceOptionsUnauthenticated()
	  {
		string fullUserUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/user/" + MockProvider.EXAMPLE_USER_ID;

		User sampleUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(sampleUser);
		when(identityServiceMock.CurrentAuthentication).thenReturn(null);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullUserUrl + "/profile")).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullUserUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullUserUrl + "/profile")).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(USER_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserResourceOptionsUnauthorized()
	  public virtual void testUserResourceOptionsUnauthorized()
	  {
		string fullUserUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/user/" + MockProvider.EXAMPLE_USER_ID;

		User sampleUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(sampleUser);

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, USER, MockProvider.EXAMPLE_USER_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, USER, MockProvider.EXAMPLE_USER_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullUserUrl + "/profile")).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(USER_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, USER, MockProvider.EXAMPLE_USER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, USER, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserResourceOptionsDeleteAuthorized()
	  public virtual void testUserResourceOptionsDeleteAuthorized()
	  {
		string fullUserUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/user/" + MockProvider.EXAMPLE_USER_ID;

		User sampleUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(sampleUser);

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, USER, MockProvider.EXAMPLE_USER_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, USER, MockProvider.EXAMPLE_USER_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullUserUrl + "/profile")).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullUserUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2]", nullValue()).when().options(USER_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, USER, MockProvider.EXAMPLE_USER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, USER, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserResourceOptionsWithAuthorizationDisabled()
	  public virtual void testUserResourceOptionsWithAuthorizationDisabled()
	  {
		string fullUserUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + "/user/" + MockProvider.EXAMPLE_USER_ID;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullUserUrl + "/profile")).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullUserUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullUserUrl + "/profile")).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(USER_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingUserProfile()
	  public virtual void testGetNonExistingUserProfile()
	  {
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(anyString())).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingUser").then().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("User with id aNonExistingUser does not exist")).when().get(USER_PROFILE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUser()
	  public virtual void testDeleteUser()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.NO_CONTENT.StatusCode).when().delete(USER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingUser()
	  public virtual void testDeleteNonExistingUser()
	  {
		given().pathParam("id", "non-existing").then().statusCode(Status.NO_CONTENT.StatusCode).when().delete(USER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteUserThrowsAuthorizationException()
	  public virtual void testDeleteUserThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).deleteUser(MockProvider.EXAMPLE_USER_ID);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(USER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateNewUserWithCredentials()
	  public virtual void testCreateNewUserWithCredentials()
	  {
		User newUser = MockProvider.createMockUser();
		when(identityServiceMock.newUser(MockProvider.EXAMPLE_USER_ID)).thenReturn(newUser);

		UserDto userDto = UserDto.fromUser(newUser, true);

		given().body(userDto).contentType(ContentType.JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(USER_CREATE_URL);

		verify(identityServiceMock).newUser(MockProvider.EXAMPLE_USER_ID);
		verify(newUser).FirstName = MockProvider.EXAMPLE_USER_FIRST_NAME;
		verify(newUser).LastName = MockProvider.EXAMPLE_USER_LAST_NAME;
		verify(newUser).Email = MockProvider.EXAMPLE_USER_EMAIL;
		verify(newUser).Password = MockProvider.EXAMPLE_USER_PASSWORD;
		verify(identityServiceMock).saveUser(newUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateNewUserWithoutCredentials()
	  public virtual void testCreateNewUserWithoutCredentials()
	  {
		User newUser = MockProvider.createMockUser();
		when(identityServiceMock.newUser(MockProvider.EXAMPLE_USER_ID)).thenReturn(newUser);

		UserDto userDto = UserDto.fromUser(newUser, false);

		given().body(userDto).contentType(ContentType.JSON).expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(USER_CREATE_URL);

		verify(identityServiceMock).newUser(MockProvider.EXAMPLE_USER_ID);
		verify(newUser).FirstName = MockProvider.EXAMPLE_USER_FIRST_NAME;
		verify(newUser).LastName = MockProvider.EXAMPLE_USER_LAST_NAME;
		verify(newUser).Email = MockProvider.EXAMPLE_USER_EMAIL;
		// no password was set
		verify(newUser, never()).Password = any(typeof(string));

		verify(identityServiceMock).saveUser(newUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserCreateExistingFails()
	  public virtual void testUserCreateExistingFails()
	  {
		User newUser = MockProvider.createMockUser();
		when(identityServiceMock.newUser(MockProvider.EXAMPLE_USER_ID)).thenReturn(newUser);
		doThrow(new ProcessEngineException("")).when(identityServiceMock).saveUser(newUser);

		UserDto userDto = UserDto.fromUser(newUser, true);

		given().body(userDto).contentType(ContentType.JSON).then().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).when().post(USER_CREATE_URL);

		verify(identityServiceMock).newUser(MockProvider.EXAMPLE_USER_ID);
		verify(identityServiceMock).saveUser(newUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserCreateThrowsAuthorizationException()
	  public virtual void testUserCreateThrowsAuthorizationException()
	  {
		User newUser = MockProvider.createMockUser();
		string message = "exception expected";
		when(identityServiceMock.newUser(MockProvider.EXAMPLE_USER_ID)).thenThrow(new AuthorizationException(message));

		UserDto userDto = UserDto.fromUser(newUser, true);

		given().body(userDto).contentType(ContentType.JSON).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(USER_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveNewUserThrowsAuthorizationException()
	  public virtual void testSaveNewUserThrowsAuthorizationException()
	  {
		User newUser = MockProvider.createMockUser();
		when(identityServiceMock.newUser(MockProvider.EXAMPLE_USER_ID)).thenReturn(newUser);
		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveUser(newUser);

		UserDto userDto = UserDto.fromUser(newUser, true);

		given().body(userDto).contentType(ContentType.JSON).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(USER_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutCredentials()
	  public virtual void testPutCredentials()
	  {
		User initialUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		UserCredentialsDto dto = new UserCredentialsDto();
		dto.Password = "new-password";

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(dto).contentType(ContentType.JSON).then().statusCode(Status.NO_CONTENT.StatusCode).when().put(USER_CREDENTIALS_URL);

		// password was updated
		verify(initialUser).Password = dto.Password;

		// and then saved
		verify(identityServiceMock).saveUser(initialUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutCredentialsThrowsAuthorizationException()
	  public virtual void testPutCredentialsThrowsAuthorizationException()
	  {
		User initialUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveUser(any(typeof(User)));

		UserCredentialsDto dto = new UserCredentialsDto();
		dto.Password = "new-password";

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(dto).contentType(ContentType.JSON).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(USER_CREDENTIALS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeCredentials()
	  public virtual void testChangeCredentials()
	  {
		User initialUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		Authentication authentication = MockProvider.createMockAuthentication();
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		when(identityServiceMock.checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD)).thenReturn(true);

		UserCredentialsDto dto = new UserCredentialsDto();
		dto.Password = "new-password";
		dto.AuthenticatedUserPassword = MockProvider.EXAMPLE_USER_PASSWORD;

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).contentType(ContentType.JSON).body(dto).then().statusCode(Status.NO_CONTENT.StatusCode).when().put(USER_CREDENTIALS_URL);

		verify(identityServiceMock).CurrentAuthentication;
		verify(identityServiceMock).checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD);

		// password was updated
		verify(initialUser).Password = dto.Password;

		// and then saved
		verify(identityServiceMock).saveUser(initialUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeCredentialsWithWrongAuthenticatedUserPassword()
	  public virtual void testChangeCredentialsWithWrongAuthenticatedUserPassword()
	  {
		User initialUser = MockProvider.createMockUser();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		Authentication authentication = MockProvider.createMockAuthentication();
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		when(identityServiceMock.checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD)).thenReturn(false);

		UserCredentialsDto dto = new UserCredentialsDto();
		dto.Password = "new-password";
		dto.AuthenticatedUserPassword = MockProvider.EXAMPLE_USER_PASSWORD;

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).contentType(ContentType.JSON).body(dto).then().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo("InvalidRequestException")).body("message", equalTo("The given authenticated user password is not valid.")).when().put(USER_CREDENTIALS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutCredentialsNonExistingUserFails()
	  public virtual void testPutCredentialsNonExistingUserFails()
	  {
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId("aNonExistingUser")).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(null);

		UserCredentialsDto dto = new UserCredentialsDto();
		dto.Password = "new-password";

		given().pathParam("id", "aNonExistingUser").body(dto).contentType(ContentType.JSON).then().then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("User with id aNonExistingUser does not exist")).when().put(USER_CREDENTIALS_URL);

		// user was not updated
		verify(identityServiceMock, never()).saveUser(any(typeof(User)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutProfile()
	  public virtual void testPutProfile()
	  {
		User initialUser = MockProvider.createMockUser();
		User userUpdate = MockProvider.createMockUserUpdate();

		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		UserProfileDto updateDto = UserProfileDto.fromUser(userUpdate);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(updateDto).contentType(ContentType.JSON).then().statusCode(Status.NO_CONTENT.StatusCode).when().put(USER_PROFILE_URL);

		// password was updated
		verify(initialUser).Email = updateDto.Email;
		verify(initialUser).FirstName = updateDto.FirstName;
		verify(initialUser).LastName = updateDto.LastName;

		// and then saved
		verify(identityServiceMock).saveUser(initialUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutProfileNonexistingFails()
	  public virtual void testPutProfileNonexistingFails()
	  {
		User userUpdate = MockProvider.createMockUserUpdate();

		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId("aNonExistingUser")).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(null);

		UserProfileDto updateDto = UserProfileDto.fromUser(userUpdate);

		given().pathParam("id", "aNonExistingUser").body(updateDto).contentType(ContentType.JSON).then().then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("User with id aNonExistingUser does not exist")).when().put(USER_PROFILE_URL);

		// nothing was saved
		verify(identityServiceMock, never()).saveUser(any(typeof(User)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPutProfileThrowsAuthorizationException()
	  public virtual void testPutProfileThrowsAuthorizationException()
	  {
		User initialUser = MockProvider.createMockUser();
		User userUpdate = MockProvider.createMockUserUpdate();

		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(identityServiceMock.createUserQuery()).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userId(MockProvider.EXAMPLE_USER_ID)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.singleResult()).thenReturn(initialUser);

		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveUser(any(typeof(User)));

		UserProfileDto updateDto = UserProfileDto.fromUser(userUpdate);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(updateDto).contentType(ContentType.JSON).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(USER_PROFILE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyUserCreateFails()
	  public virtual void testReadOnlyUserCreateFails()
	  {
		User newUser = MockProvider.createMockUser();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().body(UserDto.fromUser(newUser, true)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().post(USER_CREATE_URL);

		verify(identityServiceMock, never()).newUser(MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyPutUserProfileFails()
	  public virtual void testReadOnlyPutUserProfileFails()
	  {
		User userUdpdate = MockProvider.createMockUser();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(UserProfileDto.fromUser(userUdpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(USER_PROFILE_URL);

		verify(identityServiceMock, never()).saveUser(userUdpdate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyPutUserCredentialsFails()
	  public virtual void testReadOnlyPutUserCredentialsFails()
	  {
		User userUdpdate = MockProvider.createMockUser();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).body(UserCredentialsDto.fromUser(userUdpdate)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(USER_CREDENTIALS_URL);

		verify(identityServiceMock, never()).saveUser(userUdpdate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadOnlyUserDeleteFails()
	  public virtual void testReadOnlyUserDeleteFails()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(USER_URL);

		verify(identityServiceMock, never()).deleteUser(MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockUser()
	  public virtual void testUnlockUser()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(USER_UNLOCK);

		verify(identityServiceMock).unlockUser(MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockUserNonExistingUser()
	  public virtual void testUnlockUserNonExistingUser()
	  {
		given().pathParam("id", "non-existing").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(USER_UNLOCK);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockUserThrowsAuthorizationException()
	  public virtual void testUnlockUserThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).unlockUser(MockProvider.EXAMPLE_USER_ID);

		given().pathParam("id", MockProvider.EXAMPLE_USER_ID).then().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(USER_UNLOCK);
	  }

	  protected internal virtual void verifyNoAuthorizationCheckPerformed()
	  {
		verify(identityServiceMock, times(0)).CurrentAuthentication;
		verify(authorizationServiceMock, times(0)).isUserAuthorized(anyString(), anyListOf(typeof(string)), any(typeof(Permission)), any(typeof(Resource)));
	  }

	}

}