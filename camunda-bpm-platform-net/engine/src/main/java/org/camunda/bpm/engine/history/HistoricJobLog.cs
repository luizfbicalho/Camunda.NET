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

	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// <para>The <seealso cref="HistoricJobLog"/> is used to have a log containing
	/// information about <seealso cref="Job job"/> execution. The log provides
	/// details about the complete lifecycle of a <seealso cref="Job job"/>:</para>
	/// <ul>
	///   <li>job created</li>
	///   <li>job execution failed</li>
	///   <li>job execution successful</li>
	///   <li>job was deleted</li>
	/// </ul>
	/// 
	/// An instance of <seealso cref="HistoricJobLog"/> represents a state in
	/// the lifecycle of a <seealso cref="Job job"/>.
	/// 
	/// @author Roman Smirnov
	/// 
	/// @since 7.3
	/// </summary>
	public interface HistoricJobLog
	{

	  /// <summary>
	  /// Returns the unique identifier for <code>this</code> historic job log.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// Returns the time when <code>this</code> log occurred.
	  /// </summary>
	  DateTime Timestamp {get;}

	  /// <summary>
	  /// Returns the id of the associated job.
	  /// </summary>
	  string JobId {get;}

	  /// <summary>
	  /// Returns the due date of the associated job when <code>this</code> log occurred.
	  /// </summary>
	  DateTime JobDueDate {get;}

	  /// <summary>
	  /// Returns the retries of the associated job before the associated job has
	  /// been executed and when <code>this</code> log occurred.
	  /// </summary>
	  int JobRetries {get;}

	  /// <summary>
	  /// Returns the priority of the associated job when <code>this</code> log entry was created.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  long JobPriority {get;}

	  /// <summary>
	  /// Returns the message of the exception that occurred by executing the associated job.
	  /// 
	  /// To get the full exception stacktrace,
	  /// use <seealso cref="HistoryService.getHistoricJobLogExceptionStacktrace(string)"/>
	  /// </summary>
	  string JobExceptionMessage {get;}

	  /// <summary>
	  /// Returns the id of the job definition on which the associated job was created.
	  /// </summary>
	  string JobDefinitionId {get;}

	  /// <summary>
	  /// Returns the job definition type of the associated job.
	  /// </summary>
	  string JobDefinitionType {get;}

	  /// <summary>
	  /// Returns the job definition configuration type of the associated job.
	  /// </summary>
	  string JobDefinitionConfiguration {get;}

	  /// <summary>
	  /// Returns the id of the activity on which the associated job was created.
	  /// </summary>
	  string ActivityId {get;}

	  /// <summary>
	  /// Returns the specific execution id on which the associated job was created.
	  /// </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Returns the specific root process instance id of the process instance
	  /// on which the associated job was created.
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the specific process instance id on which the associated job was created.
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the specific process definition id on which the associated job was created.
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Returns the specific process definition key on which the associated job was created.
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Returns the specific deployment id on which the associated job was created.
	  /// </summary>
	  string DeploymentId {get;}

	  /// <summary>
	  /// Returns the id of the tenant this job log entry belongs to. Can be <code>null</code>
	  /// if the job log entry belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the creation of the associated job.
	  /// </summary>
	  bool CreationLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the failed execution of the associated job.
	  /// </summary>
	  bool FailureLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the successful execution of the associated job.
	  /// </summary>
	  bool SuccessLog {get;}

	  /// <summary>
	  /// Returns <code>true</code> when <code>this</code> log represents
	  /// the deletion of the associated job.
	  /// </summary>
	  bool DeletionLog {get;}

	  /// <summary>
	  /// The time the historic job log will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}