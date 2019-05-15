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
	/// Represents one task for a human user.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface Task
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
	  /// The <seealso cref="User#getId() userId"/> of the person that is responsible for this task.
	  /// This is used when a task is <seealso cref="TaskService#delegateTask(String, String) delegated"/>. 
	  /// </summary>
	  string Owner {get;set;}


	  /// <summary>
	  /// The <seealso cref="User#getId() userId"/> of the person to which this task is
	  /// <seealso cref="TaskService#setAssignee(String, String) assigned"/> or
	  /// <seealso cref="TaskService#delegateTask(String, String) delegated"/>. 
	  /// </summary>
	  string Assignee {get;set;}


	  /// <summary>
	  /// The current <seealso cref="DelegationState"/> for this task. </summary>
	  DelegationState DelegationState {get;set;}


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
	  string CaseInstanceId {get;set;}


	  /// <summary>
	  /// Reference to the path of case execution or null if it is not related to a case instance. </summary>
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
		/// Due date of the task. </summary>
		DateTime DueDate {get;set;}


		/// <summary>
		/// Follow-up date of the task. </summary>
		DateTime FollowUpDate {get;set;}


		/// <summary>
		/// delegates this task to the given user and sets the <seealso cref="#getDelegationState() delegationState"/> to <seealso cref="DelegationState#PENDING"/>.
		/// If no owner is set on the task, the owner is set to the current assignee of the task. 
		/// </summary>
	  void @delegate(string userId);

	  /// <summary>
	  /// the parent task for which this task is a subtask </summary>
	  string ParentTaskId {set;get;}


	  /// <summary>
	  /// Indicated whether this task is suspended or not. </summary>
	  bool Suspended {get;}

	  /// <summary>
	  /// Provides the form key for the task.
	  /// 
	  /// <para><strong>NOTE:</strong> If the task instance us obtained through a query, this property is only populated in case the
	  /// <seealso cref="TaskQuery#initializeFormKeys()"/> method is called. If this method is called without a prior call to
	  /// <seealso cref="TaskQuery#initializeFormKeys()"/>, it will throw a <seealso cref="BadUserRequestException"/>.</para>
	  /// </summary>
	  /// <returns> the form key for this task </returns>
	  /// <exception cref="BadUserRequestException"> in case the form key is not initialized. </exception>
	  string FormKey {get;}

	  /// <summary>
	  /// Returns the task's tenant id or null in case this task does not belong to a tenant.
	  /// </summary>
	  /// <returns> the task's tenant id or null
	  /// 
	  /// @since 7.5 </returns>
	  string TenantId {get;set;}


	}

	public static class Task_Fields
	{
	  public const int PRIORITY_MINIUM = 0;
	  public const int PRIORITY_NORMAL = 50;
	  public const int PRIORITY_MAXIMUM = 100;
	}

}