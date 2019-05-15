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
namespace org.camunda.bpm.engine.batch
{

	/// <summary>
	/// <para>A batch represents a number of jobs which
	/// execute a number of commands asynchronously.
	/// </para>
	/// <para>
	/// </para>
	/// <para>Batches have three types of jobs:
	/// <ul>
	/// <li>Seed jobs: Create execution jobs
	/// <li>Execution jobs: Execute the actual action
	/// <li>Monitor jobs: Manage the batch once all execution jobs have been created
	/// (e.g. responsible for deletion of the Batch after completion).
	/// </ul>
	/// </para>
	/// <para>
	/// </para>
	/// <para>All three job types have independent job definitions. They can be controlled individually
	/// (e.g. suspension) and are independently represented in the historic job log.
	/// </para>
	/// </summary>
	public interface Batch
	{

	  /// <returns> the id of the batch </returns>
	  string Id {get;}

	  /// <returns> the type of the batch </returns>
	  string Type {get;}

	  /// <returns> the number of batch execution jobs required to complete the batch </returns>
	  int TotalJobs {get;}

	  /// <returns> the number of batch execution jobs already created by the seed job </returns>
	  int JobsCreated {get;}

	  /// <returns> number of batch jobs created per batch seed job invocation </returns>
	  int BatchJobsPerSeed {get;}

	  /// <returns> the number of invocations executed per batch job </returns>
	  int InvocationsPerBatchJob {get;}

	  /// <returns> the id of the batch seed job definition </returns>
	  string SeedJobDefinitionId {get;}

	  /// <returns> the id of the batch monitor job definition </returns>
	  string MonitorJobDefinitionId {get;}

	  /// <returns> the id of the batch job definition </returns>
	  string BatchJobDefinitionId {get;}

	  /// <returns> the batch's tenant id or null </returns>
	  string TenantId {get;}

	  /// <returns> the batch creator's user id </returns>
	  string CreateUserId {get;}

	  /// <summary>
	  /// <para>
	  /// Indicates whether this batch is suspended. If a batch is suspended,
	  /// the batch jobs will not be acquired by the job executor.
	  /// </para>
	  /// <para>
	  /// </para>
	  /// <para>
	  /// <strong>Note:</strong> It is still possible to manually suspend and activate
	  /// jobs and job definitions using the <seealso cref="ManagementService"/>, which will
	  /// not change the suspension state of the batch.
	  /// </para>
	  /// </summary>
	  /// <returns> true if this batch is currently suspended, false otherwise </returns>
	  /// <seealso cref= ManagementService#suspendBatchById(String) </seealso>
	  /// <seealso cref= ManagementService#activateBatchById(String) </seealso>
	  bool Suspended {get;}

	}

	public static class Batch_Fields
	{
	  public const string TYPE_PROCESS_INSTANCE_MIGRATION = "instance-migration";
	  public const string TYPE_PROCESS_INSTANCE_MODIFICATION = "instance-modification";
	  public const string TYPE_PROCESS_INSTANCE_RESTART = "instance-restart";
	  public const string TYPE_PROCESS_INSTANCE_DELETION = "instance-deletion";
	  public const string TYPE_PROCESS_INSTANCE_UPDATE_SUSPENSION_STATE = "instance-update-suspension-state";
	  public const string TYPE_HISTORIC_PROCESS_INSTANCE_DELETION = "historic-instance-deletion";
	  public const string TYPE_HISTORIC_DECISION_INSTANCE_DELETION = "historic-decision-instance-deletion";
	  public const string TYPE_SET_JOB_RETRIES = "set-job-retries";
	  public const string TYPE_SET_EXTERNAL_TASK_RETRIES = "set-external-task-retries";
	  public const string TYPE_PROCESS_SET_REMOVAL_TIME = "process-set-removal-time";
	  public const string TYPE_DECISION_SET_REMOVAL_TIME = "decision-set-removal-time";
	  public const string TYPE_BATCH_SET_REMOVAL_TIME = "batch-set-removal-time";
	}

}