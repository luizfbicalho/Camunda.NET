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



	/// <summary>
	/// Base class for all kinds of information that is related to
	/// either a <seealso cref="HistoricProcessInstance"/> or a <seealso cref="HistoricActivityInstance"/>.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricDetail
	{

	  /// <summary>
	  /// The unique DB id for this historic detail </summary>
	  string Id {get;}

	  /// <summary>
	  /// The process definition key reference. </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// The process definition reference. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The root process instance reference </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The process instance reference. </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// The activity reference in case this detail is related to an activity instance. </summary>
	  string ActivityInstanceId {get;}

	  /// <summary>
	  /// The identifier for the path of execution. </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// The case definition key reference. </summary>
	  string CaseDefinitionKey {get;}

	  /// <summary>
	  /// The case definition reference. </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// The case instance reference. </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// The case execution reference. </summary>
	  string CaseExecutionId {get;}

	  /// <summary>
	  /// The identifier for the task. </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// The time when this detail occurred </summary>
	  DateTime Time {get;}

	  /// <summary>
	  /// The id of the tenant this historic detail belongs to. Can be <code>null</code>
	  /// if the historic detail belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// The id of operation. Helps to link records in different historic tables.
	  /// References operationId of user operation log entry.
	  /// </summary>
	  string UserOperationId {get;}

	  /// <summary>
	  /// The time the historic detail will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}