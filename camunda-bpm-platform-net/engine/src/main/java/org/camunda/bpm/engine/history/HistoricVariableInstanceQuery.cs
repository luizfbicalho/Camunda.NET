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
	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Programmatic querying for <seealso cref="HistoricVariableInstance"/>s.
	/// 
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	public interface HistoricVariableInstanceQuery : Query<HistoricVariableInstanceQuery, HistoricVariableInstance>
	{

	  /// <summary>
	  /// Only select the variable with the given Id </summary>
	  /// <param name="the"> id of the variable to select </param>
	  /// <returns> the query object  </returns>
	  HistoricVariableInstanceQuery variableId(string id);

	  /// <summary>
	  /// Only select historic process variables with the given process instance. </summary>
	  HistoricVariableInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic process variables for the given process definition </summary>
	  HistoricVariableInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic process variables for the given process definition key </summary>
	  HistoricVariableInstanceQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select historic case variables with the given case instance. </summary>
	  HistoricVariableInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic process variables with the given variable name. </summary>
	  HistoricVariableInstanceQuery variableName(string variableName);

	  /// <summary>
	  /// Only select historic process variables where the given variable name is like. </summary>
	  HistoricVariableInstanceQuery variableNameLike(string variableNameLike);

	  /// <summary>
	  /// Only select historic process variables which match one of the given variable types. </summary>
	  HistoricVariableInstanceQuery variableTypeIn(params string[] variableTypes);

	  /// <summary>
	  /// only select historic process variables with the given name and value
	  /// </summary>
	  HistoricVariableInstanceQuery variableValueEquals(string variableName, object variableValue);

	  HistoricVariableInstanceQuery orderByProcessInstanceId();

	  HistoricVariableInstanceQuery orderByVariableName();

	  /// <summary>
	  /// Only select historic process variables with the given process instance ids. </summary>
	  HistoricVariableInstanceQuery processInstanceIdIn(params string[] processInstanceIds);

	  /// <summary>
	  /// Only select historic variable instances which have one of the task ids. * </summary>
	  HistoricVariableInstanceQuery taskIdIn(params string[] taskIds);

	  /// <summary>
	  /// Only select historic variable instances which have one of the executions ids. * </summary>
	  HistoricVariableInstanceQuery executionIdIn(params string[] executionIds);

	  /// <summary>
	  /// Only select historic variable instances which have one of the case executions ids. * </summary>
	  HistoricVariableInstanceQuery caseExecutionIdIn(params string[] caseExecutionIds);

	  /// <summary>
	  /// Only select historic variable instances with one of the given case activity ids. * </summary>
	  HistoricVariableInstanceQuery caseActivityIdIn(params string[] caseActivityIds);

	  /// <summary>
	  /// Only select historic variable instances which have one of the activity instance ids. * </summary>
	  HistoricVariableInstanceQuery activityInstanceIdIn(params string[] activityInstanceIds);

	  /// <summary>
	  /// Only select historic variable instances with one of the given tenant ids. </summary>
	  HistoricVariableInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic variable instances without tenant id is database-specific.
	  /// </summary>
	  HistoricVariableInstanceQuery orderByTenantId();

	  /// <summary>
	  /// Disable fetching of byte array and file values. By default, the query will fetch such values.
	  /// By calling this method you can prevent the values of (potentially large) blob data chunks
	  /// to be fetched. The variables themselves are nonetheless included in the query result.
	  /// </summary>
	  /// <returns> the query builder </returns>
	  HistoricVariableInstanceQuery disableBinaryFetching();

	  /// <summary>
	  /// Disable deserialization of variable values that are custom objects. By default, the query
	  /// will attempt to deserialize the value of these variables. By calling this method you can
	  /// prevent such attempts in environments where their classes are not available.
	  /// Independent of this setting, variable serialized values are accessible.
	  /// </summary>
	  HistoricVariableInstanceQuery disableCustomObjectDeserialization();

	  /// <summary>
	  /// Include variables that has been already deleted during the execution
	  /// </summary>
	  HistoricVariableInstanceQuery includeDeleted();

	}

}