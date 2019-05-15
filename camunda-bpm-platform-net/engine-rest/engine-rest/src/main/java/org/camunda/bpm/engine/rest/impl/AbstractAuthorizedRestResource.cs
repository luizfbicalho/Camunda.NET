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
namespace org.camunda.bpm.engine.rest.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractAuthorizedRestResource : AbstractRestProcessEngineAware
	{

	  protected internal readonly Resource resource;
	  protected internal readonly string resourceId;

	  public AbstractAuthorizedRestResource(string processEngineName, Resource resource, string resourceId, ObjectMapper objectMapper) : base(processEngineName, objectMapper)
	  {
		this.resource = resource;
		this.resourceId = resourceId;
	  }

	  protected internal virtual bool isAuthorized(Permission permission, Resource resource, string resourceId)
	  {
		if (!processEngine.ProcessEngineConfiguration.AuthorizationEnabled)
		{
		  // if authorization is disabled everyone is authorized
		  return true;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = processEngine.getIdentityService();
		IdentityService identityService = processEngine.IdentityService;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.AuthorizationService authorizationService = processEngine.getAuthorizationService();
		AuthorizationService authorizationService = processEngine.AuthorizationService;

		Authentication authentication = identityService.CurrentAuthentication;
		if (authentication == null)
		{
		  return true;

		}
		else
		{
		  return authorizationService.isUserAuthorized(authentication.UserId, authentication.GroupIds, permission, resource, resourceId);
		}
	  }

	  protected internal virtual bool isAuthorized(Permission permission, Resource resource)
	  {
		return isAuthorized(permission, resource, resourceId);
	  }

	  protected internal virtual bool isAuthorized(Permission permission)
	  {
		return isAuthorized(permission, resource);
	  }

	}

}