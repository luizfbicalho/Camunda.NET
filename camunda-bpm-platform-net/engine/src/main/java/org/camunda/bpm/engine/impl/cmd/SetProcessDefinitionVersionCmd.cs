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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensurePositive;


	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	/// <summary>
	/// <seealso cref="Command"/> that changes the process definition version of an existing
	/// process instance.
	/// 
	/// Warning: This command will NOT perform any migration magic and simply set the
	/// process definition version in the database, assuming that the user knows,
	/// what he or she is doing.
	/// 
	/// This is only useful for simple migrations. The new process definition MUST
	/// have the exact same activity id to make it still run.
	/// 
	/// Furthermore, activities referenced by sub-executions and jobs that belong to
	/// the process instance MUST exist in the new process definition version.
	/// 
	/// The command will fail, if there is already a <seealso cref="ProcessInstance"/> or
	/// <seealso cref="HistoricProcessInstance"/> using the new process definition version and
	/// the same business key as the <seealso cref="ProcessInstance"/> that is to be migrated.
	/// 
	/// If the process instance is not currently waiting but actively running, then
	/// this would be a case for optimistic locking, meaning either the version
	/// update or the "real work" wins, i.e., this is a race condition.
	/// </summary>
	/// <seealso cref= http://forums.activiti.org/en/viewtopic.php?t=2918
	/// @author Falko Menge
	/// @author Ingo Richtsmeier </seealso>
	[Serializable]
	public class SetProcessDefinitionVersionCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly string processInstanceId;
	  private readonly int? processDefinitionVersion;

	  public SetProcessDefinitionVersionCmd(string processInstanceId, int? processDefinitionVersion)
	  {
		ensureNotEmpty("The process instance id is mandatory", "processInstanceId", processInstanceId);
		ensureNotNull("The process definition version is mandatory", "processDefinitionVersion", processDefinitionVersion);
		ensurePositive("The process definition version must be positive", "processDefinitionVersion", processDefinitionVersion.Value);
		this.processInstanceId = processInstanceId;
		this.processDefinitionVersion = processDefinitionVersion;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl configuration = commandContext.ProcessEngineConfiguration;

		// check that the new process definition is just another version of the same
		// process definition that the process instance is using
		ExecutionManager executionManager = commandContext.ExecutionManager;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity processInstance = executionManager.findExecutionById(processInstanceId);
		ExecutionEntity processInstance = executionManager.findExecutionById(processInstanceId);
		if (processInstance == null)
		{
		  throw new ProcessEngineException("No process instance found for id = '" + processInstanceId + "'.");
		}
		else if (!processInstance.ProcessInstanceExecution)
		{
		  throw new ProcessEngineException("A process instance id is required, but the provided id " + "'" + processInstanceId + "' " + "points to a child execution of process instance " + "'" + processInstance.ProcessInstanceId + "'. " + "Please invoke the " + this.GetType().Name + " with a root execution id.");
		}
		ProcessDefinitionImpl currentProcessDefinitionImpl = processInstance.getProcessDefinition();

		DeploymentCache deploymentCache = configuration.DeploymentCache;
		ProcessDefinitionEntity currentProcessDefinition;
		if (currentProcessDefinitionImpl is ProcessDefinitionEntity)
		{
		  currentProcessDefinition = (ProcessDefinitionEntity) currentProcessDefinitionImpl;
		}
		else
		{
		  currentProcessDefinition = deploymentCache.findDeployedProcessDefinitionById(currentProcessDefinitionImpl.Id);
		}

		ProcessDefinitionEntity newProcessDefinition = deploymentCache.findDeployedProcessDefinitionByKeyVersionAndTenantId(currentProcessDefinition.Key, processDefinitionVersion, currentProcessDefinition.TenantId);

		validateAndSwitchVersionOfExecution(commandContext, processInstance, newProcessDefinition);

		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_UPDATE, processInstance))
		{
		  HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, processInstance));
		}

		// switch all sub-executions of the process instance to the new process definition version
		IList<ExecutionEntity> childExecutions = executionManager.findExecutionsByProcessInstanceId(processInstanceId);
		foreach (ExecutionEntity executionEntity in childExecutions)
		{
		  validateAndSwitchVersionOfExecution(commandContext, executionEntity, newProcessDefinition);
		}

		// switch all jobs to the new process definition version
		IList<JobEntity> jobs = commandContext.JobManager.findJobsByProcessInstanceId(processInstanceId);
		IList<JobDefinitionEntity> currentJobDefinitions = commandContext.JobDefinitionManager.findByProcessDefinitionId(currentProcessDefinition.Id);
		IList<JobDefinitionEntity> newVersionJobDefinitions = commandContext.JobDefinitionManager.findByProcessDefinitionId(newProcessDefinition.Id);

		IDictionary<string, string> jobDefinitionMapping = getJobDefinitionMapping(currentJobDefinitions, newVersionJobDefinitions);
		foreach (JobEntity jobEntity in jobs)
		{
		  switchVersionOfJob(jobEntity, newProcessDefinition, jobDefinitionMapping);
		}

		// switch all incidents to the new process definition version
		IList<IncidentEntity> incidents = commandContext.IncidentManager.findIncidentsByProcessInstance(processInstanceId);
		foreach (IncidentEntity incidentEntity in incidents)
		{
		  switchVersionOfIncident(commandContext, incidentEntity, newProcessDefinition);
		}

		// add an entry to the op log
		PropertyChange change = new PropertyChange("processDefinitionVersion", currentProcessDefinition.Version, processDefinitionVersion);
		commandContext.OperationLogManager.logProcessInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE, processInstanceId, null, null, Collections.singletonList(change));

		return null;
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly SetProcessDefinitionVersionCmd outerInstance;

		  private ExecutionEntity processInstance;

		  public HistoryEventCreatorAnonymousInnerClass(SetProcessDefinitionVersionCmd outerInstance, ExecutionEntity processInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstance = processInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createProcessInstanceUpdateEvt(processInstance);
		  }
	  }

	  protected internal virtual IDictionary<string, string> getJobDefinitionMapping(IList<JobDefinitionEntity> currentJobDefinitions, IList<JobDefinitionEntity> newVersionJobDefinitions)
	  {
		IDictionary<string, string> mapping = new Dictionary<string, string>();

		foreach (JobDefinitionEntity currentJobDefinition in currentJobDefinitions)
		{
		  foreach (JobDefinitionEntity newJobDefinition in newVersionJobDefinitions)
		  {
			if (jobDefinitionsMatch(currentJobDefinition, newJobDefinition))
			{
			  mapping[currentJobDefinition.Id] = newJobDefinition.Id;
			  break;
			}
		  }
		}

		return mapping;
	  }

	  protected internal virtual bool jobDefinitionsMatch(JobDefinitionEntity currentJobDefinition, JobDefinitionEntity newJobDefinition)
	  {
		bool activitiesMatch = currentJobDefinition.ActivityId.Equals(newJobDefinition.ActivityId);

		bool typesMatch = (string.ReferenceEquals(currentJobDefinition.JobType, null) && string.ReferenceEquals(newJobDefinition.JobType, null)) || (!string.ReferenceEquals(currentJobDefinition.JobType, null) && currentJobDefinition.JobType.Equals(newJobDefinition.JobType));

		bool configurationsMatch = (string.ReferenceEquals(currentJobDefinition.JobConfiguration, null) && string.ReferenceEquals(newJobDefinition.JobConfiguration, null)) || (!string.ReferenceEquals(currentJobDefinition.JobConfiguration, null) && currentJobDefinition.JobConfiguration.Equals(newJobDefinition.JobConfiguration));

		return activitiesMatch && typesMatch && configurationsMatch;
	  }

	  protected internal virtual void switchVersionOfJob(JobEntity jobEntity, ProcessDefinitionEntity newProcessDefinition, IDictionary<string, string> jobDefinitionMapping)
	  {
		jobEntity.ProcessDefinitionId = newProcessDefinition.Id;
		jobEntity.DeploymentId = newProcessDefinition.DeploymentId;

		string newJobDefinitionId = jobDefinitionMapping[jobEntity.JobDefinitionId];
		jobEntity.JobDefinitionId = newJobDefinitionId;
	  }

	  protected internal virtual void switchVersionOfIncident(CommandContext commandContext, IncidentEntity incidentEntity, ProcessDefinitionEntity newProcessDefinition)
	  {
		incidentEntity.ProcessDefinitionId = newProcessDefinition.Id;
	  }

	  protected internal virtual void validateAndSwitchVersionOfExecution(CommandContext commandContext, ExecutionEntity execution, ProcessDefinitionEntity newProcessDefinition)
	  {
		// check that the new process definition version contains the current activity
		if (execution.getActivity() != null)
		{
		  string activityId = execution.getActivity().Id;
		  PvmActivity newActivity = newProcessDefinition.findActivity(activityId);

		  if (newActivity == null)
		  {
			throw new ProcessEngineException("The new process definition " + "(key = '" + newProcessDefinition.Key + "') " + "does not contain the current activity " + "(id = '" + activityId + "') " + "of the process instance " + "(id = '" + processInstanceId + "').");
		  }

			// clear cached activity so that outgoing transitions are refreshed
			execution.setActivity(newActivity);
		}

		// switch the process instance to the new process definition version
		execution.setProcessDefinition(newProcessDefinition);

		// and change possible existing tasks (as the process definition id is stored there too)
		IList<TaskEntity> tasks = commandContext.TaskManager.findTasksByExecutionId(execution.Id);
		foreach (TaskEntity taskEntity in tasks)
		{
		  taskEntity.ProcessDefinitionId = newProcessDefinition.Id;
		}
	  }

	}

}