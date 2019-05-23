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
namespace org.camunda.bpm.engine.runtime
{


	/// <summary>
	/// Represent a 'path of execution' in a process instance.
	/// 
	/// Note that a <seealso cref="ProcessInstance"/> also is an execution.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface Execution
	{

	  /// <summary>
	  /// The unique identifier of the execution.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// Indicates if the execution is suspended.
	  /// </summary>
	  bool Suspended {get;}

	  /// <summary>
	  /// Indicates if the execution is ended.
	  /// </summary>
	  bool Ended {get;}

	  /// <summary>
	  /// Id of the root of the execution tree representing the process instance.
	  /// It is the same as <seealso cref="getId()"/> if this execution is the process instance. 
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// The id of the tenant this execution belongs to. Can be <code>null</code>
	  /// if the execution belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	}

}