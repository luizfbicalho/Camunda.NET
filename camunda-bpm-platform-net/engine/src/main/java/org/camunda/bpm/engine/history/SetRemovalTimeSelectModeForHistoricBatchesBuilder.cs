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
namespace org.camunda.bpm.engine.history
{

	/// <summary>
	/// Fluent builder to select the mode to set the removal time for historic batches.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface SetRemovalTimeSelectModeForHistoricBatchesBuilder : SetRemovalTimeToHistoricBatchesBuilder
	{

	  /// <summary>
	  /// Sets the removal time to an absolute date.
	  /// </summary>
	  /// <param name="removalTime"> supposed to be set to historic entities. </param>
	  /// <returns> the builder. </returns>
	  SetRemovalTimeToHistoricBatchesBuilder absoluteRemovalTime(DateTime removalTime);

	  /// <summary>
	  /// Calculates the removal time dynamically based on the time to
	  /// live of the respective batch and the engine's removal time strategy.
	  /// </summary>
	  /// <returns> the builder. </returns>
	  SetRemovalTimeToHistoricBatchesBuilder calculatedRemovalTime();

	  /// <summary>
	  /// <para> Sets the removal time to {@code null}.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the builder. </returns>
	  SetRemovalTimeToHistoricBatchesBuilder clearedRemovalTime();

	}

}