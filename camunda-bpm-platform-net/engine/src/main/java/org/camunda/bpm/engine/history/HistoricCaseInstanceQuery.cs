using System;
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
namespace org.camunda.bpm.engine.history
{

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="HistoricCaseInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface HistoricCaseInstanceQuery : Query<HistoricCaseInstanceQuery, HistoricCaseInstance>
	{

	  /// <summary>
	  /// Only select historic case instances with the given case instance id. </summary>
	  HistoricCaseInstanceQuery caseInstanceId(string caseInstanceId);

	  /// <summary>
	  /// Only select historic case instances whose id is in the given set of ids. </summary>
	  HistoricCaseInstanceQuery caseInstanceIds(ISet<string> caseInstanceIds);

	  /// <summary>
	  /// Only select historic case instances for the given case definition </summary>
	  HistoricCaseInstanceQuery caseDefinitionId(string caseDefinitionId);

	  /// <summary>
	  /// Only select historic case instances that are defined by a case definition with the given key. </summary>
	  HistoricCaseInstanceQuery caseDefinitionKey(string caseDefinitionKey);

	  /// <summary>
	  /// Only select historic case instances that don't have a case definition of which the key is present in the given list </summary>
	  HistoricCaseInstanceQuery caseDefinitionKeyNotIn(IList<string> caseDefinitionKeys);

	  /// <summary>
	  /// Only select historic case instances that are defined by a case definition with the given name. </summary>
	  HistoricCaseInstanceQuery caseDefinitionName(string caseDefinitionName);

	  /// <summary>
	  /// Only select historic case instances that are defined by case definition which name
	  /// is like the given value.
	  /// </summary>
	  /// <param name="nameLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%). </param>
	  HistoricCaseInstanceQuery caseDefinitionNameLike(string nameLike);

	  /// <summary>
	  /// Only select historic case instances with the given business key </summary>
	  HistoricCaseInstanceQuery caseInstanceBusinessKey(string caseInstanceBusinessKey);

	  /// <summary>
	  /// Only select historic case instances which had a business key like the given value.
	  /// </summary>
	  /// <param name="caseInstanceBusinessKeyLike"> The string can include the wildcard character '%' to express
	  ///    like-strategy: starts with (string%), ends with (%string) or contains (%string%). </param>
	  HistoricCaseInstanceQuery caseInstanceBusinessKeyLike(string caseInstanceBusinessKeyLike);

	  /// <summary>
	  /// <para>Only selects historic case instances with historic case activity instances
	  /// in at least one of the given case activity ids.</para>
	  /// </summary>
	  HistoricCaseInstanceQuery caseActivityIdIn(params string[] caseActivityIds);

	  /// <summary>
	  /// Only select historic case instances that were created before the given date. </summary>
	  HistoricCaseInstanceQuery createdBefore(DateTime date);

	  /// <summary>
	  /// Only select historic case instances that were created after the given date. </summary>
	  HistoricCaseInstanceQuery createdAfter(DateTime date);

	  /// <summary>
	  /// Only select historic case instances that were closed before the given date. </summary>
	  HistoricCaseInstanceQuery closedBefore(DateTime date);

	  /// <summary>
	  /// Only select historic case instances that were closed after the given date. </summary>
	  HistoricCaseInstanceQuery closedAfter(DateTime date);

	  /// <summary>
	  /// Only select historic case instance that are created by the given user. </summary>
	  HistoricCaseInstanceQuery createdBy(string userId);

	  /// <summary>
	  /// Only select historic case instances started by the given case instance. </summary>
	  HistoricCaseInstanceQuery superCaseInstanceId(string superCaseInstanceId);

	  /// <summary>
	  /// Only select historic case instances having a sub case instance
	  /// with the given case instance id.
	  /// 
	  /// Note that there will always be maximum only <b>one</b>
	  /// such case instance that can be the result of this query.
	  /// </summary>
	  HistoricCaseInstanceQuery subCaseInstanceId(string subCaseInstanceId);

	  /// <summary>
	  /// Only select historic case instances started by the given process instance. </summary>
	  HistoricCaseInstanceQuery superProcessInstanceId(string superProcessInstanceId);

	  /// <summary>
	  /// Only select historic case instances having a sub process instance
	  /// with the given process instance id.
	  /// 
	  /// Note that there will always be maximum only <b>one</b>
	  /// such case instance that can be the result of this query.
	  /// </summary>
	  HistoricCaseInstanceQuery subProcessInstanceId(string subProcessInstanceId);

	  /// <summary>
	  /// Only select historic case instances with one of the given tenant ids. </summary>
	  HistoricCaseInstanceQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects historic case instances which have no tenant id. </summary>
	  HistoricCaseInstanceQuery withoutTenantId();

	  /// <summary>
	  /// Only select historic case instances which are active </summary>
	  HistoricCaseInstanceQuery active();

	  /// <summary>
	  /// Only select historic case instances which are completed </summary>
	  HistoricCaseInstanceQuery completed();

	  /// <summary>
	  /// Only select historic case instances which are terminated </summary>
	  HistoricCaseInstanceQuery terminated();

	  /// <summary>
	  /// Only select historic case instances which are closed </summary>
	  HistoricCaseInstanceQuery closed();

	  /// <summary>
	  /// Only select historic case instance that are not yet closed. </summary>
	  HistoricCaseInstanceQuery notClosed();

	  /// <summary>
	  /// Only select case instances which have a global variable with the given value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name is null </exception>
	  HistoricCaseInstanceQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select case instances which have a global variable with the given name
	  /// but a different value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name is null </exception>
	  HistoricCaseInstanceQuery variableValueNotEquals(string name, object value);

	  /// <summary>
	  /// Only select case instances which had a global variable with the given name
	  /// and a value greater than the given value when they where closed.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name or value is null </exception>
	  HistoricCaseInstanceQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select case instances which have a global variable with the given name
	  /// and a value greater or equal than the given value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name or value is null </exception>
	  HistoricCaseInstanceQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case instances which have a global variable with the given name
	  /// and a value less than the given value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name or value is null </exception>
	  HistoricCaseInstanceQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select case instances which have a global variable with the given name
	  /// and a value less or equal than the given value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable </param>
	  /// <exception cref="NotValidException"> if the name or value is null </exception>
	  HistoricCaseInstanceQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select case instances which have a global variable with the given name
	  /// and a value like given value.
	  /// </summary>
	  /// <param name="name"> the name of the variable </param>
	  /// <param name="value"> the value of the variable, it can include the wildcard character '%'
	  ///              to express like-strategy: starts with (string%), ends with (%string),
	  ///              contains (%string%) </param>
	  /// <exception cref="NotValidException"> if the name or value is null </exception>
	  HistoricCaseInstanceQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Order by the case instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseInstanceId();

	  /// <summary>
	  /// Order by the case definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseDefinitionId();

	  /// <summary>
	  /// Order by the business key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseInstanceBusinessKey();

	  /// <summary>
	  /// Order by the create time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseInstanceCreateTime();

	  /// <summary>
	  /// Order by the close time (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseInstanceCloseTime();

	  /// <summary>
	  /// Order by the duration of the case instance (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricCaseInstanceQuery orderByCaseInstanceDuration();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of historic case instances without tenant id is database-specific.
	  /// </summary>
	  HistoricCaseInstanceQuery orderByTenantId();

	}

}