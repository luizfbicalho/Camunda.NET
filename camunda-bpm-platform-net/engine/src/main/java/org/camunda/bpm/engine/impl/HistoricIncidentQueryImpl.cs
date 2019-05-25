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

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using IncidentState = org.camunda.bpm.engine.history.IncidentState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricIncidentQueryImpl : AbstractVariableQueryImpl<HistoricIncidentQuery, HistoricIncident>, HistoricIncidentQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentType_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string causeIncidentId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string rootCauseIncidentId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string configuration_Conflict;
	  protected internal IncidentState incidentState;
	  protected internal string[] tenantIds;
	  protected internal string[] jobDefinitionIds;

	  public HistoricIncidentQueryImpl()
	  {
	  }

	  public HistoricIncidentQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricIncidentQuery incidentId(string incidentId)
	  {
		ensureNotNull("incidentId", incidentId);
		this.id = incidentId;
		return this;
	  }

	  public virtual HistoricIncidentQuery incidentType(string incidentType)
	  {
		ensureNotNull("incidentType", incidentType);
		this.incidentType_Conflict = incidentType;
		return this;
	  }

	  public virtual HistoricIncidentQuery incidentMessage(string incidentMessage)
	  {
		ensureNotNull("incidentMessage", incidentMessage);
		this.incidentMessage_Conflict = incidentMessage;
		return this;
	  }

	  public virtual HistoricIncidentQuery executionId(string executionId)
	  {
		ensureNotNull("executionId", executionId);
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual HistoricIncidentQuery activityId(string activityId)
	  {
		ensureNotNull("activityId", activityId);
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual HistoricIncidentQuery processInstanceId(string processInstanceId)
	  {
		ensureNotNull("processInstanceId", processInstanceId);
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual HistoricIncidentQuery processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual HistoricIncidentQuery causeIncidentId(string causeIncidentId)
	  {
		ensureNotNull("causeIncidentId", causeIncidentId);
		this.causeIncidentId_Conflict = causeIncidentId;
		return this;
	  }

	  public virtual HistoricIncidentQuery rootCauseIncidentId(string rootCauseIncidentId)
	  {
		ensureNotNull("rootCauseIncidentId", rootCauseIncidentId);
		this.rootCauseIncidentId_Conflict = rootCauseIncidentId;
		return this;
	  }

	  public virtual HistoricIncidentQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual HistoricIncidentQuery configuration(string configuration)
	  {
		ensureNotNull("configuration", configuration);
		this.configuration_Conflict = configuration;
		return this;
	  }

	  public virtual HistoricIncidentQuery jobDefinitionIdIn(params string[] jobDefinitionIds)
	  {
		ensureNotNull("jobDefinitionIds", (object[]) jobDefinitionIds);
		this.jobDefinitionIds = jobDefinitionIds;
		return this;
	  }

	  public virtual HistoricIncidentQuery open()
	  {
		if (incidentState != null)
		{
		  throw new ProcessEngineException("Already querying for incident state <" + incidentState + ">");
		}
		incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.DEFAULT;
		return this;
	  }

	  public virtual HistoricIncidentQuery resolved()
	  {
		if (incidentState != null)
		{
		  throw new ProcessEngineException("Already querying for incident state <" + incidentState + ">");
		}
		incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.RESOLVED;
		return this;
	  }

	  public virtual HistoricIncidentQuery deleted()
	  {
		if (incidentState != null)
		{
		  throw new ProcessEngineException("Already querying for incident state <" + incidentState + ">");
		}
		incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.DELETED;
		return this;
	  }

	  // ordering ////////////////////////////////////////////////////

	  public virtual HistoricIncidentQuery orderByIncidentId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByIncidentMessage()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_MESSAGE);
		return this;
	  }


	  public virtual HistoricIncidentQuery orderByCreateTime()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_CREATE_TIME);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByEndTime()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_END_TIME);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByIncidentType()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_TYPE);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByExecutionId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByActivityId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByProcessDefinitionId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByCauseIncidentId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.CAUSE_INCIDENT_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByRootCauseIncidentId()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.ROOT_CAUSE_INCIDENT_ID);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByConfiguration()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.CONFIGURATION);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByIncidentState()
	  {
		orderBy(HistoricIncidentQueryProperty_Fields.INCIDENT_STATE);
		return this;
	  }

	  public virtual HistoricIncidentQuery orderByTenantId()
	  {
		return orderBy(HistoricIncidentQueryProperty_Fields.TENANT_ID);
	  }

	  // results ////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricIncidentManager.findHistoricIncidentCountByQueryCriteria(this);
	  }

	  public override IList<HistoricIncident> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricIncidentManager.findHistoricIncidentByQueryCriteria(this, page);
	  }


	  // getters /////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
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
			return incidentMessage_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Conflict;
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

	  public virtual string CauseIncidentId
	  {
		  get
		  {
			return causeIncidentId_Conflict;
		  }
	  }

	  public virtual string RootCauseIncidentId
	  {
		  get
		  {
			return rootCauseIncidentId_Conflict;
		  }
	  }

	  public virtual string Configuration
	  {
		  get
		  {
			return configuration_Conflict;
		  }
	  }

	  public virtual IncidentState IncidentState
	  {
		  get
		  {
			return incidentState;
		  }
	  }

	}

}