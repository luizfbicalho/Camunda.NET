using System.Collections.Generic;

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
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;

	/// <summary>
	/// <para>A fluent builder for defining message correlation</para>
	/// 
	/// @author Daniel Meyer
	/// @author Christopher Zell
	/// 
	/// </summary>
	public interface MessageCorrelationBuilder
	{

	  /// <summary>
	  /// <para>
	  /// Correlate the message such that the process instance has a business key with
	  /// the given name. If the message is correlated to a message start
	  /// event then the given business key is set on the created process instance.
	  /// </para>
	  /// </summary>
	  /// <param name="businessKey">
	  ///          the businessKey to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder processInstanceBusinessKey(string businessKey);

	  /// <summary>
	  /// <para>Correlate the message such that the process instance has a
	  /// variable with the given name and value.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the process instance variable to correlate on. </param>
	  /// <param name="variableValue"> the value of the process instance variable to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder processInstanceVariableEquals(string variableName, object variableValue);

	  /// <summary>
	  /// <para>
	  /// Correlate the message such that the process instance has the given variables.
	  /// </para>
	  /// </summary>
	  /// <param name="variables"> the variables of the process instance to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder processInstanceVariablesEqual(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Correlate the message such that the execution has a local variable with the given name and value.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the local variable to correlate on. </param>
	  /// <param name="variableValue"> the value of the local variable to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder localVariableEquals(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Correlate the message such that the execution has the given variables as local variables.
	  /// </para>
	  /// </summary>
	  /// <param name="variables"> the local variables of the execution to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder localVariablesEqual(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Correlate the message such that a process instance with the given id is selected.</para>
	  /// </summary>
	  /// <param name="id"> the id of the process instance to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder processInstanceId(string id);

	  /// <summary>
	  /// <para>Correlate the message such that a process definition with the given id is selected.
	  /// Is only supported for <seealso cref="#correlateStartMessage()"/>.</para>
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition to correlate on. </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Pass a variable to the execution waiting on the message. Use this method for passing the
	  /// message's payload.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to set </param>
	  /// <param name="variableValue"> the value of the variable to set </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder setVariable(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Pass a local variable to the execution waiting on the message. Use this method for passing the
	  /// message's payload.</para>
	  /// 
	  /// <para>Invoking this method multiple times allows passing multiple variables.</para>
	  /// </summary>
	  /// <param name="variableName"> the name of the variable to set </param>
	  /// <param name="variableValue"> the value of the variable to set </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder setVariableLocal(string variableName, object variableValue);

	  /// <summary>
	  /// <para>Pass a map of variables to the execution waiting on the message. Use this method
	  /// for passing the message's payload</para>
	  /// </summary>
	  /// <param name="variables"> the map of variables </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder setVariables(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Pass a map of local variables to the execution waiting on the message. Use this method
	  /// for passing the message's payload</para>
	  /// </summary>
	  /// <param name="variables"> the map of local variables </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder setVariablesLocal(IDictionary<string, object> variables);

	  /// <summary>
	  /// Specify a tenant to deliver the message to. The message can only be
	  /// received on executions or process definitions which belongs to the given
	  /// tenant. Cannot be used in combination with
	  /// <seealso cref="#processInstanceId(String)"/> or <seealso cref="#processDefinitionId(String)"/>.
	  /// </summary>
	  /// <param name="tenantId">
	  ///          the id of the tenant </param>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder tenantId(string tenantId);

	  /// <summary>
	  /// Specify that the message can only be received on executions or process
	  /// definitions which belongs to no tenant. Cannot be used in combination with
	  /// <seealso cref="#processInstanceId(String)"/> or <seealso cref="#processDefinitionId(String)"/>.
	  /// </summary>
	  /// <returns> the builder </returns>
	  MessageCorrelationBuilder withoutTenantId();

	  /// <summary>
	  /// Executes the message correlation.
	  /// </summary>
	  /// <seealso cref= <seealso cref="#correlateWithResult()"/> </seealso>
	  void correlate();


	  /// <summary>
	  /// Executes the message correlation and returns a <seealso cref="MessageCorrelationResult"/> object.
	  /// 
	  /// <para>The call of this method will result in either:
	  /// <ul>
	  /// <li>Exactly one waiting execution is notified to continue. The notification is performed synchronously. The result contains the execution id.</li>
	  /// <li>Exactly one Process Instance is started in case the message name matches a message start event of a
	  ///     process. The instantiation is performed synchronously. The result contains the start event activity id and process definition.</li>
	  /// <li>MismatchingMessageCorrelationException is thrown. This means that either too many executions / process definitions match the
	  ///     correlation or that no execution and process definition matches the correlation.</li>
	  /// </ul>
	  /// </para>
	  /// The result can be identified by calling the <seealso cref="MessageCorrelationResult#getResultType"/>.
	  /// </summary>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is matched by the correlation </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if one execution is matched and the user has no <seealso cref="Permissions#UPDATE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_INSTANCE"/> or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///          <li>if one process definition is matched and the user has no <seealso cref="Permissions#CREATE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_INSTANCE"/> and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  /// </exception>
	  /// <returns> The result of the message correlation. Result contains either the execution id or the start event activity id and the process definition.
	  /// @since 7.6 </returns>
	  MessageCorrelationResult correlateWithResult();

	  /// <summary>
	  /// Executes the message correlation. If you do not need access to the process variables, use <seealso cref="#correlateWithResult()"/>
	  /// to avoid unnecessary variable access.
	  /// </summary>
	  /// <seealso cref= <seealso cref="#correlateWithResult()"/>
	  /// </seealso>
	  /// <param name="deserializeValues"> if false, returned <seealso cref="SerializableValue"/>s
	  ///   will not be deserialized (unless they are passed into this method as a
	  ///   deserialized value or if the BPMN process triggers deserialization)
	  /// </param>
	  /// <returns> The result of the message correlation. Result contains either the
	  ///         execution id or the start event activity id, the process definition,
	  ///         and the process variables. </returns>
	  MessageCorrelationResultWithVariables correlateWithResultAndVariables(bool deserializeValues);

	  /// <summary>
	  /// <para>
	  ///   Behaves like <seealso cref="#correlate()"/>, however uses pessimistic locking for correlating a waiting execution, meaning
	  ///   that two threads correlating a message to the same execution in parallel do not end up continuing the
	  ///   process in parallel until the next wait state is reached
	  /// </para>
	  /// <para>
	  ///   <strong>CAUTION:</strong> Wherever there are pessimistic locks, there is a potential for deadlocks to occur.
	  ///   This can either happen when multiple messages are correlated in parallel, but also with other
	  ///   race conditions such as a message boundary event on a user task. The process engine is not able to detect such a potential.
	  ///   In consequence, the user of this API should investigate this potential in his/her use case and implement
	  ///   countermeasures if needed.
	  /// </para>
	  /// <para>
	  ///   A less error-prone alternative to this method is to set appropriate async boundaries in the process model
	  ///   such that parallel message correlation is solved by optimistic locking.
	  /// </para>
	  /// </summary>
	  void correlateExclusively();


	  /// <summary>
	  /// Executes the message correlation for multiple messages.
	  /// </summary>
	  /// <seealso cref= <seealso cref="#correlateAllWithResult()"/> </seealso>
	  void correlateAll();

	  /// <summary>
	  /// Executes the message correlation for multiple messages and returns a list of message correlation results.
	  /// 
	  /// <para>This will result in any number of the following:
	  /// <ul>
	  /// <li>Any number of waiting executions are notified to continue. The notification is performed synchronously. The result list contains the execution ids of the
	  /// notified executions.</li>
	  /// <li>Any number of process instances are started which have a message start event that matches the message name. The instantiation is performed synchronously.
	  /// The result list contains the start event activity ids and process definitions from all activities on that the messages was correlated to.</li>
	  /// </ul>
	  /// </para>
	  /// <para>Note that the message correlates to all tenants if no tenant is specified using <seealso cref="#tenantId(String)"/> or <seealso cref="#withoutTenantId()"/>.</para>
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if at least one execution is matched and the user has no <seealso cref="Permissions#UPDATE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_INSTANCE"/> or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///          <li>if one process definition is matched and the user has no <seealso cref="Permissions#CREATE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_INSTANCE"/> and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///          <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  /// </exception>
	  /// <returns> The result list of the message correlations. Each result contains
	  /// either the execution id or the start event activity id and the process definition.
	  /// @since 7.6 </returns>
	  IList<MessageCorrelationResult> correlateAllWithResult();

	  /// <summary>
	  /// Executes the message correlation. If you do not need access to the process variables, use <seealso cref="#correlateAllWithResult()"/>
	  /// to avoid unnecessary variable access.
	  /// </summary>
	  /// <seealso cref= <seealso cref="#correlateAllWithResult()"/>
	  /// </seealso>
	  /// <param name="deserializeValues"> if false, returned <seealso cref="SerializableValue"/>s
	  ///   will not be deserialized (unless they are passed into this method as a
	  ///   deserialized value or if the BPMN process triggers deserialization)
	  /// </param>
	  /// <returns> The result list of the message correlations. Each result contains
	  ///         either the execution id or the start event activity id, the process
	  ///         definition, and the process variables. </returns>
	  IList<MessageCorrelationResultWithVariables> correlateAllWithResultAndVariables(bool deserializeValues);

	  /// <summary>
	  /// Executes the message correlation.
	  /// 
	  /// <para>
	  /// This will result in either:
	  /// <ul>
	  /// <li>Exactly one Process Instance is started in case the message name
	  /// matches a message start event of a process. The instantiation is performed
	  /// synchronously.</li>
	  /// <li>MismatchingMessageCorrelationException is thrown. This means that
	  /// either no process definition or more than one process definition matches
	  /// the correlation.</li>
	  /// </ul>
	  /// </para>
	  /// </summary>
	  /// <returns> the newly created process instance
	  /// </returns>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///           if none or more than one process definition is matched by the correlation </exception>
	  /// <exception cref="AuthorizationException">
	  ///           if one process definition is matched and the user has no
	  ///           <seealso cref="Permissions#CREATE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_INSTANCE"/> and no
	  ///           <seealso cref="Permissions#CREATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance correlateStartMessage();

	}

}