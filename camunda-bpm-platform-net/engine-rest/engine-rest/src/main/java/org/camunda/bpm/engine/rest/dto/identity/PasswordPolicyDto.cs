﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.dto.identity
{

	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class PasswordPolicyDto
	{
	  protected internal IList<PasswordPolicyRuleDto> rules = new List<PasswordPolicyRuleDto>();

	  // transformers

	  public static PasswordPolicyDto fromPasswordPolicy(PasswordPolicy policy)
	  {
		PasswordPolicyDto policyDto = new PasswordPolicyDto();
		foreach (PasswordPolicyRule rule in policy.Rules)
		{
		  policyDto.rules.Add(new PasswordPolicyRuleDto(rule));
		}
		return policyDto;
	  }

	  // getters / setters

	  public virtual IList<PasswordPolicyRuleDto> Rules
	  {
		  get
		  {
			return rules;
		  }
		  set
		  {
			this.rules = value;
		  }
	  }

	}
}