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
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

	/// <summary>
	/// <para>The producer for CMMN history events. The history event producer is
	/// responsible for extracting data from the runtime structures
	/// (Executions, Tasks, ...) and adding the data to a <seealso cref="HistoryEvent"/>.
	/// 
	/// @author Sebastian Menski
	/// </para>
	/// </summary>
	public interface CmmnHistoryEventProducer
	{

	  /// <summary>
	  /// Creates the history event fired when a case instance is <strong>created</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseInstanceCreateEvt(DelegateCaseExecution caseExecution);

	  /// <summary>
	  /// Creates the history event fired when a case instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseInstanceUpdateEvt(DelegateCaseExecution caseExecution);

	  /// <summary>
	  /// Creates the history event fired when a case instance is <strong>closed</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseInstanceCloseEvt(DelegateCaseExecution caseExecution);

	  /// <summary>
	  /// Creates the history event fired when a case activity instance is <strong>created</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseActivityInstanceCreateEvt(DelegateCaseExecution caseExecution);

	  /// <summary>
	  /// Creates the history event fired when a case activity instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseActivityInstanceUpdateEvt(DelegateCaseExecution caseExecution);

	  /// <summary>
	  /// Creates the history event fired when a case activity instance is <strong>ended</strong>.
	  /// </summary>
	  /// <param name="caseExecution"> the current case execution </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createCaseActivityInstanceEndEvt(DelegateCaseExecution caseExecution);

	}

}