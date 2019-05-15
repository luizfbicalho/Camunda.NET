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

	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;

	public class CleanableHistoricProcessInstanceReportResultDto
	{

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionName;
	  protected internal int processDefinitionVersion;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedProcessInstanceCount;
	  protected internal long cleanableProcessInstanceCount;
	  protected internal string tenantId;

	  public CleanableHistoricProcessInstanceReportResultDto()
	  {
	  }

	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessDefinitionName
	  {
		  set
		  {
			this.processDefinitionName = value;
		  }
		  get
		  {
			return processDefinitionName;
		  }
	  }

	  public virtual int ProcessDefinitionVersion
	  {
		  set
		  {
			this.processDefinitionVersion = value;
		  }
		  get
		  {
			return processDefinitionVersion;
		  }
	  }

	  public virtual int? HistoryTimeToLive
	  {
		  set
		  {
			this.historyTimeToLive = value;
		  }
		  get
		  {
			return historyTimeToLive;
		  }
	  }

	  public virtual void setFinishedProcessInstanceCount(long finishedProcessInstanceCount)
	  {
		this.finishedProcessInstanceCount = finishedProcessInstanceCount;
	  }

	  public virtual void setCleanableProcessInstanceCount(long cleanableProcessInstanceCount)
	  {
		this.cleanableProcessInstanceCount = cleanableProcessInstanceCount;
	  }

	  public virtual string TenantId
	  {
		  set
		  {
			this.tenantId = value;
		  }
		  get
		  {
			return tenantId;
		  }
	  }






	  public virtual long? getFinishedProcessInstanceCount()
	  {
		return finishedProcessInstanceCount;
	  }

	  public virtual long? getCleanableProcessInstanceCount()
	  {
		return cleanableProcessInstanceCount;
	  }


	  protected internal virtual CleanableHistoricProcessInstanceReport createNewReportQuery(ProcessEngine engine)
	  {
		return engine.HistoryService.createCleanableHistoricProcessInstanceReport();
	  }

	  public static IList<CleanableHistoricProcessInstanceReportResultDto> convert(IList<CleanableHistoricProcessInstanceReportResult> reportResult)
	  {
		IList<CleanableHistoricProcessInstanceReportResultDto> dtos = new List<CleanableHistoricProcessInstanceReportResultDto>();
		foreach (CleanableHistoricProcessInstanceReportResult current in reportResult)
		{
		  CleanableHistoricProcessInstanceReportResultDto dto = new CleanableHistoricProcessInstanceReportResultDto();
		  dto.ProcessDefinitionId = current.ProcessDefinitionId;
		  dto.ProcessDefinitionKey = current.ProcessDefinitionKey;
		  dto.ProcessDefinitionName = current.ProcessDefinitionName;
		  dto.ProcessDefinitionVersion = current.ProcessDefinitionVersion;
		  dto.HistoryTimeToLive = current.HistoryTimeToLive;
		  dto.setFinishedProcessInstanceCount(current.FinishedProcessInstanceCount);
		  dto.setCleanableProcessInstanceCount(current.CleanableProcessInstanceCount);
		  dto.TenantId = current.TenantId;
		  dtos.Add(dto);
		}
		return dtos;
	  }

	}

}