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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// Represents one execution of a  <seealso cref="ProcessDefinition"/>.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public interface ProcessInstance : Execution
	{

	  /// <summary>
	  /// The id of the process definition of the process instance.
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The business key of this process instance.
	  /// </summary>
	  string BusinessKey {get;}

	  /// <summary>
	  /// The id of the root process instance associated with this process instance.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The id of the case instance associated with this process instance.
	  /// </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// returns true if the process instance is suspended
	  /// </summary>
	  bool Suspended {get;}

	}

}