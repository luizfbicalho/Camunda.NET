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
	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatchQuery = org.camunda.bpm.engine.batch.history.HistoricBatchQuery;

	/// <summary>
	/// Fluent builder to set the removal time to historic batches and
	/// all associated historic entities.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface SetRemovalTimeToHistoricBatchesBuilder
	{

	  /// <summary>
	  /// Selects historic batches by the given query.
	  /// </summary>
	  /// <param name="historicBatchQuery"> to be evaluated. </param>
	  /// <returns> the builder. </returns>
	  SetRemovalTimeToHistoricBatchesBuilder byQuery(HistoricBatchQuery historicBatchQuery);

	  /// <summary>
	  /// Selects historic batches by the given ids.
	  /// </summary>
	  /// <param name="historicBatchIds"> supposed to be affected. </param>
	  /// <returns> the builder. </returns>
	  SetRemovalTimeToHistoricBatchesBuilder byIds(params string[] historicBatchIds);

	  /// <summary>
	  /// Sets the removal time asynchronously as batch. The returned batch can be used to
	  /// track the progress of setting a removal time.
	  /// </summary>
	  /// <exception cref="BadUserRequestException"> when no historic batches could be found. </exception>
	  /// <exception cref="AuthorizationException"> when no <seealso cref="BatchPermissions#CREATE_BATCH_SET_REMOVAL_TIME"/> permission
	  /// is granted on <seealso cref="Resources#BATCH"/>.
	  /// </exception>
	  /// <returns> the batch which sets the removal time asynchronously. </returns>
	  Batch executeAsync();

	}

}