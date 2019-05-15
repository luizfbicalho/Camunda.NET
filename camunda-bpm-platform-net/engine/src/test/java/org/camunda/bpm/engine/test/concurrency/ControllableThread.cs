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
namespace org.camunda.bpm.engine.test.concurrency
{
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ControllableThread : Thread
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  public ControllableThread() : base()
	  {
		Name = generateName();
	  }

	  public ControllableThread(ThreadStart runnable) : base(runnable)
	  {
		Name = generateName();
	  }

	  protected internal virtual string generateName()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string className = this.GetType().FullName;
		int dollarIndex = className.LastIndexOf('$');
		return className.Substring(dollarIndex + 1);
	  }

	  public virtual void startAndWaitUntilControlIsReturned()
	  {
		  lock (this)
		  {
			LOG.debug("test thread will start " + Name + " and wait till it returns control");
			start();
			try
			{
			  Monitor.Wait(this);
			}
			catch (InterruptedException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
		  }
	  }

	  public virtual void returnControlToTestThreadAndWait()
	  {
		  lock (this)
		  {
			LOG.debug(Name + " will notify test thread and till test thread proceeds this thread");
			Monitor.Pulse(this);
			try
			{
			  Monitor.Wait(this);
			}
			catch (InterruptedException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
		  }
	  }

	  public virtual void returnControlToControllableThreadAndWait()
	  {
		  lock (this)
		  {
			// just for understanding the test case
			returnControlToTestThreadAndWait();
		  }
	  }

	  public virtual void proceedAndWaitTillDone()
	  {
		  lock (this)
		  {
			LOG.debug("test thread will notify " + Name + " and wait until it completes");
			Monitor.Pulse(this);
			try
			{
			  join();
			}
			catch (InterruptedException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
		  }
	  }
	}

}