﻿using System;
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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;


	/// <summary>
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class JobQueryImpl : AbstractQuery<JobQuery, Job>, JobQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
	  protected internal bool retriesLeft;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool executable_Conflict;
	  protected internal bool onlyTimers;
	  protected internal bool onlyMessages;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime duedateHigherThan_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime duedateLowerThan_Conflict;
	  protected internal DateTime duedateHigherThanOrEqual;
	  protected internal DateTime duedateLowerThanOrEqual;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdAfter_Conflict;
	  protected internal long? priorityHigherThanOrEqual;
	  protected internal long? priorityLowerThanOrEqual;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withException_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string exceptionMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool noRetriesLeft_Conflict;
	  protected internal SuspensionState suspensionState;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeJobsWithoutTenantId_Conflict = false;

	  public JobQueryImpl()
	  {
	  }

	  public JobQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual JobQuery jobId(string jobId)
	  {
		ensureNotNull("Provided job id", jobId);
		this.id = jobId;
		return this;
	  }

	  public virtual JobQuery jobDefinitionId(string jobDefinitionId)
	  {
		ensureNotNull("Provided job definition id", jobDefinitionId);
		this.jobDefinitionId_Conflict = jobDefinitionId;
		return this;
	  }

	  public virtual JobQueryImpl processInstanceId(string processInstanceId)
	  {
		ensureNotNull("Provided process instance id", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual JobQueryImpl executionId(string executionId)
	  {
		ensureNotNull("Provided execution id", executionId);
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual JobQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("Provided process definition id", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual JobQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("Provided process instance key", processDefinitionKey);
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual JobQuery activityId(string activityId)
	  {
		ensureNotNull("Provided activity id", activityId);
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual JobQuery withRetriesLeft()
	  {
		retriesLeft = true;
		return this;
	  }

	  public virtual JobQuery executable()
	  {
		executable_Conflict = true;
		return this;
	  }

	  public virtual JobQuery timers()
	  {
		if (onlyMessages)
		{
		  throw new ProcessEngineException("Cannot combine onlyTimers() with onlyMessages() in the same query");
		}
		this.onlyTimers = true;
		return this;
	  }

	  public virtual JobQuery messages()
	  {
		if (onlyTimers)
		{
		  throw new ProcessEngineException("Cannot combine onlyTimers() with onlyMessages() in the same query");
		}
		this.onlyMessages = true;
		return this;
	  }

	  public virtual JobQuery duedateHigherThan(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.duedateHigherThan_Conflict = date;
		return this;
	  }

	  public virtual JobQuery duedateLowerThan(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.duedateLowerThan_Conflict = date;
		return this;
	  }

	  public virtual JobQuery duedateHigherThen(DateTime date)
	  {
		return duedateHigherThan(date);
	  }

	  public virtual JobQuery duedateHigherThenOrEquals(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.duedateHigherThanOrEqual = date;
		return this;
	  }

	  public virtual JobQuery duedateLowerThen(DateTime date)
	  {
		return duedateLowerThan(date);
	  }

	  public virtual JobQuery duedateLowerThenOrEquals(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.duedateLowerThanOrEqual = date;
		return this;
	  }

	  public virtual JobQuery createdBefore(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.createdBefore_Conflict = date;
		return this;
	  }

	  public virtual JobQuery createdAfter(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.createdAfter_Conflict = date;
		return this;
	  }

		public virtual JobQuery priorityHigherThanOrEquals(long priority)
		{
		this.priorityHigherThanOrEqual = priority;
		return this;
		}

	  public virtual JobQuery priorityLowerThanOrEquals(long priority)
	  {
		this.priorityLowerThanOrEqual = priority;
		return this;
	  }

	  public virtual JobQuery withException()
	  {
		this.withException_Conflict = true;
		return this;
	  }

	  public virtual JobQuery exceptionMessage(string exceptionMessage)
	  {
		ensureNotNull("Provided exception message", exceptionMessage);
		this.exceptionMessage_Conflict = exceptionMessage;
		return this;
	  }

	  public virtual JobQuery noRetriesLeft()
	  {
		noRetriesLeft_Conflict = true;
		return this;
	  }

	  public virtual JobQuery active()
	  {
		suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual JobQuery suspended()
	  {
		suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(priorityHigherThanOrEqual, priorityLowerThanOrEqual) || hasExcludingDueDateParameters() || CompareUtil.areNotInAscendingOrder(createdBefore_Conflict, createdAfter_Conflict);
	  }

	  private bool hasExcludingDueDateParameters()
	  {
		IList<DateTime> dueDates = new List<DateTime>();
		if (duedateHigherThan_Conflict != null && duedateHigherThanOrEqual != null)
		{
		  dueDates.Add(CompareUtil.min(duedateHigherThan_Conflict, duedateHigherThanOrEqual));
		  dueDates.Add(CompareUtil.max(duedateHigherThan_Conflict, duedateHigherThanOrEqual));
		}
		else if (duedateHigherThan_Conflict != null)
		{
		  dueDates.Add(duedateHigherThan_Conflict);
		}
		else if (duedateHigherThanOrEqual != null)
		{
		  dueDates.Add(duedateHigherThanOrEqual);
		}

		if (duedateLowerThan_Conflict != null && duedateLowerThanOrEqual != null)
		{
		  dueDates.Add(CompareUtil.min(duedateLowerThan_Conflict, duedateLowerThanOrEqual));
		  dueDates.Add(CompareUtil.max(duedateLowerThan_Conflict, duedateLowerThanOrEqual));
		}
		else if (duedateLowerThan_Conflict != null)
		{
		  dueDates.Add(duedateLowerThan_Conflict);
		}
		else if (duedateLowerThanOrEqual != null)
		{
		  dueDates.Add(duedateLowerThanOrEqual);
		}

		return CompareUtil.areNotInAscendingOrder(dueDates);
	  }

	  public virtual JobQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual JobQuery withoutTenantId()
	  {
		isTenantIdSet = true;
		this.tenantIds = null;
		return this;
	  }

	  public virtual JobQuery includeJobsWithoutTenantId()
	  {
		this.includeJobsWithoutTenantId_Conflict = true;
		return this;
	  }

	  //sorting //////////////////////////////////////////

	  public virtual JobQuery orderByJobDuedate()
	  {
		return orderBy(JobQueryProperty_Fields.DUEDATE);
	  }

	  public virtual JobQuery orderByExecutionId()
	  {
		return orderBy(JobQueryProperty_Fields.EXECUTION_ID);
	  }

	  public virtual JobQuery orderByJobId()
	  {
		return orderBy(JobQueryProperty_Fields.JOB_ID);
	  }

	  public virtual JobQuery orderByProcessInstanceId()
	  {
		return orderBy(JobQueryProperty_Fields.PROCESS_INSTANCE_ID);
	  }

	  public virtual JobQuery orderByProcessDefinitionId()
	  {
		return orderBy(JobQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual JobQuery orderByProcessDefinitionKey()
	  {
		return orderBy(JobQueryProperty_Fields.PROCESS_DEFINITION_KEY);
	  }

	  public virtual JobQuery orderByJobRetries()
	  {
		return orderBy(JobQueryProperty_Fields.RETRIES);
	  }

	  public virtual JobQuery orderByJobPriority()
	  {
		return orderBy(JobQueryProperty_Fields.PRIORITY);
	  }

	  public virtual JobQuery orderByTenantId()
	  {
		return orderBy(JobQueryProperty_Fields.TENANT_ID);
	  }

	  //results //////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.JobManager.findJobCountByQueryCriteria(this);
	  }

	  public override IList<Job> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.JobManager.findJobsByQueryCriteria(this, page);
	  }

	  //getters //////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }
	  public virtual bool RetriesLeft
	  {
		  get
		  {
			return retriesLeft;
		  }
	  }
	  public virtual bool Executable
	  {
		  get
		  {
			return executable_Conflict;
		  }
	  }
	  public virtual DateTime Now
	  {
		  get
		  {
			return ClockUtil.CurrentTime;
		  }
	  }
	  public virtual bool WithException
	  {
		  get
		  {
			return withException_Conflict;
		  }
	  }
	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage_Conflict;
		  }
	  }

	}

}