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
namespace org.camunda.bpm.engine.history
{

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Report = org.camunda.bpm.engine.query.Report;

	/// <summary>
	/// <para>Defines a report query for <seealso cref="HistoricProcessInstance"/>s.</para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricProcessInstanceReport : Report
	{

	  /// <summary>
	  /// Only takes historic process instances into account that were started before the given date.
	  /// </summary>
	  /// <exception cref="NotValidException"> if the given started before date is null
	  ///  </exception>
	  HistoricProcessInstanceReport startedBefore(DateTime startedBefore);

	  /// <summary>
	  /// Only takes historic process instances into account that were started after the given date.
	  /// </summary>
	  /// <exception cref="NotValidException"> if the given started after date is null </exception>
	  HistoricProcessInstanceReport startedAfter(DateTime startedAfter);

	  /// <summary>
	  /// Only takes historic process instances into account for the given process definition ids.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  HistoricProcessInstanceReport processDefinitionIdIn(params string[] processDefinitionIds);

	  /// <summary>
	  /// Only takes historic process instances into account for the given process definition keys.
	  /// </summary>
	  /// <exception cref="NotValidException"> if one of the given ids is null </exception>
	  HistoricProcessInstanceReport processDefinitionKeyIn(params string[] processDefinitionKeys);

	}

}