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

	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;

	[Serializable]
	public class CleanableHistoricDecisionInstanceReportResultDto
	{

	  private const long serialVersionUID = 1L;

	  protected internal string decisionDefinitionId;
	  protected internal string decisionDefinitionKey;
	  protected internal string decisionDefinitionName;
	  protected internal int decisionDefinitionVersion;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedDecisionInstanceCount;
	  protected internal long cleanableDecisionInstanceCount;
	  protected internal string tenantId;

	  public virtual string DecisionDefinitionId
	  {
		  get
		  {
			return decisionDefinitionId;
		  }
		  set
		  {
			this.decisionDefinitionId = value;
		  }
	  }


	  public virtual string DecisionDefinitionKey
	  {
		  get
		  {
			return decisionDefinitionKey;
		  }
		  set
		  {
			this.decisionDefinitionKey = value;
		  }
	  }


	  public virtual string DecisionDefinitionName
	  {
		  get
		  {
			return decisionDefinitionName;
		  }
		  set
		  {
			this.decisionDefinitionName = value;
		  }
	  }


	  public virtual int DecisionDefinitionVersion
	  {
		  get
		  {
			return decisionDefinitionVersion;
		  }
		  set
		  {
			this.decisionDefinitionVersion = value;
		  }
	  }


	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return historyTimeToLive;
		  }
		  set
		  {
			this.historyTimeToLive = value;
		  }
	  }


	  public virtual long FinishedDecisionInstanceCount
	  {
		  get
		  {
			return finishedDecisionInstanceCount;
		  }
		  set
		  {
			this.finishedDecisionInstanceCount = value;
		  }
	  }


	  public virtual long CleanableDecisionInstanceCount
	  {
		  get
		  {
			return cleanableDecisionInstanceCount;
		  }
		  set
		  {
			this.cleanableDecisionInstanceCount = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public static IList<CleanableHistoricDecisionInstanceReportResultDto> convert(IList<CleanableHistoricDecisionInstanceReportResult> reportResult)
	  {
		IList<CleanableHistoricDecisionInstanceReportResultDto> dtos = new List<CleanableHistoricDecisionInstanceReportResultDto>();
		foreach (CleanableHistoricDecisionInstanceReportResult current in reportResult)
		{
		  CleanableHistoricDecisionInstanceReportResultDto dto = new CleanableHistoricDecisionInstanceReportResultDto();
		  dto.DecisionDefinitionId = current.DecisionDefinitionId;
		  dto.DecisionDefinitionKey = current.DecisionDefinitionKey;
		  dto.DecisionDefinitionName = current.DecisionDefinitionName;
		  dto.DecisionDefinitionVersion = current.DecisionDefinitionVersion;
		  dto.HistoryTimeToLive = current.HistoryTimeToLive;
		  dto.FinishedDecisionInstanceCount = current.FinishedDecisionInstanceCount;
		  dto.CleanableDecisionInstanceCount = current.CleanableDecisionInstanceCount;
		  dto.TenantId = current.TenantId;
		  dtos.Add(dto);
		}
		return dtos;
	  }
	}

}