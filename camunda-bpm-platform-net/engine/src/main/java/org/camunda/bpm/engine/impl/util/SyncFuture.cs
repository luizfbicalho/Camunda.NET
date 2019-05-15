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
namespace org.camunda.bpm.engine.impl.util
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SyncFuture<V> : Future<V>
	{

	  private V result;
	  private Exception e;

	  public SyncFuture(V result)
	  {
		this.result = result;
	  }

	  public SyncFuture(Exception e)
	  {
		this.e = e;
	  }

	  public virtual bool cancel(bool mayInterruptIfRunning)
	  {
		return false;
	  }

	  public virtual bool Cancelled
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual bool Done
	  {
		  get
		  {
			return true;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get() throws InterruptedException, java.util.concurrent.ExecutionException
	  public virtual V get()
	  {
		if (e == null)
		{
		  return result;
		}
		else
		{
		  throw new ExecutionException(e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.ExecutionException, java.util.concurrent.TimeoutException
	  public virtual V get(long timeout, TimeUnit unit)
	  {
		return get();
	  }

	}

}