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
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;

	public class HistoricExternalTaskLogDto
	{

	  protected internal string id;
	  protected internal DateTime timestamp;
	  protected internal DateTime removalTime;

	  protected internal string externalTaskId;
	  protected internal string topicName;
	  protected internal string workerId;
	  protected internal long priority;
	  protected internal int? retries;
	  protected internal string errorMessage;

	  protected internal string activityId;
	  protected internal string activityInstanceId;
	  protected internal string executionId;

	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string tenantId;
	  protected internal string rootProcessInstanceId;

	  protected internal bool creationLog;
	  protected internal bool failureLog;
	  protected internal bool successLog;
	  protected internal bool deletionLog;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
	  }

	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId;
		  }
	  }

	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
	  }

	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
	  }

	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual int? Retries
	  {
		  get
		  {
			return retries;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
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

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
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

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual bool CreationLog
	  {
		  get
		  {
			return creationLog;
		  }
	  }

	  public virtual bool FailureLog
	  {
		  get
		  {
			return failureLog;
		  }
	  }

	  public virtual bool SuccessLog
	  {
		  get
		  {
			return successLog;
		  }
	  }

	  public virtual bool DeletionLog
	  {
		  get
		  {
			return deletionLog;
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

	  public static HistoricExternalTaskLogDto fromHistoricExternalTaskLog(HistoricExternalTaskLog historicExternalTaskLog)
	  {
		HistoricExternalTaskLogDto result = new HistoricExternalTaskLogDto();

		result.id = historicExternalTaskLog.Id;
		result.timestamp = historicExternalTaskLog.Timestamp;
		result.removalTime = historicExternalTaskLog.RemovalTime;

		result.externalTaskId = historicExternalTaskLog.ExternalTaskId;
		result.topicName = historicExternalTaskLog.TopicName;
		result.workerId = historicExternalTaskLog.WorkerId;
		result.priority = historicExternalTaskLog.Priority;
		result.retries = historicExternalTaskLog.Retries;
		result.errorMessage = historicExternalTaskLog.ErrorMessage;

		result.activityId = historicExternalTaskLog.ActivityId;
		result.activityInstanceId = historicExternalTaskLog.ActivityInstanceId;
		result.executionId = historicExternalTaskLog.ExecutionId;

		result.processInstanceId = historicExternalTaskLog.ProcessInstanceId;
		result.processDefinitionId = historicExternalTaskLog.ProcessDefinitionId;
		result.processDefinitionKey = historicExternalTaskLog.ProcessDefinitionKey;
		result.tenantId = historicExternalTaskLog.TenantId;
		result.rootProcessInstanceId = historicExternalTaskLog.RootProcessInstanceId;

		result.creationLog = historicExternalTaskLog.CreationLog;
		result.failureLog = historicExternalTaskLog.FailureLog;
		result.successLog = historicExternalTaskLog.SuccessLog;
		result.deletionLog = historicExternalTaskLog.DeletionLog;

		return result;
	  }
	}

}