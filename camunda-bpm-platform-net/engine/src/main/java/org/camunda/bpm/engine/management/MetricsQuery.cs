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
namespace org.camunda.bpm.engine.management
{

	/// <summary>
	/// @author Daniel Meyer
	/// @since 7.3
	/// </summary>
	public interface MetricsQuery
	{

	  /// <seealso cref= constants in <seealso cref="Metrics"/> for a list of names which can be used here.
	  /// </seealso>
	  /// <param name="name"> The name of the metrics to query for </param>
	  MetricsQuery name(string name);

	  /// <summary>
	  /// Restrict to data collected by the reported with the given identifier
	  /// </summary>
	  MetricsQuery reporter(string reporter);

	  /// <summary>
	  /// Restrict to data collected after the given date (inclusive)
	  /// </summary>
	  MetricsQuery startDate(DateTime startTime);

	  /// <summary>
	  /// Restrict to data collected before the given date (exclusive)
	  /// </summary>
	  MetricsQuery endDate(DateTime endTime);


	  /// <summary>
	  /// Sets the offset of the returned results.
	  /// </summary>
	  /// <param name="offset"> indicates after which row the result begins </param>
	  /// <returns> the adjusted MetricsQuery </returns>
	  MetricsQuery offset(int offset);

	  /// <summary>
	  /// Sets the limit row count of the result.
	  /// Can't be set larger than 200, since it is the maximum row count which should be returned.
	  /// </summary>
	  /// <param name="maxResults"> the new row limit of the result </param>
	  /// <returns> the adjusted MetricsQuery </returns>
	  MetricsQuery limit(int maxResults);

	  /// <summary>
	  /// Aggregate metrics by reporters
	  /// </summary>
	  /// <returns> the adjusted MetricsQuery </returns>
	  MetricsQuery aggregateByReporter();

	  /// <summary>
	  /// Returns the metrics summed up and aggregated on a time interval.
	  /// Default interval is 900 (15 minutes). The list size has a maximum of 200
	  /// the maximum can be decreased with the MetricsQuery#limit method. Paging
	  /// is enabled with the help of the offset.
	  /// </summary>
	  /// <returns> the aggregated metrics </returns>
	  IList<MetricIntervalValue> interval();



	  /// <summary>
	  /// Returns the metrics summed up and aggregated on a time interval.
	  /// The size of the interval is given via parameter.
	  /// The time unit is seconds! The list size has a maximum of 200
	  /// the maximum can be decreased with the MetricsQuery#limit method. Paging
	  /// is enabled with the help of the offset.
	  /// </summary>
	  /// <param name="interval"> The time interval on which the metrics should be aggregated.
	  ///                  The time unit is seconds. </param>
	  /// <returns> the aggregated metrics </returns>
	  IList<MetricIntervalValue> interval(long interval);

	  /// <returns> the aggregated sum </returns>
	  long sum();

	}
}