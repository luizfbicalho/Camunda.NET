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
namespace org.camunda.bpm.engine.authorization
{
	using Query = org.camunda.bpm.engine.query.Query;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface AuthorizationQuery : Query<AuthorizationQuery, Authorization>
	{

	  /// <summary>
	  /// only selects authorizations for the given id </summary>
	  AuthorizationQuery authorizationId(string id);

	  /// <summary>
	  /// only selects authorizations for the given type. Legal values:
	  /// <seealso cref="Authorization.AUTH_TYPE_GLOBAL"/>, <seealso cref="Authorization.AUTH_TYPE_GRANT"/>
	  /// <seealso cref="Authorization.AUTH_TYPE_REVOKE"/> 
	  /// </summary>
	  AuthorizationQuery authorizationType(int? type);

	  /// <summary>
	  /// only selects authorizations for the given user ids </summary>
	  AuthorizationQuery userIdIn(params string[] userIds);

	  /// <summary>
	  /// only selects authorizations for the given group ids </summary>
	  AuthorizationQuery groupIdIn(params string[] groupIds);

	  /// <summary>
	  /// only selects authorizations for the given resource type </summary>
	  AuthorizationQuery resourceType(Resource resource);

	  /// <summary>
	  /// only selects authorizations for the given resource type </summary>
	  AuthorizationQuery resourceType(int resourceType);

	  /// <summary>
	  /// only selects authorizations for the given resource id </summary>
	  AuthorizationQuery resourceId(string resourceId);

	  /// <summary>
	  /// only selects authorizations which grant the permissions represented by the parameter.
	  /// If this method is called multiple times, all passed-in permissions will be checked with AND semantics.
	  /// Example:
	  /// 
	  /// <pre>
	  /// authorizationQuery.userId("user1")
	  ///   .resourceType("processDefinition")
	  ///   .resourceId("2313")
	  ///   .hasPermission(Permissions.READ)
	  ///   .hasPermission(Permissions.UPDATE)
	  ///   .hasPermission(Permissions.DELETE)
	  ///   .list();
	  /// </pre>
	  /// 
	  /// Selects all Authorization objects which provide READ,UPDATE,DELETE 
	  /// Permissions for the given user. 
	  /// 
	  /// </summary>
	  AuthorizationQuery hasPermission(Permission permission);

	  // order by /////////////////////////////////////////////

	  /// <summary>
	  /// Order by resource type (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  AuthorizationQuery orderByResourceType();

	  /// <summary>
	  /// Order by resource id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  AuthorizationQuery orderByResourceId();

	}

}