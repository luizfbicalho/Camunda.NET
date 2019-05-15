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
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class HistoricTaskInstanceReportResultDto
	{

	  protected internal long? count;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionName;
	  protected internal string taskName;
	  protected internal string tenantId;

	  public virtual long? Count
	  {
		  get
		  {
			return count;
		  }
		  set
		  {
			this.count = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName;
		  }
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }


	  public virtual string TaskName
	  {
		  get
		  {
			return taskName;
		  }
		  set
		  {
			this.taskName = value;
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


	  public static HistoricTaskInstanceReportResultDto fromHistoricTaskInstanceReportResult(HistoricTaskInstanceReportResult taskReportResult)
	  {
		HistoricTaskInstanceReportResultDto dto = new HistoricTaskInstanceReportResultDto();

		dto.count = taskReportResult.Count;
		dto.processDefinitionKey = taskReportResult.ProcessDefinitionKey;
		dto.processDefinitionId = taskReportResult.ProcessDefinitionId;
		dto.processDefinitionName = taskReportResult.ProcessDefinitionName;
		dto.taskName = taskReportResult.TaskName;
		dto.tenantId = taskReportResult.TenantId;

		return dto;
	  }
	}

}