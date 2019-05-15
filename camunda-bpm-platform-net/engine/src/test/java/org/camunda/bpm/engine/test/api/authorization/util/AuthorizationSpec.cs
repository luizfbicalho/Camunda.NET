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
namespace org.camunda.bpm.engine.test.api.authorization.util
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationSpec
	{
	  protected internal int type;
	  protected internal Resource resource;
	  protected internal string resourceId;
	  protected internal string userId;
	  protected internal Permission[] permissions;

	  public static AuthorizationSpec auth(int type, Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		AuthorizationSpec spec = new AuthorizationSpec();
		spec.type = type;
		spec.resource = resource;
		spec.resourceId = resourceId;
		spec.userId = userId;
		spec.permissions = permissions;
		return spec;
	  }

	  public static AuthorizationSpec global(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		return auth(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL, resource, resourceId, userId, permissions);
	  }

	  public static AuthorizationSpec grant(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		return auth(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT, resource, resourceId, userId, permissions);
	  }

	  public static AuthorizationSpec revoke(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		return auth(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE, resource, resourceId, userId, permissions);
	  }

	  public virtual Authorization instantiate(AuthorizationService authorizationService, IDictionary<string, string> replacements)
	  {
		Authorization authorization = authorizationService.createNewAuthorization(type);

		// TODO: group id is missing
		authorization.Resource = resource;

		if (replacements.ContainsKey(resourceId))
		{
		  authorization.ResourceId = replacements[resourceId];
		}
		else
		{
		  authorization.ResourceId = resourceId;
		}
		authorization.UserId = userId;
		authorization.Permissions = permissions;

		return authorization;
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("[Resource: ");
		sb.Append(resource);
		sb.Append(", Resource Id: ");
		sb.Append(resourceId);
		sb.Append(", Type: ");
		sb.Append(type);
		sb.Append(", User Id: ");
		sb.Append(userId);
		sb.Append(", Permissions: [");

		foreach (Permission permission in permissions)
		{
		  sb.Append(permission.Name);
		  sb.Append(", ");
		}

		sb.Append("]]");

		return sb.ToString();

	  }
	}

}