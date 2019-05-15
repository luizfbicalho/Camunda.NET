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
namespace org.camunda.bpm.engine.test.api.identity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ReadOnlyIdentityServiceTest
	{

	  protected internal const string CONFIGURATION_RESOURCE = "org/camunda/bpm/engine/test/api/identity/read.only.identity.service.camunda.cfg.xml";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);
	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		identityService = engineRule.IdentityService;

		assertTrue(identityService.ReadOnly);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void newUser()
	  public virtual void newUser()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.newUser("user");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveUser()
	  public virtual void saveUser()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.saveUser(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteUser()
	  public virtual void deleteUser()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteUser("user");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void newGroup()
	  public virtual void newGroup()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.newGroup("group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveGroup()
	  public virtual void saveGroup()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.saveGroup(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteGroup()
	  public virtual void deleteGroup()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteGroup("group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void newTenant()
	  public virtual void newTenant()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.newTenant("tenant");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saveTenant()
	  public virtual void saveTenant()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.saveTenant(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenant()
	  public virtual void deleteTenant()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteTenant("tenant");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createGroupMembership()
	  public virtual void createGroupMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.createMembership("user", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteGroupMembership()
	  public virtual void deleteGroupMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteMembership("user", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantUserMembership()
	  public virtual void createTenantUserMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.createTenantUserMembership("tenant", "user");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createTenantGroupMembership()
	  public virtual void createTenantGroupMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.createTenantGroupMembership("tenant", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantUserMembership()
	  public virtual void deleteTenantUserMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteTenantUserMembership("tenant", "user");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteTenantGroupMembership()
	  public virtual void deleteTenantGroupMembership()
	  {
		thrown.expect(typeof(System.NotSupportedException));
		thrown.expectMessage("This identity service implementation is read-only.");

		identityService.deleteTenantGroupMembership("tenant", "group");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void checkPassword()
	  public virtual void checkPassword()
	  {
		identityService.checkPassword("user", "password");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createQuery()
	  public virtual void createQuery()
	  {
		identityService.createUserQuery().list();
		identityService.createGroupQuery().list();
		identityService.createTenantQuery().list();
	  }

	}

}