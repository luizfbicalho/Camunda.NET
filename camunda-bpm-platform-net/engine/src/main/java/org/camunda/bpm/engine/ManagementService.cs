using System;
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

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationRegistration = org.camunda.bpm.application.ProcessApplicationRegistration;
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchQuery = org.camunda.bpm.engine.batch.BatchQuery;
	using BatchStatisticsQuery = org.camunda.bpm.engine.batch.BatchStatisticsQuery;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using DeploymentStatisticsQuery = org.camunda.bpm.engine.management.DeploymentStatisticsQuery;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using TableMetaData = org.camunda.bpm.engine.management.TableMetaData;
	using TablePage = org.camunda.bpm.engine.management.TablePage;
	using TablePageQuery = org.camunda.bpm.engine.management.TablePageQuery;
	using UpdateJobDefinitionSuspensionStateBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateBuilder;
	using UpdateJobDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateSelectBuilder;
	using UpdateJobSuspensionStateBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateBuilder;
	using UpdateJobSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateSelectBuilder;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;



	/// <summary>
	/// Service for admin and maintenance operations on the process engine.
	/// 
	/// These operations will typically not be used in a workflow driven application,
	/// but are used in for example the operational console.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Thorben Lindhauer
	/// </summary>
	public interface ManagementService
	{

	  /// <summary>
	  /// Activate a deployment for a given ProcessApplication. The effect of this
	  /// method is twofold:
	  /// <ol>
	  ///   <li>The process engine will execute atomic operations within the context of
	  ///       that ProcessApplication</li>
	  ///   <li>The job executor will start acquiring jobs from that deployment</li>
	  /// </ol>
	  /// </summary>
	  /// <param name="deploymentId">
	  ///          the Id of the deployment to activate </param>
	  /// <param name="reference">
	  ///          the reference to the process application </param>
	  /// <returns> a new <seealso cref="ProcessApplicationRegistration"/>
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  ProcessApplicationRegistration registerProcessApplication(string deploymentId, ProcessApplicationReference reference);

	  /// <summary>
	  /// Deactivate a deployment for a given ProcessApplication. This removes the association
	  /// between the process engine and the process application and optionally removes the associated
	  /// process definitions from the cache.
	  /// </summary>
	  /// <param name="deploymentId">
	  ///          the Id of the deployment to deactivate </param>
	  /// <param name="removeProcessDefinitionsFromCache">
	  ///          indicates whether the process definitions should be removed from the deployment cache
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void unregisterProcessApplication(string deploymentId, bool removeProcessDefinitionsFromCache);

	  /// <summary>
	  /// Deactivate a deployment for a given ProcessApplication. This removes the association
	  /// between the process engine and the process application and optionally removes the associated
	  /// process definitions from the cache.
	  /// </summary>
	  /// <param name="deploymentIds">
	  ///          the Ids of the deployments to deactivate </param>
	  /// <param name="removeProcessDefinitionsFromCache">
	  ///          indicates whether the process definitions should be removed from the deployment cache
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void unregisterProcessApplication(ISet<string> deploymentIds, bool removeProcessDefinitionsFromCache);

	  /// <returns> the name of the process application that is currently registered for
	  ///         the given deployment or 'null' if no process application is
	  ///         currently registered.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  string getProcessApplicationForDeployment(string deploymentId);

	  /// <summary>
	  /// Get the mapping containing {table name, row count} entries of the database schema.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  IDictionary<string, long> TableCount {get;}

	  /// <summary>
	  /// Gets the table name (including any configured prefix) for an entity like <seealso cref="Task"/>,
	  /// <seealso cref="Execution"/> or the like.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  string getTableName(Type entityClass);

	  /// <summary>
	  /// Gets the metadata (column names, column types, etc.) of a certain table.
	  /// Returns null when no table exists with the given name.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  TableMetaData getTableMetaData(string tableName);

	  /// <summary>
	  /// Creates a <seealso cref="TablePageQuery"/> that can be used to fetch <seealso cref="TablePage"/>
	  /// containing specific sections of table row data.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  TablePageQuery createTablePageQuery();

	  /// <summary>
	  /// Returns a new JobQuery implementation, that can be used
	  /// to dynamically query the jobs.
	  /// </summary>
	  JobQuery createJobQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="JobDefinitionQuery"/> implementation, that can be used
	  /// to dynamically query the job definitions.
	  /// </summary>
	  JobDefinitionQuery createJobDefinitionQuery();

	  /// <summary>
	  /// Forced synchronous execution of a job (eg. for administration or testing)
	  /// The job will be executed, even if the process definition and/or the process instance
	  /// is in suspended state.
	  /// </summary>
	  /// <param name="jobId"> id of the job to execute, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When there is no job with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void executeJob(string jobId);

	  /// <summary>
	  /// Delete the job with the provided id.
	  /// </summary>
	  /// <param name="jobId"> id of the job to execute, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When there is no job with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void deleteJob(string jobId);

	  /// <summary>
	  /// <para>Activates the <seealso cref="JobDefinition"/> with the given id immediately.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> activated.
	  /// </para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #activateJobById(String) </seealso>
	  /// <seealso cref= #activateJobByJobDefinitionId(String) </seealso>
	  void activateJobDefinitionById(string jobDefinitionId);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition id immediately.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> activated.
	  /// </para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionId(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition key immediately.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> activated.
	  /// </para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionKey(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// <para>Activates the <seealso cref="JobDefinition"/> with the given id immediately.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobById(String) </seealso>
	  /// <seealso cref= #activateJobByJobDefinitionId(String) </seealso>
	  void activateJobDefinitionById(string jobDefinitionId, bool activateJobs);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition id immediately.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionId(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition key immediately.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionKey(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs);

	  /// <summary>
	  /// Activates the <seealso cref="JobDefinition"/> with the given id.
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <param name="activationDate"> The date on which the job definition will be activated. If null, the
	  ///                       job definition is activated immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobById(String) </seealso>
	  /// <seealso cref= #activateJobByJobDefinitionId(String) </seealso>
	  void activateJobDefinitionById(string jobDefinitionId, bool activateJobs, DateTime activationDate);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition id.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <param name="activationDate"> The date on which the job definition will be activated. If null, the
	  ///                       job definition is activated immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionId(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionId(string processDefinitionId, bool activateJobs, DateTime activationDate);

	  /// <summary>
	  /// <para>Activates all <seealso cref="JobDefinition"/>s of the provided process definition key.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="activateJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be activated too.
	  /// </param>
	  /// <param name="activationDate"> The date on which the job definition will be activated. If null, the
	  ///                       job definition is activated immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>activateJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #activateJobByProcessDefinitionKey(String) </seealso>
	  void activateJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool activateJobs, DateTime activationDate);

	  /// <summary>
	  /// <para>Suspends the <seealso cref="JobDefinition"/> with the given id immediately.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> suspended.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If no such job definition can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #suspendJobById(String) </seealso>
	  /// <seealso cref= #suspendJobByJobDefinitionId(String) </seealso>
	  void suspendJobDefinitionById(string jobDefinitionId);

	  /// <summary>
	  /// <para>Suspends all <seealso cref="JobDefinition"/> of the provided process definition id immediately.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> suspended.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionId(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Suspends all <seealso cref="JobDefinition"/> of the provided process definition key immediately.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// 
	  /// <para>
	  /// <strong>Note:</strong> All <seealso cref="Job"/>s of the provided job definition
	  /// will be <strong>not</strong> suspended.
	  /// </para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>.
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionKey(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Suspends the <seealso cref="JobDefinition"/> with the given id immediately.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobById(String) </seealso>
	  /// <seealso cref= #suspendJobByJobDefinitionId(String) </seealso>
	  void suspendJobDefinitionById(string jobDefinitionId, bool suspendJobs);

	  /// <summary>
	  /// Suspends all <seealso cref="JobDefinition"/>s of the provided process definition id immediately.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionId(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs);

	  /// <summary>
	  /// Suspends all <seealso cref="JobDefinition"/>s of the provided process definition key immediately.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionKey(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs);

	  /// <summary>
	  /// Suspends the <seealso cref="JobDefinition"/> with the given id.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <param name="suspensionDate"> The date on which the job definition will be suspended. If null, the
	  ///                       job definition is suspended immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobById(String) </seealso>
	  /// <seealso cref= #suspendJobByJobDefinitionId(String) </seealso>
	  void suspendJobDefinitionById(string jobDefinitionId, bool suspendJobs, DateTime suspensionDate);

	  /// <summary>
	  /// Suspends all <seealso cref="JobDefinition"/>s of the provided process definition id.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <param name="suspensionDate"> The date on which the job definition will be suspended. If null, the
	  ///                       job definition is suspended immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionId(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId, bool suspendJobs, DateTime suspensionDate);

	  /// <summary>
	  /// Suspends all <seealso cref="JobDefinition"/>s of the provided process definition key.
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobDefinitionSuspensionState()"/>.</para>
	  /// </summary>
	  /// <param name="suspendJobs"> If true, all the <seealso cref="Job"/>s of the provided job definition
	  ///                     will be suspended too.
	  /// </param>
	  /// <param name="suspensionDate"> The date on which the job definition will be suspended. If null, the
	  ///                       job definition is suspended immediately.
	  ///                       Note: The <seealso cref="JobExecutor"/> needs to be active to use this!
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If <code>suspendJobs</code> is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on any <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///   </ul>
	  /// </exception>
	  /// <seealso cref= #suspendJobByProcessDefinitionKey(String) </seealso>
	  void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey, bool suspendJobs, DateTime suspensionDate);

	  /// <summary>
	  /// <para>Activates the <seealso cref="Job"/> with the given id.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void activateJobById(string jobId);

	  /// <summary>
	  /// <para>Activates all <seealso cref="Job"/>s of the provided job definition id.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void activateJobByJobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// <para>Activates all <seealso cref="Job"/>s of the provided process instance id.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process instance id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void activateJobByProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// <para>Activates all <seealso cref="Job"/>s of the provided process definition id.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void activateJobByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Activates <seealso cref="Job"/>s of the provided process definition key.</para>
	  /// 
	  /// <para>Note: for more complex activate commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void activateJobByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// <para>Suspends the <seealso cref="Job"/> with the given id.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void suspendJobById(string jobId);

	  /// <summary>
	  /// <para>Suspends all <seealso cref="Job"/>s of the provided job definition id.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the job definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void suspendJobByJobDefinitionId(string jobDefinitionId);

	  /// <summary>
	  /// <para>Suspends all <seealso cref="Job"/>s of the provided process instance id.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process instance id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void suspendJobByProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// <para>Suspends all <seealso cref="Job"/>s of the provided process definition id.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition id is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void suspendJobByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// <para>Suspends <seealso cref="Job"/>s of the provided process definition key.</para>
	  /// 
	  /// <para>Note: for more complex suspend commands use <seealso cref="updateJobSuspensionState()"/>.</para>
	  /// </summary>
	  /// <exception cref="ProcessEngineException">
	  ///          If the process definition key is equal null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void suspendJobByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Activate or suspend jobs using a fluent builder. Specify the jobs by
	  /// calling one of the <i>by</i> methods, like <i>byJobId</i>. To update the
	  /// suspension state call <seealso cref="UpdateJobSuspensionStateBuilder.activate()"/> or
	  /// <seealso cref="UpdateJobSuspensionStateBuilder.suspend()"/>.
	  /// </summary>
	  /// <returns> the builder to update the suspension state </returns>
	  UpdateJobSuspensionStateSelectBuilder updateJobSuspensionState();

	  /// <summary>
	  /// Activate or suspend job definitions using a fluent builder. Specify the job
	  /// definitions by calling one of the <i>by</i> methods, like
	  /// <i>byJobDefinitionId</i>. To update the suspension state call
	  /// <seealso cref="UpdateJobDefinitionSuspensionStateBuilder.activate()"/> or
	  /// <seealso cref="UpdateJobDefinitionSuspensionStateBuilder.suspend()"/>.
	  /// </summary>
	  /// <returns> the builder to update the suspension state </returns>
	  UpdateJobDefinitionSuspensionStateSelectBuilder updateJobDefinitionSuspensionState();

	  /// <summary>
	  /// Sets the number of retries that a job has left.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// </summary>
	  /// <param name="jobId"> id of the job to modify, cannot be null. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          and no <seealso cref="ProcessInstancePermissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="ProcessDefinitionPermissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void setJobRetries(string jobId, int retries);

	  /// <summary>
	  /// Sets the number of retries that jobs have left.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// </summary>
	  /// <param name="jobIds"> ids of the jobs to modify, cannot be null. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="BadUserRequestException"> if jobIds is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          and no <seealso cref="Permissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void setJobRetries(IList<string> jobIds, int retries);

	  /// <summary>
	  /// Sets the number of retries that jobs have left asynchronously.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// </summary>
	  /// <param name="jobIds"> ids of the jobs to modify, cannot be null. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="BadUserRequestException"> if jobIds is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> or
	  ///          <seealso cref="BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES"/> permission on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch setJobRetriesAsync(IList<string> jobIds, int retries);

	  /// <summary>
	  /// Sets the number of retries that jobs have left asynchronously.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// </summary>
	  /// <param name="jobQuery"> query that identifies which jobs should be modified, cannot be null. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="BadUserRequestException"> if jobQuery is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> or
	  ///          <seealso cref="BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES"/> permission on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch setJobRetriesAsync(JobQuery jobQuery, int retries);

	  /// <summary>
	  /// Sets the number of retries that jobs have left asynchronously.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// 
	  /// Either jobIds or jobQuery has to be provided. If both are provided resulting list
	  /// of affected jobs will contain jobs matching query as well as jobs defined in the list.
	  /// </summary>
	  /// <param name="jobIds"> ids of the jobs to modify. </param>
	  /// <param name="jobQuery"> query that identifies which jobs should be modified. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="BadUserRequestException"> if neither jobIds, nor jobQuery is provided or result in empty list </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> or
	  ///          <seealso cref="BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES"/> permission on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch setJobRetriesAsync(IList<string> jobIds, JobQuery jobQuery, int retries);

	  /// <summary>
	  /// Sets the number of retries that jobs have left asynchronously.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries.
	  /// 
	  /// Either jobIds or jobQuery has to be provided. If both are provided resulting list
	  /// of affected jobs will contain jobs matching query as well as jobs defined in the list.
	  /// </summary>
	  /// <param name="processInstanceIds"> ids of the process instances that for which jobs retries will be set </param>
	  /// <param name="query"> query that identifies process instances with jobs that have to be modified </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.CREATE"/> or
	  ///          <seealso cref="BatchPermissions.CREATE_BATCH_SET_JOB_RETRIES"/> permission on <seealso cref="Resources.BATCH"/>. </exception>
	  Batch setJobRetriesAsync(IList<string> processInstanceIds, ProcessInstanceQuery query, int retries);

	  /// <summary>
	  /// <para>
	  /// Set the number of retries of all <strong>failed</strong> <seealso cref="Job jobs"/>
	  /// of the provided job definition id.
	  /// </para>
	  /// 
	  /// <para>
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented.
	  /// When it hits zero, the job is supposed to be <strong>failed</strong> and
	  /// not retried again. In that case, this method can be used to increase the
	  /// number of retries.
	  /// </para>
	  /// 
	  /// <para>
	  /// <seealso cref="Incident Incidents"/> of the involved failed <seealso cref="Job jobs"/> will not
	  /// be resolved using this method! When the execution of a job was successful
	  /// the corresponding incident will be resolved.
	  /// </para>
	  /// </summary>
	  /// <param name="jobDefinitionId"> id of the job definition, cannot be null. </param>
	  /// <param name="retries"> number of retries.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>
	  ///          and no <seealso cref="Permissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          and no <seealso cref="Permissions.RETRY_JOB"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void setJobRetriesByJobDefinitionId(string jobDefinitionId, int retries);

	  /// <summary>
	  /// Sets a new due date for the provided id.
	  /// When newDuedate is null, the job is executed with the next
	  /// job executor run.
	  /// </summary>
	  /// <param name="jobId"> id of job to modify, cannot be null. </param>
	  /// <param name="newDuedate"> new date for job execution
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void setJobDuedate(string jobId, DateTime newDuedate);

	  /// <summary>
	  /// Triggers the recalculation for the job with the provided id.
	  /// </summary>
	  /// <param name="jobId"> id of job to recalculate, must neither be null nor empty. </param>
	  /// <param name="creationDateBased">
	  ///          indicates whether the recalculation should be based on the
	  ///          creation date of the job or the current date
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///           If the user has no <seealso cref="Permissions.UPDATE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_INSTANCE"/> or no
	  ///           <seealso cref="Permissions.UPDATE_INSTANCE"/> permission on
	  ///           <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  void recalculateJobDuedate(string jobId, bool creationDateBased);

	  /// <summary>
	  /// Sets a new priority for the job with the provided id.
	  /// </summary>
	  /// <param name="jobId"> the id of the job to modify, must not be null </param>
	  /// <param name="priority"> the job's new priority
	  /// </param>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  /// @since 7.4 </exception>
	  void setJobPriority(string jobId, long priority);

	  /// <summary>
	  /// <para>Sets an explicit priority for jobs of the given job definition.
	  /// Jobs created after invoking this method receive the given priority.
	  /// This setting overrides any setting specified in the BPMN 2.0 XML.</para>
	  /// 
	  /// <para>The overriding priority can be cleared by using the method
	  /// <seealso cref="clearOverridingJobPriorityForJobDefinition(string)"/>.</para>
	  /// </summary>
	  /// <param name="jobDefinitionId"> the id of the job definition to set the priority for </param>
	  /// <param name="priority"> the priority to set;
	  /// </param>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  /// @since 7.4 </exception>
	  void setOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority);

	  /// <summary>
	  /// <para>Sets an explicit default priority for jobs of the given job definition.
	  /// Jobs created after invoking this method receive the given priority.
	  /// This setting overrides any setting specified in the BPMN 2.0 XML.</para>
	  /// 
	  /// <para>If <code>cascade</code> is true, priorities of already existing jobs
	  /// are updated accordingly.</para>
	  /// 
	  /// <para>The overriding priority can be cleared by using the method
	  /// <seealso cref="clearOverridingJobPriorityForJobDefinition(string)"/>.</para>
	  /// </summary>
	  /// <param name="jobDefinitionId"> the id of the job definition to set the priority for </param>
	  /// <param name="priority"> the priority to set </param>
	  /// <param name="cascade"> if true, priorities of existing jobs of the given definition are changed as well
	  /// </param>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  ///   If cascade is <code>true</code>, the user must further possess one of the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_INSTANCE"/></li>
	  ///     <li><seealso cref="Permissions.UPDATE_INSTANCE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  /// @since 7.4 </exception>
	  void setOverridingJobPriorityForJobDefinition(string jobDefinitionId, long priority, bool cascade);

	  /// <summary>
	  /// <para>Clears the job definition's overriding job priority if set. After invoking this method,
	  /// new jobs of the given definition receive the priority as specified in the BPMN 2.0 XML
	  /// or the global default priority.</para>
	  /// 
	  /// <para>Existing job instance priorities remain unchanged.</para>
	  /// </summary>
	  /// <param name="jobDefinitionId"> the id of the job definition for which to clear the overriding priority
	  /// </param>
	  /// <exception cref="AuthorizationException"> thrown if the current user does not possess any of the following permissions
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.UPDATE"/> on <seealso cref="Resources.PROCESS_DEFINITION"/></li>
	  ///   </ul>
	  /// 
	  /// @since 7.4 </exception>
	  void clearOverridingJobPriorityForJobDefinition(string jobDefinitionId);

	  /// <summary>
	  /// Returns the full stacktrace of the exception that occurs when the job
	  /// with the given id was last executed. Returns null when the job has no
	  /// exception stacktrace.
	  /// </summary>
	  /// <param name="jobId"> id of the job, cannot be null.
	  /// </param>
	  /// <exception cref="ProcessEngineException">
	  ///          When no job exists with the given id. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_INSTANCE"/>
	  ///          or no <seealso cref="Permissions.READ_INSTANCE"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  string getJobExceptionStacktrace(string jobId);

	  /// <returns> a map of all properties.
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  IDictionary<string, string> Properties {get;}

	  /// <summary>
	  /// Set the value for a property.
	  /// </summary>
	  /// <param name="name"> the name of the property.
	  /// </param>
	  /// <param name="value"> the new value for the property.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void setProperty(string name, string value);

	  /// <summary>
	  /// Deletes a property by name. If the property does not exist, the request is ignored.
	  /// </summary>
	  /// <param name="name"> the name of the property to delete
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void deleteProperty(string name);

	  /// <summary>
	  /// programmatic schema update on a given connection returning feedback about what happened
	  /// 
	  ///  Note: will always return an empty string
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  string databaseSchemaUpgrade(Connection connection, string catalog, string schema);

	  /// <summary>
	  /// Query for the number of process instances aggregated by process definitions.
	  /// </summary>
	  ProcessDefinitionStatisticsQuery createProcessDefinitionStatisticsQuery();

	  /// <summary>
	  /// Query for the number of process instances aggregated by deployments.
	  /// </summary>
	  DeploymentStatisticsQuery createDeploymentStatisticsQuery();

	  /// <summary>
	  /// Query for the number of activity instances aggregated by activities of a single process definition.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.READ"/> permission on <seealso cref="Resources.PROCESS_DEFINITION"/>. </exception>
	  ActivityStatisticsQuery createActivityStatisticsQuery(string processDefinitionId);

	  /// <summary>
	  /// Get the deployments that are registered the engine's job executor.
	  /// This set is only relevant, if the engine configuration property <code>jobExecutorDeploymentAware</code> is set.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  ISet<string> RegisteredDeployments {get;}

	  /// <summary>
	  /// Register a deployment for the engine's job executor.
	  /// This is required, if the engine configuration property <code>jobExecutorDeploymentAware</code> is set.
	  /// If set to false, the job executor will execute any job.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void registerDeploymentForJobExecutor(string deploymentId);

	  /// <summary>
	  /// Unregister a deployment for the engine's job executor.
	  /// If the engine configuration property <code>jobExecutorDeploymentAware</code> is set,
	  /// jobs for the given deployment will no longer get acquired.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  void unregisterDeploymentForJobExecutor(string deploymentId);

	  /// <summary>
	  /// Get the configured history level for the process engine.
	  /// </summary>
	  /// <returns> the history level
	  /// </returns>
	  /// <exception cref="AuthorizationException">
	  ///          If the user is not a member of the group <seealso cref="Groups.CAMUNDA_ADMIN"/>. </exception>
	  int HistoryLevel {get;}

	  /// <returns> a new metrics Query.
	  /// @since 7.3 </returns>
	  MetricsQuery createMetricsQuery();

	  /// <summary>
	  /// Deletes all metrics events which are older than the specified timestamp.
	  /// If the timestamp is null, all metrics will be deleted
	  /// </summary>
	  /// <param name="timestamp"> or null
	  /// @since 7.3 </param>
	  void deleteMetrics(DateTime timestamp);

	  /// <summary>
	  /// Deletes all metrics events which are older than the specified timestamp
	  /// and reported by the given reporter. If a parameter is null, all metric events
	  /// are matched in that regard.
	  /// </summary>
	  /// <param name="timestamp"> or null </param>
	  /// <param name="reporter"> or null
	  /// @since 7.4 </param>
	  void deleteMetrics(DateTime timestamp, string reporter);

	  /// <summary>
	  /// Forces this engine to commit its pending collected metrics to the database.
	  /// </summary>
	  /// <exception cref="ProcessEngineException"> if metrics reporting is disabled or the db metrics
	  /// reporter is deactivated </exception>
	  void reportDbMetricsNow();

	  /// <summary>
	  /// Creates a query to search for <seealso cref="org.camunda.bpm.engine.batch.Batch"/> instances.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  BatchQuery createBatchQuery();

	  /// <summary>
	  /// <para>
	  ///   Suspends the <seealso cref="Batch"/> with the given id immediately.
	  /// </para>
	  /// 
	  /// <para>
	  ///   <strong>Note:</strong> All <seealso cref="JobDefinition"/>s and <seealso cref="Job"/>s
	  ///   related to the provided batch will be suspended.
	  /// </para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          If no such batch can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.BATCH"/>.
	  /// 
	  /// @since 7.5 </exception>
	  void suspendBatchById(string batchId);

	  /// <summary>
	  /// <para>
	  ///   Activates the <seealso cref="Batch"/> with the given id immediately.
	  /// </para>
	  /// 
	  /// <para>
	  ///   <strong>Note:</strong> All <seealso cref="JobDefinition"/>s and <seealso cref="Job"/>s
	  ///   related to the provided batch will be activated.
	  /// </para>
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          If no such batch can be found. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.UPDATE"/> permission on <seealso cref="Resources.BATCH"/>.
	  /// 
	  /// @since 7.5 </exception>
	  void activateBatchById(string batchId);

	  /// <summary>
	  /// Deletes a batch instance and the corresponding job definitions.
	  /// 
	  /// If cascade is set to true the historic batch instances and the
	  /// historic jobs logs are also removed.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions.DELETE"/> permission on <seealso cref="Resources.BATCH"/>
	  /// 
	  /// @since 7.5 </exception>
	  void deleteBatch(string batchId, bool cascade);

	  /// <summary>
	  /// Query for the statistics of the batch execution jobs of a batch.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  BatchStatisticsQuery createBatchStatisticsQuery();

	  /// <summary>
	  /// Query for entries of the database schema log.
	  /// 
	  /// @since 7.11
	  /// </summary>
	  SchemaLogQuery createSchemaLogQuery();

	}

}