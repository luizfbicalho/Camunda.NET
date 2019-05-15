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

	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class PasswordPolicySpecialCharacterRuleImpl : PasswordPolicyRule
	{

	  public const string SPECIALCHARACTERS = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
	  public static readonly string PLACEHOLDER = DefaultPasswordPolicyImpl.PLACEHOLDER_PREFIX + "SPECIAL";

	  protected internal int minSpecial;

	  public PasswordPolicySpecialCharacterRuleImpl(int minSpecial)
	  {
		this.minSpecial = minSpecial;
	  }

	  public virtual string Placeholder
	  {
		  get
		  {
			return PasswordPolicySpecialCharacterRuleImpl.PLACEHOLDER;
		  }
	  }

	  public virtual IDictionary<string, string> Parameters
	  {
		  get
		  {
			IDictionary<string, string> parameter = new Dictionary<string, string>();
			parameter["minSpecial"] = "" + this.minSpecial;
			return parameter;
		  }
	  }

	  public virtual bool execute(string password)
	  {
		int specialCount = 0;
		foreach (char? c in password.ToCharArray())
		{
		  if (SPECIALCHARACTERS.IndexOf(c) != -1)
		  {
			specialCount++;
		  }
		  if (specialCount >= this.minSpecial)
		  {
			return true;
		  }
		}
		return false;
	  }
	}
}