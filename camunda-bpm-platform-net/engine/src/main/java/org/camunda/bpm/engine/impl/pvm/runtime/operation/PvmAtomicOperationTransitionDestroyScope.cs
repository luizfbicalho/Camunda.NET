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


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class PvmAtomicOperationTransitionDestroyScope : PvmAtomicOperation
	{

	  private static readonly PvmLogger LOG = ProcessEngineLogger.PVM_LOGGER;

	  public virtual bool isAsync(PvmExecutionImpl instance)
	  {
		return false;
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

		// calculate the propagating execution
		PvmExecutionImpl propagatingExecution = execution;

		PvmActivity activity = execution.getActivity();
		IList<PvmTransition> transitionsToTake = execution.TransitionsToTake;
		execution.TransitionsToTake = null;

		// check whether the current scope needs to be destroyed
		if (execution.Scope && activity.Scope)
		{

		  if (!LegacyBehavior.destroySecondNonScope(execution))
		  {
			if (execution.Concurrent)
			{
			  // legacy behavior
			  LegacyBehavior.destroyConcurrentScope(execution);
			}
			else
			{
			  propagatingExecution = execution.Parent;
			  LOG.debugDestroyScope(execution, propagatingExecution);
			  execution.destroy();
			  propagatingExecution.setActivity(execution.getActivity());
			  propagatingExecution.setTransition(execution.getTransition());
			  propagatingExecution.Active = true;
			  execution.remove();
			}
		  }

		}
		else
		{
		  // activity is not scope => nothing to do
		  propagatingExecution = execution;
		}

		// take the specified transitions
		if (transitionsToTake.Count == 0)
		{
		  throw new ProcessEngineException(execution.ToString() + ": No outgoing transitions from " + "activity " + activity);
		}
		else if (transitionsToTake.Count == 1)
		{
		  propagatingExecution.setTransition(transitionsToTake[0]);
		  propagatingExecution.take();
		}
		else
		{
		  propagatingExecution.inactivate();

		  IList<OutgoingExecution> outgoingExecutions = new List<OutgoingExecution>();

		  for (int i = 0; i < transitionsToTake.Count; i++)
		  {
			PvmTransition transition = transitionsToTake[i];

			PvmExecutionImpl scopeExecution = propagatingExecution.Scope ? propagatingExecution : propagatingExecution.Parent;

			// reuse concurrent, propagating execution for first transition
			PvmExecutionImpl concurrentExecution = null;
			if (i == 0)
			{
			  concurrentExecution = propagatingExecution;
			}
			else
			{
			  concurrentExecution = scopeExecution.createConcurrentExecution();

			  if (i == 1 && !propagatingExecution.Concurrent)
			  {
				outgoingExecutions.RemoveAt(0);
				// get a hold of the concurrent execution that replaced the scope propagating execution
				PvmExecutionImpl replacingExecution = null;
				foreach (PvmExecutionImpl concurrentChild in scopeExecution.NonEventScopeExecutions)
				{
				  if (!(concurrentChild == propagatingExecution))
				  {
					replacingExecution = concurrentChild;
					break;
				  }
				}

				outgoingExecutions.Add(new OutgoingExecution(replacingExecution, transitionsToTake[0]));
			  }
			}

			outgoingExecutions.Add(new OutgoingExecution(concurrentExecution, transition));
		  }

		  // start executions in reverse order (order will be reversed again in command context with the effect that they are
		  // actually be started in correct order :) )
		  outgoingExecutions.Reverse();

		  foreach (OutgoingExecution outgoingExecution in outgoingExecutions)
		  {
			outgoingExecution.take();
		  }
		}

	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "transition-destroy-scope";
		  }
	  }
	}

}