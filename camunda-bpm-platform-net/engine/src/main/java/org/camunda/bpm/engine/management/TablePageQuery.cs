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
namespace org.camunda.bpm.engine.management
{


	/// <summary>
	/// Allows programmatic querying of <seealso cref="TablePage"/>s.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface TablePageQuery
	{

	  /// <summary>
	  /// The name of the table of which a page must be fetched. 
	  /// </summary>
	  TablePageQuery tableName(string tableName);

	  /// <summary>
	  /// Orders the resulting table page rows by the given column in ascending order. 
	  /// </summary>
	  TablePageQuery orderAsc(string column);

	  /// <summary>
	  /// Orders the resulting table page rows by the given column in descending order. 
	  /// </summary>
	  TablePageQuery orderDesc(string column);

	  /// <summary>
	  /// Executes the query and returns the <seealso cref="TablePage"/>. 
	  /// </summary>
	  TablePage listPage(int firstResult, int maxResults);
	}

}