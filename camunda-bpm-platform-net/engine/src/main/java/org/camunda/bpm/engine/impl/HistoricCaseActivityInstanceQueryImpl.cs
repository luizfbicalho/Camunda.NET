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
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class HistoricCaseActivityInstanceQueryImpl : AbstractQuery<HistoricCaseActivityInstanceQuery, HistoricCaseActivityInstance>, HistoricCaseActivityInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string[] caseActivityInstanceIds;
	  protected internal string[] caseActivityIds;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseActivityName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseActivityType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdAfter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime endedBefore_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime endedAfter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? ended_Renamed;
	  protected internal int? caseActivityInstanceState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? required_Renamed;
	  protected internal string[] tenantIds;

	  public HistoricCaseActivityInstanceQueryImpl()
	  {
	  }

	  public HistoricCaseActivityInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricCaseActivityInstanceManager.findHistoricCaseActivityInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricCaseActivityInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricCaseActivityInstanceManager.findHistoricCaseActivityInstancesByQueryCriteria(this, page);
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityInstanceId(string caseActivityInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityInstanceId", caseActivityInstanceId);
		return caseActivityInstanceIdIn(caseActivityInstanceId);
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityInstanceIdIn(params string[] caseActivityInstanceIds)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityInstanceIds", (object[]) caseActivityInstanceIds);
		this.caseActivityInstanceIds = caseActivityInstanceIds;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceId", caseInstanceId);
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Renamed = caseDefinitionId;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseExecutionId", caseExecutionId);
		return caseActivityInstanceIdIn(caseExecutionId);
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityId(string caseActivityId)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityId", caseActivityId);
		return caseActivityIdIn(caseActivityId);
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityIdIn(params string[] caseActivityIds)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityIds", (object[]) caseActivityIds);
		this.caseActivityIds = caseActivityIds;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityName(string caseActivityName)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityName", caseActivityName);
		this.caseActivityName_Renamed = caseActivityName;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery caseActivityType(string caseActivityType)
	  {
		ensureNotNull(typeof(NotValidException), "caseActivityType", caseActivityType);
		this.caseActivityType_Renamed = caseActivityType;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery createdBefore(DateTime date)
	  {
		ensureNotNull(typeof(NotValidException), "createdBefore", date);
		this.createdBefore_Renamed = date;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery createdAfter(DateTime date)
	  {
		ensureNotNull(typeof(NotValidException), "createdAfter", date);
		this.createdAfter_Renamed = date;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery endedBefore(DateTime date)
	  {
		ensureNotNull(typeof(NotValidException), "finishedBefore", date);
		this.endedBefore_Renamed = date;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery endedAfter(DateTime date)
	  {
		ensureNotNull(typeof(NotValidException), "finishedAfter", date);
		this.endedAfter_Renamed = date;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery required()
	  {
		this.required_Renamed = true;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery ended()
	  {
		this.ended_Renamed = true;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery notEnded()
	  {
		this.ended_Renamed = false;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery available()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = AVAILABLE.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery enabled()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = ENABLED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery disabled()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = DISABLED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery active()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = ACTIVE.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery suspended()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = SUSPENDED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery completed()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = COMPLETED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery terminated()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case activity instance state '" + caseActivityInstanceState + "'", "caseActivityState", caseActivityInstanceState);
		this.caseActivityInstanceState = TERMINATED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(createdAfter_Renamed, createdBefore_Renamed) || CompareUtil.areNotInAscendingOrder(endedAfter_Renamed, endedBefore_Renamed);
	  }

	  // ordering

	  public virtual HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceId()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.HISTORIC_CASE_ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseInstanceId()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CASE_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseExecutionId()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.HISTORIC_CASE_ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseActivityId()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CASE_ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseActivityName()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CASE_ACTIVITY_NAME);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseActivityType()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CASE_ACTIVITY_TYPE);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceCreateTime()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CREATE);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceEndTime()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.END);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByHistoricCaseActivityInstanceDuration()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.DURATION);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByCaseDefinitionId()
	  {
		orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.CASE_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricCaseActivityInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricCaseActivityInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  // getter

	  public virtual string[] CaseActivityInstanceIds
	  {
		  get
		  {
			return caseActivityInstanceIds;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Renamed;
		  }
	  }

	  public virtual string[] CaseActivityIds
	  {
		  get
		  {
			return caseActivityIds;
		  }
	  }

	  public virtual string CaseActivityName
	  {
		  get
		  {
			return caseActivityName_Renamed;
		  }
	  }

	  public virtual string CaseActivityType
	  {
		  get
		  {
			return caseActivityType_Renamed;
		  }
	  }

	  public virtual DateTime CreatedBefore
	  {
		  get
		  {
			return createdBefore_Renamed;
		  }
	  }

	  public virtual DateTime CreatedAfter
	  {
		  get
		  {
			return createdAfter_Renamed;
		  }
	  }

	  public virtual DateTime EndedBefore
	  {
		  get
		  {
			return endedBefore_Renamed;
		  }
	  }

	  public virtual DateTime EndedAfter
	  {
		  get
		  {
			return endedAfter_Renamed;
		  }
	  }

	  public virtual bool? Ended
	  {
		  get
		  {
			return ended_Renamed;
		  }
	  }

	  public virtual int? CaseActivityInstanceState
	  {
		  get
		  {
			return caseActivityInstanceState;
		  }
	  }

	  public virtual bool? Required
	  {
		  get
		  {
			return required_Renamed;
		  }
	  }

	}

}