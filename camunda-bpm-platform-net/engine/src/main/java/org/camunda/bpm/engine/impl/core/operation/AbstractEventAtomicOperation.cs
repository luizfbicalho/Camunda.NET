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
namespace org.camunda.bpm.engine.impl.core.operation
{

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class AbstractEventAtomicOperation<T> : CoreAtomicOperation<T> where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	{
		public abstract string CanonicalName {get;}

	  public virtual bool isAsync(T execution)
	  {
		return false;
	  }

	  public virtual void execute(T execution)
	  {
		CoreModelElement scope = getScope(execution);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> listeners = getListeners(scope, execution);
		IList<DelegateListener<BaseDelegateExecution>> listeners = getListeners(scope, execution);
		int listenerIndex = execution.ListenerIndex;

		if (listenerIndex == 0)
		{
		  execution = eventNotificationsStarted(execution);
		}

		if (!isSkipNotifyListeners(execution))
		{

		  if (listeners.Count > listenerIndex)
		  {
			execution.EventName = EventName;
			execution.EventSource = scope;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution> listener = listeners.get(listenerIndex);
			DelegateListener<BaseDelegateExecution> listener = listeners[listenerIndex];
			execution.ListenerIndex = listenerIndex + 1;

			try
			{
			  execution.invokeListener(listener);
			}
			catch (Exception ex)
			{
			  eventNotificationsFailed(execution, ex);
			  // do not continue listener invocation once a listener has failed
			  return;
			}

			execution.performOperationSync(this);
		  }
		  else
		  {
			resetListeners(execution);

			eventNotificationsCompleted(execution);
		  }

		}
		else
		{
		  eventNotificationsCompleted(execution);

		}
	  }

	  protected internal virtual void resetListeners(T execution)
	  {
		execution.ListenerIndex = 0;
		execution.EventName = null;
		execution.EventSource = null;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> getListeners(org.camunda.bpm.engine.impl.core.model.CoreModelElement scope, T execution)
	  protected internal virtual IList<DelegateListener<BaseDelegateExecution>> getListeners(CoreModelElement scope, T execution)
	  {
		if (execution.SkipCustomListeners)
		{
		  return scope.getBuiltInListeners(EventName);
		}
		else
		{
		  return scope.getListeners(EventName);
		}
	  }

	  protected internal virtual bool isSkipNotifyListeners(T execution)
	  {
		return false;
	  }

	  protected internal virtual T eventNotificationsStarted(T execution)
	  {
		// do nothing
		return execution;
	  }

	  protected internal abstract CoreModelElement getScope(T execution);
	  protected internal abstract string EventName {get;}
	  protected internal abstract void eventNotificationsCompleted(T execution);

	  protected internal virtual void eventNotificationsFailed(T execution, Exception exception)
	  {
		if (exception is Exception)
		{
		  throw (Exception) exception;
		}
		else
		{
		  throw new PvmException("couldn't execute event listener : " + exception.Message, exception);
		}
	  }
	}

}