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
	using ActivityStartBehavior = org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior;

	/// <summary>
	/// Implements <seealso cref="ActivityStartBehavior#CANCEL_EVENT_SCOPE"/>.
	/// 
	/// @author Throben Lindhauer
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class PvmAtomicOperationCancelActivity : PvmAtomicOperation
	{
		public abstract bool AsyncCapable {get;}

	  public virtual void execute(PvmExecutionImpl execution)
	  {

		// Assumption: execution is scope
		PvmActivity cancellingActivity = execution.NextActivity;
		execution.NextActivity = null;

		// first, cancel and destroy the current scope
		execution.Active = true;

		PvmExecutionImpl propagatingExecution = null;

		if (LegacyBehavior.isConcurrentScope(execution))
		{
		  // this is legacy behavior
		  LegacyBehavior.cancelConcurrentScope(execution, (PvmActivity) cancellingActivity.EventScope);
		  propagatingExecution = execution;
		}
		else
		{
		  // Unlike PvmAtomicOperationTransitionDestroyScope this needs to use delete() (instead of destroy() and remove()).
		  // The reason is that PvmAtomicOperationTransitionDestroyScope is executed when a scope (or non scope) is left using
		  // a sequence flow. In that case the execution will have completed all the work inside the current activity
		  // and will have no more child executions. In PvmAtomicOperationCancelScope the scope is cancelled due to
		  // a boundary event firing. In that case the execution has not completed all the work in the current scope / activity
		  // and it is necessary to delete the complete hierarchy of executions below and including the execution itself.
		  execution.deleteCascade("Cancel scope activity " + cancellingActivity + " executed.");
		  propagatingExecution = execution.Parent;
		}

		propagatingExecution.setActivity(cancellingActivity);
		propagatingExecution.Active = true;
		propagatingExecution.Ended = false;
		activityCancelled(propagatingExecution);
	  }

	  protected internal abstract void activityCancelled(PvmExecutionImpl execution);

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		return false;
	  }

	}

}