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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TENANT_MEMBERSHIP;
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
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using TenantDto = org.camunda.bpm.engine.rest.dto.identity.TenantDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public class TenantRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string SERVICE_URL = TEST_RESOURCE_ROOT_PATH + "/tenant";
	  protected internal static readonly string TENANT_URL = SERVICE_URL + "/{id}";
	  protected internal static readonly string TENANT_CREATE_URL = SERVICE_URL + "/create";
	  protected internal static readonly string TENANT_USER_MEMBERS_URL = TENANT_URL + "/user-members";
	  protected internal static readonly string TENANT_USER_MEMBER_URL = TENANT_USER_MEMBERS_URL + "/{userId}";
	  protected internal static readonly string TENANT_GROUP_MEMBERS_URL = TENANT_URL + "/group-members";
	  protected internal static readonly string TENANT_GROUP_MEMBER_URL = TENANT_GROUP_MEMBERS_URL + "/{groupId}";

	  protected internal IdentityService identityServiceMock;
	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal ProcessEngineConfiguration processEngineConfigurationMock;

	  protected internal Tenant mockTenant;
	  protected internal TenantQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupData()
	  public virtual void setupData()
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

		mockTenant = MockProvider.createMockTenant();
		mockQuery = setUpMockQuery(mockTenant);
	  }

	  protected internal virtual TenantQuery setUpMockQuery(Tenant tenant)
	  {
		TenantQuery query = mock(typeof(TenantQuery));
		when(query.tenantId(anyString())).thenReturn(query);
		when(query.singleResult()).thenReturn(tenant);

		when(identityServiceMock.createTenantQuery()).thenReturn(query);

		return query;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getTenant()
	  public virtual void getTenant()
	  {
	   given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("name", equalTo(MockProvider.EXAMPLE_TENANT_NAME)).when().get(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getNonExistingTenant()
	  public virtual void getNonExistingTenant()
	  {
		when(mockQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingTenant").then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Tenant with id aNonExistingTenant does not exist")).when().get(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenant()
	  public virtual void deleteTenant()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNonExistingTenant()
	  public virtual void deleteNonExistingTenant()
	  {
		given().pathParam("id", "aNonExistingTenant").then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantThrowsAuthorizationException()
	  public virtual void deleteTenantThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).deleteTenant(MockProvider.EXAMPLE_TENANT_ID);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTenant()
	  public virtual void updateTenant()
	  {
		Tenant updatedTenant = MockProvider.createMockTenant();
		when(updatedTenant.Name).thenReturn("updatedName");

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).body(TenantDto.fromTenant(updatedTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(TENANT_URL);

		// tenant was updated
		verify(mockTenant).Name = updatedTenant.Name;

		// and then saved
		verify(identityServiceMock).saveTenant(mockTenant);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateNonExistingTenant()
	  public virtual void updateNonExistingTenant()
	  {
		Tenant updatedTenant = MockProvider.createMockTenant();
		when(updatedTenant.Name).thenReturn("updatedName");

		when(mockQuery.singleResult()).thenReturn(null);

		given().pathParam("id", "aNonExistingTenant").body(TenantDto.fromTenant(updatedTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Tenant with id aNonExistingTenant does not exist")).when().put(TENANT_URL);

		verify(identityServiceMock, never()).saveTenant(any(typeof(Tenant)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateTenantThrowsAuthorizationException()
	  public virtual void updateTenantThrowsAuthorizationException()
	  {
		Tenant updatedTenant = MockProvider.createMockTenant();
		when(updatedTenant.Name).thenReturn("updatedName");

		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveTenant(any(typeof(Tenant)));

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).body(TenantDto.fromTenant(updatedTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(TENANT_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenant()
	  public virtual void createTenant()
	  {
		Tenant newTenant = MockProvider.createMockTenant();
		when(identityServiceMock.newTenant(MockProvider.EXAMPLE_TENANT_ID)).thenReturn(newTenant);

		given().body(TenantDto.fromTenant(mockTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(TENANT_CREATE_URL);

		verify(identityServiceMock).newTenant(MockProvider.EXAMPLE_TENANT_ID);
		verify(newTenant).Name = MockProvider.EXAMPLE_TENANT_NAME;
		verify(identityServiceMock).saveTenant(newTenant);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createExistingTenant()
	  public virtual void createExistingTenant()
	  {
		Tenant newTenant = MockProvider.createMockTenant();
		when(identityServiceMock.newTenant(MockProvider.EXAMPLE_TENANT_ID)).thenReturn(newTenant);

		string message = "exception expected";
		doThrow(new ProcessEngineException(message)).when(identityServiceMock).saveTenant(newTenant);

		given().body(TenantDto.fromTenant(newTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo(message)).when().post(TENANT_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantThrowsAuthorizationException()
	  public virtual void createTenantThrowsAuthorizationException()
	  {
		Tenant newTenant = MockProvider.createMockTenant();

		string message = "exception expected";
		when(identityServiceMock.newTenant(MockProvider.EXAMPLE_TENANT_ID)).thenThrow(new AuthorizationException(message));

		given().body(TenantDto.fromTenant(newTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TENANT_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveTenantThrowsAuthorizationException()
	  public virtual void saveTenantThrowsAuthorizationException()
	  {
		Tenant newTenant = MockProvider.createMockTenant();
		when(identityServiceMock.newTenant(MockProvider.EXAMPLE_TENANT_ID)).thenReturn(newTenant);

		string message = "exception expected";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).saveTenant(newTenant);

		given().body(TenantDto.fromTenant(newTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(TENANT_CREATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void userRestServiceOptionUnauthenticated()
	  public virtual void userRestServiceOptionUnauthenticated()
	  {
		string fullAuthorizationUrl = FullAuthorizationUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void userRestServiceOptionUnauthorized()
	  public virtual void userRestServiceOptionUnauthorized()
	  {
		string fullAuthorizationUrl = FullAuthorizationUrl;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT, ANY)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2]", nullValue()).when().options(SERVICE_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void userRestServiceOptionAuthorized()
	  public virtual void userRestServiceOptionAuthorized()
	  {
		string fullAuthorizationUrl = FullAuthorizationUrl;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT, ANY)).thenReturn(true);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void userRestServiceOptionsWithAuthorizationDisabled()
	  public virtual void userRestServiceOptionsWithAuthorizationDisabled()
	  {
		string fullAuthorizationUrl = FullAuthorizationUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantResourceOptionsUnauthenticated()
	  public virtual void tenantResourceOptionsUnauthenticated()
	  {
		string fullTenantUrl = FullAuthorizationTenantUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullTenantUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullTenantUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullTenantUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(TENANT_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantResourceOptionsUnauthorized()
	  public virtual void tenantResourceOptionsUnauthorized()
	  {
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, TENANT, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);

		string fullTenantUrl = FullAuthorizationTenantUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullTenantUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(TENANT_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, TENANT, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantResourceOptionsAuthorized()
	  public virtual void tenantResourceOptionsAuthorized()
	  {
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, TENANT, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);

		string fullTenantUrl = FullAuthorizationTenantUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullTenantUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullTenantUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2]", nullValue()).when().options(TENANT_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, TENANT, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantResourceOptionsWithAuthorizationDisabled()
	  public virtual void tenantResourceOptionsWithAuthorizationDisabled()
	  {
		string fullTenantUrl = FullAuthorizationTenantUrl;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullTenantUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullTenantUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullTenantUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(TENANT_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantUserMembership()
	  public virtual void createTenantUserMembership()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(TENANT_USER_MEMBER_URL);

		verify(identityServiceMock).createTenantUserMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantGroupMembership()
	  public virtual void createTenantGroupMembership()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("groupId", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(TENANT_GROUP_MEMBER_URL);

		verify(identityServiceMock).createTenantGroupMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantUserMembershipThrowsAuthorizationException()
	  public virtual void createTenantUserMembershipThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).createTenantUserMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_USER_ID);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(TENANT_USER_MEMBER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantUserMembership()
	  public virtual void deleteTenantUserMembership()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(TENANT_USER_MEMBER_URL);

		verify(identityServiceMock).deleteTenantUserMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantGroupMembership()
	  public virtual void deleteTenantGroupMembership()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("groupId", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(TENANT_GROUP_MEMBER_URL);

		verify(identityServiceMock).deleteTenantGroupMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantGroupMembershipThrowsAuthorizationException()
	  public virtual void deleteTenantGroupMembershipThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(identityServiceMock).deleteTenantGroupMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_GROUP_ID);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("groupId", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(TENANT_GROUP_MEMBER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantUserMembershipResourceOptionsUnauthenticated()
	  public virtual void tenantUserMembershipResourceOptionsUnauthenticated()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/user-members";

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(TENANT_USER_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantUserMembershipResourceOptionsAuthorized()
	  public virtual void tenantUserMembershipResourceOptionsAuthorized()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/user-members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(true);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(TENANT_USER_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantGroupMembershipResourceOptionsAuthorized()
	  public virtual void tenantGroupMembershipResourceOptionsAuthorized()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/group-members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(true);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(TENANT_GROUP_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantUserMembershipResourceOptionsUnauthorized()
	  public virtual void tenantUserMembershipResourceOptionsUnauthorized()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/user-members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(TENANT_USER_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantGroupMembershipResourceOptionsUnauthorized()
	  public virtual void tenantGroupMembershipResourceOptionsUnauthorized()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/group-members";

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(TENANT_GROUP_MEMBERS_URL);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, CREATE, TENANT_MEMBERSHIP, MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void tenantUserMembershipResourceOptionsWithAuthorizationDisabled()
	  public virtual void tenantUserMembershipResourceOptionsWithAuthorizationDisabled()
	  {
		string fullMembersUrl = FullAuthorizationTenantUrl + "/user-members";

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullMembersUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullMembersUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullMembersUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("create")).when().options(TENANT_USER_MEMBERS_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCreateTenantForReadOnlyService()
	  public virtual void failToCreateTenantForReadOnlyService()
	  {
		Tenant newTenant = MockProvider.createMockTenant();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().body(TenantDto.fromTenant(newTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().post(TENANT_CREATE_URL);

		verify(identityServiceMock, never()).newTenant(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToUpdateTenantForReadOnlyService()
	  public virtual void failToUpdateTenantForReadOnlyService()
	  {
		Tenant updatedTenant = MockProvider.createMockTenant();
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).body(TenantDto.fromTenant(updatedTenant)).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(TENANT_URL);

		verify(identityServiceMock, never()).saveTenant(mockTenant);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteTenantForReadOnlyService()
	  public virtual void failToDeleteTenantForReadOnlyService()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(TENANT_URL);

		verify(identityServiceMock, never()).deleteTenant(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCreateTenantUserMembershipForReadOnlyService()
	  public virtual void failToCreateTenantUserMembershipForReadOnlyService()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(TENANT_USER_MEMBER_URL);

		verify(identityServiceMock, never()).createTenantUserMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCreateTenantGroupMembershipForReadOnlyService()
	  public virtual void failToCreateTenantGroupMembershipForReadOnlyService()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("groupId", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().put(TENANT_GROUP_MEMBER_URL);

		verify(identityServiceMock, never()).createTenantGroupMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteTenantUserMembershipForReadOnlyService()
	  public virtual void failToDeleteTenantUserMembershipForReadOnlyService()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("userId", MockProvider.EXAMPLE_USER_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(TENANT_USER_MEMBER_URL);

		verify(identityServiceMock, never()).deleteTenantUserMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToDeleteTenantGroupMembershipForReadOnlyService()
	  public virtual void failToDeleteTenantGroupMembershipForReadOnlyService()
	  {
		when(identityServiceMock.ReadOnly).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_TENANT_ID).pathParam("groupId", MockProvider.EXAMPLE_GROUP_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Identity service implementation is read-only.")).when().delete(TENANT_GROUP_MEMBER_URL);

		verify(identityServiceMock, never()).deleteTenantGroupMembership(MockProvider.EXAMPLE_TENANT_ID, MockProvider.EXAMPLE_GROUP_ID);
	  }

	  protected internal virtual void verifyNoAuthorizationCheckPerformed()
	  {
		verify(identityServiceMock, times(0)).CurrentAuthentication;
		verify(authorizationServiceMock, times(0)).isUserAuthorized(anyString(), anyListOf(typeof(string)), any(typeof(Permission)), any(typeof(Resource)));
	  }

	  protected internal virtual string FullAuthorizationUrl
	  {
		  get
		  {
			return "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + TenantRestService_Fields.PATH;
		  }
	  }

	  protected internal virtual string FullAuthorizationTenantUrl
	  {
		  get
		  {
			return FullAuthorizationUrl + "/" + MockProvider.EXAMPLE_TENANT_ID;
		  }
	  }

	}

}