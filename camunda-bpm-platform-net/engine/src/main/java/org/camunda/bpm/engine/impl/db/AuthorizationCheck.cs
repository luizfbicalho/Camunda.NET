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
namespace org.camunda.bpm.engine.impl.db
{

	using Permissions = org.camunda.bpm.engine.authorization.Permissions;

	/// <summary>
	/// <para>Input for the authorization check algorithm</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class AuthorizationCheck
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// If true authorization check is enabled. for This switch is
	  /// useful when implementing a query which may perform an authorization check
	  /// only under certain circumstances.
	  /// </summary>
	  protected internal bool isAuthorizationCheckEnabled = false;

	  /// <summary>
	  /// If true authorization check is performed.
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool shouldPerformAuthorizatioCheck_Conflict = false;

	  /// <summary>
	  /// Indicates if the revoke authorization checks are enabled or not.
	  /// The authorization checks without checking revoke permissions are much more faster.
	  /// </summary>
	  protected internal bool isRevokeAuthorizationCheckEnabled = false;

	  /// <summary>
	  /// the id of the user to check permissions for </summary>
	  protected internal string authUserId;

	  /// <summary>
	  /// the ids of the groups to check permissions for </summary>
	  protected internal IList<string> authGroupIds = new List<string>();

	  /// <summary>
	  /// the default permissions to use if no matching authorization
	  /// can be found.
	  /// </summary>
	  protected internal int authDefaultPerm = Permissions.ALL.Value;

	  protected internal CompositePermissionCheck permissionChecks = new CompositePermissionCheck();

	  public AuthorizationCheck()
	  {
	  }

	  public AuthorizationCheck(string authUserId, IList<string> authGroupIds, CompositePermissionCheck permissionCheck, bool isRevokeAuthorizationCheckEnabled)
	  {
		this.authUserId = authUserId;
		this.authGroupIds = authGroupIds;
		this.permissionChecks = permissionCheck;
		this.isRevokeAuthorizationCheckEnabled = isRevokeAuthorizationCheckEnabled;
	  }

	  // getters / setters /////////////////////////////////////////

	  public virtual bool AuthorizationCheckEnabled
	  {
		  get
		  {
			return isAuthorizationCheckEnabled;
		  }
		  set
		  {
			this.isAuthorizationCheckEnabled = value;
		  }
	  }

	  public virtual bool IsAuthorizationCheckEnabled
	  {
		  get
		  {
			return isAuthorizationCheckEnabled;
		  }
	  }


	  public virtual bool shouldPerformAuthorizatioCheck()
	  {
		return shouldPerformAuthorizatioCheck_Conflict;
	  }

	  /// <summary>
	  /// is used by myBatis </summary>
	  public virtual bool ShouldPerformAuthorizatioCheck
	  {
		  get
		  {
			return isAuthorizationCheckEnabled && !PermissionChecksEmpty;
		  }
		  set
		  {
			this.shouldPerformAuthorizatioCheck_Conflict = value;
		  }
	  }


	  protected internal virtual bool PermissionChecksEmpty
	  {
		  get
		  {
			return permissionChecks.AtomicChecks.Count == 0 && permissionChecks.CompositeChecks.Count == 0;
		  }
	  }

	  public virtual string AuthUserId
	  {
		  get
		  {
			return authUserId;
		  }
		  set
		  {
			this.authUserId = value;
		  }
	  }


	  public virtual IList<string> AuthGroupIds
	  {
		  get
		  {
			return authGroupIds;
		  }
		  set
		  {
			this.authGroupIds = value;
		  }
	  }


	  public virtual int AuthDefaultPerm
	  {
		  get
		  {
			return authDefaultPerm;
		  }
		  set
		  {
			this.authDefaultPerm = value;
		  }
	  }


	  // authorization check parameters

	  public virtual CompositePermissionCheck PermissionChecks
	  {
		  get
		  {
			return permissionChecks;
		  }
		  set
		  {
			this.permissionChecks = value;
		  }
	  }

	  public virtual IList<PermissionCheck> AtomicPermissionChecks
	  {
		  set
		  {
			this.permissionChecks.AtomicChecks = value;
		  }
	  }

	  public virtual void addAtomicPermissionCheck(PermissionCheck permissionCheck)
	  {
		permissionChecks.addAtomicCheck(permissionCheck);
	  }


	  public virtual bool RevokeAuthorizationCheckEnabled
	  {
		  get
		  {
			return isRevokeAuthorizationCheckEnabled;
		  }
		  set
		  {
			this.isRevokeAuthorizationCheckEnabled = value;
		  }
	  }


	}

}