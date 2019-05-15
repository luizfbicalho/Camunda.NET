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

	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;

	[Serializable]
	public class CleanableHistoricCaseInstanceReportResultDto
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionName;
	  protected internal int caseDefinitionVersion;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedCaseInstanceCount;
	  protected internal long cleanableCaseInstanceCount;
	  protected internal string tenantId;

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }


	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName;
		  }
		  set
		  {
			this.caseDefinitionName = value;
		  }
	  }


	  public virtual int CaseDefinitionVersion
	  {
		  get
		  {
			return caseDefinitionVersion;
		  }
		  set
		  {
			this.caseDefinitionVersion = value;
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


	  public virtual long FinishedCaseInstanceCount
	  {
		  get
		  {
			return finishedCaseInstanceCount;
		  }
		  set
		  {
			this.finishedCaseInstanceCount = value;
		  }
	  }


	  public virtual long CleanableCaseInstanceCount
	  {
		  get
		  {
			return cleanableCaseInstanceCount;
		  }
		  set
		  {
			this.cleanableCaseInstanceCount = value;
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


	  public static IList<CleanableHistoricCaseInstanceReportResultDto> convert(IList<CleanableHistoricCaseInstanceReportResult> reportResult)
	  {
		IList<CleanableHistoricCaseInstanceReportResultDto> dtos = new List<CleanableHistoricCaseInstanceReportResultDto>();
		foreach (CleanableHistoricCaseInstanceReportResult current in reportResult)
		{
		  CleanableHistoricCaseInstanceReportResultDto dto = new CleanableHistoricCaseInstanceReportResultDto();
		  dto.CaseDefinitionId = current.CaseDefinitionId;
		  dto.CaseDefinitionKey = current.CaseDefinitionKey;
		  dto.CaseDefinitionName = current.CaseDefinitionName;
		  dto.CaseDefinitionVersion = current.CaseDefinitionVersion;
		  dto.HistoryTimeToLive = current.HistoryTimeToLive;
		  dto.FinishedCaseInstanceCount = current.FinishedCaseInstanceCount;
		  dto.CleanableCaseInstanceCount = current.CleanableCaseInstanceCount;
		  dto.TenantId = current.TenantId;
		  dtos.Add(dto);
		}
		return dtos;
	  }
	}

}