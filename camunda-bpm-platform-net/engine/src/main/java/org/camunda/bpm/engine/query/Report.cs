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
namespace org.camunda.bpm.engine.query
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;

	/// <summary>
	/// Describes basic methods for creating a report.
	/// 
	/// @author Roman Smirnov
	/// 
	/// @since 7.5
	/// </summary>
	public interface Report
	{

	  /// <summary>
	  /// <para>Executes the duration report query and returns a list of
	  /// <seealso cref="DurationReportResult"/>s.</para>
	  /// 
	  /// <para>Be aware that the resulting report must be interpreted by the
	  /// caller itself.</para>
	  /// </summary>
	  /// <param name="periodUnit"> A <seealso cref="PeriodUnit period unit"/> to define
	  ///          the granularity of the report.
	  /// </param>
	  /// <returns> a list of <seealso cref="DurationReportResult"/>s
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission
	  ///          on any <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  /// <exception cref="NotValidException">
	  ///          When the given period unit is null. </exception>
	  IList<DurationReportResult> duration(PeriodUnit periodUnit);

	}



}