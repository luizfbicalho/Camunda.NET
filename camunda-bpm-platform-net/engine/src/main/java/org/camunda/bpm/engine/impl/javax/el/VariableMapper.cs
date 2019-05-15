/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.engine.impl.javax.el
{
	/// <summary>
	/// The interface to a map between EL variables and the EL expressions they are associated with.
	/// </summary>
	public abstract class VariableMapper
	{
		/// <summary>
		/// Resolves the specified variable name to a ValueExpression.
		/// </summary>
		/// <param name="variable">
		///            The variable name </param>
		/// <returns> the ValueExpression assigned to the variable, null if there is no previous assignment
		///         to this variable. </returns>
		public abstract ValueExpression resolveVariable(string variable);

		/// <summary>
		/// Assign a ValueExpression to an EL variable, replacing any previously assignment to the same
		/// variable. The assignment for the variable is removed if the expression is null.
		/// </summary>
		/// <param name="variable">
		///            The variable name </param>
		/// <param name="expression">
		///            The ValueExpression to be assigned to the variable. </param>
		/// <returns> The previous ValueExpression assigned to this variable, null if there is no previous
		///         assignment to this variable. </returns>
		public abstract ValueExpression setVariable(string variable, ValueExpression expression);
	}

}