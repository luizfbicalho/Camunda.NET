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

	using Query = org.camunda.bpm.engine.query.Query;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// Allows programmatic querying for <seealso cref="HistoricTaskInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricTaskInstanceQuery : Query<HistoricTaskInstanceQuery, HistoricTaskInstance>
	{

	  /// <summary>
	  /// Only select historic task instances for the given task id. </summary>
	  HistoricTaskInstanceQuery taskId(string taskId);

	  /// <summary>
	  /// Only select historic task instances for the given process instance. </summary>
	  HistoricTaskInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic tasks for the given process instance business key </summary>
	  HistoricTaskInstanceQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select historic tasks for any of the given the given process instance business keys.
	  /// </summary>
	  HistoricTaskInstanceQuery processInstanceBusinessKeyIn(params string[] processInstanceBusinessKeys);

	  /// <summary>
	  /// Only select historic tasks matching the given process instance business key.
	  ///  The syntax is that of SQL: for example usage: nameLike(%camunda%)
	  /// </summary>
	  HistoricTaskInstanceQuery processInstanceBusinessKeyLike(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select historic task instances for the given execution. </summary>
	  HistoricTaskInstanceQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic task instances which have one of the given activity instance ids. * </summary>
	  HistoricTaskInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select historic task instances for the given process definition. </summary>
	  HistoricTaskInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic task instances which are part of a (historic) process instance
	  /// which has the given process definition key.
	  /// </summary>
	  HistoricTaskInstanceQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic task instances which are part of a (historic) process instance
	  /// which has the given definition name.
	  /// </summary>
	  HistoricTaskInstanceQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select historic task instances for the given case definition. </summary>
	  HistoricTaskInstanceQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select historic task instances which are part of a case instance
	  /// which has the given case definition key.
	  /// </summary>
	  HistoricTaskInstanceQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select historic task instances which are part of a (historic) case instance
	  /// which has the given case definition name.
	  /// </summary>
	  HistoricTaskInstanceQuery caseDefinitionName(string caseDefinitionName);

	  /// <summary>
	  /// Only select historic task instances for the given case instance.
	  /// </summary>
	  HistoricTaskInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic task instances for the given case execution id.
	  /// </summary>
	  HistoricTaskInstanceQuery caseExecutionId(string caseExecutionId);

	  /// <summary>
	  /// Only select historic task instances with the given task name.
	  /// This is the last name given to the task.
	  /// </summary>
	  HistoricTaskInstanceQuery taskName(string taskName);

	  /// <summary>
	  /// Only select historic task instances with a task name like the given value.
	  /// This is the last name given to the task.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%.
	  /// </summary>
	  HistoricTaskInstanceQuery taskNameLike(string taskNameLike);

	  /// <summary>
	  /// Only select historic task instances with the given task description.
	  /// This is the last description given to the task.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDescription(string taskDescription);

	  /// <summary>
	  /// Only select historic task instances with a task description like the given value.
	  /// This is the last description given to the task.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDescriptionLike(string taskDescriptionLike);

	  /// <summary>
	  /// Only select historic task instances with the given task definition key. </summary>
	  /// <seealso cref= Task#getTaskDefinitionKey() </seealso>
	  HistoricTaskInstanceQuery taskDefinitionKey(string taskDefinitionKey);

	  /// <summary>
	  /// Only select historic task instances with one of the given task definition keys. </summary>
	  /// <seealso cref= Task#getTaskDefinitionKey() </seealso>
	  HistoricTaskInstanceQuery taskDefinitionKeyIn(params string[] taskDefinitionKeys);

	  /// <summary>
	  /// Only select historic task instances with the given task delete reason. </summary>
	  HistoricTaskInstanceQuery taskDeleteReason(string taskDeleteReason);

	  /// <summary>
	  /// Only select historic task instances with a task description like the given value.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDeleteReasonLike(string taskDeleteReasonLike);

	  /// <summary>
	  /// Only select historic task instances with an assignee. </summary>
	  HistoricTaskInstanceQuery taskAssigned();

	  /// <summary>
	  /// Only select historic task instances without an assignee. </summary>
	  HistoricTaskInstanceQuery taskUnassigned();

	  /// <summary>
	  /// Only select historic task instances which were last taskAssigned to the given assignee.
	  /// </summary>
	  HistoricTaskInstanceQuery taskAssignee(string taskAssignee);

	  /// <summary>
	  /// Only select historic task instances which were last taskAssigned to an assignee like
	  /// the given value.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%.
	  /// </summary>
	  HistoricTaskInstanceQuery taskAssigneeLike(string taskAssigneeLike);

	  /// <summary>
	  /// Only select historic task instances which have the given owner.
	  /// </summary>
	  HistoricTaskInstanceQuery taskOwner(string taskOwner);

	  /// <summary>
	  /// Only select historic task instances which have an owner like the one specified.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%.
	  /// </summary>
	  HistoricTaskInstanceQuery taskOwnerLike(string taskOwnerLike);

	  /// <summary>
	  /// Only select historic task instances with the given priority.
	  /// </summary>
	  HistoricTaskInstanceQuery taskPriority(int? taskPriority);

	  /// <summary>
	  /// Only select historic task instances which are finished.
	  /// </summary>
	  HistoricTaskInstanceQuery finished();

	  /// <summary>
	  /// Only select historic task instances which aren't finished yet.
	  /// </summary>
	  HistoricTaskInstanceQuery unfinished();

	  /// <summary>
	  /// Only select historic task instances which are part of a process
	  /// instance which is already finished.
	  /// </summary>
	  HistoricTaskInstanceQuery processFinished();

	  /// <summary>
	  /// Only select historic task instances which are part of a process
	  /// instance which is not finished yet.
	  /// </summary>
	  HistoricTaskInstanceQuery processUnfinished();

	  /// <summary>
	  /// Only select historic task instances which have mapping
	  /// with Historic identity links based on user id
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricTaskInstanceQuery taskInvolvedUser(string involvedUser);

	  /// <summary>
	  /// Only select historic task instances which have mapping
	  /// with Historic identity links based on group id
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricTaskInstanceQuery taskInvolvedGroup(string involvedGroup);

	  /// <summary>
	  /// Only select historic task instances which have mapping
	  /// with Historic identity links with the condition of user being a candidate
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricTaskInstanceQuery taskHadCandidateUser(string candidateUser);

	  /// <summary>
	  /// Only select historic task instances which have mapping
	  /// with Historic identity links with the condition of group being a candidate
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricTaskInstanceQuery taskHadCandidateGroup(string candidateGroup);

	  /// <summary>
	  /// Only select historic task instances which have a candidate group </summary>
	  HistoricTaskInstanceQuery withCandidateGroups();

	  /// <summary>
	  /// Only select historic task instances which have no candidate group </summary>
	  HistoricTaskInstanceQuery withoutCandidateGroups();

	  /// <summary>
	  /// Only select historic task instances which have a local task variable with the
	  /// given name set to the given value. Make sure history-level is configured
	  /// >= AUDIT when this feature is used.
	  /// </summary>
	  HistoricTaskInstanceQuery taskVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select subtasks of the given parent task </summary>
	  HistoricTaskInstanceQuery taskParentTaskId(string parentTaskId);

	  /// <summary>
	  /// Only select historic task instances which are part of a process instance which have a variable
	  /// with the given name set to the given value. The last variable value in the variable updates
	  /// (<seealso cref="HistoricDetail"/>) will be used, so make sure history-level is configured
	  /// >= AUDIT when this feature is used.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which have a variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueNotEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which are part of a process that have a variable
	  /// with the given name and matching the given value.
	  /// The syntax is that of SQL: for example usage: valueLike(%value%)
	  /// 
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueLike(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which are part of a process that have a variable
	  /// with the given name and a value greater than the given one.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueGreaterThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which are part of a process that have a variable
	  /// with the given name and a value greater than or equal to the given one.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueGreaterThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which are part of a process that have a variable
	  /// with the given name and a value less than the given one.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueLessThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select historic task instances which are part of a process that have a variable
	  /// with the given name and a value less than or equal to the given one.
	  /// </summary>
	  HistoricTaskInstanceQuery processVariableValueLessThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select select historic task instances with the given due date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDueDate(DateTime dueDate);

	  /// <summary>
	  /// Only select select historic task instances which have a due date before the given date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDueBefore(DateTime dueDate);

	  /// <summary>
	  /// Only select select historic task instances which have a due date after the given date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskDueAfter(DateTime dueDate);

	  /// <summary>
	  /// Only select select historic task instances with the given follow-up date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskFollowUpDate(DateTime followUpDate);

	  /// <summary>
	  /// Only select select historic task instances which have a follow-up date before the given date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskFollowUpBefore(DateTime followUpDate);

	  /// <summary>
	  /// Only select select historic task instances which have a follow-up date after the given date.
	  /// </summary>
	  HistoricTaskInstanceQuery taskFollowUpAfter(DateTime followUpDate);

	  /// <summary>
	  /// Only select historic task instances with one of the given tenant ids. </summary>
	  HistoricTaskInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select tasks where end time is after given date
	  /// </summary>
	  HistoricTaskInstanceQuery finishedAfter(DateTime date);

	  /// <summary>
	  /// Only select tasks where end time is before given date
	  /// </summary>
	  HistoricTaskInstanceQuery finishedBefore(DateTime date);

	  /// <summary>
	  /// Only select tasks where started after given date
	  /// </summary>
	  HistoricTaskInstanceQuery startedAfter(DateTime date);

	  /// <summary>
	  /// Only select tasks where started before given date
	  /// </summary>
	  HistoricTaskInstanceQuery startedBefore(DateTime date);

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic task instances without tenant id is database-specific.
	  /// </summary>
	  HistoricTaskInstanceQuery orderByTenantId();

	  /// <summary>
	  /// Order by task id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskId();

	  /// <summary>
	  /// Order by the historic activity instance id this task was used in
	  /// (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  HistoricTaskInstanceQuery orderByHistoricActivityInstanceId();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByExecutionId();

	  /// <summary>
	  /// Order by duration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByHistoricTaskInstanceDuration();

	  /// <summary>
	  /// Order by end time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByHistoricTaskInstanceEndTime();

	  /// <summary>
	  /// Order by start time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByHistoricActivityInstanceStartTime();

	  /// <summary>
	  /// Order by task name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskName();

	  /// <summary>
	  /// Order by task description (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskDescription();

	  /// <summary>
	  /// Order by task assignee (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskAssignee();

	  /// <summary>
	  /// Order by task owner (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskOwner();

	  /// <summary>
	  /// Order by task due date (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskDueDate();

	  /// <summary>
	  /// Order by task follow-up date (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskFollowUpDate();

	  /// <summary>
	  /// Order by task delete reason (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByDeleteReason();

	  /// <summary>
	  /// Order by task definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskDefinitionKey();

	  /// <summary>
	  /// Order by task priority key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByTaskPriority();

	  /// <summary>
	  /// Order by case definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by case instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByCaseInstanceId();

	  /// <summary>
	  /// Order by case execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricTaskInstanceQuery orderByCaseExecutionId();

	}

}