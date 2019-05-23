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
	/// Allows programmatic querying of <seealso cref="User"/>
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface UserQuery : Query<UserQuery, User>
	{

	  /// <summary>
	  /// Only select <seealso cref="User"/>s with the given id/ </summary>
	  UserQuery userId(string id);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s with the given ids </summary>
	  UserQuery userIdIn(params string[] ids);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s with the given firstName. </summary>
	  UserQuery userFirstName(string firstName);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s where the first name matches the given parameter.
	  /// The syntax is that of SQL, eg. %activivi%.
	  /// </summary>
	  UserQuery userFirstNameLike(string firstNameLike);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s with the given lastName. </summary>
	  UserQuery userLastName(string lastName);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s where the last name matches the given parameter.
	  /// The syntax is that of SQL, eg. %activivi%.
	  /// </summary>
	  UserQuery userLastNameLike(string lastNameLike);

	  /// <summary>
	  /// Only those <seealso cref="User"/>s with the given email addres. </summary>
	  UserQuery userEmail(string email);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s where the email matches the given parameter.
	  /// The syntax is that of SQL, eg. %activivi%.
	  /// </summary>
	  UserQuery userEmailLike(string emailLike);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s that belong to the given group. </summary>
	  UserQuery memberOfGroup(string groupId);

	  /// <summary>
	  /// Only select <seealso cref="User"/>S that are potential starter for the given process definition. </summary>
	  UserQuery potentialStarter(string procDefId);

	  /// <summary>
	  /// Only select <seealso cref="User"/>s that belongs to the given tenant. </summary>
	  UserQuery memberOfTenant(string tenantId);

	  //sorting ////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by user id (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  UserQuery orderByUserId();

	  /// <summary>
	  /// Order by user first name (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  UserQuery orderByUserFirstName();

	  /// <summary>
	  /// Order by user last name (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  UserQuery orderByUserLastName();

	  /// <summary>
	  /// Order by user email  (needs to be followed by <seealso cref="asc()"/> or <seealso cref="desc()"/>). </summary>
	  UserQuery orderByUserEmail();

	}

}