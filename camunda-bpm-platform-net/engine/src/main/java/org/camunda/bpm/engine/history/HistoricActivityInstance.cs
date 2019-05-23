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
	/// Represents one execution of an activity and it's stored permanent for statistics, audit and other business intelligence purposes.
	/// 
	/// @author Christian Stettler
	/// </summary>
	public interface HistoricActivityInstance
	{

	  /// <summary>
	  /// The unique identifier of this historic activity instance. </summary>
	  string Id {get;}

	  /// <summary>
	  /// return the id of the parent activity instance </summary>
	  string ParentActivityInstanceId {get;}

	  /// <summary>
	  /// The unique identifier of the activity in the process </summary>
	  string ActivityId {get;}

	  /// <summary>
	  /// The display name for the activity </summary>
	  string ActivityName {get;}

	  /// <summary>
	  /// The activity type of the activity.
	  /// Typically the activity type correspond to the XML tag used in the BPMN 2.0 process definition file.
	  /// 
	  /// All activity types are available in <seealso cref="org.camunda.bpm.engine.ActivityTypes"/>
	  /// </summary>
	  /// <seealso cref= org.camunda.bpm.engine.ActivityTypes </seealso>
	  string ActivityType {get;}

	  /// <summary>
	  /// Process definition key reference </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// Process definition reference </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Root process instance reference </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// Process instance reference </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Execution reference </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// The corresponding task in case of task activity </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// The called process instance in case of call activity </summary>
	  string CalledProcessInstanceId {get;}

	  /// <summary>
	  /// The called case instance in case of (case) call activity </summary>
	  string CalledCaseInstanceId {get;}

	  /// <summary>
	  /// Assignee in case of user task activity </summary>
	  string Assignee {get;}

	  /// <summary>
	  /// Time when the activity instance started </summary>
	  DateTime StartTime {get;}

	  /// <summary>
	  /// Time when the activity instance ended </summary>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// Difference between <seealso cref="getEndTime()"/> and <seealso cref="getStartTime()"/>. </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// Did this activity instance complete a BPMN 2.0 scope </summary>
	  bool CompleteScope {get;}

	  /// <summary>
	  /// Was this activity instance canceled </summary>
	  bool Canceled {get;}

	  /// <summary>
	  /// The id of the tenant this historic activity instance belongs to. Can be <code>null</code>
	  /// if the historic activity instance belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// The time the historic activity instance will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}