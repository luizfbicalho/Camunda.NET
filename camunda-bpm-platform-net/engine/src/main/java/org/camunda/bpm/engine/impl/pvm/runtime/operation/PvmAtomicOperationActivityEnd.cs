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
namespace org.camunda.bpm.engine.impl.pvm.runtime.operation
{

	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class PvmAtomicOperationActivityEnd : PvmAtomicOperation
	{

	  protected internal virtual PvmScope getScope(PvmExecutionImpl execution)
	  {
		return execution.getActivity();
	  }

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		return execution.getActivity().AsyncAfter;
	  }

	  public virtual bool AsyncCapable
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual void execute(PvmExecutionImpl execution)
	  {
		// restore activity instance id
		if (string.ReferenceEquals(execution.ActivityInstanceId, null))
		{
		  execution.ActivityInstanceId = execution.ParentActivityInstanceId;
		}

		PvmActivity activity = execution.getActivity();
		IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping = execution.createActivityExecutionMapping();

		PvmExecutionImpl propagatingExecution = execution;

		if (execution.Scope && activity.Scope)
		{
		  if (!LegacyBehavior.destroySecondNonScope(execution))
		  {
			execution.destroy();
			if (!execution.Concurrent)
			{
			  execution.remove();
			  propagatingExecution = execution.Parent;
			  propagatingExecution.setActivity(execution.getActivity());
			}
		  }
		}

		propagatingExecution = LegacyBehavior.determinePropagatingExecutionOnEnd(propagatingExecution, activityExecutionMapping);
		PvmScope flowScope = activity.FlowScope;

		// 1. flow scope = Process Definition
		if (flowScope == activity.ProcessDefinition)
		{

		  // 1.1 concurrent execution => end + tryPrune()
		  if (propagatingExecution.Concurrent)
		  {
			propagatingExecution.remove();
			propagatingExecution.Parent.tryPruneLastConcurrentChild();
			propagatingExecution.Parent.forceUpdate();
		  }
		  else
		  {
			// 1.2 Process End
			propagatingExecution.Ended = true;
			if (!propagatingExecution.PreserveScope)
			{
			  propagatingExecution.performOperation(PvmAtomicOperation_Fields.PROCESS_END);
			}
		  }
		}
		else
		{
		  // 2. flowScope != process definition
		  PvmActivity flowScopeActivity = (PvmActivity) flowScope;

		  ActivityBehavior activityBehavior = flowScopeActivity.ActivityBehavior;
		  if (activityBehavior is CompositeActivityBehavior)
		  {
			CompositeActivityBehavior compositeActivityBehavior = (CompositeActivityBehavior) activityBehavior;
			// 2.1 Concurrent execution => composite behavior.concurrentExecutionEnded()
			if (propagatingExecution.Concurrent && !LegacyBehavior.isConcurrentScope(propagatingExecution))
			{
			  compositeActivityBehavior.concurrentChildExecutionEnded(propagatingExecution.Parent, propagatingExecution);
			}
			else
			{
			  // 2.2 Scope Execution => composite behavior.complete()
			  propagatingExecution.setActivity(flowScopeActivity);
			  compositeActivityBehavior.complete(propagatingExecution);
			}

		  }
		  else
		  {
			// activity behavior is not composite => this is unexpected
			throw new ProcessEngineException("Expected behavior of composite scope " + activity + " to be a CompositeActivityBehavior but got " + activityBehavior);
		  }
		}
	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "activity-end";
		  }
	  }

	}

}