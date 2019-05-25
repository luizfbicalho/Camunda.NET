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
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	[Serializable]
	public class IncidentQueryImpl : AbstractQuery<IncidentQuery, Incident>, IncidentQuery
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
	  protected internal string[] tenantIds;
	  protected internal string[] jobDefinitionIds;

	  public IncidentQueryImpl()
	  {
	  }

	  public IncidentQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual IncidentQuery incidentId(string incidentId)
	  {
		this.id = incidentId;
		return this;
	  }

	  public virtual IncidentQuery incidentType(string incidentType)
	  {
		this.incidentType_Conflict = incidentType;
		return this;
	  }

	  public virtual IncidentQuery incidentMessage(string incidentMessage)
	  {
		this.incidentMessage_Conflict = incidentMessage;
		return this;
	  }

	  public virtual IncidentQuery executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual IncidentQuery activityId(string activityId)
	  {
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual IncidentQuery processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual IncidentQuery processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual IncidentQuery causeIncidentId(string causeIncidentId)
	  {
		this.causeIncidentId_Conflict = causeIncidentId;
		return this;
	  }

	  public virtual IncidentQuery rootCauseIncidentId(string rootCauseIncidentId)
	  {
		this.rootCauseIncidentId_Conflict = rootCauseIncidentId;
		return this;
	  }

	  public virtual IncidentQuery configuration(string configuration)
	  {
		this.configuration_Conflict = configuration;
		return this;
	  }

	  public virtual IncidentQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		return this;
	  }

	  public virtual IncidentQuery jobDefinitionIdIn(params string[] jobDefinitionIds)
	  {
		ensureNotNull("jobDefinitionIds", (object[]) jobDefinitionIds);
		this.jobDefinitionIds = jobDefinitionIds;
		return this;
	  }

	  //ordering ////////////////////////////////////////////////////

	  public virtual IncidentQuery orderByIncidentId()
	  {
		orderBy(IncidentQueryProperty_Fields.INCIDENT_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByIncidentTimestamp()
	  {
		orderBy(IncidentQueryProperty_Fields.INCIDENT_TIMESTAMP);
		return this;
	  }

	  public virtual IncidentQuery orderByIncidentType()
	  {
		orderBy(IncidentQueryProperty_Fields.INCIDENT_TYPE);
		return this;
	  }

	  public virtual IncidentQuery orderByExecutionId()
	  {
		orderBy(IncidentQueryProperty_Fields.EXECUTION_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByActivityId()
	  {
		orderBy(IncidentQueryProperty_Fields.ACTIVITY_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByProcessInstanceId()
	  {
		orderBy(IncidentQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByProcessDefinitionId()
	  {
		orderBy(IncidentQueryProperty_Fields.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByCauseIncidentId()
	  {
		orderBy(IncidentQueryProperty_Fields.CAUSE_INCIDENT_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByRootCauseIncidentId()
	  {
		orderBy(IncidentQueryProperty_Fields.ROOT_CAUSE_INCIDENT_ID);
		return this;
	  }

	  public virtual IncidentQuery orderByConfiguration()
	  {
		orderBy(IncidentQueryProperty_Fields.CONFIGURATION);
		return this;
	  }

	  public virtual IncidentQuery orderByTenantId()
	  {
		return orderBy(IncidentQueryProperty_Fields.TENANT_ID);
	  }

	  public virtual IncidentQuery orderByIncidentMessage()
	  {
		return orderBy(IncidentQueryProperty_Fields.INCIDENT_MESSAGE);
	  }

	  //results ////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.IncidentManager.findIncidentCountByQueryCriteria(this);
	  }

	  public override IList<Incident> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.IncidentManager.findIncidentByQueryCriteria(this, page);
	  }

	}

}