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
	/// The interface to a map between EL function names and methods. A FunctionMapper maps
	/// ${prefix:name()} style functions to a static method that can execute that function.
	/// </summary>
	public abstract class FunctionMapper
	{
		/// <summary>
		/// Resolves the specified prefix and local name into a java.lang.Method. Returns null if no
		/// function could be found that matches the given prefix and local name.
		/// </summary>
		/// <param name="prefix">
		///            the prefix of the function, or "" if no prefix. For example, "fn" in
		///            ${fn:method()}, or "" in ${method()}. </param>
		/// <param name="localName">
		///            the short name of the function. For example, "method" in ${fn:method()}. </param>
		/// <returns> the static method to invoke, or null if no match was found. </returns>
		public abstract System.Reflection.MethodInfo resolveFunction(string prefix, string localName);
	}

}