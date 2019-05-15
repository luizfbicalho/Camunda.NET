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

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;

	public class HistoricActivityInstanceDto
	{

	  private string id;
	  private string parentActivityInstanceId;
	  private string activityId;
	  private string activityName;
	  private string activityType;
	  private string processDefinitionKey;
	  private string processDefinitionId;
	  private string processInstanceId;
	  private string executionId;
	  private string taskId;
	  private string calledProcessInstanceId;
	  private string calledCaseInstanceId;
	  private string assignee;
	  private DateTime startTime;
	  private DateTime endTime;
	  private long? durationInMillis;
	  private bool? canceled;
	  private bool? completeScope;
	  private string tenantId;
	  private DateTime removalTime;
	  private string rootProcessInstanceId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string ParentActivityInstanceId
	  {
		  get
		  {
			return parentActivityInstanceId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	  public virtual string CalledProcessInstanceId
	  {
		  get
		  {
			return calledProcessInstanceId;
		  }
	  }

	  public virtual string CalledCaseInstanceId
	  {
		  get
		  {
			return calledCaseInstanceId;
		  }
	  }

	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }

	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
	  }

	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
	  }

	  public virtual long? DurationInMillis
	  {
		  get
		  {
			return durationInMillis;
		  }
	  }

	  public virtual bool? Canceled
	  {
		  get
		  {
			return canceled;
		  }
	  }

	  public virtual bool? CompleteScope
	  {
		  get
		  {
			return completeScope;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
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

	  public static HistoricActivityInstanceDto fromHistoricActivityInstance(HistoricActivityInstance historicActivityInstance)
	  {

		HistoricActivityInstanceDto dto = new HistoricActivityInstanceDto();

		dto.id = historicActivityInstance.Id;
		dto.parentActivityInstanceId = historicActivityInstance.ParentActivityInstanceId;
		dto.activityId = historicActivityInstance.ActivityId;
		dto.activityName = historicActivityInstance.ActivityName;
		dto.activityType = historicActivityInstance.ActivityType;
		dto.processDefinitionKey = historicActivityInstance.ProcessDefinitionKey;
		dto.processDefinitionId = historicActivityInstance.ProcessDefinitionId;
		dto.processInstanceId = historicActivityInstance.ProcessInstanceId;
		dto.executionId = historicActivityInstance.ExecutionId;
		dto.taskId = historicActivityInstance.TaskId;
		dto.calledProcessInstanceId = historicActivityInstance.CalledProcessInstanceId;
		dto.calledCaseInstanceId = historicActivityInstance.CalledCaseInstanceId;
		dto.assignee = historicActivityInstance.Assignee;
		dto.startTime = historicActivityInstance.StartTime;
		dto.endTime = historicActivityInstance.EndTime;
		dto.durationInMillis = historicActivityInstance.DurationInMillis;
		dto.canceled = historicActivityInstance.Canceled;
		dto.completeScope = historicActivityInstance.CompleteScope;
		dto.tenantId = historicActivityInstance.TenantId;
		dto.removalTime = historicActivityInstance.RemovalTime;
		dto.rootProcessInstanceId = historicActivityInstance.RootProcessInstanceId;

		return dto;
	  }
	}

}