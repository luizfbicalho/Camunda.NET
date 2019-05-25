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

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsEmptyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
	using static org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Bernd Ruecker
	/// </summary>
	public class HistoricProcessInstanceQueryImpl : AbstractVariableQueryImpl<HistoricProcessInstanceQuery, HistoricProcessInstance>, HistoricProcessInstanceQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionNameLike_Conflict;
	  protected internal string businessKey;
	  protected internal string businessKeyLike;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool finished_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool unfinished_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withIncidents_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withRootIncidents_Conflict = false;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentStatus_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessageLike_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string startedBy_Conflict;
	  protected internal bool isRootProcessInstances;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superProcessInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subProcessInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superCaseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subCaseInstanceId_Conflict;
	  protected internal IList<string> processKeyNotIn;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executedActivityAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executedActivityBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executedJobAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime executedJobBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ISet<string> processInstanceIds_Conflict;
	  protected internal string[] tenantIds;
	  protected internal bool isTenantIdSet;
	  protected internal string[] executedActivityIds;
	  protected internal string[] activeActivityIds;
	  protected internal string state;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;

	  public HistoricProcessInstanceQueryImpl()
	  {
	  }

	  public HistoricProcessInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricProcessInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processInstanceIds(ISet<string> processInstanceIds)
	  {
		ensureNotEmpty("Set of process instance ids", processInstanceIds);
		this.processInstanceIds_Conflict = processInstanceIds;
		return this;
	  }

	  public virtual HistoricProcessInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processDefinitionName(string processDefinitionName)
	  {
		this.processDefinitionName_Conflict = processDefinitionName;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processDefinitionNameLike(string nameLike)
	  {
		this.processDefinitionNameLike_Conflict = nameLike;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processInstanceBusinessKey(string businessKey)
	  {
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processInstanceBusinessKeyLike(string businessKeyLike)
	  {
		this.businessKeyLike = businessKeyLike;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery finished()
	  {
		this.finished_Conflict = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery unfinished()
	  {
		this.unfinished_Conflict = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery withIncidents()
	  {
		this.withIncidents_Conflict = true;

		return this;
	  }

	  public virtual HistoricProcessInstanceQuery withRootIncidents()
	  {
		this.withRootIncidents_Conflict = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery incidentType(string incidentType)
	  {
		ensureNotNull("incident type", incidentType);
		this.incidentType_Conflict = incidentType;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery incidentStatus(string status)
	  {
		this.incidentStatus_Conflict = status;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery incidentMessage(string incidentMessage)
	  {
		ensureNotNull("incidentMessage", incidentMessage);
		this.incidentMessage_Conflict = incidentMessage;

		return this;
	  }

	  public virtual HistoricProcessInstanceQuery incidentMessageLike(string incidentMessageLike)
	  {
		ensureNotNull("incidentMessageLike", incidentMessageLike);
		this.incidentMessageLike_Conflict = incidentMessageLike;

		return this;
	  }

	  public virtual HistoricProcessInstanceQuery startedBy(string userId)
	  {
		this.startedBy_Conflict = userId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery processDefinitionKeyNotIn(IList<string> processDefinitionKeys)
	  {
		ensureNotContainsNull("processDefinitionKeys", processDefinitionKeys);
		ensureNotContainsEmptyString("processDefinitionKeys", processDefinitionKeys);
		this.processKeyNotIn = processDefinitionKeys;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery startedAfter(DateTime date)
	  {
		startedAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery startedBefore(DateTime date)
	  {
		startedBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery finishedAfter(DateTime date)
	  {
		finishedAfter_Conflict = date;
		finished_Conflict = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery finishedBefore(DateTime date)
	  {
		finishedBefore_Conflict = date;
		finished_Conflict = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery rootProcessInstances()
	  {
		if (!string.ReferenceEquals(superProcessInstanceId_Conflict, null))
		{
		  throw new BadUserRequestException("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId");
		}
		if (!string.ReferenceEquals(superCaseInstanceId_Conflict, null))
		{
		  throw new BadUserRequestException("Invalid query usage: cannot set both rootProcessInstances and superCaseInstanceId");
		}
		isRootProcessInstances = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery superProcessInstanceId(string superProcessInstanceId)
	  {
		if (isRootProcessInstances)
		{
		  throw new BadUserRequestException("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId");
		}
		this.superProcessInstanceId_Conflict = superProcessInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery subProcessInstanceId(string subProcessInstanceId)
	  {
		this.subProcessInstanceId_Conflict = subProcessInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery superCaseInstanceId(string superCaseInstanceId)
	  {
		if (isRootProcessInstances)
		{
		  throw new BadUserRequestException("Invalid query usage: cannot set both rootProcessInstances and superCaseInstanceId");
		}
		this.superCaseInstanceId_Conflict = superCaseInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery subCaseInstanceId(string subCaseInstanceId)
	  {
		this.subCaseInstanceId_Conflict = subCaseInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		this.isTenantIdSet = true;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery withoutTenantId()
	  {
		tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  protected internal override bool hasExcludingConditions()
	  {
		return base.hasExcludingConditions() || (finished_Conflict && unfinished_Conflict) || CompareUtil.areNotInAscendingOrder(startedAfter_Conflict, startedBefore_Conflict) || CompareUtil.areNotInAscendingOrder(finishedAfter_Conflict, finishedBefore_Conflict) || CompareUtil.elementIsContainedInList(processDefinitionKey_Conflict, processKeyNotIn) || CompareUtil.elementIsNotContainedInList(processInstanceId_Conflict, processInstanceIds_Conflict);
	  }

		public virtual HistoricProcessInstanceQuery orderByProcessInstanceBusinessKey()
		{
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.BUSINESS_KEY);
		}

	  public virtual HistoricProcessInstanceQuery orderByProcessInstanceDuration()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.DURATION);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessInstanceStartTime()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.START_TIME);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessInstanceEndTime()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.END_TIME);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessDefinitionId()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_ID);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessDefinitionKey()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_KEY);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessDefinitionName()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_NAME);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessDefinitionVersion()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_VERSION);
	  }

	  public virtual HistoricProcessInstanceQuery orderByProcessInstanceId()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID_);
	  }

	  public virtual HistoricProcessInstanceQuery orderByTenantId()
	  {
		return orderBy(HistoricProcessInstanceQueryProperty_Fields.TENANT_ID);
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricProcessInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstancesByQueryCriteria(this, page);
	  }

	  public virtual IList<string> executeIdsList(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricProcessInstanceManager.findHistoricProcessInstanceIds(this);
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

	  public virtual bool Open
	  {
		  get
		  {
			return unfinished_Conflict;
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

	  public virtual string ProcessDefinitionIdLike
	  {
		  get
		  {
			return processDefinitionKey_Conflict + ":%:%";
		  }
	  }

	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionNameLike
	  {
		  get
		  {
			return processDefinitionNameLike_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual ISet<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Conflict;
		  }
	  }

	  public virtual string StartedBy
	  {
		  get
		  {
			return startedBy_Conflict;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId_Conflict;
		  }
		  set
		  {
			this.superProcessInstanceId_Conflict = value;
		  }
	  }


	  public virtual IList<string> ProcessKeyNotIn
	  {
		  get
		  {
			return processKeyNotIn;
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

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType_Conflict;
		  }
	  }

	  public virtual string IncidentMessage
	  {
		  get
		  {
			return this.incidentMessage_Conflict;
		  }
	  }

	  public virtual string IncidentMessageLike
	  {
		  get
		  {
			return this.incidentMessageLike_Conflict;
		  }
	  }

	  public virtual bool WithRootIncidents
	  {
		  get
		  {
			return withRootIncidents_Conflict;
		  }
	  }

	  // below is deprecated and to be removed in 5.12

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startDateBy_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startDateOn_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishDateBy_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime finishDateOn_Conflict;
	  protected internal DateTime startDateOnBegin;
	  protected internal DateTime startDateOnEnd;
	  protected internal DateTime finishDateOnBegin;
	  protected internal DateTime finishDateOnEnd;

	  [Obsolete]
	  public virtual HistoricProcessInstanceQuery startDateBy(DateTime date)
	  {
		this.startDateBy_Conflict = this.calculateMidnight(date);
		return this;
	  }

	  [Obsolete]
	  public virtual HistoricProcessInstanceQuery startDateOn(DateTime date)
	  {
		this.startDateOn_Conflict = date;
		this.startDateOnBegin = this.calculateMidnight(date);
		this.startDateOnEnd = this.calculateBeforeMidnight(date);
		return this;
	  }

	  [Obsolete]
	  public virtual HistoricProcessInstanceQuery finishDateBy(DateTime date)
	  {
		this.finishDateBy_Conflict = this.calculateBeforeMidnight(date);
		return this;
	  }

	  [Obsolete]
	  public virtual HistoricProcessInstanceQuery finishDateOn(DateTime date)
	  {
		this.finishDateOn_Conflict = date;
		this.finishDateOnBegin = this.calculateMidnight(date);
		this.finishDateOnEnd = this.calculateBeforeMidnight(date);
		return this;
	  }

	  [Obsolete]
	  private DateTime calculateBeforeMidnight(DateTime date)
	  {
		DateTime cal = new DateTime();
		cal = new DateTime(date);
		cal.AddDays(1);
		cal.AddSeconds(-1);
		return cal;
	  }

	  [Obsolete]
	  private DateTime calculateMidnight(DateTime date)
	  {
		DateTime cal = new DateTime();
		cal = new DateTime(date);
		cal.set(DateTime.MILLISECOND, 0);
		cal.set(DateTime.SECOND, 0);
		cal.set(DateTime.MINUTE, 0);
		cal.set(DateTime.HOUR, 0);
		return cal;
	  }

	  public virtual bool RootProcessInstances
	  {
		  get
		  {
			return isRootProcessInstances;
		  }
	  }

	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId_Conflict;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId_Conflict;
		  }
	  }

	  public virtual string SubCaseInstanceId
	  {
		  get
		  {
			return subCaseInstanceId_Conflict;
		  }
	  }

	  public virtual HistoricProcessInstanceQuery executedActivityAfter(DateTime date)
	  {
		this.executedActivityAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery executedActivityBefore(DateTime date)
	  {
		this.executedActivityBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery executedJobAfter(DateTime date)
	  {
		this.executedJobAfter_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery executedJobBefore(DateTime date)
	  {
		this.executedJobBefore_Conflict = date;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery executedActivityIdIn(params string[] ids)
	  {
		ensureNotNull(typeof(BadUserRequestException), "activity ids", (object[]) ids);
		ensureNotContainsNull(typeof(BadUserRequestException), "activity ids", Arrays.asList(ids));
		this.executedActivityIds = ids;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery activeActivityIdIn(params string[] ids)
	  {
		ensureNotNull(typeof(BadUserRequestException), "activity ids", (object[]) ids);
		ensureNotContainsNull(typeof(BadUserRequestException), "activity ids", Arrays.asList(ids));
		this.activeActivityIds = ids;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery active()
	  {
		ensureNull(typeof(BadUserRequestException), "Already querying for historic process instance with another state", state, state);
		state = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery suspended()
	  {
		ensureNull(typeof(BadUserRequestException), "Already querying for historic process instance with another state", state, state);
		state = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery completed()
	  {
		ensureNull(typeof(BadUserRequestException), "Already querying for historic process instance with another state", state, state);
		state = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery externallyTerminated()
	  {
		ensureNull(typeof(BadUserRequestException), "Already querying for historic process instance with another state", state, state);
		state = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_EXTERNALLY_TERMINATED;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery internallyTerminated()
	  {
		ensureNull(typeof(BadUserRequestException), "Already querying for historic process instance with another state", state, state);
		state = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_INTERNALLY_TERMINATED;
		return this;
	  }
	}

}