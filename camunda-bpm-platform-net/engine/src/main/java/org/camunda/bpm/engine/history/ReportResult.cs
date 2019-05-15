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
namespace org.camunda.bpm.engine.history
{
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;

	/// <summary>
	/// This interface defines basic methods for resulting reports.
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface ReportResult
	{

	  /// <summary>
	  /// <para>Returns a period which specifies a time span within a year.</para>
	  /// 
	  /// <para>The returned period must be interpreted in conjunction
	  /// with the returned <seealso cref="PeriodUnit"/> of <seealso cref="#getPeriodUnit()"/>.</para>
	  /// 
	  /// </p>For example:</p>
	  /// <ul>
	  ///   <li><seealso cref="#getPeriodUnit()"/> returns <seealso cref="PeriodUnit#MONTH"/>
	  ///   <li><seealso cref="#getPeriod()"/> returns <code>3</code>
	  /// </ul>
	  /// 
	  /// <para>The returned period <code>3</code> must be interpreted as
	  /// the third <code>month</code> of the year (i.e. it represents
	  /// the month March).</para>
	  /// 
	  /// <para>If the <seealso cref="#getPeriodUnit()"/> returns <seealso cref="PeriodUnit#QUARTER"/>,
	  /// then the returned period <code>3</code> must be interpreted as the third
	  /// <code>quarter</code> of the year.</para>
	  /// </summary>
	  /// <returns> an integer representing span of time within a year </returns>
	  int Period {get;}

	  /// <summary>
	  /// <para>Returns the unit of the period.</para>
	  /// </summary>
	  /// <returns> a <seealso cref="PeriodUnit"/>
	  /// </returns>
	  /// <seealso cref= #getPeriod() </seealso>
	  PeriodUnit PeriodUnit {get;}

	}

}