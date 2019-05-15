﻿/*
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
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DefaultPasswordPolicyImpl = org.camunda.bpm.engine.impl.identity.DefaultPasswordPolicyImpl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class CustomPasswordPolicyTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private ProcessEngineConfigurationImpl processEngineConfiguration;
	  private IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		identityService = engineRule.IdentityService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		processEngineConfiguration.PasswordPolicy = new DefaultPasswordPolicyImpl();
		processEngineConfiguration.EnablePasswordPolicy = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		// reset configuration
		processEngineConfiguration.PasswordPolicy = null;
		processEngineConfiguration.EnablePasswordPolicy = false;
		// reset database
		identityService.deleteUser("user");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPasswordPolicyConfiguration()
	  public virtual void testPasswordPolicyConfiguration()
	  {
		PasswordPolicy policy = processEngineConfiguration.PasswordPolicy;
		assertThat(policy.GetType().IsAssignableFrom(typeof(DefaultPasswordPolicyImpl)), @is(true));
		assertThat(policy.Rules.Count, @is(5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomPasswordPolicyWithNoPassword()
	  public virtual void testCustomPasswordPolicyWithNoPassword()
	  {
		thrown.expect(typeof(ProcessEngineException));
		User user = identityService.newUser("user");
		identityService.saveUser(user);
		thrown.expectMessage("password is null");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomPasswordPolicyWithCompliantPassword()
	  public virtual void testCustomPasswordPolicyWithCompliantPassword()
	  {
		User user = identityService.newUser("user");
		user.Password = "this-is-1-STRONG-password";
		identityService.saveUser(user);
		assertThat(identityService.createUserQuery().userId(user.Id).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomPasswordPolicyWithNonCompliantPassword()
	  public virtual void testCustomPasswordPolicyWithNonCompliantPassword()
	  {
		thrown.expect(typeof(ProcessEngineException));
		User user = identityService.newUser("user");
		user.Password = "weakpassword";
		identityService.saveUser(user);
		thrown.expectMessage("Password does not match policy");
		assertThat(identityService.createUserQuery().userId(user.Id).count(), @is(0L));
	  }
	}
}