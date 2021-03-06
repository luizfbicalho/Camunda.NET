﻿using System;
using System.Collections.Generic;
using System.IO;

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

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;


	/// <summary>
	/// Access to form data and rendered forms for starting new process instances and completing tasks.
	/// 
	/// @author Tom Baeyens
	/// @author Falko Menge (camunda)
	/// </summary>
	public interface FormService
	{

	  /// <summary>
	  /// Retrieves all data necessary for rendering a form to start a new process instance.
	  /// This can be used to perform rendering of the forms outside of the process engine.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  StartFormData getStartFormData(string processDefinitionId);

	  /// <summary>
	  /// Rendered form generated by the default build-in form engine for starting a new process instance.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  object getRenderedStartForm(string processDefinitionId);

	  /// <summary>
	  /// Rendered form generated by the given build-in form engine for starting a new process instance.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  object getRenderedStartForm(string processDefinitionId, string formEngineName);

	  /// @deprecated use <seealso cref="submitStartForm(string, System.Collections.IDictionary)"/>
	  ///  
	  [Obsolete("use <seealso cref=\"submitStartForm(string, System.Collections.IDictionary)\"/>")]
	  ProcessInstance submitStartFormData(string processDefinitionId, IDictionary<string, string> properties);

	  /// <summary>
	  /// Start a new process instance with the user data that was entered as properties in a start form.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.CREATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance submitStartForm(string processDefinitionId, IDictionary<string, object> properties);

	  /// @deprecated use <seealso cref="submitStartForm(string, string, System.Collections.IDictionary)"/> 
	  [Obsolete("use <seealso cref=\"submitStartForm(string, string, System.Collections.IDictionary)\"/>")]
	  ProcessInstance submitStartFormData(string processDefinitionId, string businessKey, IDictionary<string, string> properties);

	  /// <summary>
	  /// Start a new process instance with the user data that was entered as properties in a start form.
	  /// 
	  /// A business key can be provided to associate the process instance with a
	  /// certain identifier that has a clear business meaning. For example in an
	  /// order process, the business key could be an order id. This business key can
	  /// then be used to easily look up that process instance , see
	  /// <seealso cref="ProcessInstanceQuery.processInstanceBusinessKey(string)"/>. Providing such a business
	  /// key is definitely a best practice.
	  /// 
	  /// Note that a business key MUST be unique for the given process definition.
	  /// Process instance from different process definition are allowed to have the
	  /// same business key.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition, cannot be null. </param>
	  /// <param name="businessKey"> a key that uniquely identifies the process instance in the context or the
	  ///                    given process definition. </param>
	  /// <param name="properties"> the properties to pass, can be null.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.CREATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  ProcessInstance submitStartForm(string processDefinitionId, string businessKey, IDictionary<string, object> properties);

	  /// <summary>
	  /// Retrieves all data necessary for rendering a form to complete a task.
	  /// This can be used to perform rendering of the forms outside of the process engine.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/></li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>if the user has <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> or
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li>
	  ///          </ul></para> </exception>
	  TaskFormData getTaskFormData(string taskId);

	  /// <summary>
	  /// Rendered form generated by the default build-in form engine for completing a task.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>if the user has <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> or
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para> </exception>
	  object getRenderedTaskForm(string taskId);

	  /// <summary>
	  /// Rendered form generated by the given build-in form engine for completing a task.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>if the user has <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> or
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para> </exception>
	  object getRenderedTaskForm(string taskId, string formEngineName);

	  /// @deprecated use <seealso cref="submitTaskForm(string, System.Collections.IDictionary)"/>  
	  [Obsolete("use <seealso cref=\"submitTaskForm(string, System.Collections.IDictionary)\"/>")]
	  void submitTaskFormData(string taskId, IDictionary<string, string> properties);

	  /// <summary>
	  /// Completes a task with the user data that was entered as properties in a task form.
	  /// </summary>
	  /// <param name="taskId"> </param>
	  /// <param name="properties">
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void submitTaskForm(string taskId, IDictionary<string, object> properties);

	  /// <summary>
	  /// Completes a task with the user data that was entered as properties in a task form.
	  /// </summary>
	  /// <param name="taskId"> </param>
	  /// <param name="properties"> </param>
	  /// <param name="deserializeValues"> if false, returned <seealso cref="SerializableValue"/>s
	  ///   will not be deserialized (unless they are passed into this method as a
	  ///   deserialized value or if the BPMN process triggers deserialization) </param>
	  /// <returns> a map of process variables
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          or no <seealso cref="Permissions.UPDATE_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  VariableMap submitTaskFormWithVariablesInReturn(string taskId, IDictionary<string, object> properties, bool deserializeValues);

	  /// <summary>
	  /// Retrieves a list of all variables for rendering a start from. The method takes into account
	  /// FormData specified for the start event. This allows defining default values for form fields.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition for which the start form should be retrieved. </param>
	  /// <returns> a map of VariableInstances.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  VariableMap getStartFormVariables(string processDefinitionId);

	  /// <summary>
	  /// Retrieves a list of requested variables for rendering a start from. The method takes into account
	  /// FormData specified for the start event. This allows defining default values for form fields.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the process definition for which the start form should be retrieved. </param>
	  /// <param name="formVariables"> a Collection of the names of the variables to retrieve. Allows restricting the set of retrieved variables. </param>
	  /// <param name="deserializeObjectValues"> if false object values are not deserialized </param>
	  /// <returns> a map of VariableInstances.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  VariableMap getStartFormVariables(string processDefinitionId, ICollection<string> formVariables, bool deserializeObjectValues);

	  /// <summary>
	  /// <para>Retrieves a list of all variables for rendering a task form. In addition to the task variables and process variables,
	  /// the method takes into account FormData specified for the task. This allows defining default values for form fields.</para>
	  /// 
	  /// <para>A variable is resolved in the following order:
	  /// <ul>
	  ///   <li>First, the method collects all form fields and creates variable instances for the form fields.</li>
	  ///   <li>Next, the task variables are collected.</li>
	  ///   <li>Next, process variables from the parent scopes of the task are collected, until the process instance scope is reached.</li>
	  /// </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="taskId"> the id of the task for which the variables should be retrieved. </param>
	  /// <returns> a map of VariableInstances.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>if the user has <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> or
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para> </exception>
	  VariableMap getTaskFormVariables(string taskId);

	  /// <summary>
	  /// <para>Retrieves a list of requested variables for rendering a task form. In addition to the task variables and process variables,
	  /// the method takes into account FormData specified for the task. This allows defining default values for form fields.</para>
	  /// 
	  /// <para>A variable is resolved in the following order:
	  /// <ul>
	  ///   <li>First, the method collects all form fields and creates variable instances for the form fields.</li>
	  ///   <li>Next, the task variables are collected.</li>
	  ///   <li>Next, process variables from the parent scopes of the task are collected, until the process instance scope is reached.</li>
	  /// </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="taskId"> the id of the task for which the variables should be retrieved. </param>
	  /// <param name="formVariables"> a Collection of the names of the variables to retrieve. Allows restricting the set of retrieved variables. </param>
	  /// <param name="deserializeObjectValues"> if false object values are not deserialized </param>
	  /// <returns> a map of VariableInstances.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          <para>In case of standalone tasks:
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or</li>
	  ///          <li>if the user has no <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para>
	  ///          <para>In case the task is part of a running process instance:</li>
	  ///          <li>if the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/> or
	  ///           no <seealso cref="Permissions.READ_TASK"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/> </li>
	  ///          <li>if the user has <seealso cref="TaskPermissions.READ_VARIABLE"/> permission on <seealso cref="Resources.TASK"/> or
	  ///          no <seealso cref="ProcessDefinitionPermissions.READ_TASK_VARIABLE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          when <seealso cref="ProcessEngineConfiguration.enforceSpecificVariablePermission this"/> config is enabled</li></para> </exception>
	  VariableMap getTaskFormVariables(string taskId, ICollection<string> formVariables, bool deserializeObjectValues);

	  /// <summary>
	  /// Retrieves a user defined reference to a start form.
	  /// 
	  /// In the Explorer app, it is assumed that the form key specifies a resource
	  /// in the deployment, which is the template for the form.  But users are free
	  /// to use this property differently.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  string getStartFormKey(string processDefinitionId);

	  /// <summary>
	  /// Retrieves a user defined reference to a task form.
	  /// 
	  /// In the Explorer app, it is assumed that the form key specifies a resource
	  /// in the deployment, which is the template for the form.  But users are free
	  /// to use this property differently.
	  /// 
	  /// Both arguments can be obtained from <seealso cref="Task"/> instances returned by any
	  /// <seealso cref="TaskQuery"/>.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  string getTaskFormKey(string processDefinitionId, string taskDefinitionKey);

	  /// <summary>
	  /// Retrieves a deployed start form for a process definition with a given id.
	  /// 
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  /// <exception cref="NotFoundException">
	  ///          If the start form cannot be found. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///          If the start form key has wrong format ("embedded:deployment:<path>" or "deployment:<path>" required). </exception>
	  Stream getDeployedStartForm(string processDefinitionId);

	  /// <summary>
	  /// Retrieves a deployed task form for a task with a given id.
	  /// 
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.TASK"/>. </exception>
	  /// <exception cref="NotFoundException">
	  ///          If the task form cannot be found. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///          If the task form key has wrong format ("embedded:deployment:<path>" or "deployment:<path>" required). </exception>
	  Stream getDeployedTaskForm(string taskId);

	}
}