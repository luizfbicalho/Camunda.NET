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
namespace org.camunda.bpm.engine.query
{

	/// <summary>
	/// Describes basic methods for querying.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public interface Query<T, U> where U : object
	{

	  /// <summary>
	  /// Order the results ascending on the given property as defined in this
	  /// class (needs to come after a call to one of the orderByXxxx methods).
	  /// </summary>
	  T asc();

	  /// <summary>
	  /// Order the results descending on the given property as defined in this
	  /// class (needs to come after a call to one of the orderByXxxx methods).
	  /// </summary>
	  T desc();

	  /// <summary>
	  /// Executes the query and returns the number of results </summary>
	  long count();

	  /// <summary>
	  /// Executes the query and returns the resulting entity or null if no
	  /// entity matches the query criteria. </summary>
	  /// <exception cref="ProcessEngineException"> when the query results in more than one
	  /// entities. </exception>
	  U singleResult();

	  /// <summary>
	  /// Executes the query and get a list of entities as the result. </summary>
	  IList<U> list();

	  /// <summary>
	  /// Executes the query and get a list of entities as the result. </summary>
	  IList<U> listPage(int firstResult, int maxResults);
	}

}