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
	/// An event which indicates that an <seealso cref="ELContext"/> has been created. The source object is the
	/// ELContext that was created.
	/// </summary>
	public class ELContextEvent : EventObject
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Constructs an ELContextEvent object to indicate that an ELContext has been created.
		/// </summary>
		/// <param name="source">
		///            the ELContext that was created. </param>
		public ELContextEvent(ELContext source) : base(source)
		{
		}

		/// <summary>
		/// Returns the ELContext that was created. This is a type-safe equivalent of the
		/// java.util.EventObject.getSource() method.
		/// </summary>
		/// <returns> the ELContext that was created. </returns>
		public virtual ELContext ELContext
		{
			get
			{
				return (ELContext) Source;
			}
		}
	}

}