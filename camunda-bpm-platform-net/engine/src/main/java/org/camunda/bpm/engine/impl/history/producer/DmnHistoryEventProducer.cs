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
namespace org.camunda.bpm.engine.impl.history.producer
{
	using DmnDecisionEvaluationEvent = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionEvaluationEvent;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

	/// <summary>
	/// The producer for DMN history events. The history event producer is
	/// responsible for extracting data from the dmn engine and adding the data to a
	/// <seealso cref="HistoryEvent"/>.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public interface DmnHistoryEventProducer
	{

	  /// <summary>
	  /// Creates the history event fired when a decision is evaluated while execute
	  /// a process instance.
	  /// </summary>
	  /// <param name="execution">
	  ///          the current execution </param>
	  /// <param name="decisionEvaluationEvent">
	  ///          the evaluation event </param>
	  /// <returns> the history event
	  /// </returns>
	  /// <seealso cref= #createDecisionEvaluatedEvt(DmnDecisionEvaluationEvent) </seealso>
	  HistoryEvent createDecisionEvaluatedEvt(DelegateExecution execution, DmnDecisionEvaluationEvent decisionEvaluationEvent);

	  /// <summary>
	  /// Creates the history event fired when a decision is evaluated while execute
	  /// a case instance.
	  /// </summary>
	  /// <param name="execution">
	  ///          the current case execution </param>
	  /// <param name="decisionEvaluationEvent">
	  ///          the evaluation event </param>
	  /// <returns> the history event
	  /// </returns>
	  /// <seealso cref= #createDecisionEvaluatedEvt(DmnDecisionEvaluationEvent) </seealso>
	  HistoryEvent createDecisionEvaluatedEvt(DelegateCaseExecution execution, DmnDecisionEvaluationEvent decisionEvaluationEvent);

	  /// <summary>
	  /// Creates the history event fired when a decision is evaluated. If the
	  /// decision is evaluated while execute a process instance then you should use
	  /// <seealso cref="#createDecisionEvaluatedEvt(DelegateExecution, DmnDecisionEvaluationEvent)"/> instead.
	  /// </summary>
	  /// <param name="decisionEvaluationEvent">
	  ///          the evaluation event </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createDecisionEvaluatedEvt(DmnDecisionEvaluationEvent decisionEvaluationEvent);

	}

}