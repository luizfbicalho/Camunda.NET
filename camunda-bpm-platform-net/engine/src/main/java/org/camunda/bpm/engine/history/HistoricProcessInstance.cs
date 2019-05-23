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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// A single execution of a whole process definition that is stored permanently.
	/// 
	/// @author Christian Stettler
	/// @author Askar Akhmerov
	/// </summary>
	public interface HistoricProcessInstance
	{

	  /// <summary>
	  /// The process instance id (== as the id for the runtime <seealso cref="ProcessInstance process instance"/>). </summary>
	  string Id {get;}

	  /// <summary>
	  /// The user provided unique reference to this process instance. </summary>
	  string BusinessKey {get;}

	  /// <summary>
	  /// The process definition key reference. </summary>
	  string ProcessDefinitionKey {get;}

	  /// <summary>
	  /// The process definition reference. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The process definition name. </summary>
	  string ProcessDefinitionName {get;}

	  /// <summary>
	  /// The process definition version. </summary>
	  int? ProcessDefinitionVersion {get;}

	  /// <summary>
	  /// The time the process was started. </summary>
	  DateTime StartTime {get;}

	  /// <summary>
	  /// The time the process was ended. </summary>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// The time the historic process instance will be removed. </summary>
	  DateTime RemovalTime {get;}

	  /// <summary>
	  /// The difference between <seealso cref="getEndTime()"/> and <seealso cref="getStartTime()"/> . </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// Reference to the activity in which this process instance ended.
	  ///  Note that a process instance can have multiple end events, in this case it might not be deterministic
	  ///  which activity id will be referenced here. Use a <seealso cref="HistoricActivityInstanceQuery"/> instead to query
	  ///  for end events of the process instance (use the activityTYpe attribute)
	  /// 
	  /// </summary>
	  [Obsolete]
	  string EndActivityId {get;}

	  /// <summary>
	  /// The authenticated user that started this process instance. </summary>
	  /// <seealso cref= IdentityService#setAuthenticatedUserId(String)  </seealso>
	  string StartUserId {get;}

	  /// <summary>
	  /// The start activity. </summary>
	  string StartActivityId {get;}

	  /// <summary>
	  /// Obtains the reason for the process instance's deletion. </summary>
	  string DeleteReason {get;}

	  /// <summary>
	  /// The process instance id of a potential super process instance or null if no super process instance exists
	  /// </summary>
	  string SuperProcessInstanceId {get;}

	  /// <summary>
	  /// The process instance id of the top-level (root) process instance or null if no root process instance exists
	  /// </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The case instance id of a potential super case instance or null if no super case instance exists
	  /// </summary>
	  string SuperCaseInstanceId {get;}

	  /// <summary>
	  /// The case instance id of a potential super case instance or null if no super case instance exists
	  /// </summary>
	  string CaseInstanceId {get;}

	  /// <summary>
	  /// The id of the tenant this historic process instance belongs to. Can be <code>null</code>
	  /// if the historic process instance belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// Return current state of HistoricProcessInstance, following values are recognized during process engine operations:
	  /// 
	  ///  STATE_ACTIVE - running process instance
	  ///  STATE_SUSPENDED - suspended process instances
	  ///  STATE_COMPLETED - completed through normal end event
	  ///  STATE_EXTERNALLY_TERMINATED - terminated externally, for instance through REST API
	  ///  STATE_INTERNALLY_TERMINATED - terminated internally, for instance by terminating boundary event
	  /// </summary>
	  string State {get;}
	}

	public static class HistoricProcessInstance_Fields
	{
	  public const string STATE_ACTIVE = "ACTIVE";
	  public const string STATE_SUSPENDED = "SUSPENDED";
	  public const string STATE_COMPLETED = "COMPLETED";
	  public const string STATE_EXTERNALLY_TERMINATED = "EXTERNALLY_TERMINATED";
	  public const string STATE_INTERNALLY_TERMINATED = "INTERNALLY_TERMINATED";
	}

}