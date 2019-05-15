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
	using Group = org.camunda.bpm.engine.identity.Group;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using GroupDto = org.camunda.bpm.engine.rest.dto.identity.GroupDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class GroupResourceImpl : AbstractIdentityResource, GroupResource
	{

	  private string rootResourcePath;

	  public GroupResourceImpl(string processEngineName, string groupId, string rootResourcePath, ObjectMapper objectMapper) : base(processEngineName, GROUP, groupId, objectMapper)
	  {
		this.rootResourcePath = rootResourcePath;
	  }

	  public virtual GroupDto getGroup(UriInfo context)
	  {

		Group dbGroup = findGroupObject();
		if (dbGroup == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Group with id " + resourceId + " does not exist");
		}

		GroupDto group = GroupDto.fromGroup(dbGroup);

		return group;
	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

		ResourceOptionsDto dto = new ResourceOptionsDto();

		// add links if operations are authorized
		URI uri = context.BaseUriBuilder.path(rootResourcePath).path(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH).path(resourceId).build();

		dto.addReflexiveLink(uri, HttpMethod.GET, "self");
		if (!identityService.ReadOnly && isAuthorized(DELETE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.DELETE, "delete");
		}
		if (!identityService.ReadOnly && isAuthorized(UPDATE))
		{
		  dto.addReflexiveLink(uri, HttpMethod.PUT, "update");
		}

		return dto;
	  }


	  public virtual void updateGroup(GroupDto group)
	  {
		ensureNotReadOnly();

		Group dbGroup = findGroupObject();
		if (dbGroup == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Group with id " + resourceId + " does not exist");
		}

		group.update(dbGroup);

		identityService.saveGroup(dbGroup);
	  }


	  public virtual void deleteGroup()
	  {
		ensureNotReadOnly();
		identityService.deleteGroup(resourceId);
	  }

	  public virtual GroupMembersResource GroupMembersResource
	  {
		  get
		  {
			return new GroupMembersResourceImpl(processEngine.Name, resourceId, rootResourcePath, ObjectMapper);
		  }
	  }

	  protected internal virtual Group findGroupObject()
	  {
		try
		{
		  return identityService.createGroupQuery().groupId(resourceId).singleResult();
		}
		catch (ProcessEngineException e)
		{
		  throw new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, "Exception while performing group query: " + e.Message);
		}
	  }


	}

}