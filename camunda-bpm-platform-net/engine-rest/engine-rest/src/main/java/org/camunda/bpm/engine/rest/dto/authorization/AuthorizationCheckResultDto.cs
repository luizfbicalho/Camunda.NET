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
	using ResourceUtil = org.camunda.bpm.engine.rest.util.ResourceUtil;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class AuthorizationCheckResultDto
	{

	  protected internal string permissionName;
	  protected internal string resourceName;
	  protected internal string resourceId;

	  protected internal bool? isAuthorized;

	  public AuthorizationCheckResultDto()
	  {
	  }

	  public AuthorizationCheckResultDto(bool userAuthorized, string permissionName, ResourceUtil resource, string resourceId)
	  {
		isAuthorized = userAuthorized;
		this.permissionName = permissionName;
		resourceName = resource.resourceName();
		this.resourceId = resourceId;
	  }

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

	  public virtual bool? Authorized
	  {
		  get
		  {
			return isAuthorized;
		  }
		  set
		  {
			this.isAuthorized = value;
		  }
	  }


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