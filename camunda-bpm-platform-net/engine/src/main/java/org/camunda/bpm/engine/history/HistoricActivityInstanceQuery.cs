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

	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Programmatic querying for <seealso cref="HistoricActivityInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricActivityInstanceQuery : Query<HistoricActivityInstanceQuery, HistoricActivityInstance>
	{

	  /// <summary>
	  /// Only select historic activity instances with the given id (primary key within history tables). </summary>
	  HistoricActivityInstanceQuery activityInstanceId(string activityInstanceId);

	  /// <summary>
	  /// Only select historic activity instances with the given process instance.
	  /// {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricActivityInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic activity instances for the given process definition </summary>
	  HistoricActivityInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic activity instances for the given execution </summary>
	  HistoricActivityInstanceQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic activity instances for the given activity (id from BPMN 2.0 XML) </summary>
	  HistoricActivityInstanceQuery activityId(string activityId);

	  /// <summary>
	  /// Only select historic activity instances for activities with the given name </summary>
	  HistoricActivityInstanceQuery activityName(string activityName);

	  /// <summary>
	  /// Only select historic activity instances for activities with the given activity type </summary>
	  HistoricActivityInstanceQuery activityType(string activityType);

	  /// <summary>
	  /// Only select historic activity instances for userTask activities assigned to the given user </summary>
	  HistoricActivityInstanceQuery taskAssignee(string userId);

	  /// <summary>
	  /// Only select historic activity instances that are finished. </summary>
	  HistoricActivityInstanceQuery finished();

	  /// <summary>
	  /// Only select historic activity instances that are not finished yet. </summary>
	  HistoricActivityInstanceQuery unfinished();

	  /// <summary>
	  /// Only select historic activity instances that complete a BPMN scope </summary>
	  HistoricActivityInstanceQuery completeScope();

	  /// <summary>
	  /// Only select historic activity instances that got canceled </summary>
	  HistoricActivityInstanceQuery canceled();

	  /// <summary>
	  /// Only select historic activity instances that were started before the given date. </summary>
	  HistoricActivityInstanceQuery startedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic activity instances that were started after the given date. </summary>
	  HistoricActivityInstanceQuery startedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic activity instances that were started before the given date. </summary>
	  HistoricActivityInstanceQuery finishedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic activity instances that were started after the given date. </summary>
	  HistoricActivityInstanceQuery finishedAfter(DateTime date);

	  // ordering /////////////////////////////////////////////////////////////////
	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceId();

	  /// <summary>
	  /// Order by processInstanceId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by executionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByExecutionId();

	  /// <summary>
	  /// Order by activityId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityId();

	  /// <summary>
	  /// Order by activityName (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityName();

	  /// <summary>
	  /// Order by activityType (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityType();

	  /// <summary>
	  /// Order by start (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceStartTime();

	  /// <summary>
	  /// Order by end (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceEndTime();

	  /// <summary>
	  /// Order by duration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceDuration();

	  /// <summary>
	  /// Order by processDefinitionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// <para>Sort the <seealso cref="HistoricActivityInstance activity instances"/> in the order in which
	  /// they occurred (ie. started) and needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>.</para>
	  /// 
	  /// <para>The set of all <seealso cref="HistoricActivityInstance activity instances"/> is a <strong>partially
	  /// ordered set</strong>. At a BPMN level this means that instances of concurrent activities (example:
	  /// activities on different parallel branched after a parallel gateway) cannot be compared to each other.
	  /// Instances of activities which are part of happens-before relation at the BPMN level will be ordered
	  /// in respect to that relation.</para>
	  /// 
	  /// <para>Technically this means that <seealso cref="HistoricActivityInstance activity instances"/>
	  /// with different <seealso cref="HistoricActivityInstance#getExecutionId() execution ids"/> are
	  /// <strong>incomparable</strong>. Only <seealso cref="HistoricActivityInstance activity instances"/> with
	  /// the same <seealso cref="HistoricActivityInstance#getExecutionId() execution id"/> can be <strong>totally
	  /// ordered</strong> by using <seealso cref="#executionId(String)"/> and <seealso cref="#orderPartiallyByOccurrence()"/>
	  /// which will return a result set ordered by its occurrence.</para>
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoricActivityInstanceQuery orderPartiallyByOccurrence();

	  /// <summary>
	  /// Only select historic activity instances with one of the given tenant ids. </summary>
	  HistoricActivityInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic activity instances without tenant id is database-specific.
	  /// </summary>
	  HistoricActivityInstanceQuery orderByTenantId();

	}

}