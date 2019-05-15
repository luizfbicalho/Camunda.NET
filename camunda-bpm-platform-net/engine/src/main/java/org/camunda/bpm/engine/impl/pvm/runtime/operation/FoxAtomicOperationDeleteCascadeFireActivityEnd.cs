using System;
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

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;


	public class FoxAtomicOperationDeleteCascadeFireActivityEnd : PvmAtomicOperationDeleteCascadeFireActivityEnd
	{

	  protected internal override void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {
		PvmActivity activity = execution.getActivity();
		if ((execution.Scope) && (activity != null) && (!activity.Scope))
		{
		  execution.setActivity((PvmActivity) activity.FlowScope);
		  execution.performOperation(this);

		}
		else
		{
		  if (execution.Scope)
		  {
			execution.destroy();
		  }

		  execution.remove();
		}
	  }

	  public override void execute(PvmExecutionImpl execution)
	  {
		ScopeImpl scope = getScope(execution);
		int executionListenerIndex = execution.ListenerIndex;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> executionListeners = scope.getListeners(getEventName());
		IList<DelegateListener<BaseDelegateExecution>> executionListeners = scope.getListeners(EventName);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution> listener : executionListeners)
		foreach (DelegateListener<BaseDelegateExecution> listener in executionListeners)
		{
		  execution.EventName = EventName;
		  execution.EventSource = scope;
		  try
		  {
			execution.invokeListener(listener);
		  }
		  catch (Exception e)
		  {
			throw e;
		  }
		  catch (Exception e)
		  {
			throw new PvmException("couldn't execute event listener : " + e.Message, e);
		  }
		  executionListenerIndex += 1;
		  execution.ListenerIndex = executionListenerIndex;
		}
		execution.ListenerIndex = 0;
		execution.EventName = null;
		execution.EventSource = null;

		eventNotificationsCompleted(execution);
	  }


	}

}