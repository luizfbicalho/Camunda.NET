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
namespace org.camunda.bpm.qa.performance.engine.benchmark
{

	using PerfTestConfiguration = org.camunda.bpm.qa.performance.engine.framework.PerfTestConfiguration;
	using PerfTestResult = org.camunda.bpm.qa.performance.engine.framework.PerfTestResult;
	using PerfTestResults = org.camunda.bpm.qa.performance.engine.framework.PerfTestResults;
	using TabularResultAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;

	/// <summary>
	/// @author Ingo Richtsmeier
	/// 
	/// </summary>
	public class BenchmarkLongtermAggregator : TabularResultAggregator
	{

	  public BenchmarkLongtermAggregator(string resultsFolderPath) : base(resultsFolderPath)
	  {
	  }

	  protected internal override TabularResultSet createAggregatedResultsInstance()
	  {
		return new TabularResultSet();
	  }

	  protected internal override void processResults(PerfTestResults results, TabularResultSet tabularResultSet)
	  {

		foreach (PerfTestResult passResult in results.PassResults)
		{
		  tabularResultSet.Results.Add(processRow(passResult, results));
		}

	  }

	  protected internal virtual IList<object> processRow(PerfTestResult passResult, PerfTestResults results)
	  {
		IList<object> row = new List<object>();
		PerfTestConfiguration configuration = results.Configuration;

		// test name
		row.Add(results.TestName);

		// number of runs
		int numberOfRuns = configuration.NumberOfRuns;
		row.Add(numberOfRuns);

		// database
		row.Add(configuration.DatabaseName);

		// history level
		row.Add(configuration.HistoryLevel);

		// start time
		row.Add(configuration.StartTime);

		// platform
		row.Add(configuration.Platform);

		// number of threads
		row.Add(passResult.NumberOfThreads);

		// add duration
		long duration = passResult.Duration;
		row.Add(duration);

		// throughput
		float numberOfRunsFloat = numberOfRuns;
		float throughput = (numberOfRunsFloat / duration) * 1000;
		row.Add(throughput);

		return row;
	  }

	}

}