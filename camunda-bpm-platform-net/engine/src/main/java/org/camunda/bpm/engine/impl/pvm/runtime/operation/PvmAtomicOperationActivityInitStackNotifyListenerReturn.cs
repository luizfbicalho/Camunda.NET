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
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class PvmAtomicOperationActivityInitStackNotifyListenerReturn : PvmAtomicOperationActivityInstanceStart
	{

	  public override string CanonicalName
	  {
		  get
		  {
			return "activity-init-stack-notify-listener-return";
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

		ExecutionStartContext startContext = execution.ExecutionStartContext;
		InstantiationStack instantiationStack = startContext.InstantiationStack;

		// if the stack has been instantiated
		if (instantiationStack.Activities.Count == 0)
		{
		  // done
		  return;
		}
		else
		{
		  // else instantiate the activity stack further
		  execution.setActivity(null);
		  execution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_INIT_STACK_AND_RETURN);

		}

	  }

	}

}