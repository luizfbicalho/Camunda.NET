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
namespace org.camunda.bpm.engine.identity
{
	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Allows to programmatically query for <seealso cref="Tenant"/>s.
	/// </summary>
	public interface TenantQuery : Query<TenantQuery, Tenant>
	{

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s with the given id. </summary>
	  TenantQuery tenantId(string tenantId);

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s with the given ids </summary>
	  TenantQuery tenantIdIn(params string[] ids);

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s with the given name. </summary>
	  TenantQuery tenantName(string tenantName);

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s where the name matches the given parameter.
	  ///  The syntax to use is that of SQL, eg. %tenant%. 
	  /// </summary>
	  TenantQuery tenantNameLike(string tenantNameLike);

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s where the given user is member of. </summary>
	  TenantQuery userMember(string userId);

	  /// <summary>
	  /// Only select <seealso cref="Tenant"/>s where the given group is member of. </summary>
	  TenantQuery groupMember(string groupId);

	  /// <summary>
	  /// Selects the <seealso cref="Tenant"/>s which belongs to one of the user's groups.
	  /// Can only be used in combination with <seealso cref="userMember(string)"/> 
	  /// </summary>
	  TenantQuery includingGroupsOfUser(bool includingGroups);

	  //sorting ////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  TenantQuery orderByTenantId();

	  /// <summary>
	  /// Order by tenant name (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  TenantQuery orderByTenantName();

	}

}