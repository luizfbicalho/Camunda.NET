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
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricCaseActivityInstanceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricCaseActivityInstanceResourceImpl : HistoricCaseActivityInstanceResource
	{

	  private ProcessEngine engine;
	  private string caseActivityInstanceId;

	  public HistoricCaseActivityInstanceResourceImpl(ProcessEngine engine, string caseActivityInstanceId)
	  {
		this.engine = engine;
		this.caseActivityInstanceId = caseActivityInstanceId;
	  }

	  public virtual HistoricCaseActivityInstanceDto HistoricCaseActivityInstance
	  {
		  get
		  {
			HistoryService historyService = engine.HistoryService;
			HistoricCaseActivityInstance instance = historyService.createHistoricCaseActivityInstanceQuery().caseActivityInstanceId(caseActivityInstanceId).singleResult();
    
			if (instance == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Historic case activity instance with id '" + caseActivityInstanceId + "' does not exist");
			}
    
			return HistoricCaseActivityInstanceDto.fromHistoricCaseActivityInstance(instance);
		  }
	  }

	}

}