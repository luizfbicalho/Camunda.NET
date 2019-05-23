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
	/// Represents one execution of a case activity which is stored permanent for statistics, audit and other business intelligence purposes.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface HistoricCaseActivityInstance
	{

	   /// <summary>
	   /// The id of the case activity instance (== as the id of the runtime activity). </summary>
	   string Id {get;}

	   /// <summary>
	   /// The id of the parent case activity instance. </summary>
	   string ParentCaseActivityInstanceId {get;}

	   /// <summary>
	   /// The unique identifier of the case activity in the case. </summary>
	   string CaseActivityId {get;}

	   /// <summary>
	   /// The display name for the case activity. </summary>
	   string CaseActivityName {get;}

	   /// <summary>
	   /// The display type for the case activity. </summary>
	   string CaseActivityType {get;}

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
	   /// The corresponding task in case of a human task activity. </summary>
	   string TaskId {get;}

	   /// <summary>
	   /// The corresponding process in case of a process task activity. </summary>
	   string CalledProcessInstanceId {get;}

	   /// <summary>
	   /// The corresponding case in case of a case task activity. </summary>
	   string CalledCaseInstanceId {get;}

	   /// <summary>
	   /// The id of the tenant this historic case activity instance belongs to. Can be <code>null</code>
	   /// if the historic case activity instance belongs to no single tenant.
	   /// </summary>
	   string TenantId {get;}

	   /// <summary>
	   /// The time when the case activity was created. </summary>
	   DateTime CreateTime {get;}

	   /// <summary>
	   /// The time when the case activity ended </summary>
	   DateTime EndTime {get;}

	   /// <summary>
	   /// Difference between <seealso cref="getEndTime()"/> and <seealso cref="getCreateTime()"/>. </summary>
	   long? DurationInMillis {get;}

	   /// <summary>
	   /// Check if the case activity is required. </summary>
	   bool Required {get;}

		/// <summary>
		/// Check if the case activity is available. </summary>
	   bool Available {get;}

	   /// <summary>
	   /// Check if the case activity is enabled. </summary>
	   bool Enabled {get;}

	   /// <summary>
	   /// Check if the case activity is disabled. </summary>
	   bool Disabled {get;}

	   /// <summary>
	   /// Check if the case activity is active. </summary>
	   bool Active {get;}

	   /// <summary>
	   /// Check if the case activity is completed. </summary>
	   bool Completed {get;}

	   /// <summary>
	   /// Check if the case activity is terminated. </summary>
	   bool Terminated {get;}

	}

}