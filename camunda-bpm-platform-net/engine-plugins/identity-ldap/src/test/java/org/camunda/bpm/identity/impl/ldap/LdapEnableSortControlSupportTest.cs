﻿using System;
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
	using User = org.camunda.bpm.engine.identity.User;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;

	/// <summary>
	/// Represents a test case where the sortControlSupport property is enabled.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class LdapEnableSortControlSupportTest : ResourceProcessEngineTestCase
	{

	  public LdapEnableSortControlSupportTest() : base("camunda.ldap.enable.sort.control.support.cfg.xml")
	  {
	  }

	  protected internal static LdapTestEnvironment ldapTestEnvironment;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		if (ldapTestEnvironment == null)
		{
		  ldapTestEnvironment = new LdapTestEnvironment();
		  ldapTestEnvironment.init();
		}
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



	  public virtual void testOrderByUserFirstName()
	  {
		IList<User> orderedUsers = identityService.createUserQuery().orderByUserLastName().asc().list();
		IList<User> userList = identityService.createUserQuery().list();

		userList.Sort(new ComparatorAnonymousInnerClass(this));

		int len = orderedUsers.Count;
		for (int i = 0; i < len; i++)
		{
		  assertEquals("Index: " + i, orderedUsers[i].LastName, userList[i].LastName);
		}
	  }

	  private class ComparatorAnonymousInnerClass : IComparer<User>
	  {
		  private readonly LdapEnableSortControlSupportTest outerInstance;

		  public ComparatorAnonymousInnerClass(LdapEnableSortControlSupportTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public int compare(User o1, User o2)
		  {
			return string.Compare(o1.LastName, o2.LastName, StringComparison.OrdinalIgnoreCase);
		  }
	  }
	}

}