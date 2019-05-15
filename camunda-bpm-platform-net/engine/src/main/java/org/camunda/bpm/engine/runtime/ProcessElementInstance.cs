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
	/// <para>A ProcessElementInstance is an instance of a process construct 
	/// such as an Activity (see <seealso cref="ActivityInstance"/>) or a transition 
	/// (see <seealso cref="TransitionInstance"/>).
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public interface ProcessElementInstance
	{

	  /// <summary>
	  /// The id of the process element instance </summary>
	  string Id {get;}

	  /// <summary>
	  /// The id of the parent activity instance. </summary>
	  string ParentActivityInstanceId {get;}

	  /// <summary>
	  /// the process definition id </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// the id of the process instance this process element is part of </summary>
	  string ProcessInstanceId {get;}

	}

}