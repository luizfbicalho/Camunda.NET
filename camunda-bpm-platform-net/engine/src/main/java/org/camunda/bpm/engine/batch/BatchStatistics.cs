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
	/// <para>
	///  Additional statistics for a single batch.
	/// </para>
	/// 
	/// <para>
	///   Contains the number of remaining jobs, completed and failed batch
	///   execution jobs. The following relation between these exists:
	/// 
	///   <code>
	///     batch total jobs = remaining jobs + completed jobs
	///   </code>
	/// </para>
	/// </summary>
	public interface BatchStatistics : Batch
	{
	  /// <summary>
	  /// <para>
	  ///   The number of remaining batch execution jobs.
	  ///   This does include failed batch execution jobs and
	  ///   batch execution jobs which still have to be created by the seed job.
	  /// </para>
	  /// 
	  /// <para>
	  ///   See
	  ///   <seealso cref="getTotalJobs()"/> for the number of all batch execution jobs,
	  ///   <seealso cref="getCompletedJobs()"/> for the number of completed batch execution jobs and
	  ///   <seealso cref="getFailedJobs()"/> for the number of failed batch execution jobs.
	  /// </para>
	  /// </summary>
	  /// <returns> the number of remaining batch execution jobs </returns>
	  int RemainingJobs {get;}

	  /// <summary>
	  /// <para>
	  ///   The number of completed batch execution jobs.
	  ///   This does include aborted/deleted batch execution jobs.
	  /// </para>
	  /// 
	  /// <para>
	  ///   See
	  ///   <seealso cref="getTotalJobs()"/> for the number of all batch execution jobs,
	  ///   <seealso cref="getRemainingJobs()"/> ()} for the number of remaining batch execution jobs and
	  ///   <seealso cref="getFailedJobs()"/> for the number of failed batch execution jobs.
	  /// </para>
	  /// </summary>
	  /// <returns> the number of completed batch execution jobs </returns>
	  int CompletedJobs {get;}

	  /// <summary>
	  /// <para>
	  ///   The number of failed batch execution jobs.
	  ///   This does not include aborted or deleted batch execution jobs.
	  /// </para>
	  /// 
	  /// <para>
	  ///   See
	  ///   <seealso cref="getTotalJobs()"/> for the number of all batch execution jobs,
	  ///   <seealso cref="getRemainingJobs()"/> ()} for the number of remaining batch execution jobs and
	  ///   <seealso cref="getCompletedJobs()"/> ()} for the number of completed batch execution jobs.
	  /// </para>
	  /// </summary>
	  /// <returns> the number of failed batch execution jobs </returns>
	  int FailedJobs {get;}

	}

}