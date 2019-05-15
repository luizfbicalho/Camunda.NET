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
	/// <summary>
	/// Function node interface.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public interface FunctionNode : Node
	{
		/// <summary>
		/// Get the full function name
		/// </summary>
		string Name {get;}

		/// <summary>
		/// Get the unique index of this identifier in the expression (e.g. preorder index)
		/// </summary>
		int Index {get;}

		/// <summary>
		/// Get the number of parameters for this function
		/// </summary>
		int ParamCount {get;}

		/// <returns> <code>true</code> if this node supports varargs. </returns>
		bool VarArgs {get;}
	}

}