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
namespace org.camunda.bpm.engine.runtime
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public interface InstantiationBuilder<T> where T : InstantiationBuilder<T>
	{

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Start before the specified activity.</para>
	  /// 
	  /// <para>In particular:
	  ///   <ul>
	  ///     <li>In the parent activity hierarchy, determine the closest existing ancestor activity instance</li>
	  ///     <li>Instantiate all parent activities up to the ancestor's activity</li>
	  ///     <li>Instantiate and execute the given activity (respects the asyncBefore
	  ///       attribute of the activity)</li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="activityId"> the activity to instantiate </param>
	  /// <exception cref="ProcessEngineException"> if more than one possible ancestor activity instance exists </exception>
	  T startBeforeActivity(string activityId);

	  /// <summary>
	  /// Submits an instruction that behaves like <seealso cref="startTransition(string)"/> and always instantiates
	  /// the single outgoing sequence flow of the given activity. Does not consider asyncAfter.
	  /// </summary>
	  /// <param name="activityId"> the activity for which the outgoing flow should be executed </param>
	  /// <exception cref="ProcessEngineException"> if the activity has 0 or more than 1 outgoing sequence flows </exception>
	  T startAfterActivity(string activityId);

	  /// <summary>
	  /// <para><i>Submits the instruction:</i></para>
	  /// 
	  /// <para>Start a sequence flow.</para>
	  /// 
	  /// <para>In particular:
	  ///   <ul>
	  ///     <li>In the parent activity hierarchy, determine the closest existing ancestor activity instance</li>
	  ///     <li>Instantiate all parent activities up to the ancestor's activity</li>
	  ///     <li>Execute the given transition (does not consider sequence flow conditions)</li>
	  ///   </ul>
	  /// </para>
	  /// </summary>
	  /// <param name="transitionId"> the sequence flow to execute </param>
	  /// <exception cref="ProcessEngineException"> if more than one possible ancestor activity instance exists </exception>
	  T startTransition(string transitionId);
	}

}