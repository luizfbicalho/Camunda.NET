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

	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;

	public class HistoricDecisionOutputInstanceDto : VariableValueDto
	{

	  protected internal string id;
	  protected internal string decisionInstanceId;
	  protected internal string clauseId;
	  protected internal string clauseName;
	  protected internal string ruleId;
	  protected internal int? ruleOrder;
	  protected internal string variableName;
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

	  public virtual string RuleId
	  {
		  get
		  {
			return ruleId;
		  }
	  }

	  public virtual int? RuleOrder
	  {
		  get
		  {
			return ruleOrder;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
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

	  public static HistoricDecisionOutputInstanceDto fromHistoricDecisionOutputInstance(HistoricDecisionOutputInstance historicDecisionOutputInstance)
	  {

		HistoricDecisionOutputInstanceDto dto = new HistoricDecisionOutputInstanceDto();

		dto.id = historicDecisionOutputInstance.Id;
		dto.decisionInstanceId = historicDecisionOutputInstance.DecisionInstanceId;
		dto.clauseId = historicDecisionOutputInstance.ClauseId;
		dto.clauseName = historicDecisionOutputInstance.ClauseName;
		dto.ruleId = historicDecisionOutputInstance.RuleId;
		dto.ruleOrder = historicDecisionOutputInstance.RuleOrder;
		dto.variableName = historicDecisionOutputInstance.VariableName;
		dto.createTime = historicDecisionOutputInstance.CreateTime;
		dto.removalTime = historicDecisionOutputInstance.RemovalTime;
		dto.rootProcessInstanceId = historicDecisionOutputInstance.RootProcessInstanceId;

		if (string.ReferenceEquals(historicDecisionOutputInstance.ErrorMessage, null))
		{
		  VariableValueDto.fromTypedValue(dto, historicDecisionOutputInstance.TypedValue);
		}
		else
		{
		  dto.errorMessage = historicDecisionOutputInstance.ErrorMessage;
		  dto.type = VariableValueDto.toRestApiTypeName(historicDecisionOutputInstance.TypeName);
		}

		return dto;
	  }

	}

}