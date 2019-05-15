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
namespace org.camunda.bpm.identity.impl.ldap
{

	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Assert = org.junit.Assert;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class LdapQueryToleranceTest : ResourceProcessEngineTestCase
	{

	  private LdapTestEnvironment ldapTestEnvironment;

	  public LdapQueryToleranceTest() : base("invalid-id-attributes.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		ldapTestEnvironment = new LdapTestEnvironment();
		ldapTestEnvironment.init();
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		if (ldapTestEnvironment != null)
		{
		  ldapTestEnvironment.shutdown();
		  ldapTestEnvironment = null;
		}
		base.tearDown();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testNotReturnGroupsWithNullId() throws Exception
	  public virtual void testNotReturnGroupsWithNullId()
	  {
		// given
		// LdapTestEnvironment creates six groups by default;
		// these won't return a group id, because they do not have the group id attribute
		// defined in the ldap plugin config
		// the plugin should not return such groups and instead log an error

		// when
		IList<Group> groups = processEngine.IdentityService.createGroupQuery().list();
		long count = processEngine.IdentityService.createGroupQuery().count();

		// then
		// groups with id null were not returned
		Assert.assertEquals(0, groups.Count);
		Assert.assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testNotReturnUsersWithNullId() throws Exception
	  public virtual void testNotReturnUsersWithNullId()
	  {
		// given
		// LdapTestEnvironment creates six groups by default;
		// these won't return a group id, because they do not have the group id attribute
		// defined in the ldap plugin config
		// the plugin should not return such groups and instead log an error

		// when
		IList<User> users = processEngine.IdentityService.createUserQuery().list();
		long count = processEngine.IdentityService.createGroupQuery().count();

		// then
		// groups with id null were not returned
		Assert.assertEquals(0, users.Count);
		Assert.assertEquals(0, count);
	  }
	}

}