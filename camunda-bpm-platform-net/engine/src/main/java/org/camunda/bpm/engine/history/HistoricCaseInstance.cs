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

	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// A single execution of a case definition that is stored permanently.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface HistoricCaseInstance
	{

	  /// <summary>
	  /// The case instance id (== as the id of the runtime <seealso cref="CaseInstance"/>). </summary>
	  string Id {get;}

	  /// <summary>
	  /// The user provided unique reference to this process instance. </summary>
	  string BusinessKey {get;}

	  /// <summary>
	  /// The case definition reference. </summary>
	  string CaseDefinitionId {get;}

	  /// <summary>
	  /// The case definition key </summary>
	  string CaseDefinitionKey {get;}

	  /// <summary>
	  /// The case definition name </summary>
	  string CaseDefinitionName {get;}

	  /// <summary>
	  /// The time the case was created. </summary>
	  DateTime CreateTime {get;}

	  /// <summary>
	  /// The time the case was closed. </summary>
	  DateTime CloseTime {get;}

	  /// <summary>
	  /// The difference between <seealso cref="#getCloseTime()"/> and <seealso cref="#getCreateTime()"/>. </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// The authenticated user that created this case instance. </summary>
	  /// <seealso cref= IdentityService#setAuthenticatedUserId(String)  </seealso>
	  string CreateUserId {get;}

	  /// <summary>
	  /// The case instance id of a potential super case instance or null if no super case instance exists. </summary>
	  string SuperCaseInstanceId {get;}

	  /// <summary>
	  /// The process instance id of a potential super process instance or null if no super process instance exists. </summary>
	  string SuperProcessInstanceId {get;}

	  /// <summary>
	  /// The id of the tenant this historic case instance belongs to. Can be <code>null</code>
	  /// if the historic case instance belongs to no single tenant.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// Check if the case is active. </summary>
	  bool Active {get;}

	  /// <summary>
	  /// Check if the case is completed. </summary>
	  bool Completed {get;}

	  /// <summary>
	  /// Check if the case is terminated. </summary>
	  bool Terminated {get;}

	  /// <summary>
	  /// Check if the case is closed. </summary>
	  bool Closed {get;}

	}

}