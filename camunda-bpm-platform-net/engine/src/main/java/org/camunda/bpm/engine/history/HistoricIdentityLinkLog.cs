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
namespace org.camunda.bpm.engine.history
{
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;


	/// <summary>
	/// An historic identity link stores the association of a task with a certain identity.
	/// 
	/// For example, historic identity link is logged on the following conditions:
	/// - a user can be an assignee/Candidate/Owner (= identity link type) for a task
	/// - a group can be a candidate-group (= identity link type) for a task
	/// - a user can be an candidate in the scope of process definition
	/// - a group can be a candidate-group in the scope of process definition
	/// 
	/// For every log, an operation type (add/delete) is added to the database
	/// based on the identity link operation
	/// </summary>
	public interface HistoricIdentityLinkLog
	{

	  /// <summary>
	  /// Returns the id of historic identity link (Candidate or Assignee or Owner).
	  /// </summary>
	  string Id {get;}
	  /// <summary>
	  /// Returns the type of link (Candidate or Assignee or Owner).
	  /// See <seealso cref="IdentityLinkType"/> for the native supported types by the process engine.
	  /// 
	  /// 
	  /// </summary>
	  string Type {get;}

	  /// <summary>
	  /// If the identity link involves a user, then this will be a non-null id of a user.
	  /// That userId can be used to query for user information through the <seealso cref="UserQuery"/> API.
	  /// </summary>
	  string UserId {get;}

	  /// <summary>
	  /// If the identity link involves a group, then this will be a non-null id of a group.
	  /// That groupId can be used to query for user information through the <seealso cref="GroupQuery"/> API.
	  /// </summary>
	  string GroupId {get;}

	  /// <summary>
	  /// The id of the task associated with this identity link.
	  /// </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// Returns the userId of the user who assigns a task to the user
	  /// 
	  /// </summary>
	  string AssignerId {get;}

	  /// <summary>
	  /// Returns the type of identity link history (add or delete identity link)
	  /// </summary>
	  string OperationType {get;}

	  /// <summary>
	  /// Returns the time of identity link event (Creation/Deletion)
	  /// </summary>
	  DateTime Time {get;}

	  /// <summary>
	  /// Returns the id of the related process definition 
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Returns the key of the related process definition 
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Returns the id of the related tenant 
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// Returns the root process instance id of
	  /// the related process instance
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The time the historic identity link log will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}