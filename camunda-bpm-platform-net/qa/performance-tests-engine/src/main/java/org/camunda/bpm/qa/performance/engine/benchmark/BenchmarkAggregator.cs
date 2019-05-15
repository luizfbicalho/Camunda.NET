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

	using PerfTestResult = org.camunda.bpm.qa.performance.engine.framework.PerfTestResult;
	using PerfTestResults = org.camunda.bpm.qa.performance.engine.framework.PerfTestResults;
	using TabularResultAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;

	/// <summary>
	/// The default benchmark aggregator records the duration
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BenchmarkAggregator : TabularResultAggregator
	{

	  public const string TEST_NAME = "Test Name";

	  public BenchmarkAggregator(string resultsFolderPath) : base(resultsFolderPath)
	  {
	  }

	  protected internal override TabularResultSet createAggregatedResultsInstance()
	  {
		return new TabularResultSet();
	  }

	  protected internal override void processResults(PerfTestResults results, TabularResultSet tabularResultSet)
	  {

		IList<object> row = new List<object>();
		row.Add(results.TestName);

		foreach (PerfTestResult passResult in results.PassResults)
		{
		  processRow(row, passResult, results);
		}

		tabularResultSet.Results.Add(row);
	  }

	  protected internal virtual void processRow(IList<object> row, PerfTestResult passResult, PerfTestResults results)
	  {
		// add duration
		row.Add(passResult.Duration);

		// add throughput per second
		long duration = passResult.Duration;
		float numberOfRuns = results.Configuration.NumberOfRuns;
		float throughput = (numberOfRuns / duration) * 1000;
		row.Add(throughput);

		// add speedup
		float durationForSequential = 0;
		foreach (PerfTestResult perfTestResult in results.PassResults)
		{
		  if (perfTestResult.NumberOfThreads == 1)
		  {
			durationForSequential = perfTestResult.Duration;
		  }
		}
		double speedUp = durationForSequential / passResult.Duration;
		decimal bigDecimalSpeedUp = new decimal(speedUp);
		bigDecimalSpeedUp.setScale(1, decimal.ROUND_HALF_UP);
		row.Add(bigDecimalSpeedUp.doubleValue());
	  }

	  protected internal override void postProcessResultSet(TabularResultSet tabularResultSet)
	  {
		if (tabularResultSet.Results.Count > 0)
		{
		  int columnSize = tabularResultSet.Results[0].Count;

		  List<string> columnNames = new List<string>();
		  columnNames.Add(TEST_NAME);
		  for (int i = 1; i < columnSize; i++)
		  {
			if ((i - 1) % 3 == 0)
			{
			  int numberOfThreads = (i / 3) + 1;
			  columnNames.Add("T = " + numberOfThreads);
			}
			else
			{
			  columnNames.Add(" ");
			}
		  }

		  tabularResultSet.ResultColumnNames = columnNames;
		}

	  }

	}

}