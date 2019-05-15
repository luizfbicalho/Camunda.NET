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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using Job = org.camunda.bpm.engine.runtime.Job;

	public class JobDto
	{

	  protected internal string id;
	  protected internal string jobDefinitionId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string executionId;
	  protected internal string exceptionMessage;
	  protected internal int retries;
	  protected internal DateTime dueDate;
	  protected internal bool suspended;
	  protected internal long priority;
	  protected internal string tenantId;
	  protected internal DateTime createTime;

	  public static JobDto fromJob(Job job)
	  {
		JobDto dto = new JobDto();
		dto.id = job.Id;
		dto.jobDefinitionId = job.JobDefinitionId;
		dto.processInstanceId = job.ProcessInstanceId;
		dto.processDefinitionId = job.ProcessDefinitionId;
		dto.processDefinitionKey = job.ProcessDefinitionKey;
		dto.executionId = job.ExecutionId;
		dto.exceptionMessage = job.ExceptionMessage;
		dto.retries = job.Retries;
		dto.dueDate = job.Duedate;
		dto.suspended = job.Suspended;
		dto.priority = job.Priority;
		dto.tenantId = job.TenantId;
		dto.createTime = job.CreateTime;

		return dto;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
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

	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage;
		  }
	  }

	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
	  }

	  public virtual DateTime DueDate
	  {
		  get
		  {
			return dueDate;
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

	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
		  }
	  }

	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }

	}

}