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
namespace org.camunda.bpm.engine.history
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Report = org.camunda.bpm.engine.query.Report;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public interface HistoricTaskInstanceReport : Report
	{

	  /// <summary>
	  /// <para>Sets the completed after date for constraining the query to search for all tasks
	  /// which are completed after a certain date.</para>
	  /// </summary>
	  /// <param name="completedAfter"> A <seealso cref="Date"/> to define the granularity of the report
	  /// </param>
	  /// <exception cref="NotValidException">
	  ///          When the given date is null. </exception>
	  HistoricTaskInstanceReport completedAfter(DateTime completedAfter);

	  /// <summary>
	  /// <para>Sets the completed before date for constraining the query to search for all tasks
	  /// which are completed before a certain date.</para>
	  /// </summary>
	  /// <param name="completedBefore"> A <seealso cref="Date"/> to define the granularity of the report
	  /// </param>
	  /// <exception cref="NotValidException">
	  ///          When the given date is null. </exception>
	  HistoricTaskInstanceReport completedBefore(DateTime completedBefore);

	  /// <summary>
	  /// <para>Executes the task report query and returns a list of <seealso cref="HistoricTaskInstanceReportResult"/>s</para>
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission
	  ///          on any <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <returns> a list of <seealso cref="HistoricTaskInstanceReportResult"/>s </returns>
	  IList<HistoricTaskInstanceReportResult> countByProcessDefinitionKey();

	  /// <summary>
	  /// <para>Executes the task report query and returns a list of <seealso cref="HistoricTaskInstanceReportResult"/>s</para>
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission
	  ///          on any <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <returns> a list of <seealso cref="HistoricTaskInstanceReportResult"/>s </returns>
	  IList<HistoricTaskInstanceReportResult> countByTaskName();
	}

}