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
namespace org.camunda.bpm.engine.filter
{
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface Filter
	{

	  /// <returns> the id of the filer </returns>
	  string Id {get;}

	  /// <returns> the resource type fo the filter </returns>
	  string ResourceType {get;}

	  /// <returns> the name of the filter </returns>
	  string Name {get;}

	  /// <param name="name"> the name of the filter </param>
	  /// <returns> this filter </returns>
	  Filter setName(string name);

	  /// <returns> the owner of the filter </returns>
	  string Owner {get;}

	  /// <param name="owner"> the owner of the filter </param>
	  /// <returns> this filter </returns>
	  Filter setOwner(string owner);

	  /// <returns> the saved query as query object </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T extends org.camunda.bpm.engine.query.Query<?, ?>> T getQuery();
	  T getQuery<T>() {get;}

	  /// <param name="query"> the saved query as query object </param>
	  /// <returns> this filter </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T extends org.camunda.bpm.engine.query.Query<?, ?>> Filter setQuery(T query);
	  Filter setQuery<T>(T query);

	  /// <summary>
	  /// Extends the query with the additional query. The query of the filter is therefore modified
	  /// and if the filter is saved the query is updated.
	  /// </summary>
	  /// <param name="extendingQuery"> the query to extend the filter with </param>
	  /// <returns> a copy of this filter with the extended query </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T extends org.camunda.bpm.engine.query.Query<?, ?>> Filter extend(T extendingQuery);
	  Filter extend<T>(T extendingQuery);

	  /// <returns> the properties as map </returns>
	  IDictionary<string, object> Properties {get;}

	  /// <param name="properties"> the properties to set as map </param>
	  /// <returns> this filter </returns>
	  Filter setProperties(IDictionary<string, object> properties);

	}

}