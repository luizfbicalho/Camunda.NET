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
	/// this annotation instructs the component model to start an Activiti business process on
	/// sucessful invocation of a method that's annotated with it.
	/// 
	/// @author Josh Long
	/// @since 1.0
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Target({ElementType.METHOD}) @Retention(RetentionPolicy.RUNTIME) @Documented public class StartProcess extends System.Attribute
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class StartProcess : System.Attribute
	{
		/// <summary>
		/// the name of the business process to start (by key)
		/// </summary>
		internal string processKey;

		/// <summary>
		/// returns the ID of the <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance"/>. If specified, it'll only work if
		/// the return type of the invocation is compatabile with a <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance"/>'s ID
		/// (which is a String, at the moment)
		/// </summary>
		/// <returns>  whether to return the process instance ID </returns>
		internal bool returnProcessInstanceId;





		public StartProcess(String processKey, boolean returnProcessInstanceId = false)
		{
			this.processKey = processKey;
			this.returnProcessInstanceId = returnProcessInstanceId;
		}
	}

}