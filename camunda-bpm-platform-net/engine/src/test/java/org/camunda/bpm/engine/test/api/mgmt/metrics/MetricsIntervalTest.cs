using System;
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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{
	using MetricsQueryImpl = org.camunda.bpm.engine.impl.metrics.MetricsQueryImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Test = org.junit.Test;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.management.Metrics.ACTIVTY_INSTANCE_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using DateTime = org.joda.time.DateTime;
	using Assert = org.junit.Assert;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MetricsIntervalTest : AbstractMetricsIntervalTest
	{

	  // LIMIT //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryLimit()
	  public virtual void testMeterQueryLimit()
	  {
		//since generating test data of 200 metrics will take a long time we check if the default values are set of the query
		//given metric query
		MetricsQueryImpl query = (MetricsQueryImpl) managementService.createMetricsQuery();

		//when no changes are made
		//then max results are 200, lastRow 201, offset 0, firstRow 1
		assertEquals(1, query.FirstRow);
		assertEquals(0, query.FirstResult);
		assertEquals(200, query.MaxResults);
		assertEquals(201, query.LastRow);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDecreaseLimit()
	  public virtual void testMeterQueryDecreaseLimit()
	  {
		//given metric data

		//when query metric interval data with limit of 10 values
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().limit(10).interval();

		//then 10 values are returned
		assertEquals(10, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryIncreaseLimit()
	  public virtual void testMeterQueryIncreaseLimit()
	  {
		//given metric data

		  //when query metric interval data with max results set to 1000
		exception.expect(typeof(ProcessEngineException));
		exception.expectMessage("Metrics interval query row limit can't be set larger than 200.");
		managementService.createMetricsQuery().limit(1000).interval();
	  }

	  // OFFSET //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryOffset()
	  public virtual void testMeterQueryOffset()
	  {
		//given metric data

		//when query metric interval data with offset of metrics count
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().offset(metricsCount).interval();

		//then 2 * metricsCount values are returned and highest interval is second last interval, since first 9 was skipped
		assertEquals(2 * metricsCount, metrics.Count);
		assertEquals(firstInterval.plusMinutes(DEFAULT_INTERVAL).Millis, metrics[0].Timestamp.Ticks);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryMaxOffset()
	  public virtual void testMeterQueryMaxOffset()
	  {
		//given metric data

		//when query metric interval data with max offset
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().offset(int.MaxValue).interval();

		//then 0 values are returned
		assertEquals(0, metrics.Count);
	  }

	  // INTERVAL //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultInterval()
	  public virtual void testMeterQueryDefaultInterval()
	  {
		//given metric data

		//when query metric interval data with default values
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().interval();

		//then default interval is 900 s (15 minutes)
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomInterval()
	  public virtual void testMeterQueryCustomInterval()
	  {
		//given metric data

		//when query metric interval data with custom time interval
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().interval(300);

		//then custom interval is 300 s (5 minutes)
		int interval = 5 * 60 * 1000;
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + interval);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

	  // WHERE REPORTER //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereReporter()
	  public virtual void testMeterQueryDefaultIntervalWhereReporter()
	  {
		//given metric data

		//when query metric interval data with reporter in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().reporter(REPORTER_ID).interval();

		//then result contains only metrics from given reporter, since it is the default it contains all
		assertEquals(3 * metricsCount, metrics.Count);
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		string reporter = metrics[0].Reporter;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  assertEquals(reporter, metric.Reporter);
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereReporterNotExist()
	  public virtual void testMeterQueryDefaultIntervalWhereReporterNotExist()
	  {
		//given metric data

		//when query metric interval data with not existing reporter in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().reporter("notExist").interval();

		//then result contains no metrics from given reporter
		assertEquals(0, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereReporter()
	  public virtual void testMeterQueryCustomIntervalWhereReporter()
	  {
		//given metric data and custom interval
		int interval = 5 * 60;

		//when query metric interval data with custom interval and reporter in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().reporter(REPORTER_ID).interval(interval);

		//then result contains only metrics from given reporter, since it is the default it contains all
		assertEquals(9 * metricsCount, metrics.Count);
		interval = interval * 1000;
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		string reporter = metrics[0].Reporter;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  assertEquals(reporter, metric.Reporter);
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + interval);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereReporterNotExist()
	  public virtual void testMeterQueryCustomIntervalWhereReporterNotExist()
	  {
		//given metric data

		//when query metric interval data with custom interval and non existing reporter in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().reporter("notExist").interval(300);

		//then result contains no metrics from given reporter
		assertEquals(0, metrics.Count);
	  }

	  // WHERE NAME //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereName()
	  public virtual void testMeterQueryDefaultIntervalWhereName()
	  {
		//given metric data

		//when query metric interval data with name in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).interval();

		//then result contains only metrics with given name
		assertEquals(3, metrics.Count);
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		string name = metrics[0].Name;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  assertEquals(name, metric.Name);
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereNameNotExist()
	  public virtual void testMeterQueryDefaultIntervalWhereNameNotExist()
	  {
		//given metric data

		//when query metric interval data with non existing name in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name("notExist").interval();

		//then result contains no metrics with given name
		assertEquals(0, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereName()
	  public virtual void testMeterQueryCustomIntervalWhereName()
	  {
		//given metric data and custom interval
		int interval = 5 * 60;

		//when query metric interval data with custom interval and name in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).interval(interval);

		//then result contains only metrics with given name
		assertEquals(9, metrics.Count);
		interval = interval * 1000;
		long lastTimestamp = metrics[0].Timestamp.Ticks;
		string name = metrics[0].Name;
		metrics.RemoveAt(0);
		foreach (MetricIntervalValue metric in metrics)
		{
		  assertEquals(name, metric.Name);
		  long nextTimestamp = metric.Timestamp.Ticks;
		  if (lastTimestamp != nextTimestamp)
		  {
			assertEquals(lastTimestamp, nextTimestamp + interval);
			lastTimestamp = nextTimestamp;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereNameNotExist()
	  public virtual void testMeterQueryCustomIntervalWhereNameNotExist()
	  {
		//given metric data

		//when query metric interval data with custom interval and non existing name in where clause
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name("notExist").interval(300);

		//then result contains no metrics from given name
		assertEquals(0, metrics.Count);
	  }

	  // START DATE //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereStartDate()
	  public virtual void testMeterQueryDefaultIntervalWhereStartDate()
	  {
		//given metric data created for 14.9  min intervals

		//when query metric interval data with second last interval as start date in where clause
		//second last interval = start date = Jan 1, 1970 1:15:00 AM
		DateTime startDate = firstInterval.plusMinutes(DEFAULT_INTERVAL).toDate();
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().startDate(startDate).interval();

		//then result contains 18 entries since 9 different metrics are created
		//intervals Jan 1, 1970 1:15:00 AM and Jan 1, 1970 1:30:00 AM
		assertEquals(2 * metricsCount, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereStartDate()
	  public virtual void testMeterQueryCustomIntervalWhereStartDate()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with custom interval and second last interval as start date in where clause
		DateTime startDate = firstInterval.plusMinutes(DEFAULT_INTERVAL).toDate();
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().startDate(startDate).interval(300);

		//then result contains 4 intervals * the metrics count
		//15 20 25 30 35 40
		assertEquals(6 * metricsCount, metrics.Count);
	  }

	  // END DATE //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereEndDate()
	  public virtual void testMeterQueryDefaultIntervalWhereEndDate()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with second interval as end date in where clause
		//second interval = first interval - default interval
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL);
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().endDate(endDate.toDate()).interval();

		//then result contains one interval with entry for each metric
		//and end time is exclusive
		assertEquals(metricsCount, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereEndDate()
	  public virtual void testMeterQueryCustomIntervalWhereEndDate()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with custom interval and second interval as end date in where clause
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL).toDate();
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().endDate(endDate).interval(300);

		//then result contains 3 * metrics count 3 interval before end time
		//endTime is exclusive which means the given date is not included in the result
		assertEquals(3 * metricsCount, metrics.Count);
	  }

	  // START AND END DATE //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalWhereStartAndEndDate()
	  public virtual void testMeterQueryDefaultIntervalWhereStartAndEndDate()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with start and end date in where clause
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL);
		DateTime startDate = firstInterval;
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().startDate(startDate.toDate()).endDate(endDate.toDate()).interval();

		//then result contains 9 entries since 9 different metrics are created
		//and start date is inclusive and end date exclusive
		assertEquals(metricsCount, metrics.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalWhereStartAndEndDate()
	  public virtual void testMeterQueryCustomIntervalWhereStartAndEndDate()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with custom interval, start and end date in where clause
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL);
		DateTime startDate = firstInterval;
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().startDate(startDate.toDate()).endDate(endDate.toDate()).interval(300);

		//then result contains 27 entries since 9 different metrics are created
		//endTime is exclusive which means the given date is not included in the result
		assertEquals(3 * metricsCount, metrics.Count);
	  }

	  // VALUE //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryDefaultIntervalCalculatedValue()
	  public virtual void testMeterQueryDefaultIntervalCalculatedValue()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with custom interval, start and end date in where clause
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL);
		DateTime startDate = firstInterval;
		MetricsQuery metricQuery = managementService.createMetricsQuery().startDate(startDate.toDate()).endDate(endDate.toDate()).name(ACTIVTY_INSTANCE_START);
		IList<MetricIntervalValue> metrics = metricQuery.interval();
		long sum = metricQuery.sum();

		//then result contains 1 entries
		//sum should be equal to the sum which is calculated by the metric query
		assertEquals(1, metrics.Count);
		assertEquals(sum, metrics[0].Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryCustomIntervalCalculatedValue()
	  public virtual void testMeterQueryCustomIntervalCalculatedValue()
	  {
		//given metric data created for 15 min intervals

		//when query metric interval data with custom interval, start and end date in where clause
		DateTime endDate = firstInterval.plusMinutes(DEFAULT_INTERVAL);
		DateTime startDate = firstInterval;
		MetricsQuery metricQuery = managementService.createMetricsQuery().startDate(startDate.toDate()).endDate(endDate.toDate()).name(ACTIVTY_INSTANCE_START);
		IList<MetricIntervalValue> metrics = metricQuery.interval(300);
		long sum = metricQuery.sum();

		//then result contains 3 entries
		assertEquals(3, metrics.Count);
		long summedValue = 0;
		summedValue += metrics[0].Value;
		summedValue += metrics[1].Value;
		summedValue += metrics[2].Value;

		//summed value should be equal to the summed query value
		assertEquals(sum, summedValue);
	  }

	  // NOT LOGGED METRICS //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMeterQueryNotLoggedInterval()
	  public virtual void testMeterQueryNotLoggedInterval()
	  {
		//given metric data
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).limit(1).interval();
		long value = metrics[0].Value;

		//when start process and metrics are not logged
		processEngineConfiguration.MetricsRegistry.markOccurrence(ACTIVTY_INSTANCE_START, 3);

		//then metrics values are either way aggregated to the last interval
		//on query with name
		 metrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).limit(1).interval();
		long newValue = metrics[0].Value;
		Assert.assertTrue(value + 3 == newValue);

		//on query without name also
		 metrics = managementService.createMetricsQuery().interval();
		 foreach (MetricIntervalValue intervalValue in metrics)
		 {
		   if (intervalValue.Name.Equals(ACTIVTY_INSTANCE_START, StringComparison.OrdinalIgnoreCase))
		   {
			newValue = intervalValue.Value;
			Assert.assertTrue(value + 3 == newValue);
			break;
		   }
		 }

		//clean up
		clearLocalMetrics();
	  }

	  // NEW DATA AFTER SOME TIME ////////////////////////////////////////////////
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntervallQueryWithGeneratedDataAfterSomeTime()
	  public virtual void testIntervallQueryWithGeneratedDataAfterSomeTime()
	  {
		//given metric data and result of interval query
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().interval();

		//when time is running and metrics is reported
		DateTime lastInterval = metrics[0].Timestamp;
		long nextTime = lastInterval.Ticks + DEFAULT_INTERVAL_MILLIS;
		ClockUtil.CurrentTime = new DateTime(nextTime);

		reportMetrics();

		//then query returns more results
		IList<MetricIntervalValue> newMetrics = managementService.createMetricsQuery().interval();
		assertNotEquals(metrics.Count, newMetrics.Count);
		assertEquals(metrics.Count + metricsCount, newMetrics.Count);
		assertEquals(newMetrics[0].Timestamp.Ticks, metrics[0].Timestamp.Ticks + DEFAULT_INTERVAL_MILLIS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntervallQueryWithGeneratedDataAfterSomeTimeForSpecificMetric()
	  public virtual void testIntervallQueryWithGeneratedDataAfterSomeTimeForSpecificMetric()
	  {
		//given metric data and result of interval query
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).startDate(new DateTime()).endDate(new DateTime(DEFAULT_INTERVAL_MILLIS * 200)).interval();

		//when time is running and metrics is reported
		DateTime lastInterval = metrics[0].Timestamp;
		long nextTime = lastInterval.Ticks + DEFAULT_INTERVAL_MILLIS;
		ClockUtil.CurrentTime = new DateTime(nextTime);

		reportMetrics();

		//then query returns more results
		IList<MetricIntervalValue> newMetrics = managementService.createMetricsQuery().name(ACTIVTY_INSTANCE_START).startDate(new DateTime()).endDate(new DateTime(DEFAULT_INTERVAL_MILLIS * 200)).interval();
		assertNotEquals(metrics.Count, newMetrics.Count);
		assertEquals(newMetrics[0].Timestamp.Ticks, metrics[0].Timestamp.Ticks + DEFAULT_INTERVAL_MILLIS);
		assertEquals(metrics[0].Value, newMetrics[1].Value);

		//clean up
		clearMetrics();
	  }

	  // AGGREGATE BY REPORTER ////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMetricQueryAggregatedByReporterSingleReporter()
	  public virtual void testMetricQueryAggregatedByReporterSingleReporter()
	  {
		// given metric data and result of interval query
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().interval();

		// assume
		assertTrue(metrics.Count > 0);

		// when
		IList<MetricIntervalValue> aggregatedMetrics = managementService.createMetricsQuery().aggregateByReporter().interval();

		// then
		assertEquals(metrics.Count, aggregatedMetrics.Count);
		foreach (MetricIntervalValue metricIntervalValue in aggregatedMetrics)
		{
		  assertNull(metricIntervalValue.Reporter);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMetricQueryAggregatedByReporterThreeReporters()
	  public virtual void testMetricQueryAggregatedByReporterThreeReporters()
	  {
		// given metric data for default reported
		// generate data for reporter1
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
		generateMeterData(3, DEFAULT_INTERVAL_MILLIS);

		// generate data for reporter2
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
		generateMeterData(3, DEFAULT_INTERVAL_MILLIS);

		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().interval();

		// when
		IList<MetricIntervalValue> aggregatedMetrics = managementService.createMetricsQuery().aggregateByReporter().interval();

		// then
		// multiply by 3 because there are three reporters: 'REPORTER_ID' (check the #initMetrics()), reporter1 and reporter2
		assertEquals(metrics.Count, aggregatedMetrics.Count * 3);
		foreach (MetricIntervalValue metricIntervalValue in aggregatedMetrics)
		{
		  assertNull(metricIntervalValue.Reporter);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMetricQueryAggregatedByReporterLimitAndTwoReporters()
	  public virtual void testMetricQueryAggregatedByReporterLimitAndTwoReporters()
	  {
		// clean up default recorded metrics
		clearLocalMetrics();
		// given
		// generate data for reporter1
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
		generateMeterData(10, DEFAULT_INTERVAL_MILLIS);

		// generate data for reporter2
		processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
		generateMeterData(10, DEFAULT_INTERVAL_MILLIS);

		int limit = 10;
		// when
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).limit(limit).interval();
		IList<MetricIntervalValue> aggregatedMetrics = managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).limit(limit).aggregateByReporter().interval();

		// then aggregatedMetrics contains wider time interval
		assertTrue(metrics[limit - 1].Timestamp.Ticks > aggregatedMetrics[limit - 1].Timestamp.Ticks);
		assertEquals(metrics.Count, aggregatedMetrics.Count);
	  }

	}

}