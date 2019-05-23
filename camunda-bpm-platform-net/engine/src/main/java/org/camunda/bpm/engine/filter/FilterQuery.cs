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
namespace org.camunda.bpm.engine.filter
{

	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface FilterQuery : Query<FilterQuery, Filter>
	{

	  /// <param name="filterId"> set the filter id to query </param>
	  /// <returns> this query </returns>
	  FilterQuery filterId(string filterId);

	  /// <param name="resourceType"> set the filter resource type to query </param>
	  /// <returns> this query </returns>
	  FilterQuery filterResourceType(string resourceType);

	  /// <param name="name"> set the filter name to query </param>
	  /// <returns> this query </returns>
	  FilterQuery filterName(string name);

	  /// <param name="nameLike"> set the filter name like to query </param>
	  /// <returns> this query </returns>
	  FilterQuery filterNameLike(string nameLike);

	  /// <param name="owner"> set the filter owner to query </param>
	  /// <returns> this query </returns>
	  FilterQuery filterOwner(string owner);

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by filter id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  FilterQuery orderByFilterId();

	  /// <summary>
	  /// Order by filter id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  FilterQuery orderByFilterResourceType();

	  /// <summary>
	  /// Order by filter id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  FilterQuery orderByFilterName();

	  /// <summary>
	  /// Order by filter id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  FilterQuery orderByFilterOwner();

	}

}