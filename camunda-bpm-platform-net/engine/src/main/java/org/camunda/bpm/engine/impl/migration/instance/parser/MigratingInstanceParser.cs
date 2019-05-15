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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
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
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// Builds a <seealso cref="MigratingProcessInstance"/>, a data structure that contains meta-data for the activity
	/// instances that are migrated.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MigratingInstanceParser
	{

	  protected internal ProcessEngine engine;

	  protected internal MigratingInstanceParseHandler<ActivityInstance> activityInstanceHandler = new ActivityInstanceHandler();
	  protected internal MigratingInstanceParseHandler<TransitionInstance> transitionInstanceHandler = new TransitionInstanceHandler();
	  protected internal MigratingInstanceParseHandler<EventSubscriptionEntity> compensationInstanceHandler = new CompensationInstanceHandler();

	  protected internal MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>> dependentActivityInstanceJobHandler = new ActivityInstanceJobHandler();
	  protected internal MigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>> dependentTransitionInstanceJobHandler = new TransitionInstanceJobHandler();
	  protected internal MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>> dependentEventSubscriptionHandler = new EventSubscriptionInstanceHandler();
	  protected internal MigratingDependentInstanceParseHandler<MigratingProcessElementInstance, IList<VariableInstanceEntity>> dependentVariableHandler = new VariableInstanceHandler();
	  protected internal MigratingInstanceParseHandler<IncidentEntity> incidentHandler = new IncidentInstanceHandler();

	  public MigratingInstanceParser(ProcessEngine engine)
	  {
		this.engine = engine;
	  }

	  public virtual MigratingProcessInstance parse(string processInstanceId, MigrationPlan migrationPlan, MigratingProcessInstanceValidationReportImpl processInstanceReport)
	  {

		CommandContext commandContext = Context.CommandContext;
		IList<EventSubscriptionEntity> eventSubscriptions = fetchEventSubscriptions(commandContext, processInstanceId);
		IList<ExecutionEntity> executions = fetchExecutions(commandContext, processInstanceId);
		IList<ExternalTaskEntity> externalTasks = fetchExternalTasks(commandContext, processInstanceId);
		IList<IncidentEntity> incidents = fetchIncidents(commandContext, processInstanceId);
		IList<JobEntity> jobs = fetchJobs(commandContext, processInstanceId);
		IList<TaskEntity> tasks = fetchTasks(commandContext, processInstanceId);
		IList<VariableInstanceEntity> variables = fetchVariables(commandContext, processInstanceId);

		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);
		processInstance.restoreProcessInstance(executions, eventSubscriptions, variables, tasks, jobs, incidents, externalTasks);

		ProcessDefinitionEntity targetProcessDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(migrationPlan.TargetProcessDefinitionId);
		IList<JobDefinitionEntity> targetJobDefinitions = fetchJobDefinitions(commandContext, targetProcessDefinition.Id);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MigratingInstanceParseContext parseContext = new MigratingInstanceParseContext(this, migrationPlan, processInstance, targetProcessDefinition).eventSubscriptions(eventSubscriptions).externalTasks(externalTasks).incidents(incidents).jobs(jobs).tasks(tasks).targetJobDefinitions(targetJobDefinitions).variables(variables);
		MigratingInstanceParseContext parseContext = (new MigratingInstanceParseContext(this, migrationPlan, processInstance, targetProcessDefinition)).eventSubscriptions(eventSubscriptions).externalTasks(externalTasks).incidents(incidents).jobs(jobs).tasks(tasks).targetJobDefinitions(targetJobDefinitions).variables(variables);

		ActivityInstance activityInstance = engine.RuntimeService.getActivityInstance(processInstanceId);

		ActivityInstanceWalker activityInstanceWalker = new ActivityInstanceWalker(activityInstance);

		activityInstanceWalker.addPreVisitor(new TreeVisitorAnonymousInnerClass(this, parseContext));

		activityInstanceWalker.walkWhile();

		CompensationEventSubscriptionWalker compensateSubscriptionsWalker = new CompensationEventSubscriptionWalker(parseContext.MigratingActivityInstances);

		compensateSubscriptionsWalker.addPreVisitor(new TreeVisitorAnonymousInnerClass2(this, parseContext));

		compensateSubscriptionsWalker.walkWhile();

		foreach (IncidentEntity incidentEntity in incidents)
		{
		  incidentHandler.handle(parseContext, incidentEntity);
		}

		parseContext.validateNoEntitiesLeft(processInstanceReport);

		return parseContext.MigratingProcessInstance;
	  }

	  private class TreeVisitorAnonymousInnerClass : TreeVisitor<ActivityInstance>
	  {
		  private readonly MigratingInstanceParser outerInstance;

		  private org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext parseContext;

		  public TreeVisitorAnonymousInnerClass(MigratingInstanceParser outerInstance, org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext parseContext)
		  {
			  this.outerInstance = outerInstance;
			  this.parseContext = parseContext;
		  }

		  public void visit(ActivityInstance obj)
		  {
			outerInstance.activityInstanceHandler.handle(parseContext, obj);
		  }
	  }

	  private class TreeVisitorAnonymousInnerClass2 : TreeVisitor<EventSubscriptionEntity>
	  {
		  private readonly MigratingInstanceParser outerInstance;

		  private org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext parseContext;

		  public TreeVisitorAnonymousInnerClass2(MigratingInstanceParser outerInstance, org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext parseContext)
		  {
			  this.outerInstance = outerInstance;
			  this.parseContext = parseContext;
		  }

		  public void visit(EventSubscriptionEntity obj)
		  {
			outerInstance.compensationInstanceHandler.handle(parseContext, obj);
		  }
	  }

	  public virtual MigratingInstanceParseHandler<ActivityInstance> ActivityInstanceHandler
	  {
		  get
		  {
			return activityInstanceHandler;
		  }
	  }

	  public virtual MigratingInstanceParseHandler<TransitionInstance> TransitionInstanceHandler
	  {
		  get
		  {
			return transitionInstanceHandler;
		  }
	  }

	  public virtual MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<EventSubscriptionEntity>> DependentEventSubscriptionHandler
	  {
		  get
		  {
			return dependentEventSubscriptionHandler;
		  }
	  }

	  public virtual MigratingDependentInstanceParseHandler<MigratingActivityInstance, IList<JobEntity>> DependentActivityInstanceJobHandler
	  {
		  get
		  {
			return dependentActivityInstanceJobHandler;
		  }
	  }

	  public virtual MigratingDependentInstanceParseHandler<MigratingTransitionInstance, IList<JobEntity>> DependentTransitionInstanceJobHandler
	  {
		  get
		  {
			return dependentTransitionInstanceJobHandler;
		  }
	  }

	  public virtual MigratingInstanceParseHandler<IncidentEntity> IncidentHandler
	  {
		  get
		  {
			return incidentHandler;
		  }
	  }

	  public virtual MigratingDependentInstanceParseHandler<MigratingProcessElementInstance, IList<VariableInstanceEntity>> DependentVariablesHandler
	  {
		  get
		  {
			return dependentVariableHandler;
		  }
	  }

	  protected internal virtual IList<ExecutionEntity> fetchExecutions(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.ExecutionManager.findExecutionsByProcessInstanceId(processInstanceId);
	  }

	  protected internal virtual IList<EventSubscriptionEntity> fetchEventSubscriptions(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.EventSubscriptionManager.findEventSubscriptionsByProcessInstanceId(processInstanceId);
	  }

	  protected internal virtual IList<ExternalTaskEntity> fetchExternalTasks(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.ExternalTaskManager.findExternalTasksByProcessInstanceId(processInstanceId);
	  }

	  protected internal virtual IList<JobEntity> fetchJobs(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.JobManager.findJobsByProcessInstanceId(processInstanceId);
	  }

	  protected internal virtual IList<IncidentEntity> fetchIncidents(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.IncidentManager.findIncidentsByProcessInstance(processInstanceId);
	  }

	  protected internal virtual IList<TaskEntity> fetchTasks(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.TaskManager.findTasksByProcessInstanceId(processInstanceId);
	  }

	  protected internal virtual IList<JobDefinitionEntity> fetchJobDefinitions(CommandContext commandContext, string processDefinitionId)
	  {
		return commandContext.JobDefinitionManager.findByProcessDefinitionId(processDefinitionId);
	  }

	  protected internal virtual IList<VariableInstanceEntity> fetchVariables(CommandContext commandContext, string processInstanceId)
	  {
		return commandContext.VariableInstanceManager.findVariableInstancesByProcessInstanceId(processInstanceId);
	  }

	}

}