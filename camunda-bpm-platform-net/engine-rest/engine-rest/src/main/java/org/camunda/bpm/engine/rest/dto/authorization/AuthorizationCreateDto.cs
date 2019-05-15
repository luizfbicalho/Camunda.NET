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
namespace org.camunda.bpm.engine.rest.dto.authorization
{
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using PermissionConverter = org.camunda.bpm.engine.impl.util.PermissionConverter;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationCreateDto
	{

	  protected internal int? type;
	  protected internal string[] permissions;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal int? resourceType;
	  protected internal string resourceId;

	  // transformers ///////////////////////////////////////

	  public static void update(AuthorizationCreateDto dto, Authorization dbAuthorization, ProcessEngineConfiguration engineConfiguration)
	  {

		dbAuthorization.GroupId = dto.GroupId;
		dbAuthorization.UserId = dto.UserId;
		dbAuthorization.ResourceType = dto.ResourceType.Value;
		dbAuthorization.ResourceId = dto.ResourceId;
		dbAuthorization.Permissions = PermissionConverter.getPermissionsForNames(dto.Permissions, dto.ResourceType.Value, engineConfiguration);

	  }

	  //////////////////////////////////////////////////////

	  public virtual int Type
	  {
		  get
		  {
			return type.Value;
		  }
		  set
		  {
			this.type = value;
		  }
	  }
	  public virtual string[] Permissions
	  {
		  get
		  {
			return permissions;
		  }
		  set
		  {
			this.permissions = value;
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
	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
		  set
		  {
			this.groupId = value;
		  }
	  }
	  public virtual int? ResourceType
	  {
		  get
		  {
			return resourceType;
		  }
		  set
		  {
			this.resourceType = value;
		  }
	  }
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
	}

}