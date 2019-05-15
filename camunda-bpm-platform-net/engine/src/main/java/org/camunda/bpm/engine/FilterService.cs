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
namespace org.camunda.bpm.engine
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface FilterService
	{

	  /// <summary>
	  /// Creates a new task filter.
	  /// </summary>
	  /// <returns> a new task filter </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#CREATE"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  Filter newTaskFilter();

	  /// <summary>
	  /// Creates a new task filter with a given name.
	  /// </summary>
	  /// <returns> a new task filter with a name </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#CREATE"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  Filter newTaskFilter(string filterName);

	  /// <summary>
	  /// Creates a new filter query
	  /// </summary>
	  /// <returns> a new query for filters </returns>
	  FilterQuery createFilterQuery();


	  /// <summary>
	  /// Creates a new task filter query.
	  /// </summary>
	  /// <returns> a new query for task filters </returns>
	  FilterQuery createTaskFilterQuery();

	  /// <summary>
	  /// Saves the filter in the database.
	  /// </summary>
	  /// <param name="filter"> the filter to save </param>
	  /// <returns> return the saved filter </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#CREATE"/> permissions on <seealso cref="Resources#FILTER"/> (save new filter)
	  /// or if user has no <seealso cref="Permissions#UPDATE"/> permissions on <seealso cref="Resources#FILTER"/> (update existing filter). </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  Filter saveFilter(Filter filter);

	  /// <summary>
	  /// Returns the filter for the given filter id.
	  /// </summary>
	  /// <param name="filterId"> the id of the filter </param>
	  /// <returns> the filter </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  Filter getFilter(string filterId);

	  /// <summary>
	  /// Deletes a filter by its id.
	  /// </summary>
	  /// <param name="filterId"> the id of the filter </param>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#DELETE"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  void deleteFilter(string filterId);

	  /// <summary>
	  /// Executes the query of the filter and returns the result as list.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <returns> the query result as list </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  IList<T> list<T>(string filterId);

	  /// <summary>
	  /// Executes the extended query of a filter and returns the result as list.
	  /// </summary>
	  /// <param name="filterId"> the id of the filter </param>
	  /// <param name="extendingQuery"> additional query to extend the filter query </param>
	  /// <returns> the query result as list </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///   <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///   <li>When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> java.util.List<T> list(String filterId, Q extendingQuery);
	  IList<T> list<T, Q>(string filterId, Q extendingQuery);

	  /// <summary>
	  /// Executes the query of the filter and returns the result in the given boundaries as list.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <param name="firstResult"> first result to select </param>
	  /// <param name="maxResults"> maximal number of results </param>
	  /// <returns> the query result as list </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  IList<T> listPage<T>(string filterId, int firstResult, int maxResults);

	  /// <summary>
	  /// Executes the extended query of a filter and returns the result in the given boundaries as list.
	  /// </summary>
	  /// <param name="extendingQuery"> additional query to extend the filter query </param>
	  /// <param name="filterId"> the id of the filter </param>
	  /// <param name="firstResult"> first result to select </param>
	  /// <param name="maxResults"> maximal number of results </param>
	  /// <returns> the query result as list </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  <li>When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> java.util.List<T> listPage(String filterId, Q extendingQuery, int firstResult, int maxResults);
	  IList<T> listPage<T, Q>(string filterId, Q extendingQuery, int firstResult, int maxResults);

	  /// <summary>
	  /// Executes the query of the filter and returns the a single result.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <returns> the single query result </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  T singleResult<T>(string filterId);

	  /// <summary>
	  /// Executes the extended query of the filter and returns the a single result.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <param name="extendingQuery"> additional query to extend the filter query </param>
	  /// <returns> the single query result </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  <li>When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: <T, Q extends org.camunda.bpm.engine.query.Query<?, T>> T singleResult(String filterId, Q extendingQuery);
	  T singleResult<T, Q>(string filterId, Q extendingQuery);

	  /// <summary>
	  /// Executes the query of the filter and returns the result count.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <returns> the result count </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
	  long? count(string filterId);

	  /// <summary>
	  /// Executes the extended query of the filter and returns the result count.
	  /// </summary>
	  /// <param name="filterId"> the the id of the filter </param>
	  /// <param name="extendingQuery"> additional query to extend the filter query </param>
	  /// <returns> the result count </returns>
	  /// <exception cref="AuthorizationException"> if the user has no <seealso cref="Permissions#READ"/> permissions on <seealso cref="Resources#FILTER"/>. </exception>
	  /// <exception cref="BadUserRequestException">
	  ///  <ul><li>When the filter query uses expressions and expression evaluation is deactivated for stored queries.
	  ///  <li>When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
	  ///  Expression evaluation can be activated by setting the process engine configuration properties
	  ///  <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
	  ///  <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: System.Nullable<long> count(String filterId, org.camunda.bpm.engine.query.Query<?, ?> extendingQuery);
	  long? count<T1>(string filterId, Query<T1> extendingQuery);

	}

}