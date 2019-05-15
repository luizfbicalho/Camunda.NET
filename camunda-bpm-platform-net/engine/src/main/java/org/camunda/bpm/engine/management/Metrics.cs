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
namespace org.camunda.bpm.engine.management
{
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Metrics
	{

	  public const string ACTIVTY_INSTANCE_START = "activity-instance-start";
	  public const string ACTIVTY_INSTANCE_END = "activity-instance-end";

	  /// <summary>
	  /// Number of times job acqusition is performed
	  /// </summary>
	  public const string JOB_ACQUISITION_ATTEMPT = "job-acquisition-attempt";

	  /// <summary>
	  /// Number of jobs successfully acquired (i.e. selected + locked)
	  /// </summary>
	  public const string JOB_ACQUIRED_SUCCESS = "job-acquired-success";
	  /// <summary>
	  /// Number of jobs attempted to acquire but with failure (i.e. selected + lock failed)
	  /// </summary>
	  public const string JOB_ACQUIRED_FAILURE = "job-acquired-failure";

	  /// <summary>
	  /// Number of jobs that were submitted for execution but were rejected due to
	  /// resource shortage. In the default job executor, this is the case when
	  /// the execution queue is full.
	  /// </summary>
	  public const string JOB_EXECUTION_REJECTED = "job-execution-rejected";

	  public const string JOB_SUCCESSFUL = "job-successful";
	  public const string JOB_FAILED = "job-failed";

	  /// <summary>
	  /// Number of jobs that are immediately locked and executed because they are exclusive
	  /// and created in the context of job execution
	  /// </summary>
	  public const string JOB_LOCKED_EXCLUSIVE = "job-locked-exclusive";

	  /// <summary>
	  /// Number of executed decision elements in the DMN engine.
	  /// </summary>
	  public const string EXECUTED_DECISION_ELEMENTS = "executed-decision-elements";

	  /// <summary>
	  /// Number of instances removed by history cleanup.
	  /// </summary>
	  public const string HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES = "history-cleanup-removed-process-instances";
	  public const string HISTORY_CLEANUP_REMOVED_CASE_INSTANCES = "history-cleanup-removed-case-instances";
	  public const string HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES = "history-cleanup-removed-decision-instances";
	  public const string HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS = "history-cleanup-removed-batch-operations";
	}

}