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

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricJobLogQuery : Query<HistoricJobLogQuery, HistoricJobLog>
	{

	  /// <summary>
	  /// Only select historic job log entries with the id. </summary>
	  HistoricJobLogQuery logId(string logId);

	  /// <summary>
	  /// Only select historic job log entries with the given job id. </summary>
	  HistoricJobLogQuery jobId(string jobId);

	  /// <summary>
	  /// Only select historic job log entries with the given exception message. </summary>
	  HistoricJobLogQuery jobExceptionMessage(string exceptionMessage);

	  /// <summary>
	  /// Only select historic job log entries with the given job definition id. </summary>
	  HistoricJobLogQuery jobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// Only select historic job log entries with the given job definition type. </summary>
	  HistoricJobLogQuery jobDefinitionType(string jobDefinitionType);

	  /// <summary>
	  /// Only select historic job log entries with the given job definition configuration type. </summary>
	  HistoricJobLogQuery jobDefinitionConfiguration(string jobDefinitionConfiguration);

	  /// <summary>
	  /// Only select historic job log entries which are associated with one of the given activity ids. * </summary>
	  HistoricJobLogQuery activityIdIn(params string[] activityIds);

	  /// <summary>
	  /// Only select historic job log entries which are associated with one of the given execution ids. * </summary>
	  HistoricJobLogQuery executionIdIn(params string[] executionIds);

	  /// <summary>
	  /// Only select historic job log entries with the process instance id. </summary>
	  HistoricJobLogQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic job log entries with the process definition id. </summary>
	  HistoricJobLogQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic job log entries with the process instance key. </summary>
	  HistoricJobLogQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic job log entries with the deployment id. </summary>
	  HistoricJobLogQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select historic job log entries that belong to one of the given tenant ids. </summary>
	  HistoricJobLogQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select log entries where the job had a priority higher than or
	  /// equal to the given priority.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  HistoricJobLogQuery jobPriorityHigherThanOrEquals(long priority);

	  /// <summary>
	  /// Only select log entries where the job had a priority lower than or
	  /// equal to the given priority.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  HistoricJobLogQuery jobPriorityLowerThanOrEquals(long priority);

	  /// <summary>
	  /// Only select created historic job log entries. </summary>
	  HistoricJobLogQuery creationLog();

	  /// <summary>
	  /// Only select failed historic job log entries. </summary>
	  HistoricJobLogQuery failureLog();

	  /// <summary>
	  /// Only select historic job logs which belongs to a
	  /// <code>successful</code> executed job.
	  /// </summary>
	  HistoricJobLogQuery successLog();

	  /// <summary>
	  /// Only select deleted historic job log entries. </summary>
	  HistoricJobLogQuery deletionLog();

	  /// <summary>
	  /// Order by timestamp (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByTimestamp();

	  /// <summary>
	  /// Order by job id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByJobId();

	  /// <summary>
	  /// Order by job due date (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByJobDueDate();

	  /// <summary>
	  /// Order by job retries (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByJobRetries();

	  /// <summary>
	  /// Order by job priority (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// 
	  /// @since 7.4
	  /// </summary>
	  HistoricJobLogQuery orderByJobPriority();

	  /// <summary>
	  /// Order by job definition id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByJobDefinitionId();

	  /// <summary>
	  /// Order by activity id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByActivityId();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByExecutionId();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  HistoricJobLogQuery orderByDeploymentId();


	  /// <summary>
	  /// <para>Sort the <seealso cref="HistoricJobLog historic job logs"/> in the order in which
	  /// they occurred and needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>.</para>
	  /// 
	  /// <para>The set of all <seealso cref="HistoricJobLog historic job logs"/> is a <strong>partially ordered
	  /// set</strong>. Due to this fact <seealso cref="HistoricJobLog historic job logs"/> with different
	  /// <seealso cref="HistoricJobLog.getJobId() job ids"/> are <strong>incomparable</strong>. Only {@link
	  /// HistoricJobLog historic job logs} with the same <seealso cref="HistoricJobLog.getJobId() job id"/> can
	  /// be <strong>totally ordered</strong> by using <seealso cref="jobId(string)"/> and <seealso cref="orderPartiallyByOccurrence()"/>
	  /// which will return a result set ordered by its occurrence.</para>
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoricJobLogQuery orderPartiallyByOccurrence();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of job log entries without tenant id is database-specific.
	  /// </summary>
	  HistoricJobLogQuery orderByTenantId();

	}

}