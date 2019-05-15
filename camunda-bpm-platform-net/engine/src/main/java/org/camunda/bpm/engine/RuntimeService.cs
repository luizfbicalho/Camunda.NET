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
namespace org.camunda.bpm.engine
{

	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;
	using MigrationPlanExecutionBuilder = org.camunda.bpm.engine.migration.MigrationPlanExecutionBuilder;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ConditionEvaluationBuilder = org.camunda.bpm.engine.runtime.ConditionEvaluationBuilder;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using ModificationBuilder = org.camunda.bpm.engine.runtime.ModificationBuilder;
	using NativeExecutionQuery = org.camunda.bpm.engine.runtime.NativeExecutionQuery;
	using NativeProcessInstanceQuery = org.camunda.bpm.engine.runtime.NativeProcessInstanceQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;
	using SignalEventReceivedBuilder = org.camunda.bpm.engine.runtime.SignalEventReceivedBuilder;
	using UpdateProcessInstanceSuspensionStateBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateBuilder;
	using UpdateProcessInstanceSuspensionStateSelectBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstanceSuspensionStateSelectBuilder;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// Service which provides access to <seealso cref="Deployment"/>s,
	/// <seealso cref="ProcessDefinition"/>s and <seealso cref="ProcessInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public interface RuntimeService
	{

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process definition with the given key.
	  /// </summary>
	  /// <param name="processDefinitionKey"> key of process definition, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey);

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process
	  /// definition with the given key.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionKey">
	  ///          key of process definition, cannot be null. </param>
	  /// <param name="businessKey">
	  ///          a key that uniquely identifies the process instance in the context
	  ///          of the given process definition.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey);

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process
	  /// definition with the given key.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionKey">
	  ///          key of process definition, cannot be null. </param>
	  /// <param name="businessKey">
	  ///          a key that uniquely identifies the process instance in the context
	  ///          of the given process definition. </param>
	  /// <param name="caseInstanceId">
	  ///          an id of a case instance to associate the process instance with
	  ///          a case instance.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, string caseInstanceId);

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process definition with the given key
	  /// </summary>
	  /// <param name="processDefinitionKey"> key of process definition, cannot be null. </param>
	  /// <param name="variables"> the variables to pass, can be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables);

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process definition with the given key.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added a
	  /// database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// 
	  /// The combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionKey"> key of process definition, cannot be null. </param>
	  /// <param name="variables"> the variables to pass, can be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, IDictionary<string, object> variables);

	  /// <summary>
	  /// Starts a new process instance in the latest version of the process definition with the given key.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added a
	  /// database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// 
	  /// The combination of processdefinitionKey-businessKey must be unique. </summary>
	  /// <param name="processDefinitionKey"> key of process definition, cannot be null. </param>
	  /// <param name="variables"> the variables to pass, can be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition. </param>
	  /// <param name="caseInstanceId">
	  ///          an id of a case instance to associate the process instance with
	  ///          a case instance.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, string caseInstanceId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition. </param>
	  /// <param name="caseInstanceId">
	  ///          an id of a case instance to associate the process instance with
	  ///          a case instance.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, string caseInstanceId);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="variables"> variables to be passed, can be null
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition. </param>
	  /// <param name="variables"> variables to be passed, can be null
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, IDictionary<string, object> variables);

	  /// <summary>
	  /// Starts a new process instance in the exactly specified version of the process definition with the given id.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery#processInstanceBusinessKey(String)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition WHEN you have added
	  /// a database constraint for it.
	  /// In this case, only Process instance from different process definition are allowed to have the
	  /// same business key and the combination of processdefinitionKey-businessKey must be unique.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context of the
	  ///                    given process definition. </param>
	  /// <param name="caseInstanceId">
	  ///          an id of a case instance to associate the process instance with
	  ///          a case instance. </param>
	  /// <param name="variables"> variables to be passed, can be null
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, string caseInstanceId, IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// <para>Calling this method can have two different outcomes:
	  /// <ul>
	  /// <li>If the message name is associated with a message start event, a new
	  /// process instance is started.</li>
	  /// <li>If no subscription to a message with the given name exists, <seealso cref="ProcessEngineException"/>
	  /// is thrown</li>
	  /// </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 5.9 </exception>
	  ProcessInstance startProcessInstanceByMessage(string messageName);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String, Map)"/>. This method allows
	  /// specifying a business key.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element. </param>
	  /// <param name="businessKey">
	  ///          the business key which is added to the started process instance
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 5.10 </exception>
	  ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String)"/>. In addition, this method allows
	  /// specifying a the payload of the message as a map of process variables.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element. </param>
	  /// <param name="processVariables">
	  ///          the 'payload' of the message. The variables are added as processes
	  ///          variables to the started process instance.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 5.9 </exception>
	  ProcessInstance startProcessInstanceByMessage(string messageName, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String, Map)"/>. In addition, this method allows
	  /// specifying a business key.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element. </param>
	  /// <param name="businessKey">
	  ///          the business key which is added to the started process instance </param>
	  /// <param name="processVariables">
	  ///          the 'payload' of the message. The variables are added as processes
	  ///          variables to the started process instance.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 5.9 </exception>
	  ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String)"/>. In addition, this method allows
	  /// specifying the exactly version of the process definition with the given id.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element, cannot be null. </param>
	  /// <param name="processDefinitionId">
	  ///      the id of the process definition, cannot be null.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists for the
	  ///          specified version of process definition. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.3 </exception>
	  ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String, String)"/>. In addition, this method allows
	  /// specifying the exactly version of the process definition with the given id.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element, cannot be null. </param>
	  /// <param name="processDefinitionId">
	  ///      the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey">
	  ///          the business key which is added to the started process instance
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists for the
	  ///          specified version of process definition. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.3 </exception>
	  ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, string businessKey);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String, Map)"/>. In addition, this method allows
	  /// specifying the exactly version of the process definition with the given id.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element, cannot be null. </param>
	  /// <param name="processDefinitionId">
	  ///      the id of the process definition, cannot be null. </param>
	  /// <param name="processVariables">
	  ///          the 'payload' of the message. The variables are added as processes
	  ///          variables to the started process instance.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists for the
	  ///          specified version of process definition.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.3 </exception>
	  ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// <para>Signals the process engine that a message is received and starts a new
	  /// <seealso cref="ProcessInstance"/>.</para>
	  /// 
	  /// See <seealso cref="#startProcessInstanceByMessage(String, String, Map)"/>. In addition, this method allows
	  /// specifying the exactly version of the process definition with the given id.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the 'name' of the message as specified as an attribute on the
	  ///          bpmn20 {@code <message name="messageName" />} element, cannot be null. </param>
	  /// <param name="processDefinitionId">
	  ///      the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey">
	  ///          the business key which is added to the started process instance </param>
	  /// <param name="processVariables">
	  ///          the 'payload' of the message. The variables are added as processes
	  ///          variables to the started process instance.
	  /// </param>
	  /// <returns> the <seealso cref="ProcessInstance"/> object representing the started process instance
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if no subscription to a message with the given name exists for the
	  ///          specified version of process definition. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.3 </exception>
	  ProcessInstance startProcessInstanceByMessageAndProcessDefinitionId(string messageName, string processDefinitionId, string businessKey, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstance(string processInstanceId, string deleteReason);

	  /// <summary>
	  /// Delete an existing runtime process instances asynchronously using Batch operation.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceIds"> id's of process instances to delete, cannot be null if processInstanceQuery is null. </param>
	  /// <param name="processInstanceQuery"> query that will be used to fetch affected process instances.
	  ///                             Cannot be null if processInstanceIds are null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason);

	  /// <summary>
	  /// Delete an existing runtime process instances asynchronously using Batch operation.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceIds"> id's of process instances to delete, cannot be null if processInstanceQuery is null. </param>
	  /// <param name="processInstanceQuery"> query that will be used to fetch affected process instances.
	  ///                             Cannot be null if processInstanceIds are null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> skips custom execution listeners when removing instances </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason, bool skipCustomListeners);

	  /// <summary>
	  /// Delete an existing runtime process instances asynchronously using Batch operation.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceIds"> id's of process instances to delete, cannot be null if processInstanceQuery is null. </param>
	  /// <param name="processInstanceQuery"> query that will be used to fetch affected process instances.
	  ///                             Cannot be null if processInstanceIds are null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> skips custom execution listeners when removing instances </param>
	  /// <param name="skipSubprocesses"> skips subprocesses when removing instances
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, ProcessInstanceQuery processInstanceQuery, string deleteReason, bool skipCustomListeners, bool skipSubprocesses);

	  /// <summary>
	  /// Delete an existing runtime process instances asynchronously using Batch operation.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceQuery"> query that will be used to fetch affected process instances.
	  ///                             Cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteProcessInstancesAsync(ProcessInstanceQuery processInstanceQuery, string deleteReason);

	  /// <summary>
	  /// Delete an existing runtime process instances asynchronously using Batch operation.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// 
	  /// If both process instances list and query are provided, process instances containing in both sets
	  /// will be deleted.
	  /// </summary>
	  /// <param name="processInstanceIds"> id's of process instances to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated);


	  /// <summary>
	  /// Delete existing runtime process instances.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceIds"> ids of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstances(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated);

	  /// <summary>
	  /// Delete existing runtime process instances.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceIds"> ids of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call </param>
	  /// <param name="skipSubprocesses"> specifies whether subprocesses should be deleted
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstances(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipSubprocesses);

	  /// <summary>
	  /// Delete existing runtime process instances.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// 
	  /// Does not fail if a process instance was not found.
	  /// </summary>
	  /// <param name="processInstanceIds"> ids of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call </param>
	  /// <param name="skipSubprocesses"> specifies whether subprocesses should be deleted
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstancesIfExists(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipSubprocesses);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked </param>
	  /// <param name="skipSubprocesses"> specifies whether subprocesses should be deleted
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstance(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses);

	  /// <summary>
	  /// Delete an existing runtime process instance.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// 
	  /// Does not fail if a process instance was not found.
	  /// </summary>
	  /// <param name="processInstanceId"> id of process instance to delete, cannot be null. </param>
	  /// <param name="deleteReason"> reason for deleting, which will be stored in the history. Can be null. </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="externallyTerminated"> indicator if deletion triggered from external context, for instance
	  ///                             REST API call </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked </param>
	  /// <param name="skipSubprocesses"> specifies whether subprocesses should be deleted
	  /// 
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instance is found with the given id or id is null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#DELETE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessInstanceIfExists(string processInstanceId, string deleteReason, bool skipCustomListeners, bool externallyTerminated, bool skipIoMappings, bool skipSubprocesses);

	  /// <summary>
	  /// Finds the activity ids for all executions that are waiting in activities.
	  /// This is a list because a single activity can be active multiple times.
	  /// 
	  /// Deletion propagates upward as far as necessary.
	  /// </summary>
	  /// <param name="executionId"> id of the process instance or the execution, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution exists with the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  IList<string> getActiveActivityIds(string executionId);

	  /// <summary>
	  /// <para>Allows retrieving the activity instance tree for a given process instance.
	  /// The activity instance tree is aligned with the concept of scope in the BPMN specification.
	  /// Activities that are "on the same level of subprocess" (ie. part of the same scope, contained
	  /// in the same subprocess) will have their activity instances at the same level in the tree.</para>
	  /// 
	  /// <h2>Examples:</h2>
	  /// <para><ul>
	  ///  <li>Process with two parallel user tasks after parallel Gateway: in the activity instance tree you
	  ///  will see two activity instances below the root instance, one for each user task.</li>
	  ///  <li>Process with two parallel Multi Instance user tasks after parallel Gateway: in the activity instance
	  ///  tree, all instances of both user tasks will be listed below the root activity instance. Reason: all
	  ///  activity instances are at the same level of subprocess.</li>
	  ///  <li>Usertask inside embedded subprocess: the activity instance three will have 3 levels: the root instance
	  ///  representing the process instance itself, below it an activity instance representing the instance of the embedded
	  ///  subprocess, and below this one, the activity instance representing the usertask.</li>
	  /// </ul></para>
	  /// 
	  /// <h2>Identity & Uniqueness:</h2>
	  /// <para>Each activity instance is assigned a unique Id. The id is persistent, if you invoke this method multiple times,
	  /// the same activity instance ids will be returned for the same activity instances. (However, there might be
	  /// different executions assigned, see below)</para>
	  /// 
	  /// <h2>Relation to Executions</h2>
	  /// <para>The <seealso cref="Execution"/> concept in the process engine is not completely aligned with the activity
	  /// instance concept because the execution tree is in general not aligned with the activity / scope concept in
	  /// BPMN. In general, there is a n-1 relationship between Executions and ActivityInstances, ie. at a given
	  /// point in time, an activity instance can be linked to multiple executions. In addition, it is not guaranteed
	  /// that the same execution that started a given activity instance will also end it. The process engine performs
	  /// several internal optimizations concerning the compacting of the execution tree which might lead to executions
	  /// being reordered and pruned. This can lead to situations where a given execution starts an activity instance
	  /// but another execution ends it. Another special case is the process instance: if the process instance is executing
	  /// a non-scope activity (for example a user task) below the process definition scope, it will be referenced
	  /// by both the root activity instance and the user task activity instance.
	  /// 
	  /// </para>
	  /// <para><strong>If you need to interpret the state of a process instance in terms of a BPMN process model, it is usually easier to
	  /// use the activity instance tree as opposed to the execution tree.</strong></para>
	  /// </summary>
	  /// <param name="processInstanceId"> the id of the process instance for which the activity instance tree should be constructed.
	  /// </param>
	  /// <returns> the activity instance tree for a given process instance or null if no such process instance exists.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          if processInstanceId is 'null' or an internal error occurs. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.0 </exception>
	  ActivityInstance getActivityInstance(string processInstanceId);

	  /// <summary>
	  /// Sends an external trigger to an activity instance that is waiting inside the given execution.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the signal
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to signal, cannot be null.
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no execution is found for the given executionId or id is null. </exception>
	  /// <exception cref="SuspendedEntityInteractionException">
	  ///          when the execution is suspended. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void signal(string executionId);

	  /// <summary>
	  /// Sends an external trigger to an activity instance that is waiting inside the given execution.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the signal
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to signal, cannot be null. </param>
	  /// <param name="signalName"> name of the signal (can be null) </param>
	  /// <param name="signalData"> additional data of the signal (can be null) </param>
	  /// <param name="processVariables"> a map of process variables (can be null)
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void signal(string executionId, string signalName, object signalData, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Sends an external trigger to an activity instance that is waiting inside the given execution.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the signal
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to signal, cannot be null. </param>
	  /// <param name="processVariables"> a map of process variables
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no execution is found for the given executionId or id is null. </exception>
	  /// <exception cref="SuspendedEntityInteractionException">
	  ///          when the execution is suspended. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void signal(string executionId, IDictionary<string, object> processVariables);

	  // Variables ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// All variables visible from the given execution scope (including parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  IDictionary<string, object> getVariables(string executionId);

	  /// <summary>
	  /// All variables visible from the given execution scope (including parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  VariableMap getVariablesTyped(string executionId);

	  /// <summary>
	  /// All variables visible from the given execution scope (including parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="deserializeValues"> if false, <seealso cref="SerializableValue"/>s will not be deserialized
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  VariableMap getVariablesTyped(string executionId, bool deserializeValues);

	  /// <summary>
	  /// All variable values that are defined in the execution scope, without taking outer scopes into account.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="#getVariablesLocal(String, Collection)"/>
	  /// for better performance. </summary>
	  /// <param name="executionId"> id of execution, cannot be null.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  IDictionary<string, object> getVariablesLocal(string executionId);

	  /// <summary>
	  /// All variable values that are defined in the execution scope, without taking outer scopes into account.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="#getVariablesLocal(String, Collection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="executionId"> id of execution, cannot be null.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  VariableMap getVariablesLocalTyped(string executionId);

	  /// <summary>
	  /// All variable values that are defined in the execution scope, without taking outer scopes into account.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="#getVariablesLocal(String, Collection)"/>
	  /// for better performance.
	  /// </summary>
	  /// <param name="executionId"> id of execution, cannot be null. </param>
	  /// <param name="deserializeObjectValues"> if false, <seealso cref="SerializableValue"/>s will not be deserialized
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2 </exception>
	  VariableMap getVariablesLocalTyped(string executionId, bool deserializeValues);

	  /// <summary>
	  /// The variable values for all given variableNames, takes all variables into account which are visible from the given execution scope (including parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableNames"> the collection of variable names that should be retrieved.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  IDictionary<string, object> getVariables(string executionId, ICollection<string> variableNames);

	  /// <summary>
	  /// The variable values for all given variableNames, takes all variables into account which are visible from the given execution scope (including parent scopes). </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableNames"> the collection of variable names that should be retrieved. </param>
	  /// <param name="deserializeObjectValues"> if false, <seealso cref="SerializableValue"/>s will not be deserialized
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  VariableMap getVariablesTyped(string executionId, ICollection<string> variableNames, bool deserializeValues);

	  /// <summary>
	  /// The variable values for the given variableNames only taking the given execution scope into account, not looking in outer scopes.
	  /// </summary>
	  /// <param name="executionId"> id of execution, cannot be null. </param>
	  /// <param name="variableNames"> the collection of variable names that should be retrieved.
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  IDictionary<string, object> getVariablesLocal(string executionId, ICollection<string> variableNames);

	  /// <summary>
	  /// The variable values for the given variableNames only taking the given execution scope into account, not looking in outer scopes. </summary>
	  /// <param name="executionId"> id of execution, cannot be null. </param>
	  /// <param name="variableNames"> the collection of variable names that should be retrieved. </param>
	  /// <param name="deserializeObjectValues"> if false, <seealso cref="SerializableValue"/>s will not be deserialized
	  /// </param>
	  /// <returns> the variables or an empty map if no such variables are found.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  VariableMap getVariablesLocalTyped(string executionId, ICollection<string> variableNames, bool deserializeValues);

	  /// <summary>
	  /// The variable value.  Searching for the variable is done in all scopes that are visible to the given execution (including parent scopes).
	  /// Returns null when no variable value is found with the given name or when the value is set to null.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null.
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined or the value of the variable is null.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  object getVariable(string executionId, string variableName);

	  /// <summary>
	  /// Returns a <seealso cref="TypedValue"/> for the variable. Searching for the variable is done in all scopes that are visible
	  /// to the given execution (including parent scopes). Returns null when no variable value is found with the given name.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null.
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  T getVariableTyped<T>(string executionId, string variableName);

	  /// <summary>
	  /// Returns a <seealso cref="TypedValue"/> for the variable. Searching for the variable is done in all scopes that are visible
	  /// to the given execution (including parent scopes). Returns null when no variable value is found with the given name.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null. </param>
	  /// <param name="deserializeValue"> if false, a <seealso cref="SerializableValue"/> will not be deserialized
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  T getVariableTyped<T>(string executionId, string variableName, bool deserializeValue);

	  /// <summary>
	  /// The variable value for an execution. Returns the value when the variable is set
	  /// for the execution (and not searching parent scopes). Returns null when no variable value is found with the given name or when the value is set to null.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null.
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined or the value of the variable is null.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  object getVariableLocal(string executionId, string variableName);

	  /// <summary>
	  /// Returns a <seealso cref="TypedValue"/> for the variable. Returns the value when the variable is set
	  /// for the execution (and not searching parent scopes). Returns null when no variable value is found with the given name.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null.
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  T getVariableLocalTyped<T>(string executionId, string variableName);

	  /// <summary>
	  /// Returns a <seealso cref="TypedValue"/> for the variable. Searching for the variable is done in all scopes that are visible
	  /// to the given execution (and not searching parent scopes). Returns null when no variable value is found with the given name.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution, cannot be null. </param>
	  /// <param name="variableName"> name of variable, cannot be null. </param>
	  /// <param name="deserializeValue"> if false, a <seealso cref="SerializableValue"/> will not be deserialized
	  /// </param>
	  /// <returns> the variable value or null if the variable is undefined.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or
	  ///          no <seealso cref="Permissions#READ_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li> In case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission this"/> config is enabled and
	  ///          the user has no <seealso cref="ProcessDefinitionPermisions#READ_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  /// 
	  /// @since 7.2
	  ///  </exception>
	  T getVariableLocalTyped<T>(string executionId, string variableName, bool deserializeValue);

	  /// <summary>
	  /// Update or create a variable for an execution.  If the variable does not already exist
	  /// somewhere in the execution hierarchy (i.e. the specified execution or any ancestor),
	  /// it will be created in the process instance (which is the root execution).
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to set variable in, cannot be null. </param>
	  /// <param name="variableName"> name of variable to set, cannot be null. </param>
	  /// <param name="value"> value to set. When null is passed, the variable is not removed,
	  /// only it's value will be set to null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void setVariable(string executionId, string variableName, object value);

	  /// <summary>
	  /// Update or create a variable for an execution (not considering parent scopes).
	  /// If the variable does not already exist, it will be created in the given execution.
	  /// </summary>
	  /// <param name="executionId"> id of execution to set variable in, cannot be null. </param>
	  /// <param name="variableName"> name of variable to set, cannot be null. </param>
	  /// <param name="value"> value to set. When null is passed, the variable is not removed,
	  /// only it's value will be set to null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void setVariableLocal(string executionId, string variableName, object value);

	  /// <summary>
	  /// Update or create given variables for an execution (including parent scopes). If the variables are not already existing, they will be created in the process instance
	  /// (which is the root execution).
	  /// </summary>
	  /// <param name="executionId"> id of the process instance or the execution, cannot be null. </param>
	  /// <param name="variables"> map containing name (key) and value of variables, can be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void setVariables<T1>(string executionId, IDictionary<T1> variables);

	  /// <summary>
	  /// Update or create given variables for an execution (not considering parent scopes). If the variables are not already existing, it will be created in the given execution.
	  /// </summary>
	  /// <param name="executionId"> id of the execution, cannot be null. </param>
	  /// <param name="variables"> map containing name (key) and value of variables, can be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void setVariablesLocal<T1>(string executionId, IDictionary<T1> variables);

	  /// <summary>
	  /// Removes a variable for an execution.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to remove variable in. </param>
	  /// <param name="variableName"> name of variable to remove.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void removeVariable(string executionId, string variableName);

	  /// <summary>
	  /// Removes a variable for an execution (not considering parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of execution to remove variable in. </param>
	  /// <param name="variableName"> name of variable to remove.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void removeVariableLocal(string executionId, string variableName);

	  /// <summary>
	  /// Removes variables for an execution.
	  /// </summary>
	  /// <param name="executionId"> id of process instance or execution to remove variable in. </param>
	  /// <param name="variableNames"> collection containing name of variables to remove.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void removeVariables(string executionId, ICollection<string> variableNames);

	  /// <summary>
	  /// Remove variables for an execution (not considering parent scopes).
	  /// </summary>
	  /// <param name="executionId"> id of execution to remove variable in. </param>
	  /// <param name="variableNames"> collection containing name of variables to remove.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no execution is found for the given executionId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#UPDATE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#UPDATE_INSTANCE_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void removeVariablesLocal(string executionId, ICollection<string> variableNames);

	  // Queries ////////////////////////////////////////////////////////

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutionQuery"/> instance,
	  /// that can be used to query the executions and process instances. 
	  /// </summary>
	  ExecutionQuery createExecutionQuery();

	  /// <summary>
	  /// creates a new <seealso cref="NativeExecutionQuery"/> to query <seealso cref="Execution"/>s
	  /// by SQL directly
	  /// </summary>
	  NativeExecutionQuery createNativeExecutionQuery();

	  /// <summary>
	  /// Creates a new <seealso cref="ProcessInstanceQuery"/> instance, that can be used
	  /// to query process instances.
	  /// </summary>
	  ProcessInstanceQuery createProcessInstanceQuery();

	  /// <summary>
	  /// creates a new <seealso cref="NativeProcessInstanceQuery"/> to query <seealso cref="ProcessInstance"/>s
	  /// by SQL directly
	  /// </summary>
	  NativeProcessInstanceQuery createNativeProcessInstanceQuery();

	  /// <summary>
	  /// Creates a new <seealso cref="IncidentQuery"/> instance, that can be used
	  /// to query incidents.
	  /// </summary>
	  IncidentQuery createIncidentQuery();

	  /// <summary>
	  /// Creates a new <seealso cref="EventSubscriptionQuery"/> instance, that can be used to query
	  /// event subscriptions.
	  /// </summary>
	  EventSubscriptionQuery createEventSubscriptionQuery();

	  /// <summary>
	  /// Creates a new <seealso cref="VariableInstanceQuery"/> instance, that can be used to query
	  /// variable instances.
	  /// </summary>
	  VariableInstanceQuery createVariableInstanceQuery();

	  // Process instance state //////////////////////////////////////////

	  /// <summary>
	  /// <para>Suspends the process instance with the given id. This means that the
	  /// execution is stopped, so the <i>token state</i> will not change.
	  /// However, actions that do not change token state, like setting/removing
	  /// variables, etc. will succeed.</para>
	  /// 
	  /// <para>Tasks belonging to this process instance will also be suspended. This means
	  /// that any actions influencing the tasks' lifecycles will fail, such as
	  /// <ul>
	  ///   <li>claiming</li>
	  ///   <li>completing</li>
	  ///   <li>delegation</li>
	  ///   <li>changes in task assignees, owners, etc.</li>
	  /// </ul>
	  /// Actions that only change task properties will succeed, such as changing variables
	  /// or adding comments.
	  /// </para>
	  /// 
	  /// <para>If a process instance is in state suspended, the engine will also not
	  /// execute jobs (timers, messages) associated with this instance.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, suspending
	  /// one process instance from the hierarchy will not suspend other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such processInstance can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void suspendProcessInstanceById(string processInstanceId);

	  /// <summary>
	  /// <para>Suspends the process instances with the given process definition id.
	  /// This means that the execution is stopped, so the <i>token state</i>
	  /// will not change. However, actions that do not change token state, like
	  /// setting/removing variables, etc. will succeed.</para>
	  /// 
	  /// <para>Tasks belonging to the suspended process instance will also be suspended.
	  /// This means that any actions influencing the tasks' lifecycles will fail, such as
	  /// <ul>
	  ///   <li>claiming</li>
	  ///   <li>completing</li>
	  ///   <li>delegation</li>
	  ///   <li>changes in task assignees, owners, etc.</li>
	  /// </ul>
	  /// Actions that only change task properties will succeed, such as changing variables
	  /// or adding comments.
	  /// </para>
	  /// 
	  /// <para>If a process instance is in state suspended, the engine will also not
	  /// execute jobs (timers, messages) associated with this instance.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, suspending
	  /// one process instance from the hierarchy will not suspend other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such processInstance can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void suspendProcessInstanceByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Suspends the process instances with the given process definition key.
	  /// This means that the execution is stopped, so the <i>token state</i>
	  /// will not change. However, actions that do not change token state, like
	  /// setting/removing variables, etc. will succeed.</para>
	  /// 
	  /// <para>Tasks belonging to the suspended process instance will also be suspended.
	  /// This means that any actions influencing the tasks' lifecycles will fail, such as
	  /// <ul>
	  ///   <li>claiming</li>
	  ///   <li>completing</li>
	  ///   <li>delegation</li>
	  ///   <li>changes in task assignees, owners, etc.</li>
	  /// </ul>
	  /// Actions that only change task properties will succeed, such as changing variables
	  /// or adding comments.
	  /// </para>
	  /// 
	  /// <para>If a process instance is in state suspended, the engine will also not
	  /// execute jobs (timers, messages) associated with this instance.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, suspending
	  /// one process instance from the hierarchy will not suspend other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such processInstance can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void suspendProcessInstanceByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// <para>Activates the process instance with the given id.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, activating
	  /// one process instance from the hierarchy will not activate other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such processInstance can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void activateProcessInstanceById(string processInstanceId);

	  /// <summary>
	  /// <para>Activates the process instance with the given process definition id.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, activating
	  /// one process instance from the hierarchy will not activate other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if the process definition id is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void activateProcessInstanceByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Activates the process instance with the given process definition key.</para>
	  /// 
	  /// <para>If you have a process instance hierarchy, activating
	  /// one process instance from the hierarchy will not activate other
	  /// process instances from that hierarchy.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessInstanceSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          if the process definition id is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has none of the following:
	  ///          <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void activateProcessInstanceByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Activate or suspend process instances using a fluent builder. Specify the
	  /// instances by calling one of the <i>by</i> methods, like
	  /// <i>byProcessInstanceId</i>. To update the suspension state call
	  /// <seealso cref="UpdateProcessInstanceSuspensionStateBuilder#activate()"/> or
	  /// <seealso cref="UpdateProcessInstanceSuspensionStateBuilder#suspend()"/>.
	  /// </summary>
	  /// <returns> the builder to update the suspension state </returns>
	  UpdateProcessInstanceSuspensionStateSelectBuilder updateProcessInstanceSuspensionState();

	  // Events ////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Notifies the process engine that a signal event of name 'signalName' has
	  /// been received. Delivers the signal to all executions waiting on
	  /// the signal and to all process definitions that can started by this signal. <p/>
	  /// 
	  /// <strong>NOTE:</strong> Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="signalName">
	  ///          the name of the signal event
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if notify an execution and the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///          <li>if start a new process instance and the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li> </exception>
	  void signalEventReceived(string signalName);

	  /// <summary>
	  /// Notifies the process engine that a signal event of name 'signalName' has
	  /// been received. Delivers the signal to all executions waiting on
	  /// the signal and to all process definitions that can started by this signal. <p/>
	  /// 
	  /// <strong>NOTE:</strong> Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="signalName">
	  ///          the name of the signal event </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution(s)
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          <li>if notify an execution and the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///          <li>if start a new process instance and the user has no <seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li> </exception>
	  void signalEventReceived(string signalName, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Notifies the process engine that a signal event of name 'signalName' has
	  /// been received. This method delivers the signal to a single execution, being the
	  /// execution referenced by 'executionId'.
	  /// The waiting execution is notified synchronously.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the signal
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="signalName">
	  ///          the name of the signal event </param>
	  /// <param name="executionId">
	  ///          id of the process instance or the execution to deliver the signal to
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such execution exists or if the execution
	  ///          has not subscribed to the signal </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void signalEventReceived(string signalName, string executionId);

	  /// <summary>
	  /// Notifies the process engine that a signal event of name 'signalName' has
	  /// been received. This method delivers the signal to a single execution, being the
	  /// execution referenced by 'executionId'.
	  /// The waiting execution is notified synchronously.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the signal
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="signalName">
	  ///          the name of the signal event </param>
	  /// <param name="executionId">
	  ///          the id of the process instance or the execution to deliver the signal to </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution(s)
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such execution exists or if the execution
	  ///          has not subscribed to the signal </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void signalEventReceived(string signalName, string executionId, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Notifies the process engine that a signal event has been received using a
	  /// fluent builder.
	  /// </summary>
	  /// <param name="signalName">
	  ///          the name of the signal event </param>
	  /// <returns> the fluent builder to send the signal </returns>
	  SignalEventReceivedBuilder createSignalEvent(string signalName);

	  /// <summary>
	  /// Notifies the process engine that a message event with name 'messageName' has
	  /// been received and has been correlated to an execution with id 'executionId'.
	  /// 
	  /// The waiting execution is notified synchronously.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the message
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event </param>
	  /// <param name="executionId">
	  ///          the id of the process instance or the execution to deliver the message to
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such execution exists or if the execution
	  ///          has not subscribed to the signal </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void messageEventReceived(string messageName, string executionId);

	  /// <summary>
	  /// Notifies the process engine that a message event with the name 'messageName' has
	  /// been received and has been correlated to an execution with id 'executionId'.
	  /// 
	  /// The waiting execution is notified synchronously.
	  /// 
	  /// Note that you need to provide the exact execution that is waiting for the message
	  /// if the process instance contains multiple executions.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event </param>
	  /// <param name="executionId">
	  ///          the id of the process instance or the execution to deliver the message to </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          if no such execution exists or if the execution
	  ///          has not subscribed to the signal </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void messageEventReceived(string messageName, string executionId, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Define a complex message correlation using a fluent builder.
	  /// </summary>
	  /// <param name="messageName"> the name of the message. Corresponds to the 'name' element
	  /// of the message defined in BPMN 2.0 Xml.
	  /// Can be null to correlate by other criteria (businessKey, processInstanceId, correlationKeys) only.
	  /// </param>
	  /// <returns> the fluent builder for defining the message correlation. </returns>
	  MessageCorrelationBuilder createMessageCorrelation(string messageName);

	  /// <summary>
	  /// Correlates a message to either an execution that is waiting for this message or a process definition
	  /// that can be started by this message.
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName);

	  /// <summary>
	  /// Correlates a message to
	  /// <ul>
	  ///  <li>
	  ///    an execution that is waiting for a matching message and belongs to a process instance with the given business key
	  ///  </li>
	  ///  <li>
	  ///    a process definition that can be started by a matching message.
	  ///  </li>
	  /// </ul>
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event </param>
	  /// <param name="businessKey">
	  ///          the business key of process instances to correlate against
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null and businessKey is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName, string businessKey);

	  /// <summary>
	  /// Correlates a message to
	  /// <ul>
	  ///  <li>
	  ///    an execution that is waiting for a matching message and can be correlated according
	  ///    to the given correlation keys. This is typically matched against process instance variables.
	  ///  </li>
	  ///  <li>
	  ///    a process definition that can be started by message with the provided name.
	  ///  </li>
	  /// </ul>
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event </param>
	  /// <param name="correlationKeys">
	  ///          a map of key value pairs that are used to correlate the message to an execution
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null and correlationKeys is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName, IDictionary<string, object> correlationKeys);

	  /// <summary>
	  /// Correlates a message to
	  /// <ul>
	  ///  <li>
	  ///    an execution that is waiting for a matching message and belongs to a process instance with the given business key
	  ///  </li>
	  ///  <li>
	  ///    a process definition that can be started by this message.
	  ///  </li>
	  /// </ul>
	  /// and updates the process instance variables.
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event </param>
	  /// <param name="businessKey">
	  ///          the business key of process instances to correlate against </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution or newly created process instance
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null and businessKey is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName, string businessKey, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Correlates a message to
	  /// <ul>
	  ///  <li>
	  ///    an execution that is waiting for a matching message and can be correlated according
	  ///    to the given correlation keys. This is typically matched against process instance variables.
	  ///  </li>
	  ///  <li>
	  ///    a process definition that can be started by this message.
	  ///  </li>
	  /// </ul>
	  /// and updates the process instance variables.
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event </param>
	  /// <param name="correlationKeys">
	  ///          a map of key value pairs that are used to correlate the message to an execution </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution or newly created process instance
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null and correlationKeys is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName, IDictionary<string, object> correlationKeys, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Correlates a message to
	  /// <ul>
	  ///  <li>
	  ///    an execution that is waiting for a matching message and can be correlated according
	  ///    to the given correlation keys. This is typically matched against process instance variables.
	  ///    The process instance it belongs to has to have the given business key.
	  ///  </li>
	  ///  <li>
	  ///    a process definition that can be started by this message.
	  ///  </li>
	  /// </ul>
	  /// and updates the process instance variables.
	  /// 
	  /// Notification and instantiation happen synchronously.
	  /// </summary>
	  /// <param name="messageName">
	  ///          the name of the message event; if null, matches any event </param>
	  /// <param name="businessKey">
	  ///          the business key of process instances to correlate against </param>
	  /// <param name="correlationKeys">
	  ///          a map of key value pairs that are used to correlate the message to an execution </param>
	  /// <param name="processVariables">
	  ///          a map of variables added to the execution or newly created process instance
	  /// </param>
	  /// <exception cref="MismatchingMessageCorrelationException">
	  ///          if none or more than one execution or process definition is correlated </exception>
	  /// <exception cref="ProcessEngineException">
	  ///          if messageName is null and businessKey is null and correlationKeys is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void correlateMessage(string messageName, string businessKey, IDictionary<string, object> correlationKeys, IDictionary<string, object> processVariables);

	  /// <summary>
	  /// Define a modification of a process instance in terms of activity cancellations
	  /// and instantiations via a fluent builder. Instructions are executed in the order they are specified.
	  /// </summary>
	  /// <param name="processInstanceId"> the process instance to modify </param>
	  ProcessInstanceModificationBuilder createProcessInstanceModification(string processInstanceId);

	  /// <summary>
	  /// Returns a fluent builder to start a new process instance in the exactly
	  /// specified version of the process definition with the given id. The builder
	  /// can be used to set further properties and specify instantiation
	  /// instructions to start the instance at any set of activities in the process.
	  /// If no instantiation instructions are set then the instance start at the
	  /// default start activity.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          the id of the process definition, cannot be <code>null</code>.
	  /// </param>
	  /// <returns> a builder to create a process instance of the definition </returns>
	  ProcessInstantiationBuilder createProcessInstanceById(string processDefinitionId);

	  /// <summary>
	  /// Returns a fluent builder to start a new process instance in the latest
	  /// version of the process definition with the given key. The builder can be
	  /// used to set further properties and specify instantiation instructions to
	  /// start the instance at any set of activities in the process. If no
	  /// instantiation instructions are set then the instance start at the default
	  /// start activity.
	  /// </summary>
	  /// <param name="processDefinitionKey">
	  ///          the key of the process definition, cannot be <code>null</code>.
	  /// </param>
	  /// <returns> a builder to create a process instance of the definition </returns>
	  ProcessInstantiationBuilder createProcessInstanceByKey(string processDefinitionKey);

	  /// <summary>
	  /// Creates a migration plan to migrate process instance between different process definitions.
	  /// Returns a fluent builder that can be used to specify migration instructions and build the plan.
	  /// </summary>
	  /// <param name="sourceProcessDefinitionId"> the process definition that instances are migrated from </param>
	  /// <param name="targetProcessDefinitionId"> the process definition that instances are migrated to </param>
	  /// <returns> a fluent builder </returns>
	  MigrationPlanBuilder createMigrationPlan(string sourceProcessDefinitionId, string targetProcessDefinitionId);

	  /// <summary>
	  /// Executes a migration plan for a given list of process instances. The migration can
	  /// either be executed synchronously or asynchronously. A synchronously migration
	  /// blocks the caller until the migration was completed. The migration can only be
	  /// successfully completed if all process instances can be migrated.
	  /// 
	  /// If the migration is executed asynchronously a <seealso cref="Batch"/> is immediately returned.
	  /// The migration is then executed as jobs from the process engine and the batch can
	  /// be used to track the progress of the migration. The Batch splits the migration
	  /// in smaller chunks which will be executed independently.
	  /// </summary>
	  /// <param name="migrationPlan"> the migration plan to executed </param>
	  /// <returns> a fluent builder </returns>
	  MigrationPlanExecutionBuilder newMigration(MigrationPlan migrationPlan);

	  /// <summary>
	  /// Creates a modification of multiple process instances in terms of activity cancellations
	  /// and instantiations via a fluent builder. Returns a fluent builder that can be used to specify
	  /// modification instructions and set process instances that should be modified.
	  /// 
	  /// The modification can
	  /// either be executed synchronously or asynchronously. A synchronously modification
	  /// blocks the caller until the modification was completed. The modification can only be
	  /// successfully completed if all process instances can be modified.
	  /// 
	  /// If the modification is executed asynchronously a <seealso cref="Batch"/> is immediately returned.
	  /// The modification is then executed as jobs from the process engine and the batch can
	  /// be used to track the progress of the modification. The Batch splits the modification
	  /// in smaller chunks which will be executed independently.
	  /// </summary>
	  /// <param name="processDefinitionId"> the process definition that instances are modified of </param>
	  /// <returns> a fluent builder </returns>

	  ModificationBuilder createModification(string processDefinitionId);

	  /// <summary>
	  /// Restarts process instances that are completed or deleted with the initial or last set of variables.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          when no process definition is deployed with the given key or a process instance is still active. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has not all of the following permissions
	  ///     <ul>
	  ///       <li><seealso cref="Permissions#CREATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///       <li><seealso cref="Permissions#CREATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///       <li><seealso cref="Permissions#READ_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///     </ul> </exception>
	  RestartProcessInstanceBuilder restartProcessInstances(string processDefinitionId);

	  /// <summary>
	  /// Creates an incident
	  /// </summary>
	  /// <param name="incidentType"> the type of incident, cannot be null </param>
	  /// <param name="executionId"> execution id, cannot be null </param>
	  /// <param name="configuration">
	  /// </param>
	  /// <returns> a new incident
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  Incident createIncident(string incidentType, string executionId, string configuration);

	  /// <summary>
	  /// Creates an incident
	  /// </summary>
	  /// <param name="incidentType"> the type of incident, cannot be null </param>
	  /// <param name="executionId"> execution id, cannot be null </param>
	  /// <param name="configuration"> </param>
	  /// <param name="message">
	  /// </param>
	  /// <returns> a new incident
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  Incident createIncident(string incidentType, string executionId, string configuration, string message);

	  /// <summary>
	  /// Resolves and remove an incident
	  /// </summary>
	  /// <param name="incidentId"> the id of an incident to resolve
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          if the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void resolveIncident(string incidentId);

	  /// <summary>
	  /// Define a complex condition evaluation using a fluent builder.
	  /// </summary>
	  /// <returns> the fluent builder for defining the condition evaluation. </returns>
	  ConditionEvaluationBuilder createConditionEvaluation();

	}

}