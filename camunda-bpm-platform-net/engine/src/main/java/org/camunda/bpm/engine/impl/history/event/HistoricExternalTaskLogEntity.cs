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
	using ExternalTaskState = org.camunda.bpm.engine.history.ExternalTaskState;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ExceptionUtil.createExceptionByteArray;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.StringUtil.toByteArray;

	[Serializable]
	public class HistoricExternalTaskLogEntity : HistoryEvent, HistoricExternalTaskLog
	{

	  private const long serialVersionUID = 1L;
	  private const string EXCEPTION_NAME = "historicExternalTaskLog.exceptionByteArray";

	  protected internal DateTime timestamp;

	  protected internal string externalTaskId;

	  protected internal string topicName;
	  protected internal string workerId;
	  protected internal long priority;
	  protected internal int? retries;

	  protected internal string errorMessage;

	  protected internal string errorDetailsByteArrayId;
	  protected internal string activityId;

	  protected internal string activityInstanceId;
	  protected internal string tenantId;

	  protected internal int state;

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


	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId;
		  }
		  set
		  {
			this.externalTaskId = value;
		  }
	  }


	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
		  set
		  {
			this.topicName = value;
		  }
	  }


	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
		  set
		  {
			this.workerId = value;
		  }
	  }


	  public virtual int? Retries
	  {
		  get
		  {
			return retries;
		  }
		  set
		  {
			this.retries = value;
		  }
	  }


	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
		  set
		  {
			// note: it is not a clean way to truncate where the history event is produced, since truncation is only
			//   relevant for relational history databases that follow our schema restrictions;
			//   a similar problem exists in ExternalTaskEntity#setErrorMessage where truncation may not be required for custom
			//   persistence implementations
			if (!string.ReferenceEquals(value, null) && value.Length > ExternalTaskEntity.MAX_EXCEPTION_MESSAGE_LENGTH)
			{
			  this.errorMessage = value.Substring(0, ExternalTaskEntity.MAX_EXCEPTION_MESSAGE_LENGTH);
			}
			else
			{
			  this.errorMessage = value;
			}
		  }
	  }


	  public virtual string ErrorDetailsByteArrayId
	  {
		  get
		  {
			return errorDetailsByteArrayId;
		  }
		  set
		  {
			this.errorDetailsByteArrayId = value;
		  }
	  }


	  public virtual string ErrorDetails
	  {
		  get
		  {
			ByteArrayEntity byteArray = ErrorByteArray;
			return ExceptionUtil.getExceptionStacktrace(byteArray);
		  }
		  set
		  {
			EnsureUtil.ensureNotNull("exception", value);
    
			sbyte[] exceptionBytes = toByteArray(value);
			ByteArrayEntity byteArray = createExceptionByteArray(EXCEPTION_NAME, exceptionBytes, ResourceTypes.HISTORY);
			byteArray.RootProcessInstanceId = rootProcessInstanceId;
			byteArray.RemovalTime = removalTime;
    
			errorDetailsByteArrayId = byteArray.Id;
		  }
	  }


	  protected internal virtual ByteArrayEntity ErrorByteArray
	  {
		  get
		  {
			if (!string.ReferenceEquals(errorDetailsByteArrayId, null))
			{
			  return Context.CommandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), errorDetailsByteArrayId);
			}
			return null;
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


	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
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


	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
		  set
		  {
			this.priority = value;
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


	  public virtual bool CreationLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.ExternalTaskState_Fields.CREATED.StateCode;
		  }
	  }

	  public virtual bool FailureLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.ExternalTaskState_Fields.FAILED.StateCode;
		  }
	  }

	  public virtual bool SuccessLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.ExternalTaskState_Fields.SUCCESSFUL.StateCode;
		  }
	  }

	  public virtual bool DeletionLog
	  {
		  get
		  {
			return state == org.camunda.bpm.engine.history.ExternalTaskState_Fields.DELETED.StateCode;
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