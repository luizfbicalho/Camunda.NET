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
namespace org.camunda.bpm.engine.test.api.cfg
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.spy;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using AuthorizationCheck = org.camunda.bpm.engine.impl.db.AuthorizationCheck;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationCheckRevokesCfgTest
	{

	  private static readonly IList<string> AUTHENTICATED_GROUPS = Arrays.asList("aGroup");
	  private const string AUTHENTICATED_USER_ID = "userId";

	  internal CommandContext mockedCmdContext;
	  internal ProcessEngineConfigurationImpl mockedConfiguration;
	  internal AuthorizationManager authorizationManager;
	  internal DbEntityManager mockedEntityManager;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {

		mockedCmdContext = mock(typeof(CommandContext));
		mockedConfiguration = mock(typeof(ProcessEngineConfigurationImpl));
		authorizationManager = spy(new AuthorizationManager());
		mockedEntityManager = mock(typeof(DbEntityManager));

		when(mockedCmdContext.getSession(eq(typeof(DbEntityManager)))).thenReturn(mockedEntityManager);

		when(authorizationManager.filterAuthenticatedGroupIds(eq(AUTHENTICATED_GROUPS))).thenReturn(AUTHENTICATED_GROUPS);
		when(mockedCmdContext.Authentication).thenReturn(new Authentication(AUTHENTICATED_USER_ID, AUTHENTICATED_GROUPS));
		when(mockedCmdContext.AuthorizationCheckEnabled).thenReturn(true);
		when(mockedConfiguration.AuthorizationEnabled).thenReturn(true);

		Context.CommandContext = mockedCmdContext;
		Context.ProcessEngineConfiguration = mockedConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanup()
	  public virtual void cleanup()
	  {
		Context.removeCommandContext();
		Context.removeProcessEngineConfiguration();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUseCfgValue_always()
	  public virtual void shouldUseCfgValue_always()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_ALWAYS);

		// if
		authorizationManager.configureQuery(query);

		// then
		assertEquals(true, authCheck.RevokeAuthorizationCheckEnabled);
		verifyNoMoreInteractions(mockedEntityManager);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUseCfgValue_never()
	  public virtual void shouldUseCfgValue_never()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_NEVER);

		// if
		authorizationManager.configureQuery(query);

		// then
		assertEquals(false, authCheck.RevokeAuthorizationCheckEnabled);
		verify(mockedEntityManager, never()).selectBoolean(eq("selectRevokeAuthorization"), any());
		verifyNoMoreInteractions(mockedEntityManager);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckDbForCfgValue_auto()
	  public virtual void shouldCheckDbForCfgValue_auto()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
		Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
		expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
		expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
		when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).thenReturn(true);

		// if
		authorizationManager.configureQuery(query);

		// then
		assertEquals(true, authCheck.RevokeAuthorizationCheckEnabled);
		verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckDbForCfgValueWithNoRevokes_auto()
	  public virtual void shouldCheckDbForCfgValueWithNoRevokes_auto()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
		Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
		expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
		expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
		when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).thenReturn(false);

		// if
		authorizationManager.configureQuery(query);

		// then
		assertEquals(false, authCheck.RevokeAuthorizationCheckEnabled);
		verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckDbForCfgCaseInsensitive()
	  public virtual void shouldCheckDbForCfgCaseInsensitive()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
		Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
		expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
		expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn("AuTo");
		when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).thenReturn(true);

		// if
		authorizationManager.configureQuery(query);

		// then
		assertEquals(true, authCheck.RevokeAuthorizationCheckEnabled);
		verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCacheCheck()
	  public virtual void shouldCacheCheck()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.ListQueryParameterObject query = new org.camunda.bpm.engine.impl.db.ListQueryParameterObject();
		ListQueryParameterObject query = new ListQueryParameterObject();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.AuthorizationCheck authCheck = query.getAuthCheck();
		AuthorizationCheck authCheck = query.AuthCheck;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
		Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
		expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
		expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

		// given
		when(mockedConfiguration.AuthorizationCheckRevokes).thenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
		when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).thenReturn(true);

		// if
		authorizationManager.configureQuery(query);
		authorizationManager.configureQuery(query);

		// then
		assertEquals(true, authCheck.RevokeAuthorizationCheckEnabled);
		verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAutoIsDefault()
	  public virtual void testAutoIsDefault()
	  {
		assertEquals(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO, (new StandaloneProcessEngineConfiguration()).AuthorizationCheckRevokes);
	  }

	}

}