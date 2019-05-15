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
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionQueryImpl : AbstractVariableQueryImpl<CaseExecutionQuery, CaseExecution>, CaseExecutionQuery
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
	  protected internal string businessKey;
	  protected internal CaseExecutionState state;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool? required_Renamed = false;

	  protected internal bool isTenantIdSet = false;
	  protected internal string[] tenantIds;

	  // Not used by end-users, but needed for dynamic ibatis query
	  protected internal string superProcessInstanceId;
	  protected internal string subProcessInstanceId;
	  protected internal string superCaseInstanceId;
	  protected internal string subCaseInstanceId;
	  protected internal string deploymentId;

	  public CaseExecutionQueryImpl()
	  {
	  }

	  public CaseExecutionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CaseExecutionQuery caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceId", caseInstanceId);
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual CaseExecutionQuery caseDefinitionId(string caseDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionId", caseDefinitionId);
		this.caseDefinitionId_Renamed = caseDefinitionId;
		return this;
	  }

	  public virtual CaseExecutionQuery caseDefinitionKey(string caseDefinitionKey)
	  {
		ensureNotNull(typeof(NotValidException), "caseDefinitionKey", caseDefinitionKey);
		this.caseDefinitionKey_Renamed = caseDefinitionKey;
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceBusinessKey(string caseInstanceBusinessKey)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceBusinessKey", caseInstanceBusinessKey);
		this.businessKey = caseInstanceBusinessKey;
		return this;
	  }

	  public virtual CaseExecutionQuery caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseExecutionId", caseExecutionId);
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual CaseExecutionQuery activityId(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual CaseExecutionQuery tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull("tenantIds", (object[]) tenantIds);
		this.tenantIds = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseExecutionQuery withoutTenantId()
	  {
		this.tenantIds = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CaseExecutionQuery required()
	  {
		this.required_Renamed = true;
		return this;
	  }

	  public virtual CaseExecutionQuery available()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
		return this;
	  }

	  public virtual CaseExecutionQuery enabled()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
		return this;
	  }

	  public virtual CaseExecutionQuery active()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual CaseExecutionQuery disabled()
	  {
		state = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueEquals(string name, object value)
	  {
		addVariable(name, value, QueryOperator.EQUALS, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueNotEquals(string name, object value)
	  {
		addVariable(name, value, QueryOperator.NOT_EQUALS, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueGreaterThan(string name, object value)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueGreaterThanOrEqual(string name, object value)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN_OR_EQUAL, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueLessThan(string name, object value)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueLessThanOrEqual(string name, object value)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN_OR_EQUAL, false);
		return this;
	  }

	  public virtual CaseExecutionQuery caseInstanceVariableValueLike(string name, string value)
	  {
		addVariable(name, value, QueryOperator.LIKE, false);
		return this;
	  }

	  // order by ///////////////////////////////////////////

	  public virtual CaseExecutionQuery orderByCaseExecutionId()
	  {
		orderBy(CaseExecutionQueryProperty_Fields.CASE_EXECUTION_ID);
		return this;
	  }

	  public virtual CaseExecutionQuery orderByCaseDefinitionKey()
	  {
		orderBy(new QueryOrderingProperty(QueryOrderingProperty.RELATION_CASE_DEFINITION, CaseExecutionQueryProperty_Fields.CASE_DEFINITION_KEY));
		return this;
	  }

	  public virtual CaseExecutionQuery orderByCaseDefinitionId()
	  {
		orderBy(CaseExecutionQueryProperty_Fields.CASE_DEFINITION_ID);
		return this;
	  }

	  public virtual CaseExecutionQuery orderByTenantId()
	  {
		orderBy(CaseExecutionQueryProperty_Fields.TENANT_ID);
		return this;
	  }

	  // results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.CaseExecutionManager.findCaseExecutionCountByQueryCriteria(this);
	  }

	  public override IList<CaseExecution> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		IList<CaseExecution> result = commandContext.CaseExecutionManager.findCaseExecutionsByQueryCriteria(this, page);

		foreach (CaseExecution caseExecution in result)
		{
		  CaseExecutionEntity caseExecutionEntity = (CaseExecutionEntity) caseExecution;
		  // initializes the name, type and description
		  // of the activity on current case execution
		  caseExecutionEntity.Activity;
		}

		return result;
	  }

	  // getters /////////////////////////////////////////////

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Renamed;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Renamed;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
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
			return false;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
	  }

	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId;
		  }
	  }

	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId;
		  }
	  }

	  public virtual string SubCaseInstanceId
	  {
		  get
		  {
			return subCaseInstanceId;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
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