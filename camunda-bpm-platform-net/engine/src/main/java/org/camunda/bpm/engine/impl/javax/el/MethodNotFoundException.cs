using System;

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
	/// Thrown when a method could not be found while evaluating a <seealso cref="MethodExpression"/>.
	/// </summary>
	public class MethodNotFoundException : ELException
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Creates a MethodNotFoundException with no detail message.
		/// </summary>
		public MethodNotFoundException() : base()
		{
		}

		/// <summary>
		/// Creates a MethodNotFoundException with the provided detail message.
		/// </summary>
		/// <param name="message">
		///            the detail message </param>
		public MethodNotFoundException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a MethodNotFoundException with the given root cause.
		/// </summary>
		/// <param name="cause">
		///            the originating cause of this exception </param>
		public MethodNotFoundException(Exception cause) : base(cause)
		{
		}

		/// <summary>
		/// Creates a MethodNotFoundException with the given detail message and root cause.
		/// </summary>
		/// <param name="message">
		///            the detail message </param>
		/// <param name="cause">
		///            the originating cause of this exception </param>
		public MethodNotFoundException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}