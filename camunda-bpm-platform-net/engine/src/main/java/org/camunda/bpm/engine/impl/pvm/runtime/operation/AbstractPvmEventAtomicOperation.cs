using System;

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
	using BpmnExceptionHandler = org.camunda.bpm.engine.impl.bpmn.helper.BpmnExceptionHandler;
	using ErrorPropagationException = org.camunda.bpm.engine.impl.bpmn.helper.ErrorPropagationException;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using AbstractEventAtomicOperation = org.camunda.bpm.engine.impl.core.operation.AbstractEventAtomicOperation;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public abstract class AbstractPvmEventAtomicOperation : AbstractEventAtomicOperation<PvmExecutionImpl>, PvmAtomicOperation
	{
		public override abstract string CanonicalName {get;}

	  protected internal abstract CoreModelElement getScope(PvmExecutionImpl execution);

	  public virtual bool AsyncCapable
	  {
		  get
		  {
			return false;
		  }
	  }

	  protected internal override void eventNotificationsFailed(PvmExecutionImpl execution, Exception exception)
	  {

		if (shouldHandleFailureAsBpmnError())
		{
		  ActivityExecution activityExecution = (ActivityExecution) execution;
		  try
		  {
			resetListeners(execution);
			BpmnExceptionHandler.propagateException(activityExecution, exception);
		  }
		  catch (ErrorPropagationException)
		  {
			// exception has been logged by thrower
			// re-throw the original exception so that it is logged
			// and set as cause of the failure
			base.eventNotificationsFailed(execution, exception);
		  }
		  catch (Exception e)
		  {
			base.eventNotificationsFailed(execution, e);
		  }
		}
		else
		{
		  base.eventNotificationsFailed(execution, exception);
		}
	  }

	  public virtual bool shouldHandleFailureAsBpmnError()
	  {
		return false;
	  }
	}

}