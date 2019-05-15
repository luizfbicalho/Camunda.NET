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
namespace org.camunda.bpm.engine.impl.cfg
{
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using HistoricProcessInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using HistoricTaskInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;

	/// <summary>
	/// Is invoked while executing a command to check if the current operation is
	/// allowed on the entity. If it is not allowed, the checker throws a
	/// <seealso cref="ProcessEngineException"/>.
	/// </summary>
	public interface CommandChecker
	{

	  /// <summary>
	  /// Checks if it is allowed to evaluate the given decision.
	  /// </summary>
	  void checkEvaluateDecision(DecisionDefinition decisionDefinition);

	  /// <summary>
	  /// Checks if it is allowed to create an instance of the given process definition.
	  /// </summary>
	  void checkCreateProcessInstance(ProcessDefinition processDefinition);

	  /// <summary>
	  /// Checks if it is allowed to read the given process definition.
	  /// </summary>
	  void checkReadProcessDefinition(ProcessDefinition processDefinition);

	  /// <summary>
	  /// Checks if it is allowed to create an instance of the given case definition.
	  /// </summary>
	  void checkCreateCaseInstance(CaseDefinition caseDefinition);

	  /// <summary>
	  /// Checks if it is allowed to update a process definition of the given process definition id.
	  /// </summary>
	  void checkUpdateProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to update the suspension state of a process definition.
	  /// </summary>
	  void checkUpdateProcessDefinitionSuspensionStateById(string processDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance of the given process definition id.
	  /// </summary>
	  void checkUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  ///  Checks if it is allowed to update a process instance's retries of the given process definition.
	  /// </summary>
	  void checkUpdateRetriesProcessInstanceByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance's suspension state of the given process definition.
	  /// </summary>
	  void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to update a decision definition with given id.
	  /// </summary>
	  void checkUpdateDecisionDefinitionById(string decisionDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to update a process definition of the given process definition key.
	  /// </summary>
	  void checkUpdateProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  /// Checks if it is allowed to update the suspension state of a process definition.
	  /// </summary>
	  void checkUpdateProcessDefinitionSuspensionStateByKey(string processDefinitionKey);

	  /// <summary>
	  /// Checks if it is allowed to delete a process definition, which corresponds to the given id.
	  /// </summary>
	  /// <param name="processDefinitionId"> the id which corresponds to the process definition </param>
	  void checkDeleteProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Checks if it is allowed to delete a process definition, which corresponds to the given key.
	  /// </summary>
	  /// <param name="processDefinitionKey"> the key which corresponds to the process definition </param>
	  void checkDeleteProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  ///  Checks if it is allowed to update a process instance of the given process definition key.
	  /// </summary>
	  void checkUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance's suspension state of the given process definition.
	  /// </summary>
	  void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance of the given process instance id.
	  /// </summary>
	  void checkUpdateProcessInstanceById(string processInstanceId);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance's suspension state.
	  /// </summary>
	  void checkUpdateProcessInstanceSuspensionStateById(string processInstanceId);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance of the given execution.
	  /// </summary>
	  void checkUpdateProcessInstance(ExecutionEntity execution);

	  /// <summary>
	  /// Checks if it is allowed to update a process instance's variables of the given execution.
	  /// </summary>
	  void checkUpdateProcessInstanceVariables(ExecutionEntity execution);

	  void checkCreateMigrationPlan(ProcessDefinition sourceProcessDefinition, ProcessDefinition targetProcessDefinition);

	  void checkMigrateProcessInstance(ExecutionEntity processInstance, ProcessDefinition targetProcessDefinition);

	  void checkReadProcessInstance(string processInstanceId);

	  /// <summary>
	  /// Checks if it is allowed to read the given job.
	  /// </summary>
	  void checkReadJob(JobEntity job);

	  /// <summary>
	  /// Checks if it is allowed to update the given job.
	  /// </summary>
	  void checkUpdateJob(JobEntity job);

	  /// <summary>
	  /// Checks if it is allowed to update a job retries.
	  /// </summary>
	  void checkUpdateRetriesJob(JobEntity job);

	  /// <summary>
	  /// Checks if it is allowed to read a process instance of the given execution.
	  /// </summary>
	  void checkReadProcessInstance(ExecutionEntity execution);

	  /// <summary>
	  /// Checks if it is allowed to read a process instance's variables of the given execution.
	  /// </summary>
	  void checkReadProcessInstanceVariable(ExecutionEntity execution);

	  /// <summary>
	  /// Check if it is allowed to delete a process instance of the given execution.
	  /// </summary>
	  void checkDeleteProcessInstance(ExecutionEntity execution);

	  /// <summary>
	  /// Check if it is allowed to read a task.
	  /// </summary>
	  void checkReadTask(TaskEntity task);

	  /// <summary>
	  /// Check if it is allowed to read a task's variable.
	  /// </summary>
	  void checkReadTaskVariable(TaskEntity task);

	  /// <summary>
	  /// Check if it is allowed to update a task's variable
	  /// </summary>
	  void checkUpdateTaskVariable(TaskEntity task);

	  /// <summary>
	  /// Check if it is allowed to create a batch
	  /// </summary>
	  void checkCreateBatch(Permission permission);

	  /// <summary>
	  /// Check if it is allowed to delete a batch
	  /// </summary>
	  void checkDeleteBatch(BatchEntity batch);

	  /// <summary>
	  /// Check if it is allowed to delete a historic batch
	  /// </summary>
	  void checkDeleteHistoricBatch(HistoricBatchEntity batch);

	  /// <summary>
	  /// Check if it is allowed to suspend a batch
	  /// </summary>
	  void checkSuspendBatch(BatchEntity batch);

	  /// <summary>
	  /// Check if it is allowed to activate a batch
	  /// </summary>
	  void checkActivateBatch(BatchEntity batch);

	  /// <summary>
	  /// Check if it is allowed to read historic batch
	  /// </summary>
	  void checkReadHistoricBatch();

	  /// <summary>
	  /// Checks if it is allowed to create a deployment.
	  /// </summary>
	  void checkCreateDeployment();

	  /// <summary>
	  /// Checks if it is allowed to read a deployment of the given deployment id.
	  /// </summary>
	  void checkReadDeployment(string deploymentId);

	  /// <summary>
	  /// Checks if it is allowed to delete a deployment of the given deployment id.
	  /// </summary>
	  void checkDeleteDeployment(string deploymentId);

	  /// <summary>
	  /// Check if it is allowed to assign a task
	  /// </summary>
	  void checkTaskAssign(TaskEntity task);

	  /// <summary>
	  /// Check if it is allowed to create a task
	  /// </summary>
	  void checkCreateTask(TaskEntity task);

	  /// <summary>
	  /// Check if it is allowed to create a task
	  /// </summary>
	  void checkCreateTask();

	  /// <summary>
	  /// Check if it is allowed to work on a task
	  /// </summary>
	  void checkTaskWork(TaskEntity task);

	  /// <summary>
	  ///  Check if it is allowed to delete a task
	  /// </summary>
	  void checkDeleteTask(TaskEntity task);

	  /// <summary>
	  /// Checks if it is allowed to read the given decision definition.
	  /// </summary>
	  void checkReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition);

	  /// <summary>
	  /// Checks if it is allowed to read the given decision requirements definition.
	  /// </summary>
	  void checkReadDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition);

	  /// <summary>
	  /// Checks if it is allowed to read the given case definition.
	  /// </summary>
	  void checkReadCaseDefinition(CaseDefinition caseDefinition);

	  /// <summary>
	  /// Checks if it is allowed to update the given case definition.
	  /// </summary>
	  void checkUpdateCaseDefinition(CaseDefinition caseDefinition);

	  /// <summary>
	  /// Checks if it is allowed to delete the given historic task instance.
	  /// </summary>
	  void checkDeleteHistoricTaskInstance(HistoricTaskInstanceEntity task);

	  /// <summary>
	  /// Checks if it is allowed to delete the given historic process instance.
	  /// </summary>
	  void checkDeleteHistoricProcessInstance(HistoricProcessInstance instance);

	  /// <summary>
	  /// Checks if it is allowed to delete the given historic case instance.
	  /// </summary>
	  void checkDeleteHistoricCaseInstance(HistoricCaseInstance instance);

	  /// <summary>
	  /// Checks if it is allowed to delete the historic decision instance of the given
	  /// decision definition key.
	  /// </summary>
	  void checkDeleteHistoricDecisionInstance(string decisionDefinitionKey);

	  /// <summary>
	  /// Checks if it is allowed to delete the given historic decision instance.
	  /// </summary>
	  void checkDeleteHistoricDecisionInstance(HistoricDecisionInstance instance);

	  /// <summary>
	  /// Checks if it is allowed to read the given historic job log.
	  /// </summary>
	  void checkReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog);

	  /// <summary>
	  /// Check if it is allowed to read the history for any process definition.
	  /// </summary>
	  void checkReadHistoryAnyProcessDefinition();

	  /// <summary>
	  /// Check if it is allowed to read the history of the given process definition.
	  /// </summary>
	  void checkReadHistoryProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Check if it is allowed to read the history for any task instance
	  /// </summary>
	  void checkReadHistoryAnyTaskInstance();

	  /// <summary>
	  /// Check if it is allowed to update a case instance of the given case execution.
	  /// </summary>
	  void checkUpdateCaseInstance(CaseExecution caseExecution);

	  /// <summary>
	  /// Check if it is allowed to delete the user operation log of the given user operation log entry.
	  /// </summary>
	  void checkDeleteUserOperationLog(UserOperationLogEntry entry);

	  /// <summary>
	  /// Check if it is allowed to read a case instance of the given case execution.
	  /// </summary>
	  void checkReadCaseInstance(CaseExecution caseExecution);

	  /// <summary>
	  /// Checks if it is allowed to read the given historic external task log.
	  /// </summary>
	  void checkReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog);

	  /// <summary>
	  /// Checks if it is allowed to delete the given historic variable instance.
	  /// </summary>
	  void checkDeleteHistoricVariableInstance(HistoricVariableInstanceEntity variable);

	  /// <summary>
	  /// Checks if it is allowed to delete the historic variable instances of the given process instance.
	  /// </summary>
	  void checkDeleteHistoricVariableInstancesByProcessInstance(HistoricProcessInstanceEntity instance);

	}

}