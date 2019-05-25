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


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	[Serializable]
	public class CleanableHistoricCaseInstanceReportImpl : AbstractQuery<CleanableHistoricCaseInstanceReport, CleanableHistoricCaseInstanceReportResult>, CleanableHistoricCaseInstanceReport
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] caseDefinitionIdIn_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] caseDefinitionKeyIn_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] tenantIdIn_Conflict;
	  protected internal bool isTenantIdSet = false;
	  protected internal bool isCompact = false;

	  protected internal DateTime currentTimestamp;

	  public CleanableHistoricCaseInstanceReportImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CleanableHistoricCaseInstanceReport caseDefinitionIdIn(params string[] caseDefinitionIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "caseDefinitionIdIn", (object[]) caseDefinitionIds);
		this.caseDefinitionIdIn_Conflict = caseDefinitionIds;
		return this;
	  }

	  public virtual CleanableHistoricCaseInstanceReport caseDefinitionKeyIn(params string[] caseDefinitionKeys)
	  {
		ensureNotNull(typeof(NotValidException), "", "caseDefinitionKeyIn", (object[]) caseDefinitionKeys);
		this.caseDefinitionKeyIn_Conflict = caseDefinitionKeys;
		return this;
	  }

	  public virtual CleanableHistoricCaseInstanceReport tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "tenantIdIn", (object[]) tenantIds);
		this.tenantIdIn_Conflict = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricCaseInstanceReport withoutTenantId()
	  {
		this.tenantIdIn_Conflict = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricCaseInstanceReport compact()
	  {
		this.isCompact = true;
		return this;
	  }

	  public virtual CleanableHistoricCaseInstanceReport orderByFinished()
	  {
		orderBy(CleanableHistoricInstanceReportProperty_Fields.FINISHED_AMOUNT);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricCaseInstanceManager.findCleanableHistoricCaseInstancesReportCountByCriteria(this);
	  }

	  public override IList<CleanableHistoricCaseInstanceReportResult> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricCaseInstanceManager.findCleanableHistoricCaseInstancesReportByCriteria(this, page);
	  }

	  public virtual string[] CaseDefinitionIdIn
	  {
		  get
		  {
			return caseDefinitionIdIn_Conflict;
		  }
		  set
		  {
			this.caseDefinitionIdIn_Conflict = value;
		  }
	  }


	  public virtual string[] CaseDefinitionKeyIn
	  {
		  get
		  {
			return caseDefinitionKeyIn_Conflict;
		  }
		  set
		  {
			this.caseDefinitionKeyIn_Conflict = value;
		  }
	  }


	  public virtual DateTime CurrentTimestamp
	  {
		  get
		  {
			return currentTimestamp;
		  }
		  set
		  {
			this.currentTimestamp = value;
		  }
	  }


	  public virtual string[] TenantIdIn
	  {
		  get
		  {
			return tenantIdIn_Conflict;
		  }
		  set
		  {
			this.tenantIdIn_Conflict = value;
		  }
	  }


	  public virtual bool TenantIdSet
	  {
		  get
		  {
			return isTenantIdSet;
		  }
	  }

	  public virtual bool Compact
	  {
		  get
		  {
			return isCompact;
		  }
	  }

	}

}