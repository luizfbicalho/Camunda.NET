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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseExecutionState = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceQueryImpl : AbstractVariableQueryImpl<CaseInstanceQuery, CaseInstance>, CaseInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseExecutionId;
	  protected internal string businessKey;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string deploymentId_Conflict;
	  protected internal CaseExecutionState state;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superProcessInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subProcessInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string superCaseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string subCaseInstanceId_Conflict;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;

	  // Not used by end-users, but needed for dynamic ibatis query
	  protected internal bool? required;
	  protected internal bool? repeatable;
	  protected internal bool? repetition;


	  public CaseInstanceQueryImpl()
	  {
	  }

	  public CaseInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CaseInstanceQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceId", caseInstanceId);
		caseExecutionId = caseInstanceId;
		return this;
	  }

	  public virtual CaseInstanceQuery caseInstanceBusinessKey(string caseInstanceBusinessKey)
	  {
		ensureNotNull(typeof(NotValidException), "businessKey", caseInstanceBusinessKey);
		this.businessKey = caseInstanceBusinessKey;
		return this;
	  }

	  public virtual CaseInstanceQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionKey", caseDefinitionKey);
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual CaseInstanceQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual CaseInstanceQuery deploymentId(string deploymentId)
	  {
		ensureNotNull(typeof(NotValidException), "deploymentId", deploymentId);
		this.deploymentId_Conflict = deploymentId;
		return this;
	  }

	  public virtual CaseInstanceQuery superProcessInstanceId(string superProcessInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "superProcessInstanceId", superProcessInstanceId);
		this.superProcessInstanceId_Conflict = superProcessInstanceId;
		return this;
	  }

	  public virtual CaseInstanceQuery subProcessInstanceId(string subProcessInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "subProcessInstanceId", subProcessInstanceId);
		this.subProcessInstanceId_Conflict = subProcessInstanceId;
		return this;
	  }

	  public virtual CaseInstanceQuery superCaseInstanceId(string superCaseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "superCaseInstanceId", superCaseInstanceId);
		this.superCaseInstanceId_Conflict = superCaseInstanceId;
		return this;
	  }

	  public virtual CaseInstanceQuery subCaseInstanceId(string subCaseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "subCaseInstanceId", subCaseInstanceId);
		this.subCaseInstanceId_Conflict = subCaseInstanceId;
		return this;
	  }

	  public virtual CaseInstanceQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseInstanceQuery withoutTenantId()
	  {
		tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseInstanceQuery active()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual CaseInstanceQuery completed()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
		return this;
	  }

	  public virtual CaseInstanceQuery terminated()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;
		return this;
	  }

	  //ordering /////////////////////////////////////////////////////////////////

	  public virtual CaseInstanceQuery orderByCaseInstanceId()
	  {
		orderBy(CaseInstanceQueryProperty_Fields.CASE_INSTANCE_ID);
		return this;
	  }

	  public virtual CaseInstanceQuery orderByCaseDefinitionKey()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_CASE_DEFINITION, CaseInstanceQueryProperty_Fields.CASE_DEFINITION_KEY));
		return this;
	  }

	  public virtual CaseInstanceQuery orderByCaseDefinitionId()
	  {
		orderBy(CaseInstanceQueryProperty_Fields.CASE_DEFINITION_ID);
		return this;
	  }

	  public virtual CaseInstanceQuery orderByTenantId()
	  {
		orderBy(CaseInstanceQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  //results /////////////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.CaseExecutionManager.findCaseInstanceCountByQueryCriteria(this);
	  }

	  public override IList<CaseInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.CaseExecutionManager.findCaseInstanceByQueryCriteria(this, page);
	  }

	  //getters /////////////////////////////////////////////////////////////////

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
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

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Conflict;
		  }
	  }

	  public virtual CaseExecutionState State
	  {
		  get
		  {
			return state;
		  }
	  }

	  public virtual bool CaseInstancesOnly
	  {
		  get
		  {
			return true;
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

	  public virtual bool? Required
	  {
		  get
		  {
			return required;
		  }
	  }

	  public virtual bool? Repeatable
	  {
		  get
		  {
			return repeatable;
		  }
	  }

	  public virtual bool? Repetition
	  {
		  get
		  {
			return repetition;
		  }
	  }

	}

}