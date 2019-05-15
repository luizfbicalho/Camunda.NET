﻿/*
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
namespace org.camunda.bpm.engine.impl.cfg.multitenancy
{

	/// <summary>
	/// SPI which can be implemented to assign tenant ids to process instances, case instances and historic decision instances.
	/// <para>
	/// The SPI is invoked if the process definition, case definition or decision definition does not have a tenant id or
	/// execution does not have a tenant id.
	/// </para>
	/// <para>
	/// An implementation of this SPI can be set on the <seealso cref="ProcessEngineConfigurationImpl"/>.
	/// 
	/// @author Daniel Meyer
	/// @since 7.5
	/// </para>
	/// </summary>
	public interface TenantIdProvider
	{

	  /// <summary>
	  /// Invoked when a process instance is started and the Process Definition does not have a tenant id.
	  /// <para>
	  /// Implementors can either return a tenant id or null. If null is returned the process instance is not assigned a tenant id.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="ctx"> holds information about the process instance which is about to be started. </param>
	  /// <returns> a tenant id or null if case the implementation does not assign a tenant id to the process instance </returns>
	  string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx);

	  /// <summary>
	  /// Invoked when a case instance is started and the Case Definition does not have a tenant id.
	  /// <para>
	  /// Implementors can either return a tenant id or null. If null is returned the case instance is not assigned a tenant id.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="ctx"> holds information about the case instance which is about to be started. </param>
	  /// <returns> a tenant id or null if case the implementation does not assign a tenant id to case process instance </returns>
	  string provideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx);

	  /// <summary>
	  /// Invoked when a historic decision instance is created and the Decision Definition or the Execution does not have a tenant id.
	  /// <para>
	  /// Implementors can either return a tenant id or null. If null is returned the historic decision instance is not assigned a tenant id.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="ctx"> holds information about the decision definition and the execution. </param>
	  /// <returns> a tenant id or null if case the implementation does not assign a tenant id to the historic decision instance </returns>
	  string provideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx);

	}

}