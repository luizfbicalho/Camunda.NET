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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;

	/// <summary>
	/// <para>The <seealso cref="HistoricExternalTaskLog"/> is used to have a log containing
	/// information about <seealso cref="ExternalTask task"/> execution. The log provides
	/// details about the last lifecycle state of a <seealso cref="ExternalTask task"/>:</para>
	/// 
	/// An instance of <seealso cref="HistoricExternalTaskLog"/> represents the latest historic
	/// state in the lifecycle of a <seealso cref="ExternalTask task"/>.
	/// 
	/// @since 7.7
	/// </summary>
	public interface HistoricExternalTaskLog
	{

	  /// <summary>
	  /// Returns the unique identifier for <code>this</code> historic external task log.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// Returns the time when <code>this</code> log occurred.
	  /// </summary>
	  DateTime Timestamp {get;}

	  /// <summary>
	  /// Returns the id of the associated external task.
	  /// </summary>
	  string ExternalTaskId {get;}

	  /// <summary>
	  /// Returns the retries of the associated external task before the associated external task has
	  /// been executed and when <code>this</code> log occurred.
	  /// </summary>
	  int? Retries {get;}

	  /// <summary>
	  /// Returns the priority of the associated external task when <code>this</code> log entry was created.
	  /// </summary>
	  long Priority {get;}

	  /// <summary>
	  /// Returns the topic name of the associated external task.
	  /// </summary>
	  string TopicName {get;}

	  /// <summary>
	  /// Returns the id of the worker that fetched the external task most recently.
	  /// </summary>
	  string WorkerId {get;}

	  /// <summary>
	  /// Returns the message of the error that occurred by executing the associated external task.
	  /// 
	  /// To get the full error details,
	  /// use <seealso cref="HistoryService.getHistoricExternalTaskLogErrorDetails(string)"/>
	  /// </summary>
	  string ErrorMessage {get;}

	  /// <summary>
	  /// Returns the id of the activity which the external task associated with.
	  /// </summary>
	  string ActivityId {get;}

	  /// <summary>
	  /// Returns the id of the activity instance on which the associated external task was created.
	  /// </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// Returns the specific execution id on which the associated external task was created.
	  /// </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Returns the specific root process instance id of the process instance
	  /// on which the associated external task was created.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the specific process instance id on which the associated external task was created.
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the specific process definition id on which the associated external task was created.
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Returns the specific process definition key on which the associated external task was created.
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Returns the id of the tenant this external task log entry belongs to. Can be <code>null</code>
	  /// if the external task log entry belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the creation of the associated external task.
	  /// </summary>
	  bool CreationLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the failed execution of the associated external task.
	  /// </summary>
	  bool FailureLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the successful execution of the associated external task.
	  /// </summary>
	  bool SuccessLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the deletion of the associated external task.
	  /// </summary>
	  bool DeletionLog {get;}

	  /// <summary>
	  /// The time the historic external task log will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}