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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ProcessInstanceQueryImpl : AbstractVariableQueryImpl<ProcessInstanceQuery, ProcessInstance>, ProcessInstanceQuery
	{

	  private const long serialVersionUID = 1L;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
	  protected internal string businessKey;
	  protected internal string businessKeyLike;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ISet<string> processInstanceIds_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superProcessInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subProcessInstanceId_Renamed;
	  protected internal SuspensionState suspensionState;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withIncident_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentType_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string incidentMessageLike_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superCaseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subCaseInstanceId_Renamed;
	  protected internal string[] activityIds;
	  protected internal bool isRootProcessInstances;
	  protected internal bool isLeafProcessInstances;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;
	  protected internal bool isProcessDefinitionWithoutTenantId = false;

	  public ProcessInstanceQueryImpl()
	  {
	  }

	  public ProcessInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ProcessInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		ensureNotNull("Process instance id", processInstanceId);
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceIds(ISet<string> processInstanceIds)
	  {
		ensureNotEmpty("Set of process instance ids", processInstanceIds);
		this.processInstanceIds_Renamed = processInstanceIds;
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceBusinessKey(string businessKey)
	  {
		ensureNotNull("Business key", businessKey);
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceBusinessKey(string businessKey, string processDefinitionKey)
	  {
		ensureNotNull("Business key", businessKey);
		this.businessKey = businessKey;
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceBusinessKeyLike(string businessKeyLike)
	  {
		this.businessKeyLike = businessKeyLike;
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		ensureNotNull("Process definition id", processDefinitionId);
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl processDefinitionKey(string processDefinitionKey)
	  {
		ensureNotNull("Process definition key", processDefinitionKey);
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual ProcessInstanceQuery deploymentId(string deploymentId)
	  {
		ensureNotNull("Deployment id", deploymentId);
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual ProcessInstanceQuery superProcessInstanceId(string superProcessInstanceId)
	  {
		if (isRootProcessInstances)
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId");
		}
		this.superProcessInstanceId_Renamed = superProcessInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery subProcessInstanceId(string subProcessInstanceId)
	  {
		this.subProcessInstanceId_Renamed = subProcessInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull("caseInstanceId", caseInstanceId);
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery superCaseInstanceId(string superCaseInstanceId)
	  {
		ensureNotNull("superCaseInstanceId", superCaseInstanceId);
		this.superCaseInstanceId_Renamed = superCaseInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery subCaseInstanceId(string subCaseInstanceId)
	  {
		ensureNotNull("subCaseInstanceId", subCaseInstanceId);
		this.subCaseInstanceId_Renamed = subCaseInstanceId;
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessInstanceId()
	  {
		orderBy(ProcessInstanceQueryProperty_Fields.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessDefinitionId()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_PROCESS_DEFINITION, ProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_ID));
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessDefinitionKey()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_PROCESS_DEFINITION, ProcessInstanceQueryProperty_Fields.PROCESS_DEFINITION_KEY));
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByTenantId()
	  {
		orderBy(ProcessInstanceQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByBusinessKey()
	  {
		orderBy(ProcessInstanceQueryProperty_Fields.BUSINESS_KEY);
		return this;
	  }

	  public virtual ProcessInstanceQuery active()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual ProcessInstanceQuery suspended()
	  {
		this.suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual ProcessInstanceQuery withIncident()
	  {
		this.withIncident_Renamed = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery incidentType(string incidentType)
	  {
		ensureNotNull("incident type", incidentType);
		this.incidentType_Renamed = incidentType;
		return this;
	  }

	  public virtual ProcessInstanceQuery incidentId(string incidentId)
	  {
		ensureNotNull("incident id", incidentId);
		this.incidentId_Renamed = incidentId;
		return this;
	  }

	  public virtual ProcessInstanceQuery incidentMessage(string incidentMessage)
	  {
		ensureNotNull("incident message", incidentMessage);
		this.incidentMessage_Renamed = incidentMessage;
		return this;
	  }

	  public virtual ProcessInstanceQuery incidentMessageLike(string incidentMessageLike)
	  {
		ensureNotNull("incident messageLike", incidentMessageLike);
		this.incidentMessageLike_Renamed = incidentMessageLike;
		return this;
	  }

	  public virtual ProcessInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery withoutTenantId()
	  {
		tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery activityIdIn(params string[] activityIds)
	  {
		ensureNotNull("activity ids", (object[]) activityIds);
		this.activityIds = activityIds;
		return this;
	  }

	  public virtual ProcessInstanceQuery rootProcessInstances()
	  {
		if (!string.ReferenceEquals(superProcessInstanceId_Renamed, null))
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set both rootProcessInstances and superProcessInstanceId");
		}
		isRootProcessInstances = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery leafProcessInstances()
	  {
		if (!string.ReferenceEquals(subProcessInstanceId_Renamed, null))
		{
		  throw new ProcessEngineException("Invalid query usage: cannot set both leafProcessInstances and subProcessInstanceId");
		}
		isLeafProcessInstances = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery processDefinitionWithoutTenantId()
	  {
		isProcessDefinitionWithoutTenantId = true;
		return this;
	  }

	  //results /////////////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionManager.findProcessInstanceCountByQueryCriteria(this);
	  }

	  public override IList<ProcessInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionManager.findProcessInstancesByQueryCriteria(this, page);
	  }

	  public virtual IList<string> executeIdsList(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionManager.findProcessInstancesIdsByQueryCriteria(this);
	  }

	  //getters /////////////////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual ISet<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Renamed;
		  }
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

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId_Renamed;
		  }
	  }

	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId_Renamed;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual bool WithIncident
	  {
		  get
		  {
			return withIncident_Renamed;
		  }
	  }

	  public virtual string IncidentId
	  {
		  get
		  {
			return incidentId_Renamed;
		  }
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType_Renamed;
		  }
	  }

	  public virtual string IncidentMessage
	  {
		  get
		  {
			return incidentMessage_Renamed;
		  }
	  }

	  public virtual string IncidentMessageLike
	  {
		  get
		  {
			return incidentMessageLike_Renamed;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId_Renamed;
		  }
	  }

	  public virtual string SubCaseInstanceId
	  {
		  get
		  {
			return subCaseInstanceId_Renamed;
		  }
	  }

	  public virtual bool RootProcessInstances
	  {
		  get
		  {
			return isRootProcessInstances;
		  }
	  }

	  public virtual bool ProcessDefinitionWithoutTenantId
	  {
		  get
		  {
			return isProcessDefinitionWithoutTenantId;
		  }
	  }
	}
}