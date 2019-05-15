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


	/// <summary>
	/// Log entry about an operation performed by a user. This is used for logging
	/// actions such as creating a new task, completing a task,
	/// canceling a process instance, ...
	/// 
	/// <h2>Operation Type</h2>
	/// <para>The type of the operation which has been performed. A user may create a new task,
	/// complete a task, delegate a tasks, etc... Check this class for a list of built-in
	/// operation type constants.</para>
	/// 
	/// <h2>Entity Type</h2>
	/// <para>The type of the entity on which the operation was performed. Operations may be
	/// performed on tasks, attachments, ...</para>
	/// 
	/// <h2>Affected Entity Criteria</h2>
	/// <para>
	///   The methods that reference other entities (except users), such as <seealso cref="#getProcessInstanceId()"/>
	///   or <seealso cref="#getProcessDefinitionId()"/>, describe which entities were affected
	///   by the operation and represent restriction criteria.
	///   A <code>null</code> return value of any of those methods means that regarding
	///   this criterion, any entity was affected.
	/// </para>
	/// <para>
	///   For example, if an operation suspends all process instances that belong to a certain
	///   process definition id, one operation log entry is created.
	///   Its return value for the method <seealso cref="#getProcessInstanceId()"/> is <code>null</code>,
	///   while <seealso cref="#getProcessDefinitionId()"/> returns an id. Thus, the return values
	///   of these methods can be understood as selection criteria of instances of the entity type
	///   that were affected by the operation.
	/// </para>
	/// 
	/// <h2>Additional Considerations</h2>
	/// <para>The event describes which user has requested out the operation and the time
	/// at which the operation was performed. Furthermore, one operation can result in multiple
	/// <seealso cref="UserOperationLogEntry"/> entities whicha are linked by the value of the
	/// <seealso cref="#getOperationId()"/> method.</para>
	/// 
	/// @author Danny Gräf
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface UserOperationLogEntry
	{

	  /// @deprecated Please use <seealso cref="EntityTypes#TASK"/> instead. 
	  /// @deprecated Please use <seealso cref="EntityTypes#IDENTITY_LINK"/> instead. 
	  /// @deprecated Please use <seealso cref="EntityTypes#ATTACHMENT"/> instead. 

	  /// <summary>
	  /// The unique identifier of this log entry. </summary>
	  string Id {get;}

	  /// <summary>
	  /// Deployment reference </summary>
	  string DeploymentId {get;}

	  /// <summary>
	  /// Process definition reference. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Key of the process definition this log entry belongs to; <code>null</code> means any.
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Root process instance reference. </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// Process instance reference. </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Execution reference. </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Case definition reference. </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// Case instance reference. </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// Case execution reference. </summary>
	  string CaseExecutionId {get;}

	  /// <summary>
	  /// Task instance reference. </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// Job instance reference. </summary>
	  string JobId {get;}

	  /// <summary>
	  /// Job definition reference. </summary>
	  string JobDefinitionId {get;}

	  /// <summary>
	  /// Batch reference. </summary>
	  string BatchId {get;}

	  /// <summary>
	  /// The User who performed the operation </summary>
	  string UserId {get;}

	  /// <summary>
	  /// Timestamp of this change. </summary>
	  DateTime Timestamp {get;}

	  /// <summary>
	  /// The unique identifier of this operation.
	  /// 
	  /// If an operation modifies multiple properties, multiple <seealso cref="UserOperationLogEntry"/> instances will be
	  /// created with a common operationId. This allows grouping multiple entries which are part of a composite operation.
	  /// </summary>
	  string OperationId {get;}

	  /// <summary>
	  /// External task reference. </summary>
	  string ExternalTaskId {get;}

	  /// <summary>
	  /// Type of this operation, like create, assign, claim and so on.
	  /// </summary>
	  /// <seealso cref= #OPERATION_TYPE_ASSIGN and other fields beginning with OPERATION_TYPE </seealso>
	  string OperationType {get;}

	  /// <summary>
	  /// The type of the entity on which this operation was executed.
	  /// </summary>
	  /// <seealso cref= #ENTITY_TYPE_TASK and other fields beginning with ENTITY_TYPE </seealso>
	  string EntityType {get;}

	  /// <summary>
	  /// The property changed by this operation. </summary>
	  string Property {get;}

	  /// <summary>
	  /// The original value of the property. </summary>
	  string OrgValue {get;}

	  /// <summary>
	  /// The new value of the property. </summary>
	  string NewValue {get;}

	  /// <summary>
	  /// The time the historic user operation log will be removed. </summary>
	  DateTime RemovalTime {get;}

	  /// <summary>
	  /// The category this entry is associated with </summary>
	  string Category {get;}

	}

	public static class UserOperationLogEntry_Fields
	{
	  public const string ENTITY_TYPE_TASK = EntityTypes.TASK;
	  public const string ENTITY_TYPE_IDENTITY_LINK = EntityTypes.IDENTITY_LINK;
	  public const string ENTITY_TYPE_ATTACHMENT = EntityTypes.ATTACHMENT;
	  public const string OPERATION_TYPE_ASSIGN = "Assign";
	  public const string OPERATION_TYPE_CLAIM = "Claim";
	  public const string OPERATION_TYPE_COMPLETE = "Complete";
	  public const string OPERATION_TYPE_CREATE = "Create";
	  public const string OPERATION_TYPE_DELEGATE = "Delegate";
	  public const string OPERATION_TYPE_DELETE = "Delete";
	  public const string OPERATION_TYPE_RESOLVE = "Resolve";
	  public const string OPERATION_TYPE_SET_OWNER = "SetOwner";
	  public const string OPERATION_TYPE_SET_PRIORITY = "SetPriority";
	  public const string OPERATION_TYPE_UPDATE = "Update";
	  public const string OPERATION_TYPE_ACTIVATE = "Activate";
	  public const string OPERATION_TYPE_SUSPEND = "Suspend";
	  public const string OPERATION_TYPE_MIGRATE = "Migrate";
	  public const string OPERATION_TYPE_ADD_USER_LINK = "AddUserLink";
	  public const string OPERATION_TYPE_DELETE_USER_LINK = "DeleteUserLink";
	  public const string OPERATION_TYPE_ADD_GROUP_LINK = "AddGroupLink";
	  public const string OPERATION_TYPE_DELETE_GROUP_LINK = "DeleteGroupLink";
	  public const string OPERATION_TYPE_SET_DUEDATE = "SetDueDate";
	  public const string OPERATION_TYPE_RECALC_DUEDATE = "RecalculateDueDate";
	  public const string OPERATION_TYPE_UNLOCK = "Unlock";
	  public const string OPERATION_TYPE_EXECUTE = "Execute";
	  public const string OPERATION_TYPE_EVALUATE = "Evaluate";
	  public const string OPERATION_TYPE_ADD_ATTACHMENT = "AddAttachment";
	  public const string OPERATION_TYPE_DELETE_ATTACHMENT = "DeleteAttachment";
	  public const string OPERATION_TYPE_SUSPEND_JOB_DEFINITION = "SuspendJobDefinition";
	  public const string OPERATION_TYPE_ACTIVATE_JOB_DEFINITION = "ActivateJobDefinition";
	  public const string OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION = "SuspendProcessDefinition";
	  public const string OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION = "ActivateProcessDefinition";
	  public const string OPERATION_TYPE_CREATE_HISTORY_CLEANUP_JOB = "CreateHistoryCleanupJobs";
	  public const string OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE = "UpdateHistoryTimeToLive";
	  public const string OPERATION_TYPE_DELETE_HISTORY = "DeleteHistory";
	  public const string OPERATION_TYPE_MODIFY_PROCESS_INSTANCE = "ModifyProcessInstance";
	  public const string OPERATION_TYPE_RESTART_PROCESS_INSTANCE = "RestartProcessInstance";
	  public const string OPERATION_TYPE_SUSPEND_JOB = "SuspendJob";
	  public const string OPERATION_TYPE_ACTIVATE_JOB = "ActivateJob";
	  public const string OPERATION_TYPE_SET_JOB_RETRIES = "SetJobRetries";
	  public const string OPERATION_TYPE_SET_EXTERNAL_TASK_RETRIES = "SetExternalTaskRetries";
	  public const string OPERATION_TYPE_SET_VARIABLE = "SetVariable";
	  public const string OPERATION_TYPE_REMOVE_VARIABLE = "RemoveVariable";
	  public const string OPERATION_TYPE_MODIFY_VARIABLE = "ModifyVariable";
	  public const string OPERATION_TYPE_SUSPEND_BATCH = "SuspendBatch";
	  public const string OPERATION_TYPE_ACTIVATE_BATCH = "ActivateBatch";
	  public const string OPERATION_TYPE_CREATE_INCIDENT = "CreateIncident";
	  public const string OPERATION_TYPE_SET_REMOVAL_TIME = "SetRemovalTime";
	  public const string CATEGORY_ADMIN = "Admin";
	  public const string CATEGORY_OPERATOR = "Operator";
	  public const string CATEGORY_TASK_WORKER = "TaskWorker";
	}

}