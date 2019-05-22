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
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using UserOperationLogCategoryPermissions = org.camunda.bpm.engine.authorization.UserOperationLogCategoryPermissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using HistoricCaseActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricCaseActivityStatisticsQuery;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceQuery;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using CleanableHistoricBatchReport = org.camunda.bpm.engine.history.CleanableHistoricBatchReport;
	using CleanableHistoricCaseInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReport;
	using CleanableHistoricDecisionInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricDecisionInstanceReport;
	using CleanableHistoricProcessInstanceReport = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReport;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricTaskInstanceReport = org.camunda.bpm.engine.history.HistoricTaskInstanceReport;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using NativeHistoricActivityInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricActivityInstanceQuery;
	using NativeHistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricCaseActivityInstanceQuery;
	using NativeHistoricCaseInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricCaseInstanceQuery;
	using NativeHistoricDecisionInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricDecisionInstanceQuery;
	using NativeHistoricProcessInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricProcessInstanceQuery;
	using NativeHistoricTaskInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricTaskInstanceQuery;
	using NativeHistoricVariableInstanceQuery = org.camunda.bpm.engine.history.NativeHistoricVariableInstanceQuery;
	using SetRemovalTimeSelectModeForHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricBatchesBuilder;
	using SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder;
	using SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder;
	using SetRemovalTimeToHistoricBatchesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricBatchesBuilder;
	using SetRemovalTimeToHistoricDecisionInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricDecisionInstancesBuilder;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using HistoricDecisionInstanceStatisticsQuery = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatisticsQuery;
	using SetRemovalTimeToHistoricProcessInstancesBuilder = org.camunda.bpm.engine.history.SetRemovalTimeToHistoricProcessInstancesBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;


	/// <summary>
	/// Service exposing information about ongoing and past process instances.  This is different
	/// from the runtime information in the sense that this runtime information only contains
	/// the actual runtime state at any given moment and it is optimized for runtime
	/// process execution performance.  The history information is optimized for easy
	/// querying and remains permanent in the persistent storage.
	/// 
	/// @author Christian Stettler
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface HistoryService
	{

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricProcessInstance"/>s. </summary>
	  HistoricProcessInstanceQuery createHistoricProcessInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricActivityInstance"/>s. </summary>
	  HistoricActivityInstanceQuery createHistoricActivityInstanceQuery();

	  /// <summary>
	  /// Query for the number of historic activity instances aggregated by activities of a single process definition.
	  /// </summary>
	  HistoricActivityStatisticsQuery createHistoricActivityStatisticsQuery(string processDefinitionId);

	  /// <summary>
	  /// Query for the number of historic case activity instances aggregated by case activities of a single case definition.
	  /// </summary>
	  HistoricCaseActivityStatisticsQuery createHistoricCaseActivityStatisticsQuery(string caseDefinitionId);

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricTaskInstance"/>s. </summary>
	  HistoricTaskInstanceQuery createHistoricTaskInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricDetail"/>s. </summary>
	  HistoricDetailQuery createHistoricDetailQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricVariableInstance"/>s.
	  /// <para>
	  /// The result of the query is empty:
	  /// <li>if the user has no <seealso cref="Permissions#READ_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> or</li>
	  /// <li>the user has no <seealso cref="ProcessDefinitionPermissions#READ_HISTORY_VARIABLE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
	  /// in case <seealso cref="ProcessEngineConfiguration#enforceSpecificVariablePermission"/> is enabled.</li>
	  /// </para>
	  /// </summary>
	  HistoricVariableInstanceQuery createHistoricVariableInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="UserOperationLogEntry"/> instances. </summary>
	  UserOperationLogQuery createUserOperationLogQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricIncident historic incidents"/>. </summary>
	  HistoricIncidentQuery createHistoricIncidentQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricIdentityLinkLog historic identity links"/>. </summary>
	  HistoricIdentityLinkLogQuery createHistoricIdentityLinkLogQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricCaseInstance"/>s. </summary>
	  HistoricCaseInstanceQuery createHistoricCaseInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricCaseActivityInstance"/>s. </summary>
	  HistoricCaseActivityInstanceQuery createHistoricCaseActivityInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricDecisionInstance"/>s.
	  /// 
	  /// If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>
	  /// then the result of the query is empty.
	  /// </summary>
	  HistoricDecisionInstanceQuery createHistoricDecisionInstanceQuery();

	  /// <summary>
	  /// Deletes historic task instance.  This might be useful for tasks that are
	  /// <seealso cref="TaskService#newTask() dynamically created"/> and then <seealso cref="TaskService#complete(String) completed"/>.
	  /// If the historic task instance doesn't exist, no exception is thrown and the
	  /// method returns normal.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricTaskInstance(string taskId);

	  /// <summary>
	  /// Deletes historic process instance. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricProcessInstance(string processInstanceId);

	  /// <summary>
	  /// Deletes historic process instance. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// Does not fail if a process instance was not found.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricProcessInstanceIfExists(string processInstanceId);

	  /// <summary>
	  /// Deletes historic process instances. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instances are found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricProcessInstances(IList<string> processInstanceIds);

	  /// <summary>
	  /// Deletes historic process instances. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well. Does not
	  /// fail if a process instance was not found.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricProcessInstancesIfExists(IList<string> processInstanceIds);

	  /// <summary>
	  /// Deletes historic process instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
	  /// of given process instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
	  /// </summary>
	  /// <param name="processInstanceIds"> list of process instance ids for removal
	  /// </param>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instances are found with the given ids or ids are null or when some of the process instances are not finished yet </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricProcessInstancesBulk(IList<string> processInstanceIds);

	  /// <summary>
	  /// Schedules history cleanup job at batch window start time. The job will delete historic data for
	  /// finished process, decision and case instances, and batch operations taking into account <seealso cref="ProcessDefinition#getHistoryTimeToLive()"/>,
	  /// <seealso cref="DecisionDefinition#getHistoryTimeToLive()"/>, <seealso cref="CaseDefinition#getHistoryTimeToLive()"/>, <seealso cref="ProcessEngineConfigurationImpl#getBatchOperationHistoryTimeToLive()"/>
	  /// and <seealso cref="ProcessEngineConfigurationImpl#getBatchOperationsForHistoryCleanup()"/> values.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> </exception>
	  /// <returns> history cleanup job. NB! As of v. 7.9.0, method does not guarantee to return a job. Use <seealso cref="#findHistoryCleanupJobs()"/> instead. </returns>
	  Job cleanUpHistoryAsync();

	  /// <summary>
	  /// Schedules history cleanup job at batch window start time. The job will delete historic data for
	  /// finished process, decision and case instances, and batch operations taking into account <seealso cref="ProcessDefinition#getHistoryTimeToLive()"/>,
	  /// <seealso cref="DecisionDefinition#getHistoryTimeToLive()"/>, <seealso cref="CaseDefinition#getHistoryTimeToLive()"/>, <seealso cref="ProcessEngineConfigurationImpl#getBatchOperationHistoryTimeToLive()"/>
	  /// and <seealso cref="ProcessEngineConfigurationImpl#getBatchOperationsForHistoryCleanup()"/> values.
	  /// </summary>
	  /// <param name="immediatelyDue"> must be true if cleanup must be scheduled at once, otherwise is will be scheduled according to configured batch window </param>
	  /// <exception cref="AuthorizationException">
	  ///      If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/> </exception>
	  /// <returns> history cleanup job. Job id can be used to check job logs, incident etc.
	  ///  </returns>
	  Job cleanUpHistoryAsync(bool immediatelyDue);

	  /// <summary>
	  /// Finds history cleanup job, if present. </summary>
	  /// @deprecated As of v. 7.9.0, because there can be more than one history cleanup job at once, use <seealso cref="#findHistoryCleanupJobs"/> instead. 
	  /// <returns> job entity </returns>
	  [Obsolete("As of v. 7.9.0, because there can be more than one history cleanup job at once, use <seealso cref="#findHistoryCleanupJobs"/> instead.")]
	  Job findHistoryCleanupJob();

	  /// <summary>
	  /// Finds history cleanup job if present. </summary>
	  /// <returns> job entity </returns>
	  IList<Job> findHistoryCleanupJobs();

	  /// <summary>
	  /// Deletes historic process instances asynchronously. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instances is found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, string deleteReason);

	  /// <summary>
	  /// Deletes historic process instances asynchronously based on query. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instances is found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricProcessInstancesAsync(HistoricProcessInstanceQuery query, string deleteReason);

	  /// <summary>
	  /// Deletes historic process instances asynchronously based on query and a list of process instances. Query result and
	  /// list of ids will be merged.
	  /// All historic activities, historic task and historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no process instances is found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricProcessInstancesAsync(IList<string> processInstanceIds, HistoricProcessInstanceQuery query, string deleteReason);

	  /// <summary>
	  /// Deletes a user operation log entry. Does not cascade to any related entities.
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///           For entries related to process definition keys: If the user has
	  ///           neither <seealso cref="Permissions#DELETE_HISTORY"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/> nor
	  ///           <seealso cref="UserOperationLogCategoryPermissions#DELETE"/> permission on
	  ///           <seealso cref="Resources#OPERATION_LOG_CATEGORY"/>. For entries not related
	  ///           to process definition keys: If the user has no
	  ///           <seealso cref="UserOperationLogCategoryPermissions#DELETE"/> permission on
	  ///           <seealso cref="Resources#OPERATION_LOG_CATEGORY"/>. </exception>
	  void deleteUserOperationLogEntry(string entryId);

	  /// <summary>
	  /// Deletes historic case instance. All historic case activities, historic task and
	  /// historic details are deleted as well.
	  /// </summary>
	  void deleteHistoricCaseInstance(string caseInstanceId);

	  /// <summary>
	  /// Deletes historic case instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
	  /// of given case instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
	  /// </summary>
	  /// <param name="caseInstanceIds"> list of case instance ids for removal </param>
	  void deleteHistoricCaseInstancesBulk(IList<string> caseInstanceIds);

	  /// <summary>
	  /// Deletes historic decision instances of a decision definition. All historic
	  /// decision inputs and outputs are deleted as well.
	  /// </summary>
	  /// @deprecated Note that this method name is not expressive enough, because it is also possible to delete the historic
	  /// decision instance by the instance id. Therefore use <seealso cref="#deleteHistoricDecisionInstanceByDefinitionId"/> instead
	  /// to delete the historic decision instance by the definition id.
	  /// 
	  /// <param name="decisionDefinitionId">
	  ///          the id of the decision definition
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  [Obsolete("Note that this method name is not expressive enough, because it is also possible to delete the historic")]
	  void deleteHistoricDecisionInstance(string decisionDefinitionId);

	  /// <summary>
	  /// Deletes decision instances and all related historic data in bulk manner. DELETE SQL statement will be created for each entity type. They will have list
	  /// of given decision instance ids in IN clause. Therefore, DB limitation for number of values in IN clause must be taken into account.
	  /// </summary>
	  /// <param name="decisionInstanceIds"> list of decision instance ids for removal.
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  void deleteHistoricDecisionInstancesBulk(IList<string> decisionInstanceIds);

	  /// <summary>
	  /// Deletes historic decision instances of a decision definition. All historic
	  /// decision inputs and outputs are deleted as well.
	  /// </summary>
	  /// <param name="decisionDefinitionId">
	  ///          the id of the decision definition
	  /// </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  void deleteHistoricDecisionInstanceByDefinitionId(string decisionDefinitionId);


	  /// <summary>
	  /// Deletes historic decision instances by its id. All historic
	  /// decision inputs and outputs are deleted as well.
	  /// </summary>
	  /// <param name="historicDecisionInstanceId">
	  ///          the id of the historic decision instance </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
	  void deleteHistoricDecisionInstanceByInstanceId(string historicDecisionInstanceId);

	  /// <summary>
	  /// Deletes historic decision instances asynchronously based on a list of decision instances.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no decision instances are found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_DECISION_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricDecisionInstancesAsync(IList<string> decisionInstanceIds, string deleteReason);

	  /// <summary>
	  /// Deletes historic decision instances asynchronously based on query of decision instances.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no decision instances are found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_DECISION_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricDecisionInstancesAsync(HistoricDecisionInstanceQuery query, string deleteReason);

	  /// <summary>
	  /// Deletes historic decision instances asynchronously based on query and a list of decision instances, whereby query result and
	  /// list of ids will be merged.
	  /// </summary>
	  /// <exception cref="BadUserRequestException">
	  ///          when no decision instances are found with the given ids or ids are null. </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#CREATE"/> or
	  ///          <seealso cref="BatchPermissions#CREATE_BATCH_DELETE_DECISION_INSTANCES"/> permission on <seealso cref="Resources#BATCH"/>. </exception>
	  Batch deleteHistoricDecisionInstancesAsync(IList<string> decisionInstanceIds, HistoricDecisionInstanceQuery query, string deleteReason);

	  /// <summary>
	  /// Deletes a historic variable instance by its id. All related historic
	  /// details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  /// <param name="variableInstanceId">
	  ///          the id of the variable instance </param>
	  /// <exception cref="BadUserRequestException">
	  ///           when the historic variable instance is not found by the given id
	  ///           or if id is null </exception>
	  /// <exception cref="AuthorizationException">
	  ///           If the variable instance has a process definition key and
	  ///           the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on
	  ///           <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricVariableInstance(string variableInstanceId);

	  /// <summary>
	  /// Deletes all historic variables and historic details (variable updates, form properties) of a process instance.
	  /// </summary>
	  /// <param name="processInstanceId">
	  ///          the id of the process instance </param>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
	  void deleteHistoricVariableInstancesByProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricProcessInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricProcessInstanceQuery createNativeHistoricProcessInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricTaskInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricTaskInstanceQuery createNativeHistoricTaskInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricActivityInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricActivityInstanceQuery createNativeHistoricActivityInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricCaseInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricCaseInstanceQuery createNativeHistoricCaseInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricCaseActivityInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricCaseActivityInstanceQuery createNativeHistoricCaseActivityInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricDecisionInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricDecisionInstanceQuery createNativeHistoricDecisionInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricVariableInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricVariableInstanceQuery createNativeHistoricVariableInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricJobLog historic job logs"/>.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoricJobLogQuery createHistoricJobLogQuery();

	  /// <summary>
	  /// Returns the full stacktrace of the exception that occurs when the
	  /// historic job log with the given id was last executed. Returns null
	  /// when the historic job log has no exception stacktrace.
	  /// </summary>
	  /// <param name="historicJobLogId"> id of the historic job log, cannot be null. </param>
	  /// <exception cref="ProcessEngineException"> when no historic job log exists with the given id.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.3 </exception>
	  string getHistoricJobLogExceptionStacktrace(string historicJobLogId);

	  /// <summary>
	  /// Creates a new programmatic query to create a historic process instance report.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricProcessInstanceReport createHistoricProcessInstanceReport();

	  /// <summary>
	  /// Creates a new programmatic query to create a historic task instance report.
	  /// 
	  /// @since 7.6
	  /// </summary>
	  HistoricTaskInstanceReport createHistoricTaskInstanceReport();

	  /// <summary>
	  /// Creates a new programmatic query to create a cleanable historic process instance report.
	  /// 
	  /// @since 7.8
	  /// </summary>
	  CleanableHistoricProcessInstanceReport createCleanableHistoricProcessInstanceReport();

	  /// <summary>
	  /// Creates a new programmatic query to create a cleanable historic decision instance report.
	  /// 
	  /// @since 7.8
	  /// </summary>
	  CleanableHistoricDecisionInstanceReport createCleanableHistoricDecisionInstanceReport();

	  /// <summary>
	  /// Creates a new programmatic query to create a cleanable historic case instance report.
	  /// 
	  /// @since 7.8
	  /// </summary>
	  CleanableHistoricCaseInstanceReport createCleanableHistoricCaseInstanceReport();

	  /// <summary>
	  /// Creates a new programmatic query to create a cleanable historic batch report.
	  /// 
	  /// @since 7.8
	  /// </summary>
	  CleanableHistoricBatchReport createCleanableHistoricBatchReport();

	  /// <summary>
	  /// Creates a query to search for <seealso cref="org.camunda.bpm.engine.batch.history.HistoricBatch"/> instances.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoricBatchQuery createHistoricBatchQuery();

	  /// <summary>
	  /// Deletes a historic batch instance. All corresponding historic job logs are deleted as well;
	  /// 
	  /// @since 7.5
	  /// </summary>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#BATCH"/> </exception>
	  void deleteHistoricBatch(string id);


	  /// <summary>
	  /// Query for the statistics of DRD evaluation.
	  /// </summary>
	  /// <param name="decisionRequirementsDefinitionId"> - id of decision requirement definition
	  /// @since 7.6 </param>
	  HistoricDecisionInstanceStatisticsQuery createHistoricDecisionInstanceStatisticsQuery(string decisionRequirementsDefinitionId);

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricExternalTaskLog historic external task logs"/>.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  HistoricExternalTaskLogQuery createHistoricExternalTaskLogQuery();

	  /// <summary>
	  /// Returns the full error details that occurs when the
	  /// historic external task log with the given id was last executed. Returns null
	  /// when the historic external task log contains no error details.
	  /// </summary>
	  /// <param name="historicExternalTaskLogId"> id of the historic external task log, cannot be null. </param>
	  /// <exception cref="ProcessEngineException"> when no historic external task log exists with the given id.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///          If the user has no <seealso cref="Permissions#READ_HISTORY"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
	  /// 
	  /// @since 7.7 </exception>
	  string getHistoricExternalTaskLogErrorDetails(string historicExternalTaskLogId);

	  /// <summary>
	  /// <para>Set a removal time to historic process instances and
	  /// all associated historic entities using a fluent builder.
	  /// 
	  /// </para>
	  /// <para>Historic process instances can be specified by passing a query to
	  /// <seealso cref="SetRemovalTimeToHistoricProcessInstancesBuilder#byQuery(HistoricProcessInstanceQuery)"/>.
	  /// 
	  /// </para>
	  /// <para>An absolute time can be specified via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder#absoluteRemovalTime(Date)"/>.
	  /// Pass {@code null} to clear the removal time.
	  /// 
	  /// </para>
	  /// <para>As an alternative, the removal time can also be calculated via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder#calculatedRemovalTime()"/>
	  /// based on the configured time to live values.
	  /// 
	  /// </para>
	  /// <para>To additionally take those historic process instances into account that are part of
	  /// a hierarchy, enable the flag
	  /// <seealso cref="SetRemovalTimeToHistoricProcessInstancesBuilder#hierarchical()"/>
	  /// 
	  /// </para>
	  /// <para>To create the batch and complete the configuration chain, call
	  /// <seealso cref="SetRemovalTimeToHistoricProcessInstancesBuilder#executeAsync()"/>.
	  /// 
	  /// @since 7.11
	  /// </para>
	  /// </summary>
	  SetRemovalTimeSelectModeForHistoricProcessInstancesBuilder setRemovalTimeToHistoricProcessInstances();

	  /// <summary>
	  /// <para>Set a removal time to historic decision instances and
	  /// all associated historic entities using a fluent builder.
	  /// 
	  /// </para>
	  /// <para>Historic decision instances can be specified by passing a query to
	  /// <seealso cref="SetRemovalTimeToHistoricDecisionInstancesBuilder#byQuery(HistoricDecisionInstanceQuery)"/>.
	  /// 
	  /// </para>
	  /// <para>An absolute time can be specified via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder#absoluteRemovalTime(Date)"/>.
	  /// Pass {@code null} to clear the removal time.
	  /// 
	  /// </para>
	  /// <para>As an alternative, the removal time can also be calculated via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder#calculatedRemovalTime()"/>
	  /// based on the configured time to live values.
	  /// 
	  /// </para>
	  /// <para>To additionally take those historic decision instances into account that are part of
	  /// a hierarchy, enable the flag
	  /// <seealso cref="SetRemovalTimeToHistoricProcessInstancesBuilder#hierarchical()"/>
	  /// 
	  /// </para>
	  /// <para>To create the batch and complete the configuration chain, call
	  /// <seealso cref="SetRemovalTimeToHistoricDecisionInstancesBuilder#executeAsync()"/>.
	  /// 
	  /// @since 7.11
	  /// </para>
	  /// </summary>
	  SetRemovalTimeSelectModeForHistoricDecisionInstancesBuilder setRemovalTimeToHistoricDecisionInstances();

	  /// <summary>
	  /// <para>Set a removal time to historic batches and all
	  /// associated historic entities using a fluent builder.
	  /// 
	  /// </para>
	  /// <para>Historic batches can be specified by passing a query to
	  /// <seealso cref="SetRemovalTimeToHistoricBatchesBuilder#byQuery(HistoricBatchQuery)"/>.
	  /// 
	  /// </para>
	  /// <para>An absolute time can be specified via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricBatchesBuilder#absoluteRemovalTime(Date)"/>.
	  /// Pass {@code null} to clear the removal time.
	  /// 
	  /// </para>
	  /// <para>As an alternative, the removal time can also be calculated via
	  /// <seealso cref="SetRemovalTimeSelectModeForHistoricBatchesBuilder#calculatedRemovalTime()"/>
	  /// based on the configured time to live values.
	  /// 
	  /// </para>
	  /// <para>To create the batch and complete the configuration chain, call
	  /// <seealso cref="SetRemovalTimeToHistoricBatchesBuilder#executeAsync()"/>.
	  /// 
	  /// @since 7.11
	  /// </para>
	  /// </summary>
	  SetRemovalTimeSelectModeForHistoricBatchesBuilder setRemovalTimeToHistoricBatches();

	}

}