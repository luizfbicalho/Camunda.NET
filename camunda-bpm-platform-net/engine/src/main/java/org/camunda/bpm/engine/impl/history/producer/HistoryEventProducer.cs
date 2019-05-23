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
namespace org.camunda.bpm.engine.impl.history.producer
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using UserOperationLogContext = org.camunda.bpm.engine.impl.oplog.UserOperationLogContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

	/// <summary>
	/// <para>The producer for history events. The history event producer is
	/// responsible for extracting data from the runtime structures
	/// (Executions, Tasks, ...) and adding the data to a <seealso cref="HistoryEvent"/>.
	/// 
	/// @author Daniel Meyer
	/// @author Marcel Wieczorek
	/// @author Ingo Richtsmeier
	/// 
	/// </para>
	/// </summary>
	public interface HistoryEventProducer
	{

	  // Process instances //////////////////////////////////////

	  /// <summary>
	  /// Creates the history event fired when a process instances is <strong>created</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createProcessInstanceStartEvt(DelegateExecution execution);

	  /// <summary>
	  /// Creates the history event fired when a process instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="execution"> the process instance </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createProcessInstanceUpdateEvt(DelegateExecution execution);

	  /// <summary>
	  /// Creates the history event fired when a process instance is <strong>migrated</strong>.
	  /// </summary>
	  /// <param name="execution"> the process instance </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createProcessInstanceMigrateEvt(DelegateExecution execution);

	  /// <summary>
	  /// Creates the history event fired when a process instance is <strong>ended</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createProcessInstanceEndEvt(DelegateExecution execution);

	  // Activity instances /////////////////////////////////////

	  /// <summary>
	  /// Creates the history event fired when an activity instance is <strong>started</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createActivityInstanceStartEvt(DelegateExecution execution);

	  /// <summary>
	  /// Creates the history event fired when an activity instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createActivityInstanceUpdateEvt(DelegateExecution execution);

	  /// <summary>
	  /// Creates the history event fired when an activity instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <param name="task"> the task association that is currently updated. (May be null in case there is not task associated.) </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createActivityInstanceUpdateEvt(DelegateExecution execution, DelegateTask task);

	  /// <summary>
	  /// Creates the history event which is fired when an activity instance is migrated.
	  /// </summary>
	  /// <param name="actInstance"> the migrated activity instance which contains the new id's </param>
	  /// <returns> the created history event </returns>
	  HistoryEvent createActivityInstanceMigrateEvt(MigratingActivityInstance actInstance);

	  /// <summary>
	  /// Creates the history event fired when an activity instance is <strong>ended</strong>.
	  /// </summary>
	  /// <param name="execution"> the current execution. </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createActivityInstanceEndEvt(DelegateExecution execution);


	  // Task Instances /////////////////////////////////////////

	  /// <summary>
	  /// Creates the history event fired when a task instance is <strong>created</strong>.
	  /// </summary>
	  /// <param name="task"> the task </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createTaskInstanceCreateEvt(DelegateTask task);

	  /// <summary>
	  /// Creates the history event fired when a task instance is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="task"> the task </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createTaskInstanceUpdateEvt(DelegateTask task);

	  /// <summary>
	  /// Creates the history event fired when a task instance is <strong>migrated</strong>.
	  /// </summary>
	  /// <param name="task"> the task </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createTaskInstanceMigrateEvt(DelegateTask task);

	  /// <summary>
	  /// Creates the history event fired when a task instances is <strong>completed</strong>.
	  /// </summary>
	  /// <param name="task"> the task </param>
	  /// <param name="deleteReason"> </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createTaskInstanceCompleteEvt(DelegateTask task, string deleteReason);

	  // User Operation Logs ///////////////////////////////

	  /// <summary>
	  /// Creates the history event fired whenever an operation has been performed by a user. This is
	  /// used for logging actions such as creating a new Task, completing a task, canceling a
	  /// a process instance, ...
	  /// </summary>
	  /// <param name="context"> the <seealso cref="UserOperationLogContext"/> providing the needed informations </param>
	  /// <returns> a <seealso cref="System.Collections.IList"/> of <seealso cref="HistoryEvent"/>s </returns>
	  IList<HistoryEvent> createUserOperationLogEvents(UserOperationLogContext context);

	  // HistoricVariableUpdateEventEntity //////////////////////

	  /// <summary>
	  /// Creates the history event fired when a variable is <strong>created</strong>.
	  /// </summary>
	  /// <param name="variableInstance"> the runtime variable instance </param>
	  /// <param name="the"> scope to which the variable is linked </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createHistoricVariableCreateEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope);

	  /// <summary>
	  /// Creates the history event fired when a variable is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="variableInstance"> the runtime variable instance </param>
	  /// <param name="the"> scope to which the variable is linked </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createHistoricVariableUpdateEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope);

	  /// <summary>
	  /// Creates the history event fired when a variable is <strong>migrated</strong>.
	  /// </summary>
	  /// <param name="variableInstance"> the runtime variable instance </param>
	  /// <param name="the"> scope to which the variable is linked </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createHistoricVariableMigrateEvt(VariableInstanceEntity variableInstance);

	  /// <summary>
	  /// Creates the history event fired when a variable is <strong>deleted</strong>.
	  /// </summary>
	  /// <param name="variableInstance"> </param>
	  /// <param name="variableScopeImpl"> </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createHistoricVariableDeleteEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope);

	  // Form properties //////////////////////////////////////////

	  /// <summary>
	  /// Creates the history event fired when a form property is <strong>updated</strong>.
	  /// </summary>
	  /// <param name="processInstance"> the id for the process instance </param>
	  /// <param name="propertyId"> the id of the form property </param>
	  /// <param name="propertyValue"> the value of the form property </param>
	  /// <param name="taskId"> </param>
	  /// <returns> the history event </returns>
	  HistoryEvent createFormPropertyUpdateEvt(ExecutionEntity execution, string propertyId, string propertyValue, string taskId);

	  // Incidents //////////////////////////////////////////

	  HistoryEvent createHistoricIncidentCreateEvt(Incident incident);

	  HistoryEvent createHistoricIncidentResolveEvt(Incident incident);

	  HistoryEvent createHistoricIncidentDeleteEvt(Incident incident);

	  HistoryEvent createHistoricIncidentMigrateEvt(Incident incident);

	  // Job Log ///////////////////////////////////////////

	  /// <summary>
	  /// Creates the history event fired when a job has been <strong>created</strong>.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoryEvent createHistoricJobLogCreateEvt(Job job);

	  /// <summary>
	  /// Creates the history event fired when the execution of a job <strong>failed</strong>.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoryEvent createHistoricJobLogFailedEvt(Job job, Exception exception);

	  /// <summary>
	  /// Creates the history event fired when the execution of a job was <strong>successful</strong>.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoryEvent createHistoricJobLogSuccessfulEvt(Job job);

	  /// <summary>
	  /// Creates the history event fired when the a job has been <strong>deleted</strong>.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  HistoryEvent createHistoricJobLogDeleteEvt(Job job);

	  /// <summary>
	  /// Creates the history event fired when the a batch has been <strong>started</strong>.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoryEvent createBatchStartEvent(Batch batch);


	  /// <summary>
	  /// Creates the history event fired when the a batch has been <strong>completed</strong>.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  HistoryEvent createBatchEndEvent(Batch batch);

	  /// <summary>
	  /// Fired when an identity link is added </summary>
	  /// <param name="identitylink">
	  /// @return </param>
	  HistoryEvent createHistoricIdentityLinkAddEvent(IdentityLink identitylink);

	  /// <summary>
	  /// Fired when an identity links is deleted </summary>
	  /// <param name="identityLink">
	  /// @return </param>
	  HistoryEvent createHistoricIdentityLinkDeleteEvent(IdentityLink identityLink);

	  /// <summary>
	  /// Creates the history event when an external task has been <strong>created</strong>.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  HistoryEvent createHistoricExternalTaskLogCreatedEvt(ExternalTask task);

	  /// <summary>
	  /// Creates the history event when the execution of an external task has <strong>failed</strong>.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  HistoryEvent createHistoricExternalTaskLogFailedEvt(ExternalTask task);

	  /// <summary>
	  /// Creates the history event when the execution of an external task was <strong>successful</strong>.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  HistoryEvent createHistoricExternalTaskLogSuccessfulEvt(ExternalTask task);

	  /// <summary>
	  /// Creates the history event when an external task has been <strong>deleted</strong>.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  HistoryEvent createHistoricExternalTaskLogDeletedEvt(ExternalTask task);

	}

}