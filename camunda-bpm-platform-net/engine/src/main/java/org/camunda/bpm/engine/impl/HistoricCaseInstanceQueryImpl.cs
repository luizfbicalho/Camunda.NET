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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class HistoricCaseInstanceQueryImpl : AbstractVariableQueryImpl<HistoricCaseInstanceQuery, HistoricCaseInstance>, HistoricCaseInstanceQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ISet<string> caseInstanceIds_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionNameLike_Conflict;
	  protected internal string businessKey;
	  protected internal string businessKeyLike;
	  protected internal int? state;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? notClosed_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string createdBy_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superCaseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subCaseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superProcessInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subProcessInstanceId_Conflict;
	  protected internal IList<string> caseKeyNotIn;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createdAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime closedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime closedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
	  protected internal string[] caseActivityIds;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;

	  public HistoricCaseInstanceQueryImpl()
	  {
	  }

	  public HistoricCaseInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricCaseInstanceQueryImpl caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseInstanceIds(ISet<string> caseInstanceIds)
	  {
		ensureNotEmpty("Set of case instance ids", caseInstanceIds);
		this.caseInstanceIds_Conflict = caseInstanceIds;
		return this;
	  }

	  public virtual HistoricCaseInstanceQueryImpl caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseDefinitionName(string caseDefinitionName)
	  {
		this.caseDefinitionName_Conflict = caseDefinitionName;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseDefinitionNameLike(string nameLike)
	  {
		this.caseDefinitionNameLike_Conflict = nameLike;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseInstanceBusinessKey(string businessKey)
	  {
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseInstanceBusinessKeyLike(string businessKeyLike)
	  {
		this.businessKeyLike = businessKeyLike;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery createdBy(string userId)
	  {
		this.createdBy_Conflict = userId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseDefinitionKeyNotIn(IList<string> caseDefinitionKeys)
	  {
		ensureNotContainsNull("caseDefinitionKeys", caseDefinitionKeys);
		ensureNotContainsEmptyString("caseDefinitionKeys", caseDefinitionKeys);
		this.caseKeyNotIn = caseDefinitionKeys;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery caseActivityIdIn(params string[] caseActivityIds)
	  {
		ensureNotNull("caseActivityIds", (object[]) caseActivityIds);
		this.caseActivityIds = caseActivityIds;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery createdAfter(DateTime date)
	  {
		createdAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery createdBefore(DateTime date)
	  {
		createdBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery closedAfter(DateTime date)
	  {
		if (state != null && (!state.Equals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED.StateCode)))
		{
		  throw new NotValidException("Already querying for case instance state '" + state + "'");
		}

		closedAfter_Conflict = date;
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery closedBefore(DateTime date)
	  {
		if (state != null && (!state.Equals(org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED.StateCode)))
		{
		  throw new NotValidException("Already querying for case instance state '" + state + "'");
		}

		closedBefore_Conflict = date;
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery superCaseInstanceId(string superCaseInstanceId)
	  {
		  this.superCaseInstanceId_Conflict = superCaseInstanceId;
		  return this;
	  }

	  public virtual HistoricCaseInstanceQuery subCaseInstanceId(string subCaseInstanceId)
	  {
		this.subCaseInstanceId_Conflict = subCaseInstanceId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery superProcessInstanceId(string superProcessInstanceId)
	  {
		this.superProcessInstanceId_Conflict = superProcessInstanceId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery subProcessInstanceId(string subProcessInstanceId)
	  {
		this.subProcessInstanceId_Conflict = subProcessInstanceId;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery withoutTenantId()
	  {
		tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery active()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery completed()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery terminated()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery failed()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.FAILED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery suspended()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery closed()
	  {
		ensureNull(typeof(NotValidException), "Already querying for case instance state '" + state + "'", "state", state);
		this.state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED.StateCode;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery notClosed()
	  {
		this.notClosed_Conflict = true;
		return this;
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseInstanceBusinessKey()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.BUSINESS_KEY);
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseInstanceDuration()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.DURATION);
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseInstanceCreateTime()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.CREATE_TIME);
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseInstanceCloseTime()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.CLOSE_TIME);
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseDefinitionId()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual HistoricCaseInstanceQuery orderByCaseInstanceId()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID_);
	  }

	  public virtual HistoricCaseInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricCaseInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricCaseInstanceManager.findHistoricCaseInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricCaseInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricCaseInstanceManager.findHistoricCaseInstancesByQueryCriteria(this, page);
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || CompareUtil.areNotInAscendingOrder(createdAfter_Conflict, createdBefore_Conflict) || CompareUtil.areNotInAscendingOrder(closedAfter_Conflict, closedBefore_Conflict) || CompareUtil.elementIsNotContainedInList(caseInstanceId_Conflict, caseInstanceIds_Conflict) || CompareUtil.elementIsContainedInList(caseDefinitionKey_Conflict, caseKeyNotIn);
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  public virtual string BusinessKeyLike
	  {
		  get
		  {
			return businessKeyLike;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionIdLike
	  {
		  get
		  {
			return caseDefinitionKey_Conflict + ":%:%";
		  }
	  }

	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionNameLike
	  {
		  get
		  {
			return caseDefinitionNameLike_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual ISet<string> CaseInstanceIds
	  {
		  get
		  {
			return caseInstanceIds_Conflict;
		  }
	  }

	  public virtual string StartedBy
	  {
		  get
		  {
			return createdBy_Conflict;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId_Conflict;
		  }
		  set
		  {
			this.superCaseInstanceId_Conflict = value;
		  }
	  }


	  public virtual IList<string> CaseKeyNotIn
	  {
		  get
		  {
			return caseKeyNotIn;
		  }
	  }

	  public virtual DateTime CreatedAfter
	  {
		  get
		  {
			return createdAfter_Conflict;
		  }
	  }

	  public virtual DateTime CreatedBefore
	  {
		  get
		  {
			return createdBefore_Conflict;
		  }
	  }

	  public virtual DateTime ClosedAfter
	  {
		  get
		  {
			return closedAfter_Conflict;
		  }
	  }

	  public virtual DateTime ClosedBefore
	  {
		  get
		  {
			return closedBefore_Conflict;
		  }
	  }

	  public virtual string SubCaseInstanceId
	  {
		  get
		  {
			return subCaseInstanceId_Conflict;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId_Conflict;
		  }
	  }

	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId_Conflict;
		  }
	  }

	}

}