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
	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricDecisionInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReportResult;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	[Serializable]
	public class CleanableHistoricDecisionInstanceReportImpl : AbstractQuery<CleanableHistoricDecisionInstanceReport, CleanableHistoricDecisionInstanceReportResult>, CleanableHistoricDecisionInstanceReport
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] decisionDefinitionIdIn_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] decisionDefinitionKeyIn_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] tenantIdIn_Renamed;
	  protected internal bool isTenantIdSet = false;
	  protected internal bool isCompact = false;

	  protected internal DateTime currentTimestamp;

	  protected internal bool isHistoryCleanupStrategyRemovalTimeBased;

	  public CleanableHistoricDecisionInstanceReportImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CleanableHistoricDecisionInstanceReport decisionDefinitionIdIn(params string[] decisionDefinitionIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "decisionDefinitionIdIn", (object[]) decisionDefinitionIds);
		this.decisionDefinitionIdIn_Renamed = decisionDefinitionIds;
		return this;
	  }

	  public virtual CleanableHistoricDecisionInstanceReport decisionDefinitionKeyIn(params string[] decisionDefinitionKeys)
	  {
		ensureNotNull(typeof(NotValidException), "", "decisionDefinitionKeyIn", (object[]) decisionDefinitionKeys);
		this.decisionDefinitionKeyIn_Renamed = decisionDefinitionKeys;
		return this;
	  }

	  public virtual CleanableHistoricDecisionInstanceReport tenantIdIn(params string[] tenantIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "tenantIdIn", (object[]) tenantIds);
		this.tenantIdIn_Renamed = tenantIds;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricDecisionInstanceReport withoutTenantId()
	  {
		this.tenantIdIn_Renamed = null;
		isTenantIdSet = true;
		return this;
	  }

	  public virtual CleanableHistoricDecisionInstanceReport compact()
	  {
		this.isCompact = true;
		return this;
	  }

	  public virtual CleanableHistoricDecisionInstanceReport orderByFinished()
	  {
		orderBy(CleanableHistoricInstanceReportProperty_Fields.FINISHED_AMOUNT);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		return commandContext.HistoricDecisionInstanceManager.findCleanableHistoricDecisionInstancesReportCountByCriteria(this);
	  }

	  public override IList<CleanableHistoricDecisionInstanceReportResult> executeList(CommandContext commandContext, Page page)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		return commandContext.HistoricDecisionInstanceManager.findCleanableHistoricDecisionInstancesReportByCriteria(this, page);
	  }

	  public virtual string[] DecisionDefinitionIdIn
	  {
		  get
		  {
			return decisionDefinitionIdIn_Renamed;
		  }
		  set
		  {
			this.decisionDefinitionIdIn_Renamed = value;
		  }
	  }


	  public virtual string[] DecisionDefinitionKeyIn
	  {
		  get
		  {
			return decisionDefinitionKeyIn_Renamed;
		  }
		  set
		  {
			this.decisionDefinitionKeyIn_Renamed = value;
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