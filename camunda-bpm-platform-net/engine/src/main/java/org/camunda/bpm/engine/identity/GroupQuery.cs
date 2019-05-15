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
	/// Allows to programmatically query for <seealso cref="Group"/>s.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface GroupQuery : Query<GroupQuery, Group>
	{

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s with the given id. </summary>
	  GroupQuery groupId(string groupId);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s with the given ids </summary>
	  GroupQuery groupIdIn(params string[] ids);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s with the given name. </summary>
	  GroupQuery groupName(string groupName);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s where the name matches the given parameter.
	  ///  The syntax to use is that of SQL, eg. %activiti%. 
	  /// </summary>
	  GroupQuery groupNameLike(string groupNameLike);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s which have the given type. </summary>
	  GroupQuery groupType(string groupType);

	  /// <summary>
	  /// Only selects <seealso cref="Group"/>s where the given user is a member of. </summary>
	  GroupQuery groupMember(string groupMemberUserId);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>S that are potential starter for the given process definition. </summary>
	  GroupQuery potentialStarter(string procDefId);

	  /// <summary>
	  /// Only select <seealso cref="Group"/>s that belongs to the given tenant. </summary>
	  GroupQuery memberOfTenant(string tenantId);

	  //sorting ////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by group id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  GroupQuery orderByGroupId();

	  /// <summary>
	  /// Order by group name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  GroupQuery orderByGroupName();

	  /// <summary>
	  /// Order by group type (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  GroupQuery orderByGroupType();

	}

}