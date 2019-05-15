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
namespace org.camunda.bpm.qa.performance.engine.framework
{

	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using ActivityPerfTestWatcher = org.camunda.bpm.qa.performance.engine.framework.activitylog.ActivityPerfTestWatcher;

	/// <summary>
	/// @author Daniel Meyer, Ingo Richtsmeier
	/// 
	/// </summary>
	public class PerfTestRunner
	{

	  protected internal ExecutorService executor;
	  protected internal PerfTest test;
	  protected internal PerfTestConfiguration configuration;

	  // global state
	  public static PerfTestPass currentPass;
	  protected internal PerfTestResults results;
	  protected internal object passMonitor;
	  protected internal object doneMonitor;
	  protected internal bool isDone;
	  protected internal Exception exception;
	  protected internal IList<PerfTestWatcher> watchers;

	  public PerfTestRunner(PerfTest test, PerfTestConfiguration configuration)
	  {
		this.test = test;
		this.configuration = configuration;
		init();
	  }

	  protected internal virtual void init()
	  {

		results = new PerfTestResults(configuration);

		doneMonitor = new object();
		isDone = false;

		// init test watchers
		string testWatchers = configuration.TestWatchers;
		if (!string.ReferenceEquals(testWatchers, null))
		{
		  watchers = new List<PerfTestWatcher>();
		  string[] watcherClassNames = testWatchers.Split(",", true);
		  foreach (string watcherClassName in watcherClassNames)
		  {
			if (watcherClassName.Length > 0)
			{
			  object watcher = ReflectUtil.instantiate(watcherClassName);
			  if (watcher is PerfTestWatcher)
			  {
				watchers.Add((PerfTestWatcher) watcher);
			  }
			  else
			  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new PerfTestException("Test watcher " + watcherClassName + " must implement " + typeof(PerfTestWatcher).FullName);
			  }
			}
		  }
		}
		// add activity watcher
		if (configuration.WatchActivities != null)
		{
		  if (watchers == null)
		  {
			watchers = new List<PerfTestWatcher>();
		  }
		  watchers.Add(new ActivityPerfTestWatcher(configuration.WatchActivities));
		}
		configuration.StartTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis());
	  }

	  public virtual Future<PerfTestResults> execute()
	  {

		// run a pass for each number of threads
		new Thread(() =>
		{
	for (int i = 1; i <= configuration.NumberOfThreads; i++)
	{
	  runPassWithThreadCount(i);
	}

	lock (doneMonitor)
	{
	  isDone = true;
	  Monitor.PulseAll(doneMonitor);
	}

		});
		.start();

		return new FutureAnonymousInnerClass(this);
	  }

	  private class FutureAnonymousInnerClass : Future<PerfTestResults>
	  {
		  private readonly PerfTestRunner outerInstance;

		  public FutureAnonymousInnerClass(PerfTestRunner outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public bool Done
		  {
			  get
			  {
				lock (outerInstance.doneMonitor)
				{
				  return outerInstance.isDone;
				}
			  }
		  }

		  public bool Cancelled
		  {
			  get
			  {
				throw new System.NotSupportedException("Cannot cancel a performance test.");
			  }
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PerfTestResults get(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException, java.util.concurrent.ExecutionException, java.util.concurrent.TimeoutException
		  public PerfTestResults get(long timeout, TimeUnit unit)
		  {
			lock (outerInstance.doneMonitor)
			{
			  if (!outerInstance.isDone)
			  {
				Monitor.Wait(outerInstance.doneMonitor, TimeSpan.FromMilliseconds(unit.convert(timeout, TimeUnit.MILLISECONDS)));
				if (outerInstance.exception != null)
				{
				  throw new ExecutionException(outerInstance.exception);
				}
			  }
			}
			return outerInstance.results;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PerfTestResults get() throws InterruptedException, java.util.concurrent.ExecutionException
		  public PerfTestResults get()
		  {
			lock (outerInstance.doneMonitor)
			{
			  if (!outerInstance.isDone)
			  {
				Monitor.Wait(outerInstance.doneMonitor);
				if (outerInstance.exception != null)
				{
				  throw new ExecutionException(outerInstance.exception);
				}
			  }
			}
			return outerInstance.results;
		  }

		  public bool cancel(bool mayInterruptIfRunning)
		  {
			throw new System.NotSupportedException("Cannot cancel a performance test.");
		  }
	  }

	  public virtual ExecutorService Executor
	  {
		  get
		  {
			return executor;
		  }
	  }

	  protected internal virtual void runPassWithThreadCount(int passNumberOfThreads)
	  {

		currentPass = new PerfTestPass(passNumberOfThreads);
		executor = Executors.newFixedThreadPool(passNumberOfThreads);

		// do a GC pause before running the test
		for (int i = 0; i < 5; i++)
		{
		  System.GC.Collect();
		}

		passMonitor = new object();

		PerfTestStep firstStep = test.FirstStep;
		int numberOfRuns = configuration.NumberOfRuns;

		// first create the runs
		currentPass.createRuns(this, firstStep, numberOfRuns);

		// start the pass
		currentPass.startPass();
		notifyWatchersBeforePass();

		// now execute the runs
		foreach (PerfTestRun run in currentPass.Runs.Values)
		{
		  executor.execute(run);
		}

		lock (passMonitor)
		{
		  if (!currentPass.Completed)
		  {
			try
			{
			  Monitor.Wait(passMonitor);

			  executor.shutdownNow();
			  try
			  {
				executor.awaitTermination(60, TimeUnit.SECONDS);
			  }
			  catch (InterruptedException e)
			  {
				exception = e;
			  }

			}
			catch (InterruptedException)
			{
			  throw new PerfTestException("Interrupted wile waiting for pass " + passNumberOfThreads + " to complete.");
			}
		  }
		}
	  }

	  protected internal virtual void notifyWatchersBeforePass()
	  {
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.beforePass(currentPass);
		  }
		}
	  }

	  protected internal virtual void notifyWatchersAfterPass()
	  {
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.afterPass(currentPass);
		  }
		}
	  }

	  /// <summary>
	  /// Invoked when a <seealso cref="PerfTestRun"/> completed a step
	  /// </summary>
	  /// <param name="run"> the current Run </param>
	  /// <param name="currentStep"> the completed step </param>
	  public virtual void completedStep(PerfTestRun run, PerfTestStep currentStep)
	  {
		PerfTestStep nextStep = currentStep.NextStep;

		if (nextStep != null)
		{
		  // if test has more steps, execute the next step
		  run.CurrentStep = nextStep;
		  executor.execute(run);

		}
		else
		{
		  // performance test run is completed
		  completedRun(run);
		}
	  }

	  /// <summary>
	  /// Invoked when a <seealso cref="PerfTestRun"/> is completed. </summary>
	  /// <param name="run"> the completed run </param>
	  public virtual void completedRun(PerfTestRun run)
	  {
		run.endRun();

		long currentlyCompleted = currentPass.completeRun();
		if (currentlyCompleted >= configuration.NumberOfRuns)
		{
		  lock (passMonitor)
		  {

			// record the results:
			currentPass.endPass();
			notifyWatchersAfterPass();

			results.PassResults.Add(currentPass.Result);

			Monitor.PulseAll(passMonitor);
		  }
		}
	  }

	  public virtual void failed(PerfTestRun perfTestRun, Exception t)
	  {
		lock (doneMonitor)
		{
		  this.exception = t;
		  isDone = true;
		  lock (passMonitor)
		  {
			Monitor.PulseAll(passMonitor);
		  }
		  Monitor.PulseAll(doneMonitor);
		}
	  }

	  public virtual IList<PerfTestWatcher> Watchers
	  {
		  get
		  {
			return watchers;
		  }
	  }

	  public virtual PerfTest Test
	  {
		  get
		  {
			return test;
		  }
	  }

	  public virtual void logStepResult(PerfTestRun perfTestRun, object stepResult)
	  {
		currentPass.logStepResult(perfTestRun.CurrentStep, stepResult);
	  }

	  public static void signalRun(string runId)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PerfTestRun run = currentPass.getRun(runId);
		PerfTestRun run = currentPass.getRun(runId);
		if (run.WaitingForSignal)
		{
		  // only complete step if the run is already waiting for a signal
		  run.Runner.Executor.execute(() =>
		  {
	  run.Runner.completedStep(run, run.CurrentStep);
		  });
		}
	  }

	}

}