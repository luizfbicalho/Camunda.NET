﻿/*
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
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;


	/// 
	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public abstract class AbstractPvmAtomicOperationTransitionNotifyListenerTake : AbstractPvmEventAtomicOperation
	{

	  protected internal virtual void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {
		PvmActivity destination = execution.getTransition().getDestination();

		// check start behavior of next activity
		switch (destination.ActivityStartBehavior)
		{
		case DEFAULT:
		  execution.setActivity(destination);
		  execution.dispatchDelayedEventsAndPerformOperation(PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE);
		  break;
		case INTERRUPT_FLOW_SCOPE:
		  execution.setActivity(null);
		  execution.performOperation(PvmAtomicOperation_Fields.TRANSITION_INTERRUPT_FLOW_SCOPE);
		  break;
		default:
		  throw new ProcessEngineException("Unsupported start behavior for activity '" + destination + "' started from a sequence flow: " + destination.ActivityStartBehavior);
		}
	  }

	  protected internal override CoreModelElement getScope(PvmExecutionImpl execution)
	  {
		return execution.getTransition();
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE;
		  }
	  }

	}

}