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
	using MetricsRegistry = org.camunda.bpm.engine.impl.metrics.MetricsRegistry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.management.Metrics.ACTIVTY_INSTANCE_START;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// Represents a test suite for the metrics interval query to check if the
	/// timestamps are read in a correct time zone.
	/// 
	/// This was a problem before the column MILLISECONDS_ was added.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MetricsIntervalTimezoneTest : AbstractMetricsIntervalTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTimestampIsInCorrectTimezone()
	  public virtual void testTimestampIsInCorrectTimezone()
	  {
		//given generated metric data started at DEFAULT_INTERVAL ends at 3 * DEFAULT_INTERVAL

		//when metric query is executed (hint last interval is returned as first)
		IList<MetricIntervalValue> metrics = managementService.createMetricsQuery().limit(1).interval();

		//then metric interval time should be less than FIRST_INTERVAL + 3 * DEFAULT_INTERVAL
		long metricIntervalTime = metrics[0].Timestamp.Ticks;
		Assert.assertTrue(metricIntervalTime < firstInterval.plusMinutes(3 * DEFAULT_INTERVAL).Millis);
		//and larger than first interval time, if not than we have a timezone problem
		Assert.assertTrue(metricIntervalTime > firstInterval.Millis);

		//when current time is used and metric is reported
		DateTime currentTime = DateTime.Now;
		MetricsRegistry metricsRegistry = processEngineConfiguration.MetricsRegistry;
		ClockUtil.CurrentTime = currentTime;
		metricsRegistry.markOccurrence(ACTIVTY_INSTANCE_START, 1);
		processEngineConfiguration.DbMetricsReporter.reportNow();

		//then current time should be larger than metric interval time
		IList<MetricIntervalValue> m2 = managementService.createMetricsQuery().limit(1).interval();
		Assert.assertTrue(m2[0].Timestamp.Ticks < currentTime.Ticks);
	  }
	}

}