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
namespace org.camunda.bpm.engine.impl.identity
{

	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class DefaultPasswordPolicyImpl : PasswordPolicy
	{

	  protected internal const string PLACEHOLDER_PREFIX = "PASSWORD_POLICY_";

	  // password length
	  public const int MIN_LENGTH = 10;
	  // password complexity
	  public const int MIN_LOWERCASE = 1;
	  public const int MIN_UPPERCSE = 1;
	  public const int MIN_DIGIT = 1;
	  public const int MIN_SPECIAL = 1;

	  protected internal readonly IList<PasswordPolicyRule> rules = new List<PasswordPolicyRule>();

	  public DefaultPasswordPolicyImpl()
	  {
		rules.Add(new PasswordPolicyLengthRuleImpl(MIN_LENGTH));
		rules.Add(new PasswordPolicyLowerCaseRuleImpl(MIN_LOWERCASE));
		rules.Add(new PasswordPolicyUpperCaseRuleImpl(MIN_UPPERCSE));
		rules.Add(new PasswordPolicyDigitRuleImpl(MIN_DIGIT));
		rules.Add(new PasswordPolicySpecialCharacterRuleImpl(MIN_SPECIAL));
	  }

	  public virtual IList<PasswordPolicyRule> Rules
	  {
		  get
		  {
			return this.rules;
		  }
	  }
	}
}