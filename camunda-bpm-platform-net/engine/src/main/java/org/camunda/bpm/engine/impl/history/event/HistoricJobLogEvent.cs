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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using JobState = org.camunda.bpm.engine.history.JobState;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricJobLogEvent : HistoryEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal DateTime timestamp;

	  protected internal string jobId;

	  protected internal DateTime jobDueDate;

	  protected internal int jobRetries;

	  protected internal long jobPriority;

	  protected internal string jobExceptionMessage;

	  protected internal string exceptionByteArrayId;

	  protected internal string jobDefinitionId;

	  protected internal string jobDefinitionType;

	  protected internal string jobDefinitionConfiguration;

	  protected internal string activityId;

	  protected internal string deploymentId;

	  protected internal int state;

	  protected internal string tenantId;

	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }


	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
		  set
		  {
			this.jobId = value;
		  }
	  }


	  public virtual DateTime JobDueDate
	  {
		  get
		  {
			return jobDueDate;
		  }
		  set
		  {
			this.jobDueDate = value;
		  }
	  }


	  public virtual int JobRetries
	  {
		  get
		  {
			return jobRetries;
		  }
		  set
		  {
			this.jobRetries = value;
		  }
	  }


	  public virtual long JobPriority
	  {
		  get
		  {
			return jobPriority;
		  }
		  set
		  {
			this.jobPriority = value;
		  }
	  }


	  public virtual string JobExceptionMessage
	  {
		  get
		  {
			return jobExceptionMessage;
		  }
		  set
		  {
			// note: it is not a clean way to truncate where the history event is produced, since truncation is only
			//   relevant for relational history databases that follow our schema restrictions;
			//   a similar problem exists in JobEntity#setExceptionMessage where truncation may not be required for custom
			//   persistence implementations
			this.jobExceptionMessage = StringUtil.trimToMaximumLengthAllowed(value);
		  }
	  }


	  public virtual string ExceptionByteArrayId
	  {
		  get
		  {
			return exceptionByteArrayId;
		  }
		  set
		  {
			this.exceptionByteArrayId = value;
		  }
	  }


	  public virtual string ExceptionStacktrace
	  {
		  get
		  {
			ByteArrayEntity byteArray = ExceptionByteArray;
			return ExceptionUtil.getExceptionStacktrace(byteArray);
		  }
	  }

	  protected internal virtual ByteArrayEntity ExceptionByteArray
	  {
		  get
		  {
			if (!string.ReferenceEquals(exceptionByteArrayId, null))
			{
			  return Context.CommandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), exceptionByteArrayId);
			}
    
			return null;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }


	  public virtual string JobDefinitionType
	  {
		  get
		  {
			return jobDefinitionType;
		  }
		  set
		  {
			this.jobDefinitionType = value;
		  }
	  }


	  public virtual string JobDefinitionConfiguration
	  {
		  get
		  {
			return jobDefinitionConfiguration;
		  }
		  set
		  {
			this.jobDefinitionConfiguration = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual int State
	  {
		  get
		  {
			return state;
		  }
		  set
		  {
			this.state = value;
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


	  public virtual bool CreationLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.JobState_Fields.CREATED.StateCode;
		  }
	  }

	  public virtual bool FailureLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.JobState_Fields.FAILED.StateCode;
		  }
	  }

	  public virtual bool SuccessLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.JobState_Fields.SUCCESSFUL.StateCode;
		  }
	  }

	  public virtual bool DeletionLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.JobState_Fields.DELETED.StateCode;
		  }
	  }

	  public override string RootProcessInstanceId
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

	}

}