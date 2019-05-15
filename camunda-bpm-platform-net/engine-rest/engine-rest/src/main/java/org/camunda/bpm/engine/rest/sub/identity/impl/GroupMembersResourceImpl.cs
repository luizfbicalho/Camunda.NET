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
namespace org.camunda.bpm.engine.rest.sub.identity.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using PathUtil = org.camunda.bpm.engine.rest.util.PathUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class GroupMembersResourceImpl : AbstractIdentityResource, GroupMembersResource
	{

	  public GroupMembersResourceImpl(string processEngineName, string resourceId, string rootResourcePath, ObjectMapper objectMapper) : base(processEngineName, Resources.GROUP_MEMBERSHIP, resourceId, objectMapper)
	  {
		this.relativeRootResourcePath = rootResourcePath;
	  }

	  public virtual void createGroupMember(string userId)
	  {
		ensureNotReadOnly();

		userId = PathUtil.decodePathParam(userId);
		identityService.createMembership(userId, resourceId);
	  }

	  public virtual void deleteGroupMember(string userId)
	  {
		ensureNotReadOnly();

		userId = PathUtil.decodePathParam(userId);
		identityService.deleteMembership(userId, resourceId);
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		ResourceOptionsDto dto = new ResourceOptionsDto();

		URI uri = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH).path(resourceId).path(org.camunda.bpm.engine.rest.sub.identity.GroupMembersResource_Fields.PATH).build();

		dto.addReflexiveLink(uri, HttpMethod.GET, "self");

		if (!identityService.ReadOnly && isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.DELETE, "delete");
		}
		if (!identityService.ReadOnly && isAuthorized(CREATE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.PUT, "create");
		}

		return dto;

	  }

	}

}