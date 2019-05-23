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
namespace org.camunda.bpm.engine.rest.dto
{

	/// <summary>
	/// <para>Dto for <seealso cref="AuthorizationException"/></para>
	/// 
	/// <para>The exception contains a list of Missing authorizations. The List is a
	/// disjunction i.e. a user should have any of the authorization for the engine
	/// to continue the execution beyond the point where it failed.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationExceptionDto : ExceptionDto
	{

	  protected internal string userId;
	  protected internal string resourceName;
	  protected internal string resourceId;
	  protected internal string permissionName;
	  protected internal IList<MissingAuthorizationDto> missingAuthorizations;

	  // transformer /////////////////////////////

	  public static AuthorizationExceptionDto fromException(AuthorizationException e)
	  {
		AuthorizationExceptionDto dto = new AuthorizationExceptionDto();

		dto.Message = e.Message;
		dto.Type = typeof(AuthorizationException).Name;

		dto.UserId = e.UserId;
		dto.MissingAuthorizations = MissingAuthorizationDto.fromInfo(e.MissingAuthorizations);
		dto.PermissionName = e.ViolatedPermissionName;
		dto.ResourceId = e.ResourceId;
		dto.ResourceName = e.ResourceType;

		return dto;
	  }

	  // getter / setters ////////////////////////
	  /// <returns> the name of the resource if there
	  /// is only one <seealso cref="MissingAuthorizationDto"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="getMissingAuthorizations()"/> to get the name of the resource
	  /// of the <seealso cref="MissingAuthorizationDto"/>(s). This method will be removed in future version. 
	  [Obsolete("Use <seealso cref=\"getMissingAuthorizations()\"/> to get the name of the resource")]
	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
		  set
		  {
			this.resourceName = value;
		  }
	  }


	  /// <returns> the id of the resource if there
	  /// is only one <seealso cref="MissingAuthorizationDto"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="getMissingAuthorizations()"/> to get the id of the resource
	  /// of the <seealso cref="MissingAuthorizationDto"/>(s). This method will be removed in future version. 
	  [Obsolete("Use <seealso cref=\"getMissingAuthorizations()\"/> to get the id of the resource")]
	  public virtual string ResourceId
	  {
		  get
		  {
			return resourceId;
		  }
		  set
		  {
			this.resourceId = value;
		  }
	  }


	  /// <returns> the name of the violated permission if there
	  /// is only one <seealso cref="MissingAuthorizationDto"/>, {@code null} otherwise
	  /// </returns>
	  /// @deprecated Use <seealso cref="getMissingAuthorizations()"/> to get the name of the violated permission
	  /// of the <seealso cref="MissingAuthorizationDto"/>(s). This method will be removed in future version. 
	  [Obsolete("Use <seealso cref=\"getMissingAuthorizations()\"/> to get the name of the violated permission")]
	  public virtual string PermissionName
	  {
		  get
		  {
			return permissionName;
		  }
		  set
		  {
			this.permissionName = value;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }
	  /// <returns> Disjunctive list of <seealso cref="MissingAuthorizationDto"/> from
	  /// which a user needs to have at least one for the authorization to pass </returns>
	  public virtual IList<MissingAuthorizationDto> MissingAuthorizations
	  {
		  get
		  {
			return missingAuthorizations;
		  }
		  set
		  {
			this.missingAuthorizations = value;
		  }
	  }
	}

}