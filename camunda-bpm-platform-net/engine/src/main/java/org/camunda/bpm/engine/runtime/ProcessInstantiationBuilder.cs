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
namespace org.camunda.bpm.engine.runtime
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface ProcessInstantiationBuilder : ActivityInstantiationBuilder<ProcessInstantiationBuilder>, InstantiationBuilder<ProcessInstantiationBuilder>
	{

	  /// <summary>
	  /// Specify the id of the tenant the process definition belongs to. Can only be
	  /// used when the definition is referenced by <code>key</code> and not by <code>id</code>.
	  /// </summary>
	  ProcessInstantiationBuilder processDefinitionTenantId(string tenantId);

	  /// <summary>
	  /// Specify that the process definition belongs to no tenant. Can only be
	  /// used when the definition is referenced by <code>key</code> and not by <code>id</code>.
	  /// </summary>
	  ProcessInstantiationBuilder processDefinitionWithoutTenantId();

	  /// <summary>
	  /// Set the business key for the process instance
	  /// </summary>
	  ProcessInstantiationBuilder businessKey(string businessKey);

	  /// <summary>
	  /// Associate a case instance with the process instance
	  /// </summary>
	  ProcessInstantiationBuilder caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Start the process instance.
	  /// </summary>
	  /// <returns> the newly created process instance </returns>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> and no
	  ///           <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  /// <seealso cref= also <seealso cref="#executeWithVariablesInReturn()"/>. </seealso>
	  ProcessInstance execute();

	  /// <summary>
	  /// Start the process instance.
	  /// </summary>
	  /// <param name="skipCustomListeners">
	  ///          specifies whether custom listeners (task and execution) should be
	  ///          invoked when executing the instructions. Only supported for
	  ///          instructions. </param>
	  /// <param name="skipIoMappings">
	  ///          specifies whether input/output mappings for tasks should be
	  ///          invoked throughout the transaction when executing the
	  ///          instructions. Only supported for instructions. </param>
	  /// <returns> the newly created process instance </returns>
	  /// <seealso cref= also <seealso cref="#executeWithVariablesInReturn(boolean, boolean)"/>. </seealso>
	  ProcessInstance execute(bool skipCustomListeners, bool skipIoMappings);

	  /// <summary>
	  /// Start the process instance. If no instantiation instructions are set then
	  /// the instance start at the default start activity. Otherwise, all
	  /// instructions are executed in the order they are submitted. Custom execution
	  /// and task listeners, as well as task input output mappings are triggered.
	  /// </summary>
	  /// <returns> the newly created process instance with the latest variables
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> and no
	  ///           <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstanceWithVariables executeWithVariablesInReturn();

	  /// <summary>
	  /// Start the process instance. If no instantiation instructions are set then
	  /// the instance start at the default start activity. Otherwise, all
	  /// instructions are executed in the order they are submitted.
	  /// </summary>
	  /// <param name="skipCustomListeners">
	  ///          specifies whether custom listeners (task and execution) should be
	  ///          invoked when executing the instructions. Only supported for
	  ///          instructions. </param>
	  /// <param name="skipIoMappings">
	  ///          specifies whether input/output mappings for tasks should be
	  ///          invoked throughout the transaction when executing the
	  ///          instructions. Only supported for instructions. </param>
	  /// <returns> the newly created process instance with the latest variables
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///           if the user has no <seealso cref="Permissions#CREATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> and no
	  ///           <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <exception cref="ProcessEngineException">
	  ///           if {@code skipCustomListeners} or {@code skipIoMappings} is set
	  ///           to true but no instructions are submitted. Both options are not
	  ///           supported when the instance starts at the default start activity.
	  ///           Use <seealso cref="#execute()"/> instead.
	  ///  </exception>
	  ProcessInstanceWithVariables executeWithVariablesInReturn(bool skipCustomListeners, bool skipIoMappings);
	}

}