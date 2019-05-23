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
namespace org.camunda.bpm.engine.runtime
{

	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Allows programmatic querying of <seealso cref="Job"/>s.
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface JobQuery : Query<JobQuery, Job>
	{

	  /// <summary>
	  /// Only select jobs with the given id </summary>
	  JobQuery jobId(string jobId);

	  /// <summary>
	  /// Only select jobs which exist for the given job definition id. * </summary>
	  JobQuery jobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// Only select jobs which exist for the given process instance. * </summary>
	  JobQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select jobs which exist for the given process definition id. * </summary>
	  JobQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select jobs which exist for the given process definition key. * </summary>
	  JobQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select jobs which exist for the given execution </summary>
	  JobQuery executionId(string executionId);

	  /// <summary>
	  /// Only select jobs which are defined on an activity with the given id. * </summary>
	  JobQuery activityId(string activityId);

	  /// <summary>
	  /// Only select jobs which have retries left </summary>
	  JobQuery withRetriesLeft();

	  /// <summary>
	  /// Only select jobs which are executable,
	  /// ie. retries &gt; 0 and duedate is null or duedate is in the past *
	  /// </summary>
	  JobQuery executable();

	  /// <summary>
	  /// Only select jobs that are timers.
	  /// Cannot be used together with <seealso cref="messages()"/> 
	  /// </summary>
	  JobQuery timers();

	  /// <summary>
	  /// Only select jobs that are messages.
	  /// Cannot be used together with <seealso cref="timers()"/> 
	  /// </summary>
	  JobQuery messages();

	  /// <summary>
	  /// Only select jobs where the duedate is lower than the given date. </summary>
	  JobQuery duedateLowerThan(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then the given date. </summary>
	  JobQuery duedateHigherThan(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is lower then the given date.
	  /// @deprecated
	  /// </summary>
	  [Obsolete]
	  JobQuery duedateLowerThen(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is lower then or equals the given date.
	  /// @deprecated
	  /// </summary>
	  [Obsolete]
	  JobQuery duedateLowerThenOrEquals(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then the given date.
	  /// @deprecated
	  /// </summary>
	  [Obsolete]
	  JobQuery duedateHigherThen(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then or equals the given date.
	  /// @deprecated
	  /// </summary>
	  [Obsolete]
	  JobQuery duedateHigherThenOrEquals(DateTime date);

	  /// <summary>
	  /// Only select jobs created before the given date. </summary>
	  JobQuery createdBefore(DateTime date);

	  /// <summary>
	  /// Only select jobs created after the given date. </summary>
	  JobQuery createdAfter(DateTime date);

	  /// <summary>
	  /// Only select jobs with a priority that is higher than or equal to the given priority.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  JobQuery priorityHigherThanOrEquals(long priority);

	  /// <summary>
	  /// Only select jobs with a priority that is lower than or equal to the given priority.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  JobQuery priorityLowerThanOrEquals(long priority);

	  /// <summary>
	  /// Only select jobs that failed due to an exception. </summary>
	  JobQuery withException();

	  /// <summary>
	  /// Only select jobs that failed due to an exception with the given message. </summary>
	  JobQuery exceptionMessage(string exceptionMessage);

	  /// <summary>
	  /// Only select jobs which have no retries left </summary>
	  JobQuery noRetriesLeft();

	  /// <summary>
	  /// Only select jobs that are not suspended. </summary>
	  JobQuery active();

	  /// <summary>
	  /// Only select jobs that are suspended. </summary>
	  JobQuery suspended();

	  /// <summary>
	  /// Only select jobs that belong to one of the given tenant ids. </summary>
	  JobQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select jobs which have no tenant id. </summary>
	  JobQuery withoutTenantId();

	  /// <summary>
	  /// Select jobs which have no tenant id. Can be used in combination
	  /// with <seealso cref="tenantIdIn(string...)"/>.
	  /// </summary>
	  JobQuery includeJobsWithoutTenantId();

	  //sorting //////////////////////////////////////////

	  /// <summary>
	  /// Order by job id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByJobId();

	  /// <summary>
	  /// Order by duedate (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByJobDuedate();

	  /// <summary>
	  /// Order by retries (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByJobRetries();

	  /// <summary>
	  /// Order by priority for execution (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// 
	  /// @since 7.4
	  /// </summary>
	  JobQuery orderByJobPriority();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  JobQuery orderByExecutionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of job without tenant id is database-specific.
	  /// </summary>
	  JobQuery orderByTenantId();

	}

}