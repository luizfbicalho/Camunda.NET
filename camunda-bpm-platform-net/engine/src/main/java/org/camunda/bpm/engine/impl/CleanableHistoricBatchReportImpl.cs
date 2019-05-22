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

	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

	[Serializable]
	public class CleanableHistoricBatchReportImpl : AbstractQuery<CleanableHistoricBatchReport, CleanableHistoricBatchReportResult>, CleanableHistoricBatchReport
	{

	  private const long serialVersionUID = 1L;

	  protected internal DateTime currentTimestamp;

	  protected internal bool isHistoryCleanupStrategyRemovalTimeBased;

	  public CleanableHistoricBatchReportImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CleanableHistoricBatchReport orderByFinishedBatchOperation()
	  {
		orderBy(CleanableHistoricInstanceReportProperty_Fields.FINISHED_AMOUNT);
		return this;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		checkPermissions(commandContext);

		IDictionary<string, int> batchOperationsForHistoryCleanup = commandContext.ProcessEngineConfiguration.ParsedBatchOperationsForHistoryCleanup;

		if (HistoryCleanupStrategyRemovalTimeBased)
		{
		  addBatchOperationsWithoutTTL(batchOperationsForHistoryCleanup);
		}

		return commandContext.HistoricBatchManager.findCleanableHistoricBatchesReportCountByCriteria(this, batchOperationsForHistoryCleanup);
	  }

	  public override IList<CleanableHistoricBatchReportResult> executeList(CommandContext commandContext, Page page)
	  {
		provideHistoryCleanupStrategy(commandContext);

		checkQueryOk();
		checkPermissions(commandContext);

		IDictionary<string, int> batchOperationsForHistoryCleanup = commandContext.ProcessEngineConfiguration.ParsedBatchOperationsForHistoryCleanup;

		if (HistoryCleanupStrategyRemovalTimeBased)
		{
		  addBatchOperationsWithoutTTL(batchOperationsForHistoryCleanup);
		}

		return commandContext.HistoricBatchManager.findCleanableHistoricBatchesReportByCriteria(this, page, batchOperationsForHistoryCleanup);
	  }

	  protected internal virtual void addBatchOperationsWithoutTTL(IDictionary<string, int> batchOperations)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Map<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> batchJobHandlers = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getBatchHandlers();
		IDictionary<string, BatchJobHandler<object>> batchJobHandlers = Context.ProcessEngineConfiguration.BatchHandlers;

		ISet<string> batchOperationKeys = null;
		if (batchJobHandlers != null)
		{
		  batchOperationKeys = batchJobHandlers.Keys;
		}

		if (batchOperationKeys != null)
		{
		  foreach (string batchOperation in batchOperationKeys)
		  {
			int? ttl = batchOperations[batchOperation];
			batchOperations[batchOperation] = ttl.Value;

		  }
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


	  private void checkPermissions(CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadHistoricBatch();
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