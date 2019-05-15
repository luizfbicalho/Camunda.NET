using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.impl.history
{
	using HistoricDecisionInstanceStatistics = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatistics;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using HistoricDecisionInstanceStatisticsDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceStatisticsDto;
	using HistoricDecisionStatisticsRestService = org.camunda.bpm.engine.rest.history.HistoricDecisionStatisticsRestService;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class HistoricDecisionStatisticsRestServiceImpl : HistoricDecisionStatisticsRestService
	{

	  protected internal ProcessEngine processEngine;

	  public HistoricDecisionStatisticsRestServiceImpl(ProcessEngine processEngine)
	  {
		this.processEngine = processEngine;
	  }

	  public virtual IList<HistoricDecisionInstanceStatisticsDto> getDecisionStatistics(string decisionRequirementsDefinitionId, string decisionInstanceId)
	  {
		IList<HistoricDecisionInstanceStatisticsDto> result = new List<HistoricDecisionInstanceStatisticsDto>();
		HistoricDecisionInstanceStatisticsQuery statisticsQuery = processEngine.HistoryService.createHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinitionId);
		if (!string.ReferenceEquals(decisionInstanceId, null))
		{
		  statisticsQuery.decisionInstanceId(decisionInstanceId);
		}

		foreach (HistoricDecisionInstanceStatistics stats in statisticsQuery.list())
		{
		  result.Add(HistoricDecisionInstanceStatisticsDto.fromDecisionDefinitionStatistics(stats));
		}

		return result;
	  }

	}

}