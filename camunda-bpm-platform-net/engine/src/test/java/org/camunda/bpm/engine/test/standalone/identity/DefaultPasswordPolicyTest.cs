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
namespace org.camunda.bpm.engine.test.standalone.identity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsInstanceOf.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;

	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;
	using DefaultPasswordPolicyImpl = org.camunda.bpm.engine.impl.identity.DefaultPasswordPolicyImpl;
	using PasswordPolicyDigitRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyDigitRuleImpl;
	using PasswordPolicyLengthRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyLengthRuleImpl;
	using PasswordPolicyLowerCaseRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyLowerCaseRuleImpl;
	using PasswordPolicySpecialCharacterRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicySpecialCharacterRuleImpl;
	using PasswordPolicyUpperCaseRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyUpperCaseRuleImpl;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class DefaultPasswordPolicyTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule rule = new ProcessEngineRule(true);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal IdentityService identityService;

	  // enforces a minimum length of 10 characters, at least one upper, one
	  // lower case, one digit and one special character
	  protected internal PasswordPolicy policy = new DefaultPasswordPolicyImpl();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		identityService = rule.IdentityService;

		rule.ProcessEngineConfiguration.setPasswordPolicy(new DefaultPasswordPolicyImpl()).setEnablePasswordPolicy(true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetProcessEngineConfig()
	  public virtual void resetProcessEngineConfig()
	  {
		rule.ProcessEngineConfiguration.setPasswordPolicy(null).setEnablePasswordPolicy(false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGoodPassword()
	  public virtual void testGoodPassword()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "LongPas$w0rd");
		assertThat(result.ViolatedRules.Count, @is(0));
		assertThat(result.FulfilledRules.Count, @is(5));
		assertThat(result.Valid, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckValidPassword_WithoutPassingPolicy()
	  public virtual void shouldCheckValidPassword_WithoutPassingPolicy()
	  {
		// given

		// when
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy("LongPas$w0rd");

		// then
		assertThat(result, notNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPasswordWithoutLowerCase()
	  public virtual void testPasswordWithoutLowerCase()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "LONGPAS$W0RD");
		checkThatPasswordWasInvalid(result);

		PasswordPolicyRule rule = result.ViolatedRules[0];
		assertThat(rule.Placeholder, @is(PasswordPolicyLowerCaseRuleImpl.PLACEHOLDER));
		assertThat(rule, instanceOf(typeof(PasswordPolicyLowerCaseRuleImpl)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPasswordWithoutUpperCase()
	  public virtual void testPasswordWithoutUpperCase()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "longpas$w0rd");
		checkThatPasswordWasInvalid(result);

		PasswordPolicyRule rule = result.ViolatedRules[0];
		assertThat(rule.Placeholder, @is(PasswordPolicyUpperCaseRuleImpl.PLACEHOLDER));
		assertThat(rule, instanceOf(typeof(PasswordPolicyUpperCaseRuleImpl)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPasswordWithoutSpecialChar()
	  public virtual void testPasswordWithoutSpecialChar()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "LongPassw0rd");
		checkThatPasswordWasInvalid(result);

		PasswordPolicyRule rule = result.ViolatedRules[0];
		assertThat(rule.Placeholder, @is(PasswordPolicySpecialCharacterRuleImpl.PLACEHOLDER));
		assertThat(rule, instanceOf(typeof(PasswordPolicySpecialCharacterRuleImpl)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPasswordWithoutDigit()
	  public virtual void testPasswordWithoutDigit()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "LongPas$word");
		checkThatPasswordWasInvalid(result);

		PasswordPolicyRule rule = result.ViolatedRules[0];
		assertThat(rule.Placeholder, @is(PasswordPolicyDigitRuleImpl.PLACEHOLDER));
		assertThat(rule, instanceOf(typeof(PasswordPolicyDigitRuleImpl)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testShortPassword()
	  public virtual void testShortPassword()
	  {
		PasswordPolicyResult result = identityService.checkPasswordAgainstPolicy(policy, "Pas$w0rd");
		checkThatPasswordWasInvalid(result);

		PasswordPolicyRule rule = result.ViolatedRules[0];
		assertThat(rule.Placeholder, @is(PasswordPolicyLengthRuleImpl.PLACEHOLDER));
		assertThat(rule, instanceOf(typeof(PasswordPolicyLengthRuleImpl)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowNullValueException_policyNull()
	  public virtual void shouldThrowNullValueException_policyNull()
	  {
		// given

		// then
		thrown.expectMessage("policy is null");
		thrown.expect(typeof(NullValueException));

		// when
		identityService.checkPasswordAgainstPolicy(null, "Pas$w0rd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowNullValueException_passwordNull()
	  public virtual void shouldThrowNullValueException_passwordNull()
	  {
		// given

		// then
		thrown.expectMessage("password is null");
		thrown.expect(typeof(NullValueException));

		// when
		identityService.checkPasswordAgainstPolicy(policy, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGetPasswordPolicy()
	  public virtual void shouldGetPasswordPolicy()
	  {
		// given

		// then
		PasswordPolicy passwordPolicy = identityService.PasswordPolicy;

		// when
		assertThat(passwordPolicy, notNullValue());
	  }

	  private void checkThatPasswordWasInvalid(PasswordPolicyResult result)
	  {
		assertThat(result.ViolatedRules.Count, @is(1));
		assertThat(result.FulfilledRules.Count, @is(4));
		assertThat(result.Valid, @is(false));
	  }
	}
}