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
namespace org.camunda.bpm.qa.upgrade.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	public class AuthorizationTest
	{

	  protected internal AuthorizationService authorizationService;

	  protected internal IdentityService identityService;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal bool defaultAuthorizationEnabled;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.ProcessEngineRule();
	  public ProcessEngineRule rule = new ProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		authorizationService = rule.AuthorizationService;
		identityService = rule.IdentityService;
		processEngineConfiguration = rule.ProcessEngineConfiguration;
		defaultAuthorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void restoreAuthorization()
	  public virtual void restoreAuthorization()
	  {
		processEngineConfiguration.AuthorizationEnabled = defaultAuthorizationEnabled;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultAuthorizationQueryForCamundaAdminOnUpgrade()
	  public virtual void testDefaultAuthorizationQueryForCamundaAdminOnUpgrade()
	  {

		processEngineConfiguration.AuthorizationEnabled = true;

		assertEquals(1, authorizationService.createAuthorizationQuery().resourceType(Resources.TENANT).groupIdIn(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN).hasPermission(Permissions.ALL).count());

		assertEquals(1, authorizationService.createAuthorizationQuery().resourceType(Resources.TENANT_MEMBERSHIP).groupIdIn(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN).hasPermission(Permissions.ALL).count());

		assertEquals(1, authorizationService.createAuthorizationQuery().resourceType(Resources.BATCH).groupIdIn(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN).hasPermission(Permissions.ALL).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultAuthorizationForCamundaAdminOnUpgrade()
	  public virtual void testDefaultAuthorizationForCamundaAdminOnUpgrade()
	  {

		// The below test cases are skipped for H2 as there is a bug in H2 version 1.3 (Query does not return the expected output)
		// This H2 exclusion check will be removed as part of CAM-6044, when the H2 database is upgraded to the version 1.4 (Bug was fixed)
		// Update: Upgrading to 1.4.190 did not help, still failing -> CAM-
		if (DbSqlSessionFactory.H2.Equals(processEngineConfiguration.DatabaseType))
		{
		  return;
		}

		processEngineConfiguration.AuthorizationEnabled = true;
		assertEquals(true,authorizationService.isUserAuthorized(null, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), Permissions.ALL, Resources.TENANT));
		assertEquals(true,authorizationService.isUserAuthorized(null, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), Permissions.ALL, Resources.TENANT_MEMBERSHIP));
		assertEquals(true,authorizationService.isUserAuthorized(null, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN), Permissions.ALL, Resources.BATCH));
	  }
	}

}