using System;

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
namespace org.camunda.bpm.engine.task
{


	/// <summary>
	/// User comments that form discussions around tasks.
	/// </summary>
	/// <seealso cref= {@link TaskService#getTaskComments(String)
	/// @author Tom Baeyens </seealso>
	public interface Comment
	{

	  /// <summary>
	  /// comment id </summary>
	  string Id {get;}

	  /// <summary>
	  /// reference to the user that made the comment </summary>
	  string UserId {get;}

	  /// <summary>
	  /// time and date when the user made the comment </summary>
	  DateTime Time {get;}

	  /// <summary>
	  /// reference to the task on which this comment was made </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// reference to the root process instance id of the process instance on which this comment was made </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// reference to the process instance on which this comment was made </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// the full comment message the user had related to the task and/or process instance </summary>
	  /// <seealso cref= TaskService#getTaskComments(String)  </seealso>
	  string FullMessage {get;}

	  /// <summary>
	  /// The time the historic comment will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}