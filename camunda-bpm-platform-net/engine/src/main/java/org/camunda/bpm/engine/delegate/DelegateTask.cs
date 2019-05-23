using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.@delegate
{

	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;

	/// <summary>
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
	public interface DelegateTask : VariableScope, BpmnModelExecutionContext, ProcessEngineServicesAware
	{

	  /// <summary>
	  /// DB id of the task. </summary>
	  string Id {get;}

	  /// <summary>
	  /// Name or title of the task. </summary>
	  string Name {get;set;}


	  /// <summary>
	  /// Free text description of the task. </summary>
	  string Description {get;set;}


	  /// <summary>
	  /// indication of how important/urgent this task is with a number between
	  /// 0 and 100 where higher values mean a higher priority and lower values mean
	  /// lower priority: [0..19] lowest, [20..39] low, [40..59] normal, [60..79] high
	  /// [80..100] highest 
	  /// </summary>
	  int Priority {get;set;}


	  /// <summary>
	  /// Reference to the process instance or null if it is not related to a process instance. </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Reference to the path of execution or null if it is not related to a process instance. </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Reference to the process definition or null if it is not related to a process. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Reference to the case instance or null if it is not related to a case instance. </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// Reference to the case execution or null if it is not related to a case instance. </summary>
	  string CaseExecutionId {get;}

	  /// <summary>
	  /// Reference to the case definition or null if it is not related to a case. </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// The date/time when this task was created </summary>
	  DateTime CreateTime {get;}

	  /// <summary>
	  /// The id of the activity in the process defining this task or null if this is not related to a process </summary>
	  string TaskDefinitionKey {get;}

	  /// <summary>
	  /// Returns the execution currently at the task. </summary>
	  DelegateExecution Execution {get;}

	  /// <summary>
	  /// Returns the case execution currently at the task. </summary>
	  DelegateCaseExecution CaseExecution {get;}

	  /// <summary>
	  /// Returns the event name which triggered the task listener to fire for this task. </summary>
	  string EventName {get;}

	  /// <summary>
	  /// Adds the given user as a candidate user to this task. </summary>
	  void addCandidateUser(string userId);

	  /// <summary>
	  /// Adds multiple users as candidate user to this task. </summary>
	  void addCandidateUsers(ICollection<string> candidateUsers);

	  /// <summary>
	  /// Adds the given group as candidate group to this task </summary>
	  void addCandidateGroup(string groupId);

	  /// <summary>
	  /// Adds multiple groups as candidate group to this task. </summary>
	  void addCandidateGroups(ICollection<string> candidateGroups);

	  /// <summary>
	  /// The <seealso cref="User.getId() userId"/> of the person responsible for this task. </summary>
	  string Owner {get;set;}


	  /// <summary>
	  /// The <seealso cref="User.getId() userId"/> of the person to which this task is delegated. </summary>
	  string Assignee {get;set;}


	  /// <summary>
	  /// Due date of the task. </summary>
	  DateTime DueDate {get;set;}


	  /// <summary>
	  /// Get delete reason of the task. </summary>
	  string DeleteReason {get;}

	  /// <summary>
	  /// Involves a user with a task. The type of identity link is defined by the given identityLinkType. </summary>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ProcessEngineException"> when the task or user doesn't exist. </exception>
	  void addUserIdentityLink(string userId, string identityLinkType);

	  /// <summary>
	  /// Involves a group with group task. The type of identityLink is defined by the given identityLink. </summary>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ProcessEngineException"> when the task or group doesn't exist. </exception>
	  void addGroupIdentityLink(string groupId, string identityLinkType);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="deleteUserIdentityLink(string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/> </summary>
	  /// <param name="userId"> id of the user to use as candidate, cannot be null. </param>
	  /// <exception cref="ProcessEngineException"> when the task or user doesn't exist. </exception>
	  void deleteCandidateUser(string userId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="deleteGroupIdentityLink(string, string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/> </summary>
	  /// <param name="groupId"> id of the group to use as candidate, cannot be null. </param>
	  /// <exception cref="ProcessEngineException"> when the task or group doesn't exist. </exception>
	  void deleteCandidateGroup(string groupId);

	  /// <summary>
	  /// Removes the association between a user and a task for the given identityLinkType. </summary>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ProcessEngineException"> when the task or user doesn't exist. </exception>
	  void deleteUserIdentityLink(string userId, string identityLinkType);

	  /// <summary>
	  /// Removes the association between a group and a task for the given identityLinkType. </summary>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ProcessEngineException"> when the task or group doesn't exist. </exception>
	  void deleteGroupIdentityLink(string groupId, string identityLinkType);

	  /// <summary>
	  /// Retrieves the candidate users and groups associated with the task. </summary>
	  /// <returns> set of <seealso cref="IdentityLink"/>s of type <seealso cref="IdentityLinkType.CANDIDATE"/>. </returns>
	  ISet<IdentityLink> Candidates {get;}

	  /// <summary>
	  /// Provides access to the current <seealso cref="UserTask"/> Element from the Bpmn Model. </summary>
	  /// <returns> the current <seealso cref="UserTask"/> Element from the Bpmn Model. </returns>
	  UserTask BpmnModelElementInstance {get;}

	  /// <summary>
	  /// Return the id of the tenant this task belongs to. Can be <code>null</code>
	  /// if the task belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// set status to complete.
	  /// </summary>
	  /// <exception cref="IllegalStateException"> if performed on completion or deletion </exception>
	  void complete();
	}

}