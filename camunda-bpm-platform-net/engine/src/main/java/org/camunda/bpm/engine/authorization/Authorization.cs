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
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;

	/// <summary>
	/// <para>An <seealso cref="Authorization"/> assigns a set of <seealso cref="Permission Permissions"/>
	/// to an identity to interact with a given <seealso cref="Resource"/>.</para>
	/// <para>EXAMPLES:
	/// <ul>
	///   <li>User 'jonny' is authorized to start new instances of the 'invoice' process</li>
	///   <li>Group 'marketing' is not authorized to cancel process instances.</li>
	///   <li>Group 'marketing' is not allowed to use the tasklist application.</li>
	///   <li>Nobody is allowed to edit process variables in the cockpit application,
	///   except the distinct user 'admin'.</li>
	/// </ul>
	/// 
	/// <h2>Identities</h2>
	/// </para>
	/// <para>camunda BPM distinguished two types of identities: <em>users</em> and
	/// <em>groups</em>. Authorizations can either range over all users
	/// (userId = <seealso cref="#ANY"/>), an individual <seealso cref="User"/> or a <seealso cref="Group"/> of users.</para>
	/// 
	/// <h2>Permissions</h2>
	/// <para>A <seealso cref="Permission"/> defines the way an identity is allowed to interact
	/// with a certain resource. Examples of permissions are <seealso cref="Permissions#CREATE CREATE"/>,
	/// <seealso cref="Permissions#READ READ"/>, <seealso cref="Permissions#UPDATE UPDATE"/>,
	/// <seealso cref="Permissions#DELETE DELETE"/>, ... See <seealso cref="Permissions"/> for a set of
	/// built-in permissions.</para>
	/// 
	/// <para>A single authorization object may assign multiple permissions to a single user
	/// and resource:</para>
	/// <pre>
	/// authorization.addPermission(Permissions.READ);
	/// authorization.addPermission(Permissions.UPDATE);
	/// authorization.addPermission(Permissions.DELETE);
	/// </pre>
	/// <para>On top of the built-in permissions, camunda BPM allows using custom
	/// permission types.</para>
	/// 
	/// <h2>Resources</h2>
	/// <para>Resources are the entities the user interacts with. Examples of resources are
	/// <seealso cref="Resources#GROUP GROUPS"/>, <seealso cref="Resources#USER USERS"/>,
	/// process-definitions, process-instances, tasks ... See <seealso cref="Resources"/> for a set
	/// of built-in resource. The camunda BPM framework supports custom resources.</para>
	/// 
	/// <h2>Authorization Type</h2>
	/// <para>There are three types of authorizations:
	/// <ul>
	///   <li><strong>Global Authorizations</strong> (<seealso cref="#AUTH_TYPE_GLOBAL"/>) range over
	///   all users and groups (userId = <seealso cref="#ANY"/>) and are usually used for fixing the
	///   "base" permission for a resource.</li>
	///   <li><strong>Grant Authorizations</strong> (<seealso cref="#AUTH_TYPE_GRANT"/>) range over
	///   users and groups and grant a set of permissions. Grant authorizations are commonly
	///   used for adding permissions to a user or group that the global authorization revokes.</li>
	///   <li><strong>Revoke Authorizations</strong> (<seealso cref="#AUTH_TYPE_REVOKE"/>) range over
	///   users and groups and revoke a set of permissions. Revoke authorizations are commonly
	///   used for revoking permissions to a user or group the the global authorization grants.</li>
	/// </ul>
	/// </para>
	/// 
	/// <h2>Authorization Precedence</h2>
	/// <para>Authorizations may range over all users, an individual user or a group of users or .
	/// They may apply to an individual resource instance or all instances of the same type
	/// (resourceId = <seealso cref="#ANY"/>). The precedence is as follows:
	/// <ol>
	///  <li>An authorization applying to an individual resource instance preceds over an authorization
	///  applying to all instances of the same resource type.</li>
	///  <li>An authorization for an individual user preceds over an authorization for a group.</li>
	///  <li>A Group authorization preced over a <seealso cref="#AUTH_TYPE_GLOBAL GLOBAL"/> authorization.</li>
	///  <li>A Group <seealso cref="#AUTH_TYPE_REVOKE REVOKE"/> authorization preced over a Group
	///  <seealso cref="#AUTH_TYPE_GRANT GRANT"/> authorization.</li>
	/// </ol>
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// @since 7.0
	/// 
	/// </summary>
	public interface Authorization
	{

	  /// <summary>
	  /// A Global Authorization ranges over all users and groups (userId = <seealso cref="#ANY"/>) and are
	  /// usually used for fixing the "base" permission for a resource.
	  /// </summary>

	  /// <summary>
	  /// A Grant Authorization ranges over a users or a group and grants a set of permissions.
	  /// Grant authorizations are commonly used for adding permissions to a user or group that
	  /// the global authorization revokes.
	  /// </summary>

	  /// <summary>
	  /// A Revoke Authorization ranges over a user or a group and revokes a set of permissions.
	  /// Revoke authorizations are commonly used for revoking permissions to a user or group the
	  /// the global authorization grants.
	  /// </summary>

	  /// <summary>
	  /// The identifier used for relating to all users or all resourceIds.
	  ///  Cannot be used for groups.
	  /// </summary>

	  /// <summary>
	  /// allows granting a permission. Out-of-the-box constants can be found in <seealso cref="Permissions"/>.
	  /// 
	  /// </summary>
	  void addPermission(Permission permission);

	  /// <summary>
	  /// allows removing a permission. Out-of-the-box constants can be found in <seealso cref="Permissions"/>.
	  /// 
	  /// </summary>
	  void removePermission(Permission permission);

	  /// <summary>
	  /// Allows checking whether this authorization grants a specific permission.
	  /// </summary>
	  /// <param name="perm"> the permission to check for </param>
	  /// <exception cref="IllegalStateException"> if this <seealso cref="Authorization"/> is of type <seealso cref="#AUTH_TYPE_REVOKE"/> </exception>
	  bool isPermissionGranted(Permission permission);

	  /// <summary>
	  /// Allows checking whether this authorization revokes a specific permission.
	  /// </summary>
	  /// <param name="perm"> the permission to check for </param>
	  /// <exception cref="IllegalStateException"> if this <seealso cref="Authorization"/> is of type <seealso cref="#AUTH_TYPE_GRANT"/> </exception>
	  bool isPermissionRevoked(Permission permission);

	  /// <summary>
	  /// Allows checking whether this authorization grants every single permission.
	  /// </summary>
	  /// <returns> true if every single permission is granted otherwise false </returns>
	  /// <exception cref="IllegalStateException"> if this <seealso cref="Authorization"/> is of type <seealso cref="#AUTH_TYPE_REVOKE"/> </exception>
	  bool EveryPermissionGranted {get;}

	  /// <summary>
	  /// Allows checking whether this authorization revokes every single permission.
	  /// </summary>
	  /// <returns> true if every single permission is revoked otherwise false </returns>
	  /// <exception cref="IllegalStateException"> if this <seealso cref="Authorization"/> is of type <seealso cref="#AUTH_TYPE_GRANT"/> </exception>
	  bool EveryPermissionRevoked {get;}

	  /// <summary>
	  /// Allows checking whether this authorization grants / revokes a set of permissions.
	  /// Usually the set of built-in permissions is used: <seealso cref="Permissions#values()"/>
	  /// 
	  /// The return value of this method depends on the type of the authorization:
	  /// <ul>
	  ///  <li>For <seealso cref="#AUTH_TYPE_GLOBAL"/>: all permissions in the parameter list granted by this authorization are returned. </li>
	  ///  <li>For <seealso cref="#AUTH_TYPE_GRANT"/>: all permissions in the parameter list granted by this authorization are returned. </li>
	  ///  <li>For <seealso cref="#AUTH_TYPE_REVOKE"/>: all permissions in the parameter list revoked by this authorization are returned. </li>
	  /// </ul>
	  /// </summary>
	  /// <param name="an"> array of permissions to check for. </param>
	  /// <returns> Returns the set of <seealso cref="Permission Permissions"/> provided by this <seealso cref="Authorization"/>.
	  ///  </returns>
	  Permission[] getPermissions(Permission[] permissions);

	  /// <summary>
	  /// Sets the permissions to the provided value. Replaces all permissions.
	  /// 
	  /// The effect of this method depends on the type of this authorization:
	  /// <ul>
	  ///  <li>For <seealso cref="#AUTH_TYPE_GLOBAL"/>: all provided permissions are granted.</li>
	  ///  <li>For <seealso cref="#AUTH_TYPE_GRANT"/>: all provided permissions are granted.</li>
	  ///  <li>For <seealso cref="#AUTH_TYPE_REVOKE"/>: all provided permissions are revoked.</li>
	  /// </ul>
	  /// </summary>
	  ///  <param name="a"> set of permissions.
	  ///  </param>
	  Permission[] Permissions {set;}


	  /// <returns> the ID of the <seealso cref="Authorization"/> object </returns>
	  string Id {get;}

	  /// <summary>
	  /// set the id of the resource
	  /// </summary>
	  string ResourceId {set;get;}


	  /// <summary>
	  /// sets the type of the resource
	  /// </summary>
	  int ResourceType {set;get;}

	  /// <summary>
	  /// sets the type of the resource
	  /// </summary>
	  Resource Resource {set;}


	  /// <summary>
	  /// set the id of the user this authorization is created for
	  /// </summary>
	  string UserId {set;get;}


	  /// <summary>
	  /// set the id of the group this authorization is created for
	  /// </summary>
	  string GroupId {set;get;}


	  /// <summary>
	  /// The type og the authorization. Legal values:
	  /// <ul>
	  /// <li><seealso cref="#AUTH_TYPE_GLOBAL"/></li>
	  /// <li><seealso cref="#AUTH_TYPE_GRANT"/></li>
	  /// <li><seealso cref="#AUTH_TYPE_REVOKE"/></li>
	  /// </ul>
	  /// </summary>
	  /// <returns> the type of the authorization.
	  ///  </returns>
	  int AuthorizationType {get;}

	}

	public static class Authorization_Fields
	{
	  public const int AUTH_TYPE_GLOBAL = 0;
	  public const int AUTH_TYPE_GRANT = 1;
	  public const int AUTH_TYPE_REVOKE = 2;
	  public const string ANY = "*";
	}

}