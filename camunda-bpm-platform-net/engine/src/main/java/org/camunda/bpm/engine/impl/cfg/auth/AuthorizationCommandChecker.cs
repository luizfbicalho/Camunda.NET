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
namespace org.camunda.bpm.engine.impl.cfg.auth
{
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CompositePermissionCheck = org.camunda.bpm.engine.impl.db.CompositePermissionCheck;
	using PermissionCheckBuilder = org.camunda.bpm.engine.impl.db.PermissionCheckBuilder;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using org.camunda.bpm.engine.impl.persistence.entity;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
	using static org.camunda.bpm.engine.authorization.Permissions;
	using static org.camunda.bpm.engine.authorization.Resources;


	/// <summary>
	/// <seealso cref="CommandChecker"/> that uses the <seealso cref="AuthorizationManager"/> to perform
	/// authorization checks.
	/// </summary>
	public class AuthorizationCommandChecker : CommandChecker
	{

	  public virtual void checkEvaluateDecision(DecisionDefinition decisionDefinition)
	  {
		AuthorizationManager.checkAuthorization(CREATE_INSTANCE, DECISION_DEFINITION, decisionDefinition.Key);
	  }

	  public virtual void checkCreateProcessInstance(ProcessDefinition processDefinition)
	  {
		// necessary permissions:
		// - CREATE on PROCESS_INSTANCE
		// AND
		// - CREATE_INSTANCE on PROCESS_DEFINITION
		AuthorizationManager.checkAuthorization(CREATE, PROCESS_INSTANCE);
		AuthorizationManager.checkAuthorization(CREATE_INSTANCE, PROCESS_DEFINITION, processDefinition.Key);
	  }

	  public virtual void checkReadProcessDefinition(ProcessDefinition processDefinition)
	  {
		AuthorizationManager.checkAuthorization(READ, PROCESS_DEFINITION, processDefinition.Key);
	  }

	  public virtual void checkCreateCaseInstance(CaseDefinition caseDefinition)
	  {
		// no authorization check for CMMN
	  }

	  public virtual void checkUpdateProcessDefinitionById(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {
			checkUpdateProcessDefinitionByKey(processDefinition.Key);
		  }
		}
	  }

	  public virtual void checkUpdateProcessDefinitionSuspensionStateById(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {
			checkUpdateProcessDefinitionSuspensionStateByKey(processDefinition.Key);
		  }
		}
	  }

	  public virtual void checkUpdateDecisionDefinitionById(string decisionDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  DecisionDefinitionEntity decisionDefinition = findLatestDecisionDefinitionById(decisionDefinitionId);
		  if (decisionDefinition != null)
		  {
			checkUpdateDecisionDefinition(decisionDefinition);
		  }
		}
	  }

	  public virtual void checkUpdateProcessDefinitionByKey(string processDefinitionKey)
	  {
		AuthorizationManager.checkAuthorization(UPDATE, PROCESS_DEFINITION, processDefinitionKey);
	  }

	  public virtual void checkUpdateProcessDefinitionSuspensionStateByKey(string processDefinitionKey)
	  {
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, ProcessDefinitionPermissions.SUSPEND).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, UPDATE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkDeleteProcessDefinitionById(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {
			checkDeleteProcessDefinitionByKey(processDefinition.Key);
		  }
		}
	  }

	  public virtual void checkDeleteProcessDefinitionByKey(string processDefinitionKey)
	  {
		AuthorizationManager.checkAuthorization(DELETE, PROCESS_DEFINITION, processDefinitionKey);
	  }

	  public virtual void checkUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {
			checkUpdateProcessInstanceByProcessDefinitionKey(processDefinition.Key);
		  }
		}
	  }

	  public virtual void checkUpdateRetriesProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {

			CompositePermissionCheck retryJobPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, ANY, ProcessInstancePermissions.RETRY_JOB).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionId, ProcessDefinitionPermissions.RETRY_JOB).atomicCheckForResourceId(PROCESS_INSTANCE, ANY, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionId, UPDATE_INSTANCE).build();

			AuthorizationManager.checkAuthorization(retryJobPermission);
		  }
		}
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionId(string processDefinitionId)
	  {
		if (AuthorizationManager.AuthorizationEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null)
		  {
			checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(processDefinition.Key);
		  }
		}
	  }

	  public virtual void checkUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
	  {
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, null, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(string processDefinitionKey)
	  {
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, null, ProcessInstancePermissions.SUSPEND).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, ProcessDefinitionPermissions.SUSPEND_INSTANCE).atomicCheckForResourceId(PROCESS_INSTANCE, null, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkReadProcessInstance(string processInstanceId)
	  {
		ExecutionEntity execution = findExecutionById(processInstanceId);
		if (execution != null)
		{
		  checkReadProcessInstance(execution);
		}
	  }

	  public virtual void checkDeleteProcessInstance(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();

		// necessary permissions:
		// - DELETE on PROCESS_INSTANCE
		// ... OR ...
		// - DELETE_INSTANCE on PROCESS_DEFINITION

		CompositePermissionCheck deleteInstancePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, DELETE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, DELETE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(deleteInstancePermission);
	  }

	  public virtual void checkUpdateProcessInstanceById(string processInstanceId)
	  {
		ExecutionEntity execution = findExecutionById(processInstanceId);
		if (execution != null)
		{
		  checkUpdateProcessInstance(execution);
		}
	  }

	  public virtual void checkUpdateProcessInstance(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkUpdateProcessInstanceVariables(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, ProcessInstancePermissions.UPDATE_VARIABLE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, ProcessDefinitionPermissions.UPDATE_INSTANCE_VARIABLE).atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateById(string processInstanceId)
	  {
		ExecutionEntity execution = findExecutionById(processInstanceId);
		if (execution != null)
		{
		  checkUpdateProcessInstanceSuspensionState(execution);
		}
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionState(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		CompositePermissionCheck suspensionStatePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, ProcessInstancePermissions.SUSPEND).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, ProcessDefinitionPermissions.SUSPEND_INSTANCE).atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(suspensionStatePermission);
	  }

	  public virtual void checkUpdateJob(JobEntity job)
	  {
		if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		{
		  // "standalone" job: nothing to do!
		  return;
		}

		CompositePermissionCheck retryJobPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, job.ProcessInstanceId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, job.ProcessDefinitionKey, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(retryJobPermission);
	  }

	  public virtual void checkUpdateRetriesJob(JobEntity job)
	  {
		if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		{
		  // "standalone" job: nothing to do!
		  return;
		}

		CompositePermissionCheck retryJobPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, job.ProcessInstanceId, ProcessInstancePermissions.RETRY_JOB).atomicCheckForResourceId(PROCESS_DEFINITION, job.ProcessDefinitionKey, ProcessDefinitionPermissions.RETRY_JOB).atomicCheckForResourceId(PROCESS_INSTANCE, job.ProcessInstanceId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, job.ProcessDefinitionKey, UPDATE_INSTANCE).build();

		AuthorizationManager.checkAuthorization(retryJobPermission);
	  }

	  public virtual void checkCreateMigrationPlan(ProcessDefinition sourceProcessDefinition, ProcessDefinition targetProcessDefinition)
	  {
		checkReadProcessDefinition(sourceProcessDefinition);
		checkReadProcessDefinition(targetProcessDefinition);
	  }

	  public virtual void checkMigrateProcessInstance(ExecutionEntity processInstance, ProcessDefinition targetProcessDefinition)
	  {
	  }

	  public virtual void checkReadProcessInstance(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();

		// necessary permissions:
		// - READ on PROCESS_INSTANCE
		// ... OR ...
		// - READ_INSTANCE on PROCESS_DEFINITION
		CompositePermissionCheck readProcessInstancePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, execution.ProcessInstanceId, READ).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, READ_INSTANCE).build();

		AuthorizationManager.checkAuthorization(readProcessInstancePermission);
	  }

	  public virtual void checkReadProcessInstanceVariable(ExecutionEntity execution)
	  {
		if (AuthorizationManager.EnsureSpecificVariablePermission)
		{
		  ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();

		  // necessary permissions:
		  // - READ_INSTANCE_VARIABLE on PROCESS_DEFINITION
		  CompositePermissionCheck readProcessInstancePermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, ProcessDefinitionPermissions.READ_INSTANCE_VARIABLE).build();

		  AuthorizationManager.checkAuthorization(readProcessInstancePermission);
		}
		else
		{
		  checkReadProcessInstance(execution);
		}
	  }

	  public virtual void checkReadJob(JobEntity job)
	  {
		if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		{
		  // "standalone" job: nothing to do!
		  return;
		}

		// necessary permissions:
		// - READ on PROCESS_INSTANCE
		// ... OR ...
		// - READ_INSTANCE on PROCESS_DEFINITION
		CompositePermissionCheck readJobPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_INSTANCE, job.ProcessInstanceId, READ).atomicCheckForResourceId(PROCESS_DEFINITION, job.ProcessDefinitionKey, READ_INSTANCE).build();

		AuthorizationManager.checkAuthorization(readJobPermission);
	  }

	  public virtual void checkReadTask(TaskEntity task)
	  {
		checkTaskPermission(task, READ_TASK, READ);
	  }

	  public virtual void checkReadTaskVariable(TaskEntity task)
	  {
		Permission readProcessInstanceTaskPermission;
		Permission readStandaloneTaskPermission;
		if (AuthorizationManager.EnsureSpecificVariablePermission)
		{
		  readProcessInstanceTaskPermission = ProcessDefinitionPermissions.READ_TASK_VARIABLE;
		  readStandaloneTaskPermission = TaskPermissions.READ_VARIABLE;
		}
		else
		{
		  readProcessInstanceTaskPermission = READ_TASK;
		  readStandaloneTaskPermission = READ;
		}
		checkTaskPermission(task, readProcessInstanceTaskPermission, readStandaloneTaskPermission);
	  }

	  protected internal virtual void checkTaskPermission(TaskEntity task, Permission processDefinitionPermission, Permission taskPermission)
	  {
		string taskId = task.Id;
		string executionId = task.ExecutionId;

		if (!string.ReferenceEquals(executionId, null))
		{

		  // if task exists in context of a process instance
		  // then check the following permissions:
		  // - 'taskPermission' on TASK
		  // - 'processDefinitionPermission' on PROCESS_DEFINITION

		  ExecutionEntity execution = task.getExecution();
		  ProcessDefinitionEntity processDefinition = execution.getProcessDefinition();

		  CompositePermissionCheck readTaskPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, taskPermission).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, processDefinitionPermission).build();

		  AuthorizationManager.checkAuthorization(readTaskPermission);

		}
		else
		{

		  // if task does not exist in context of process
		  // instance, then it is either a (a) standalone task
		  // or (b) it exists in context of a case instance.

		  // (a) standalone task: check following permission
		  // - 'taskPermission' on TASK
		  // (b) task in context of a case instance, in this
		  // case it is not necessary to check any permission,
		  // because such tasks can always be read

		  string caseExecutionId = task.CaseExecutionId;
		  if (string.ReferenceEquals(caseExecutionId, null))
		  {
			AuthorizationManager.checkAuthorization(taskPermission, TASK, taskId);
		  }

		}
	  }

	  public virtual void checkUpdateTaskVariable(TaskEntity task)
	  {
		string taskId = task.Id;

		string executionId = task.ExecutionId;
		if (!string.ReferenceEquals(executionId, null))
		{

		  // if task exists in context of a process instance
		  // then check the following permissions:
		  // - UPDATE_VARIABLE on TASK
		  // - UPDATE_TASK_VARIABLE on PROCESS_DEFINITION
		  // - UPDATE on TASK
		  // - UPDATE_TASK on PROCESS_DEFINITION

		  ExecutionEntity execution = task.getExecution();
		  ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();

		  CompositePermissionCheck updateTaskPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TaskPermissions.UPDATE_VARIABLE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, ProcessDefinitionPermissions.UPDATE_TASK_VARIABLE).atomicCheckForResourceId(TASK, taskId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinition.Key, UPDATE_TASK).build();

		  AuthorizationManager.checkAuthorization(updateTaskPermissionCheck);

		}
		else
		{

		  // if task does not exist in context of process
		  // instance, then it is either a (a) standalone task
		  // or (b) it exists in context of a case instance.

		  // (a) standalone task: check following permission
		  // - READ on TASK
		  // (b) task in context of a case instance, in this
		  // case it is not necessary to check any permission,
		  // because such tasks can always be updated

		  string caseExecutionId = task.CaseExecutionId;
		  if (string.ReferenceEquals(caseExecutionId, null))
		  {
			// standalone task
			CompositePermissionCheck updateTaskPermissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TaskPermissions.UPDATE_VARIABLE).atomicCheckForResourceId(TASK, taskId, UPDATE).build();

			AuthorizationManager.checkAuthorization(updateTaskPermissionCheck);
		  }

		}
	  }

	  public virtual void checkCreateBatch(Permission permission)
	  {
		CompositePermissionCheck createBatchPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(BATCH, null, permission).atomicCheckForResourceId(BATCH, null, CREATE).build();

		AuthorizationManager.checkAuthorization(createBatchPermission);
	  }

	  public virtual void checkDeleteBatch(BatchEntity batch)
	  {
		AuthorizationManager.checkAuthorization(DELETE, BATCH, batch.Id);
	  }

	  public virtual void checkDeleteHistoricBatch(HistoricBatchEntity batch)
	  {
		AuthorizationManager.checkAuthorization(DELETE_HISTORY, BATCH, batch.Id);
	  }

	  public virtual void checkSuspendBatch(BatchEntity batch)
	  {
		AuthorizationManager.checkAuthorization(UPDATE, BATCH, batch.Id);
	  }

	  public virtual void checkActivateBatch(BatchEntity batch)
	  {
		AuthorizationManager.checkAuthorization(UPDATE, BATCH, batch.Id);
	  }

	  public virtual void checkReadHistoricBatch()
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, BATCH);
	  }

	  /* DEPLOYMENT */

	  // create permission ////////////////////////////////////////////////

	  public virtual void checkCreateDeployment()
	  {
		AuthorizationManager.checkAuthorization(CREATE, DEPLOYMENT);
	  }

	  // read permission //////////////////////////////////////////////////

	  public virtual void checkReadDeployment(string deploymentId)
	  {
		AuthorizationManager.checkAuthorization(READ, DEPLOYMENT, deploymentId);
	  }

	  // delete permission //////////////////////////////////////////////////

	  public virtual void checkDeleteDeployment(string deploymentId)
	  {
		AuthorizationManager.checkAuthorization(DELETE, DEPLOYMENT, deploymentId);
	  }

	  public virtual void checkReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		AuthorizationManager.checkAuthorization(READ, DECISION_DEFINITION, decisionDefinition.Key);
	  }

	  public virtual void checkUpdateDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		AuthorizationManager.checkAuthorization(UPDATE, DECISION_DEFINITION, decisionDefinition.Key);
	  }

	  public virtual void checkReadDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
	  {
		AuthorizationManager.checkAuthorization(READ, DECISION_REQUIREMENTS_DEFINITION, decisionRequirementsDefinition.Key);
	  }

	  public virtual void checkReadCaseDefinition(CaseDefinition caseDefinition)
	  {
	  }

	  public virtual void checkUpdateCaseDefinition(CaseDefinition caseDefinition)
	  {
	  }

	  // delete permission ////////////////////////////////////////

	  public virtual void checkDeleteHistoricTaskInstance(HistoricTaskInstanceEntity task)
	  {
		// deleting unexisting historic task instance should be silently ignored
		// see javaDoc HistoryService.deleteHistoricTaskInstance
		if (task != null)
		{
		  if (!string.ReferenceEquals(task.ProcessDefinitionKey, null))
		  {
			AuthorizationManager.checkAuthorization(DELETE_HISTORY, PROCESS_DEFINITION, task.ProcessDefinitionKey);
		  }
		}
	  }

	  // delete permission /////////////////////////////////////////////////

	  public virtual void checkDeleteHistoricProcessInstance(HistoricProcessInstance instance)
	  {
		AuthorizationManager.checkAuthorization(DELETE_HISTORY, PROCESS_DEFINITION, instance.ProcessDefinitionKey);
	  }

	  public virtual void checkDeleteHistoricCaseInstance(HistoricCaseInstance instance)
	  {
	  }

	  public virtual void checkDeleteHistoricDecisionInstance(string decisionDefinitionKey)
	  {
		AuthorizationManager.checkAuthorization(DELETE_HISTORY, DECISION_DEFINITION, decisionDefinitionKey);
	  }

	  public virtual void checkDeleteHistoricDecisionInstance(HistoricDecisionInstance decisionInstance)
	  {
		AuthorizationManager.checkAuthorization(DELETE_HISTORY, DECISION_DEFINITION, decisionInstance.DecisionDefinitionKey);
	  }

	  public virtual void checkReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog)
	  {
		if (!string.ReferenceEquals(historicJobLog.ProcessDefinitionKey, null))
		{
		  AuthorizationManager.checkAuthorization(READ_HISTORY, PROCESS_DEFINITION, historicJobLog.ProcessDefinitionKey);
		}
	  }

	  public virtual void checkReadHistoryAnyProcessDefinition()
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, PROCESS_DEFINITION, ANY);
	  }

	  public virtual void checkReadHistoryProcessDefinition(string processDefinitionKey)
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, PROCESS_DEFINITION, processDefinitionKey);
	  }

	  public virtual void checkReadHistoryAnyTaskInstance()
	  {
		AuthorizationManager.checkAuthorization(READ_HISTORY, TASK, ANY);
	  }

	  public virtual void checkUpdateCaseInstance(CaseExecution caseExecution)
	  {
	  }

	  public virtual void checkReadCaseInstance(CaseExecution caseExecution)
	  {
	  }

	  // helper ////////////////////////////////////////

	  protected internal virtual AuthorizationManager AuthorizationManager
	  {
		  get
		  {
			return Context.CommandContext.AuthorizationManager;
		  }
	  }

	  protected internal virtual ProcessDefinitionEntity findLatestProcessDefinitionById(string processDefinitionId)
	  {
		return Context.CommandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);
	  }

	  protected internal virtual DecisionDefinitionEntity findLatestDecisionDefinitionById(string decisionDefinitionId)
	  {
		return Context.CommandContext.DecisionDefinitionManager.findDecisionDefinitionById(decisionDefinitionId);
	  }

	  protected internal virtual ExecutionEntity findExecutionById(string processInstanceId)
	  {
		return Context.CommandContext.ExecutionManager.findExecutionById(processInstanceId);
	  }

	  public virtual void checkTaskAssign(TaskEntity task)
	  {

		string taskId = task.Id;

		string executionId = task.ExecutionId;
		if (!string.ReferenceEquals(executionId, null))
		{

		  // Permissions to task actions is based on the order in which PermissioncheckBuilder is built
		  CompositePermissionCheck taskWorkPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TASK_ASSIGN).atomicCheckForResourceId(PROCESS_DEFINITION, task.ProcessDefinition.Key, TASK_ASSIGN).atomicCheckForResourceId(TASK, taskId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, task.ProcessDefinition.Key, UPDATE_TASK).build();

		  AuthorizationManager.checkAuthorization(taskWorkPermission);

		}
		else
		{

		  // if task does not exist in context of process
		  // instance, then it is either a (a) standalone task
		  // or (b) it exists in context of a case instance.

		  // (a) standalone task: check following permission
		  // - TASK_ASSIGN or UPDATE
		  // (b) task in context of a case instance, in this
		  // case it is not necessary to check any permission,
		  // because such tasks can always be updated

		  string caseExecutionId = task.CaseExecutionId;
		  if (string.ReferenceEquals(caseExecutionId, null))
		  {
			// standalone task
			CompositePermissionCheck taskWorkPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TASK_ASSIGN).atomicCheckForResourceId(TASK, taskId, UPDATE).build();

			AuthorizationManager.checkAuthorization(taskWorkPermission);
		  }
		}
	  }

	  // create permission /////////////////////////////////////////////

	  public virtual void checkCreateTask(TaskEntity entity)
	  {
		AuthorizationManager.checkAuthorization(CREATE, TASK);
	  }

	  public virtual void checkCreateTask()
	  {
		AuthorizationManager.checkAuthorization(CREATE, TASK);
	  }

	  public virtual void checkTaskWork(TaskEntity task)
	  {

		string taskId = task.Id;

		string executionId = task.ExecutionId;
		if (!string.ReferenceEquals(executionId, null))
		{

		  // Permissions to task actions is based on the order in which PermissioncheckBuilder is built
		  CompositePermissionCheck taskWorkPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TASK_WORK).atomicCheckForResourceId(PROCESS_DEFINITION, task.ProcessDefinition.Key, TASK_WORK).atomicCheckForResourceId(TASK, taskId, UPDATE).atomicCheckForResourceId(PROCESS_DEFINITION, task.ProcessDefinition.Key, UPDATE_TASK).build();

		  AuthorizationManager.checkAuthorization(taskWorkPermission);

		}
		else
		{

		  // if task does not exist in context of process
		  // instance, then it is either a (a) standalone task
		  // or (b) it exists in context of a case instance.

		  // (a) standalone task: check following permission
		  // - TASK_WORK or UPDATE
		  // (b) task in context of a case instance, in this
		  // case it is not necessary to check any permission,
		  // because such tasks can always be updated

		  string caseExecutionId = task.CaseExecutionId;
		  if (string.ReferenceEquals(caseExecutionId, null))
		  {
			// standalone task
			CompositePermissionCheck taskWorkPermission = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(TASK, taskId, TASK_WORK).atomicCheckForResourceId(TASK, taskId, UPDATE).build();

			  AuthorizationManager.checkAuthorization(taskWorkPermission);
		  }
		}
	  }

	  public virtual void checkDeleteTask(TaskEntity task)
	  {
		string taskId = task.Id;

		// Note: Calling TaskService#deleteTask() to
		// delete a task which exists in context of
		// a process instance or case instance cannot
		// be deleted. In such a case TaskService#deleteTask()
		// throws an exception before invoking the
		// authorization check.

		string executionId = task.ExecutionId;
		string caseExecutionId = task.CaseExecutionId;

		if (string.ReferenceEquals(executionId, null) && string.ReferenceEquals(caseExecutionId, null))
		{
		  AuthorizationManager.checkAuthorization(DELETE, TASK, taskId);
		}
	  }

	  public virtual void checkDeleteUserOperationLog(UserOperationLogEntry entry)
	  {
		/*
		 * (1) if entry has a category and a process definition key:
		 *   => entry in context of process definition
		 *   => check either 
		 *        DELETE_HISTORY on PROCESS_DEFINITION with processDefinitionKey OR
		 *        DELETE on OPERATION_LOG_CATEGORY with category
		 * 
		 * (2) if entry has a category but no process definition key:
		 *   => standalone entry (task, job, batch, ...), admin entry (user, tenant, ...) or CMMN related
		 *   => check DELETE on OPERATION_LOG_CATEGORY with category
		 *   
		 * (3) if entry has no category but a process definition key:
		 *   => pre-7.11.0 entry in context of process definition 
		 *   => check DELETE_HISTORY on PROCESS_DEFINITION with processDefinitionKey
		 *   
		 * (4) if entry has no category and no process definition key:
		 *   => pre-7.11.0 standalone entry (task, job, batch, ...) or CMMN related
		 *   => no authorization check like before 7.11.0
		 */
		if (entry != null)
		{
		  string category = entry.Category;
		  string processDefinitionKey = entry.ProcessDefinitionKey;
		  if (!string.ReferenceEquals(category, null) || !string.ReferenceEquals(processDefinitionKey, null))
		  {
			CompositePermissionCheck permissionCheck = null;
			if (string.ReferenceEquals(category, null))
			{
			  // case (3)
			  permissionCheck = (new PermissionCheckBuilder()).atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, DELETE_HISTORY).build();
			}
			else if (string.ReferenceEquals(processDefinitionKey, null))
			{
			  // case (2)
			  permissionCheck = (new PermissionCheckBuilder()).atomicCheckForResourceId(Resources.OPERATION_LOG_CATEGORY, category, UserOperationLogCategoryPermissions.DELETE).build();
			}
			else
			{
			  // case (1)
			  permissionCheck = (new PermissionCheckBuilder()).disjunctive().atomicCheckForResourceId(PROCESS_DEFINITION, processDefinitionKey, DELETE_HISTORY).atomicCheckForResourceId(Resources.OPERATION_LOG_CATEGORY, category, UserOperationLogCategoryPermissions.DELETE).build();
			}
			AuthorizationManager.checkAuthorization(permissionCheck);
		  }
		  // case (4)
		}
	  }

	  public virtual void checkReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog)
	  {
		if (!string.ReferenceEquals(historicExternalTaskLog.ProcessDefinitionKey, null))
		{
		  AuthorizationManager.checkAuthorization(READ_HISTORY, PROCESS_DEFINITION, historicExternalTaskLog.ProcessDefinitionKey);
		}
	  }

	  public virtual void checkDeleteHistoricVariableInstance(HistoricVariableInstanceEntity variable)
	  {
		if (variable != null && !string.ReferenceEquals(variable.ProcessDefinitionKey, null))
		{
		  AuthorizationManager.checkAuthorization(DELETE_HISTORY, PROCESS_DEFINITION, variable.ProcessDefinitionKey);
		}
		// XXX if CAM-6570 is implemented, there should be a check for variables of standalone tasks here as well
	  }

	  public virtual void checkDeleteHistoricVariableInstancesByProcessInstance(HistoricProcessInstanceEntity instance)
	  {
		checkDeleteHistoricProcessInstance(instance);
	  }

	}

}