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
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TransitionInstanceHandler : MigratingInstanceParseHandler<TransitionInstance>
	{

	  public virtual void handle(MigratingInstanceParseContext parseContext, TransitionInstance transitionInstance)
	  {

		if (!isAsyncTransitionInstance(transitionInstance))
		{
		  return;
		}

		MigrationInstruction applyingInstruction = parseContext.getInstructionFor(transitionInstance.ActivityId);

		ScopeImpl sourceScope = parseContext.SourceProcessDefinition.findActivity(transitionInstance.ActivityId);
		ScopeImpl targetScope = null;

		if (applyingInstruction != null)
		{
		  string activityId = applyingInstruction.TargetActivityId;
		  targetScope = parseContext.TargetProcessDefinition.findActivity(activityId);
		}

		ExecutionEntity asyncExecution = Context.CommandContext.ExecutionManager.findExecutionById(transitionInstance.ExecutionId);

		MigratingTransitionInstance migratingTransitionInstance = parseContext.MigratingProcessInstance.addTransitionInstance(applyingInstruction, transitionInstance, sourceScope, targetScope, asyncExecution);

		MigratingActivityInstance parentInstance = parseContext.getMigratingActivityInstanceById(transitionInstance.ParentActivityInstanceId);
		migratingTransitionInstance.setParent(parentInstance);

		IList<JobEntity> jobs = asyncExecution.Jobs;
		parseContext.handleDependentTransitionInstanceJobs(migratingTransitionInstance, jobs);

		parseContext.handleDependentVariables(migratingTransitionInstance, collectTransitionInstanceVariables(migratingTransitionInstance));

	  }

	  /// <summary>
	  /// Workaround for CAM-5609: In general, only async continuations should be represented as TransitionInstances, but
	  /// due to this bug, completed multi-instances are represented like that as well. We tolerate the second case.
	  /// </summary>
	  protected internal virtual bool isAsyncTransitionInstance(TransitionInstance transitionInstance)
	  {
		string executionId = transitionInstance.ExecutionId;
		ExecutionEntity execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
		foreach (JobEntity job in execution.Jobs)
		{
		  if (AsyncContinuationJobHandler.TYPE.Equals(job.JobHandlerType))
		  {
			return true;
		  }
		}

		return false;
	  }

	  protected internal virtual IList<VariableInstanceEntity> collectTransitionInstanceVariables(MigratingTransitionInstance instance)
	  {
		IList<VariableInstanceEntity> variables = new List<VariableInstanceEntity>();
		ExecutionEntity representativeExecution = instance.resolveRepresentativeExecution();

		if (representativeExecution.Concurrent)
		{
		  ((IList<VariableInstanceEntity>)variables).AddRange(representativeExecution.VariablesInternal);
		}
		else
		{
		  ((IList<VariableInstanceEntity>)variables).AddRange(ActivityInstanceHandler.getConcurrentLocalVariables(representativeExecution));
		}

		return variables;
	  }

	}

}