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
namespace org.camunda.bpm.engine.rest.standalone
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using AuthorizationServiceImpl = org.camunda.bpm.engine.impl.AuthorizationServiceImpl;
	using IdentityServiceImpl = org.camunda.bpm.engine.impl.IdentityServiceImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using NamedProcessEngineRestServiceImpl = org.camunda.bpm.engine.rest.impl.NamedProcessEngineRestServiceImpl;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public abstract class AbstractAuthenticationFilterTest : AbstractRestServiceTest
	{

	  protected internal const string SERVLET_PATH = "/rest";
	  protected internal static readonly string SERVICE_PATH = TEST_RESOURCE_ROOT_PATH + SERVLET_PATH + NamedProcessEngineRestServiceImpl.PATH + "/{name}" + org.camunda.bpm.engine.rest.ProcessDefinitionRestService_Fields.PATH;

	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal IdentityService identityServiceMock;
	  protected internal RepositoryService repositoryServiceMock;

	  protected internal User userMock;
	  protected internal IList<string> groupIds;
	  protected internal IList<string> tenantIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		authorizationServiceMock = mock(typeof(AuthorizationServiceImpl));
		identityServiceMock = mock(typeof(IdentityServiceImpl));
		repositoryServiceMock = mock(typeof(RepositoryService));

		when(processEngine.AuthorizationService).thenReturn(authorizationServiceMock);
		when(processEngine.IdentityService).thenReturn(identityServiceMock);
		when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);

		// for authentication
		userMock = MockProvider.createMockUser();

		IList<Group> groupMocks = MockProvider.createMockGroups();
		groupIds = setupGroupQueryMock(groupMocks);

		IList<Tenant> tenantMocks = Collections.singletonList(MockProvider.createMockTenant());
		tenantIds = setupTenantQueryMock(tenantMocks);

		// example method
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		IList<ProcessDefinition> mockDefinitions = Arrays.asList(mockDefinition);
		ProcessDefinitionQuery mockQuery = mock(typeof(ProcessDefinitionQuery));
		when(repositoryServiceMock.createProcessDefinitionQuery()).thenReturn(mockQuery);
		when(mockQuery.list()).thenReturn(mockDefinitions);
	  }

	  protected internal virtual IList<string> setupGroupQueryMock(IList<Group> groups)
	  {
		GroupQuery mockGroupQuery = mock(typeof(GroupQuery));

		when(identityServiceMock.createGroupQuery()).thenReturn(mockGroupQuery);
		when(mockGroupQuery.groupMember(anyString())).thenReturn(mockGroupQuery);
		when(mockGroupQuery.list()).thenReturn(groups);

		IList<string> groupIds = new List<string>();
		foreach (Group groupMock in groups)
		{
		  groupIds.Add(groupMock.Id);
		}
		return groupIds;
	  }

	  protected internal virtual IList<string> setupTenantQueryMock(IList<Tenant> tenants)
	  {
		TenantQuery mockTenantQuery = mock(typeof(TenantQuery));

		when(identityServiceMock.createTenantQuery()).thenReturn(mockTenantQuery);
		when(mockTenantQuery.userMember(anyString())).thenReturn(mockTenantQuery);
		when(mockTenantQuery.includingGroupsOfUser(anyBoolean())).thenReturn(mockTenantQuery);
		when(mockTenantQuery.list()).thenReturn(tenants);

		IList<string> tenantIds = new List<string>();
		foreach (Tenant tenant in tenants)
		{
		  tenantIds.Add(tenant.Id);
		}
		return tenantIds;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHttpBasicAuthenticationCheck()
	  public virtual void testHttpBasicAuthenticationCheck()
	  {
		when(identityServiceMock.checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD)).thenReturn(true);

		given().auth().basic(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD).pathParam("name", "default").then().expect().statusCode(Status.OK.StatusCode).contentType(MediaType.APPLICATION_JSON).when().get(SERVICE_PATH);

		verify(identityServiceMock).setAuthentication(MockProvider.EXAMPLE_USER_ID, groupIds, tenantIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailingAuthenticationCheck()
	  public virtual void testFailingAuthenticationCheck()
	  {
		when(identityServiceMock.checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD)).thenReturn(false);

		given().auth().basic(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD).pathParam("name", "default").then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).header(HttpHeaders.WWW_AUTHENTICATE, "Basic realm=\"default\"").when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingAuthHeader()
	  public virtual void testMissingAuthHeader()
	  {
		given().pathParam("name", "someengine").then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).header(HttpHeaders.WWW_AUTHENTICATE, "Basic realm=\"someengine\"").when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnexpectedAuthHeaderFormat()
	  public virtual void testUnexpectedAuthHeaderFormat()
	  {
		given().header(HttpHeaders.AUTHORIZATION, "Digest somevalues, and, some, more").pathParam("name", "someengine").then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).header(HttpHeaders.WWW_AUTHENTICATE, "Basic realm=\"someengine\"").when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMalformedCredentials()
	  public virtual void testMalformedCredentials()
	  {
		given().header(HttpHeaders.AUTHORIZATION, "Basic " + StringHelper.NewString(Base64.encodeBase64("this is not a valid format".GetBytes()))).pathParam("name", "default").then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).header(HttpHeaders.WWW_AUTHENTICATE, "Basic realm=\"default\"").when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNonExistingEngineAuthentication()
	  public virtual void testNonExistingEngineAuthentication()
	  {
		given().auth().basic(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD).pathParam("name", MockProvider.NON_EXISTING_PROCESS_ENGINE_NAME).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Process engine " + MockProvider.NON_EXISTING_PROCESS_ENGINE_NAME + " not available")).when().get(SERVICE_PATH);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMalformedBase64Value()
	  public virtual void testMalformedBase64Value()
	  {
		given().header(HttpHeaders.AUTHORIZATION, "Basic someNonBase64Characters!(#").pathParam("name", "default").then().expect().statusCode(Status.UNAUTHORIZED.StatusCode).header(HttpHeaders.WWW_AUTHENTICATE, "Basic realm=\"default\"").when().get(SERVICE_PATH);
	  }

	}

}