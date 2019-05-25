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


	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ActivityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class HistoricActivityInstanceQueryImpl : AbstractQuery<HistoricActivityInstanceQuery, HistoricActivityInstance>, HistoricActivityInstanceQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityType_Conflict;
	  protected internal string assignee;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool finished_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool unfinished_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedAfter_Conflict;
	  protected internal ActivityInstanceState activityInstanceState;
	  protected internal string[] tenantIds;

	  public HistoricActivityInstanceQueryImpl()
	  {
	  }

	  public HistoricActivityInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricActivityInstanceManager.findHistoricActivityInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricActivityInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricActivityInstanceManager.findHistoricActivityInstancesByQueryCriteria(this, page);
	  }

	  public virtual HistoricActivityInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityId(string activityId)
	  {
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityName(string activityName)
	  {
		this.activityName_Conflict = activityName;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityType(string activityType)
	  {
		this.activityType_Conflict = activityType;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl taskAssignee(string assignee)
	  {
		this.assignee = assignee;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl finished()
	  {
		this.finished_Conflict = true;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl unfinished()
	  {
		this.unfinished_Conflict = true;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl completeScope()
	  {
		if (activityInstanceState != null)
		{
		  throw new ProcessEngineException("Already querying for activity instance state <" + activityInstanceState + ">");
		}

		this.activityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.SCOPE_COMPLETE;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl canceled()
	  {
		if (activityInstanceState != null)
		{
		  throw new ProcessEngineException("Already querying for activity instance state <" + activityInstanceState + ">");
		}
		this.activityInstanceState = org.camunda.bpm.engine.impl.pvm.runtime.ActivityInstanceState_Fields.CANCELED;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl startedAfter(DateTime date)
	  {
		startedAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl startedBefore(DateTime date)
	  {
		startedBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl finishedAfter(DateTime date)
	  {
		finishedAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl finishedBefore(DateTime date)
	  {
		finishedBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricActivityInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(startedAfter_Conflict, startedBefore_Conflict) || CompareUtil.areNotInAscendingOrder(finishedAfter_Conflict, finishedBefore_Conflict);
	  }

	  // ordering /////////////////////////////////////////////////////////////////

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceDuration()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.DURATION);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceEndTime()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.END);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByExecutionId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.HISTORIC_ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByProcessDefinitionId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByProcessInstanceId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceStartTime()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.START);
		return this;
	  }

	  public virtual HistoricActivityInstanceQuery orderByActivityId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByActivityName()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.ACTIVITY_NAME);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByActivityType()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.ACTIVITY_TYPE);
		return this;
	  }

	  public virtual HistoricActivityInstanceQuery orderPartiallyByOccurrence()
	  {
		orderBy(HistoricActivityInstanceQueryProperty_Fields.SEQUENCE_COUNTER);
		return this;
	  }

	  public virtual HistoricActivityInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricActivityInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  // getters and setters //////////////////////////////////////////////////////

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
	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName_Conflict;
		  }
	  }
	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType_Conflict;
		  }
	  }
	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }
	  public virtual bool Finished
	  {
		  get
		  {
			return finished_Conflict;
		  }
	  }
	  public virtual bool Unfinished
	  {
		  get
		  {
			return unfinished_Conflict;
		  }
	  }
	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Conflict;
		  }
	  }
	  public virtual DateTime StartedAfter
	  {
		  get
		  {
			return startedAfter_Conflict;
		  }
	  }
	  public virtual DateTime StartedBefore
	  {
		  get
		  {
			return startedBefore_Conflict;
		  }
	  }
	  public virtual DateTime FinishedAfter
	  {
		  get
		  {
			return finishedAfter_Conflict;
		  }
	  }
	  public virtual DateTime FinishedBefore
	  {
		  get
		  {
			return finishedBefore_Conflict;
		  }
	  }
	  public virtual ActivityInstanceState ActivityInstanceState
	  {
		  get
		  {
			return activityInstanceState;
		  }
	  }
	}

}