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
namespace org.camunda.bpm.engine.batch
{
	using Query = org.camunda.bpm.engine.query.Query;

	public interface BatchStatisticsQuery : Query<BatchStatisticsQuery, BatchStatistics>
	{

	  /// <summary>
	  /// Only select batch statistics for the given batch id.
	  /// </summary>
	  BatchStatisticsQuery batchId(string batchId);

	  /// <summary>
	  /// Only select batch statistics of the given type.
	  /// </summary>
	  BatchStatisticsQuery type(string type);

	  /// <summary>
	  /// Only selects batch statistics with one of the given tenant ids. </summary>
	  BatchStatisticsQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects batch statistics which have no tenant id. </summary>
	  BatchStatisticsQuery withoutTenantId();

	  /// <summary>
	  /// Only selects batches which are active * </summary>
	  BatchStatisticsQuery active();

	  /// <summary>
	  /// Only selects batches which are suspended * </summary>
	  BatchStatisticsQuery suspended();

	  /// <summary>
	  /// Returns batch statistics sorted by batch id; must be followed by an invocation of <seealso cref="asc()"/> or <seealso cref="desc()"/>.
	  /// </summary>
	  BatchStatisticsQuery orderById();

	  /// <summary>
	  /// Returns batch statistics sorted by tenant id; must be followed by an invocation of <seealso cref="asc()"/> or <seealso cref="desc()"/>.
	  /// </summary>
	  BatchStatisticsQuery orderByTenantId();

	}

}