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

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

	/// <summary>
	/// Batch of work for history cleanup.
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class HistoryCleanupBatch : HistoryCleanupHandler
	{

	  private IList<string> historicProcessInstanceIds = Collections.emptyList();
	  private IList<string> historicDecisionInstanceIds = Collections.emptyList();
	  private IList<string> historicCaseInstanceIds = Collections.emptyList();
	  private IList<string> historicBatchIds = Collections.emptyList();

	  public virtual IList<string> HistoricProcessInstanceIds
	  {
		  get
		  {
			return historicProcessInstanceIds;
		  }
		  set
		  {
			this.historicProcessInstanceIds = value;
		  }
	  }


	  public virtual IList<string> HistoricDecisionInstanceIds
	  {
		  get
		  {
			return historicDecisionInstanceIds;
		  }
		  set
		  {
			this.historicDecisionInstanceIds = value;
		  }
	  }


	  public virtual IList<string> HistoricCaseInstanceIds
	  {
		  get
		  {
			return historicCaseInstanceIds;
		  }
		  set
		  {
			this.historicCaseInstanceIds = value;
		  }
	  }


	  public virtual IList<string> HistoricBatchIds
	  {
		  get
		  {
			return historicBatchIds;
		  }
		  set
		  {
			this.historicBatchIds = value;
		  }
	  }


	  /// <summary>
	  /// Size of the batch.
	  /// </summary>
	  public virtual int size()
	  {
		return historicProcessInstanceIds.Count + historicDecisionInstanceIds.Count + historicCaseInstanceIds.Count + historicBatchIds.Count;
	  }

	  public override void performCleanup()
	  {
		CommandContext commandContext = Context.CommandContext;
		HistoryCleanupHelper.prepareNextBatch(this, commandContext);

		if (size() > 0)
		{
		  if (historicProcessInstanceIds.Count > 0)
		  {
			commandContext.HistoricProcessInstanceManager.deleteHistoricProcessInstanceByIds(historicProcessInstanceIds);
		  }
		  if (historicDecisionInstanceIds.Count > 0)
		  {
			commandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionInstanceByIds(historicDecisionInstanceIds);
		  }
		  if (historicCaseInstanceIds.Count > 0)
		  {
			commandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstancesByIds(historicCaseInstanceIds);
		  }
		  if (historicBatchIds.Count > 0)
		  {
			commandContext.HistoricBatchManager.deleteHistoricBatchesByIds(historicBatchIds);
		  }
		}
	  }

	  protected internal override IDictionary<string, long> reportMetrics()
	  {
		IDictionary<string, long> reports = new Dictionary<string, long>();

		if (historicProcessInstanceIds.Count > 0)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES] = (long) historicProcessInstanceIds.Count;
		}
		if (historicDecisionInstanceIds.Count > 0)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES] = (long) historicDecisionInstanceIds.Count;
		}
		if (historicCaseInstanceIds.Count > 0)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_CASE_INSTANCES] = (long) historicCaseInstanceIds.Count;
		}
		if (historicBatchIds.Count > 0)
		{
		  reports[Metrics.HISTORY_CLEANUP_REMOVED_BATCH_OPERATIONS] = (long) historicBatchIds.Count;
		}

		return reports;
	  }

	  internal override bool shouldRescheduleNow()
	  {
		return size() >= BatchSizeThreshold.Value;
	  }

	  public virtual int? BatchSizeThreshold
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryCleanupBatchThreshold;
		  }
	  }

	}

}