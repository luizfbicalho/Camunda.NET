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
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.BATCH;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;
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



	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using AuthorizationServiceImpl = org.camunda.bpm.engine.impl.AuthorizationServiceImpl;
	using IdentityServiceImpl = org.camunda.bpm.engine.impl.IdentityServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultPermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultPermissionProvider;
	using PermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.PermissionProvider;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using AuthorizationDto = org.camunda.bpm.engine.rest.dto.authorization.AuthorizationDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ResourceUtil = org.camunda.bpm.engine.rest.util.ResourceUtil;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationRestServiceInteractionTest : AbstractRestServiceTest
	{

	  protected internal static readonly string SERVICE_PATH = TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH;
	  protected internal static readonly string AUTH_CREATE_PATH = SERVICE_PATH + "/create";
	  protected internal static readonly string AUTH_CHECK_PATH = SERVICE_PATH + "/check";
	  protected internal static readonly string AUTH_RESOURCE_PATH = SERVICE_PATH + "/{id}";

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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedTrue()
	  public virtual void testIsUserAuthorizedTrue()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(null)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedFalse()
	  public virtual void testIsUserAuthorizedFalse()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(null)).body("authorized", equalTo(false)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedBatchResource()
	  public virtual void testIsUserAuthorizedBatchResource()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		string resourceName = BATCH.resourceName();
		int resourceType = BATCH.resourceType();
		ResourceUtil resource = new ResourceUtil(resourceName, resourceType);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, resourceType);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", resourceName).queryParam("resourceType", resourceType).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(resourceName)).body("resourceId", equalTo(null)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedProcessDefinitionResource()
	  public virtual void testIsUserAuthorizedProcessDefinitionResource()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		string resourceName = PROCESS_DEFINITION.resourceName();
		int resourceType = PROCESS_DEFINITION.resourceType();
		ResourceUtil resource = new ResourceUtil(resourceName, resourceType);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, resourceType);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", resourceName).queryParam("resourceType", resourceType).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(resourceName)).body("resourceId", equalTo(null)).body("authorized", equalTo(false)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedProcessInstanceResource()
	  public virtual void testIsUserAuthorizedProcessInstanceResource()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		int resourceType = PROCESS_INSTANCE.resourceType();
		string resourceName = PROCESS_INSTANCE.resourceName();
		ResourceUtil resource = new ResourceUtil(resourceName, resourceType);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, resourceType);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", resourceName).queryParam("resourceType", resourceType).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(resourceName)).body("resourceId", equalTo(null)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedTaskResource()
	  public virtual void testIsUserAuthorizedTaskResource()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		int resourceType = TASK.resourceType();
		string resourceName = TASK.resourceName();
		ResourceUtil resource = new ResourceUtil(resourceName, resourceType);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, resourceType);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", resourceName).queryParam("resourceType", resourceType).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(resourceName)).body("resourceId", equalTo(null)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedWithUserIdTrue()
	  public virtual void testIsUserAuthorizedWithUserIdTrue()
	  {

		IList<string> currentUserGroups = new List<string>();
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, currentUserGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		IList<string> userToCheckGroups = setupGroupQueryMock();

		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID2, userToCheckGroups, permission, resource)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).queryParam("userId", MockProvider.EXAMPLE_USER_ID2).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(null)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID2, userToCheckGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedWithUserIdFalse()
	  public virtual void testIsUserAuthorizedWithUserIdFalse()
	  {

		IList<string> currentUserGroups = new List<string>();
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, currentUserGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		IList<string> userToCheckGroups = setupGroupQueryMock();

		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID2, userToCheckGroups, permission, resource)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).queryParam("userId", MockProvider.EXAMPLE_USER_ID2).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(null)).body("authorized", equalTo(false)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID2, userToCheckGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedWithUserIdNoReadPermission()
	  public virtual void testIsUserAuthorizedWithUserIdNoReadPermission()
	  {

		IList<string> currentUserGroups = new List<string>();
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, currentUserGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		IList<string> userToCheckGroups = setupGroupQueryMock();

		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).queryParam("userId", MockProvider.EXAMPLE_USER_ID2).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(MediaType.APPLICATION_JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("You must have READ permission for Authorization resource.")).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, currentUserGroups, Permissions.READ, Resources.AUTHORIZATION);
		verify(authorizationServiceMock, never()).isUserAuthorized(MockProvider.EXAMPLE_USER_ID2, userToCheckGroups, permission, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedNotValidPermission()
	  public virtual void testIsUserAuthorizedNotValidPermission()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		string resourceName = BATCH.resourceName();
		int resourceType = BATCH.resourceType();
		ResourceUtil resource = new ResourceUtil(resourceName, resourceType);

		// ACCESS permission is not valid for BATCH

		given().queryParam("permissionName", Permissions.ACCESS.name()).queryParam("resourceName", resourceName).queryParam("resourceType", resourceType).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo("The permission 'ACCESS' is not valid for 'BATCH' resource type.")).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(0)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, Permissions.ACCESS, resource);
		verify(identityServiceMock, times(1)).CurrentAuthentication;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedResourceIdTrue()
	  public virtual void testIsUserAuthorizedResourceIdTrue()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource, MockProvider.EXAMPLE_RESOURCE_ID)).thenReturn(true);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).queryParam("resourceId", MockProvider.EXAMPLE_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(MockProvider.EXAMPLE_RESOURCE_ID)).body("authorized", equalTo(true)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource, MockProvider.EXAMPLE_RESOURCE_ID);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsUserAuthorizedResourceIdFalse()
	  public virtual void testIsUserAuthorizedResourceIdFalse()
	  {

		IList<string> exampleGroups = new List<string>();

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, exampleGroups);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource, MockProvider.EXAMPLE_RESOURCE_ID)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).queryParam("resourceId", MockProvider.EXAMPLE_RESOURCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).body("permissionName", equalTo(MockProvider.EXAMPLE_PERMISSION_NAME)).body("resourceName", equalTo(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME)).body("resourceId", equalTo(MockProvider.EXAMPLE_RESOURCE_ID)).body("authorized", equalTo(false)).when().get(AUTH_CHECK_PATH);

		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource, MockProvider.EXAMPLE_RESOURCE_ID);
		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void testIsUserAuthorizedNoAuthentication()
	  public virtual void testIsUserAuthorizedNoAuthentication()
	  {

		IList<string> exampleGroups = new List<string>();

		when(identityServiceMock.CurrentAuthentication).thenReturn(null);

		ResourceUtil resource = new ResourceUtil(MockProvider.EXAMPLE_RESOURCE_TYPE_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		Permission permission = PermissionProvider.getPermissionForName(MockProvider.EXAMPLE_PERMISSION_NAME, MockProvider.EXAMPLE_RESOURCE_TYPE_ID);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, exampleGroups, permission, resource)).thenReturn(false);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).when().get(AUTH_CHECK_PATH);

		verify(identityServiceMock, times(1)).CurrentAuthentication;
		verify(authorizationServiceMock, never()).isUserAuthorized(any(typeof(string)), any(typeof(System.Collections.IList)), any(typeof(Permission)), any(typeof(Resource)));
		verify(authorizationServiceMock, never()).isUserAuthorized(any(typeof(string)), any(typeof(System.Collections.IList)), any(typeof(Permission)), any(typeof(Resource)), any(typeof(string)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") public void testIsUserAuthorizedBadRequests()
	  public virtual void testIsUserAuthorizedBadRequests()
	  {

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(AUTH_CHECK_PATH);

		given().queryParam("permissionName", MockProvider.EXAMPLE_PERMISSION_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(AUTH_CHECK_PATH);

		given().queryParam("resourceName", MockProvider.EXAMPLE_RESOURCE_TYPE_NAME).queryParam("resourceType", MockProvider.EXAMPLE_RESOURCE_TYPE_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(AUTH_CHECK_PATH);

		verify(identityServiceMock, never()).CurrentAuthentication;
		verify(authorizationServiceMock, never()).isUserAuthorized(any(typeof(string)), any(typeof(System.Collections.IList)), any(typeof(Permission)), any(typeof(Resource)));
		verify(authorizationServiceMock, never()).isUserAuthorized(any(typeof(string)), any(typeof(System.Collections.IList)), any(typeof(Permission)), any(typeof(Resource)), any(typeof(string)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateGlobalAuthorization()
	  public virtual void testCreateGlobalAuthorization()
	  {

		Authorization authorization = MockProvider.createMockGlobalAuthorization();
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL)).thenReturn(authorization);
		when(authorizationServiceMock.saveAuthorization(authorization)).thenReturn(authorization);

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(AUTH_CREATE_PATH);

		verify(authorizationServiceMock).createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		verify(authorization).UserId = org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
		verify(authorization, times(4)).ResourceType = authorization.AuthorizationType;
		verify(authorization, times(2)).ResourceId = authorization.ResourceId;
		verify(authorization, times(2)).Permissions = authorization.getPermissions(Permissions.values());
		verify(authorizationServiceMock).saveAuthorization(authorization);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateGrantAuthorization()
	  public virtual void testCreateGrantAuthorization()
	  {

		Authorization authorization = MockProvider.createMockGrantAuthorization();
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)).thenReturn(authorization);
		when(authorizationServiceMock.saveAuthorization(authorization)).thenReturn(authorization);

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(AUTH_CREATE_PATH);

		verify(authorizationServiceMock).createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		verify(authorization, times(2)).UserId = authorization.UserId;
		verify(authorization, times(4)).ResourceType = authorization.AuthorizationType;
		verify(authorization, times(2)).ResourceId = authorization.ResourceId;
		verify(authorization, times(2)).Permissions = authorization.getPermissions(Permissions.values());
		verify(authorizationServiceMock).saveAuthorization(authorization);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateRevokeAuthorization()
	  public virtual void testCreateRevokeAuthorization()
	  {

		Authorization authorization = MockProvider.createMockRevokeAuthorization();
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE)).thenReturn(authorization);
		when(authorizationServiceMock.saveAuthorization(authorization)).thenReturn(authorization);

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(AUTH_CREATE_PATH);

		verify(authorizationServiceMock).createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE);
		verify(authorization, times(2)).UserId = authorization.UserId;
		verify(authorization, times(4)).ResourceType = authorization.AuthorizationType;
		verify(authorization, times(2)).ResourceId = authorization.ResourceId;
		verify(authorization, times(2)).Permissions = authorization.getPermissions(Permissions.values());
		verify(authorizationServiceMock).saveAuthorization(authorization);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAuthorizationThrowsAuthorizationException()
	  public virtual void testCreateAuthorizationThrowsAuthorizationException()
	  {
		string message = "expected authorization exception";
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)).thenThrow(new AuthorizationException(message));

		Authorization authorization = MockProvider.createMockGrantAuthorization();
		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(AUTH_CREATE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAuthorizationNotValidPermission()
	  public virtual void testCreateAuthorizationNotValidPermission()
	  {
		Authorization authorization = MockProvider.createMockGrantAuthorization();
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)).thenReturn(authorization);

		IDictionary<string, object> jsonBody = new Dictionary<string, object>();

		jsonBody["type"] = org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
		jsonBody["permissions"] = Arrays.asList(Permissions.READ_INSTANCE.name());
		jsonBody["userId"] = MockProvider.EXAMPLE_USER_ID + "," + MockProvider.EXAMPLE_USER_ID2;
		jsonBody["groupId"] = MockProvider.EXAMPLE_GROUP_ID + "," + MockProvider.EXAMPLE_GROUP_ID2;
		jsonBody["resourceType"] = Resources.TASK.resourceType();
		jsonBody["resourceId"] = MockProvider.EXAMPLE_RESOURCE_ID;

		// READ_INSTANCE permission is not valid for TASK

		given().body(jsonBody).contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo("The permission 'READ_INSTANCE' is not valid for 'TASK' resource type.")).when().post(AUTH_CREATE_PATH);

		verify(authorizationServiceMock, times(1)).createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		verify(authorizationServiceMock, never()).saveAuthorization(authorization);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveAuthorizationThrowsAuthorizationException()
	  public virtual void testSaveAuthorizationThrowsAuthorizationException()
	  {
		string message = "expected authorization exception";
		when(authorizationServiceMock.saveAuthorization(any(typeof(Authorization)))).thenThrow(new AuthorizationException(message));

		Authorization authorization = MockProvider.createMockGrantAuthorization();
		when(authorizationServiceMock.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT)).thenReturn(authorization);
		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(AUTH_CREATE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAuthorization()
	  public virtual void testDeleteAuthorization()
	  {

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(AUTH_RESOURCE_PATH);

		verify(authorizationQuery).authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID);
		verify(authorizationServiceMock).deleteAuthorization(MockProvider.EXAMPLE_AUTHORIZATION_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingAuthorization()
	  public virtual void testDeleteNonExistingAuthorization()
	  {

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("message", equalTo("Authorization with id " + MockProvider.EXAMPLE_AUTHORIZATION_ID + " does not exist.")).when().delete(AUTH_RESOURCE_PATH);

		verify(authorizationServiceMock, never()).deleteAuthorization(MockProvider.EXAMPLE_AUTHORIZATION_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteAuthorizationThrowsAuthorizationException()
	  public virtual void testDeleteAuthorizationThrowsAuthorizationException()
	  {
		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		string message = "expected authorization exception";
		doThrow(new AuthorizationException(message)).when(authorizationServiceMock).deleteAuthorization(MockProvider.EXAMPLE_AUTHORIZATION_ID);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().delete(AUTH_RESOURCE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateAuthorization()
	  public virtual void testUpdateAuthorization()
	  {

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(AUTH_RESOURCE_PATH);

		verify(authorizationQuery).authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID);

		verify(authorization).GroupId = dto.GroupId;
		verify(authorization).UserId = dto.UserId;
		verify(authorization).ResourceId = dto.ResourceId;
		verify(authorization).ResourceType = dto.ResourceType;

		verify(authorizationServiceMock).saveAuthorization(authorization);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateNonExistingAuthorization()
	  public virtual void testUpdateNonExistingAuthorization()
	  {

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(null);

		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("message", equalTo("Authorization with id " + MockProvider.EXAMPLE_AUTHORIZATION_ID + " does not exist.")).when().put(AUTH_RESOURCE_PATH);

		verify(authorizationServiceMock, never()).saveAuthorization(authorization);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateAuthorizationThrowsAuthorizationException()
	  public virtual void testUpdateAuthorizationThrowsAuthorizationException()
	  {
		Authorization authorization = MockProvider.createMockGlobalAuthorization();
		AuthorizationDto dto = AuthorizationDto.fromAuthorization(authorization, processEngineConfigurationMock);

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		string message = "expected authorization exception";
		when(authorizationServiceMock.saveAuthorization(any(typeof(Authorization)))).thenThrow(new AuthorizationException(message));

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).body(dto).contentType(ContentType.JSON).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(AUTH_RESOURCE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateAuthorizationNotValidPermission()
	  public virtual void testUpdateAuthorizationNotValidPermission()
	  {
		Authorization authorization = MockProvider.createMockGlobalAuthorization();
		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		IDictionary<string, object> jsonBody = new Dictionary<string, object>();

		jsonBody["permissions"] = Arrays.asList(Permissions.TASK_WORK.name());
		jsonBody["userId"] = MockProvider.EXAMPLE_USER_ID + "," + MockProvider.EXAMPLE_USER_ID2;
		jsonBody["groupId"] = MockProvider.EXAMPLE_GROUP_ID + "," + MockProvider.EXAMPLE_GROUP_ID2;
		jsonBody["resourceType"] = Resources.PROCESS_INSTANCE.resourceType();
		jsonBody["resourceId"] = MockProvider.EXAMPLE_RESOURCE_ID;

		// READ_INSTANCE permission is not valid for TASK

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).body(jsonBody).contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo("The permission 'TASK_WORK' is not valid for 'PROCESS_INSTANCE' resource type.")).when().put(AUTH_RESOURCE_PATH);

		verify(authorizationServiceMock, never()).saveAuthorization(authorization);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetAuthorizationById()
	  public virtual void testGetAuthorizationById()
	  {

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.JSON).body("id", equalTo(authorization.Id)).body("type", equalTo(authorization.AuthorizationType)).body("permissions[0]", equalTo(Permissions.READ.Name)).body("permissions[1]", equalTo(Permissions.UPDATE.Name)).body("userId", equalTo(authorization.UserId)).body("groupId", equalTo(authorization.GroupId)).body("resourceType", equalTo(authorization.ResourceType)).body("resourceId", equalTo(authorization.ResourceId)).when().get(AUTH_RESOURCE_PATH);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingAuthorizationById()
	  public virtual void testGetNonExistingAuthorizationById()
	  {

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("message", equalTo("Authorization with id " + MockProvider.EXAMPLE_AUTHORIZATION_ID + " does not exist.")).when().get(AUTH_RESOURCE_PATH);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthenticationRestServiceOptions()
	  public virtual void testAuthenticationRestServiceOptions()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_PATH);

		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthenticationRestServiceOptionsWithAuthorizationDisabled()
	  public virtual void testAuthenticationRestServiceOptionsWithAuthorizationDisabled()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullAuthorizationUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullAuthorizationUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(SERVICE_PATH);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationResourceOptions()
	  public virtual void testAuthorizationResourceOptions()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_AUTHORIZATION_ID;

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);
		when(identityServiceMock.CurrentAuthentication).thenReturn(null);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullAuthorizationUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullAuthorizationUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(AUTH_RESOURCE_PATH);

		verify(identityServiceMock, times(2)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationResourceOptionsUnauthorized()
	  public virtual void testAuthorizationResourceOptionsUnauthorized()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_AUTHORIZATION_ID;

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(false);

		when(processEngine.ProcessEngineConfiguration.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1]", nullValue()).body("links[2]", nullValue()).when().options(AUTH_RESOURCE_PATH);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationResourceOptionsUpdateUnauthorized()
	  public virtual void testAuthorizationResourceOptionsUpdateUnauthorized()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_AUTHORIZATION_ID;

		Authorization authorization = MockProvider.createMockGlobalAuthorization();

		AuthorizationQuery authorizationQuery = mock(typeof(AuthorizationQuery));
		when(authorizationServiceMock.createAuthorizationQuery()).thenReturn(authorizationQuery);
		when(authorizationQuery.authorizationId(MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(authorizationQuery);
		when(authorizationQuery.singleResult()).thenReturn(authorization);

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID)).thenReturn(false);

		when(processEngine.ProcessEngineConfiguration.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullAuthorizationUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2]", nullValue()).when().options(AUTH_RESOURCE_PATH);

		verify(identityServiceMock, times(2)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, AUTHORIZATION, MockProvider.EXAMPLE_AUTHORIZATION_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationResourceOptionsWithAuthorizationDisabled()
	  public virtual void testAuthorizationResourceOptionsWithAuthorizationDisabled()
	  {
		string fullAuthorizationUrl = "http://localhost:" + PORT + TEST_RESOURCE_ROOT_PATH + AuthorizationRestService_Fields.PATH + "/" + MockProvider.EXAMPLE_AUTHORIZATION_ID;

		when(processEngine.ProcessEngineConfiguration.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", MockProvider.EXAMPLE_AUTHORIZATION_ID).then().statusCode(Status.OK.StatusCode).body("links[0].href", equalTo(fullAuthorizationUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullAuthorizationUrl)).body("links[1].method", equalTo(HttpMethod.DELETE)).body("links[1].rel", equalTo("delete")).body("links[2].href", equalTo(fullAuthorizationUrl)).body("links[2].method", equalTo(HttpMethod.PUT)).body("links[2].rel", equalTo("update")).when().options(AUTH_RESOURCE_PATH);

		verifyNoAuthorizationCheckPerformed();
	  }

	  protected internal virtual void verifyNoAuthorizationCheckPerformed()
	  {
		verify(identityServiceMock, times(0)).CurrentAuthentication;
		verify(authorizationServiceMock, times(0)).isUserAuthorized(anyString(), anyListOf(typeof(string)), any(typeof(Permission)), any(typeof(Resource)));
	  }


	  protected internal virtual PermissionProvider PermissionProvider
	  {
		  get
		  {
			return processEngineConfigurationMock.PermissionProvider;
		  }
	  }

	  protected internal virtual IList<string> setupGroupQueryMock()
	  {
		GroupQuery mockGroupQuery = mock(typeof(GroupQuery));
		IList<Group> groupMocks = MockProvider.createMockGroups();
		when(identityServiceMock.createGroupQuery()).thenReturn(mockGroupQuery);
		when(mockGroupQuery.groupMember(anyString())).thenReturn(mockGroupQuery);
		when(mockGroupQuery.list()).thenReturn(groupMocks);
		IList<string> groupIds = new List<string>();
		foreach (Group group in groupMocks)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }
	}

}