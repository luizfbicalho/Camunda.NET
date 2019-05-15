using System;

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
namespace org.camunda.bpm.engine.impl.history
{
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class DefaultHistoryRemovalTimeProvider : HistoryRemovalTimeProvider
	{

	  public virtual DateTime calculateRemovalTime(HistoricProcessInstanceEventEntity historicRootProcessInstance, ProcessDefinition processDefinition)
	  {

		int? historyTimeToLive = processDefinition.HistoryTimeToLive;

		if (historyTimeToLive != null)
		{
		  if (isProcessInstanceRunning(historicRootProcessInstance))
		  {
			DateTime startTime = historicRootProcessInstance.StartTime;
			return determineRemovalTime(startTime, historyTimeToLive);

		  }
		  else if (isProcessInstanceEnded(historicRootProcessInstance))
		  {
			DateTime endTime = historicRootProcessInstance.EndTime;
			return determineRemovalTime(endTime, historyTimeToLive);

		  }
		}

		return null;
	  }

	  public virtual DateTime calculateRemovalTime(HistoricDecisionInstanceEntity historicRootDecisionInstance, DecisionDefinition decisionDefinition)
	  {

		int? historyTimeToLive = decisionDefinition.HistoryTimeToLive;

		if (historyTimeToLive != null)
		{
		  DateTime evaluationTime = historicRootDecisionInstance.EvaluationTime;
		  return determineRemovalTime(evaluationTime, historyTimeToLive);
		}

		return null;
	  }

	  public virtual DateTime calculateRemovalTime(HistoricBatchEntity historicBatch)
	  {
		string batchOperation = historicBatch.Type;
		if (!string.ReferenceEquals(batchOperation, null))
		{
		  int? historyTimeToLive = getTTLByBatchOperation(batchOperation);
		  if (historyTimeToLive != null)
		  {
			if (isBatchRunning(historicBatch))
			{
			  DateTime startTime = historicBatch.StartTime;
			  return determineRemovalTime(startTime, historyTimeToLive);

			}
			else if (isBatchEnded(historicBatch))
			{
			  DateTime endTime = historicBatch.EndTime;
			  return determineRemovalTime(endTime, historyTimeToLive);

			}
		  }
		}

		return null;
	  }

	  protected internal virtual bool isBatchRunning(HistoricBatchEntity historicBatch)
	  {
		return historicBatch.EndTime == null;
	  }

	  protected internal virtual bool isBatchEnded(HistoricBatchEntity historicBatch)
	  {
		return historicBatch.EndTime != null;
	  }

	  protected internal virtual int? getTTLByBatchOperation(string batchOperation)
	  {
		return Context.CommandContext.ProcessEngineConfiguration.ParsedBatchOperationsForHistoryCleanup[batchOperation];
	  }

	  protected internal virtual bool isProcessInstanceRunning(HistoricProcessInstanceEventEntity historicProcessInstance)
	  {
		return historicProcessInstance.EndTime == null;
	  }

	  protected internal virtual bool isProcessInstanceEnded(HistoricProcessInstanceEventEntity historicProcessInstance)
	  {
		return historicProcessInstance.EndTime != null;
	  }

	  protected internal virtual DateTime determineRemovalTime(DateTime initTime, int? timeToLive)
	  {
		DateTime removalTime = new DateTime();
		removalTime = new DateTime(initTime);
		removalTime.AddDays(timeToLive);

		return removalTime;
	  }

	}

}