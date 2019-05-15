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
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class PermissionCheck
	{

	  /// <summary>
	  /// the permission to check for </summary>
	  protected internal Permission permission;
	  protected internal int perms;

	  /// <summary>
	  /// the type of the resource to check permissions for </summary>
	  protected internal Resource resource;
	  protected internal int resourceType;

	  /// <summary>
	  /// the id of the resource to check permission for </summary>
	  protected internal string resourceId;

	  /// <summary>
	  /// query parameter for resource Id. Is injected as RAW parameter into the query </summary>
	  protected internal string resourceIdQueryParam;

	  protected internal long? authorizationNotFoundReturnValue = null;

	  public PermissionCheck()
	  {
	  }

	  public virtual Permission Permission
	  {
		  get
		  {
			return permission;
		  }
		  set
		  {
			this.permission = value;
			if (value != null)
			{
			  perms = value.Value;
			}
		  }
	  }


	  public virtual int Perms
	  {
		  get
		  {
			return perms;
		  }
	  }

	  public virtual Resource Resource
	  {
		  get
		  {
			return resource;
		  }
		  set
		  {
			this.resource = value;
    
			if (value != null)
			{
			  resourceType = value.resourceType();
			}
		  }
	  }


	  public virtual int ResourceType
	  {
		  get
		  {
			return resourceType;
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


	  public virtual string ResourceIdQueryParam
	  {
		  get
		  {
			return resourceIdQueryParam;
		  }
		  set
		  {
			this.resourceIdQueryParam = value;
		  }
	  }


	  public virtual long? AuthorizationNotFoundReturnValue
	  {
		  get
		  {
			return authorizationNotFoundReturnValue;
		  }
		  set
		  {
			this.authorizationNotFoundReturnValue = value;
		  }
	  }

	}

}