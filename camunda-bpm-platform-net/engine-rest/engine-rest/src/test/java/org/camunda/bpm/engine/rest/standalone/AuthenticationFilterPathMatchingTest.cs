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
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
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
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ProcessEngineAuthenticationFilter = org.camunda.bpm.engine.rest.security.auth.ProcessEngineAuthenticationFilter;
	using HttpBasicAuthenticationProvider = org.camunda.bpm.engine.rest.security.auth.impl.HttpBasicAuthenticationProvider;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameters = org.junit.runners.Parameterized.Parameters;
	using MockFilterChain = org.springframework.mock.web.MockFilterChain;
	using MockFilterConfig = org.springframework.mock.web.MockFilterConfig;
	using MockHttpServletRequest = org.springframework.mock.web.MockHttpServletRequest;
	using MockHttpServletResponse = org.springframework.mock.web.MockHttpServletResponse;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class AuthenticationFilterPathMatchingTest extends org.camunda.bpm.engine.rest.AbstractRestServiceTest
	public class AuthenticationFilterPathMatchingTest : AbstractRestServiceTest
	{

	  protected internal const string SERVICE_PATH = TEST_RESOURCE_ROOT_PATH;

	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal IdentityService identityServiceMock;
	  protected internal RepositoryService repositoryServiceMock;

	  protected internal User userMock;
	  protected internal IList<string> groupIds;
	  protected internal IList<string> tenantIds;

	  protected internal Filter authenticationFilter;

	  protected internal string servletPath;
	  protected internal string requestUrl;
	  protected internal string engineName;
	  protected internal bool authenticationExpected;

	  protected internal ProcessEngine currentEngine;

	  /// <summary>
	  /// Makes a request against the url SERVICE_PATH + 'servletPath' + 'requestUrl' and depending on the 'authenticationExpected' value,
	  /// asserts that authentication was carried out (or not) against the engine named 'engineName'
	  /// </summary>
	  public AuthenticationFilterPathMatchingTest(string servletPath, string requestUrl, string engineName, bool authenticationExpected)
	  {
		this.servletPath = servletPath;
		this.requestUrl = requestUrl;
		this.engineName = engineName;
		if (string.ReferenceEquals(engineName, null))
		{
		  this.engineName = "default";
		}
		this.authenticationExpected = authenticationExpected;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> getRequestUrls()
	  public static ICollection<object[]> RequestUrls
	  {
		  get
		  {
			return Arrays.asList(new object[][]
			{
				new object[] {"", "/engine/default/process-definition/and/a/longer/path", "default", true},
				new object[] {"", "/engine/default/process-definition/and/a/longer/path", "default", true},
				new object[] {"", "/engine/default/process-definition", "default", true},
				new object[] {"", "/engine/someOtherEngine/process-definition", "someOtherEngine", true},
				new object[] {"", "/engine/default/", "default", true},
				new object[] {"", "/engine/default", "default", true},
				new object[] {"", "/process-definition", "default", true},
				new object[] {"", "/engine", null, false},
				new object[] {"", "/engine/", null, false},
				new object[] {"", "/", "default", true},
				new object[] {"", "", "default", true},
				new object[] {"/someservlet", "/engine/someengine/process-definition", "someengine", true}
			});
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup() throws javax.servlet.ServletException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setup()
	  {
		currentEngine = getProcessEngine(engineName);

		authorizationServiceMock = mock(typeof(AuthorizationServiceImpl));
		identityServiceMock = mock(typeof(IdentityServiceImpl));
		repositoryServiceMock = mock(typeof(RepositoryService));

		when(currentEngine.AuthorizationService).thenReturn(authorizationServiceMock);
		when(currentEngine.IdentityService).thenReturn(identityServiceMock);

		// for authentication
		userMock = MockProvider.createMockUser();

		IList<Group> groupMocks = MockProvider.createMockGroups();
		groupIds = setupGroupQueryMock(groupMocks);

		IList<Tenant> tenantMocks = Collections.singletonList(MockProvider.createMockTenant());
		tenantIds = setupTenantQueryMock(tenantMocks);

		GroupQuery mockGroupQuery = mock(typeof(GroupQuery));

		when(identityServiceMock.createGroupQuery()).thenReturn(mockGroupQuery);
		when(mockGroupQuery.groupMember(anyString())).thenReturn(mockGroupQuery);
		when(mockGroupQuery.list()).thenReturn(groupMocks);

		setupFilter();
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setupFilter() throws javax.servlet.ServletException
	  protected internal virtual void setupFilter()
	  {
		MockFilterConfig config = new MockFilterConfig();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		config.addInitParameter(ProcessEngineAuthenticationFilter.AUTHENTICATION_PROVIDER_PARAM, typeof(HttpBasicAuthenticationProvider).FullName);
		authenticationFilter = new ProcessEngineAuthenticationFilter();
		authenticationFilter.init(config);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void applyFilter(org.springframework.mock.web.MockHttpServletRequest request, org.springframework.mock.web.MockHttpServletResponse response, String username, String password) throws java.io.IOException, javax.servlet.ServletException
	  protected internal virtual void applyFilter(MockHttpServletRequest request, MockHttpServletResponse response, string username, string password)
	  {
		string credentials = username + ":" + password;
		request.addHeader("Authorization", "Basic " + StringHelper.NewString(Base64.encodeBase64(credentials.GetBytes())));
		FilterChain filterChain = new MockFilterChain();

		authenticationFilter.doFilter(request, response, filterChain);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHttpBasicAuthenticationCheck() throws java.io.IOException, javax.servlet.ServletException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testHttpBasicAuthenticationCheck()
	  {
		if (authenticationExpected)
		{
		  when(identityServiceMock.checkPassword(MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD)).thenReturn(true);
		}

		MockHttpServletResponse response = new MockHttpServletResponse();
		MockHttpServletRequest request = new MockHttpServletRequest();
		request.RequestURI = SERVICE_PATH + servletPath + requestUrl;
		request.ContextPath = SERVICE_PATH;
		request.ServletPath = servletPath;
		applyFilter(request, response, MockProvider.EXAMPLE_USER_ID, MockProvider.EXAMPLE_USER_PASSWORD);

		Assert.assertEquals(Status.OK.StatusCode, response.Status);

		if (authenticationExpected)
		{
		  verify(identityServiceMock).setAuthentication(MockProvider.EXAMPLE_USER_ID, groupIds, tenantIds);
		  verify(identityServiceMock).clearAuthentication();

		}
		else
		{
		  verify(identityServiceMock, never()).setAuthentication(any(typeof(string)), anyListOf(typeof(string)), anyListOf(typeof(string)));
		  verify(identityServiceMock, never()).clearAuthentication();
		}
	  }


	}

}