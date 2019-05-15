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
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class PvmAtomicOperationProcessStart : AbstractPvmEventAtomicOperation
	{

	  public override bool isAsync(PvmExecutionImpl execution)
	  {
		ProcessInstanceStartContext startContext = execution.ProcessInstanceStartContext;
		return startContext != null && startContext.Async;
	  }

	  public override bool AsyncCapable
	  {
		  get
		  {
			return true;
		  }
	  }

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		return execution.ProcessDefinition;
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
		  }
	  }

	  protected internal virtual PvmExecutionImpl eventNotificationsStarted(PvmExecutionImpl execution)
	  {
		// Note: the following method call initializes the property
		// "processInstanceStartContext" on the given execution.
		// Do not remove it!
		execution.ProcessInstanceStartContext;
		return execution;
	  }

	  protected internal virtual void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {

		execution.continueIfExecutionDoesNotAffectNextOperation(new CallbackAnonymousInnerClass(this, execution)
	   , new CallbackAnonymousInnerClass2(this, execution)
	   , execution);

	  }

	  private class CallbackAnonymousInnerClass : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmAtomicOperationProcessStart outerInstance;

		  private PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass(PvmAtomicOperationProcessStart outerInstance, PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void callback(PvmExecutionImpl execution)
		  {
			execution.dispatchEvent(null);
			return null;
		  }
	  }

	  private class CallbackAnonymousInnerClass2 : Callback<PvmExecutionImpl, Void>
	  {
		  private readonly PvmAtomicOperationProcessStart outerInstance;

		  private PvmExecutionImpl execution;

		  public CallbackAnonymousInnerClass2(PvmAtomicOperationProcessStart outerInstance, PvmExecutionImpl execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void callback(PvmExecutionImpl execution)
		  {
			ProcessInstanceStartContext processInstanceStartContext = execution.ProcessInstanceStartContext;
			InstantiationStack instantiationStack = processInstanceStartContext.InstantiationStack;

			if (instantiationStack.Activities.Count == 0)
			{
			  execution.setActivity(instantiationStack.TargetActivity);
			  execution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE);
			}
			else
			{
			  // initialize the activity instance id
			  execution.ActivityInstanceId = execution.Id;
			  execution.performOperation(PvmAtomicOperation_Fields.ACTIVITY_INIT_STACK);

			}
			return null;
		  }
	  }

	  public override string CanonicalName
	  {
		  get
		  {
			return "process-start";
		  }
	  }

	}

}