using System;
using System.Collections.Generic;
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

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class ConcurrencyTestCase : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<ControllableCommand<?>> controllableCommands;
	  protected internal IList<ControllableCommand<object>> controllableCommands;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: controllableCommands = new java.util.ArrayList<ControllableCommand<?>>();
		controllableCommands = new List<ControllableCommand<object>>();
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {

		// wait for all spawned threads to end
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (ControllableCommand<?> controllableCommand : controllableCommands)
		foreach (ControllableCommand<object> controllableCommand in controllableCommands)
		{
		  ThreadControl threadControl = controllableCommand.monitor;
		  threadControl.executingThread.Interrupt();
		  threadControl.executingThread.Join();
		}

		// clear the test thread's interruption state
		Thread.interrupted();

		base.tearDown();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected ThreadControl executeControllableCommand(final ControllableCommand<?> command)
	  protected internal virtual ThreadControl executeControllableCommand<T1>(ControllableCommand<T1> command)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Thread controlThread = Thread.currentThread();
		Thread controlThread = Thread.CurrentThread;

		Thread thread = new Thread(() =>
		{
	try
	{
	  processEngineConfiguration.CommandExecutorTxRequiresNew.execute(command);
	}
	catch (Exception e)
	{
	  command.monitor.Exception = e;
	  controlThread.Interrupt();
	  throw e;
	}
		});

		controllableCommands.Add(command);
		command.monitor.executingThread = thread;

		thread.Start();

		return command.monitor;
	  }


	  public abstract class ControllableCommand<T> : Command<T>
	  {
		  public abstract T execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext);

		protected internal readonly ThreadControl monitor;

		public ControllableCommand()
		{
		  this.monitor = new ThreadControl();
		}

		public ControllableCommand(ThreadControl threadControl)
		{
		  this.monitor = threadControl;
		}

	  }

	  public class ThreadControl
	  {

		protected internal volatile bool syncAvailable = false;

		protected internal Thread executingThread;

		protected internal volatile bool reportFailure;
		protected internal volatile Exception exception;

		protected internal bool ignoreSync = false;

		public ThreadControl()
		{
		}

		public ThreadControl(Thread executingThread)
		{
		  this.executingThread = executingThread;
		}

		public virtual void waitForSync()
		{
		  waitForSync(long.MaxValue);
		}

		public virtual void waitForSync(long timeout)
		{
		  lock (this)
		  {
			if (exception != null)
			{
			  if (reportFailure)
			  {
				return;
			  }
			  else
			  {
				fail();
			  }
			}
			try
			{
			  if (!syncAvailable)
			  {
				try
				{
				  Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
				}
				catch (InterruptedException)
				{
				  if (!reportFailure || exception == null)
				  {
					fail("unexpected interruption");
				  }
				}
			  }
			}
			finally
			{
			  syncAvailable = false;
			}
		  }
		}

		public virtual void waitUntilDone()
		{
		  waitUntilDone(false);
		}

		public virtual void waitUntilDone(bool ignoreUpcomingSyncs)
		{
		  ignoreSync = ignoreUpcomingSyncs;
		  makeContinue();
		  join();
		}

		public virtual void join()
		{
		  try
		  {
			executingThread.Join();
		  }
		  catch (InterruptedException)
		  {
			if (!reportFailure || exception == null)
			{
			  fail("Unexpected interruption");
			}
		  }
		}

		public virtual void sync()
		{
		  lock (this)
		  {
			if (ignoreSync)
			{
			  return;
			}

			syncAvailable = true;
			try
			{
			  Monitor.PulseAll(this);
			  Monitor.Wait(this);
			}
			catch (InterruptedException)
			{
			  if (!reportFailure || exception == null)
			  {
				fail("Unexpected interruption");
			  }
			}
		  }
		}

		public virtual void makeContinue()
		{
		  lock (this)
		  {
			if (exception != null)
			{
			  fail();
			}
			Monitor.PulseAll(this);
		  }
		}

		public virtual void makeContinueAndWaitForSync()
		{
		  makeContinue();
		  waitForSync();
		}

		public virtual void reportInterrupts()
		{
		  this.reportFailure = true;
		}

		public virtual void ignoreFutureSyncs()
		{
		  this.ignoreSync = true;
		}

		public virtual Exception Exception
		{
			set
			{
				lock (this)
				{
				  this.exception = value;
				}
			}
			get
			{
			  return exception;
			}
		}

	  }

	}

}