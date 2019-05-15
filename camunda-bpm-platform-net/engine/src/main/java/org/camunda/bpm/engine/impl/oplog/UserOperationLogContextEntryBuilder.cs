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
	  protected internal UserOperationLogContextEntry entry_Renamed;

	  public static UserOperationLogContextEntryBuilder entry(string operationType, string entityType)
	  {
		UserOperationLogContextEntryBuilder builder = new UserOperationLogContextEntryBuilder();
		builder.entry_Renamed = new UserOperationLogContextEntry(operationType, entityType);
		return builder;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(JobEntity job)
	  {
		entry_Renamed.JobDefinitionId = job.JobDefinitionId;
		entry_Renamed.ProcessInstanceId = job.ProcessInstanceId;
		entry_Renamed.ProcessDefinitionId = job.ProcessDefinitionId;
		entry_Renamed.ProcessDefinitionKey = job.ProcessDefinitionKey;
		entry_Renamed.DeploymentId = job.DeploymentId;

		ExecutionEntity execution = job.Execution;
		if (execution != null)
		{
		  entry_Renamed.RootProcessInstanceId = execution.RootProcessInstanceId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(JobDefinitionEntity jobDefinition)
	  {
		entry_Renamed.JobDefinitionId = jobDefinition.Id;
		entry_Renamed.ProcessDefinitionId = jobDefinition.ProcessDefinitionId;
		entry_Renamed.ProcessDefinitionKey = jobDefinition.ProcessDefinitionKey;

		if (!string.ReferenceEquals(jobDefinition.ProcessDefinitionId, null))
		{
		  ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(jobDefinition.ProcessDefinitionId);
		  entry_Renamed.DeploymentId = processDefinition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ExecutionEntity execution)
	  {
		entry_Renamed.ProcessInstanceId = execution.ProcessInstanceId;
		entry_Renamed.RootProcessInstanceId = execution.RootProcessInstanceId;
		entry_Renamed.ProcessDefinitionId = execution.ProcessDefinitionId;

		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		entry_Renamed.ProcessDefinitionKey = processDefinition.Key;
		entry_Renamed.DeploymentId = processDefinition.DeploymentId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ProcessDefinitionEntity processDefinition)
	  {
		entry_Renamed.ProcessDefinitionId = processDefinition.Id;
		entry_Renamed.ProcessDefinitionKey = processDefinition.Key;
		entry_Renamed.DeploymentId = processDefinition.DeploymentId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(TaskEntity task, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Renamed.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Renamed.PropertyChanges = propertyChanges;

		ProcessDefinitionEntity definition = task.ProcessDefinition;
		if (definition != null)
		{
		  entry_Renamed.ProcessDefinitionKey = definition.Key;
		  entry_Renamed.DeploymentId = definition.DeploymentId;
		}
		else if (!string.ReferenceEquals(task.CaseDefinitionId, null))
		{
		  entry_Renamed.DeploymentId = task.CaseDefinition.DeploymentId;
		}

		entry_Renamed.ProcessDefinitionId = task.ProcessDefinitionId;
		entry_Renamed.ProcessInstanceId = task.ProcessInstanceId;
		entry_Renamed.ExecutionId = task.ExecutionId;
		entry_Renamed.CaseDefinitionId = task.CaseDefinitionId;
		entry_Renamed.CaseInstanceId = task.CaseInstanceId;
		entry_Renamed.CaseExecutionId = task.CaseExecutionId;
		entry_Renamed.TaskId = task.Id;

		ExecutionEntity execution = task.getExecution();
		if (execution != null)
		{
		  entry_Renamed.RootProcessInstanceId = execution.RootProcessInstanceId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(HistoricTaskInstance task, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Renamed.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Renamed.PropertyChanges = propertyChanges;

		entry_Renamed.ProcessDefinitionKey = task.ProcessDefinitionKey;
		entry_Renamed.ProcessDefinitionId = task.ProcessDefinitionId;
		entry_Renamed.ProcessInstanceId = task.ProcessInstanceId;
		entry_Renamed.ExecutionId = task.ExecutionId;
		entry_Renamed.CaseDefinitionId = task.CaseDefinitionId;
		entry_Renamed.CaseInstanceId = task.CaseInstanceId;
		entry_Renamed.CaseExecutionId = task.CaseExecutionId;
		entry_Renamed.TaskId = task.Id;
		entry_Renamed.RootProcessInstanceId = task.RootProcessInstanceId;

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf(ExecutionEntity processInstance, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Renamed.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Renamed.PropertyChanges = propertyChanges;
		entry_Renamed.RootProcessInstanceId = processInstance.RootProcessInstanceId;
		entry_Renamed.ProcessInstanceId = processInstance.ProcessInstanceId;
		entry_Renamed.ProcessDefinitionId = processInstance.ProcessDefinitionId;
		entry_Renamed.ExecutionId = processInstance.Id;
		entry_Renamed.CaseInstanceId = processInstance.CaseInstanceId;

		ProcessDefinitionEntity definition = processInstance.getProcessDefinition();
		if (definition != null)
		{
		  entry_Renamed.ProcessDefinitionKey = definition.Key;
		  entry_Renamed.DeploymentId = definition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf<T1>(HistoryEvent historyEvent, ResourceDefinitionEntity<T1> definition, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Renamed.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Renamed.PropertyChanges = propertyChanges;
		entry_Renamed.RootProcessInstanceId = historyEvent.RootProcessInstanceId;
		entry_Renamed.ProcessDefinitionId = historyEvent.ProcessDefinitionId;
		entry_Renamed.ProcessInstanceId = historyEvent.ProcessInstanceId;
		entry_Renamed.ExecutionId = historyEvent.ExecutionId;
		entry_Renamed.CaseDefinitionId = historyEvent.CaseDefinitionId;
		entry_Renamed.CaseInstanceId = historyEvent.CaseInstanceId;
		entry_Renamed.CaseExecutionId = historyEvent.CaseExecutionId;

		if (definition != null)
		{
		  if (definition is ProcessDefinitionEntity)
		  {
			entry_Renamed.ProcessDefinitionKey = definition.Key;
		  }
		  entry_Renamed.DeploymentId = definition.DeploymentId;
		}

		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder inContextOf<T1>(HistoricVariableInstanceEntity variable, ResourceDefinitionEntity<T1> definition, IList<PropertyChange> propertyChanges)
	  {

		if (propertyChanges == null || propertyChanges.Count == 0)
		{
		  if (OPERATION_TYPE_CREATE.Equals(entry_Renamed.OperationType))
		  {
			propertyChanges = Arrays.asList(PropertyChange.EMPTY_CHANGE);
		  }
		}
		entry_Renamed.PropertyChanges = propertyChanges;
		entry_Renamed.RootProcessInstanceId = variable.RootProcessInstanceId;
		entry_Renamed.ProcessDefinitionId = variable.ProcessDefinitionId;
		entry_Renamed.ProcessInstanceId = variable.ProcessInstanceId;
		entry_Renamed.ExecutionId = variable.ExecutionId;
		entry_Renamed.CaseDefinitionId = variable.CaseDefinitionId;
		entry_Renamed.CaseInstanceId = variable.CaseInstanceId;
		entry_Renamed.CaseExecutionId = variable.CaseExecutionId;
		entry_Renamed.TaskId = variable.TaskId;

		if (definition != null)
		{
		  if (definition is ProcessDefinitionEntity)
		  {
			entry_Renamed.ProcessDefinitionKey = definition.Key;
		  }
		  entry_Renamed.DeploymentId = definition.DeploymentId;
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
		entry_Renamed.ExternalTaskId = task.Id;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder propertyChanges(IList<PropertyChange> propertyChanges)
	  {
		entry_Renamed.PropertyChanges = propertyChanges;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder propertyChanges(PropertyChange propertyChange)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(propertyChange);
		entry_Renamed.PropertyChanges = propertyChanges;
		return this;
	  }

	  public virtual UserOperationLogContextEntry create()
	  {
		return entry_Renamed;
	  }

	  public virtual UserOperationLogContextEntryBuilder jobId(string jobId)
	  {
		entry_Renamed.JobId = jobId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder jobDefinitionId(string jobDefinitionId)
	  {
		entry_Renamed.JobDefinitionId = jobDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processDefinitionId(string processDefinitionId)
	  {
		entry_Renamed.ProcessDefinitionId = processDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processDefinitionKey(string processDefinitionKey)
	  {
		entry_Renamed.ProcessDefinitionKey = processDefinitionKey;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder processInstanceId(string processInstanceId)
	  {
		entry_Renamed.ProcessInstanceId = processInstanceId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder caseDefinitionId(string caseDefinitionId)
	  {
		entry_Renamed.CaseDefinitionId = caseDefinitionId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder deploymentId(string deploymentId)
	  {
		entry_Renamed.DeploymentId = deploymentId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder batchId(string batchId)
	  {
		entry_Renamed.BatchId = batchId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder taskId(string taskId)
	  {
		entry_Renamed.TaskId = taskId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder caseInstanceId(string caseInstanceId)
	  {
		entry_Renamed.CaseInstanceId = caseInstanceId;
		return this;
	  }

	  public virtual UserOperationLogContextEntryBuilder category(string category)
	  {
		entry_Renamed.Category = category;
		return this;
	  }

	}

}