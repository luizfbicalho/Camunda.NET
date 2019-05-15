using System;
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
namespace org.camunda.bpm.engine.rest.dto.history
{

	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;

	using JsonInclude = com.fasterxml.jackson.annotation.JsonInclude;
	using Include = com.fasterxml.jackson.annotation.JsonInclude.Include;

	public class HistoricDecisionInstanceDto
	{

	  protected internal string id;
	  protected internal string decisionDefinitionId;
	  protected internal string decisionDefinitionKey;
	  protected internal string decisionDefinitionName;
	  protected internal DateTime evaluationTime;
	  protected internal DateTime removalTime;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processInstanceId;
	  protected internal string rootProcessInstanceId;
	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseInstanceId;
	  protected internal string activityId;
	  protected internal string activityInstanceId;
	  protected internal string userId;
	  protected internal IList<HistoricDecisionInputInstanceDto> inputs;
	  protected internal IList<HistoricDecisionOutputInstanceDto> outputs;
	  protected internal double? collectResultValue;
	  protected internal string rootDecisionInstanceId;
	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string decisionRequirementsDefinitionKey;
	  protected internal string tenantId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string DecisionDefinitionId
	  {
		  get
		  {
			return decisionDefinitionId;
		  }
	  }

	  public virtual string DecisionDefinitionKey
	  {
		  get
		  {
			return decisionDefinitionKey;
		  }
	  }

	  public virtual string DecisionDefinitionName
	  {
		  get
		  {
			return decisionDefinitionName;
		  }
	  }

	  public virtual DateTime EvaluationTime
	  {
		  get
		  {
			return evaluationTime;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonInclude(com.fasterxml.jackson.annotation.JsonInclude.Include.NON_NULL) public java.util.List<HistoricDecisionInputInstanceDto> getInputs()
	  public virtual IList<HistoricDecisionInputInstanceDto> Inputs
	  {
		  get
		  {
			return inputs;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonInclude(com.fasterxml.jackson.annotation.JsonInclude.Include.NON_NULL) public java.util.List<HistoricDecisionOutputInstanceDto> getOutputs()
	  public virtual IList<HistoricDecisionOutputInstanceDto> Outputs
	  {
		  get
		  {
			return outputs;
		  }
	  }

	  public virtual double? CollectResultValue
	  {
		  get
		  {
			return collectResultValue;
		  }
	  }

	  public virtual string RootDecisionInstanceId
	  {
		  get
		  {
			return rootDecisionInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string DecisionRequirementsDefinitionId
	  {
		  get
		  {
			return decisionRequirementsDefinitionId;
		  }
	  }

	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  get
		  {
			return decisionRequirementsDefinitionKey;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
		  set
		  {
			this.removalTime = value;
		  }
	  }


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public static HistoricDecisionInstanceDto fromHistoricDecisionInstance(HistoricDecisionInstance historicDecisionInstance)
	  {
		HistoricDecisionInstanceDto dto = new HistoricDecisionInstanceDto();

		dto.id = historicDecisionInstance.Id;
		dto.decisionDefinitionId = historicDecisionInstance.DecisionDefinitionId;
		dto.decisionDefinitionKey = historicDecisionInstance.DecisionDefinitionKey;
		dto.decisionDefinitionName = historicDecisionInstance.DecisionDefinitionName;
		dto.evaluationTime = historicDecisionInstance.EvaluationTime;
		dto.removalTime = historicDecisionInstance.RemovalTime;
		dto.processDefinitionId = historicDecisionInstance.ProcessDefinitionId;
		dto.processDefinitionKey = historicDecisionInstance.ProcessDefinitionKey;
		dto.processInstanceId = historicDecisionInstance.ProcessInstanceId;
		dto.caseDefinitionId = historicDecisionInstance.CaseDefinitionId;
		dto.caseDefinitionKey = historicDecisionInstance.CaseDefinitionKey;
		dto.caseInstanceId = historicDecisionInstance.CaseInstanceId;
		dto.activityId = historicDecisionInstance.ActivityId;
		dto.activityInstanceId = historicDecisionInstance.ActivityInstanceId;
		dto.userId = historicDecisionInstance.UserId;
		dto.collectResultValue = historicDecisionInstance.CollectResultValue;
		dto.rootDecisionInstanceId = historicDecisionInstance.RootDecisionInstanceId;
		dto.rootProcessInstanceId = historicDecisionInstance.RootProcessInstanceId;
		dto.decisionRequirementsDefinitionId = historicDecisionInstance.DecisionRequirementsDefinitionId;
		dto.decisionRequirementsDefinitionKey = historicDecisionInstance.DecisionRequirementsDefinitionKey;
		dto.tenantId = historicDecisionInstance.TenantId;

		try
		{
		  IList<HistoricDecisionInputInstanceDto> inputs = new List<HistoricDecisionInputInstanceDto>();
		  foreach (HistoricDecisionInputInstance input in historicDecisionInstance.Inputs)
		  {
			HistoricDecisionInputInstanceDto inputDto = HistoricDecisionInputInstanceDto.fromHistoricDecisionInputInstance(input);
			inputs.Add(inputDto);
		  }
		  dto.inputs = inputs;
		}
		catch (ProcessEngineException)
		{
		  // no inputs fetched
		}

		try
		{
		  IList<HistoricDecisionOutputInstanceDto> outputs = new List<HistoricDecisionOutputInstanceDto>();
		  foreach (HistoricDecisionOutputInstance output in historicDecisionInstance.Outputs)
		  {
			HistoricDecisionOutputInstanceDto outputDto = HistoricDecisionOutputInstanceDto.fromHistoricDecisionOutputInstance(output);
			outputs.Add(outputDto);
		  }
		  dto.outputs = outputs;
		}
		catch (ProcessEngineException)
		{
		  // no outputs fetched
		}

		return dto;
	  }

	}

}