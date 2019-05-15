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
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricActivityInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricActivityInstanceResourceImpl : HistoricActivityInstanceResource
	{

	  private ProcessEngine engine;
	  private string activityInstanceId;

	  public HistoricActivityInstanceResourceImpl(ProcessEngine engine, string activityInstanceId)
	  {
		this.engine = engine;
		this.activityInstanceId = activityInstanceId;
	  }

	  public virtual HistoricActivityInstanceDto HistoricActivityInstance
	  {
		  get
		  {
			HistoryService historyService = engine.HistoryService;
			HistoricActivityInstance instance = historyService.createHistoricActivityInstanceQuery().activityInstanceId(activityInstanceId).singleResult();
    
			if (instance == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Historic activity instance with id '" + activityInstanceId + "' does not exist");
			}
    
			return HistoricActivityInstanceDto.fromHistoricActivityInstance(instance);
		  }
	  }

	}

}