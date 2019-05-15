using System;
using System.Threading;

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
namespace org.camunda.bpm.container.impl.jboss.util
{

	using AbstractServiceListener = org.jboss.msc.service.AbstractServiceListener;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Substate = org.jboss.msc.service.ServiceController.Substate;
	using Transition = org.jboss.msc.service.ServiceController.Transition;
	using ServiceListener = org.jboss.msc.service.ServiceListener;

	/// <summary>
	/// <para>A <seealso cref="Future"/> implementation backed by a <seealso cref="ServiceListener"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class ServiceListenerFuture<S, V> : AbstractServiceListener<S>, ServiceListener<S>, Future<V>
	{

	  protected internal readonly S serviceInstance;

	  public ServiceListenerFuture(S serviceInstance)
	  {
		this.serviceInstance = serviceInstance;
	  }

	  protected internal V value;
	  internal bool cancelled;
	  internal bool failed;

	  public override void transition<T1>(ServiceController<T1> controller, ServiceController.Transition transition) where T1 : S
	  {
		if (transition.After.Equals(ServiceController.Substate.UP))
		{
		  serviceAvailable();
		  lock (this)
		  {
			Monitor.PulseAll(this);
		  }
		}
		else if (transition.After.Equals(ServiceController.Substate.CANCELLED))
		{
		  lock (this)
		  {
			cancelled = true;
			Monitor.PulseAll(this);
		  }
		}
		else if (transition.After.Equals(ServiceController.Substate.START_FAILED))
		{
		  lock (this)
		  {
			failed = true;
			Monitor.PulseAll(this);
		  }
		}
	  }

	  protected internal abstract void serviceAvailable();

	  public virtual bool cancel(bool mayInterruptIfRunning)
	  {
		// unsupported
		return false;
	  }

	  public virtual bool Cancelled
	  {
		  get
		  {
			// unsupported
			return cancelled;
		  }
	  }

	  public virtual bool Done
	  {
		  get
		  {
			return value != default(V);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get() throws InterruptedException, java.util.concurrent.ExecutionException
	  public virtual V get()
	  {
		if (!failed && !cancelled && value == default(V))
		{
		  lock (this)
		  {
			if (!failed && !cancelled && value == default(V))
			{
			  Monitor.Wait(this);
			}
		  }
		}
		return value;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.ExecutionException, java.util.concurrent.TimeoutException
	  public virtual V get(long timeout, TimeUnit unit)
	  {
		if (!failed && !cancelled && value == default(V))
		{
		  lock (this)
		  {
			if (!failed && !cancelled && value == default(V))
			{
			  Monitor.Wait(this, TimeSpan.FromMilliseconds(unit.convert(timeout, TimeUnit.MILLISECONDS)));
			}
			lock (this)
			{
			  if (value == default(V))
			  {
				throw new TimeoutException();
			  }
			}
		  }
		}
		return value;
	  }

	}

}