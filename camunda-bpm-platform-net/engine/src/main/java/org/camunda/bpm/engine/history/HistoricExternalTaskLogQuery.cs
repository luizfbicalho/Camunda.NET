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

	public interface HistoricExternalTaskLogQuery : Query<HistoricExternalTaskLogQuery, HistoricExternalTaskLog>
	{

	  /// <summary>
	  /// Only select historic external task log entries with the id. </summary>
	  HistoricExternalTaskLogQuery logId(string historicExternalTaskLogId);

	  /// <summary>
	  /// Only select historic external task log entries with the given external task id. </summary>
	  HistoricExternalTaskLogQuery externalTaskId(string taskId);

	  /// <summary>
	  /// Only select historic external task log entries with the given topic name. </summary>
	  HistoricExternalTaskLogQuery topicName(string topicName);

	  /// <summary>
	  /// Only select historic external task log entries with the given worker id. </summary>
	  HistoricExternalTaskLogQuery workerId(string workerId);

	  /// <summary>
	  /// Only select historic external task log entries with the given error message. </summary>
	  HistoricExternalTaskLogQuery errorMessage(string errorMessage);

	  /// <summary>
	  /// Only select historic external task log entries which are associated with one of the given activity ids. * </summary>
	  HistoricExternalTaskLogQuery activityIdIn(params string[] activityIds);

	  /// <summary>
	  /// Only select historic external task log entries which are associated with one of the given activity instance ids. * </summary>
	  HistoricExternalTaskLogQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select historic external task log entries which are associated with one of the given execution ids. * </summary>
	  HistoricExternalTaskLogQuery executionIdIn(params string[] executionIds);

	  /// <summary>
	  /// Only select historic external task log entries with the process instance id. </summary>
	  HistoricExternalTaskLogQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic external task log entries with the process definition id. </summary>
	  HistoricExternalTaskLogQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic external task log entries with the process instance key. </summary>
	  HistoricExternalTaskLogQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic external task log entries that belong to one of the given tenant ids. </summary>
	  HistoricExternalTaskLogQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select log entries where the external task had a priority higher than or
	  /// equal to the given priority.
	  /// </summary>
	  HistoricExternalTaskLogQuery priorityHigherThanOrEquals(long priority);

	  /// <summary>
	  /// Only select log entries where the external task had a priority lower than or
	  /// equal to the given priority.
	  /// </summary>
	  HistoricExternalTaskLogQuery priorityLowerThanOrEquals(long priority);

	  /// <summary>
	  /// Only select created historic external task log entries. </summary>
	  HistoricExternalTaskLogQuery creationLog();

	  /// <summary>
	  /// Only select failed historic external task log entries. </summary>
	  HistoricExternalTaskLogQuery failureLog();

	  /// <summary>
	  /// Only select successful historic external task log entries. </summary>
	  HistoricExternalTaskLogQuery successLog();

	  /// <summary>
	  /// Only select deleted historic external task log entries. </summary>
	  HistoricExternalTaskLogQuery deletionLog();


	  /// <summary>
	  /// Order by timestamp (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByTimestamp();

	  /// <summary>
	  /// Order by external task id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByExternalTaskId();

	  /// <summary>
	  /// Order by external task retries (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByRetries();

	  /// <summary>
	  /// Order by external task priority (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  HistoricExternalTaskLogQuery orderByPriority();

	  /// <summary>
	  /// Order by topic name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByTopicName();

	  /// <summary>
	  /// Order by worker id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByWorkerId();

	  /// <summary>
	  /// Order by activity id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByActivityId();

	  /// <summary>
	  /// Order by activity instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByActivityInstanceId();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByExecutionId();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricExternalTaskLogQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of external task log entries without tenant id is database-specific.
	  /// </summary>
	  HistoricExternalTaskLogQuery orderByTenantId();
	}

}