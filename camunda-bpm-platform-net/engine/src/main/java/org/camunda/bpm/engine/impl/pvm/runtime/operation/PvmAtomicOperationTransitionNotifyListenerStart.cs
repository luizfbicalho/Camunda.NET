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
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmAtomicOperationTransitionNotifyListenerStart : PvmAtomicOperationActivityInstanceStart
	{

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		return execution.getActivity();
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

		TransitionImpl transition = execution.getTransition();
		PvmActivity destination;
		if (transition == null)
		{ // this is null after async cont. -> transition is not stored in execution
		  destination = execution.getActivity();
		}
		else
		{
		  destination = transition.getDestination();
		}
		execution.setTransition(null);
		execution.setActivity(destination);

		ExecutionStartContext executionStartContext = execution.ExecutionStartContext;
		if (executionStartContext != null)
		{
		  executionStartContext.executionStarted(execution);
		  execution.disposeExecutionStartContext();
		}

		execution.dispatchDelayedEventsAndPerformOperation(PvmAtomicOperation_Fields.ACTIVITY_EXECUTE);
	  }

	  public override string CanonicalName
	  {
		  get
		  {
			return "transition-notifiy-listener-start";
		  }
	  }

	  public override bool shouldHandleFailureAsBpmnError()
	  {
		return true;
	  }
	}

}