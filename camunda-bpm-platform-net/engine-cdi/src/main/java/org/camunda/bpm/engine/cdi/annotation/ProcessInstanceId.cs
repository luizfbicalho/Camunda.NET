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
namespace org.camunda.bpm.engine.cdi.annotation
{

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// Qualifier annotation for injecting the id of the current process instance.
	/// <p />
	/// Example:
	/// 
	/// <pre>
	/// {@code @Inject} @ProcessInstanceId String pid;
	/// </pre>
	/// <p />
	/// Note that the <seealso cref="ProcessInstance"/> is also available for injection:
	/// 
	/// <pre>
	/// {@code @Inject} ProcessInstance pi;
	/// </pre>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Qualifier @Retention(RetentionPolicy.RUNTIME) @Target({ ElementType.FIELD, ElementType.PARAMETER, ElementType.METHOD, ElementType.TYPE }) public class ProcessInstanceId extends System.Attribute
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ProcessInstanceId : System.Attribute
	{

	}

}