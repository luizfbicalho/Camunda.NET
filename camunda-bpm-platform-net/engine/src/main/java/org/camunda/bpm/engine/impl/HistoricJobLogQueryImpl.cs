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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsEmptyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using JobState = org.camunda.bpm.engine.history.JobState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class HistoricJobLogQueryImpl : AbstractQuery<HistoricJobLogQuery, HistoricJobLog>, HistoricJobLogQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobExceptionMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionConfiguration_Conflict;
	  protected internal string[] activityIds;
	  protected internal string[] executionIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Conflict;
	  protected internal JobState state;
	  protected internal long? jobPriorityHigherThanOrEqual;
	  protected internal long? jobPriorityLowerThanOrEqual;
	  protected internal string[] tenantIds;

	  public HistoricJobLogQueryImpl()
	  {
	  }

	  public HistoricJobLogQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  // query parameter ////////////////////////////////////////////

	  public virtual HistoricJobLogQuery logId(string historicJobLogId)
	  {
		ensureNotNull(typeof(NotValidException), "historicJobLogId", historicJobLogId);
		this.id = historicJobLogId;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobId(string jobId)
	  {
		ensureNotNull(typeof(NotValidException), "jobId", jobId);
		this.jobId_Conflict = jobId;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobExceptionMessage(string jobExceptionMessage)
	  {
		ensureNotNull(typeof(NotValidException), "jobExceptionMessage", jobExceptionMessage);
		this.jobExceptionMessage_Conflict = jobExceptionMessage;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobDefinitionId(string jobDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "jobDefinitionId", jobDefinitionId);
		this.jobDefinitionId_Conflict = jobDefinitionId;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobDefinitionType(string jobDefinitionType)
	  {
		ensureNotNull(typeof(NotValidException), "jobDefinitionType", jobDefinitionType);
		this.jobDefinitionType_Conflict = jobDefinitionType;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobDefinitionConfiguration(string jobDefinitionConfiguration)
	  {
		ensureNotNull(typeof(NotValidException), "jobDefinitionConfiguration", jobDefinitionConfiguration);
		this.jobDefinitionConfiguration_Conflict = jobDefinitionConfiguration;
		return this;
	  }

	  public virtual HistoricJobLogQuery activityIdIn(params string[] activityIds)
	  {
		IList<string> activityIdList = CollectionUtil.asArrayList(activityIds);
		ensureNotContainsNull("activityIds", activityIdList);
		ensureNotContainsEmptyString("activityIds", activityIdList);
		this.activityIds = activityIds;
		return this;
	  }

	  public virtual HistoricJobLogQuery executionIdIn(params string[] executionIds)
	  {
		IList<string> executionIdList = CollectionUtil.asArrayList(executionIds);
		ensureNotContainsNull("executionIds", executionIdList);
		ensureNotContainsEmptyString("executionIds", executionIdList);
		this.executionIds = executionIds;
		return this;
	  }

	  public virtual HistoricJobLogQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricJobLogQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricJobLogQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionKey", processDefinitionKey);
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricJobLogQuery deploymentId(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobPriorityHigherThanOrEquals(long priority)
	  {
		this.jobPriorityHigherThanOrEqual = priority;
		return this;
	  }

	  public virtual HistoricJobLogQuery jobPriorityLowerThanOrEquals(long priority)
	  {
		this.jobPriorityLowerThanOrEqual = priority;
		return this;
	  }

	  public virtual HistoricJobLogQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricJobLogQuery creationLog()
	  {
		State = org.camunda.bpm.engine.history.JobState_Fields.CREATED;
		return this;
	  }

	  public virtual HistoricJobLogQuery failureLog()
	  {
		State = org.camunda.bpm.engine.history.JobState_Fields.FAILED;
		return this;
	  }

	  public virtual HistoricJobLogQuery successLog()
	  {
		State = org.camunda.bpm.engine.history.JobState_Fields.SUCCESSFUL;
		return this;
	  }

	  public virtual HistoricJobLogQuery deletionLog()
	  {
		State = org.camunda.bpm.engine.history.JobState_Fields.DELETED;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(jobPriorityHigherThanOrEqual, jobPriorityLowerThanOrEqual);
	  }

	  // order by //////////////////////////////////////////////

	  public virtual HistoricJobLogQuery orderByTimestamp()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.TIMESTAMP);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByJobId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.JOB_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByJobDueDate()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.DUEDATE);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByJobRetries()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.RETRIES);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByJobPriority()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.PRIORITY);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByJobDefinitionId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.JOB_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByActivityId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByExecutionId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByProcessDefinitionId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByProcessDefinitionKey()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.PROCESS_DEFINITION_KEY);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByDeploymentId()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.DEPLOYMENT_ID);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderPartiallyByOccurrence()
	  {
		orderBy(HistoricJobLogQueryProperty_Fields.SEQUENCE_COUNTER);
		return this;
	  }

	  public virtual HistoricJobLogQuery orderByTenantId()
	  {
		return orderBy(HistoricJobLogQueryProperty_Fields.TENANT_ID);
	  }

	  // results //////////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricJobLogManager.findHistoricJobLogsCountByQueryCriteria(this);
	  }

	  public override IList<HistoricJobLog> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricJobLogManager.findHistoricJobLogsByQueryCriteria(this, page);
	  }

	  // getter //////////////////////////////////

	  public virtual string JobId
	  {
		  get
		  {
			return jobId_Conflict;
		  }
	  }

	  public virtual string JobExceptionMessage
	  {
		  get
		  {
			return jobExceptionMessage_Conflict;
		  }
	  }

	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId_Conflict;
		  }
	  }

	  public virtual string JobDefinitionType
	  {
		  get
		  {
			return jobDefinitionType_Conflict;
		  }
	  }

	  public virtual string JobDefinitionConfiguration
	  {
		  get
		  {
			return jobDefinitionConfiguration_Conflict;
		  }
	  }

	  public virtual string[] ActivityIds
	  {
		  get
		  {
			return activityIds;
		  }
	  }

	  public virtual string[] ExecutionIds
	  {
		  get
		  {
			return executionIds;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Conflict;
		  }
	  }

	  public virtual JobState State
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

	  public virtual string[] TenantIds
	  {
		  get
		  {
			return tenantIds;
		  }
	  }

	  // setter //////////////////////////////////


	}

}