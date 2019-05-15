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
	  protected internal string activityId_Renamed;
	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string jobDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
	  protected internal bool retriesLeft;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool executable_Renamed;
	  protected internal bool onlyTimers;
	  protected internal bool onlyMessages;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime duedateHigherThan_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime duedateLowerThan_Renamed;
	  protected internal DateTime duedateHigherThanOrEqual;
	  protected internal DateTime duedateLowerThanOrEqual;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdAfter_Renamed;
	  protected internal long? priorityHigherThanOrEqual;
	  protected internal long? priorityLowerThanOrEqual;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withException_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string exceptionMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool noRetriesLeft_Renamed;
	  protected internal SuspensionState suspensionState;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool includeJobsWithoutTenantId_Renamed = false;

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
		this.jobDefinitionId_Renamed = jobDefinitionId;
		return this;
	  }

	  public virtual JobQueryImpl processInstanceId(string processInstanceId)
	  {
		ensureNotNull("Provided process instance id", processInstanceId);
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual JobQueryImpl executionId(string executionId)
	  {
		ensureNotNull("Provided execution id", executionId);
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual JobQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("Provided process definition id", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual JobQuery processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("Provided process instance key", processDefinitionKey);
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual JobQuery activityId(string activityId)
	  {
		ensureNotNull("Provided activity id", activityId);
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual JobQuery withRetriesLeft()
	  {
		retriesLeft = true;
		return this;
	  }

	  public virtual JobQuery executable()
	  {
		executable_Renamed = true;
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
		this.duedateHigherThan_Renamed = date;
		return this;
	  }

	  public virtual JobQuery duedateLowerThan(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.duedateLowerThan_Renamed = date;
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
		this.createdBefore_Renamed = date;
		return this;
	  }

	  public virtual JobQuery createdAfter(DateTime date)
	  {
		ensureNotNull("Provided date", date);
		this.createdAfter_Renamed = date;
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
		this.withException_Renamed = true;
		return this;
	  }

	  public virtual JobQuery exceptionMessage(string exceptionMessage)
	  {
		ensureNotNull("Provided exception message", exceptionMessage);
		this.exceptionMessage_Renamed = exceptionMessage;
		return this;
	  }

	  public virtual JobQuery noRetriesLeft()
	  {
		noRetriesLeft_Renamed = true;
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
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(priorityHigherThanOrEqual, priorityLowerThanOrEqual) || hasExcludingDueDateParameters() || CompareUtil.areNotInAscendingOrder(createdBefore_Renamed, createdAfter_Renamed);
	  }

	  private bool hasExcludingDueDateParameters()
	  {
		IList<DateTime> dueDates = new List<DateTime>();
		if (duedateHigherThan_Renamed != null && duedateHigherThanOrEqual != null)
		{
		  dueDates.Add(CompareUtil.min(duedateHigherThan_Renamed, duedateHigherThanOrEqual));
		  dueDates.Add(CompareUtil.max(duedateHigherThan_Renamed, duedateHigherThanOrEqual));
		}
		else if (duedateHigherThan_Renamed != null)
		{
		  dueDates.Add(duedateHigherThan_Renamed);
		}
		else if (duedateHigherThanOrEqual != null)
		{
		  dueDates.Add(duedateHigherThanOrEqual);
		}

		if (duedateLowerThan_Renamed != null && duedateLowerThanOrEqual != null)
		{
		  dueDates.Add(CompareUtil.min(duedateLowerThan_Renamed, duedateLowerThanOrEqual));
		  dueDates.Add(CompareUtil.max(duedateLowerThan_Renamed, duedateLowerThanOrEqual));
		}
		else if (duedateLowerThan_Renamed != null)
		{
		  dueDates.Add(duedateLowerThan_Renamed);
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
		this.includeJobsWithoutTenantId_Renamed = true;
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
			return processInstanceId_Renamed;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
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
			return executable_Renamed;
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
			return withException_Renamed;
		  }
	  }
	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage_Renamed;
		  }
	  }

	}

}