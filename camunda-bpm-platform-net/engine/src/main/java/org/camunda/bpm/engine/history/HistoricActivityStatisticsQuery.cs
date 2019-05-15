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

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricActivityStatisticsQuery : Query<HistoricActivityStatisticsQuery, HistoricActivityStatistics>
	{

	  /// <summary>
	  /// Include an aggregation of finished instances in the result.
	  /// </summary>
	  HistoricActivityStatisticsQuery includeFinished();

	  /// <summary>
	  /// Include an aggregation of canceled instances in the result.
	  /// </summary>
	  HistoricActivityStatisticsQuery includeCanceled();

	  /// <summary>
	  /// Include an aggregation of instances, which complete a scope (ie. in bpmn manner: an activity
	  /// which consumed a token and did not produced a new one), in the result.
	  /// </summary>
	  HistoricActivityStatisticsQuery includeCompleteScope();

	  /// <summary>
	  /// Only select historic activities of process instances that were started before the given date. </summary>
	  HistoricActivityStatisticsQuery startedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic activities of process instances that were started after the given date. </summary>
	  HistoricActivityStatisticsQuery startedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic activities of process instances that were finished before the given date. </summary>
	  HistoricActivityStatisticsQuery finishedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic activities of process instances that were finished after the given date. </summary>
	  HistoricActivityStatisticsQuery finishedAfter(DateTime date);

	  /// <summary>
	  /// Order by activity id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// </summary>
	  HistoricActivityStatisticsQuery orderByActivityId();

	}

}