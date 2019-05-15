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
namespace org.camunda.bpm.engine.impl.management
{

	/// <summary>
	/// Represents an interface for the purge reporting.
	/// Contains all information of the data which is deleted during the purge.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public interface PurgeReporting<T>
	{

	  /// <summary>
	  /// Adds the key value pair as report information to the current purge report.
	  /// </summary>
	  /// <param name="key"> the report key </param>
	  /// <param name="value"> the report value </param>
	  void addPurgeInformation(string key, T value);

	  /// <summary>
	  /// Returns the current purge report.
	  /// </summary>
	  /// <returns> the purge report </returns>
	  IDictionary<string, T> PurgeReport {get;}

	  /// <summary>
	  /// Transforms and returns the purge report to a string.
	  /// </summary>
	  /// <returns> the purge report as string </returns>
	  string PurgeReportAsString {get;}

	  /// <summary>
	  /// Returns the value for the given key.
	  /// </summary>
	  /// <param name="key"> the key which exist in the current report </param>
	  /// <returns> the corresponding value </returns>
	  T getReportValue(string key);

	  /// <summary>
	  /// Returns true if the key is present in the current report. </summary>
	  /// <param name="key"> the key </param>
	  /// <returns> true if the key is present </returns>
	  bool containsReport(string key);

	  /// <summary>
	  /// Returns true if the report is empty.
	  /// </summary>
	  /// <returns> true if the report is empty, false otherwise </returns>
	  bool Empty {get;}
	}

}