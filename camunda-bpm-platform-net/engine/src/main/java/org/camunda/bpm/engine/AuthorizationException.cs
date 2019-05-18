using System;
using System.Collections.Generic;
using System.Text;

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

	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;

	/// <summary>
	/// <para>Exception thrown by the process engine in case a user tries to
	/// interact with a resource in an unauthorized way.</para>
	/// 
	/// <para>The exception contains a list of Missing authorizations. The List is a
	/// disjunction i.e. a user should have any of the authorization for the engine
	/// to continue the execution beyond the point where it failed.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationException : ProcessEngineException
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string userId;
	  protected internal readonly IList<MissingAuthorization> missingAuthorizations;

	  // these properties have been replaced by the list of missingAuthorizations
	  // and are only left because this is a public API package and users might
	  // have subclasses relying on these fields
	  [Obsolete]
	  protected internal string resourceType;
	  [Obsolete]
	  protected internal string permissionName;
	  [Obsolete]
	  protected internal string resourceId;

	  public AuthorizationException(string message) : base(message)
	  {
		this.userId = null;
		missingAuthorizations = new List<MissingAuthorization>();
	  }

	  public AuthorizationException(string userId, string permissionName, string resourceType, string resourceId) : this(userId, new MissingAuthorization(permissionName, resourceType, resourceId))
	  {
	  }

	  public AuthorizationException(string userId, MissingAuthorization exceptionInfo) : base("The user with id '" + userId + "' does not have " + generateMissingAuthorizationMessage(exceptionInfo) + ".")
	  {
		this.userId = userId;
		missingAuthorizations = new List<MissingAuthorization>();
		missingAuthorizations.Add(exceptionInfo);

		this.resourceType = exceptionInfo.ResourceType;
		this.permissionName = exceptionInfo.ViolatedPermissionName;
		this.resourceId = exceptionInfo.ResourceId;
	  }

	  public AuthorizationException(string userId, IList<MissingAuthorization> info) : base(generateExceptionMessage(userId, info))
	  {
		this.userId = userId;
		this.missingAuthorizations = info;
	  }

	  /// <returns> the type of the resource if there
	  /// is only one <seealso cref="MissingAuthorization"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="#getMissingAuthorizations()"/> to get the type of the resource
	  /// of the <seealso cref="MissingAuthorization"/>(s). This method may be removed in future versions. 
	  [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the type of the resource")]
	  public virtual string ResourceType
	  {
		  get
		  {
			string resourceType = null;
			if (missingAuthorizations.Count == 1)
			{
			  resourceType = missingAuthorizations[0].ResourceType;
			}
			return resourceType;
		  }
	  }

	  /// <returns> the type of the violated permission name if there
	  /// is only one <seealso cref="MissingAuthorization"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="#getMissingAuthorizations()"/> to get the violated permission name
	  /// of the <seealso cref="MissingAuthorization"/>(s). This method may be removed in future versions. 
	  [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the violated permission name")]
	  public virtual string ViolatedPermissionName
	  {
		  get
		  {
			if (missingAuthorizations.Count == 1)
			{
			  return missingAuthorizations[0].ViolatedPermissionName;
			}
			return null;
		  }
	  }

	  /// <returns> id of the user in which context the request was made and who misses authorizations
	  ///  to perform it successfully </returns>
	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

	  /// <returns> the id of the resource if there
	  /// is only one <seealso cref="MissingAuthorization"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="#getMissingAuthorizations()"/> to get the id of the resource
	  /// of the <seealso cref="MissingAuthorization"/>(s). This method may be removed in future versions. 
	  [Obsolete("Use <seealso cref=\"#getMissingAuthorizations()\"/> to get the id of the resource")]
	  public virtual string ResourceId
	  {
		  get
		  {
			if (missingAuthorizations.Count == 1)
			{
			  return missingAuthorizations[0].ResourceId;
			}
			return null;
		  }
	  }

	  /// <returns> Disjunctive list of <seealso cref="MissingAuthorization"/> from
	  /// which a user needs to have at least one for the authorization to pass </returns>
	  public virtual IList<MissingAuthorization> MissingAuthorizations
	  {
		  get
		  {
			return Collections.unmodifiableList(missingAuthorizations);
		  }
	  }

	  /// <summary>
	  /// Generate exception message from the missing authorizations.
	  /// </summary>
	  /// <param name="userId"> to use </param>
	  /// <param name="missingAuthorizations"> to use </param>
	  /// <returns> The prepared exception message </returns>
	  private static string generateExceptionMessage(string userId, IList<MissingAuthorization> missingAuthorizations)
	  {
		StringBuilder sBuilder = new StringBuilder();
		sBuilder.Append("The user with id '");
		sBuilder.Append(userId);
		sBuilder.Append("' does not have one of the following permissions: ");
		bool first = true;
		foreach (MissingAuthorization missingAuthorization in missingAuthorizations)
		{
		  if (!first)
		  {
			sBuilder.Append(" or ");
		  }
		  else
		  {
			first = false;
		  }
		  sBuilder.Append(generateMissingAuthorizationMessage(missingAuthorization));
		}
		return sBuilder.ToString();
	  }

	  /// <summary>
	  /// Generated exception message for the missing authorization.
	  /// </summary>
	  /// <param name="exceptionInfo"> to use </param>
	  private static string generateMissingAuthorizationMessage(MissingAuthorization exceptionInfo)
	  {
		StringBuilder builder = new StringBuilder();
		string permissionName = exceptionInfo.ViolatedPermissionName;
		string resourceType = exceptionInfo.ResourceType;
		string resourceId = exceptionInfo.ResourceId;
		builder.Append("'");
		builder.Append(permissionName);
		builder.Append("' permission on resource '");
		builder.Append((!string.ReferenceEquals(resourceId, null) ? (resourceId + "' of type '") : ""));
		builder.Append(resourceType);
		builder.Append("'");

		return builder.ToString();
	  }
	}

}