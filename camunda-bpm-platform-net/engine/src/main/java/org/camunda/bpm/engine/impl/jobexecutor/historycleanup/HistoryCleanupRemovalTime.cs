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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{

	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using HistoricProcessInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class HistoryCleanupRemovalTime : HistoryCleanupHandler
	{

	  protected internal IDictionary<Type, DbOperation> deleteOperations = new Dictionary<Type, DbOperation>();

	  public override void performCleanup()
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		deleteOperations.putAll(performProcessCleanup());

		if (DmnEnabled)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  deleteOperations.putAll(performDmnCleanup());
		}

		DbOperation batchCleanup = performBatchCleanup();

		deleteOperations[batchCleanup.EntityType] = batchCleanup;
	  }

	  protected internal virtual IDictionary<Type, DbOperation> performDmnCleanup()
	  {
		return Context.CommandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionsByRemovalTime(ClockUtil.CurrentTime, configuration.MinuteFrom, configuration.MinuteTo, BatchSize);
	  }

	  protected internal virtual IDictionary<Type, DbOperation> performProcessCleanup()
	  {
		return Context.CommandContext.HistoricProcessInstanceManager.deleteHistoricProcessInstancesByRemovalTime(ClockUtil.CurrentTime, configuration.MinuteFrom, configuration.MinuteTo, BatchSize);
	  }

	  protected internal virtual DbOperation performBatchCleanup()
	  {
		return Context.CommandContext.HistoricBatchManager.deleteHistoricBatchesByRemovalTime(ClockUtil.CurrentTime, configuration.MinuteFrom, configuration.MinuteTo, BatchSize);
	  }

	  protected internal override IDictionary<string, long> reportMetrics()
	  {
		IDictionary<string, long> reports = new Dictionary<string, long>();

		DbOperation deleteOperationProcessInstance = deleteOperations[typeof(HistoricProcessInstanceEntity)];
		if (deleteOperationProcessInstance != null)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES] = (long) deleteOperationProcessInstance.RowsAffected;
		}

		DbOperation deleteOperationDecisionInstance = deleteOperations[typeof(HistoricDecisionInstanceEntity)];
		if (deleteOperationDecisionInstance != null)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES] = (long) deleteOperationDecisionInstance.RowsAffected;
		}

		DbOperation deleteOperationBatch = deleteOperations[typeof(HistoricBatchEntity)];
		if (deleteOperationBatch != null)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS] = (long) deleteOperationBatch.RowsAffected;
		}

		return reports;
	  }

	  protected internal virtual bool DmnEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.DmnEnabled;
		  }
	  }

	  protected internal override bool shouldRescheduleNow()
	  {
		int batchSize = BatchSize;

		foreach (DbOperation deleteOperation in deleteOperations.Values)
		{
		  if (deleteOperation.RowsAffected == batchSize)
		  {
			return true;
		  }
		}

		return false;
	  }

	  public virtual int BatchSize
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryCleanupBatchSize;
		  }
	  }

	}

}