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
namespace org.camunda.bpm.engine.impl.cfg.multitenancy
{
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using org.camunda.bpm.engine.impl.persistence.entity;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;

	/// <summary>
	/// <seealso cref="CommandChecker"/> to ensure that commands are only executed for
	/// entities which belongs to one of the authenticated tenants.
	/// </summary>
	public class TenantCommandChecker : CommandChecker
	{

	  protected internal static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  public virtual void checkEvaluateDecision(DecisionDefinition decisionDefinition)
	  {
		if (!TenantManager.isAuthenticatedTenant(decisionDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("evaluate the decision '" + decisionDefinition.Id + "'");
		}
	  }

	  public virtual void checkCreateProcessInstance(ProcessDefinition processDefinition)
	  {
		if (!TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("create an instance of the process definition '" + processDefinition.Id + "'");
		}
	  }

	  public virtual void checkReadProcessDefinition(ProcessDefinition processDefinition)
	  {
		if (!TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the process definition '" + processDefinition.Id + "'");
		}
	  }

	  public virtual void checkCreateCaseInstance(CaseDefinition caseDefinition)
	  {
		if (!TenantManager.isAuthenticatedTenant(caseDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("create an instance of the case definition '" + caseDefinition.Id + "'");
		}
	  }

	  public virtual void checkUpdateProcessDefinitionById(string processDefinitionId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null && !TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("update the process definition '" + processDefinitionId + "'");
		  }
		}
	  }

	  public virtual void checkUpdateProcessDefinitionSuspensionStateById(string processDefinitionId)
	  {
		checkUpdateProcessDefinitionById(processDefinitionId);
	  }

	  public virtual void checkUpdateProcessDefinitionByKey(string processDefinitionKey)
	  {
	  }

	  public virtual void checkUpdateProcessDefinitionSuspensionStateByKey(string processDefinitionKey)
	  {
	  }

	  public virtual void checkDeleteProcessDefinitionById(string processDefinitionId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null && !TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("delete the process definition '" + processDefinitionId + "'");
		  }
		}
	  }

	  public virtual void checkDeleteProcessDefinitionByKey(string processDefinitionKey)
	  {
	  }



	  public virtual void checkUpdateProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  ProcessDefinitionEntity processDefinition = findLatestProcessDefinitionById(processDefinitionId);
		  if (processDefinition != null && !TenantManager.isAuthenticatedTenant(processDefinition.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("update the process definition '" + processDefinitionId + "'");
		  }
		}
	  }

	  public virtual void checkUpdateRetriesProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		checkUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionId(string processDefinitionId)
	  {
		checkUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
	  }

	  public virtual void checkUpdateProcessInstance(ExecutionEntity execution)
	  {
		if (execution != null && !TenantManager.isAuthenticatedTenant(execution.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("update the process instance '" + execution.Id + "'");
		}
	  }

	  public virtual void checkUpdateProcessInstanceVariables(ExecutionEntity execution)
	  {
		checkUpdateProcessInstance(execution);
	  }

	  public virtual void checkUpdateJob(JobEntity job)
	  {
		if (job != null && !TenantManager.isAuthenticatedTenant(job.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("update the job '" + job.Id + "'");
		}
	  }

	  public virtual void checkUpdateRetriesJob(JobEntity job)
	  {
		checkUpdateJob(job);
	  }

	  public virtual void checkUpdateProcessInstanceByProcessDefinitionKey(string processDefinitionKey)
	  {
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateByProcessDefinitionKey(string processDefinitionKey)
	  {
	  }

	  public virtual void checkUpdateProcessInstanceById(string processInstanceId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  ExecutionEntity execution = findExecutionById(processInstanceId);
		  if (execution != null && !TenantManager.isAuthenticatedTenant(execution.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("update the process instance '" + processInstanceId + "'");
		  }
		}
	  }

	  public virtual void checkUpdateProcessInstanceSuspensionStateById(string processInstanceId)
	  {
		checkUpdateProcessInstanceById(processInstanceId);
	  }

	  public virtual void checkCreateMigrationPlan(ProcessDefinition sourceProcessDefinition, ProcessDefinition targetProcessDefinition)
	  {
		string sourceTenant = sourceProcessDefinition.TenantId;
		string targetTenant = targetProcessDefinition.TenantId;

		if (!TenantManager.isAuthenticatedTenant(sourceTenant))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get process definition '" + sourceProcessDefinition.Id + "'");
		}
		if (!TenantManager.isAuthenticatedTenant(targetTenant))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get process definition '" + targetProcessDefinition.Id + "'");
		}

		if (!string.ReferenceEquals(sourceTenant, null) && !string.ReferenceEquals(targetTenant, null) && !sourceTenant.Equals(targetTenant))
		{
		  throw ProcessEngineLogger.MIGRATION_LOGGER.cannotMigrateBetweenTenants(sourceTenant, targetTenant);
		}
	  }

	  public virtual void checkReadProcessInstance(string processInstanceId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  ExecutionEntity execution = findExecutionById(processInstanceId);
		  if (execution != null && !TenantManager.isAuthenticatedTenant(execution.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("read the process instance '" + processInstanceId + "'");
		  }
		}
	  }

	  public virtual void checkReadJob(JobEntity job)
	  {
		if (job != null && !TenantManager.isAuthenticatedTenant(job.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("read the job '" + job.Id + "'");
		}
	  }

	  public virtual void checkReadProcessInstance(ExecutionEntity execution)
	  {
		if (execution != null && !TenantManager.isAuthenticatedTenant(execution.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("read the process instance '" + execution.Id + "'");
		}
	  }

	  public virtual void checkReadProcessInstanceVariable(ExecutionEntity execution)
	  {
		checkReadProcessInstance(execution);
	  }

	  public virtual void checkDeleteProcessInstance(ExecutionEntity execution)
	  {
		if (execution != null && !TenantManager.isAuthenticatedTenant(execution.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the process instance '" + execution.Id + "'");
		}
	  }

	  public virtual void checkMigrateProcessInstance(ExecutionEntity processInstance, ProcessDefinition targetProcessDefinition)
	  {
		string sourceTenant = processInstance.TenantId;
		string targetTenant = targetProcessDefinition.TenantId;

		if (TenantManager.TenantCheckEnabled)
		{
		  if (processInstance != null && !TenantManager.isAuthenticatedTenant(processInstance.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("migrate process instance '" + processInstance.Id + "'");
		  }
		}

		if (!string.ReferenceEquals(targetTenant, null) && (string.ReferenceEquals(sourceTenant, null) || !sourceTenant.Equals(targetTenant)))
		{
		  throw ProcessEngineLogger.MIGRATION_LOGGER.cannotMigrateInstanceBetweenTenants(processInstance.Id, sourceTenant, targetTenant);
		}
	  }

	  public virtual void checkReadTask(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("read the task '" + task.Id + "'");
		}
	  }

	  public virtual void checkReadTaskVariable(TaskEntity task)
	  {
		checkReadTask(task);
	  }

	  public virtual void checkUpdateTaskVariable(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("update the task '" + task.Id + "'");
		}
	  }

	  public virtual void checkCreateBatch(Permission permission)
	  {
	  }

	  public virtual void checkDeleteBatch(BatchEntity batch)
	  {
		if (batch != null && !TenantManager.isAuthenticatedTenant(batch.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete batch '" + batch.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricBatch(HistoricBatchEntity batch)
	  {
		if (batch != null && !TenantManager.isAuthenticatedTenant(batch.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete historic batch '" + batch.Id + "'");
		}
	  }

	  public virtual void checkSuspendBatch(BatchEntity batch)
	  {
		if (batch != null && !TenantManager.isAuthenticatedTenant(batch.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("suspend batch '" + batch.Id + "'");
		}
	  }

	  public virtual void checkActivateBatch(BatchEntity batch)
	  {
		if (batch != null && !TenantManager.isAuthenticatedTenant(batch.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("activate batch '" + batch.Id + "'");
		}
	  }

	  public virtual void checkReadHistoricBatch()
	  {
	  }

	  public virtual void checkCreateDeployment()
	  {
	  }

	  public virtual void checkReadDeployment(string deploymentId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  DeploymentEntity deployment = findDeploymentById(deploymentId);
		  if (deployment != null && !TenantManager.isAuthenticatedTenant(deployment.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("get the deployment '" + deploymentId + "'");
		  }
		}
	  }

	  public virtual void checkDeleteDeployment(string deploymentId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  DeploymentEntity deployment = findDeploymentById(deploymentId);
		  if (deployment != null && !TenantManager.isAuthenticatedTenant(deployment.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("delete the deployment '" + deploymentId + "'");
		  }
		}
	  }

	  public virtual void checkDeleteTask(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the task '" + task.Id + "'");
		}
	  }

	  public virtual void checkTaskAssign(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("assign the task '" + task.Id + "'");
		}
	  }

	  public virtual void checkCreateTask(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("create the task '" + task.Id + "'");
		}
	  }

	  public virtual void checkCreateTask()
	  {
	  }

	  public virtual void checkTaskWork(TaskEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("work on task '" + task.Id + "'");
		}
	  }

	  public virtual void checkReadDecisionDefinition(DecisionDefinitionEntity decisionDefinition)
	  {
		if (decisionDefinition != null && !TenantManager.isAuthenticatedTenant(decisionDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the decision definition '" + decisionDefinition.Id + "'");
		}
	  }

	  public virtual void checkUpdateDecisionDefinitionById(string decisionDefinitionId)
	  {
		if (TenantManager.TenantCheckEnabled)
		{
		  DecisionDefinitionEntity decisionDefinition = findLatestDecisionDefinitionById(decisionDefinitionId);
		  if (decisionDefinition != null && !TenantManager.isAuthenticatedTenant(decisionDefinition.TenantId))
		  {
			throw LOG.exceptionCommandWithUnauthorizedTenant("update the decision definition '" + decisionDefinitionId + "'");
		  }
		}
	  }

	  public virtual void checkReadDecisionRequirementsDefinition(DecisionRequirementsDefinitionEntity decisionRequirementsDefinition)
	  {
		if (decisionRequirementsDefinition != null && !TenantManager.isAuthenticatedTenant(decisionRequirementsDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the decision requirements definition '" + decisionRequirementsDefinition.Id + "'");
		}
	  }

	  public virtual void checkReadCaseDefinition(CaseDefinition caseDefinition)
	  {
		if (caseDefinition != null && !TenantManager.isAuthenticatedTenant(caseDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the case definition '" + caseDefinition.Id + "'");
		}
	  }

	  public virtual void checkUpdateCaseDefinition(CaseDefinition caseDefinition)
	  {
		if (caseDefinition != null && !TenantManager.isAuthenticatedTenant(caseDefinition.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("update the case definition '" + caseDefinition.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricTaskInstance(HistoricTaskInstanceEntity task)
	  {
		if (task != null && !TenantManager.isAuthenticatedTenant(task.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic task instance '" + task.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricProcessInstance(HistoricProcessInstance instance)
	  {
		if (instance != null && !TenantManager.isAuthenticatedTenant(instance.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic process instance '" + instance.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricCaseInstance(HistoricCaseInstance instance)
	  {
		if (instance != null && !TenantManager.isAuthenticatedTenant(instance.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic case instance '" + instance.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricDecisionInstance(string decisionDefinitionKey)
	  {
		// No tenant check here because it is called in the SQL query:
		// HistoricDecisionInstance.selectHistoricDecisionInstancesByDecisionDefinitionId
		// It is necessary to make the check there because of performance issues. If the check
		// is done here then the we get all history decision instances (also from possibly
		// other tenants) and then filter them. If there are a lot instances this can cause
		// latency. Therefore does the SQL query only return the
		// historic decision instances which belong to the authenticated tenant.
	  }

	  public virtual void checkDeleteHistoricDecisionInstance(HistoricDecisionInstance decisionInstance)
	  {
		if (decisionInstance != null && !TenantManager.isAuthenticatedTenant(decisionInstance.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic decision instance '" + decisionInstance.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricVariableInstance(HistoricVariableInstanceEntity variable)
	  {
		if (variable != null && !TenantManager.isAuthenticatedTenant(variable.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic variable instance '" + variable.Id + "'");
		}
	  }

	  public virtual void checkDeleteHistoricVariableInstancesByProcessInstance(HistoricProcessInstanceEntity instance)
	  {
		if (instance != null && !TenantManager.isAuthenticatedTenant(instance.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("delete the historic variable instances of process instance '" + instance.Id + "'");
		}
	  }

	  public virtual void checkReadHistoricJobLog(HistoricJobLogEventEntity historicJobLog)
	  {
		if (historicJobLog != null && !TenantManager.isAuthenticatedTenant(historicJobLog.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the historic job log '" + historicJobLog.Id + "'");
		}
	  }

	  public virtual void checkReadHistoryAnyProcessDefinition()
	  {
		// No tenant check here because it is called in the SQL query:
		// Report.selectHistoricProcessInstanceDurationReport
		// It is necessary to make the check there because the query may be return only the
		// historic process instances which belong to the authenticated tenant.
	  }

	  public virtual void checkReadHistoryProcessDefinition(string processDefinitionId)
	  {
		// No tenant check here because it is called in the SQL query:
		// Report.selectHistoricProcessInstanceDurationReport
		// It is necessary to make the check there because the query may be return only the
		// historic process instances which belong to the authenticated tenant.
	  }

	  public virtual void checkReadHistoryAnyTaskInstance()
	  {
		// No tenant check here because it is called in the SQL query:
		// Report.selectHistoricProcessInstanceDurationReport
		// It is necessary to make the check there because the query may be return only the
		// historic process instances which belong to the authenticated tenant.
	  }

	  public virtual void checkUpdateCaseInstance(CaseExecution caseExecution)
	  {
		if (caseExecution != null && !TenantManager.isAuthenticatedTenant(caseExecution.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("update the case execution '" + caseExecution.Id + "'");
		}
	  }

	  public virtual void checkReadCaseInstance(CaseExecution caseExecution)
	  {
		if (caseExecution != null && !TenantManager.isAuthenticatedTenant(caseExecution.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the case execution '" + caseExecution.Id + "'");
		}
	  }

	  public virtual void checkDeleteUserOperationLog(UserOperationLogEntry entry)
	  {
		// tenant check is not available for user operation log
	  }

	  public virtual void checkReadHistoricExternalTaskLog(HistoricExternalTaskLogEntity historicExternalTaskLog)
	  {
		if (historicExternalTaskLog != null && !TenantManager.isAuthenticatedTenant(historicExternalTaskLog.TenantId))
		{
		  throw LOG.exceptionCommandWithUnauthorizedTenant("get the historic external task log '" + historicExternalTaskLog.Id + "'");
		}
	  }

	  // helper //////////////////////////////////////////////////

	  protected internal virtual TenantManager TenantManager
	  {
		  get
		  {
			return Context.CommandContext.TenantManager;
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

	  protected internal virtual DeploymentEntity findDeploymentById(string deploymentId)
	  {
		return Context.CommandContext.DeploymentManager.findDeploymentById(deploymentId);
	  }
	}

}