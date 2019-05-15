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

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricJobLogDto
	{

	  protected internal string id;
	  protected internal DateTime timestamp;
	  protected internal DateTime removalTime;

	  protected internal string jobId;
	  protected internal DateTime jobDueDate;
	  protected internal int jobRetries;
	  protected internal long jobPriority;
	  protected internal string jobExceptionMessage;

	  protected internal string jobDefinitionId;
	  protected internal string jobDefinitionType;
	  protected internal string jobDefinitionConfiguration;

	  protected internal string activityId;
	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string deploymentId;
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

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
	  }

	  public virtual DateTime JobDueDate
	  {
		  get
		  {
			return jobDueDate;
		  }
	  }

	  public virtual int JobRetries
	  {
		  get
		  {
			return jobRetries;
		  }
	  }

	  public virtual long JobPriority
	  {
		  get
		  {
			return jobPriority;
		  }
	  }

	  public virtual string JobExceptionMessage
	  {
		  get
		  {
			return jobExceptionMessage;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
	  }

	  public virtual string JobDefinitionType
	  {
		  get
		  {
			return jobDefinitionType;
		  }
	  }

	  public virtual string JobDefinitionConfiguration
	  {
		  get
		  {
			return jobDefinitionConfiguration;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
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

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
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

	  public static HistoricJobLogDto fromHistoricJobLog(HistoricJobLog historicJobLog)
	  {
		HistoricJobLogDto result = new HistoricJobLogDto();

		result.id = historicJobLog.Id;
		result.timestamp = historicJobLog.Timestamp;
		result.removalTime = historicJobLog.RemovalTime;

		result.jobId = historicJobLog.JobId;
		result.jobDueDate = historicJobLog.JobDueDate;
		result.jobRetries = historicJobLog.JobRetries;
		result.jobPriority = historicJobLog.JobPriority;
		result.jobExceptionMessage = historicJobLog.JobExceptionMessage;

		result.jobDefinitionId = historicJobLog.JobDefinitionId;
		result.jobDefinitionType = historicJobLog.JobDefinitionType;
		result.jobDefinitionConfiguration = historicJobLog.JobDefinitionConfiguration;

		result.activityId = historicJobLog.ActivityId;
		result.executionId = historicJobLog.ExecutionId;
		result.processInstanceId = historicJobLog.ProcessInstanceId;
		result.processDefinitionId = historicJobLog.ProcessDefinitionId;
		result.processDefinitionKey = historicJobLog.ProcessDefinitionKey;
		result.deploymentId = historicJobLog.DeploymentId;
		result.tenantId = historicJobLog.TenantId;
		result.rootProcessInstanceId = historicJobLog.RootProcessInstanceId;

		result.creationLog = historicJobLog.CreationLog;
		result.failureLog = historicJobLog.FailureLog;
		result.successLog = historicJobLog.SuccessLog;
		result.deletionLog = historicJobLog.DeletionLog;

		return result;
	  }

	}

}