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
namespace org.camunda.bpm.engine.rest.dto.history
{

	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;

	public class HistoricDecisionInputInstanceDto : VariableValueDto
	{

	  protected internal string id;
	  protected internal string decisionInstanceId;
	  protected internal string clauseId;
	  protected internal string clauseName;
	  protected internal string errorMessage;
	  protected internal DateTime createTime;
	  protected internal DateTime removalTime;
	  protected internal string rootProcessInstanceId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string DecisionInstanceId
	  {
		  get
		  {
			return decisionInstanceId;
		  }
	  }

	  public virtual string ClauseId
	  {
		  get
		  {
			return clauseId;
		  }
	  }

	  public virtual string ClauseName
	  {
		  get
		  {
			return clauseName;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
	  }

	  public static HistoricDecisionInputInstanceDto fromHistoricDecisionInputInstance(HistoricDecisionInputInstance historicDecisionInputInstance)
	  {

		HistoricDecisionInputInstanceDto dto = new HistoricDecisionInputInstanceDto();

		dto.id = historicDecisionInputInstance.Id;
		dto.decisionInstanceId = historicDecisionInputInstance.DecisionInstanceId;
		dto.clauseId = historicDecisionInputInstance.ClauseId;
		dto.clauseName = historicDecisionInputInstance.ClauseName;
		dto.createTime = historicDecisionInputInstance.CreateTime;
		dto.removalTime = historicDecisionInputInstance.RemovalTime;
		dto.rootProcessInstanceId = historicDecisionInputInstance.RootProcessInstanceId;

		if (string.ReferenceEquals(historicDecisionInputInstance.ErrorMessage, null))
		{
		  VariableValueDto.fromTypedValue(dto, historicDecisionInputInstance.TypedValue);
		}
		else
		{
		  dto.errorMessage = historicDecisionInputInstance.ErrorMessage;
		  dto.type = VariableValueDto.toRestApiTypeName(historicDecisionInputInstance.TypeName);
		}

		return dto;
	  }

	}

}