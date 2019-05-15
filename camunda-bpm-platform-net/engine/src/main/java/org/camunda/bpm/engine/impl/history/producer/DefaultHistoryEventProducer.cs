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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ExceptionUtil.createJobExceptionByteArray;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskState = org.camunda.bpm.engine.history.ExternalTaskState;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using IncidentState = org.camunda.bpm.engine.history.IncidentState;
	using JobState = org.camunda.bpm.engine.history.JobState;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using org.camunda.bpm.engine.impl.history.@event;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using UserOperationLogContext = org.camunda.bpm.engine.impl.oplog.UserOperationLogContext;
	using UserOperationLogContextEntry = org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntry;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using CompensationBehavior = org.camunda.bpm.engine.impl.pvm.runtime.CompensationBehavior;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ExceptionUtil.getExceptionStacktrace;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.StringUtil.toByteArray;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Ingo Richtsmeier
	/// 
	/// </summary>
	public class DefaultHistoryEventProducer : HistoryEventProducer
	{

	  protected internal virtual void initActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, ExecutionEntity execution, HistoryEventType eventType)
	  {
		PvmScope eventSource = execution.getActivity();
		if (eventSource == null)
		{
		  eventSource = (PvmScope) execution.EventSource;
		}
		string activityInstanceId = execution.ActivityInstanceId;

		string parentActivityInstanceId = null;
		ExecutionEntity parentExecution = execution.Parent;

		if (parentExecution != null && CompensationBehavior.isCompensationThrowing(parentExecution) && execution.getActivity() != null)
		{
		  parentActivityInstanceId = CompensationBehavior.getParentActivityInstanceId(execution);
		}
		else
		{
		  parentActivityInstanceId = execution.ParentActivityInstanceId;
		}

		initActivityInstanceEvent(evt, execution, eventSource, activityInstanceId, parentActivityInstanceId, eventType);
	  }

	  protected internal virtual void initActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, MigratingActivityInstance migratingActivityInstance, HistoryEventType eventType)
	  {
		PvmScope eventSource = migratingActivityInstance.TargetScope;
		string activityInstanceId = migratingActivityInstance.ActivityInstanceId;

		MigratingActivityInstance parentInstance = migratingActivityInstance.getParent();
		string parentActivityInstanceId = null;
		if (parentInstance != null)
		{
		  parentActivityInstanceId = parentInstance.ActivityInstanceId;
		}

		ExecutionEntity execution = migratingActivityInstance.resolveRepresentativeExecution();

		initActivityInstanceEvent(evt, execution, eventSource, activityInstanceId, parentActivityInstanceId, eventType);
	  }

	  protected internal virtual void initActivityInstanceEvent(HistoricActivityInstanceEventEntity evt, ExecutionEntity execution, PvmScope eventSource, string activityInstanceId, string parentActivityInstanceId, HistoryEventType eventType)
	  {

		evt.Id = activityInstanceId;
		evt.EventType = eventType.EventName;
		evt.ActivityInstanceId = activityInstanceId;
		evt.ParentActivityInstanceId = parentActivityInstanceId;
		evt.ProcessDefinitionId = execution.ProcessDefinitionId;
		evt.ProcessInstanceId = execution.ProcessInstanceId;
		evt.ExecutionId = execution.Id;
		evt.TenantId = execution.TenantId;
		evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(evt);
		}

		ProcessDefinitionEntity definition = execution.getProcessDefinition();
		if (definition != null)
		{
		  evt.ProcessDefinitionKey = definition.Key;
		}

		evt.ActivityId = eventSource.Id;
		evt.ActivityName = (string) eventSource.getProperty("name");
		evt.ActivityType = (string) eventSource.getProperty("type");

		// update sub process reference
		ExecutionEntity subProcessInstance = execution.getSubProcessInstance();
		if (subProcessInstance != null)
		{
		  evt.CalledProcessInstanceId = subProcessInstance.Id;
		}

		// update sub case reference
		CaseExecutionEntity subCaseInstance = execution.getSubCaseInstance();
		if (subCaseInstance != null)
		{
		  evt.CalledCaseInstanceId = subCaseInstance.Id;
		}
	  }


	  protected internal virtual void initProcessInstanceEvent(HistoricProcessInstanceEventEntity evt, ExecutionEntity execution, HistoryEventType eventType)
	  {

		string processDefinitionId = execution.ProcessDefinitionId;
		string processInstanceId = execution.ProcessInstanceId;
		string executionId = execution.Id;
		// the given execution is the process instance!
		string caseInstanceId = execution.CaseInstanceId;
		string tenantId = execution.TenantId;

		ProcessDefinitionEntity definition = execution.getProcessDefinition();
		string processDefinitionKey = null;
		if (definition != null)
		{
		  processDefinitionKey = definition.Key;
		}

		evt.Id = processInstanceId;
		evt.EventType = eventType.EventName;
		evt.ProcessDefinitionKey = processDefinitionKey;
		evt.ProcessDefinitionId = processDefinitionId;
		evt.ProcessInstanceId = processInstanceId;
		evt.ExecutionId = executionId;
		evt.BusinessKey = execution.ProcessBusinessKey;
		evt.CaseInstanceId = caseInstanceId;
		evt.TenantId = tenantId;
		evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		if (execution.getSuperCaseExecution() != null)
		{
		  evt.SuperCaseInstanceId = execution.getSuperCaseExecution().CaseInstanceId;
		}
		if (execution.getSuperExecution() != null)
		{
		  evt.SuperProcessInstanceId = execution.getSuperExecution().ProcessInstanceId;
		}
	  }

	  protected internal virtual void initTaskInstanceEvent(HistoricTaskInstanceEventEntity evt, TaskEntity taskEntity, HistoryEventType eventType)
	  {

		string processDefinitionKey = null;
		ProcessDefinitionEntity definition = taskEntity.ProcessDefinition;
		if (definition != null)
		{
		  processDefinitionKey = definition.Key;
		}

		string processDefinitionId = taskEntity.ProcessDefinitionId;
		string processInstanceId = taskEntity.ProcessInstanceId;
		string executionId = taskEntity.ExecutionId;

		string caseDefinitionKey = null;
		CaseDefinitionEntity caseDefinition = taskEntity.CaseDefinition;
		if (caseDefinition != null)
		{
		  caseDefinitionKey = caseDefinition.Key;
		}

		string caseDefinitionId = taskEntity.CaseDefinitionId;
		string caseExecutionId = taskEntity.CaseExecutionId;
		string caseInstanceId = taskEntity.CaseInstanceId;
		string tenantId = taskEntity.TenantId;

		evt.Id = taskEntity.Id;
		evt.EventType = eventType.EventName;
		evt.TaskId = taskEntity.Id;

		evt.ProcessDefinitionKey = processDefinitionKey;
		evt.ProcessDefinitionId = processDefinitionId;
		evt.ProcessInstanceId = processInstanceId;
		evt.ExecutionId = executionId;

		evt.CaseDefinitionKey = caseDefinitionKey;
		evt.CaseDefinitionId = caseDefinitionId;
		evt.CaseExecutionId = caseExecutionId;
		evt.CaseInstanceId = caseInstanceId;

		evt.Assignee = taskEntity.Assignee;
		evt.Description = taskEntity.Description;
		evt.DueDate = taskEntity.DueDate;
		evt.FollowUpDate = taskEntity.FollowUpDate;
		evt.Name = taskEntity.Name;
		evt.Owner = taskEntity.Owner;
		evt.ParentTaskId = taskEntity.ParentTaskId;
		evt.Priority = taskEntity.Priority;
		evt.TaskDefinitionKey = taskEntity.TaskDefinitionKey;
		evt.TenantId = tenantId;

		ExecutionEntity execution = taskEntity.getExecution();
		if (execution != null)
		{
		  evt.ActivityInstanceId = execution.ActivityInstanceId;
		  evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			provideRemovalTime(evt);
		  }
		}

	  }

	  protected internal virtual void initHistoricVariableUpdateEvt(HistoricVariableUpdateEventEntity evt, VariableInstanceEntity variableInstance, HistoryEventType eventType)
	  {

		// init properties
		evt.EventType = eventType.EventName;
		evt.Timestamp = ClockUtil.CurrentTime;
		evt.VariableInstanceId = variableInstance.Id;
		evt.ProcessInstanceId = variableInstance.ProcessInstanceId;
		evt.ExecutionId = variableInstance.ExecutionId;
		evt.CaseInstanceId = variableInstance.CaseInstanceId;
		evt.CaseExecutionId = variableInstance.CaseExecutionId;
		evt.TaskId = variableInstance.TaskId;
		evt.Revision = variableInstance.Revision;
		evt.VariableName = variableInstance.Name;
		evt.SerializerName = variableInstance.SerializerName;
		evt.TenantId = variableInstance.TenantId;
		evt.UserOperationId = Context.CommandContext.OperationId;

		ExecutionEntity execution = variableInstance.Execution;
		if (execution != null)
		{
		  ProcessDefinitionEntity definition = execution.getProcessDefinition();
		  if (definition != null)
		  {
			evt.ProcessDefinitionId = definition.Id;
			evt.ProcessDefinitionKey = definition.Key;
		  }
		  evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			provideRemovalTime(evt);
		  }
		}

		CaseExecutionEntity caseExecution = variableInstance.CaseExecution;
		if (caseExecution != null)
		{
		  CaseDefinitionEntity definition = (CaseDefinitionEntity) caseExecution.CaseDefinition;
		  if (definition != null)
		  {
			evt.CaseDefinitionId = definition.Id;
			evt.CaseDefinitionKey = definition.Key;
		  }
		}

		// copy value
		evt.TextValue = variableInstance.TextValue;
		evt.TextValue2 = variableInstance.TextValue2;
		evt.DoubleValue = variableInstance.DoubleValue;
		evt.LongValue = variableInstance.LongValue;
		if (!string.ReferenceEquals(variableInstance.ByteArrayValueId, null))
		{
		  evt.ByteValue = variableInstance.ByteArrayValue;
		}
	  }

	  protected internal virtual void initUserOperationLogEvent(UserOperationLogEntryEventEntity evt, UserOperationLogContext context, UserOperationLogContextEntry contextEntry, PropertyChange propertyChange)
	  {
		// init properties
		evt.DeploymentId = contextEntry.DeploymentId;
		evt.EntityType = contextEntry.EntityType;
		evt.OperationType = contextEntry.OperationType;
		evt.OperationId = context.OperationId;
		evt.UserId = context.UserId;
		evt.ProcessDefinitionId = contextEntry.ProcessDefinitionId;
		evt.ProcessDefinitionKey = contextEntry.ProcessDefinitionKey;
		evt.ProcessInstanceId = contextEntry.ProcessInstanceId;
		evt.ExecutionId = contextEntry.ExecutionId;
		evt.CaseDefinitionId = contextEntry.CaseDefinitionId;
		evt.CaseInstanceId = contextEntry.CaseInstanceId;
		evt.CaseExecutionId = contextEntry.CaseExecutionId;
		evt.TaskId = contextEntry.TaskId;
		evt.JobId = contextEntry.JobId;
		evt.JobDefinitionId = contextEntry.JobDefinitionId;
		evt.BatchId = contextEntry.BatchId;
		evt.Category = contextEntry.Category;
		evt.Timestamp = ClockUtil.CurrentTime;
		evt.RootProcessInstanceId = contextEntry.RootProcessInstanceId;
		evt.ExternalTaskId = contextEntry.ExternalTaskId;

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(evt);
		}

		// init property value
		evt.Property = propertyChange.PropertyName;
		evt.OrgValue = propertyChange.OrgValueString;
		evt.NewValue = propertyChange.NewValueString;
	  }

	  protected internal virtual void initHistoricIncidentEvent(HistoricIncidentEventEntity evt, Incident incident, HistoryEventType eventType)
	  {
		// init properties
		evt.Id = incident.Id;
		evt.ProcessDefinitionId = incident.ProcessDefinitionId;
		evt.ProcessInstanceId = incident.ProcessInstanceId;
		evt.ExecutionId = incident.ExecutionId;
		evt.CreateTime = incident.IncidentTimestamp;
		evt.IncidentType = incident.IncidentType;
		evt.ActivityId = incident.ActivityId;
		evt.CauseIncidentId = incident.CauseIncidentId;
		evt.RootCauseIncidentId = incident.RootCauseIncidentId;
		evt.Configuration = incident.Configuration;
		evt.IncidentMessage = incident.IncidentMessage;
		evt.TenantId = incident.TenantId;
		evt.JobDefinitionId = incident.JobDefinitionId;

		string jobId = incident.Configuration;
		if (!string.ReferenceEquals(jobId, null) && HistoryRemovalTimeStrategyStart)
		{
		  HistoricBatchEntity historicBatch = getHistoricBatchByJobId(jobId);
		  if (historicBatch != null)
		  {
			evt.RemovalTime = historicBatch.RemovalTime;
		  }
		}

		IncidentEntity incidentEntity = (IncidentEntity) incident;
		ProcessDefinitionEntity definition = incidentEntity.ProcessDefinition;
		if (definition != null)
		{
		  evt.ProcessDefinitionKey = definition.Key;
		}

		ExecutionEntity execution = incidentEntity.Execution;
		if (execution != null)
		{
		  evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			provideRemovalTime(evt);
		  }
		}

		// init event type
		evt.EventType = eventType.EventName;

		// init state
		IncidentState incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.DEFAULT;
		if (HistoryEventTypes.INCIDENT_DELETE.Equals(eventType))
		{
		  incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.DELETED;
		}
		else if (HistoryEventTypes.INCIDENT_RESOLVE.Equals(eventType))
		{
		  incidentState = org.camunda.bpm.engine.history.IncidentState_Fields.RESOLVED;
		}
		evt.IncidentState = incidentState.StateCode;
	  }

	  protected internal virtual HistoryEvent createHistoricVariableEvent(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope, HistoryEventType eventType)
	  {
		string scopeActivityInstanceId = null;
		string sourceActivityInstanceId = null;

		if (!string.ReferenceEquals(variableInstance.ExecutionId, null))
		{
		  ExecutionEntity scopeExecution = Context.CommandContext.DbEntityManager.selectById(typeof(ExecutionEntity), variableInstance.ExecutionId);

		  if (string.ReferenceEquals(variableInstance.TaskId, null) && !variableInstance.ConcurrentLocal)
		  {
			scopeActivityInstanceId = scopeExecution.ParentActivityInstanceId;

		  }
		  else
		  {
			scopeActivityInstanceId = scopeExecution.ActivityInstanceId;
		  }
		}
		else if (!string.ReferenceEquals(variableInstance.CaseExecutionId, null))
		{
		  scopeActivityInstanceId = variableInstance.CaseExecutionId;
		}

		ExecutionEntity sourceExecution = null;
		CaseExecutionEntity sourceCaseExecution = null;
		if (sourceVariableScope is ExecutionEntity)
		{
		  sourceExecution = (ExecutionEntity) sourceVariableScope;
		  sourceActivityInstanceId = sourceExecution.ActivityInstanceId;

		}
		else if (sourceVariableScope is TaskEntity)
		{
		  sourceExecution = ((TaskEntity) sourceVariableScope).getExecution();
		  if (sourceExecution != null)
		  {
			sourceActivityInstanceId = sourceExecution.ActivityInstanceId;
		  }
		  else
		  {
			sourceCaseExecution = ((TaskEntity) sourceVariableScope).getCaseExecution();
			if (sourceCaseExecution != null)
			{
			  sourceActivityInstanceId = sourceCaseExecution.Id;
			}
		  }
		}
		else if (sourceVariableScope is CaseExecutionEntity)
		{
		  sourceCaseExecution = (CaseExecutionEntity) sourceVariableScope;
		  sourceActivityInstanceId = sourceCaseExecution.Id;
		}

		// create event
		HistoricVariableUpdateEventEntity evt = newVariableUpdateEventEntity(sourceExecution);
		// initialize
		initHistoricVariableUpdateEvt(evt, variableInstance, eventType);
		// initialize sequence counter
		initSequenceCounter(variableInstance, evt);

		// set scope activity instance id
		evt.ScopeActivityInstanceId = scopeActivityInstanceId;

		// set source activity instance id
		evt.ActivityInstanceId = sourceActivityInstanceId;

		return evt;
	  }

	  // event instance factory ////////////////////////

	  protected internal virtual HistoricProcessInstanceEventEntity newProcessInstanceEventEntity(ExecutionEntity execution)
	  {
		return new HistoricProcessInstanceEventEntity();
	  }

	  protected internal virtual HistoricActivityInstanceEventEntity newActivityInstanceEventEntity(ExecutionEntity execution)
	  {
		return new HistoricActivityInstanceEventEntity();
	  }

	  protected internal virtual HistoricTaskInstanceEventEntity newTaskInstanceEventEntity(DelegateTask task)
	  {
		return new HistoricTaskInstanceEventEntity();
	  }

	  protected internal virtual HistoricVariableUpdateEventEntity newVariableUpdateEventEntity(ExecutionEntity execution)
	  {
		return new HistoricVariableUpdateEventEntity();
	  }

	  protected internal virtual HistoricFormPropertyEventEntity newHistoricFormPropertyEvent()
	  {
		return new HistoricFormPropertyEventEntity();
	  }

	  protected internal virtual HistoricIncidentEventEntity newIncidentEventEntity(Incident incident)
	  {
		return new HistoricIncidentEventEntity();
	  }

	  protected internal virtual HistoricJobLogEventEntity newHistoricJobLogEntity(Job job)
	  {
		return new HistoricJobLogEventEntity();
	  }

	  protected internal virtual HistoricBatchEntity newBatchEventEntity(BatchEntity batch)
	  {
		return new HistoricBatchEntity();
	  }

	  protected internal virtual HistoricProcessInstanceEventEntity loadProcessInstanceEventEntity(ExecutionEntity execution)
	  {
		return newProcessInstanceEventEntity(execution);
	  }

	  protected internal virtual HistoricActivityInstanceEventEntity loadActivityInstanceEventEntity(ExecutionEntity execution)
	  {
		return newActivityInstanceEventEntity(execution);
	  }

	  protected internal virtual HistoricTaskInstanceEventEntity loadTaskInstanceEvent(DelegateTask task)
	  {
		return newTaskInstanceEventEntity(task);
	  }

	  protected internal virtual HistoricIncidentEventEntity loadIncidentEvent(Incident incident)
	  {
		return newIncidentEventEntity(incident);
	  }

	  protected internal virtual HistoricBatchEntity loadBatchEntity(BatchEntity batch)
	  {
		return newBatchEventEntity(batch);
	  }

	  // Implementation ////////////////////////////////

	  public virtual HistoryEvent createProcessInstanceStartEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricProcessInstanceEventEntity evt = newProcessInstanceEventEntity(executionEntity);

		// initialize event
		initProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.PROCESS_INSTANCE_START);

		evt.StartActivityId = executionEntity.ActivityId;
		evt.StartTime = ClockUtil.CurrentTime;

		// set super process instance id
		ExecutionEntity superExecution = executionEntity.getSuperExecution();
		if (superExecution != null)
		{
		  evt.SuperProcessInstanceId = superExecution.ProcessInstanceId;
		}

		//state
		evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE;

		// set start user Id
		evt.StartUserId = Context.CommandContext.AuthenticatedUserId;

		if (HistoryRemovalTimeStrategyStart)
		{
		  if (isRootProcessInstance(evt))
		  {
			DateTime removalTime = calculateRemovalTime(evt);
			evt.RemovalTime = removalTime;
		  }
		  else
		  {
			provideRemovalTime(evt);
		  }
		}

		return evt;
	  }

	  public virtual HistoryEvent createProcessInstanceUpdateEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricProcessInstanceEventEntity evt = loadProcessInstanceEventEntity(executionEntity);

		// initialize event
		initProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.PROCESS_INSTANCE_UPDATE);

		if (executionEntity.Suspended)
		{
		  evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED;
		}
		else
		{
		  evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE;
		}

		return evt;
	  }

	  public virtual HistoryEvent createProcessInstanceMigrateEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricProcessInstanceEventEntity evt = newProcessInstanceEventEntity(executionEntity);

		// initialize event
		initProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.PROCESS_INSTANCE_MIGRATE);

		return evt;
	  }

	  public virtual HistoryEvent createProcessInstanceEndEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricProcessInstanceEventEntity evt = loadProcessInstanceEventEntity(executionEntity);

		// initialize event
		initProcessInstanceEvent(evt, executionEntity, HistoryEventTypes.PROCESS_INSTANCE_END);

		determineEndState(executionEntity, evt);

		// set end activity id
		evt.EndActivityId = executionEntity.ActivityId;
		evt.EndTime = ClockUtil.CurrentTime;

		if (evt.StartTime != null)
		{
		  evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;
		}

		if (isRootProcessInstance(evt) && HistoryRemovalTimeStrategyEnd)
		{
		  DateTime removalTime = calculateRemovalTime(evt);

		  if (removalTime != null)
		  {
			addRemovalTimeToHistoricProcessInstances(evt.RootProcessInstanceId, removalTime);

			if (DmnEnabled)
			{
			  addRemovalTimeToHistoricDecisions(evt.RootProcessInstanceId, removalTime);
			}
		  }
		}

		// set delete reason (if applicable).
		if (!string.ReferenceEquals(executionEntity.DeleteReason, null))
		{
		  evt.DeleteReason = executionEntity.DeleteReason;
		}

		return evt;
	  }

	  protected internal virtual void addRemovalTimeToHistoricDecisions(string rootProcessInstanceId, DateTime removalTime)
	  {
		Context.CommandContext.HistoricDecisionInstanceManager.addRemovalTimeToDecisionsByRootProcessInstanceId(rootProcessInstanceId, removalTime);
	  }

	  protected internal virtual void addRemovalTimeToHistoricProcessInstances(string rootProcessInstanceId, DateTime removalTime)
	  {
		Context.CommandContext.HistoricProcessInstanceManager.addRemovalTimeToProcessInstancesByRootProcessInstanceId(rootProcessInstanceId, removalTime);
	  }

	  protected internal virtual bool DmnEnabled
	  {
		  get
		  {
			return Context.CommandContext.ProcessEngineConfiguration.DmnEnabled;
		  }
	  }

	  protected internal virtual void determineEndState(ExecutionEntity executionEntity, HistoricProcessInstanceEventEntity evt)
	  {
		//determine state
		if (executionEntity.getActivity() != null)
		{
		  evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_COMPLETED;
		}
		else if (executionEntity.getActivity() == null && executionEntity.ExternallyTerminated)
		{
		  evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_EXTERNALLY_TERMINATED;
		}
		else if (executionEntity.getActivity() == null && !executionEntity.ExternallyTerminated)
		{
		  evt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_INTERNALLY_TERMINATED;
		}
	  }

	  public virtual HistoryEvent createActivityInstanceStartEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricActivityInstanceEventEntity evt = newActivityInstanceEventEntity(executionEntity);

		// initialize event
		initActivityInstanceEvent(evt, executionEntity, HistoryEventTypes.ACTIVITY_INSTANCE_START);

		// initialize sequence counter
		initSequenceCounter(executionEntity, evt);

		evt.StartTime = ClockUtil.CurrentTime;

		return evt;
	  }

	  public virtual HistoryEvent createActivityInstanceUpdateEvt(DelegateExecution execution)
	  {
		return createActivityInstanceUpdateEvt(execution, null);
	  }

	  public virtual HistoryEvent createActivityInstanceUpdateEvt(DelegateExecution execution, DelegateTask task)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricActivityInstanceEventEntity evt = loadActivityInstanceEventEntity(executionEntity);

		// initialize event
		initActivityInstanceEvent(evt, executionEntity, HistoryEventTypes.ACTIVITY_INSTANCE_UPDATE);

		// update task assignment
		if (task != null)
		{
		  evt.TaskId = task.Id;
		  evt.TaskAssignee = task.Assignee;
		}

		return evt;
	  }

	  public virtual HistoryEvent createActivityInstanceMigrateEvt(MigratingActivityInstance actInstance)
	  {

		// create event instance
		HistoricActivityInstanceEventEntity evt = loadActivityInstanceEventEntity(actInstance.resolveRepresentativeExecution());

		// initialize event
		initActivityInstanceEvent(evt, actInstance, HistoryEventTypes.ACTIVITY_INSTANCE_MIGRATE);

		return evt;
	  }


	  public virtual HistoryEvent createActivityInstanceEndEvt(DelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution;
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		// create event instance
		HistoricActivityInstanceEventEntity evt = loadActivityInstanceEventEntity(executionEntity);
		evt.ActivityInstanceState = executionEntity.ActivityInstanceState;

		// initialize event
		initActivityInstanceEvent(evt, (ExecutionEntity) execution, HistoryEventTypes.ACTIVITY_INSTANCE_END);

		evt.EndTime = ClockUtil.CurrentTime;
		if (evt.StartTime != null)
		{
		  evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;
		}

		return evt;
	  }

	  public virtual HistoryEvent createTaskInstanceCreateEvt(DelegateTask task)
	  {

		// create event instance
		HistoricTaskInstanceEventEntity evt = newTaskInstanceEventEntity(task);

		// initialize event
		initTaskInstanceEvent(evt, (TaskEntity) task, HistoryEventTypes.TASK_INSTANCE_CREATE);

		evt.StartTime = ClockUtil.CurrentTime;

		return evt;
	  }

	  public virtual HistoryEvent createTaskInstanceUpdateEvt(DelegateTask task)
	  {

		// create event instance
		HistoricTaskInstanceEventEntity evt = loadTaskInstanceEvent(task);

		// initialize event
		initTaskInstanceEvent(evt, (TaskEntity) task, HistoryEventTypes.TASK_INSTANCE_UPDATE);

		return evt;
	  }

	  public virtual HistoryEvent createTaskInstanceMigrateEvt(DelegateTask task)
	  {
		// create event instance
		HistoricTaskInstanceEventEntity evt = loadTaskInstanceEvent(task);

		// initialize event
		initTaskInstanceEvent(evt, (TaskEntity) task, HistoryEventTypes.TASK_INSTANCE_MIGRATE);

		return evt;
	  }

	  public virtual HistoryEvent createTaskInstanceCompleteEvt(DelegateTask task, string deleteReason)
	  {

		// create event instance
		HistoricTaskInstanceEventEntity evt = loadTaskInstanceEvent(task);

		// initialize event
		initTaskInstanceEvent(evt, (TaskEntity) task, HistoryEventTypes.TASK_INSTANCE_COMPLETE);

		// set end time
		evt.EndTime = ClockUtil.CurrentTime;
		if (evt.StartTime != null)
		{
		  evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;
		}

		// set delete reason
		evt.DeleteReason = deleteReason;

		return evt;
	  }

	  // User Operation Logs ///////////////////////////

	  public virtual IList<HistoryEvent> createUserOperationLogEvents(UserOperationLogContext context)
	  {
		IList<HistoryEvent> historyEvents = new List<HistoryEvent>();

		string operationId = Context.CommandContext.OperationId;
		context.OperationId = operationId;

		foreach (UserOperationLogContextEntry entry in context.Entries)
		{
		  foreach (PropertyChange propertyChange in entry.PropertyChanges)
		  {
			UserOperationLogEntryEventEntity evt = new UserOperationLogEntryEventEntity();

			initUserOperationLogEvent(evt, context, entry, propertyChange);

			historyEvents.Add(evt);
		  }
		}

		return historyEvents;
	  }

	  // variables /////////////////////////////////

	  public virtual HistoryEvent createHistoricVariableCreateEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope)
	  {
		return createHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VARIABLE_INSTANCE_CREATE);
	  }

	  public virtual HistoryEvent createHistoricVariableDeleteEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope)
	  {
		return createHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VARIABLE_INSTANCE_DELETE);
	  }

	  public virtual HistoryEvent createHistoricVariableUpdateEvt(VariableInstanceEntity variableInstance, VariableScope sourceVariableScope)
	  {
		return createHistoricVariableEvent(variableInstance, sourceVariableScope, HistoryEventTypes.VARIABLE_INSTANCE_UPDATE);
	  }

	  public virtual HistoryEvent createHistoricVariableMigrateEvt(VariableInstanceEntity variableInstance)
	  {
		return createHistoricVariableEvent(variableInstance, null, HistoryEventTypes.VARIABLE_INSTANCE_MIGRATE);
	  }

	  // form Properties ///////////////////////////

	  public virtual HistoryEvent createFormPropertyUpdateEvt(ExecutionEntity execution, string propertyId, string propertyValue, string taskId)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.IdGenerator idGenerator = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration().getIdGenerator();
		IdGenerator idGenerator = Context.ProcessEngineConfiguration.IdGenerator;

		HistoricFormPropertyEventEntity historicFormPropertyEntity = newHistoricFormPropertyEvent();

		historicFormPropertyEntity.Id = idGenerator.NextId;
		historicFormPropertyEntity.EventType = HistoryEventTypes.FORM_PROPERTY_UPDATE.EventName;
		historicFormPropertyEntity.Timestamp = ClockUtil.CurrentTime;
		historicFormPropertyEntity.ActivityInstanceId = execution.ActivityInstanceId;
		historicFormPropertyEntity.ExecutionId = execution.Id;
		historicFormPropertyEntity.ProcessDefinitionId = execution.ProcessDefinitionId;
		historicFormPropertyEntity.ProcessInstanceId = execution.ProcessInstanceId;
		historicFormPropertyEntity.PropertyId = propertyId;
		historicFormPropertyEntity.setPropertyValue(propertyValue);
		historicFormPropertyEntity.TaskId = taskId;
		historicFormPropertyEntity.TenantId = execution.TenantId;
		historicFormPropertyEntity.UserOperationId = Context.CommandContext.OperationId;
		historicFormPropertyEntity.RootProcessInstanceId = execution.RootProcessInstanceId;

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(historicFormPropertyEntity);
		}

		ProcessDefinitionEntity definition = execution.getProcessDefinition();
		if (definition != null)
		{
		  historicFormPropertyEntity.ProcessDefinitionKey = definition.Key;
		}

		// initialize sequence counter
		initSequenceCounter(execution, historicFormPropertyEntity);

		return historicFormPropertyEntity;
	  }

	  // Incidents //////////////////////////////////

	  public virtual HistoryEvent createHistoricIncidentCreateEvt(Incident incident)
	  {
		return createHistoricIncidentEvt(incident, HistoryEventTypes.INCIDENT_CREATE);
	  }

	  public virtual HistoryEvent createHistoricIncidentResolveEvt(Incident incident)
	  {
		return createHistoricIncidentEvt(incident, HistoryEventTypes.INCIDENT_RESOLVE);
	  }

	  public virtual HistoryEvent createHistoricIncidentDeleteEvt(Incident incident)
	  {
		return createHistoricIncidentEvt(incident, HistoryEventTypes.INCIDENT_DELETE);
	  }

	  public virtual HistoryEvent createHistoricIncidentMigrateEvt(Incident incident)
	  {
		return createHistoricIncidentEvt(incident, HistoryEventTypes.INCIDENT_MIGRATE);
	  }

	  protected internal virtual HistoryEvent createHistoricIncidentEvt(Incident incident, HistoryEventTypes eventType)
	  {
		// create event
		HistoricIncidentEventEntity evt = loadIncidentEvent(incident);
		// initialize
		initHistoricIncidentEvent(evt, incident, eventType);

		if (HistoryEventTypes.INCIDENT_RESOLVE.Equals(eventType) || HistoryEventTypes.INCIDENT_DELETE.Equals(eventType))
		{
		  evt.EndTime = ClockUtil.CurrentTime;
		}

		return evt;
	  }

	  // Historic identity link
	  public virtual HistoryEvent createHistoricIdentityLinkAddEvent(IdentityLink identityLink)
	  {
		return createHistoricIdentityLinkEvt(identityLink, HistoryEventTypes.IDENTITY_LINK_ADD);
	  }

	  public virtual HistoryEvent createHistoricIdentityLinkDeleteEvent(IdentityLink identityLink)
	  {
		return createHistoricIdentityLinkEvt(identityLink, HistoryEventTypes.IDENTITY_LINK_DELETE);
	  }

	  protected internal virtual HistoryEvent createHistoricIdentityLinkEvt(IdentityLink identityLink, HistoryEventTypes eventType)
	  {
		// create historic identity link event
		HistoricIdentityLinkLogEventEntity evt = newIdentityLinkEventEntity();
		// Mapping all the values of identity link to HistoricIdentityLinkEvent
		initHistoricIdentityLinkEvent(evt, identityLink, eventType);
		return evt;
	  }

	  protected internal virtual HistoricIdentityLinkLogEventEntity newIdentityLinkEventEntity()
	  {
		return new HistoricIdentityLinkLogEventEntity();
	  }

	  protected internal virtual void initHistoricIdentityLinkEvent(HistoricIdentityLinkLogEventEntity evt, IdentityLink identityLink, HistoryEventType eventType)
	  {

		if (!string.ReferenceEquals(identityLink.TaskId, null))
		{
		  TaskEntity task = Context.CommandContext.TaskManager.findTaskById(identityLink.TaskId);

		  evt.ProcessDefinitionId = task.ProcessDefinitionId;

		  if (task.ProcessDefinition != null)
		  {
			evt.ProcessDefinitionKey = task.ProcessDefinition.Key;
		  }

		  ExecutionEntity execution = task.getExecution();
		  if (execution != null)
		  {
			evt.RootProcessInstanceId = execution.RootProcessInstanceId;

			if (HistoryRemovalTimeStrategyStart)
			{
			  provideRemovalTime(evt);
			}
		  }
		}

		if (!string.ReferenceEquals(identityLink.ProcessDefId, null))
		{
		  evt.ProcessDefinitionId = identityLink.ProcessDefId;

		  ProcessDefinitionEntity definition = Context.ProcessEngineConfiguration.DeploymentCache.findProcessDefinitionFromCache(identityLink.ProcessDefId);
		  evt.ProcessDefinitionKey = definition.Key;
		}

		evt.Time = ClockUtil.CurrentTime;
		evt.Type = identityLink.Type;
		evt.UserId = identityLink.UserId;
		evt.GroupId = identityLink.GroupId;
		evt.TaskId = identityLink.TaskId;
		evt.TenantId = identityLink.TenantId;
		// There is a conflict in HistoryEventTypes for 'delete' keyword,
		// So HistoryEventTypes.IDENTITY_LINK_ADD /
		// HistoryEventTypes.IDENTITY_LINK_DELETE is provided with the event name
		// 'add-identity-link' /'delete-identity-link'
		// and changed to 'add'/'delete' (While inserting it into the database) on
		// Historic identity link add / delete event
		string operationType = "add";
		if (eventType.EventName.Equals(HistoryEventTypes.IDENTITY_LINK_DELETE.EventName))
		{
		  operationType = "delete";
		}

		evt.OperationType = operationType;
		evt.EventType = eventType.EventName;
		evt.AssignerId = Context.CommandContext.AuthenticatedUserId;
	  }
	  // Batch

	  public virtual HistoryEvent createBatchStartEvent(Batch batch)
	  {
		HistoryEvent historicBatch = createBatchEvent((BatchEntity) batch, HistoryEventTypes.BATCH_START);

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime((HistoricBatchEntity) historicBatch);
		}

		return historicBatch;
	  }

	  public virtual HistoryEvent createBatchEndEvent(Batch batch)
	  {
		HistoryEvent historicBatch = createBatchEvent((BatchEntity) batch, HistoryEventTypes.BATCH_END);

		if (HistoryRemovalTimeStrategyEnd)
		{
		  provideRemovalTime((HistoricBatchEntity) historicBatch);

		  addRemovalTimeToHistoricJobLog((HistoricBatchEntity) historicBatch);
		  addRemovalTimeToHistoricIncidents((HistoricBatchEntity) historicBatch);
		}

		return historicBatch;
	  }

	  protected internal virtual HistoryEvent createBatchEvent(BatchEntity batch, HistoryEventTypes eventType)
	  {
		HistoricBatchEntity @event = loadBatchEntity(batch);

		@event.Id = batch.Id;
		@event.Type = batch.Type;
		@event.TotalJobs = batch.TotalJobs;
		@event.BatchJobsPerSeed = batch.BatchJobsPerSeed;
		@event.InvocationsPerBatchJob = batch.InvocationsPerBatchJob;
		@event.SeedJobDefinitionId = batch.SeedJobDefinitionId;
		@event.MonitorJobDefinitionId = batch.MonitorJobDefinitionId;
		@event.BatchJobDefinitionId = batch.BatchJobDefinitionId;
		@event.TenantId = batch.TenantId;
		@event.EventType = eventType.EventName;

		if (HistoryEventTypes.BATCH_START.Equals(eventType))
		{
		  @event.StartTime = ClockUtil.CurrentTime;
		  @event.CreateUserId = Context.CommandContext.AuthenticatedUserId;
		}

		if (HistoryEventTypes.BATCH_END.Equals(eventType))
		{
		  @event.EndTime = ClockUtil.CurrentTime;
		}

		return @event;
	  }

	  // Job Log

	  public virtual HistoryEvent createHistoricJobLogCreateEvt(Job job)
	  {
		return createHistoricJobLogEvt(job, HistoryEventTypes.JOB_CREATE);
	  }

	  public virtual HistoryEvent createHistoricJobLogFailedEvt(Job job, Exception exception)
	  {
		HistoricJobLogEventEntity @event = (HistoricJobLogEventEntity) createHistoricJobLogEvt(job, HistoryEventTypes.JOB_FAIL);

		if (exception != null)
		{
		  // exception message
		  @event.JobExceptionMessage = exception.Message;

		  // stacktrace
		  string exceptionStacktrace = getExceptionStacktrace(exception);
		  sbyte[] exceptionBytes = toByteArray(exceptionStacktrace);

		  ByteArrayEntity byteArray = createJobExceptionByteArray(exceptionBytes, ResourceTypes.HISTORY);
		  byteArray.RootProcessInstanceId = @event.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			byteArray.RemovalTime = @event.RemovalTime;
		  }

		  @event.ExceptionByteArrayId = byteArray.Id;
		}

		return @event;
	  }

	  public virtual HistoryEvent createHistoricJobLogSuccessfulEvt(Job job)
	  {
		return createHistoricJobLogEvt(job, HistoryEventTypes.JOB_SUCCESS);
	  }

	  public virtual HistoryEvent createHistoricJobLogDeleteEvt(Job job)
	  {
		return createHistoricJobLogEvt(job, HistoryEventTypes.JOB_DELETE);
	  }

	  protected internal virtual HistoryEvent createHistoricJobLogEvt(Job job, HistoryEventType eventType)
	  {
		HistoricJobLogEventEntity @event = newHistoricJobLogEntity(job);
		initHistoricJobLogEvent(@event, job, eventType);
		return @event;
	  }

	  protected internal virtual void initHistoricJobLogEvent(HistoricJobLogEventEntity evt, Job job, HistoryEventType eventType)
	  {
		evt.Timestamp = ClockUtil.CurrentTime;

		JobEntity jobEntity = (JobEntity) job;
		evt.JobId = jobEntity.Id;
		evt.JobDueDate = jobEntity.Duedate;
		evt.JobRetries = jobEntity.Retries;
		evt.JobPriority = jobEntity.Priority;

		JobDefinition jobDefinition = jobEntity.JobDefinition;
		if (jobDefinition != null)
		{
		  evt.JobDefinitionId = jobDefinition.Id;
		  evt.JobDefinitionType = jobDefinition.JobType;
		  evt.JobDefinitionConfiguration = jobDefinition.JobConfiguration;

		  string historicBatchId = jobDefinition.JobConfiguration;
		  if (!string.ReferenceEquals(historicBatchId, null) && HistoryRemovalTimeStrategyStart)
		  {
			HistoricBatchEntity historicBatch = getHistoricBatchById(historicBatchId);
			if (historicBatch != null)
			{
			  evt.RemovalTime = historicBatch.RemovalTime;
			}
		  }

		}
		else
		{
		  // in case of async signal there does not exist a job definition
		  // but we use the jobHandlerType as jobDefinitionType
		  evt.JobDefinitionType = jobEntity.JobHandlerType;
		}

		evt.ActivityId = jobEntity.ActivityId;
		evt.ExecutionId = jobEntity.ExecutionId;
		evt.ProcessInstanceId = jobEntity.ProcessInstanceId;
		evt.ProcessDefinitionId = jobEntity.ProcessDefinitionId;
		evt.ProcessDefinitionKey = jobEntity.ProcessDefinitionKey;
		evt.DeploymentId = jobEntity.DeploymentId;
		evt.TenantId = jobEntity.TenantId;

		ExecutionEntity execution = jobEntity.Execution;
		if (execution != null)
		{
		  evt.RootProcessInstanceId = execution.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			provideRemovalTime(evt);
		  }
		}

		// initialize sequence counter
		initSequenceCounter(jobEntity, evt);

		JobState state = null;
		if (HistoryEventTypes.JOB_CREATE.Equals(eventType))
		{
		  state = org.camunda.bpm.engine.history.JobState_Fields.CREATED;
		}
		else if (HistoryEventTypes.JOB_FAIL.Equals(eventType))
		{
		  state = org.camunda.bpm.engine.history.JobState_Fields.FAILED;
		}
		else if (HistoryEventTypes.JOB_SUCCESS.Equals(eventType))
		{
		  state = org.camunda.bpm.engine.history.JobState_Fields.SUCCESSFUL;
		}
		else if (HistoryEventTypes.JOB_DELETE.Equals(eventType))
		{
		  state = org.camunda.bpm.engine.history.JobState_Fields.DELETED;
		}
		evt.State = state.StateCode;
	  }

	  public virtual HistoryEvent createHistoricExternalTaskLogCreatedEvt(ExternalTask task)
	  {
		return initHistoricExternalTaskLog((ExternalTaskEntity) task, org.camunda.bpm.engine.history.ExternalTaskState_Fields.CREATED);
	  }

	  public virtual HistoryEvent createHistoricExternalTaskLogFailedEvt(ExternalTask task)
	  {
		HistoricExternalTaskLogEntity @event = initHistoricExternalTaskLog((ExternalTaskEntity) task, org.camunda.bpm.engine.history.ExternalTaskState_Fields.FAILED);
		@event.ErrorMessage = task.ErrorMessage;
		string errorDetails = ((ExternalTaskEntity) task).ErrorDetails;
		if (!string.ReferenceEquals(errorDetails, null))
		{
		  @event.ErrorDetails = errorDetails;
		}
		return @event;
	  }

	  public virtual HistoryEvent createHistoricExternalTaskLogSuccessfulEvt(ExternalTask task)
	  {
		return initHistoricExternalTaskLog((ExternalTaskEntity) task, org.camunda.bpm.engine.history.ExternalTaskState_Fields.SUCCESSFUL);
	  }

	  public virtual HistoryEvent createHistoricExternalTaskLogDeletedEvt(ExternalTask task)
	  {
		return initHistoricExternalTaskLog((ExternalTaskEntity) task, org.camunda.bpm.engine.history.ExternalTaskState_Fields.DELETED);
	  }

	  protected internal virtual HistoricExternalTaskLogEntity initHistoricExternalTaskLog(ExternalTaskEntity entity, ExternalTaskState state)
	  {
		HistoricExternalTaskLogEntity @event = new HistoricExternalTaskLogEntity();
		@event.Timestamp = ClockUtil.CurrentTime;
		@event.ExternalTaskId = entity.Id;
		@event.TopicName = entity.TopicName;
		@event.WorkerId = entity.WorkerId;

		@event.Priority = entity.Priority;
		@event.Retries = entity.Retries;

		@event.ActivityId = entity.ActivityId;
		@event.ActivityInstanceId = entity.ActivityInstanceId;
		@event.ExecutionId = entity.ExecutionId;

		@event.ProcessInstanceId = entity.ProcessInstanceId;
		@event.ProcessDefinitionId = entity.ProcessDefinitionId;
		@event.ProcessDefinitionKey = entity.ProcessDefinitionKey;
		@event.TenantId = entity.TenantId;
		@event.State = state.StateCode;

		ExecutionEntity execution = entity.Execution;
		if (execution != null)
		{
		  @event.RootProcessInstanceId = execution.RootProcessInstanceId;

		  if (HistoryRemovalTimeStrategyStart)
		  {
			provideRemovalTime(@event);
		  }
		}

		return @event;
	  }

	  protected internal virtual bool isRootProcessInstance(HistoricProcessInstanceEventEntity evt)
	  {
		return evt.ProcessInstanceId.Equals(evt.RootProcessInstanceId);
	  }

	  protected internal virtual bool HistoryRemovalTimeStrategyStart
	  {
		  get
		  {
			return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(HistoryRemovalTimeStrategy);
		  }
	  }

	  protected internal virtual bool HistoryRemovalTimeStrategyEnd
	  {
		  get
		  {
			return HISTORY_REMOVAL_TIME_STRATEGY_END.Equals(HistoryRemovalTimeStrategy);
		  }
	  }

	  protected internal virtual string HistoryRemovalTimeStrategy
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
		  }
	  }

	  protected internal virtual DateTime calculateRemovalTime(HistoryEvent historyEvent)
	  {
		string processDefinitionId = historyEvent.ProcessDefinitionId;
		ProcessDefinition processDefinition = findProcessDefinitionById(processDefinitionId);

		return Context.ProcessEngineConfiguration.HistoryRemovalTimeProvider.calculateRemovalTime((HistoricProcessInstanceEventEntity) historyEvent, processDefinition);
	  }

	  protected internal virtual DateTime calculateRemovalTime(HistoricBatchEntity historicBatch)
	  {
		return Context.ProcessEngineConfiguration.HistoryRemovalTimeProvider.calculateRemovalTime(historicBatch);
	  }

	  protected internal virtual void provideRemovalTime(HistoricBatchEntity historicBatch)
	  {
		DateTime removalTime = calculateRemovalTime(historicBatch);
		if (removalTime != null)
		{
		  historicBatch.RemovalTime = removalTime;
		}
	  }

	  protected internal virtual void provideRemovalTime(HistoryEvent historyEvent)
	  {
		string rootProcessInstanceId = historyEvent.RootProcessInstanceId;
		if (!string.ReferenceEquals(rootProcessInstanceId, null))
		{
		  HistoricProcessInstanceEventEntity historicRootProcessInstance = getHistoricRootProcessInstance(rootProcessInstanceId);

		  if (historicRootProcessInstance != null)
		  {
			DateTime removalTime = historicRootProcessInstance.RemovalTime;
			historyEvent.RemovalTime = removalTime;
		  }
		}
	  }

	  protected internal virtual HistoricProcessInstanceEventEntity getHistoricRootProcessInstance(string rootProcessInstanceId)
	  {
		return Context.CommandContext.DbEntityManager.selectById(typeof(HistoricProcessInstanceEventEntity), rootProcessInstanceId);
	  }

	  protected internal virtual ProcessDefinition findProcessDefinitionById(string processDefinitionId)
	  {
		return Context.CommandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }

	  protected internal virtual HistoricBatchEntity getHistoricBatchById(string batchId)
	  {
		return Context.CommandContext.HistoricBatchManager.findHistoricBatchById(batchId);
	  }

	  protected internal virtual HistoricBatchEntity getHistoricBatchByJobId(string jobId)
	  {
		return Context.CommandContext.HistoricBatchManager.findHistoricBatchByJobId(jobId);
	  }

	  protected internal virtual void addRemovalTimeToHistoricJobLog(HistoricBatchEntity historicBatch)
	  {
		DateTime removalTime = historicBatch.RemovalTime;
		if (removalTime != null)
		{
		  Context.CommandContext.HistoricJobLogManager.addRemovalTimeToJobLogByBatchId(historicBatch.Id, removalTime);
		}
	  }

	  protected internal virtual void addRemovalTimeToHistoricIncidents(HistoricBatchEntity historicBatch)
	  {
		DateTime removalTime = historicBatch.RemovalTime;
		if (removalTime != null)
		{
		  Context.CommandContext.HistoricIncidentManager.addRemovalTimeToHistoricIncidentsByBatchId(historicBatch.Id, removalTime);
		}
	  }

	  // sequence counter //////////////////////////////////////////////////////

	  protected internal virtual void initSequenceCounter(ExecutionEntity execution, HistoryEvent @event)
	  {
		initSequenceCounter(execution.SequenceCounter, @event);
	  }

	  protected internal virtual void initSequenceCounter(VariableInstanceEntity variable, HistoryEvent @event)
	  {
		initSequenceCounter(variable.SequenceCounter, @event);
	  }

	  protected internal virtual void initSequenceCounter(JobEntity job, HistoryEvent @event)
	  {
		initSequenceCounter(job.SequenceCounter, @event);
	  }

	  protected internal virtual void initSequenceCounter(long sequenceCounter, HistoryEvent @event)
	  {
		@event.SequenceCounter = sequenceCounter;
	  }



	}

}