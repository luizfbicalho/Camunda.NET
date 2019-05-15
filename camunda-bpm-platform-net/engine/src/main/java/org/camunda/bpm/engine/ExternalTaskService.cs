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
namespace org.camunda.bpm.engine
{

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using UpdateExternalTaskRetriesBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesBuilder;
	using UpdateExternalTaskRetriesSelectBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesSelectBuilder;

	/// <summary>
	/// Service that provides access to <seealso cref="ExternalTask"/> instances. External tasks
	/// represent work items that are processed externally and independently of the process
	/// engine.
	/// 
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public interface ExternalTaskService
	{

	  /// <summary>
	  /// Calls method fetchAndLock(maxTasks, workerId, usePriority), where usePriority is false.
	  /// </summary>
	  /// <param name="maxTasks"> the maximum number of tasks to return </param>
	  /// <param name="workerId"> the id of the worker to lock the tasks for </param>
	  /// <returns> a builder to define and execute an external task fetching operation </returns>
	  /// <seealso cref= <seealso cref="ExternalTaskService#fetchAndLock(int, java.lang.String, boolean)"/>. </seealso>
	  ExternalTaskQueryBuilder fetchAndLock(int maxTasks, string workerId);

	  /// <summary>
	  /// <para>Defines fetching of external tasks by using a fluent builder.
	  /// The following parameters must be specified:
	  /// A worker id, a maximum number of tasks to fetch and a flag that indicates
	  /// whether priority should be regarded or not.
	  /// The builder allows to specify multiple topics to fetch tasks for and
	  /// individual lock durations. For every topic, variables can be fetched
	  /// in addition.Is the priority enabled the tasks with the highest priority are fetched.</para>
	  /// 
	  /// <para>Returned tasks are locked for the given worker until
	  /// <code>now + lockDuration</code> expires.
	  /// Locked tasks cannot be fetched or completed by other workers. When the lock time has expired,
	  /// a task may be fetched and locked by other workers.</para>
	  /// 
	  /// <para>Returns at most <code>maxTasks</code> tasks. The tasks are arbitrarily
	  /// distributed among the specified topics. Example: Fetching 10 tasks of topics
	  /// "a"/"b"/"c" may return 3/3/4 tasks, or 10/0/0 tasks, etc.</para>
	  /// 
	  /// <para>May return less than <code>maxTasks</code> tasks, if there exist not enough
	  /// unlocked tasks matching the provided topics or if parallel fetching by other workers
	  /// results in locking failures.</para>
	  /// 
	  /// <para>
	  ///   Returns only tasks that the currently authenticated user has at least one
	  ///   permission out of all of the following groups for:
	  /// 
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#READ"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#READ_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="maxTasks"> the maximum number of tasks to return </param>
	  /// <param name="workerId"> the id of the worker to lock the tasks for </param>
	  /// <param name="usePriority"> the flag to enable the priority fetching mechanism </param>
	  /// <returns> a builder to define and execute an external task fetching operation </returns>
	  ExternalTaskQueryBuilder fetchAndLock(int maxTasks, string workerId, bool usePriority);

	  /// <summary>
	  /// <para>Completes an external task on behalf of a worker. The given task must be
	  /// assigned to the worker.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external to complete </param>
	  /// <param name="workerId"> the id of the worker that completes the task </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void complete(string externalTaskId, string workerId);

	  /// <summary>
	  /// <para>Completes an external task on behalf of a worker and submits variables
	  /// to the process instance before continuing execution. The given task must be
	  /// assigned to the worker.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external to complete </param>
	  /// <param name="workerId"> the id of the worker that completes the task </param>
	  /// <param name="variables"> a map of variables to set on the execution (non-local)
	  ///   the external task is assigned to
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void complete(string externalTaskId, string workerId, IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Completes an external task on behalf of a worker and submits variables
	  /// to the process instance before continuing execution. The given task must be
	  /// assigned to the worker.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external to complete </param>
	  /// <param name="workerId"> the id of the worker that completes the task </param>
	  /// <param name="variables"> a map of variables to set on the execution
	  ///   the external task is assigned to </param>
	  /// <param name="localVariables"> a map of variables to set on the execution locally
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void complete(string externalTaskId, string workerId, IDictionary<string, object> variables, IDictionary<string, object> localVariables);

	  /// <summary>
	  /// <para>Extends a lock of an external task on behalf of a worker.
	  /// The given task must be assigned to the worker.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task </param>
	  /// <param name="workerId"> the id of the worker that extends the lock of the task
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void extendLock(string externalTaskId, string workerId, long newLockDuration);

	  /// <summary>
	  /// <para>Signals that an external task could not be successfully executed.
	  /// The task must be assigned to the given worker. The number of retries left can be specified. In addition, a timeout can be
	  /// provided, such that the task cannot be fetched before <code>now + retryTimeout</code> again.</para>
	  /// 
	  /// <para>If <code>retries</code> is 0, an incident with the given error message is created. The incident gets resolved,
	  /// once the number of retries is increased again.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task to report a failure for </param>
	  /// <param name="workerId"> the id of the worker that reports the failure </param>
	  /// <param name="errorMessage"> short error message related to this failure. This message can be retrieved via
	  ///   <seealso cref="ExternalTask#getErrorMessage()"/> and is used as the incident message in case <code>retries</code> is <code>null</code>.
	  ///   May be <code>null</code>. </param>
	  /// <param name="retries"> the number of retries left. External tasks with 0 retries cannot be fetched anymore unless
	  ///   the number of retries is increased via API. Must be >= 0. </param>
	  /// <param name="retryTimeout"> the timeout before the task can be fetched again. Must be >= 0.
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void handleFailure(string externalTaskId, string workerId, string errorMessage, int retries, long retryTimeout);

	  /// <summary>
	  /// <para>Signals that an external task could not be successfully executed.
	  /// The task must be assigned to the given worker. The number of retries left can be specified. In addition, a timeout can be
	  /// provided, such that the task cannot be fetched before <code>now + retryTimeout</code> again.</para>
	  /// 
	  /// <para>If <code>retries</code> is 0, an incident with the given error message is created. The incident gets resolved,
	  /// once the number of retries is increased again.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task to report a failure for </param>
	  /// <param name="workerId"> the id of the worker that reports the failure </param>
	  /// <param name="errorMessage"> short error message related to this failure. This message can be retrieved via
	  ///   <seealso cref="ExternalTask#getErrorMessage()"/> and is used as the incident message in case <code>retries</code> is <code>null</code>.
	  ///   May be <code>null</code>. </param>
	  /// <param name="errorDetails"> full error message related to this failure. This message can be retrieved via
	  ///   <seealso cref="ExternalTaskService#getExternalTaskErrorDetails(String)"/> ()} </param>
	  /// <param name="retries"> the number of retries left. External tasks with 0 retries cannot be fetched anymore unless
	  ///   the number of retries is increased via API. Must be >= 0. </param>
	  /// <param name="retryTimeout"> the timeout before the task can be fetched again. Must be >= 0.
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void handleFailure(string externalTaskId, string workerId, string errorMessage, string errorDetails, int retries, long retryTimeout);

	  /// <summary>
	  /// <para>Signals that an business error appears, which should be handled by the process engine.
	  /// The task must be assigned to the given worker. The error will be propagated to the next error handler.
	  /// Is no existing error handler for the given bpmn error the activity instance of the external task
	  /// ends.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task to report a bpmn error </param>
	  /// <param name="workerId"> the id of the worker that reports the bpmn error </param>
	  /// <param name="errorCode"> the error code of the corresponding bmpn error
	  /// @since 7.5
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void handleBpmnError(string externalTaskId, string workerId, string errorCode);

	  /// <summary>
	  /// <para>Signals that an business error appears, which should be handled by the process engine.
	  /// The task must be assigned to the given worker. The error will be propagated to the next error handler.
	  /// Is no existing error handler for the given bpmn error the activity instance of the external task
	  /// ends.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task to report a bpmn error </param>
	  /// <param name="workerId"> the id of the worker that reports the bpmn error </param>
	  /// <param name="errorCode"> the error code of the corresponding bmpn error </param>
	  /// <param name="errorMessage"> the error message of the corresponding bmpn error
	  /// @since 7.10
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void handleBpmnError(string externalTaskId, string workerId, string errorCode, string errorMessage);


	  /// <summary>
	  /// <para>Signals that an business error appears, which should be handled by the process engine.
	  /// The task must be assigned to the given worker. The error will be propagated to the next error handler.
	  /// Is no existing error handler for the given bpmn error the activity instance of the external task
	  /// ends.</para>
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the external task to report a bpmn error </param>
	  /// <param name="workerId"> the id of the worker that reports the bpmn error </param>
	  /// <param name="errorCode"> the error code of the corresponding bmpn error </param>
	  /// <param name="errorMessage"> the error message of the corresponding bmpn error </param>
	  /// <param name="variables"> the variables to pass to the execution
	  /// @since 7.10
	  /// </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the task is assigned to a different worker </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void handleBpmnError(string externalTaskId, string workerId, string errorCode, string errorMessage, IDictionary<string, object> variables);

	  /// <summary>
	  /// Unlocks an external task instance.
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the task to unlock </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void unlock(string externalTaskId);

	  /// <summary>
	  /// Sets the retries for an external task. If the new value is 0, a new incident with a <code>null</code>
	  /// message is created. If the old value is 0 and the new value is greater than 0, an existing incident
	  /// is resolved.
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the task to set the </param>
	  /// <param name="retries"> </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void setRetries(string externalTaskId, int retries);

	  /// <summary>
	  /// Sets the retries for external tasks. If the new value is 0, a new incident with a <code>null</code>
	  /// message is created. If the old value is 0 and the new value is greater than 0, an existing incident
	  /// is resolved.
	  /// </summary>
	  /// <param name="externalTaskIds"> the ids of the tasks to set the </param>
	  /// <param name="retries"> </param>
	  /// <exception cref="NotFoundException"> if no external task with one of the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the ids are null or the number of retries is negative </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void setRetries(IList<string> externalTaskIds, int retries);

	  /// <summary>
	  /// Sets the retries for external tasks asynchronously as batch. The returned batch
	  /// can be used to track the progress. If the new value is 0, a new incident with a <code>null</code>
	  /// message is created. If the old value is 0 and the new value is greater than 0, an existing incident
	  /// is resolved.
	  /// 
	  /// </summary>
	  /// <returns> the batch
	  /// </returns>
	  /// <param name="externalTaskIds"> the ids of the tasks to set the </param>
	  /// <param name="retries"> </param>
	  /// <param name="externalTaskQuery"> a query which selects the external tasks to set the retries for. </param>
	  /// <exception cref="NotFoundException"> if no external task with one of the given id exists </exception>
	  /// <exception cref="BadUserRequestException"> if the ids are null or the number of retries is negative </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch setRetriesAsync(IList<string> externalTaskIds, ExternalTaskQuery externalTaskQuery, int retries);

	  /// <summary>
	  /// Sets the retries for external tasks using a fluent builder.
	  /// 
	  /// Specify the instances by calling one of the following methods, like
	  /// <i>externalTaskIds</i>. To set the retries call
	  /// <seealso cref="UpdateExternalTaskRetriesBuilder#set(int)"/> or
	  /// <seealso cref="UpdateExternalTaskRetriesBuilder#setAsync(int)"/>.
	  /// 
	  /// @since 7.8
	  /// </summary>
	  UpdateExternalTaskRetriesSelectBuilder updateRetries();

	  /// <summary>
	  /// Sets the priority for an external task.
	  /// </summary>
	  /// <param name="externalTaskId"> the id of the task to set the </param>
	  /// <param name="priority"> the new priority of the task </param>
	  /// <exception cref="NotFoundException"> if no external task with the given id exists </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#UPDATE"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#UPDATE_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul> </exception>
	  void setPriority(string externalTaskId, long priority);

	  /// <summary>
	  /// <para>
	  ///   Queries for tasks that the currently authenticated user has at least one
	  ///   of the following permissions for:
	  /// 
	  ///   <ul>
	  ///     <li><seealso cref="Permissions#READ"/> on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions#READ_INSTANCE"/> on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <returns> a new <seealso cref="ExternalTaskQuery"/> that can be used to dynamically
	  /// query for external tasks. </returns>
	  ExternalTaskQuery createExternalTaskQuery();

	  /// <summary>
	  /// Returns the full error details that occurred while running external task
	  /// with the given id. Returns null when the external task has no error details.
	  /// </summary>
	  /// <param name="externalTaskId"> id of the external task, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When no external task exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// @since 7.6 </exception>
	  string getExternalTaskErrorDetails(string externalTaskId);
	}

}