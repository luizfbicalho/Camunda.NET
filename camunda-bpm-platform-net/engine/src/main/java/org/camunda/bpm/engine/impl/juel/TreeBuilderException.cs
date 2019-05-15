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
namespace org.camunda.bpm.engine.impl.juel
{
	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;


	/// <summary>
	/// Exception type thrown in build phase (scan/parse).
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class TreeBuilderException : ELException
	{
		private const long serialVersionUID = 1L;

		private readonly string expression;
		private readonly int position;
		private readonly string encountered;
		private readonly string expected;

		public TreeBuilderException(string expression, int position, string encountered, string expected, string message) : base(LocalMessages.get("error.build", expression, message))
		{
			this.expression = expression;
			this.position = position;
			this.encountered = encountered;
			this.expected = expected;
		}

		/// <returns> the expression string </returns>
		public virtual string Expression
		{
			get
			{
				return expression;
			}
		}

		/// <returns> the error position </returns>
		public virtual int Position
		{
			get
			{
				return position;
			}
		}

		/// <returns> the substring (or description) that has been encountered </returns>
		public virtual string Encountered
		{
			get
			{
				return encountered;
			}
		}

		/// <returns> the substring (or description) that was expected </returns>
		public virtual string Expected
		{
			get
			{
				return expected;
			}
		}
	}

}