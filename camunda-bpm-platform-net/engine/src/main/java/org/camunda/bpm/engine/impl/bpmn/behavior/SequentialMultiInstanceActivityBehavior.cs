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

	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class SequentialMultiInstanceActivityBehavior : MultiInstanceActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void createInstances(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, int nrOfInstances) throws Exception
	  protected internal override void createInstances(ActivityExecution execution, int nrOfInstances)
	  {

		prepareScope(execution, nrOfInstances);
		setLoopVariable(execution, NUMBER_OF_ACTIVE_INSTANCES, 1);

		ActivityImpl innerActivity = getInnerActivity(execution.Activity);
		performInstance(execution, innerActivity, 0);
	  }

	  public override void complete(ActivityExecution scopeExecution)
	  {
		int loopCounter = getLoopVariable(scopeExecution, LOOP_COUNTER) + 1;
		int nrOfInstances = getLoopVariable(scopeExecution, NUMBER_OF_INSTANCES).Value;
		int nrOfCompletedInstances = getLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES) + 1;

		setLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES, nrOfCompletedInstances);

		if (loopCounter == nrOfInstances || completionConditionSatisfied(scopeExecution))
		{
		  leave(scopeExecution);
		}
		else
		{
		  PvmActivity innerActivity = getInnerActivity(scopeExecution.Activity);
		  performInstance(scopeExecution, innerActivity, loopCounter);
		}
	  }

	  public override void concurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {
		// cannot happen
	  }

	  protected internal virtual void prepareScope(ActivityExecution scopeExecution, int totalNumberOfInstances)
	  {
		setLoopVariable(scopeExecution, NUMBER_OF_INSTANCES, totalNumberOfInstances);
		setLoopVariable(scopeExecution, NUMBER_OF_COMPLETED_INSTANCES, 0);
	  }

	  public override IList<ActivityExecution> initializeScope(ActivityExecution scopeExecution, int nrOfInstances)
	  {
		if (nrOfInstances > 1)
		{
		  LOG.unsupportedConcurrencyException(scopeExecution.ToString(), this.GetType().Name);
		}

		IList<ActivityExecution> executions = new List<ActivityExecution>();

		prepareScope(scopeExecution, nrOfInstances);
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfInstances);

		if (nrOfInstances > 0)
		{
		  setLoopVariable(scopeExecution, LOOP_COUNTER, 0);
		  executions.Add(scopeExecution);
		}

		return executions;
	  }

	  public override ActivityExecution createInnerInstance(ActivityExecution scopeExecution)
	  {

		if (hasLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES) && getLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES) > 0)
		{
		  throw LOG.unsupportedConcurrencyException(scopeExecution.ToString(), this.GetType().Name);
		}
		else
		{
		  int nrOfInstances = getLoopVariable(scopeExecution, NUMBER_OF_INSTANCES).Value;

		  setLoopVariable(scopeExecution, LOOP_COUNTER, nrOfInstances);
		  setLoopVariable(scopeExecution, NUMBER_OF_INSTANCES, nrOfInstances + 1);
		  setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, 1);
		}

		return scopeExecution;
	  }

	  public override void destroyInnerInstance(ActivityExecution scopeExecution)
	  {
		removeLoopVariable(scopeExecution, LOOP_COUNTER);

		int nrOfActiveInstances = getLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES).Value;
		setLoopVariable(scopeExecution, NUMBER_OF_ACTIVE_INSTANCES, nrOfActiveInstances - 1);
	  }

	}

}