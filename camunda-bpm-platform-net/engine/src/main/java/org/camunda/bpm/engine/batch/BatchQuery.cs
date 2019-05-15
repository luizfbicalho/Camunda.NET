﻿/*
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

	/// <summary>
	/// Query for <seealso cref="Batch"/> instances.
	/// </summary>
	public interface BatchQuery : Query<BatchQuery, Batch>
	{

	  /// <summary>
	  /// Only select batch instances for the given batch id. </summary>
	  BatchQuery batchId(string batchId);

	  /// <summary>
	  /// Only select batches of the given type.
	  /// </summary>
	  BatchQuery type(string type);

	  /// <summary>
	  /// Only selects batches with one of the given tenant ids. </summary>
	  BatchQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects batches which have no tenant id. </summary>
	  BatchQuery withoutTenantId();

	  /// <summary>
	  /// Only selects batches which are active * </summary>
	  BatchQuery active();

	  /// <summary>
	  /// Only selects batches which are suspended * </summary>
	  BatchQuery suspended();

	  /// <summary>
	  /// Returns batches sorted by id; must be followed by an invocation of <seealso cref="#asc()"/> or <seealso cref="#desc()"/>.
	  /// </summary>
	  BatchQuery orderById();

	  /// <summary>
	  /// Returns batches sorted by tenant id; must be followed by an invocation of <seealso cref="#asc()"/> or <seealso cref="#desc()"/>.
	  /// </summary>
	  BatchQuery orderByTenantId();

	}

}