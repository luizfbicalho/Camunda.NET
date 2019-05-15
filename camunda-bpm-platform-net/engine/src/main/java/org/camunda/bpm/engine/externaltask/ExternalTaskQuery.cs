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
namespace org.camunda.bpm.engine.externaltask
{

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public interface ExternalTaskQuery : Query<ExternalTaskQuery, ExternalTask>
	{

	  /// <summary>
	  /// Only select the external task with the given id
	  /// </summary>
	  ExternalTaskQuery externalTaskId(string externalTaskId);

	  /// <summary>
	  /// Only select external tasks that was most recently locked by the given worker
	  /// </summary>
	  ExternalTaskQuery workerId(string workerId);

	  /// <summary>
	  /// Only select external tasks that have a lock expiring before the given date
	  /// </summary>
	  ExternalTaskQuery lockExpirationBefore(DateTime lockExpirationDate);

	  /// <summary>
	  /// Only select external tasks that have a lock expiring after the given date
	  /// </summary>
	  ExternalTaskQuery lockExpirationAfter(DateTime lockExpirationDate);

	  /// <summary>
	  /// Only select external tasks of the given topic
	  /// </summary>
	  ExternalTaskQuery topicName(string topicName);

	  /// <summary>
	  /// Only select external tasks that are currently locked, i.e. that have a lock expiration
	  /// time that is in the future
	  /// </summary>
	  ExternalTaskQuery locked();

	  /// <summary>
	  /// Only select external tasks that are not currently locked, i.e. that have no
	  /// lock expiration time or one that is overdue
	  /// </summary>
	  ExternalTaskQuery notLocked();

	  /// <summary>
	  /// Only select external tasks created in the context of the given execution
	  /// </summary>
	  ExternalTaskQuery executionId(string executionId);

	  /// <summary>
	  /// Only select external tasks created in the context of the given process instance
	  /// </summary>
	  ExternalTaskQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select external tasks created in the context of the given process instances
	  /// </summary>
	  ExternalTaskQuery processInstanceIdIn(params string[] processInstanceIdIn);

	  /// <summary>
	  /// Only select external tasks that belong to an instance of the given process definition
	  /// </summary>
	  ExternalTaskQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select external tasks that belong to an instance of the given activity
	  /// </summary>
	  ExternalTaskQuery activityId(string activityId);

	  /// <summary>
	  /// Only select external tasks that belong to an instances of the given activities.
	  /// </summary>
	  ExternalTaskQuery activityIdIn(params string[] activityIdIn);

	  /// <summary>
	  /// Only select external tasks with a priority that is higher than or equal to the given priority.
	  /// 
	  /// @since 7.5 </summary>
	  /// <param name="priority"> the priority which is used for the query </param>
	  /// <returns> the builded external task query </returns>
	  ExternalTaskQuery priorityHigherThanOrEquals(long priority);

	  /// <summary>
	  /// Only select external tasks with a priority that is lower than or equal to the given priority.
	  /// 
	  /// @since 7.5 </summary>
	  /// <param name="priority"> the priority which is used for the query </param>
	  /// <returns> the builded external task query </returns>
	  ExternalTaskQuery priorityLowerThanOrEquals(long priority);

	  /// <summary>
	  /// Only select external tasks that are currently suspended
	  /// </summary>
	  ExternalTaskQuery suspended();

	  /// <summary>
	  /// Only select external tasks that are currently not suspended
	  /// </summary>
	  ExternalTaskQuery active();

	  /// <summary>
	  /// Only select external tasks that have retries > 0
	  /// </summary>
	  ExternalTaskQuery withRetriesLeft();

	  /// <summary>
	  /// Only select external tasks that have retries = 0
	  /// </summary>
	  ExternalTaskQuery noRetriesLeft();

	  /// <summary>
	  /// Only select external tasks that belong to one of the given tenant ids. </summary>
	  ExternalTaskQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Order by external task id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  ExternalTaskQuery orderById();

	  /// <summary>
	  /// Order by lock expiration time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Ordering of tasks with no lock expiration time is database-dependent.
	  /// </summary>
	  ExternalTaskQuery orderByLockExpirationTime();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  ExternalTaskQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  ExternalTaskQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  ExternalTaskQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of external tasks without tenant id is database-specific.
	  /// </summary>
	  ExternalTaskQuery orderByTenantId();

	  /// <summary>
	  /// Order by priority (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// @since 7.5
	  /// </summary>
	  ExternalTaskQuery orderByPriority();

	}

}