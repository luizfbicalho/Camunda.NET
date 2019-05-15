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
namespace org.camunda.bpm.engine.impl.cmd
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using UpdateJobSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobSuspensionStateBuilderImpl;
	using org.camunda.bpm.engine.impl.persistence.entity;
	using UpdateProcessInstanceSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.runtime.UpdateProcessInstanceSuspensionStateBuilderImpl;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// @author roman.smirnov
	/// </summary>
	public abstract class AbstractSetProcessInstanceStateCmd : AbstractSetStateCmd
	{

	  protected internal readonly string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

	  protected internal string processDefinitionTenantId;
	  protected internal bool isProcessDefinitionTenantIdSet = false;

	  public AbstractSetProcessInstanceStateCmd(UpdateProcessInstanceSuspensionStateBuilderImpl builder) : base(true, null)
	  {

		this.processInstanceId = builder.ProcessInstanceId;
		this.processDefinitionId = builder.ProcessDefinitionId;
		this.processDefinitionKey = builder.ProcessDefinitionKey;
		this.processDefinitionTenantId = builder.ProcessDefinitionTenantId;
		this.isProcessDefinitionTenantIdSet = builder.ProcessDefinitionTenantIdSet;
	  }

	  protected internal override void checkParameters(CommandContext commandContext)
	  {
		if (string.ReferenceEquals(processInstanceId, null) && string.ReferenceEquals(processDefinitionId, null) && string.ReferenceEquals(processDefinitionKey, null))
		{
		  throw new ProcessEngineException("ProcessInstanceId, ProcessDefinitionId nor ProcessDefinitionKey cannot be null.");
		}
	  }

	  protected internal override void checkAuthorization(CommandContext commandContext)
	  {

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (!string.ReferenceEquals(processInstanceId, null))
		  {
			checker.checkUpdateProcessInstanceSuspensionStateById(processInstanceId);
		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionId, null))
		  {
			checker.checkUpdateProcessInstanceSuspensionStateByProcessDefinitionId(processDefinitionId);
		  }
		  else
		  {

		  if (!string.ReferenceEquals(processDefinitionKey, null))
		  {
			checker.checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(processDefinitionKey);
		  }
		  }
		  }
		}
	  }

	  protected internal override void updateSuspensionState(CommandContext commandContext, SuspensionState suspensionState)
	  {
		ExecutionManager executionManager = commandContext.ExecutionManager;
		TaskManager taskManager = commandContext.TaskManager;
		ExternalTaskManager externalTaskManager = commandContext.ExternalTaskManager;

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  executionManager.updateExecutionSuspensionStateByProcessInstanceId(processInstanceId, suspensionState);
		  taskManager.updateTaskSuspensionStateByProcessInstanceId(processInstanceId, suspensionState);
		  externalTaskManager.updateExternalTaskSuspensionStateByProcessInstanceId(processInstanceId, suspensionState);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  executionManager.updateExecutionSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);
		  taskManager.updateTaskSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);
		  externalTaskManager.updateExternalTaskSuspensionStateByProcessDefinitionId(processDefinitionId, suspensionState);

		}
		else if (isProcessDefinitionTenantIdSet)
		{
		  executionManager.updateExecutionSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);
		  taskManager.updateTaskSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);
		  externalTaskManager.updateExternalTaskSuspensionStateByProcessDefinitionKeyAndTenantId(processDefinitionKey, processDefinitionTenantId, suspensionState);

		}
		else
		{
		  executionManager.updateExecutionSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);
		  taskManager.updateTaskSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);
		  externalTaskManager.updateExternalTaskSuspensionStateByProcessDefinitionKey(processDefinitionKey, suspensionState);
		}
	  }

	  protected internal override void triggerHistoryEvent(CommandContext commandContext)
	  {
		HistoryLevel historyLevel = commandContext.ProcessEngineConfiguration.HistoryLevel;
		IList<ProcessInstance> updatedProcessInstances = obtainProcessInstances(commandContext);
		//suspension state is not updated synchronously
		if (NewSuspensionState != null && updatedProcessInstances != null)
		{
		  foreach (ProcessInstance processInstance in updatedProcessInstances)
		  {

			if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_UPDATE, processInstance))
			{
			  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
			}
		  }
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly AbstractSetProcessInstanceStateCmd outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass(AbstractSetProcessInstanceStateCmd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			HistoricProcessInstanceEventEntity processInstanceUpdateEvt = (HistoricProcessInstanceEventEntity) producer.createProcessInstanceUpdateEvt((DelegateExecution) processInstance);
			if (SuspensionState_Fields.SUSPENDED.StateCode == outerInstance.NewSuspensionState.StateCode)
			{
			  processInstanceUpdateEvt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_SUSPENDED;
			}
			else
			{
			  processInstanceUpdateEvt.State = org.camunda.bpm.engine.history.HistoricProcessInstance_Fields.STATE_ACTIVE;
			}
			return processInstanceUpdateEvt;
		  }
	  }

	  protected internal virtual IList<ProcessInstance> obtainProcessInstances(CommandContext commandContext)
	  {
		ProcessInstanceQueryImpl query = new ProcessInstanceQueryImpl();
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		else if (isProcessDefinitionTenantIdSet)
		{
		  query.processDefinitionKey(processDefinitionKey);
		  if (!string.ReferenceEquals(processDefinitionTenantId, null))
		  {
			query.tenantIdIn(processDefinitionTenantId);
		  }
		  else
		  {
			query.withoutTenantId();
		  }
		}
		else
		{
		  query.processDefinitionKey(processDefinitionKey);
		}
		IList<ProcessInstance> result = new List<ProcessInstance>();
		((IList<ProcessInstance>)result).AddRange(commandContext.ExecutionManager.findProcessInstancesByQueryCriteria(query,null));
		return result;
	  }

	  protected internal override void logUserOperation(CommandContext commandContext)
	  {
		PropertyChange propertyChange = new PropertyChange(SUSPENSION_STATE_PROPERTY, null, NewSuspensionState.Name);
		commandContext.OperationLogManager.logProcessInstanceOperation(LogEntryOperation, processInstanceId, processDefinitionId, processDefinitionKey, Collections.singletonList(propertyChange));
	  }

	  protected internal virtual UpdateJobSuspensionStateBuilderImpl createJobCommandBuilder()
	  {
		UpdateJobSuspensionStateBuilderImpl builder = new UpdateJobSuspensionStateBuilderImpl();

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  builder.byProcessInstanceId(processInstanceId);

		}
		else if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  builder.byProcessDefinitionId(processDefinitionId);

		}
		else if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  builder.byProcessDefinitionKey(processDefinitionKey);

		  if (isProcessDefinitionTenantIdSet && !string.ReferenceEquals(processDefinitionTenantId, null))
		  {
			return builder.processDefinitionTenantId(processDefinitionTenantId);

		  }
		  else if (isProcessDefinitionTenantIdSet)
		  {
			return builder.processDefinitionWithoutTenantId();
		  }
		}
		return builder;
	  }

	  protected internal override AbstractSetJobStateCmd NextCommand
	  {
		  get
		  {
			UpdateJobSuspensionStateBuilderImpl jobCommandBuilder = createJobCommandBuilder();
    
			return getNextCommand(jobCommandBuilder);
		  }
	  }

	  protected internal abstract AbstractSetJobStateCmd getNextCommand(UpdateJobSuspensionStateBuilderImpl jobCommandBuilder);

	}

}