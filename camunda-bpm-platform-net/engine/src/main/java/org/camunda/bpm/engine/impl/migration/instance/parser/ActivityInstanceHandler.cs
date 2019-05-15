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

	using CoreActivityBehavior = org.camunda.bpm.engine.impl.core.@delegate.CoreActivityBehavior;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityInstanceHandler : MigratingInstanceParseHandler<ActivityInstance>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, ActivityInstance element)
	  {
		MigratingActivityInstance migratingInstance = null;

		MigrationInstruction applyingInstruction = parseContext.getInstructionFor(element.ActivityId);
		ScopeImpl sourceScope = null;
		ScopeImpl targetScope = null;
		ExecutionEntity representativeExecution = parseContext.Mapping.getExecution(element);

		if (element.Id.Equals(element.ProcessInstanceId))
		{
		  sourceScope = parseContext.SourceProcessDefinition;
		  targetScope = parseContext.TargetProcessDefinition;
		}
		else
		{
		  sourceScope = parseContext.SourceProcessDefinition.findActivity(element.ActivityId);

		  if (applyingInstruction != null)
		  {
			string activityId = applyingInstruction.TargetActivityId;
			targetScope = parseContext.TargetProcessDefinition.findActivity(activityId);
		  }
		}

		migratingInstance = parseContext.MigratingProcessInstance.addActivityInstance(applyingInstruction, element, sourceScope, targetScope, representativeExecution);

		MigratingActivityInstance parentInstance = parseContext.getMigratingActivityInstanceById(element.ParentActivityInstanceId);

		if (parentInstance != null)
		{
		  migratingInstance.setParent(parentInstance);
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.core.delegate.CoreActivityBehavior<?> sourceActivityBehavior = sourceScope.getActivityBehavior();
		CoreActivityBehavior<object> sourceActivityBehavior = sourceScope.ActivityBehavior;
		if (sourceActivityBehavior is MigrationObserverBehavior)
		{
		  ((MigrationObserverBehavior) sourceActivityBehavior).onParseMigratingInstance(parseContext, migratingInstance);
		}

		parseContext.submit(migratingInstance);

		parseTransitionInstances(parseContext, migratingInstance);

		parseDependentInstances(parseContext, migratingInstance);
	  }

	  public virtual void parseTransitionInstances(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		foreach (TransitionInstance transitionInstance in migratingInstance.ActivityInstance.ChildTransitionInstances)
		{
		  parseContext.handleTransitionInstance(transitionInstance);
		}
	  }

	  public virtual void parseDependentInstances(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		parseContext.handleDependentVariables(migratingInstance, collectActivityInstanceVariables(migratingInstance));
		parseContext.handleDependentActivityInstanceJobs(migratingInstance, collectActivityInstanceJobs(migratingInstance));
		parseContext.handleDependentEventSubscriptions(migratingInstance, collectActivityInstanceEventSubscriptions(migratingInstance));
	  }

	  protected internal virtual IList<VariableInstanceEntity> collectActivityInstanceVariables(MigratingActivityInstance instance)
	  {
		IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();
		ExecutionEntity representativeExecution = instance.resolveRepresentativeExecution();
		ExecutionEntity parentExecution = representativeExecution.Parent;

		// decide for representative execution and parent execution whether to none/all/concurrentLocal variables
		// belong to this activity instance
		bool addAllRepresentativeExecutionVariables = instance.SourceScope.Scope || representativeExecution.Concurrent;

		if (addAllRepresentativeExecutionVariables)
		{
		  ((IList<VariableInstanceEntity>)variables).AddRange(representativeExecution.VariablesInternal);
		}
		else
		{
		  ((IList<VariableInstanceEntity>)variables).AddRange(getConcurrentLocalVariables(representativeExecution));
		}

		bool addAnyParentExecutionVariables = parentExecution != null && instance.SourceScope.Scope;
		if (addAnyParentExecutionVariables)
		{
		  bool addAllParentExecutionVariables = parentExecution.Concurrent;

		  if (addAllParentExecutionVariables)
		  {
			((IList<VariableInstanceEntity>)variables).AddRange(parentExecution.VariablesInternal);
		  }
		  else
		  {
			((IList<VariableInstanceEntity>)variables).AddRange(getConcurrentLocalVariables(parentExecution));
		  }
		}

		return variables;
	  }

	  protected internal virtual IList<EventSubscriptionEntity> collectActivityInstanceEventSubscriptions(MigratingActivityInstance migratingInstance)
	  {

		if (migratingInstance.SourceScope.Scope)
		{
		  return migratingInstance.resolveRepresentativeExecution().EventSubscriptions;
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	  protected internal virtual IList<JobEntity> collectActivityInstanceJobs(MigratingActivityInstance migratingInstance)
	  {
		if (migratingInstance.SourceScope.Scope)
		{
		  return migratingInstance.resolveRepresentativeExecution().Jobs;
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	  public static IList<VariableInstanceEntity> getConcurrentLocalVariables(ExecutionEntity execution)
	  {
		IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();

		foreach (VariableInstanceEntity variable in execution.VariablesInternal)
		{
		  if (variable.ConcurrentLocal)
		  {
			variables.Add(variable);
		  }
		}

		return variables;
	  }


	}

}