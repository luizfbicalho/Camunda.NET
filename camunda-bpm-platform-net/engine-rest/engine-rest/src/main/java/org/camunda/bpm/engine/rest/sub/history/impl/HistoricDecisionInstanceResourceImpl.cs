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
namespace org.camunda.bpm.engine.rest.sub.history.impl
{

	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricDecisionInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricDecisionInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricDecisionInstanceResourceImpl : HistoricDecisionInstanceResource
	{

	  private ProcessEngine engine;
	  private string decisionInstanceId;

	  public HistoricDecisionInstanceResourceImpl(ProcessEngine engine, string decisionInstanceId)
	  {
		this.engine = engine;
		this.decisionInstanceId = decisionInstanceId;
	  }

	  public virtual HistoricDecisionInstanceDto getHistoricDecisionInstance(bool? includeInputs, bool? includeOutputs, bool? disableBinaryFetching, bool? disableCustomObjectDeserialization)
	  {
		HistoryService historyService = engine.HistoryService;

		HistoricDecisionInstanceQuery query = historyService.createHistoricDecisionInstanceQuery().decisionInstanceId(decisionInstanceId);
		if (includeInputs != null && includeInputs)
		{
		  query.includeInputs();
		}
		if (includeOutputs != null && includeOutputs)
		{
		  query.includeOutputs();
		}
		if (disableBinaryFetching != null && disableBinaryFetching)
		{
		  query.disableBinaryFetching();
		}
		if (disableCustomObjectDeserialization != null && disableCustomObjectDeserialization)
		{
		  query.disableCustomObjectDeserialization();
		}

		HistoricDecisionInstance instance = query.singleResult();

		if (instance == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Historic decision instance with id '" + decisionInstanceId + "' does not exist");
		}

		return HistoricDecisionInstanceDto.fromHistoricDecisionInstance(instance);
	  }
	}

}