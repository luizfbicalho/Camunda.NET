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

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricProcessInstanceResourceImpl : HistoricProcessInstanceResource
	{

	  private ProcessEngine engine;
	  private string processInstanceId;

	  public HistoricProcessInstanceResourceImpl(ProcessEngine engine, string processInstanceId)
	  {
		this.engine = engine;
		this.processInstanceId = processInstanceId;
	  }

	  public virtual HistoricProcessInstanceDto HistoricProcessInstance
	  {
		  get
		  {
			HistoryService historyService = engine.HistoryService;
			HistoricProcessInstance instance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
    
			if (instance == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Historic process instance with id " + processInstanceId + " does not exist");
			}
    
			return HistoricProcessInstanceDto.fromHistoricProcessInstance(instance);
		  }
	  }

	  public virtual void deleteHistoricProcessInstance(bool? failIfNotExists)
	  {
		HistoryService historyService = engine.HistoryService;
		try
		{
		  if (failIfNotExists == null || failIfNotExists)
		  {
			historyService.deleteHistoricProcessInstance(processInstanceId);
		  }
		  else
		  {
			historyService.deleteHistoricProcessInstanceIfExists(processInstanceId);
		  }
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, e, "Historic process instance with id " + processInstanceId + " does not exist");
		}
	  }

	}

}