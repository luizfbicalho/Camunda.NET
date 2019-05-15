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

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ModificationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ModificationObserverBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class PvmAtomicOperationActivityInitStackNotifyListenerStart : PvmAtomicOperationActivityInstanceStart
	{

	  public override string CanonicalName
	  {
		  get
		  {
			return "activity-init-stack-notify-listener-start";
		  }
	  }

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		ActivityImpl activity = execution.getActivity();

		if (activity != null)
		{
		  return activity;
		}
		else
		{
		  PvmExecutionImpl parent = execution.Parent;
		  if (parent != null)
		  {
			return getScope(execution.Parent);
		  }
		  return execution.ProcessDefinition;
		}
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {
		base.eventNotificationsCompleted(execution);

		execution.activityInstanceStarted();

		ExecutionStartContext startContext = execution.ExecutionStartContext;
		InstantiationStack instantiationStack = startContext.InstantiationStack;

		PvmExecutionImpl propagatingExecution = execution;
		ActivityImpl activity = execution.getActivity();
		if (activity.ActivityBehavior is ModificationObserverBehavior)
		{
		  ModificationObserverBehavior behavior = (ModificationObserverBehavior) activity.ActivityBehavior;
		  IList<ActivityExecution> concurrentExecutions = behavior.initializeScope(propagatingExecution, 1);
		  propagatingExecution = (PvmExecutionImpl) concurrentExecutions[0];
		}

		// if the stack has been instantiated
		if (instantiationStack.Activities.Count == 0 && instantiationStack.TargetActivity != null)
		{
		  // as if we are entering the target activity instance id via a transition
		  propagatingExecution.ActivityInstanceId = null;

		  // execute the target activity with this execution
		  startContext.applyVariables(propagatingExecution);
		  propagatingExecution.setActivity(instantiationStack.TargetActivity);
		  propagatingExecution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE);

		}
		else if (instantiationStack.Activities.Count == 0 && instantiationStack.TargetTransition != null)
		{
		  // as if we are entering the target activity instance id via a transition
		  propagatingExecution.ActivityInstanceId = null;

		  // execute the target transition with this execution
		  PvmTransition transition = instantiationStack.TargetTransition;
		  startContext.applyVariables(propagatingExecution);
		  propagatingExecution.setActivity(transition.Source);
		  propagatingExecution.setTransition((TransitionImpl) transition);
		  propagatingExecution.performOperation(PvmAtomicOperation_Fields.TRANSITION_START_NOTIFY_LISTENER_TAKE);
		}
		else
		{
		  // else instantiate the activity stack further
		  propagatingExecution.setActivity(null);
		  propagatingExecution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_INIT_STACK);

		}

	  }

	}

}