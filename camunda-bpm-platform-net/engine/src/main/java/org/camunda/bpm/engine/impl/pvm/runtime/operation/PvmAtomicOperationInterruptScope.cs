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
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class PvmAtomicOperationInterruptScope : PvmAtomicOperation
	{

	  public virtual void execute(PvmExecutionImpl execution)
	  {
		PvmActivity interruptingActivity = getInterruptingActivity(execution);

		PvmExecutionImpl scopeExecution = !execution.Scope ? execution.Parent : execution;

		if (scopeExecution != execution)
		{
		  // remove the current execution before interrupting and continuing executing the interrupted activity
		  // reason:
		  //   * interrupting should not attempt to fire end events for this execution
		  //   * the interruptingActivity is executed with the scope execution
		  execution.remove();
		}

		scopeExecution.interrupt("Interrupting activity " + interruptingActivity + " executed.");

		scopeExecution.setActivity(interruptingActivity);
		scopeExecution.Active = true;
		scopeExecution.setTransition(execution.getTransition());
		scopeInterrupted(scopeExecution);
	  }

	  protected internal abstract void scopeInterrupted(PvmExecutionImpl execution);

	  protected internal abstract PvmActivity getInterruptingActivity(PvmExecutionImpl execution);

	  public virtual bool isAsync(PvmExecutionImpl execution)
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

	}

}