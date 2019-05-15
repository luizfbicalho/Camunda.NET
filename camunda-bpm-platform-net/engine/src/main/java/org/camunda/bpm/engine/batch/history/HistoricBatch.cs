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
namespace org.camunda.bpm.engine.batch.history
{

	/// <summary>
	/// Historic representation of a <seealso cref="org.camunda.bpm.engine.batch.Batch"/>.
	/// </summary>
	public interface HistoricBatch
	{

	  /// <returns> the id of the batch </returns>
	  string Id {get;}

	  /// <returns> the type of the batch </returns>
	  string Type {get;}

	  /// <returns> the number of batch execution jobs required to complete the batch </returns>
	  int TotalJobs {get;}

	  /// <returns> number of batch jobs created per batch seed job invocation </returns>
	  int BatchJobsPerSeed {get;}

	  /// <returns> the number of invocations executed per batch job </returns>
	  int InvocationsPerBatchJob {get;}

	  /// <returns> the id of the batch seed job definition </returns>
	  string SeedJobDefinitionId {get;}

	  /// <returns> the id of the batch monitor job definition </returns>
	  string MonitorJobDefinitionId {get;}

	  /// <returns> the id of the batch job definition </returns>
	  string BatchJobDefinitionId {get;}

	  /// <returns> the batch's tenant id or null </returns>
	  string TenantId {get;}

	  /// <returns> the batch creator's user id </returns>
	  string CreateUserId {get;}

	  /// <returns> the date the batch was started </returns>
	  DateTime StartTime {get;}

	  /// <returns> the date the batch was completed </returns>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// The time the historic batch will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}