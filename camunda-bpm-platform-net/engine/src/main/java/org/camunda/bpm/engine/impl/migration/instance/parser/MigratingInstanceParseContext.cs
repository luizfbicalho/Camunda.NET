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
namespace org.camunda.bpm.engine.impl.migration.instance.parser
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using MigratingProcessInstanceValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingProcessInstanceValidationReportImpl;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingInstanceParseContext
	{

	  protected internal MigratingProcessInstance migratingProcessInstance;

	  protected internal IDictionary<string, MigratingActivityInstance> activityInstances = new Dictionary<string, MigratingActivityInstance>();
	  protected internal IDictionary<string, MigratingEventScopeInstance> compensationInstances = new Dictionary<string, MigratingEventScopeInstance>();
	  protected internal IDictionary<string, MigratingJobInstance> migratingJobs = new Dictionary<string, MigratingJobInstance>();
	  protected internal IDictionary<string, MigratingExternalTaskInstance> migratingExternalTasks = new Dictionary<string, MigratingExternalTaskInstance>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<EventSubscriptionEntity> eventSubscriptions_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<IncidentEntity> incidents_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<JobEntity> jobs_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<TaskEntity> tasks_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<ExternalTaskEntity> externalTasks_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ICollection<VariableInstanceEntity> variables_Renamed;

	  protected internal ProcessDefinitionEntity sourceProcessDefinition;
	  protected internal ProcessDefinitionEntity targetProcessDefinition;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IDictionary<string, IList<JobDefinitionEntity>> targetJobDefinitions_Renamed;
	  protected internal ActivityExecutionTreeMapping mapping;
	  protected internal IDictionary<string, IList<MigrationInstruction>> instructionsBySourceScope;

	  protected internal MigratingInstanceParser parser;

	  public MigratingInstanceParseContext(MigratingInstanceParser parser, MigrationPlan migrationPlan, ExecutionEntity processInstance, ProcessDefinitionEntity targetProcessDefinition)
	  {
		this.parser = parser;
		this.sourceProcessDefinition = processInstance.getProcessDefinition();
		this.targetProcessDefinition = targetProcessDefinition;
		this.migratingProcessInstance = new MigratingProcessInstance(processInstance.Id, sourceProcessDefinition, targetProcessDefinition);
		this.mapping = new ActivityExecutionTreeMapping(Context.CommandContext, processInstance.Id);
		this.instructionsBySourceScope = organizeInstructionsBySourceScope(migrationPlan);
	  }

	  public virtual MigratingInstanceParseContext jobs(ICollection<JobEntity> jobs)
	  {
		this.jobs_Renamed = new HashSet<JobEntity>(jobs);
		return this;
	  }

	  public virtual MigratingInstanceParseContext incidents(ICollection<IncidentEntity> incidents)
	  {
		this.incidents_Renamed = new HashSet<IncidentEntity>(incidents);
		return this;
	  }

	  public virtual MigratingInstanceParseContext tasks(ICollection<TaskEntity> tasks)
	  {
		this.tasks_Renamed = new HashSet<TaskEntity>(tasks);
		return this;
	  }

	  public virtual MigratingInstanceParseContext externalTasks(ICollection<ExternalTaskEntity> externalTasks)
	  {
		this.externalTasks_Renamed = new HashSet<ExternalTaskEntity>(externalTasks);
		return this;
	  }

	  public virtual MigratingInstanceParseContext eventSubscriptions(ICollection<EventSubscriptionEntity> eventSubscriptions)
	  {
		this.eventSubscriptions_Renamed = new HashSet<EventSubscriptionEntity>(eventSubscriptions);
		return this;
	  }

	  public virtual MigratingInstanceParseContext targetJobDefinitions(ICollection<JobDefinitionEntity> jobDefinitions)
	  {
		this.targetJobDefinitions_Renamed = new Dictionary<string, IList<JobDefinitionEntity>>();

		foreach (JobDefinitionEntity jobDefinition in jobDefinitions)
		{
		  CollectionUtil.addToMapOfLists(this.targetJobDefinitions_Renamed, jobDefinition.ActivityId, jobDefinition);
		}
		return this;
	  }

	  public virtual MigratingInstanceParseContext variables(ICollection<VariableInstanceEntity> variables)
	  {
		this.variables_Renamed = new HashSet<VariableInstanceEntity>(variables);
		return this;
	  }

	  public virtual void submit(MigratingActivityInstance activityInstance)
	  {
		activityInstances[activityInstance.ActivityInstance.Id] = activityInstance;
	  }

	  public virtual void submit(MigratingEventScopeInstance compensationInstance)
	  {
		ExecutionEntity scopeExecution = compensationInstance.resolveRepresentativeExecution();
		if (scopeExecution != null)
		{
		  compensationInstances[scopeExecution.Id] = compensationInstance;
		}
	  }

	  public virtual void submit(MigratingJobInstance job)
	  {
		migratingJobs[job.JobEntity.Id] = job;
	  }

	  public virtual void submit(MigratingExternalTaskInstance externalTask)
	  {
		migratingExternalTasks[externalTask.Id] = externalTask;
	  }

	  public virtual void consume(TaskEntity task)
	  {
		tasks_Renamed.remove(task);
	  }

	  public virtual void consume(ExternalTaskEntity externalTask)
	  {
		externalTasks_Renamed.remove(externalTask);
	  }

	  public virtual void consume(IncidentEntity incident)
	  {
		incidents_Renamed.remove(incident);
	  }

	  public virtual void consume(JobEntity job)
	  {
		jobs_Renamed.remove(job);
	  }

	  public virtual void consume(EventSubscriptionEntity eventSubscription)
	  {
		eventSubscriptions_Renamed.remove(eventSubscription);
	  }

	  public virtual void consume(VariableInstanceEntity variableInstance)
	  {
		variables_Renamed.remove(variableInstance);
	  }

	  public virtual MigratingProcessInstance MigratingProcessInstance
	  {
		  get
		  {
			return migratingProcessInstance;
		  }
	  }

	  public virtual ICollection<MigratingActivityInstance> MigratingActivityInstances
	  {
		  get
		  {
			return activityInstances.Values;
		  }
	  }

	  public virtual ProcessDefinitionImpl SourceProcessDefinition
	  {
		  get
		  {
			return sourceProcessDefinition;
		  }
	  }

	  public virtual ProcessDefinitionImpl TargetProcessDefinition
	  {
		  get
		  {
			return targetProcessDefinition;
		  }
	  }

	  public virtual ActivityImpl getTargetActivity(MigrationInstruction instruction)
	  {
		if (instruction != null)
		{
		  return targetProcessDefinition.findActivity(instruction.TargetActivityId);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual JobDefinitionEntity getTargetJobDefinition(string activityId, string jobHandlerType)
	  {
		IList<JobDefinitionEntity> jobDefinitionsForActivity = targetJobDefinitions_Renamed[activityId];

		if (jobDefinitionsForActivity != null)
		{
		  foreach (JobDefinitionEntity jobDefinition in jobDefinitionsForActivity)
		  {
			if (jobHandlerType.Equals(jobDefinition.JobType))
			{
			  // assuming there is no more than one job definition per pair of activity and type
			  return jobDefinition;
			}
		  }
		}

		return null;
	  }

	  public virtual ActivityExecutionTreeMapping Mapping
	  {
		  get
		  {
			return mapping;
		  }
	  }

	  // TODO: conditions would go here
	  public virtual MigrationInstruction getInstructionFor(string scopeId)
	  {
		IList<MigrationInstruction> instructions = instructionsBySourceScope[scopeId];

		if (instructions == null || instructions.Count == 0)
		{
		  return null;
		}
		else
		{
		  return instructions[0];
		}
	  }

	  public virtual MigratingActivityInstance getMigratingActivityInstanceById(string activityInstanceId)
	  {
		return activityInstances[activityInstanceId];
	  }

	  public virtual MigratingScopeInstance getMigratingCompensationInstanceByExecutionId(string id)
	  {
		return compensationInstances[id];
	  }

	  public virtual MigratingJobInstance getMigratingJobInstanceById(string jobId)
	  {
		return migratingJobs[jobId];
	  }

	  public virtual MigratingExternalTaskInstance getMigratingExternalTaskInstanceById(string externalTaskId)
	  {
		return migratingExternalTasks[externalTaskId];
	  }

	  public virtual MigrationInstruction findSingleMigrationInstruction(string sourceScopeId)
	  {
		IList<MigrationInstruction> instructions = instructionsBySourceScope[sourceScopeId];

		if (instructions != null && instructions.Count > 0)
		{
		  return instructions[0];
		}
		else
		{
		  return null;
		}

	  }

	  protected internal virtual IDictionary<string, IList<MigrationInstruction>> organizeInstructionsBySourceScope(MigrationPlan migrationPlan)
	  {
		IDictionary<string, IList<MigrationInstruction>> organizedInstructions = new Dictionary<string, IList<MigrationInstruction>>();

		foreach (MigrationInstruction instruction in migrationPlan.Instructions)
		{
		  CollectionUtil.addToMapOfLists(organizedInstructions, instruction.SourceActivityId, instruction);
		}

		return organizedInstructions;
	  }

	  public virtual void handleDependentActivityInstanceJobs(MigratingActivityInstance migratingInstance, IList<JobEntity> jobs)
	  {
		parser.DependentActivityInstanceJobHandler.handle(this, migratingInstance, jobs);
	  }

	  public virtual void handleDependentTransitionInstanceJobs(MigratingTransitionInstance migratingInstance, IList<JobEntity> jobs)
	  {
		parser.DependentTransitionInstanceJobHandler.handle(this, migratingInstance, jobs);
	  }

	  public virtual void handleDependentEventSubscriptions(MigratingActivityInstance migratingInstance, IList<EventSubscriptionEntity> eventSubscriptions)
	  {
		parser.DependentEventSubscriptionHandler.handle(this, migratingInstance, eventSubscriptions);
	  }

	  public virtual void handleDependentVariables(MigratingProcessElementInstance migratingInstance, IList<VariableInstanceEntity> variables)
	  {
		parser.DependentVariablesHandler.handle(this, migratingInstance, variables);
	  }

	  public virtual void handleTransitionInstance(TransitionInstance transitionInstance)
	  {
		parser.TransitionInstanceHandler.handle(this, transitionInstance);
	  }

	  public virtual void validateNoEntitiesLeft(MigratingProcessInstanceValidationReportImpl processInstanceReport)
	  {
		processInstanceReport.ProcessInstanceId = migratingProcessInstance.ProcessInstanceId;

		ensureNoEntitiesAreLeft("tasks", tasks_Renamed, processInstanceReport);
		ensureNoEntitiesAreLeft("externalTask", externalTasks_Renamed, processInstanceReport);
		ensureNoEntitiesAreLeft("incidents", incidents_Renamed, processInstanceReport);
		ensureNoEntitiesAreLeft("jobs", jobs_Renamed, processInstanceReport);
		ensureNoEntitiesAreLeft("event subscriptions", eventSubscriptions_Renamed, processInstanceReport);
		ensureNoEntitiesAreLeft("variables", variables_Renamed, processInstanceReport);
	  }

	  public virtual void ensureNoEntitiesAreLeft<T1>(string entityName, ICollection<T1> dbEntities, MigratingProcessInstanceValidationReportImpl processInstanceReport) where T1 : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		if (dbEntities.Count > 0)
		{
		  processInstanceReport.addFailure("Process instance contains not migrated " + entityName + ": [" + StringUtil.joinDbEntityIds(dbEntities) + "]");
		}
	  }



	}

}