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
namespace org.camunda.bpm.qa.performance.engine.framework
{

	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using StrongUuidGenerator = org.camunda.bpm.engine.impl.persistence.StrongUuidGenerator;
	using ActivityPerfTestResult = org.camunda.bpm.qa.performance.engine.framework.activitylog.ActivityPerfTestResult;

	/// <summary>
	/// A step in a performance test.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestPass
	{

	  protected internal static readonly IdGenerator idGenerator = new StrongUuidGenerator();

	  // pass state
	  protected internal int numberOfThreads;
	  protected internal AtomicLong completedRuns;
	  protected internal PerfTestResult result;
	  protected internal bool completed;
	  protected internal IDictionary<string, PerfTestRun> runs;
	  protected internal long startTime;
	  protected internal long endTime;


	  public PerfTestPass(int numberOfThreads)
	  {
		this.numberOfThreads = numberOfThreads;
		this.completedRuns = new AtomicLong();
		this.result = new PerfTestResult();
		this.completed = false;
	  }

	  public virtual void createRuns(PerfTestRunner runner, PerfTestStep firstStep, int numberOfRuns)
	  {
		runs = new Dictionary<string, PerfTestRun>();
		for (int i = 0; i < numberOfRuns; i++)
		{
		  string runId = idGenerator.NextId;
		  runs[runId] = new PerfTestRun(runner, runId, firstStep);
		}
		runs = Collections.unmodifiableMap(runs);
	  }

	  public virtual int NumberOfThreads
	  {
		  get
		  {
			return numberOfThreads;
		  }
	  }

	  public virtual AtomicLong CompletedRuns
	  {
		  get
		  {
			return completedRuns;
		  }
	  }

	  public virtual PerfTestResult Result
	  {
		  get
		  {
			return result;
		  }
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return completed;
		  }
	  }

	  public virtual IDictionary<string, PerfTestRun> Runs
	  {
		  get
		  {
			return runs;
		  }
	  }

	  public virtual long StartTime
	  {
		  get
		  {
			return startTime;
		  }
	  }

	  public virtual long EndTime
	  {
		  get
		  {
			return endTime;
		  }
	  }

	  public virtual void startPass()
	  {
		startTime = DateTimeHelper.CurrentUnixTimeMillis();
	  }

	  public virtual long completeRun()
	  {
		return completedRuns.incrementAndGet();
	  }

	  public virtual void endPass()
	  {
		endTime = DateTimeHelper.CurrentUnixTimeMillis();
		result.Duration = endTime - startTime;
		result.NumberOfThreads = numberOfThreads;
		completed = true;
	  }

	  public virtual void logStepResult(PerfTestStep currentStep, object stepResult)
	  {
		result.logStepResult(currentStep, stepResult);
	  }

	  public virtual void logActivityResult(string identifier, IList<ActivityPerfTestResult> results)
	  {
		result.logActivityResult(identifier, results);
	  }

	  public virtual PerfTestRun getRun(string runId)
	  {
		return runs[runId];
	  }
	}

}