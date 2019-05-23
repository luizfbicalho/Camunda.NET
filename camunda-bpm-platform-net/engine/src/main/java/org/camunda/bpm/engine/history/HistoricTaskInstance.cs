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
	/// Represents a historic task instance (waiting, finished or deleted) that is stored permanent for
	/// statistics, audit and other business intelligence purposes.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricTaskInstance
	{

	  /// <summary>
	  /// The unique identifier of this historic task instance. This is the same identifier as the
	  /// runtime Task instance.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// Process definition key reference. </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Process definition reference. </summary>
	  string ProcessDefinitionId {get;}

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
	  /// Case definition key reference. </summary>
	  string CaseDefinitionKey {get;}

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
	  /// Activity instance reference. </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// The latest name given to this task. </summary>
	  string Name {get;}

	  /// <summary>
	  /// The latest description given to this task. </summary>
	  string Description {get;}

	  /// <summary>
	  /// The reason why this task was deleted {'completed' | 'deleted' | any other user defined string }. </summary>
	  string DeleteReason {get;}

	  /// <summary>
	  /// Task owner </summary>
	  string Owner {get;}

	  /// <summary>
	  /// The latest assignee given to this task. </summary>
	  string Assignee {get;}

	  /// <summary>
	  /// Time when the task started. </summary>
	  DateTime StartTime {get;}

	  /// <summary>
	  /// Time when the task was deleted or completed. </summary>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// Difference between <seealso cref="getEndTime()"/> and <seealso cref="getStartTime()"/> in milliseconds. </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// Task definition key. </summary>
	  string TaskDefinitionKey {get;}

	  /// <summary>
	  /// Task priority * </summary>
	  int Priority {get;}

	  /// <summary>
	  /// Task due date * </summary>
	  DateTime DueDate {get;}

	  /// <summary>
	  /// The parent task of this task, in case this task was a subtask </summary>
	  string ParentTaskId {get;}

	  /// <summary>
	  /// Task follow-up date </summary>
	  DateTime FollowUpDate {get;}

	  /// <summary>
	  /// The id of the tenant this historic task instance belongs to. Can be <code>null</code>
	  /// if the historic task instance belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// The time the historic task instance will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}