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
namespace org.camunda.bpm.engine.impl.oplog
{
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;

	public class UserOperationLogContextEntryBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal UserOperationLogContextEntry entry_Conflict;

	  public static UserOperationLogContextEntryBuilder entry(string operationType, string entityType)
	  {
		UserOperationLogContextEntryBuilder builder = new UserOperationLogContextEntryBuilder();
		builder.entry_Conflict = new UserOperationLogContextEntry(operationType, entityType);
		return builder;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(JobEntity job)
	  {
		entry_Conflict.JobDefinitionId = job.JobDefinitionId;
		entry_Conflict.ProcessInstanceId = job.ProcessInstanceId;
		entry_Conflict.ProcessDefinitionId = job.ProcessDefinitionId;
		entry_Conflict.ProcessDefinitionKey = job.ProcessDefinitionKey;
		entry_Conflict.DeploymentId = job.DeploymentId;

		ExecutionEntity execution = job.Execution;
		if (execution != null)
		{
		  entry_Conflict.RootProcessInstanceId = execution.RootProcessInstanceId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(JobDefinitionEntity jobDefinition)
	  {
		entry_Conflict.JobDefinitionId = jobDefinition.Id;
		entry_Conflict.ProcessDefinitionId = jobDefinition.ProcessDefinitionId;
		entry_Conflict.ProcessDefinitionKey = jobDefinition.ProcessDefinitionKey;

		if (!string.ReferenceEquals(jobDefinition.ProcessDefinitionId, null))
		{
		  ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(jobDefinition.ProcessDefinitionId);
		  entry_Conflict.DeploymentId = processDefinition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ExecutionEntity execution)
	  {
		entry_Conflict.ProcessInstanceId = execution.ProcessInstanceId;
		entry_Conflict.RootProcessInstanceId = execution.RootProcessInstanceId;
		entry_Conflict.ProcessDefinitionId = execution.ProcessDefinitionId;

		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		entry_Conflict.ProcessDefinitionKey = processDefinition.Key;
		entry_Conflict.DeploymentId = processDefinition.DeploymentId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ProcessDefinitionEntity processDefinition)
	  {
		entry_Conflict.ProcessDefinitionId = processDefinition.Id;
		entry_Conflict.ProcessDefinitionKey = processDefinition.Key;
		entry_Conflict.DeploymentId = processDefinition.DeploymentId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(TaskEntity task, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Conflict.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Conflict.PropertyChanges = propertyChanges;

		ProcessDefinitionEntity definition = task.ProcessDefinition;
		if (definition != null)
		{
		  entry_Conflict.ProcessDefinitionKey = definition.Key;
		  entry_Conflict.DeploymentId = definition.DeploymentId;
		}
		else if (!string.ReferenceEquals(task.CaseDefinitionId, null))
		{
		  entry_Conflict.DeploymentId = task.CaseDefinition.DeploymentId;
		}

		entry_Conflict.ProcessDefinitionId = task.ProcessDefinitionId;
		entry_Conflict.ProcessInstanceId = task.ProcessInstanceId;
		entry_Conflict.ExecutionId = task.ExecutionId;
		entry_Conflict.CaseDefinitionId = task.CaseDefinitionId;
		entry_Conflict.CaseInstanceId = task.CaseInstanceId;
		entry_Conflict.CaseExecutionId = task.CaseExecutionId;
		entry_Conflict.TaskId = task.Id;

		ExecutionEntity execution = task.getExecution();
		if (execution != null)
		{
		  entry_Conflict.RootProcessInstanceId = execution.RootProcessInstanceId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(HistoricTaskInstance task, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Conflict.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Conflict.PropertyChanges = propertyChanges;

		entry_Conflict.ProcessDefinitionKey = task.ProcessDefinitionKey;
		entry_Conflict.ProcessDefinitionId = task.ProcessDefinitionId;
		entry_Conflict.ProcessInstanceId = task.ProcessInstanceId;
		entry_Conflict.ExecutionId = task.ExecutionId;
		entry_Conflict.CaseDefinitionId = task.CaseDefinitionId;
		entry_Conflict.CaseInstanceId = task.CaseInstanceId;
		entry_Conflict.CaseExecutionId = task.CaseExecutionId;
		entry_Conflict.TaskId = task.Id;
		entry_Conflict.RootProcessInstanceId = task.RootProcessInstanceId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ExecutionEntity processInstance, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Conflict.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Conflict.PropertyChanges = propertyChanges;
		entry_Conflict.RootProcessInstanceId = processInstance.RootProcessInstanceId;
		entry_Conflict.ProcessInstanceId = processInstance.ProcessInstanceId;
		entry_Conflict.ProcessDefinitionId = processInstance.ProcessDefinitionId;
		entry_Conflict.ExecutionId = processInstance.Id;
		entry_Conflict.CaseInstanceId = processInstance.CaseInstanceId;

		ProcessDefinitionEntity definition = processInstance.getProcessDefinition();
		if (definition != null)
		{
		  entry_Conflict.ProcessDefinitionKey = definition.Key;
		  entry_Conflict.DeploymentId = definition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf<T1>(HistoryEvent historyEvent, ResourceDefinitionEntity<T1> definition, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Conflict.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Conflict.PropertyChanges = propertyChanges;
		entry_Conflict.RootProcessInstanceId = historyEvent.RootProcessInstanceId;
		entry_Conflict.ProcessDefinitionId = historyEvent.ProcessDefinitionId;
		entry_Conflict.ProcessInstanceId = historyEvent.ProcessInstanceId;
		entry_Conflict.ExecutionId = historyEvent.ExecutionId;
		entry_Conflict.CaseDefinitionId = historyEvent.CaseDefinitionId;
		entry_Conflict.CaseInstanceId = historyEvent.CaseInstanceId;
		entry_Conflict.CaseExecutionId = historyEvent.CaseExecutionId;

		if (definition != null)
		{
		  if (definition is ProcessDefinitionEntity)
		  {
			entry_Conflict.ProcessDefinitionKey = definition.Key;
		  }
		  entry_Conflict.DeploymentId = definition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf<T1>(HistoricVariableInstanceEntity variable, ResourceDefinitionEntity<T1> definition, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Conflict.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Conflict.PropertyChanges = propertyChanges;
		entry_Conflict.RootProcessInstanceId = variable.RootProcessInstanceId;
		entry_Conflict.ProcessDefinitionId = variable.ProcessDefinitionId;
		entry_Conflict.ProcessInstanceId = variable.ProcessInstanceId;
		entry_Conflict.ExecutionId = variable.ExecutionId;
		entry_Conflict.CaseDefinitionId = variable.CaseDefinitionId;
		entry_Conflict.CaseInstanceId = variable.CaseInstanceId;
		entry_Conflict.CaseExecutionId = variable.CaseExecutionId;
		entry_Conflict.TaskId = variable.TaskId;

		if (definition != null)
		{
		  if (definition is ProcessDefinitionEntity)
		  {
			entry_Conflict.ProcessDefinitionKey = definition.Key;
		  }
		  entry_Conflict.DeploymentId = definition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ExternalTaskEntity task, ExecutionEntity execution, ProcessDefinitionEntity definition)
	  {
		if (execution != null)
		{
		  inContextOf(execution);
		}
		else if (definition != null)
		{
		  inContextOf(definition);
		}
		entry_Conflict.ExternalTaskId = task.Id;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder propertyChanges(IList<PropertyChange> propertyChanges)
	  {
		entry_Conflict.PropertyChanges = propertyChanges;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder propertyChanges(PropertyChange propertyChange)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(propertyChange);
		entry_Conflict.PropertyChanges = propertyChanges;
		return this;
	  }

	  public virtual UserOperationLogContextEntry create()
	  {
		return entry_Conflict;
	  }

	  public virtual UserOperationLogContextEntryBuilder jobId(string jobId)
	  {
		entry_Conflict.JobId = jobId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder jobDefinitionId(string jobDefinitionId)
	  {
		entry_Conflict.JobDefinitionId = jobDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processDefinitionId(string processDefinitionId)
	  {
		entry_Conflict.ProcessDefinitionId = processDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processDefinitionKey(string processDefinitionKey)
	  {
		entry_Conflict.ProcessDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processInstanceId(string processInstanceId)
	  {
		entry_Conflict.ProcessInstanceId = processInstanceId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder caseDefinitionId(string caseDefinitionId)
	  {
		entry_Conflict.CaseDefinitionId = caseDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder deploymentId(string deploymentId)
	  {
		entry_Conflict.DeploymentId = deploymentId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder batchId(string batchId)
	  {
		entry_Conflict.BatchId = batchId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder taskId(string taskId)
	  {
		entry_Conflict.TaskId = taskId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder caseInstanceId(string caseInstanceId)
	  {
		entry_Conflict.CaseInstanceId = caseInstanceId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder category(string category)
	  {
		entry_Conflict.Category = category;
		return this;
	  }

	}

}