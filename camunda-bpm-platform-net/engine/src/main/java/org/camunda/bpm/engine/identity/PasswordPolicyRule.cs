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
namespace org.camunda.bpm.engine.identity
{

	/// <summary>
	/// Describes a rule of a <seealso cref="PasswordPolicy"/>. All rules attached to a
	/// <seealso cref="PasswordPolicy"/> must be matched by passwords for engine-managed users
	/// to be policy compliant.
	/// 
	/// @author Miklas Boskamp
	/// </summary>
	public interface PasswordPolicyRule
	{

	  /// <summary>
	  /// Placeholder string that can be used to display a description of this rule.
	  /// The actual description text must be managed on the calling side.
	  /// </summary>
	  /// <returns> the placeholder for the description text. </returns>
	  string Placeholder {get;}

	  /// <summary>
	  /// Additional parameter that can be used to display a meaningful description.
	  /// </summary>
	  /// <returns> a map of parameters </returns>
	  IDictionary<string, string> Parameters {get;}

	  /// <summary>
	  /// Checks the given password against this rule.
	  /// </summary>
	  /// <param name="password">
	  ///          the string that should be checked </param>
	  /// <returns> <code>true</code> if the given password matches this rule.
	  ///         <code>false</code> if the given password is not compliant with this
	  ///         rule. </returns>
	  bool execute(string password);
	}
}