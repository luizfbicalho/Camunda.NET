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
namespace org.camunda.bpm.engine.impl
{
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ExternalTaskState = org.camunda.bpm.engine.history.ExternalTaskState;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;

	using static org.camunda.bpm.engine.impl.util.EnsureUtil;

	[Serializable]
	public class HistoricExternalTaskLogQueryImpl : AbstractQuery<HistoricExternalTaskLogQuery, HistoricExternalTaskLog>, HistoricExternalTaskLogQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string externalTaskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string topicName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string workerId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Renamed;
	  protected internal string[] activityIds;
	  protected internal string[] activityInstanceIds;
	  protected internal string[] executionIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
	  protected internal long? priorityHigherThanOrEqual;
	  protected internal long? priorityLowerThanOrEqual;
	  protected internal string[] tenantIds;
	  protected internal ExternalTaskState state;

	  public HistoricExternalTaskLogQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  // query parameter ////////////////////////////////////////////

	  public virtual HistoricExternalTaskLogQuery logId(string historicExternalTaskLogId)
	  {
		ensureNotNull(typeof(NotValidException), "historicExternalTaskLogId", historicExternalTaskLogId);
		this.id = historicExternalTaskLogId;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery externalTaskId(string externalTaskId)
	  {
		ensureNotNull(typeof(NotValidException), "externalTaskId", externalTaskId);
		this.externalTaskId_Renamed = externalTaskId;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery topicName(string topicName)
	  {
		ensureNotNull(typeof(NotValidException), "topicName", topicName);
		this.topicName_Renamed = topicName;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery workerId(string workerId)
	  {
		ensureNotNull(typeof(NotValidException), "workerId", workerId);
		this.workerId_Renamed = workerId;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery errorMessage(string errorMessage)
	  {
		ensureNotNull(typeof(NotValidException), "errorMessage", errorMessage);
		this.errorMessage_Renamed = errorMessage;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery activityIdIn(params string[] activityIds)
	  {
		ensureNotNull(typeof(NotValidException), "activityIds", (object[]) activityIds);
		IList<string> activityIdList = CollectionUtil.asArrayList(activityIds);
		ensureNotContainsNull("activityIds", activityIdList);
		ensureNotContainsEmptyString("activityIds", activityIdList);
		this.activityIds = activityIds;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery activityInstanceIdIn(params string[] activityInstanceIds)
	  {
		ensureNotNull(typeof(NotValidException), "activityIds", (object[]) activityInstanceIds);
		IList<string> activityInstanceIdList = CollectionUtil.asArrayList(activityInstanceIds);
		ensureNotContainsNull("activityInstanceIds", activityInstanceIdList);
		ensureNotContainsEmptyString("activityInstanceIds", activityInstanceIdList);
		this.activityInstanceIds = activityInstanceIds;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery executionIdIn(params string[] executionIds)
	  {
		ensureNotNull(typeof(NotValidException), "activityIds", (object[]) executionIds);
		IList<string> executionIdList = CollectionUtil.asArrayList(executionIds);
		ensureNotContainsNull("executionIds", executionIdList);
		ensureNotContainsEmptyString("executionIds", executionIdList);
		this.executionIds = executionIds;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "processInstanceId", processInstanceId);
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery priorityHigherThanOrEquals(long priority)
	  {
		this.priorityHigherThanOrEqual = priority;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery priorityLowerThanOrEquals(long priority)
	  {
		this.priorityLowerThanOrEqual = priority;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery creationLog()
	  {
		State = org.camunda.bpm.engine.history.ExternalTaskState_Fields.CREATED;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery failureLog()
	  {
		State = org.camunda.bpm.engine.history.ExternalTaskState_Fields.FAILED;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery successLog()
	  {
		State = org.camunda.bpm.engine.history.ExternalTaskState_Fields.SUCCESSFUL;
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery deletionLog()
	  {
		State = org.camunda.bpm.engine.history.ExternalTaskState_Fields.DELETED;
		return this;
	  }

	  // order by //////////////////////////////////////////////


	  public virtual HistoricExternalTaskLogQuery orderByTimestamp()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.TIMESTAMP);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByExternalTaskId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.EXTERNAL_TASK_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByRetries()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.RETRIES);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByPriority()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.PRIORITY);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByTopicName()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.TOPIC_NAME);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByWorkerId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.WORKER_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByActivityId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByActivityInstanceId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByExecutionId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByProcessDefinitionId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByProcessDefinitionKey()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.PROCESS_DEFINITION_KEY);
		return this;
	  }

	  public virtual HistoricExternalTaskLogQuery orderByTenantId()
	  {
		orderBy(HistoricExternalTaskLogQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  // results //////////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricExternalTaskLogManager.findHistoricExternalTaskLogsCountByQueryCriteria(this);
	  }

	  public override IList<HistoricExternalTaskLog> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricExternalTaskLogManager.findHistoricExternalTaskLogsByQueryCriteria(this, page);
	  }

	  // setters ////////////////////////////////////////////////////////////

	  protected internal virtual ExternalTaskState State
	  {
		  set
		  {
			this.state = value;
		  }
	  }
	}

}