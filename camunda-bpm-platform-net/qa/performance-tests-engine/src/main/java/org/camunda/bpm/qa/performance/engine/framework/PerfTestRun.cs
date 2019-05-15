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

	using ActivityPerfTestResult = org.camunda.bpm.qa.performance.engine.framework.activitylog.ActivityPerfTestResult;
	using PerfTestConstants = org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants;

	/// <summary>
	/// An individual run of a performance test. Holds all state related to a test run.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestRun : PerfTestRunContext, ThreadStart
	{

	  protected internal bool isStarted;

	  protected internal long runStartTime;
	  protected internal long runEndTime;

	  protected internal long stepStartTime;
	  protected internal long stepEndTime;

	  protected internal volatile PerfTestStep currentStep;
	  protected internal AtomicInteger state = new AtomicInteger();

	  protected internal PerfTestRunner runner;

	  protected internal IDictionary<string, object> runContext = new Dictionary<string, object>();

	  public PerfTestRun(PerfTestRunner runner, string runId, PerfTestStep firstStep)
	  {
		this.runner = runner;
		this.currentStep = firstStep;
		setVariable(PerfTestConstants.RUN_ID, runId);
	  }

	  public virtual void startRun()
	  {
		runStartTime = DateTimeHelper.CurrentUnixTimeMillis();
		isStarted = true;
		notifyWatchersStartRun();
	  }

	  public virtual void endRun()
	  {
		runEndTime = DateTimeHelper.CurrentUnixTimeMillis();
		notifyWatchersEndRun();
	  }

	  public virtual void run()
	  {
		try
		{
		  if (!isStarted)
		  {
			startRun();
		  }

		  PerfTestRunContext_Fields.currentContext.set(this);

		  if (!currentStep.WaitStep)
		  {
			continueRun();
		  }
		  else
		  {
			pauseRun();
		  }

		}
		catch (Exception t)
		{
		  runner.failed(this, t);

		}
		finally
		{
		  PerfTestRunContext_Fields.currentContext.remove();

		}
	  }

	  protected internal virtual void continueRun()
	  {
		notifyWatchersBeforeStep();
		currentStep.StepBehavior.execute(this);
		notifyWatchersAfterStep();
		runner.completedStep(this, currentStep);
	  }

	  protected internal virtual void pauseRun()
	  {
		if (AlreadySignaled)
		{
		  // if a signal was already received immediately continue
		  runner.completedStep(this, currentStep);
		}
	  }

	  public virtual T getVariable<T>(string name)
	  {
		object var = runContext[name];
		if (var == null)
		{
		  return default(T);
		}
		else
		{
		  return (T) var;
		}
	  }

	  public virtual void setVariable(string name, object value)
	  {
		runContext[name] = value;
	  }

	  public virtual PerfTestStep CurrentStep
	  {
		  set
		  {
			this.currentStep = value;
		  }
		  get
		  {
			return currentStep;
		  }
	  }

	  public virtual long RunStartTime
	  {
		  get
		  {
			return runStartTime;
		  }
	  }

	  public virtual long RunEndTime
	  {
		  get
		  {
			return runEndTime;
		  }
	  }


	  public virtual PerfTestRunner Runner
	  {
		  get
		  {
			return runner;
		  }
	  }

	  public virtual long StepEndTime
	  {
		  get
		  {
			return stepEndTime;
		  }
	  }

	  public virtual long StepStartTime
	  {
		  get
		  {
			return stepStartTime;
		  }
	  }

	  /// <summary>
	  /// Sets the run into waiting state and returns if the run
	  /// was already signaled.
	  /// 
	  /// Note: This method will change the state of the run
	  /// to waiting.
	  /// </summary>
	  /// <returns> true if the run was already signaled, false otherwise </returns>
	  public virtual bool AlreadySignaled
	  {
		  get
		  {
			int newState = this.state.incrementAndGet();
			return newState == 0;
		  }
	  }

	  /// <summary>
	  /// Signals the run and returns if the run was already
	  /// waiting for a signal.
	  /// 
	  /// Note: This method will change the state of the run
	  /// to signaled.
	  /// </summary>
	  /// <returns> true if the run was waiting, false otherwise </returns>
	  public virtual bool WaitingForSignal
	  {
		  get
		  {
			int newState = this.state.decrementAndGet();
			return newState == 0;
		  }
	  }

	  protected internal virtual void notifyWatchersStartRun()
	  {
		IList<PerfTestWatcher> watchers = runner.Watchers;
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.beforeRun(runner.Test, this);
		  }
		}
	  }

	  protected internal virtual void notifyWatchersEndRun()
	  {
		IList<PerfTestWatcher> watchers = runner.Watchers;
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.afterRun(runner.Test, this);
		  }
		}
	  }

	  protected internal virtual void notifyWatchersBeforeStep()
	  {
		IList<PerfTestWatcher> watchers = runner.Watchers;
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.beforeStep(currentStep, this);
		  }
		}
	  }

	  protected internal virtual void notifyWatchersAfterStep()
	  {
		IList<PerfTestWatcher> watchers = runner.Watchers;
		if (watchers != null)
		{
		  foreach (PerfTestWatcher perfTestWatcher in watchers)
		  {
			perfTestWatcher.afterStep(currentStep, this);
		  }
		}
	  }

	  public virtual void logStepResult(object stepResult)
	  {
		runner.logStepResult(this, stepResult);
	  }

	}

}