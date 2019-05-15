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
	/// @author Stefan Hentschel.
	/// </summary>
	public interface HistoricTaskInstanceReportResult
	{

	  /// <summary>
	  /// <para>Returns the count of the grouped items.</para>
	  /// </summary>
	  long? Count {get;}

	  /// <summary>
	  /// <para>Returns the process definition key for the selected definition key.</para>
	  /// </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// <para>Returns the process definition id for the selected definition key</para>
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// <para></para>Returns the process definition name for the selected definition key</p>
	  /// </summary>
	  string ProcessDefinitionName {get;}

	  /// <summary>
	  /// <para>Returns the name of the task</para>
	  /// </summary>
	  /// <returns> A task name when the query is triggered with a 'countByTaskName'. Else the return
	  /// value is null. </returns>
	  string TaskName {get;}

	  /// <summary>
	  /// <para>Returns the id of the tenant task</para>
	  /// </summary>
	  string TenantId {get;}
	}

}