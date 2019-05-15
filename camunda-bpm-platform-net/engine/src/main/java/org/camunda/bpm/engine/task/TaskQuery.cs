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
namespace org.camunda.bpm.engine.task
{

	using Query = org.camunda.bpm.engine.query.Query;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="Task"/>s;
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface TaskQuery : Query<TaskQuery, Task>
	{

	  /// <summary>
	  /// Only select tasks with the given task id (in practice, there will be
	  /// maximum one of this kind)
	  /// </summary>
	  TaskQuery taskId(string taskId);

	  /// <summary>
	  /// Only select tasks with the given name </summary>
	  TaskQuery taskName(string name);

	  /// <summary>
	  /// Only select tasks with a name not matching the given name </summary>
	  TaskQuery taskNameNotEqual(string name);

	  /// <summary>
	  /// Only select tasks with a name matching the parameter.
	  ///  The syntax is that of SQL: for example usage: nameLike(%camunda%)
	  /// </summary>
	  TaskQuery taskNameLike(string nameLike);

	  /// <summary>
	  /// Only select tasks with a name not matching the parameter.
	  ///  The syntax is that of SQL: for example usage: nameNotLike(%camunda%)
	  /// </summary>
	  TaskQuery taskNameNotLike(string nameNotLike);

	  /// <summary>
	  /// Only select tasks with the given description. </summary>
	  TaskQuery taskDescription(string description);

	  /// <summary>
	  /// Only select tasks with a description matching the parameter .
	  ///  The syntax is that of SQL: for example usage: descriptionLike(%camunda%)
	  /// </summary>
	  TaskQuery taskDescriptionLike(string descriptionLike);

	  /// <summary>
	  /// Only select tasks with the given priority. </summary>
	  TaskQuery taskPriority(int? priority);

	  /// <summary>
	  /// Only select tasks with the given priority or higher. </summary>
	  TaskQuery taskMinPriority(int? minPriority);

	  /// <summary>
	  /// Only select tasks with the given priority or lower. </summary>
	  TaskQuery taskMaxPriority(int? maxPriority);

	  /// <summary>
	  /// Only select tasks which are assigned to the given user. </summary>
	  TaskQuery taskAssignee(string assignee);

	  /// <summary>
	  ///  <para>Only select tasks which are assigned to the user described by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskAssigneeExpression(string assigneeExpression);

	  /// <summary>
	  /// Only select tasks which are matching the given user.
	  ///  The syntax is that of SQL: for example usage: nameLike(%camunda%)
	  /// </summary>
	  TaskQuery taskAssigneeLike(string assignee);

	  /// <summary>
	  /// <para>Only select tasks which are assigned to the user described by the given expression.
	  ///  The syntax is that of SQL: for example usage: taskAssigneeLikeExpression("${'%test%'}")</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskAssigneeLikeExpression(string assigneeLikeExpression);

	  /// <summary>
	  /// Only select tasks for which the given user is the owner. </summary>
	  TaskQuery taskOwner(string owner);

	  /// <summary>
	  /// <para>Only select tasks for which the described user by the given expression is the owner.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskOwnerExpression(string ownerExpression);

	  /// <summary>
	  /// Only select tasks which don't have an assignee. </summary>
	  TaskQuery taskUnassigned();

	  /// <seealso cref= <seealso cref="#taskUnassigned"/> </seealso>
	  [Obsolete]
	  TaskQuery taskUnnassigned();

	  /// <summary>
	  /// Only select tasks which have an assignee. </summary>
	  TaskQuery taskAssigned();

	  /// <summary>
	  /// Only select tasks with the given <seealso cref="DelegationState"/>. </summary>
	  TaskQuery taskDelegationState(DelegationState delegationState);

	  /// <summary>
	  /// Only select tasks for which the given user or one of his groups is a candidate.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///     <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the "and query" instance.
	  ///     No exception is thrown when query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///     <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the "or query" instance.
	  ///   <li>When passed user is <code>null</code>.
	  ///   </ul> </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
	  ///  </exception>
	  TaskQuery taskCandidateUser(string candidateUser);

	  /// <summary>
	  /// Only select tasks for which the described user by the given expression is a candidate.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///     <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the query instance.
	  ///   <li>When passed user is <code>null</code>.
	  ///   </ul> </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskCandidateUserExpression(string candidateUserExpression);

	  /// <summary>
	  /// Only select tasks for which there exist an <seealso cref="IdentityLink"/> with the given user </summary>
	  TaskQuery taskInvolvedUser(string involvedUser);

	  /// <summary>
	  /// <para>Only select tasks for which there exist an <seealso cref="IdentityLink"/> with the
	  /// described user by the given expression</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskInvolvedUserExpression(string involvedUserExpression);

	  /// <summary>
	  /// Only select tasks which have a candidate group
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query". </exception>
	  TaskQuery withCandidateGroups();

	  /// <summary>
	  /// Only select tasks which have no candidate group
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query". </exception>
	  TaskQuery withoutCandidateGroups();

	  /// <summary>
	  /// Only select tasks which have a candidate user
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query". </exception>
	  TaskQuery withCandidateUsers();

	  /// <summary>
	  /// Only select tasks which have no candidate user
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query". </exception>
	  TaskQuery withoutCandidateUsers();

	  /// <summary>
	  ///  Only select tasks for which users in the given group are candidates.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateUser(String)"/> or
	  ///     <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the "and query" instance.</li>
	  ///   No exception is thrown when query is executed and <seealso cref="#taskCandidateUser(String)"/> or
	  ///   <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the "or query" instance.</li>
	  ///   <li>When passed group is <code>null</code>.</li></ul> </exception>
	  TaskQuery taskCandidateGroup(string candidateGroup);

	  /// <summary>
	  /// Only select tasks for which users in the described group by the given expression are candidates.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateUser(String)"/> or
	  ///     <seealso cref="#taskCandidateGroupIn(List)"/> has been executed on the query instance.
	  ///   <li>When passed group is <code>null</code>.
	  ///   </ul> </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskCandidateGroupExpression(string candidateGroupExpression);

	  /// <summary>
	  /// Only select tasks for which the 'candidateGroup' is one of the given groups.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///     <seealso cref="#taskCandidateUser(String)"/> has been executed on the "and query" instance.</li>
	  ///   No exception is thrown when query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///   <seealso cref="#taskCandidateUser(String)"/> has been executed on the "or query" instance.</li>
	  ///   <li>When passed group list is empty or <code>null</code>.</li></ul> </exception>
	  TaskQuery taskCandidateGroupIn(IList<string> candidateGroups);

	  /// <summary>
	  /// Only select tasks for which the 'candidateGroup' is one of the described groups of the given expression.
	  /// 
	  /// <para>
	  /// Per default it only selects tasks which are not already assigned
	  /// to a user. To also include assigned task in the result specify
	  /// <seealso cref="#includeAssignedTasks()"/> in your query.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   <ul><li>When query is executed and <seealso cref="#taskCandidateGroup(String)"/> or
	  ///     <seealso cref="#taskCandidateUser(String)"/> has been executed on the query instance.
	  ///   <li>When passed group list is empty or <code>null</code>.</ul> </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery taskCandidateGroupInExpression(string candidateGroupsExpression);

	  /// <summary>
	  /// Select both assigned and not assigned tasks for candidate user or group queries.
	  /// <para>
	  /// By default <seealso cref="#taskCandidateUser(String)"/>, <seealso cref="#taskCandidateGroup(String)"/>
	  /// and <seealso cref="#taskCandidateGroupIn(List)"/> queries only select not assigned tasks.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///    When no candidate user or group(s) are specified beforehand </exception>
	  TaskQuery includeAssignedTasks();

	  /// <summary>
	  /// Only select tasks for the given process instance id. </summary>
	  TaskQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select tasks for the given process instance business key </summary>
	  TaskQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select tasks for the given process instance business key described by the given expression </summary>
	  TaskQuery processInstanceBusinessKeyExpression(string processInstanceBusinessKeyExpression);

	  /// <summary>
	  /// Only select tasks for any of the given the given process instance business keys.
	  /// </summary>
	  TaskQuery processInstanceBusinessKeyIn(params string[] processInstanceBusinessKeys);

	  /// <summary>
	  /// Only select tasks matching the given process instance business key.
	  ///  The syntax is that of SQL: for example usage: nameLike(%camunda%)
	  /// </summary>
	  TaskQuery processInstanceBusinessKeyLike(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select tasks matching the given process instance business key described by the given expression.
	  ///  The syntax is that of SQL: for example usage: processInstanceBusinessKeyLikeExpression("${ '%camunda%' }")
	  /// </summary>
	  TaskQuery processInstanceBusinessKeyLikeExpression(string processInstanceBusinessKeyExpression);

	  /// <summary>
	  /// Only select tasks for the given execution. </summary>
	  TaskQuery executionId(string executionId);

	  /// <summary>
	  /// Only select task which have one of the activity instance ids. * </summary>
	  TaskQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select tasks that are created on the given date. * </summary>
	  TaskQuery taskCreatedOn(DateTime createTime);

	  /// <summary>
	  /// Only select tasks that are created on the described date by the given expression. * </summary>
	  TaskQuery taskCreatedOnExpression(string createTimeExpression);

	  /// <summary>
	  /// Only select tasks that are created before the given date. * </summary>
	  TaskQuery taskCreatedBefore(DateTime before);

	  /// <summary>
	  /// Only select tasks that are created before the described date by the given expression. * </summary>
	  TaskQuery taskCreatedBeforeExpression(string beforeExpression);

	  /// <summary>
	  /// Only select tasks that are created after the given date. * </summary>
	  TaskQuery taskCreatedAfter(DateTime after);

	  /// <summary>
	  /// Only select tasks that are created after the described date by the given expression. * </summary>
	  TaskQuery taskCreatedAfterExpression(string afterExpression);

	  /// <summary>
	  /// Only select tasks that have no parent (i.e. do not select subtasks). * </summary>
	  TaskQuery excludeSubtasks();

	  /// <summary>
	  /// Only select tasks with the given taskDefinitionKey.
	  /// The task definition key is the id of the userTask:
	  /// &lt;userTask id="xxx" .../&gt;
	  /// 
	  /// </summary>
	  TaskQuery taskDefinitionKey(string key);

	  /// <summary>
	  /// Only select tasks with a taskDefinitionKey that match the given parameter.
	  ///  The syntax is that of SQL: for example usage: taskDefinitionKeyLike("%camunda%").
	  /// The task definition key is the id of the userTask:
	  /// &lt;userTask id="xxx" .../&gt;
	  /// 
	  /// </summary>
	  TaskQuery taskDefinitionKeyLike(string keyLike);

	  /// <summary>
	  /// Only select tasks which have one of the taskDefinitionKeys. * </summary>
	  TaskQuery taskDefinitionKeyIn(params string[] taskDefinitionKeys);

	  /// <summary>
	  /// Select the tasks which are sub tasks of the given parent task.
	  /// </summary>
	  TaskQuery taskParentTaskId(string parentTaskId);

	  /// <summary>
	  /// Only select tasks for the given case instance id. </summary>
	  TaskQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select tasks for the given case instance business key </summary>
	  TaskQuery caseInstanceBusinessKey(string caseInstanceBusinessKey);

	  /// <summary>
	  /// Only select tasks matching the given case instance business key.
	  ///  The syntax is that of SQL: for example usage: nameLike(%aBusinessKey%)
	  /// </summary>
	  TaskQuery caseInstanceBusinessKeyLike(string caseInstanceBusinessKeyLike);

	  /// <summary>
	  /// Only select tasks for the given case execution. </summary>
	  TaskQuery caseExecutionId(string caseExecutionId);

	  /// <summary>
	  /// Only select tasks which are part of a case instance which has the given
	  /// case definition key.
	  /// </summary>
	  TaskQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select tasks which are part of a case instance which has the given
	  /// case definition id.
	  /// </summary>
	  TaskQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select tasks which are part of a case instance which has the given
	  /// case definition name.
	  /// </summary>
	  TaskQuery caseDefinitionName(string caseDefinitionName);

	  /// <summary>
	  /// Only select tasks which are part of a case instance which case definition
	  /// name is like the given parameter.
	  /// The syntax is that of SQL: for example usage: nameLike(%processDefinitionName%)
	  /// </summary>
	  TaskQuery caseDefinitionNameLike(string caseDefinitionNameLike);

	  /// <summary>
	  /// All queries for task-, process- and case-variables will match the variable names in a case-insensitive way.
	  /// </summary>
	  TaskQuery matchVariableNamesIgnoreCase();

	  /// <summary>
	  /// All queries for task-, process- and case-variables will match the variable values in a case-insensitive way.
	  /// </summary>
	  TaskQuery matchVariableValuesIgnoreCase();

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// set to the given value.
	  /// </summary>
	  TaskQuery taskVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  TaskQuery taskVariableValueNotEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// matching the given value.
	  /// The syntax is that of SQL: for example usage: valueLike(%value%)
	  /// </summary>
	  TaskQuery taskVariableValueLike(string variableName, string variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// and a value greater than the given one.
	  /// </summary>
	  TaskQuery taskVariableValueGreaterThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// and a value greater than or equal to the given one.
	  /// </summary>
	  TaskQuery taskVariableValueGreaterThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// and a value less than the given one.
	  /// </summary>
	  TaskQuery taskVariableValueLessThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a local task variable with the given name
	  /// and a value less than or equal to the given one.
	  /// </summary>
	  TaskQuery taskVariableValueLessThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have are part of a process that have a variable
	  /// with the given name set to the given value.
	  /// </summary>
	  TaskQuery processVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which have a variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  TaskQuery processVariableValueNotEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process that have a variable
	  /// with the given name and matching the given value.
	  /// The syntax is that of SQL: for example usage: valueLike(%value%)
	  /// </summary>
	  TaskQuery processVariableValueLike(string variableName, string variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process that have a variable
	  /// with the given name and a value greater than the given one.
	  /// </summary>
	  TaskQuery processVariableValueGreaterThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process that have a variable
	  /// with the given name and a value greater than or equal to the given one.
	  /// </summary>
	  TaskQuery processVariableValueGreaterThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process that have a variable
	  /// with the given name and a value less than the given one.
	  /// </summary>
	  TaskQuery processVariableValueLessThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process that have a variable
	  /// with the given name and a value greater than or equal to the given one.
	  /// </summary>
	  TaskQuery processVariableValueLessThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable
	  /// with the given name set to the given value. The type of variable is determined based
	  /// on the value, using types configured in <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable
	  /// with the given name, but with a different value than the passed value. The
	  /// type of variable is determined based on the value, using types configured
	  /// in <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueNotEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable value
	  /// like the given value.
	  /// 
	  /// This be used on string variables only.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null.
	  /// </param>
	  /// <param name="value"> variable value. The string can include the
	  /// wildcard character '%' to express like-strategy:
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  TaskQuery caseInstanceVariableValueLike(string variableName, string variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable
	  /// with the given name and a variable value greater than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueGreaterThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a
	  /// variable value greater than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueGreaterThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable
	  /// value less than the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueLessThan(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a case instance that have a variable
	  /// value less than or equal to the passed value.
	  /// 
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  TaskQuery caseInstanceVariableValueLessThanOrEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select tasks which are part of a process instance which has the given
	  /// process definition key.
	  /// </summary>
	  TaskQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select tasks which are part of a process instance which has one of the
	  /// given process definition keys.
	  /// </summary>
	  TaskQuery processDefinitionKeyIn(params string[] processDefinitionKeys);

	  /// <summary>
	  /// Only select tasks which are part of a process instance which has the given
	  /// process definition id.
	  /// </summary>
	  TaskQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select tasks which are part of a process instance which has the given
	  /// process definition name.
	  /// </summary>
	  TaskQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select tasks which are part of a process instance which process definition
	  /// name  is like the given parameter.
	  /// The syntax is that of SQL: for example usage: nameLike(%processDefinitionName%)
	  /// </summary>
	  TaskQuery processDefinitionNameLike(string processDefinitionName);

	  /// <summary>
	  /// Only select tasks with the given due date.
	  /// </summary>
	  TaskQuery dueDate(DateTime dueDate);

	  /// <summary>
	  /// <para>Only select tasks with the described due date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery dueDateExpression(string dueDateExpression);

	  /// <summary>
	  /// Only select tasks which have a due date before the given date.
	  /// </summary>
	  TaskQuery dueBefore(DateTime dueDate);

	  /// <summary>
	  /// <para>Only select tasks which have a due date before the described date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery dueBeforeExpression(string dueDateExpression);

	  /// <summary>
	  /// Only select tasks which have a due date after the given date.
	  /// </summary>
	  TaskQuery dueAfter(DateTime dueDate);

	  /// <summary>
	  /// <para>Only select tasks which have a due date after the described date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery dueAfterExpression(string dueDateExpression);

	  /// <summary>
	  /// Only select tasks with the given follow-up date.
	  /// </summary>
	  TaskQuery followUpDate(DateTime followUpDate);

	  /// <summary>
	  /// <para>Only select tasks with the described follow-up date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery followUpDateExpression(string followUpDateExpression);

	  /// <summary>
	  /// Only select tasks which have a follow-up date before the given date.
	  /// </summary>
	  TaskQuery followUpBefore(DateTime followUpDate);

	  /// <summary>
	  /// <para>Only select tasks which have a follow-up date before the described date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery followUpBeforeExpression(string followUpDateExpression);

	  /// <summary>
	  /// Only select tasks which have no follow-up date or a follow-up date before the given date.
	  /// Serves the typical use case "give me all tasks without follow-up or follow-up date which is already due"
	  /// </summary>
	  TaskQuery followUpBeforeOrNotExistent(DateTime followUpDate);

	  /// <summary>
	  /// <para>Only select tasks which have no follow-up date or a follow-up date before the described date by the given expression.
	  /// Serves the typical use case "give me all tasks without follow-up or follow-up date which is already due"</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery followUpBeforeOrNotExistentExpression(string followUpDateExpression);

	  /// <summary>
	  /// Only select tasks which have a follow-up date after the given date.
	  /// </summary>
	  TaskQuery followUpAfter(DateTime followUpDate);

	  /// <summary>
	  /// <para>Only select tasks which have a follow-up date after the described date by the given expression.</para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the query is executed and expressions are disabled for adhoc queries
	  ///  (in case the query is executed via <seealso cref="#list()"/>, <seealso cref="#listPage(int, int)"/>, <seealso cref="#singleResult()"/>, or <seealso cref="#count()"/>)
	  ///  or stored queries (in case the query is stored along with a filter).
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  TaskQuery followUpAfterExpression(string followUpDateExpression);

	  /// <summary>
	  /// Only select tasks which are suspended, because its process instance was suspended.
	  /// </summary>
	  TaskQuery suspended();

	  /// <summary>
	  /// Only select tasks which are active (ie. not suspended)
	  /// </summary>
	  TaskQuery active();

	  /// <summary>
	  /// If called, the form keys of the fetched tasks are initialized and
	  /// <seealso cref="Task#getFormKey()"/> will return a value (in case the task has a form key).
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///   When method has been executed within "or query". Method must be executed on the base query.
	  /// </exception>
	  /// <returns> the query itself </returns>
	  TaskQuery initializeFormKeys();

	  /// <summary>
	  /// Only select tasks with one of the given tenant ids. </summary>
	  TaskQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select tasks which have no tenant id. </summary>
	  TaskQuery withoutTenantId();

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by task id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskId();

	  /// <summary>
	  /// Order by task name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskName();

	  /// <summary>
	  /// Order by task name case insensitive (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskNameCaseInsensitive();

	  /// <summary>
	  /// Order by description (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskDescription();

	  /// <summary>
	  /// Order by priority (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskPriority();

	  /// <summary>
	  /// Order by assignee (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskAssignee();

	  /// <summary>
	  /// Order by the time on which the tasks were created (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskCreateTime();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by case instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByCaseInstanceId();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByExecutionId();

	  /// <summary>
	  /// Order by case execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByCaseExecutionId();

	  /// <summary>
	  /// Order by due date (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByDueDate();

	  /// <summary>
	  /// Order by follow-up date (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByFollowUpDate();

	  /// <summary>
	  /// Order by a process instance variable value of a certain type. Calling this method multiple times
	  /// specifies secondary, tertiary orderings, etc. The ordering of variables with <code>null</code>
	  /// values is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByProcessVariable(string variableName, ValueType valueType);

	  /// <summary>
	  /// Order by an execution variable value of a certain type. Calling this method multiple times
	  /// specifies secondary, tertiary orderings, etc. The ordering of variables with <code>null</code>
	  /// values is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByExecutionVariable(string variableName, ValueType valueType);

	  /// <summary>
	  /// Order by a task variable value of a certain type. Calling this method multiple times
	  /// specifies secondary, tertiary orderings, etc. The ordering of variables with <code>null</code>
	  /// values is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTaskVariable(string variableName, ValueType valueType);

	  /// <summary>
	  /// Order by a task variable value of a certain type. Calling this method multiple times
	  /// specifies secondary, tertiary orderings, etc. The ordering of variables with <code>null</code>
	  /// values is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByCaseExecutionVariable(string variableName, ValueType valueType);

	  /// <summary>
	  /// Order by a task variable value of a certain type. Calling this method multiple times
	  /// specifies secondary, tertiary orderings, etc. The ordering of variables with <code>null</code>
	  /// values is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByCaseInstanceVariable(string variableName, ValueType valueType);

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of tasks without tenant id is database-specific.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> When method has been executed within "or query".
	  ///  </exception>
	  TaskQuery orderByTenantId();

	  /// <summary>
	  /// <para>After calling or(), a chain of several filter criteria could follow. Each filter criterion that follows or()
	  /// will be linked together with an OR expression until the OR query is terminated. To terminate the OR query right
	  /// after the last filter criterion was applied, <seealso cref="#endOr()"/> must be invoked.</para>
	  /// </summary>
	  /// <returns> an object of the type <seealso cref="TaskQuery"/> on which an arbitrary amount of filter criteria could be applied.
	  /// The several filter criteria will be linked together by an OR expression.
	  /// </returns>
	  /// <exception cref="ProcessEngineException"> when or() has been invoked directly after or() or after or() and trailing filter
	  /// criteria. To prevent throwing this exception, <seealso cref="#endOr()"/> must be invoked after a chain of filter criteria to
	  /// mark the end of the OR query.
	  ///  </exception>
	  TaskQuery or();

	  /// <summary>
	  /// <para>endOr() terminates an OR query on which an arbitrary amount of filter criteria were applied. To terminate the
	  /// OR query which has been started by invoking <seealso cref="#or()"/>, endOr() must be invoked. Filter criteria which are
	  /// applied after calling endOr() are linked together by an AND expression.</para>
	  /// </summary>
	  /// <returns> an object of the type <seealso cref="TaskQuery"/> on which an arbitrary amount of filter criteria could be applied.
	  /// The filter criteria will be linked together by an AND expression.
	  /// </returns>
	  /// <exception cref="ProcessEngineException"> when endOr() has been invoked before <seealso cref="#or()"/> was invoked. To prevent throwing
	  /// this exception, <seealso cref="#or()"/> must be invoked first.
	  ///  </exception>
	  TaskQuery endOr();
	}

}