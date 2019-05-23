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
	using Query = org.camunda.bpm.engine.query.Query;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;

	/// <summary>
	/// Programmatic querying for <seealso cref="HistoricDetail"/>s.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricDetailQuery : Query<HistoricDetailQuery, HistoricDetail>
	{

	  /// <summary>
	  /// Only select the historic detail with the given id.
	  /// </summary>
	  /// <param name="id"> the historic detail to select </param>
	  /// <returns> the query builder </returns>
	  HistoricDetailQuery detailId(string id);

	  /// <summary>
	  /// Only select historic variable updates with the given process instance.
	  /// <seealso cref="ProcessInstance"/> ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricDetailQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic variable updates with the given case instance.
	  /// <seealso cref="CaseInstance"/> ids and <seealso cref="HistoricCaseInstance"/> ids match. 
	  /// </summary>
	  HistoricDetailQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic variable updates with the given execution.
	  /// Note that <seealso cref="Execution"/> ids are not stored in the history as first class citizen,
	  /// only process instances are.
	  /// </summary>
	  HistoricDetailQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic variable updates with the given case execution.
	  /// Note that <seealso cref="CaseExecution"/> ids are not stored in the history as first class citizen,
	  /// only case instances are.
	  /// </summary>
	  HistoricDetailQuery caseExecutionId(string caseExecutionId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricActivityInstance activity instance"/>. </summary>
	  /// @deprecated since 5.2, use <seealso cref="activityInstanceId(string)"/> instead  
	  HistoricDetailQuery activityId(string activityId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricActivityInstance activity instance"/>. </summary>
	  HistoricDetailQuery activityInstanceId(string activityInstanceId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricTaskInstance historic task instance"/>. </summary>
	  HistoricDetailQuery taskId(string taskId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricVariableInstance historic variable instance"/>. </summary>
	  HistoricDetailQuery variableInstanceId(string variableInstanceId);

	  /// <summary>
	  /// Only select historic process variables which match one of the given variable types. </summary>
	  HistoricDetailQuery variableTypeIn(params string[] variableTypes);

	  /// <summary>
	  /// Only select <seealso cref="HistoricFormProperty"/>s. </summary>
	  [Obsolete]
	  HistoricDetailQuery formProperties();

	  /// <summary>
	  /// Only select <seealso cref="HistoricFormField"/>s. </summary>
	  HistoricDetailQuery formFields();

	  /// <summary>
	  /// Only select <seealso cref="HistoricVariableUpdate"/>s. </summary>
	  HistoricDetailQuery variableUpdates();

	  /// <summary>
	  /// Disable fetching of byte array and file values. By default, the query will fetch such values.
	  /// By calling this method you can prevent the values of (potentially large) blob data chunks to be fetched.
	  ///  The variables themselves are nonetheless included in the query result.
	  /// </summary>
	  /// <returns> the query builder </returns>
	  HistoricDetailQuery disableBinaryFetching();

	  /// <summary>
	  /// Disable deserialization of variable values that are custom objects. By default, the query
	  /// will attempt to deserialize the value of these variables. By calling this method you can
	  /// prevent such attempts in environments where their classes are not available.
	  /// Independent of this setting, variable serialized values are accessible.
	  /// </summary>
	  HistoricDetailQuery disableCustomObjectDeserialization();

	  /// <summary>
	  /// Exclude all task-related <seealso cref="HistoricDetail"/>s, so only items which have no
	  /// task-id set will be selected. When used together with <seealso cref="taskId(string)"/>, this
	  /// call is ignored task details are NOT excluded.
	  /// </summary>
	  HistoricDetailQuery excludeTaskDetails();

	  /// <summary>
	  /// Only select historic details with one of the given tenant ids. </summary>
	  HistoricDetailQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only select historic details with the given process instance ids. </summary>
	  HistoricDetailQuery processInstanceIdIn(params string[] processInstanceIds);

	  /// <summary>
	  /// Select historic details related with given userOperationId.
	  /// </summary>
	  HistoricDetailQuery userOperationId(string userOperationId);

	  /// <summary>
	  /// Only select historic details that have occurred before the given date (inclusive). </summary>
	  HistoricDetailQuery occurredBefore(DateTime date);

	  /// <summary>
	  /// Only select historic details that have occurred after the given date (inclusive). </summary>
	  HistoricDetailQuery occurredAfter(DateTime date);

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>).
	  /// Note that the ordering of historic details without tenant id is database-specific.
	  /// </summary>
	  HistoricDetailQuery orderByTenantId();

	  HistoricDetailQuery orderByProcessInstanceId();

	  HistoricDetailQuery orderByVariableName();

	  HistoricDetailQuery orderByFormPropertyId();

	  HistoricDetailQuery orderByVariableType();

	  HistoricDetailQuery orderByVariableRevision();

	  HistoricDetailQuery orderByTime();

	  /// <summary>
	  /// <para>Sort the <seealso cref="HistoricDetail historic detail events"/> in the order in which
	  /// they occurred and needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>.</para>
	  /// 
	  /// <para>The set of all <seealso cref="HistoricVariableUpdate historic variable update events"/> is
	  /// a <strong>partially ordered set</strong>. Due to this fact {@link HistoricVariableUpdate
	  /// historic variable update events} for two different {@link VariableInstance variable
	  /// instances} are <strong>incomparable</strong>. So that it is not possible to sort
	  /// the <seealso cref="HistoricDetail historic variable update events"/> for two {@link VariableInstance
	  /// variable instances} in the order they occurred. Just for one {@link VariableInstance variable
	  /// instance} the set of <seealso cref="HistoricVariableUpdate historic variable update events"/> can be
	  /// <strong>totally ordered</strong> by using <seealso cref="variableInstanceId(string)"/> and {@link
	  /// #orderPartiallyByOccurrence()} which will return a result set ordered by its occurrence.</para>
	  /// 
	  /// <para><strong>For example:</strong><br>
	  /// An execution variable <code>myVariable</code> will be updated multiple times:</para>
	  /// 
	  /// <code>
	  /// runtimeService.setVariable("anExecutionId", "myVariable", 1000);<br>
	  /// execution.setVariable("myVariable", 5000);<br>
	  /// runtimeService.setVariable("anExecutionId", "myVariable", 2500);<br>
	  /// runtimeService.removeVariable("anExecutionId", "myVariable");
	  /// </code>
	  /// 
	  /// <para>As a result there exists four <seealso cref="HistoricVariableUpdate historic variable update events"/>.</para>
	  /// 
	  /// <para>By using <seealso cref="variableInstanceId(string)"/> and <seealso cref="orderPartiallyByOccurrence()"/> it
	  /// is possible to sort the events in the order in which they occurred. The following query</para>
	  /// 
	  /// <code>
	  /// historyService.createHistoricDetailQuery()<br>
	  /// &nbsp;&nbsp;.variableInstanceId("myVariableInstId")<br>
	  /// &nbsp;&nbsp;.orderPartiallyByOccurrence()<br>
	  /// &nbsp;&nbsp;.asc()<br>
	  /// &nbsp;&nbsp;.list()
	  /// </code>
	  /// 
	  /// <para>will return the following totally ordered result set</para>
	  /// 
	  /// <code>
	  /// [<br>
	  /// &nbsp;&nbsp;HistoricVariableUpdate[id: "myVariableInstId", variableName: "myVariable", value: 1000],<br>
	  /// &nbsp;&nbsp;HistoricVariableUpdate[id: "myVariableInstId", variableName: "myVariable", value: 5000],<br>
	  /// &nbsp;&nbsp;HistoricVariableUpdate[id: "myVariableInstId", variableName: "myVariable", value: 2500]<br>
	  /// &nbsp;&nbsp;HistoricVariableUpdate[id: "myVariableInstId", variableName: "myVariable", value: null]<br>
	  /// ]
	  /// </code>
	  /// 
	  /// <para><strong>Note:</strong><br>
	  /// Please note that a <seealso cref="HistoricFormField historic form field event"/> can occur only once.</para>
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoricDetailQuery orderPartiallyByOccurrence();
	}

}