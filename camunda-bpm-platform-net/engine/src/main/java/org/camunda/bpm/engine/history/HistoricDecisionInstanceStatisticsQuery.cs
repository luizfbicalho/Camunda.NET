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
	/// @author Askar Akhmerov
	/// </summary>
	public interface HistoricDecisionInstanceStatisticsQuery : Query<HistoricDecisionInstanceStatisticsQuery, HistoricDecisionInstanceStatistics>
	{

	  /// <summary>
	  /// Restrict query results to be based only on specified decision instance id
	  /// </summary>
	  /// <param name="decisionInstanceId"> - decision instance based on which constraint will be applied </param>
	  /// <returns> - query object to execute </returns>
	  HistoricDecisionInstanceStatisticsQuery decisionInstanceId(string decisionInstanceId);
	}

}