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

	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	[Serializable]
	public class ExternalTaskQueryImpl : AbstractQuery<ExternalTaskQuery, ExternalTask>, ExternalTaskQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string externalTaskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string workerId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime lockExpirationBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime lockExpirationAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string topicName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? locked_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? notLocked_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] processInstanceIdIn_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] activityIdIn_Conflict;
	  protected internal SuspensionState suspensionState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? priorityHigherThanOrEquals_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal long? priorityLowerThanOrEquals_Conflict;
	  protected internal bool? retriesLeft;
	  protected internal string[] tenantIds;

	  public ExternalTaskQueryImpl()
	  {
	  }

	  public ExternalTaskQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ExternalTaskQuery externalTaskId(string externalTaskId)
	  {
		ensureNotNull("externalTaskId", externalTaskId);
		this.externalTaskId_Conflict = externalTaskId;
		return this;
	  }

	  public virtual ExternalTaskQuery workerId(string workerId)
	  {
		ensureNotNull("workerId", workerId);
		this.workerId_Conflict = workerId;
		return this;
	  }

	  public virtual ExternalTaskQuery lockExpirationBefore(DateTime lockExpirationDate)
	  {
		ensureNotNull("lockExpirationBefore", lockExpirationDate);
		this.lockExpirationBefore_Conflict = lockExpirationDate;
		return this;
	  }

	  public virtual ExternalTaskQuery lockExpirationAfter(DateTime lockExpirationDate)
	  {
		ensureNotNull("lockExpirationAfter", lockExpirationDate);
		this.lockExpirationAfter_Conflict = lockExpirationDate;
		return this;
	  }

	  public virtual ExternalTaskQuery topicName(string topicName)
	  {
		ensureNotNull("topicName", topicName);
		this.topicName_Conflict = topicName;
		return this;
	  }

	  public virtual ExternalTaskQuery locked()
	  {
		this.locked_Conflict = true;
		return this;
	  }

	  public virtual ExternalTaskQuery notLocked()
	  {
		this.notLocked_Conflict = true;
		return this;
	  }

	  public virtual ExternalTaskQuery executionId(string executionId)
	  {
		ensureNotNull("executionId", executionId);
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual ExternalTaskQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull("processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual ExternalTaskQuery processInstanceIdIn(params string[] processInstanceIdIn)
	  {
		ensureNotNull("processInstanceIdIn", (object[]) processInstanceIdIn);
		this.processInstanceIdIn_Conflict = processInstanceIdIn;
		return this;
	  }

	  public virtual ExternalTaskQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual ExternalTaskQuery activityId(string activityId)
	  {
		ensureNotNull("activityId", activityId);
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual ExternalTaskQuery activityIdIn(params string[] activityIdIn)
	  {
		ensureNotNull("activityIdIn", (object[]) activityIdIn);
		this.activityIdIn_Conflict = activityIdIn;
		return this;
	  }
	  public virtual ExternalTaskQuery priorityHigherThanOrEquals(long priority)
	  {
		this.priorityHigherThanOrEquals_Conflict = priority;
		return this;
	  }

	  public virtual ExternalTaskQuery priorityLowerThanOrEquals(long priority)
	  {
		this.priorityLowerThanOrEquals_Conflict = priority;
		return this;
	  }


	  public virtual ExternalTaskQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual ExternalTaskQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual ExternalTaskQuery withRetriesLeft()
	  {
		this.retriesLeft = true;
		return this;
	  }

	  public virtual ExternalTaskQuery noRetriesLeft()
	  {
		this.retriesLeft = false;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(priorityHigherThanOrEquals_Conflict, priorityLowerThanOrEquals_Conflict);
	  }

	  public virtual ExternalTaskQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual ExternalTaskQuery orderById()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.ID);
	  }

	  public virtual ExternalTaskQuery orderByLockExpirationTime()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.LOCK_EXPIRATION_TIME);
	  }

	  public virtual ExternalTaskQuery orderByProcessInstanceId()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.PROCESS_INSTANCE_ID);
	  }

	  public virtual ExternalTaskQuery orderByProcessDefinitionId()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual ExternalTaskQuery orderByProcessDefinitionKey()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.PROCESS_DEFINITION_KEY);
	  }

	  public virtual ExternalTaskQuery orderByTenantId()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual ExternalTaskQuery orderByPriority()
	  {
		return orderBy(ExternalTaskQueryProperty_Fields.PRIORITY);
	  }
	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.ExternalTaskManager.findExternalTaskCountByQueryCriteria(this);
	  }

	  public override IList<ExternalTask> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.ExternalTaskManager.findExternalTasksByQueryCriteria(this);
	  }

	  public override IList<string> executeIdsList(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.ExternalTaskManager.findExternalTaskIdsByQueryCriteria(this);
	  }

	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId_Conflict;
		  }
	  }

	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId_Conflict;
		  }
	  }

	  public virtual DateTime LockExpirationBefore
	  {
		  get
		  {
			return lockExpirationBefore_Conflict;
		  }
	  }

	  public virtual DateTime LockExpirationAfter
	  {
		  get
		  {
			return lockExpirationAfter_Conflict;
		  }
	  }

	  public virtual string TopicName
	  {
		  get
		  {
			return topicName_Conflict;
		  }
	  }

	  public virtual bool? Locked
	  {
		  get
		  {
			return locked_Conflict;
		  }
	  }

	  public virtual bool? NotLocked
	  {
		  get
		  {
			return notLocked_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
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

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Conflict;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
	  }

	  public virtual bool? RetriesLeft
	  {
		  get
		  {
			return retriesLeft;
		  }
	  }

	  public virtual DateTime Now
	  {
		  get
		  {
			return ClockUtil.CurrentTime;
		  }
	  }

	}

}