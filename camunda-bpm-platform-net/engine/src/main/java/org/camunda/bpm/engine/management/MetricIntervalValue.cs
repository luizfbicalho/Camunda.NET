using System;

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
	/// Represents a metric which contains a name, reporter like the node,
	/// timestamp and corresponding value.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// @since 7.6.0
	/// </summary>
	public interface MetricIntervalValue
	{

	  /// <summary>
	  /// Returns the name of the metric.
	  /// </summary>
	  /// <seealso cref= constants in <seealso cref="Metrics"/> for a list of names which can be returned here
	  /// </seealso>
	  /// <returns> the name of the metric </returns>
	  string Name {get;}

	  /// <summary>
	  /// Returns
	  ///        the reporter name of the metric, identifies the node which generates this metric.
	  ///        'null' when the metrics are aggregated by reporter.
	  /// </summary>
	  /// <returns> the reporter name </returns>
	  string Reporter {get;}

	  /// <summary>
	  /// Returns the timestamp as date object, on which the metric was created.
	  /// </summary>
	  /// <returns> the timestamp </returns>
	  DateTime Timestamp {get;}

	  /// <summary>
	  /// Returns the value of the metric.
	  /// </summary>
	  /// <returns> the value </returns>
	  long Value {get;}
	}

}