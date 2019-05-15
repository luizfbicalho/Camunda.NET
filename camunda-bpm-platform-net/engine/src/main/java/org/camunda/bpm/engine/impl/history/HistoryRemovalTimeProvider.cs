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
	using HistoricDecisionInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceEntity;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// The provider is either invoked on root process instance start or end
	/// based on the selected history removal time strategy.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface HistoryRemovalTimeProvider
	{

	  /// <summary>
	  /// Calculates the removal time of historic entities related to processes.
	  /// 
	  /// START: the removal time is set for each historic entity separately on occurrence (creation).
	  ///        <seealso cref="HistoricProcessInstanceEventEntity#getEndTime()"/> is {@code null}
	  /// 
	  /// END:   the removal time is updated simultaneously for all historic entities which belong to
	  ///        the root process instance when it ends.
	  ///        <seealso cref="HistoricProcessInstanceEventEntity#getEndTime()"/> is not {@code null}
	  /// </summary>
	  /// <param name="historicRootProcessInstance"> which is either in state running or ended </param>
	  /// <param name="processDefinition"> of the historic root process instance </param>
	  /// <returns> the removal time for historic process instances </returns>
	  DateTime calculateRemovalTime(HistoricProcessInstanceEventEntity historicRootProcessInstance, ProcessDefinition processDefinition);

	  /// <summary>
	  /// Calculates the removal time of historic entities related to decisions.
	  /// </summary>
	  /// <param name="historicRootDecisionInstance"> </param>
	  /// <param name="decisionDefinition"> of the historic root decision instance </param>
	  /// <returns> the removal time for historic decision instances </returns>
	  DateTime calculateRemovalTime(HistoricDecisionInstanceEntity historicRootDecisionInstance, DecisionDefinition decisionDefinition);

	  /// <summary>
	  /// Calculates the removal time of historic batches.
	  /// 
	  /// START: the removal time is set for the historic batch entity on start.
	  ///        <seealso cref="HistoricBatchEntity#getEndTime()"/> is {@code null}
	  /// 
	  /// END:   the removal time is set for the historic batch entity on end.
	  ///        <seealso cref="HistoricBatchEntity#getEndTime()"/> is not {@code null}
	  /// </summary>
	  /// <param name="historicBatch"> which is either in state running or ended </param>
	  /// <returns> the removal time of historic entities </returns>
	  DateTime calculateRemovalTime(HistoricBatchEntity historicBatch);

	}

}