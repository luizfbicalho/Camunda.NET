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

	using ActivityPerfTestResult = org.camunda.bpm.qa.performance.engine.framework.activitylog.ActivityPerfTestResult;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestResult
	{

	  protected internal long duration;

	  protected internal int numberOfThreads;

	  protected internal IList<PerfTestStepResult> stepResults = Collections.synchronizedList(new List<PerfTestStepResult>());

	  protected internal readonly IDictionary<string, IList<ActivityPerfTestResult>> activityResults = Collections.synchronizedMap(new Dictionary<string, IList<ActivityPerfTestResult>>());

	  public virtual long Duration
	  {
		  get
		  {
			return duration;
		  }
		  set
		  {
			this.duration = value;
		  }
	  }


	  public virtual int NumberOfThreads
	  {
		  get
		  {
			return numberOfThreads;
		  }
		  set
		  {
			this.numberOfThreads = value;
		  }
	  }


	  public virtual IList<PerfTestStepResult> StepResults
	  {
		  get
		  {
			return stepResults;
		  }
		  set
		  {
			this.stepResults = value;
		  }
	  }


	  public virtual IDictionary<string, IList<ActivityPerfTestResult>> ActivityResults
	  {
		  get
		  {
			return activityResults;
		  }
	  }

	  /// <summary>
	  /// log a step result. NOTE: this is expensive as it requires synchronization on the stepResultList.
	  /// </summary>
	  /// <param name="currentStep"> </param>
	  /// <param name="stepResult"> </param>
	  public virtual void logStepResult(PerfTestStep currentStep, object stepResult)
	  {
		stepResults.Add(new PerfTestStepResult(currentStep.StepName, stepResult));
	  }

	  public virtual void logActivityResult(string identifier, IList<ActivityPerfTestResult> results)
	  {
		activityResults[identifier] = results;
	  }

	}

}