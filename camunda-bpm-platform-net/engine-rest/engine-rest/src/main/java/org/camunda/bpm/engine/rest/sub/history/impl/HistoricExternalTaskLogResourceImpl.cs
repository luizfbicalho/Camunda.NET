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
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogDto = org.camunda.bpm.engine.rest.dto.history.HistoricExternalTaskLogDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricExternalTaskLogResourceImpl : HistoricExternalTaskLogResource
	{

	  protected internal string id;
	  protected internal ProcessEngine engine;

	  public HistoricExternalTaskLogResourceImpl(string id, ProcessEngine engine)
	  {
		this.id = id;
		this.engine = engine;
	  }

	  public virtual HistoricExternalTaskLogDto HistoricExternalTaskLog
	  {
		  get
		  {
			HistoryService historyService = engine.HistoryService;
			HistoricExternalTaskLog historicExternalTaskLog = historyService.createHistoricExternalTaskLogQuery().logId(id).singleResult();
    
			if (historicExternalTaskLog == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Historic external task log with id " + id + " does not exist");
			}
    
			return HistoricExternalTaskLogDto.fromHistoricExternalTaskLog(historicExternalTaskLog);
		  }
	  }

	  public virtual string ErrorDetails
	  {
		  get
		  {
			try
			{
			  HistoryService historyService = engine.HistoryService;
			  return historyService.getHistoricExternalTaskLogErrorDetails(id);
			}
			catch (AuthorizationException e)
			{
			  throw e;
			}
			catch (ProcessEngineException e)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, e.Message);
			}
		  }
	  }
	}

}