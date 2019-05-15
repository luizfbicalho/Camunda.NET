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
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	[Serializable]
	public class CleanableHistoricProcessInstanceReportImpl : AbstractQuery<CleanableHistoricProcessInstanceReport, CleanableHistoricProcessInstanceReportResult>, CleanableHistoricProcessInstanceReport
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] processDefinitionIdIn_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] processDefinitionKeyIn_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] tenantIdIn_Renamed;
	  protected internal bool isTenantIdSet = false;
	  protected internal bool isCompact = false;

	  protected internal DateTime currentTimestamp;

	  protected internal bool isHistoryCleanupStrategyRemovalTimeBased;

	  public CleanableHistoricProcessInstanceReportImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CleanableHistoricProcessInstanceReport processDefinitionIdIn(params string[] processDefinitionIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "processDefinitionIdIn", (object[]) processDefinitionIds);
		this.processDefinitionIdIn_Renamed = processDefinitionIds;
		return this;
	  }

	  public virtual CleanableHistoricProcessInstanceReport processDefinitionKeyIn(params string[] processDefinitionKeys)
	  {
		ensureNotNull(typeof(NotValidException), "", "processDefinitionKeyIn", (object[]) processDefinitionKeys);
		this.processDefinitionKeyIn_Renamed = processDefinitionKeys;
		return this;
	  }

	  public virtual CleanableHistoricProcessInstanceReport tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "tenantIdIn", (object[]) tenantIds);
		this.tenantIdIn_Renamed = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricProcessInstanceReport withoutTenantId()
	  {
		this.tenantIdIn_Renamed = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricProcessInstanceReport compact()
	  {
		this.isCompact = true;
		return this;
	  }

	  public virtual CleanableHistoricProcessInstanceReport orderByFinished()
	  {
		orderBy(CleanableHistoricInstanceReportProperty_Fields.FINISHED_AMOUNT);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		return commandContext.HistoricProcessInstanceManager.findCleanableHistoricProcessInstancesReportCountByCriteria(this);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult> executeList(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext, final Page page)
	  public override IList<CleanableHistoricProcessInstanceReportResult> executeList(CommandContext commandContext, Page page)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		return commandContext.HistoricProcessInstanceManager.findCleanableHistoricProcessInstancesReportByCriteria(this, page);
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


	  public virtual string[] ProcessDefinitionIdIn
	  {
		  get
		  {
			return processDefinitionIdIn_Renamed;
		  }
	  }

	  public virtual string[] ProcessDefinitionKeyIn
	  {
		  get
		  {
			return processDefinitionKeyIn_Renamed;
		  }
	  }

	  public virtual string[] TenantIdIn
	  {
		  get
		  {
			return tenantIdIn_Renamed;
		  }
		  set
		  {
			this.tenantIdIn_Renamed = value;
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

	  protected internal virtual void provideHistoryCleanupStrategy(CommandContext commandContext)
	  {
		string historyCleanupStrategy = commandContext.ProcessEngineConfiguration.HistoryCleanupStrategy;

		isHistoryCleanupStrategyRemovalTimeBased = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED.Equals(historyCleanupStrategy);
	  }

	  public virtual bool HistoryCleanupStrategyRemovalTimeBased
	  {
		  get
		  {
			return isHistoryCleanupStrategyRemovalTimeBased;
		  }
	  }

	}

}