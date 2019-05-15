/*
 * Copyright 2011 the original author or authors
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
namespace org.camunda.bpm.engine.spring.annotations
{

	/// <summary>
	/// indicates that a method is to be enlisted as a handler for a given BPMN state
	/// 
	/// @author Josh Long
	/// @since 1.0
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Target({ElementType.METHOD}) @Retention(RetentionPolicy.RUNTIME) @Documented public class State extends System.Attribute
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class State : System.Attribute
	{

		/// <summary>
		/// the business process name
		/// </summary>
		internal string process;

		/// <summary>
		/// the state that the component responds to,
		/// </summary>
		internal string state;

		/// <summary>
		/// by default, this will be the #stateName
		/// </summary>
		internal string value;

		public State(String process = "", String state = "", String value = "")
		{
			this.process = process;
			this.state = state;
			this.value = value;
		}
	}

}