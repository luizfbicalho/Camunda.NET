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
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class PvmAtomicOperationDeleteCascadeFireActivityEnd : PvmAtomicOperationActivityInstanceEnd
	{

	  protected internal override PvmExecutionImpl eventNotificationsStarted(PvmExecutionImpl execution)
	  {
		execution.Canceled = true;
		return base.eventNotificationsStarted(execution);
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
		  // TODO: when can this happen?
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
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {

		PvmActivity activity = execution.getActivity();

		if (execution.Scope && (executesNonScopeActivity(execution) || isAsyncBeforeActivity(execution)) && !CompensationBehavior.executesNonScopeCompensationHandler(execution))
		{
		  execution.removeAllTasks();
		  // case this is a scope execution and the activity is not a scope
		  execution.leaveActivityInstance();
		  execution.setActivity(getFlowScopeActivity(activity));
		  execution.performOperation(PvmAtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);

		}
		else
		{
		  if (execution.Scope)
		  {
			execution.destroy();
		  }

		  // remove this execution and its concurrent parent (if exists)
		  execution.remove();

		  bool continueRemoval = !execution.DeleteRoot;

		  if (continueRemoval)
		  {
			PvmExecutionImpl propagatingExecution = execution.Parent;
			if (propagatingExecution != null && !propagatingExecution.Scope && !propagatingExecution.hasChildren())
			{
			  propagatingExecution.remove();
			  continueRemoval = !propagatingExecution.DeleteRoot;
			  propagatingExecution = propagatingExecution.Parent;
			}

			if (continueRemoval)
			{
			  if (propagatingExecution != null)
			  {
				// continue deletion with the next scope execution
				// set activity on parent in case the parent is an inactive scope execution and activity has been set to 'null'.
				if (propagatingExecution.getActivity() == null && activity != null && activity.FlowScope != null)
				{
				  propagatingExecution.setActivity(getFlowScopeActivity(activity));
				}
			  }
			}
		  }
		}
	  }

	  protected internal virtual bool executesNonScopeActivity(PvmExecutionImpl execution)
	  {
		ActivityImpl activity = execution.getActivity();
		return activity != null && !activity.Scope;
	  }

	  protected internal virtual bool isAsyncBeforeActivity(PvmExecutionImpl execution)
	  {
		return !string.ReferenceEquals(execution.ActivityId, null) && string.ReferenceEquals(execution.ActivityInstanceId, null);
	  }

	  protected internal virtual ActivityImpl getFlowScopeActivity(PvmActivity activity)
	  {
		ScopeImpl flowScope = activity.FlowScope;
		ActivityImpl flowScopeActivity = null;
		if (flowScope.ProcessDefinition != flowScope)
		{
		  flowScopeActivity = (ActivityImpl) flowScope;
		}
		return flowScopeActivity;
	  }

	  public override string CanonicalName
	  {
		  get
		  {
			return "delete-cascade-fire-activity-end";
		  }
	  }
	}

}