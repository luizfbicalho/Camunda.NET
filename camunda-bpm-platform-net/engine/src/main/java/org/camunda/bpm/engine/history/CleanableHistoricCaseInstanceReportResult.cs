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
	/// <summary>
	/// This interface defines the result of Historic finished case instance report.
	/// 
	/// </summary>
	public interface CleanableHistoricCaseInstanceReportResult
	{

	  /// <summary>
	  /// Returns the case definition id for the selected definition.
	  /// </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// Returns the case definition key for the selected definition.
	  /// </summary>
	  string CaseDefinitionKey {get;}

	  /// <summary>
	  /// Returns the case definition name for the selected definition.
	  /// </summary>
	  string CaseDefinitionName {get;}

	  /// <summary>
	  /// Returns the case definition version for the selected definition.
	  /// </summary>
	  int CaseDefinitionVersion {get;}

	  /// <summary>
	  /// Returns the history time to live for the selected definition.
	  /// </summary>
	  int? HistoryTimeToLive {get;}

	  /// <summary>
	  /// Returns the amount of finished historic case instances.
	  /// </summary>
	  long FinishedCaseInstanceCount {get;}

	  /// <summary>
	  /// Returns the amount of cleanable historic case instances.
	  /// </summary>
	  long CleanableCaseInstanceCount {get;}

	  /// 
	  /// <summary>
	  /// Returns the tenant id of the current case instances.
	  /// </summary>
	  string TenantId {get;}
	}

}