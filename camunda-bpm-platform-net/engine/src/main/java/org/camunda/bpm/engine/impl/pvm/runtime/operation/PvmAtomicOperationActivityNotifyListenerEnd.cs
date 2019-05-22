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

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class PvmAtomicOperationActivityNotifyListenerEnd : PvmAtomicOperationActivityInstanceEnd
	{

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		return execution.getActivity();
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

		// perform activity end behavior
		execution.dispatchDelayedEventsAndPerformOperation(new CallbackAnonymousInnerClass(this, execution));
	  }

	  private class CallbackAnonymousInnerClass : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmAtomicOperationActivityNotifyListenerEnd outerInstance;

		  private PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass(PvmAtomicOperationActivityNotifyListenerEnd outerInstance, PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }


		  public Void callback(PvmExecutionImpl execution)
		  {
			execution.leaveActivityInstance();
			execution.ActivityInstanceId = null;
			execution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_END);
			return null;
		  }
	  }

	  public override string CanonicalName
	  {
		  get
		  {
			return "activity-notify-listener-end";
		  }
	  }

	  public override bool shouldHandleFailureAsBpmnError()
	  {
		return true;
	  }
	}

}