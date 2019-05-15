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
	/// Holds information about a method that a <seealso cref="MethodExpression"/> evaluated to.
	/// </summary>
	public class MethodInfo
	{
		private readonly string name;
		private readonly Type returnType;
		private readonly Type[] paramTypes;

		/// <summary>
		/// Creates a new instance of MethodInfo with the given information.
		/// </summary>
		/// <param name="name">
		///            The name of the method </param>
		/// <param name="returnType">
		///            The return type of the method </param>
		/// <param name="paramTypes">
		///            The types of each of the method's parameters </param>
		public MethodInfo(string name, Type returnType, Type[] paramTypes)
		{
			this.name = name;
			this.returnType = returnType;
			this.paramTypes = paramTypes;
		}

		/// <summary>
		/// Returns the name of the method
		/// </summary>
		/// <returns> the name of the method </returns>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Returns the parameter types of the method
		/// </summary>
		/// <returns> the parameter types of the method </returns>
		public virtual Type[] ParamTypes
		{
			get
			{
				return paramTypes;
			}
		}

		/// <summary>
		/// Returns the return type of the method
		/// </summary>
		/// <returns> the return type of the method </returns>
		public virtual Type ReturnType
		{
			get
			{
				return returnType;
			}
		}
	}

}