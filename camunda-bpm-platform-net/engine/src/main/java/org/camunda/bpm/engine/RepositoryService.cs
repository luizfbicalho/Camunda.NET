using System;
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

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using DeleteProcessDefinitionsSelectBuilder = org.camunda.bpm.engine.repository.DeleteProcessDefinitionsSelectBuilder;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessApplicationDeploymentBuilder = org.camunda.bpm.engine.repository.ProcessApplicationDeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Resource = org.camunda.bpm.engine.repository.Resource;
	using UpdateProcessDefinitionSuspensionStateBuilder = org.camunda.bpm.engine.repository.UpdateProcessDefinitionSuspensionStateBuilder;
	using UpdateProcessDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.repository.UpdateProcessDefinitionSuspensionStateSelectBuilder;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;


	/// <summary>
	/// Service providing access to the repository of process definitions and deployments.
	/// 
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	public interface RepositoryService
	{

	  /// <summary>
	  /// Starts creating a new deployment
	  /// </summary>
	  DeploymentBuilder createDeployment();

	  /// <summary>
	  /// Starts creating a new <seealso cref="ProcessApplicationDeployment"/>.
	  /// </summary>
	  /// <seealso cref= ProcessApplicationDeploymentBuilder </seealso>
	  ProcessApplicationDeploymentBuilder createDeployment(ProcessApplicationReference processApplication);

	  /// <summary>
	  /// Deletes the given deployment.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// </param>
	  /// <exception cref="RuntimeException">
	  ///          If there are still runtime or history process instances or jobs. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  void deleteDeployment(string deploymentId);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances,
	  /// history process instances and jobs.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>.
	  /// </exception>
	  /// @deprecated use <seealso cref="#deleteDeployment(String, boolean)"/>. This methods may be deleted from 5.3. 
	  [Obsolete("use <seealso cref="#deleteDeployment(String, boolean)"/>. This methods may be deleted from 5.3.")]
	  void deleteDeploymentCascade(string deploymentId);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances,
	  /// history process instances and jobs.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  void deleteDeployment(string deploymentId, bool cascade);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances,
	  /// history process instances and jobs.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  void deleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances,
	  /// history process instances and jobs.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
	  /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  void deleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings);


	  /// <summary>
	  /// Deletes the process definition which belongs to the given process definition id.
	  /// Same behavior as <seealso cref="RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean)"/>
	  /// Both boolean parameters of this method are per default false. The deletion is
	  /// in this case not cascading.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition does not exist
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  /// <seealso cref= RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean) </seealso>
	  void deleteProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Deletes the process definition which belongs to the given process definition id.
	  /// Cascades the deletion if the cascade is set to true.
	  /// Same behavior as <seealso cref="RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean)"/>
	  /// The skipCustomListeners parameter is per default false. The custom listeners are called
	  /// if the cascading flag is set to true and the process instances are deleted.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
	  /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition does not exist
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  /// <seealso cref= RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean) </seealso>
	  void deleteProcessDefinition(string processDefinitionId, bool cascade);


	  /// <summary>
	  /// Deletes the process definition which belongs to the given process definition id.
	  /// Cascades the deletion if the cascade is set to true the custom listener
	  /// can be skipped if the third parameter is set to true.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
	  /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  ///            are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
	  ///            Is only used if cascade set to true.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition does not exist
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners);


	  /// <summary>
	  /// Deletes the process definition which belongs to the given process definition id.
	  /// Cascades the deletion if the cascade is set to true, the custom listener can be skipped if
	  /// the third parameter is set to true, io mappings can be skipped if the forth parameter is set to true.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
	  /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
	  /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
	  ///            are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
	  ///            Is only used if cascade set to true. </param>
	  /// <param name="skipIoMappings"> Specifies whether input/output mappings for tasks should be invoked
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition does not exist
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners, bool skipIoMappings);

	  /// <summary>
	  /// Fluent builder to delete process definitions.
	  /// </summary>
	  /// <returns> the builder to delete process definitions </returns>
	  DeleteProcessDefinitionsSelectBuilder deleteProcessDefinitions();

	  /// <summary>
	  /// Retrieves a list of deployment resource names for the given deployment,
	  /// ordered alphabetically.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  IList<string> getDeploymentResourceNames(string deploymentId);

	  /// <summary>
	  /// Retrieves a list of deployment resources for the given deployment,
	  /// ordered alphabetically by name.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  IList<Resource> getDeploymentResources(string deploymentId);

	  /// <summary>
	  /// Gives access to a deployment resource through a stream of bytes.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// <param name="resourceName"> name of the resource, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the resource doesn't exist in the given deployment or when no deployment exists
	  ///          for the given deploymentId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  Stream getResourceAsStream(string deploymentId, string resourceName);

	  /// <summary>
	  /// Gives access to a deployment resource through a stream of bytes.
	  /// </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// <param name="resourceId"> id of the resource, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the resource doesn't exist in the given deployment or when no deployment exists
	  ///          for the given deploymentId. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
	  Stream getResourceAsStreamById(string deploymentId, string resourceId);

	  /// <summary>
	  /// Query process definitions.
	  /// </summary>
	  ProcessDefinitionQuery createProcessDefinitionQuery();

	  /// <summary>
	  /// Query case definitions.
	  /// </summary>
	  CaseDefinitionQuery createCaseDefinitionQuery();

	  /// <summary>
	  /// Query decision definitions.
	  /// </summary>
	  DecisionDefinitionQuery createDecisionDefinitionQuery();

	  /// <summary>
	  /// Query decision requirements definition.
	  /// </summary>
	  DecisionRequirementsDefinitionQuery createDecisionRequirementsDefinitionQuery();

	  /// <summary>
	  /// Query process definitions.
	  /// </summary>
	  DeploymentQuery createDeploymentQuery();

	  /// <summary>
	  /// Suspends the process definition with the given id.
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <strong>Note: all the process instances of the process definition will still be active
	  /// (ie. not suspended)!</strong>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has none of the following:
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void suspendProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Suspends the process definition with the given id.
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be suspended too. </param>
	  /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the
	  ///                       process definition is suspended immediately.
	  ///                       Note: The job executor needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///           <li>and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  /// </exception>
	  /// <seealso cref= RuntimeService#suspendProcessInstanceById(String) </seealso>
	  void suspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances, DateTime suspensionDate);

	  /// <summary>
	  /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <strong>Note: all the process instances of the process definition will still be active
	  /// (ie. not suspended)!</strong>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has none of the following:
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void suspendProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be suspended too. </param>
	  /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the
	  ///                       process definition is suspended immediately.
	  ///                       Note: The job executor needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///           <li>and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  /// </exception>
	  /// <seealso cref= RuntimeService#suspendProcessInstanceById(String) </seealso>
	  void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate);

	  /// <summary>
	  /// Activates the process definition with the given id.
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found or if the process definition is already in state active. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has none of the following:
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void activateProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Activates the process definition with the given id.
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be activated too. </param>
	  /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
	  ///                       process definition is suspended immediately.
	  ///                       Note: The job executor needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///           <li>and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  /// </exception>
	  /// <seealso cref= RuntimeService#activateProcessInstanceById(String) </seealso>
	  void activateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances, DateTime activationDate);

	  /// <summary>
	  /// Activates the process definition with the given key (=id in the bpmn20.xml file).
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has none of the following:
	  ///          <li><seealso cref="ProcessDefinitionPermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///          <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li> </exception>
	  void activateProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  /// Activates the process definition with the given key (=id in the bpmn20.xml file).
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be activated too. </param>
	  /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
	  ///                       process definition is suspended immediately.
	  ///                       Note: The job executor needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such processDefinition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///           <li>If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.</li>
	  ///           <li>and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user has none of the following:</li>
	  ///           <ul>
	  ///           <li><seealso cref="ProcessInstancePermissions#SUSPEND"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="ProcessDefinitionPermissions#SUSPEND_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/></li>
	  ///           <li><seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/></li>
	  ///           </ul>
	  /// </exception>
	  /// <seealso cref= RuntimeService#activateProcessInstanceById(String) </seealso>
	  void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate);

	  /// <summary>
	  /// Activate or suspend process definitions using a fluent builder. Specify the
	  /// definitions by calling one of the <i>by</i> methods, like
	  /// <i>byProcessDefinitionId</i>. To update the suspension state call
	  /// <seealso cref="UpdateProcessDefinitionSuspensionStateBuilder#activate()"/> or
	  /// <seealso cref="UpdateProcessDefinitionSuspensionStateBuilder#suspend()"/>.
	  /// </summary>
	  /// <returns> the builder to update the suspension state </returns>
	  UpdateProcessDefinitionSuspensionStateSelectBuilder updateProcessDefinitionSuspensionState();

	  /// <summary>
	  /// Updates time to live of process definition. The field is used within history cleanup process. </summary>
	  /// <param name="processDefinitionId"> </param>
	  /// <param name="historyTimeToLive"> </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void updateProcessDefinitionHistoryTimeToLive(string processDefinitionId, int? historyTimeToLive);

	  /// <summary>
	  /// Updates time to live of decision definition. The field is used within history cleanup process. </summary>
	  /// <param name="decisionDefinitionId"> </param>
	  /// <param name="historyTimeToLive"> </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  void updateDecisionDefinitionHistoryTimeToLive(string decisionDefinitionId, int? historyTimeToLive);

	  /// <summary>
	  /// Updates time to live of case definition. The field is used within history cleanup process. </summary>
	  /// <param name="caseDefinitionId"> </param>
	  /// <param name="historyTimeToLive"> </param>
	  void updateCaseDefinitionHistoryTimeToLive(string caseDefinitionId, int? historyTimeToLive);

	  /// <summary>
	  /// Gives access to a deployed process model, e.g., a BPMN 2.0 XML file,
	  /// through a stream of bytes.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///           when the process model doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  Stream getProcessModel(string processDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed process diagram, e.g., a PNG image, through a
	  /// stream of bytes.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
	  /// <returns> null when the diagram resource name of a <seealso cref="ProcessDefinition"/> is null.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///           when the process diagram doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  Stream getProcessDiagram(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="ProcessDefinition"/> including all BPMN information like additional
	  /// Properties (e.g. documentation).
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  ProcessDefinition getProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Provides positions and dimensions of elements in a process diagram as
	  /// provided by <seealso cref="RepositoryService#getProcessDiagram(String)"/>.
	  /// 
	  /// This method requires a process model and a diagram image to be deployed.
	  /// </summary>
	  /// <param name="processDefinitionId"> id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
	  /// <returns> Map with process element ids as keys and positions and dimensions as values.
	  /// </returns>
	  /// <returns> null when the input stream of a process diagram is null.
	  /// </returns>
	  /// <exception cref="ProcessEngineException">
	  ///          When the process model or diagram doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  DiagramLayout getProcessDiagramLayout(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="BpmnModelInstance"/> for the given processDefinitionId.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id of the Process Definition for which the <seealso cref="BpmnModelInstance"/>
	  ///  should be retrieved.
	  /// </param>
	  /// <returns> the <seealso cref="BpmnModelInstance"/>
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  BpmnModelInstance getBpmnModelInstance(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="CmmnModelInstance"/> for the given caseDefinitionId.
	  /// </summary>
	  /// <param name="caseDefinitionId"> the id of the Case Definition for which the <seealso cref="CmmnModelInstance"/>
	  ///  should be retrieved.
	  /// </param>
	  /// <returns> the <seealso cref="CmmnModelInstance"/>
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given case definition id or deployment id or resource name is null </exception>
	  /// <exception cref="NotFoundException"> when no CMMN model instance or deployment resource is found for the given
	  ///     case definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  CmmnModelInstance getCmmnModelInstance(string caseDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="DmnModelInstance"/> for the given decisionDefinitionId.
	  /// </summary>
	  /// <param name="decisionDefinitionId"> the id of the Decision Definition for which the <seealso cref="DmnModelInstance"/>
	  ///  should be retrieved.
	  /// </param>
	  /// <returns> the <seealso cref="DmnModelInstance"/>
	  /// </returns>
	  /// <exception cref="NotValidException"> when the given decision definition id or deployment id or resource name is null </exception>
	  /// <exception cref="NotFoundException"> when no DMN model instance or deployment resource is found for the given
	  ///     decision definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  DmnModelInstance getDmnModelInstance(string decisionDefinitionId);

	  /// <summary>
	  /// Authorizes a candidate user for a process definition.
	  /// </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the process definition or user doesn't exist.
	  /// </exception>
	  /// @deprecated Use authorization mechanism instead.
	  ///  
	  [Obsolete("Use authorization mechanism instead.")]
	  void addCandidateStarterUser(string processDefinitionId, string userId);

	  /// <summary>
	  /// Authorizes a candidate group for a process definition.
	  /// </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="groupId"> id of the group involve, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the process definition or group doesn't exist.
	  /// </exception>
	  /// @deprecated Use authorization mechanism instead.
	  ///  
	  [Obsolete("Use authorization mechanism instead.")]
	  void addCandidateStarterGroup(string processDefinitionId, string groupId);

	  /// <summary>
	  /// Removes the authorization of a candidate user for a process definition.
	  /// </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the process definition or user doesn't exist.
	  /// </exception>
	  /// @deprecated Use authorization mechanism instead.
	  ///  
	  [Obsolete("Use authorization mechanism instead.")]
	  void deleteCandidateStarterUser(string processDefinitionId, string userId);

	  /// <summary>
	  /// Removes the authorization of a candidate group for a process definition.
	  /// </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="groupId"> id of the group involve, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When the process definition or group doesn't exist.
	  /// </exception>
	  /// @deprecated Use authorization mechanism instead.
	  ///  
	  [Obsolete("Use authorization mechanism instead.")]
	  void deleteCandidateStarterGroup(string processDefinitionId, string groupId);

	  /// <summary>
	  /// Retrieves the <seealso cref="IdentityLink"/>s associated with the given process definition.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is authorized for a certain process definition
	  /// </summary>
	  /// @deprecated Use authorization mechanism instead.
	  ///  
	  [Obsolete("Use authorization mechanism instead.")]
	  IList<IdentityLink> getIdentityLinksForProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="CaseDefinition"/>.
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given case definition id is null </exception>
	  /// <exception cref="NotFoundException"> when no case definition is found for the given case definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
	  ///     of the command. </exception>
	  CaseDefinition getCaseDefinition(string caseDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed case model, e.g., a CMMN 1.0 XML file,
	  /// through a stream of bytes.
	  /// </summary>
	  /// <param name="caseDefinitionId">
	  ///          id of a <seealso cref="CaseDefinition"/>, cannot be null.
	  /// </param>
	  /// <exception cref="NotValidException"> when the given case definition id or deployment id or resource name is null </exception>
	  /// <exception cref="NotFoundException"> when no case definition or deployment resource is found for the given case definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
	  Stream getCaseModel(string caseDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed case diagram, e.g., a PNG image, through a
	  /// stream of bytes.
	  /// </summary>
	  /// <param name="caseDefinitionId"> id of a <seealso cref="CaseDefinition"/>, cannot be null. </param>
	  /// <returns> null when the diagram resource name of a <seealso cref="CaseDefinition"/> is null. </returns>
	  /// <exception cref="ProcessEngineException"> when the process diagram doesn't exist. </exception>
	  Stream getCaseDiagram(string caseDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="DecisionDefinition"/>.
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given decision definition id is null </exception>
	  /// <exception cref="NotFoundException"> when no decision definition is found for the given decision definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  DecisionDefinition getDecisionDefinition(string decisionDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="DecisionRequirementsDefinition"/>.
	  /// </summary>
	  /// <exception cref="NotValidException"> when the given decision requirements definition id is null </exception>
	  /// <exception cref="NotFoundException"> when no decision requirements definition is found for the given decision requirements definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
	  DecisionRequirementsDefinition getDecisionRequirementsDefinition(string decisionRequirementsDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed decision model, e.g., a DMN 1.1 XML file,
	  /// through a stream of bytes.
	  /// </summary>
	  /// <param name="decisionDefinitionId">
	  ///          id of a <seealso cref="DecisionDefinition"/>, cannot be null.
	  /// </param>
	  /// <exception cref="NotValidException"> when the given decision definition id or deployment id or resource name is null </exception>
	  /// <exception cref="NotFoundException"> when no decision definition or deployment resource is found for the given decision definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  Stream getDecisionModel(string decisionDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed decision requirements model, e.g., a DMN 1.1 XML file,
	  /// through a stream of bytes.
	  /// </summary>
	  /// <param name="decisionRequirementsDefinitionId">
	  ///          id of a <seealso cref="DecisionRequirementsDefinition"/>, cannot be null.
	  /// </param>
	  /// <exception cref="NotValidException"> when the given decision requirements definition id or deployment id or resource name is null </exception>
	  /// <exception cref="NotFoundException"> when no decision requirements definition or deployment resource is found for the given decision requirements definition id </exception>
	  /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
	  Stream getDecisionRequirementsModel(string decisionRequirementsDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed decision diagram, e.g., a PNG image, through a
	  /// stream of bytes.
	  /// </summary>
	  /// <param name="decisionDefinitionId"> id of a <seealso cref="DecisionDefinition"/>, cannot be null. </param>
	  /// <returns> null when the diagram resource name of a <seealso cref="DecisionDefinition"/> is null. </returns>
	  /// <exception cref="ProcessEngineException"> when the decision diagram doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  Stream getDecisionDiagram(string decisionDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed decision requirements diagram, e.g., a PNG image, through a
	  /// stream of bytes.
	  /// </summary>
	  /// <param name="decisionRequirementsDefinitionId"> id of a <seealso cref="DecisionRequirementsDefinition"/>, cannot be null. </param>
	  /// <returns> null when the diagram resource name of a <seealso cref="DecisionRequirementsDefinition"/> is null. </returns>
	  /// <exception cref="ProcessEngineException"> when the decision requirements diagram doesn't exist. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
	  Stream getDecisionRequirementsDiagram(string decisionRequirementsDefinitionId);

	}


}