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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingInstanceParseContext = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using Callback = org.camunda.bpm.engine.impl.pvm.runtime.Callback;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ParallelMultiInstanceActivityBehavior : MultiInstanceActivityBehavior, MigrationObserverBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void createInstances(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, int nrOfInstances) throws Exception
	  protected internal override void createInstances(ActivityExecution execution, int nrOfInstances)
	  {
		PvmActivity innerActivity = getInnerActivity(execution.Activity);

		// initialize the scope and create the desired number of child executions
		prepareScopeExecution(execution, nrOfInstances);

		IList<ActivityExecution> concurrentExecutions = new List<ActivityExecution>();
		for (int i = 0; i < nrOfInstances; i++)
		{
		  concurrentExecutions.Add(createConcurrentExecution(execution));
		}

		// start the concurrent child executions
		// start executions in reverse order (order will be reversed again in command context with the effect that they are
		// actually be started in correct order :) )
		for (int i = (nrOfInstances - 1); i >= 0; i--)
		{
		  ActivityExecution activityExecution = concurrentExecutions[i];
		  performInstance(activityExecution, innerActivity, i);
		}
	  }

	  protected internal virtual void prepareScopeExecution(ActivityExecution scopeExecution, int nrOfInstances)
	  {
		// set the MI-body scoped variables
		setLoopVariable(scopeExecution, NUMBER_OF_INSTANCES, nrOfInstances);
		setLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES, 0);
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfInstances);
		scopeExecution.Activity = null;
		scopeExecution.inactivate();
	  }

	  protected internal virtual ActivityExecution createConcurrentExecution(ActivityExecution scopeExecution)
	  {
		ActivityExecution concurrentChild = scopeExecution.createExecution();
		scopeExecution.forceUpdate();
		concurrentChild.Concurrent = true;
		concurrentChild.Scope = false;
		return concurrentChild;
	  }

	  public override void concurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {

		int nrOfCompletedInstances = getLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES) + 1;
		setLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES, nrOfCompletedInstances);
		int nrOfActiveInstances = getLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES) - 1;
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfActiveInstances);

		// inactivate the concurrent execution
		endedExecution.inactivate();
		endedExecution.ActivityInstanceId = null;

		// join
		scopeExecution.forceUpdate();
		// TODO: should the completion condition be evaluated on the scopeExecution or on the endedExecution?
		if (completionConditionSatisfied(endedExecution) || allExecutionsEnded(scopeExecution, endedExecution))
		{

		  List<ActivityExecution> childExecutions = new List<ActivityExecution>(((PvmExecutionImpl) scopeExecution).NonEventScopeExecutions);
		  foreach (ActivityExecution childExecution in childExecutions)
		  {
			// delete all not-ended instances; these are either active (for non-scope tasks) or inactive but have no activity id (for subprocesses, etc.)
			if (childExecution.Active || childExecution.Activity == null)
			{
			  ((PvmExecutionImpl)childExecution).deleteCascade("Multi instance completion condition satisfied.");
			}
			else
			{
			  childExecution.remove();
			}
		  }

		  scopeExecution.Activity = (PvmActivity) endedExecution.Activity.FlowScope;
		  scopeExecution.Active = true;
		  leave(scopeExecution);
		}
		else
		{
		  ((ExecutionEntity) scopeExecution).dispatchDelayedEventsAndPerformOperation((Callback<PvmExecutionImpl, Void>) null);
		}
	  }

	  protected internal virtual bool allExecutionsEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {
		int numberOfInactiveConcurrentExecutions = endedExecution.findInactiveConcurrentExecutions(endedExecution.Activity).Count;
		int concurrentExecutions = scopeExecution.Executions.Count;

		// no active instances exist and all concurrent executions are inactive
		return getLocalLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES) <= 0 && numberOfInactiveConcurrentExecutions == concurrentExecutions;
	  }

	  public override void complete(ActivityExecution scopeExecution)
	  {
		// can't happen
	  }

	  public override IList<ActivityExecution> initializeScope(ActivityExecution scopeExecution, int numberOfInstances)
	  {

		prepareScopeExecution(scopeExecution, numberOfInstances);

		IList<ActivityExecution> executions = new List<ActivityExecution>();
		for (int i = 0; i < numberOfInstances; i++)
		{
		  ActivityExecution concurrentChild = createConcurrentExecution(scopeExecution);
		  setLoopVariable(concurrentChild, LOOP_COUNTER, i);
		  executions.Add(concurrentChild);
		}

		return executions;
	  }

	  public override ActivityExecution createInnerInstance(ActivityExecution scopeExecution)
	  {
		// even though there is only one instance, there is always a concurrent child
		ActivityExecution concurrentChild = createConcurrentExecution(scopeExecution);

		int nrOfInstances = getLoopVariable(scopeExecution, NUMBER_OF_INSTANCES).Value;
		setLoopVariable(scopeExecution, NUMBER_OF_INSTANCES, nrOfInstances + 1);
		int nrOfActiveInstances = getLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES).Value;
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfActiveInstances + 1);

		setLoopVariable(concurrentChild, LOOP_COUNTER, nrOfInstances);

		return concurrentChild;
	  }

	  public override void destroyInnerInstance(ActivityExecution concurrentExecution)
	  {

		ActivityExecution scopeExecution = concurrentExecution.Parent;
		concurrentExecution.remove();
		scopeExecution.forceUpdate();

		int nrOfActiveInstances = getLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES).Value;
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfActiveInstances - 1);

	  }

	  public virtual void migrateScope(ActivityExecution scopeExecution)
	  {
		// migrate already completed instances
		foreach (ActivityExecution child in scopeExecution.Executions)
		{
		  if (!child.Active)
		  {
			((PvmExecutionImpl) child).ProcessDefinition = ((PvmExecutionImpl) scopeExecution).ProcessDefinition;
		  }
		}
	  }

	  public virtual void onParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		ExecutionEntity scopeExecution = migratingInstance.resolveRepresentativeExecution();

		IList<ActivityExecution> concurrentInActiveExecutions = scopeExecution.findInactiveChildExecutions(getInnerActivity((ActivityImpl) migratingInstance.SourceScope));

		// variables on ended inner instance executions need not be migrated anywhere
		// since they are also not represented in the tree of migrating instances, we remove
		// them from the parse context here to avoid a validation exception
		foreach (ActivityExecution execution in concurrentInActiveExecutions)
		{
		  foreach (VariableInstanceEntity variable in ((ExecutionEntity) execution).VariablesInternal)
		  {
			parseContext.consume(variable);
		  }
		}

	  }

	}

}