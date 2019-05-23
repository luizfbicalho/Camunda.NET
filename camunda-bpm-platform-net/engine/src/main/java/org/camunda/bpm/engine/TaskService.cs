using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using TaskPermissions = org.camunda.bpm.engine.authorization.TaskPermissions;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Event = org.camunda.bpm.engine.task.Event;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using NativeTaskQuery = org.camunda.bpm.engine.task.NativeTaskQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using TaskReport = org.camunda.bpm.engine.task.TaskReport;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Service which provides access to <seealso cref="Task"/> and form related operations.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Thorben Lindhauer
	/// </summary>
	public interface TaskService
	{

		/// <summary>
		/// Creates a new task that is not related to any process instance.
		/// 
		/// The returned task is transient and must be saved with <seealso cref="saveTask(Task)"/> 'manually'.
		/// </summary>
		/// <exception cref="AuthorizationException">
		///          if the user has no <seealso cref="Permissions.CREATE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  Task newTask();

	  /// <summary>
	  /// create a new task with a user defined task id </summary>
	  Task newTask(string taskId);

		/// <summary>
		/// Saves the given task to the persistent data store. If the task is already
		/// present in the persistent store, it is updated.
		/// After a new task has been saved, the task instance passed into this method
		/// is updated with the id of the newly created task.
		/// </summary>
		/// <param name="task"> the task, cannot be null.
		/// </param>
		/// <exception cref="AuthorizationException">
		///          If the task is already present and the user has no <seealso cref="Permissions.UPDATE"/> permission
		///          on <seealso cref="Resources.TASK"/> or no <seealso cref="Permissions.UPDATE_TASK"/> permission on
		///          <seealso cref="Resources.PROCESS_DEFINITION"/>.
		///          Or if the task is not present and the user has no <seealso cref="Permissions.CREATE"/> permission
		///          on <seealso cref="Resources.TASK"/>. </exception>
		void saveTask(Task task);

		/// <summary>
		/// Deletes the given task, not deleting historic information that is related to this task.
		/// </summary>
		/// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
		/// exists with the given taskId, the operation is ignored.
		/// </param>
		/// <exception cref="ProcessEngineException">
		///          when an error occurs while deleting the task or in case the task is part
		///          of a running process or case instance. </exception>
		/// <exception cref="AuthorizationException">
		///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
		void deleteTask(string taskId);

		/// <summary>
		/// Deletes all tasks of the given collection, not deleting historic information that is related
		/// to these tasks.
		/// </summary>
		/// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
		/// id's in the list that don't have an existing task will be ignored.
		/// </param>
		/// <exception cref="ProcessEngineException">
		///          when an error occurs while deleting the tasks or in case one of the tasks
		///          is part of a running process or case instance. </exception>
		/// <exception cref="AuthorizationException">
		///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
		void deleteTasks(ICollection<string> taskIds);

	  /// <summary>
	  /// Deletes the given task.
	  /// </summary>
	  /// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
	  /// exists with the given taskId, the operation is ignored.
	  /// </param>
	  /// <param name="cascade"> If cascade is true, also the historic information related to this task is deleted.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when an error occurs while deleting the task or in case the task is part
	  ///          of a running process or case instance. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  void deleteTask(string taskId, bool cascade);

	  /// <summary>
	  /// Deletes all tasks of the given collection.
	  /// </summary>
	  /// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
	  /// id's in the list that don't have an existing task will be ignored. </param>
	  /// <param name="cascade"> If cascade is true, also the historic information related to this task is deleted.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when an error occurs while deleting the tasks or in case one of the tasks
	  ///          is part of a running process or case instance. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  void deleteTasks(ICollection<string> taskIds, bool cascade);

	  /// <summary>
	  /// Deletes the given task, not deleting historic information that is related to this task.
	  /// </summary>
	  /// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
	  /// exists with the given taskId, the operation is ignored. </param>
	  /// <param name="deleteReason"> reason the task is deleted. Is recorded in history, if enabled.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when an error occurs while deleting the task or in case the task is part
	  ///          of a running process or case instance. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  void deleteTask(string taskId, string deleteReason);

	  /// <summary>
	  /// Deletes all tasks of the given collection, not deleting historic information that is related to these tasks.
	  /// </summary>
	  /// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
	  /// id's in the list that don't have an existing task will be ignored. </param>
	  /// <param name="deleteReason"> reason the task is deleted. Is recorded in history, if enabled.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when an error occurs while deleting the tasks or in case one of the tasks
	  ///          is part of a running process or case instance. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  void deleteTasks(ICollection<string> taskIds, string deleteReason);

	  /// <summary>
	  /// Claim responsibility for a task:
	  /// the given user is made <seealso cref="Task.getAssignee() assignee"/> for the task.
	  /// The difference with <seealso cref="setAssignee(string, string)"/> is that here
	  /// a check is done if the task already has a user assigned to it.
	  /// No check is done whether the user is known by the identity component.
	  /// </summary>
	  /// <param name="taskId"> task to claim, cannot be null. </param>
	  /// <param name="userId"> user that claims the task. When userId is null the task is unclaimed,
	  /// assigned to no one.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist or when the task is already claimed by another user. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void claim(string taskId, string userId);

	  /// <summary>
	  /// Marks a task as done and continues process execution.
	  /// 
	  /// This method is typically called by a task list user interface
	  /// after a task form has been submitted by the
	  /// <seealso cref="Task.getAssignee() assignee"/>.
	  /// </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id or when this task is <seealso cref="DelegationState.PENDING"/> delegation. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void complete(string taskId);

	  /// <summary>
	  /// Delegates the task to another user.
	  /// 
	  /// This means that the <seealso cref="Task.getAssignee() assignee"/> is set
	  /// and the <seealso cref="Task.getDelegationState() delegation state"/> is set to
	  /// <seealso cref="DelegationState.PENDING"/>.
	  /// If no owner is set on the task, the owner is set to the current
	  /// <seealso cref="Task.getAssignee() assignee"/> of the task.
	  /// The new assignee must use <seealso cref="TaskService.resolveTask(string)"/>
	  /// to report back to the owner.
	  /// Only the owner can <seealso cref="TaskService.complete(string) complete"/> the task.
	  /// </summary>
	  /// <param name="taskId"> The id of the task that will be delegated. </param>
	  /// <param name="userId"> The id of the user that will be set as assignee.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void delegateTask(string taskId, string userId);

	  /// <summary>
	  /// Marks that the <seealso cref="Task.getAssignee() assignee"/> is done with the task
	  /// <seealso cref="TaskService.delegateTask(string, string) delegated"/>
	  /// to her and that it can be sent back to the <seealso cref="Task.getOwner() owner"/>.
	  /// Can only be called when this task is <seealso cref="DelegationState.PENDING"/> delegation.
	  /// After this method returns, the <seealso cref="Task.getDelegationState() delegation state"/>
	  /// is set to <seealso cref="DelegationState.RESOLVED"/> and the task can be
	  /// <seealso cref="TaskService.complete(string) completed"/>.
	  /// </summary>
	  /// <param name="taskId"> the id of the task to resolve, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void resolveTask(string taskId);

	  /// <summary>
	  /// Marks that the <seealso cref="Task.getAssignee() assignee"/> is done with the task
	  /// <seealso cref="TaskService.delegateTask(string, string) delegated"/>
	  /// to her and that it can be sent back to the <seealso cref="Task.getOwner() owner"/>
	  /// with the provided variables.
	  /// Can only be called when this task is <seealso cref="DelegationState.PENDING"/> delegation.
	  /// After this method returns, the <seealso cref="Task.getDelegationState() delegation state"/>
	  /// is set to <seealso cref="DelegationState.RESOLVED"/> and the task can be
	  /// <seealso cref="TaskService.complete(string) completed"/>.
	  /// </summary>
	  /// <param name="taskId"> </param>
	  /// <param name="variables">
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void resolveTask(string taskId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Marks a task as done and continues process execution.
	  /// 
	  /// This method is typically called by a task list user interface
	  /// after a task form has been submitted by the
	  /// <seealso cref="Task.getAssignee() assignee"/>
	  /// and the required task parameters have been provided.
	  /// </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
	  /// <param name="variables"> task parameters. May be null or empty.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void complete(string taskId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Marks a task as done and continues process execution.
	  /// 
	  /// This method is typically called by a task list user interface
	  /// after a task form has been submitted by the
	  /// <seealso cref="Task.getAssignee() assignee"/>
	  /// and the required task parameters have been provided.
	  /// </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
	  /// <param name="variables"> task parameters. May be null or empty. </param>
	  /// <param name="deserializeValues"> if false, returned <seealso cref="SerializableValue"/>s
	  ///   will not be deserialized (unless they are passed into this method as a
	  ///   deserialized value or if the BPMN process triggers deserialization)
	  /// </param>
	  /// <returns> All task variables with their current value
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  VariableMap completeWithVariablesInReturn(string taskId, IDictionary<string, object> variables, bool deserializeValues);

	  /// <summary>
	  /// Changes the assignee of the given task to the given userId.
	  /// No check is done whether the user is known by the identity component.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as assignee.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void setAssignee(string taskId, string userId);

	  /// <summary>
	  /// Transfers ownership of this task to another user.
	  /// No check is done whether the user is known by the identity component.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> of the person that is receiving ownership.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void setOwner(string taskId, string userId);

	  /// <summary>
	  /// Retrieves the <seealso cref="IdentityLink"/>s associated with the given task.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is associated with a certain task (eg. as candidate, assignee, etc.)
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  IList<IdentityLink> getIdentityLinksForTask(string taskId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="addUserIdentityLink(string, string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/>
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as candidate, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void addCandidateUser(string taskId, string userId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="addGroupIdentityLink(string, string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/>
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to use as candidate, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or group doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void addCandidateGroup(string taskId, string groupId);

	  /// <summary>
	  /// Involves a user with a task. The type of identity link is defined by the
	  /// given identityLinkType.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>).
	  /// </seealso>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void addUserIdentityLink(string taskId, string userId, string identityLinkType);

	  /// <summary>
	  /// Involves a group with a task. The type of identityLink is defined by the
	  /// given identityLink.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>).
	  /// </seealso>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or group doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void addGroupIdentityLink(string taskId, string groupId, string identityLinkType);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="deleteUserIdentityLink(string, string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/>
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as candidate, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void deleteCandidateUser(string taskId, string userId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="deleteGroupIdentityLink(string, string, string)"/>; with type <seealso cref="IdentityLinkType.CANDIDATE"/>
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to use as candidate, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or group doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void deleteCandidateGroup(string taskId, string groupId);

	  /// <summary>
	  /// Removes the association between a user and a task for the given identityLinkType.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>).
	  /// </seealso>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or user doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void deleteUserIdentityLink(string taskId, string userId, string identityLinkType);

	  /// <summary>
	  /// Removes the association between a group and a task for the given identityLinkType.
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>).
	  /// </seealso>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task or group doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void deleteGroupIdentityLink(string taskId, string groupId, string identityLinkType);

	  /// <summary>
	  /// Changes the priority of the task.
	  /// 
	  /// Authorization: actual owner / business admin
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="priority"> the new priority for the task.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          (if the task is part of a running process instance). </exception>
	  void setPriority(string taskId, int priority);

	  /// <summary>
	  /// Returns a new <seealso cref="TaskQuery"/> that can be used to dynamically query tasks.
	  /// </summary>
	  TaskQuery createTaskQuery();

	  /// <summary>
	  /// Returns a new
	  /// </summary>
	  NativeTaskQuery createNativeTaskQuery();

	  /// <summary>
	  /// Set variable on a task. If the variable is not already existing, it will be created in the
	  /// most outer scope.  This means the process instance in case this task is related to an
	  /// execution.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void setVariable(string taskId, string variableName, object value);

	  /// <summary>
	  /// Set variables on a task. If the variable is not already existing, it will be created in the
	  /// most outer scope.  This means the process instance in case this task is related to an
	  /// execution.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void setVariables<T1>(string taskId, IDictionary<T1> variables);

	  /// <summary>
	  /// Set variable on a task. If the variable is not already existing, it will be created in the
	  /// task.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void setVariableLocal(string taskId, string variableName, object value);

	  /// <summary>
	  /// Set variables on a task. If the variable is not already existing, it will be created in the
	  /// task.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void setVariablesLocal<T1>(string taskId, IDictionary<T1> variables);

	  /// <summary>
	  /// Get a variables and search in the task scope and if available also the execution scopes.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para> </exception>
	  object getVariable(string taskId, string variableName);

	  /// <summary>
	  /// Get a variables and search in the task scope and if available also the execution scopes.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableName"> the name of the variable to fetch
	  /// </param>
	  /// <returns> the TypedValue for the variable or 'null' in case no such variable exists.
	  /// </returns>
	  /// <exception cref="ClassCastException">
	  ///          in case the value is not of the requested type </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  T getVariableTyped<T>(string taskId, string variableName);

	  /// <summary>
	  /// Get a variables and search in the task scope and if available also the execution scopes.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableName"> the name of the variable to fetch </param>
	  /// <param name="deserializeValue"> if false a, <seealso cref="SerializableValue"/> will not be deserialized.
	  /// </param>
	  /// <returns> the TypedValue for the variable or 'null' in case no such variable exists.
	  /// </returns>
	  /// <exception cref="ClassCastException">
	  ///          in case the value is not of the requested type </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  T getVariableTyped<T>(string taskId, string variableName, bool deserializeValue);

	  /// <summary>
	  /// Get a variables and only search in the task scope.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para> </exception>
	  object getVariableLocal(string taskId, string variableName);

	  /// <summary>
	  /// Get a variables and only search in the task scope.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableName"> the name of the variable to fetch
	  /// </param>
	  /// <returns> the TypedValue for the variable or 'null' in case no such variable exists.
	  /// </returns>
	  /// <exception cref="ClassCastException">
	  ///          in case the value is not of the requested type </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  T getVariableLocalTyped<T>(string taskId, string variableName);

	  /// <summary>
	  /// Get a variables and only search in the task scope.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableName"> the name of the variable to fetch </param>
	  /// <param name="deserializeValue"> if false a, <seealso cref="SerializableValue"/> will not be deserialized.
	  /// </param>
	  /// <returns> the TypedValue for the variable or 'null' in case no such variable exists.
	  /// </returns>
	  /// <exception cref="ClassCastException">
	  ///          in case the value is not of the requested type </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  T getVariableLocalTyped<T>(string taskId, string variableName, bool deserializeValue);

	  /// <summary>
	  /// Get all variables and search in the task scope and if available also the execution scopes.
	  /// If you have many variables and you only need a few, consider using <seealso cref="getVariables(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para> </exception>
	  IDictionary<string, object> getVariables(string taskId);

	  /// <summary>
	  /// Get all variables and search in the task scope and if available also the execution scopes.
	  /// If you have many variables and you only need a few, consider using <seealso cref="getVariables(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="taskId"> the id of the task
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// @since 7.2 </exception>
	  VariableMap getVariablesTyped(string taskId);

	  /// <summary>
	  /// Get all variables and search in the task scope and if available also the execution scopes.
	  /// If you have many variables and you only need a few, consider using <seealso cref="getVariables(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue SerializableValues"/> will not be deserialized.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  VariableMap getVariablesTyped(string taskId, bool deserializeValues);

	  /// <summary>
	  /// Get all variables and search only in the task scope.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="getVariablesLocal(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para> </exception>
	  IDictionary<string, object> getVariablesLocal(string taskId);

	  /// <summary>
	  /// Get all variables and search only in the task scope.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="getVariablesLocal(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue SerializableValues"/> will not be deserialized.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  VariableMap getVariablesLocalTyped(string taskId);

	  /// <summary>
	  /// Get all variables and search only in the task scope.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="getVariablesLocal(string, System.Collections.ICollection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue SerializableValues"/> will not be deserialized.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  VariableMap getVariablesLocalTyped(string taskId, bool deserializeValues);

	  /// <summary>
	  /// Get values for all given variableNames
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  ///  </exception>
	  IDictionary<string, object> getVariables(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Get values for all given variableName
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableNames"> only fetch variables whose names are in the collection. </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue SerializableValues"/> will not be deserialized.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  VariableMap getVariablesTyped(string taskId, ICollection<string> variableNames, bool deserializeValues);

	  /// <summary>
	  /// Get a variable on a task
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  ///  </exception>
	  IDictionary<string, object> getVariablesLocal(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Get values for all given variableName. Only search in the local task scope.
	  /// </summary>
	  /// <param name="taskId"> the id of the task </param>
	  /// <param name="variableNames"> only fetch variables whose names are in the collection. </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue SerializableValues"/> will not be deserialized.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> and
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>in case <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> configuration is enabled and
	  ///          the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> and
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li></para>
	  /// 
	  /// @since 7.2 </exception>
	  VariableMap getVariablesLocalTyped(string taskId, ICollection<string> variableNames, bool deserializeValues);

	  /// <summary>
	  /// Removes the variable from the task.
	  /// When the variable does not exist, nothing happens.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void removeVariable(string taskId, string variableName);

	  /// <summary>
	  /// Removes the variable from the task (not considering parent scopes).
	  /// When the variable does not exist, nothing happens.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void removeVariableLocal(string taskId, string variableName);

	  /// <summary>
	  /// Removes all variables in the given collection from the task.
	  /// Non existing variable names are simply ignored.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void removeVariables(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Removes all variables in the given collection from the task (not considering parent scopes).
	  /// Non existing variable names are simply ignored.
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          when the task doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has none of the following:
	  ///           <li><seealso cref="TaskPermissions.UPDATE_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/></li>
	  ///           <li>or if the task is part of a running process instance:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///           </ul> </exception>
	  void removeVariablesLocal(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Add a comment to a task and/or process instance.
	  /// </summary>
	  /// @deprecated Use <seealso cref="createComment(string, string, string)"/> instead 
	  [Obsolete("Use <seealso cref=\"createComment(string, string, string)\"/> instead")]
	  void addComment(string taskId, string processInstanceId, string message);

	  /// <summary>
	  /// Creates a comment to a task and/or process instance and returns the comment. </summary>
	  Comment createComment(string taskId, string processInstanceId, string message);

	  /// <summary>
	  /// The comments related to the given task. </summary>
	  IList<Comment> getTaskComments(string taskId);

	  /// <summary>
	  /// Retrieve a particular task comment </summary>
	  Comment getTaskComment(string taskId, string commentId);

	  /// <summary>
	  /// <para>The all events related to the given task.</para>
	  /// 
	  /// <para>As of Camunda BPM 7.4 task events are only written in context of a logged in
	  /// user. This behavior can be toggled in the process engine configuration using the
	  /// property <code>legacyUserOperationLog</code> (default false). To restore the engine's
	  /// previous behavior, set the flag to <code>true</code>.</para>
	  /// </summary>
	  /// @deprecated This method has been deprecated as of camunda BPM 7.1. It has been replaced with
	  /// the operation log. See <seealso cref="UserOperationLogEntry"/> and <seealso cref="UserOperationLogQuery"/>.
	  /// 
	  /// <seealso cref= HistoryService#createUserOperationLogQuery() </seealso>
	  [Obsolete("This method has been deprecated as of camunda BPM 7.1. It has been replaced with")]
	  IList<Event> getTaskEvents(string taskId);

	  /// <summary>
	  /// The comments related to the given process instance. </summary>
	  IList<Comment> getProcessInstanceComments(string processInstanceId);

	  /// <summary>
	  /// Add a new attachment to a task and/or a process instance and use an input stream to provide the content
	  /// please use method in runtime service to operate on process instance.
	  /// 
	  /// Either taskId or processInstanceId has to be provided
	  /// </summary>
	  /// <param name="taskId"> - task that should have an attachment </param>
	  /// <param name="processInstanceId"> - id of a process to use if task id is null </param>
	  /// <param name="attachmentType"> - name of the attachment, can be null </param>
	  /// <param name="attachmentName"> - name of the attachment, can be null </param>
	  /// <param name="attachmentDescription">  - full text description, can be null </param>
	  /// <param name="content"> - byte array with content of attachment
	  ///  </param>
	  Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, Stream content);

	  /// <summary>
	  /// Add a new attachment to a task and/or a process instance and use an url as the content
	  /// please use method in runtime service to operate on process instance
	  /// 
	  /// Either taskId or processInstanceId has to be provided
	  /// </summary>
	  /// <param name="taskId"> - task that should have an attachment </param>
	  /// <param name="processInstanceId"> - id of a process to use if task id is null </param>
	  /// <param name="attachmentType"> - name of the attachment, can be null </param>
	  /// <param name="attachmentName"> - name of the attachment, can be null </param>
	  /// <param name="attachmentDescription">  - full text description, can be null </param>
	  /// <param name="url"> - url of the attachment, can be null
	  ///  </param>
	  Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, string url);

	  /// <summary>
	  /// Update the name and decription of an attachment </summary>
	  void saveAttachment(Attachment attachment);

	  /// <summary>
	  /// Retrieve a particular attachment </summary>
	  Attachment getAttachment(string attachmentId);

	  /// <summary>
	  /// Retrieve a particular attachment to the given task id and attachment id </summary>
	  Attachment getTaskAttachment(string taskId, string attachmentId);

	  /// <summary>
	  /// Retrieve stream content of a particular attachment </summary>
	  Stream getAttachmentContent(string attachmentId);

	  /// <summary>
	  /// Retrieve stream content of a particular attachment to the given task id and attachment id </summary>
	  Stream getTaskAttachmentContent(string taskId, string attachmentId);

	  /// <summary>
	  /// The list of attachments associated to a task </summary>
	  IList<Attachment> getTaskAttachments(string taskId);

	  /// <summary>
	  /// The list of attachments associated to a process instance </summary>
	  IList<Attachment> getProcessInstanceAttachments(string processInstanceId);

	  /// <summary>
	  /// Delete an attachment </summary>
	  void deleteAttachment(string attachmentId);

	  /// <summary>
	  /// Delete an attachment to the given task id and attachment id </summary>
	  void deleteTaskAttachment(string taskId, string attachmentId);

	  /// <summary>
	  /// The list of subtasks for this parent task </summary>
	  IList<Task> getSubTasks(string parentTaskId);

	  /// <summary>
	  /// Instantiate a task report </summary>
	  TaskReport createTaskReport();
	}

}