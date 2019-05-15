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
namespace org.camunda.bpm.engine.rest.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using GroupDto = org.camunda.bpm.engine.rest.dto.identity.GroupDto;
	using GroupQueryDto = org.camunda.bpm.engine.rest.dto.identity.GroupQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using GroupResource = org.camunda.bpm.engine.rest.sub.identity.GroupResource;
	using GroupResourceImpl = org.camunda.bpm.engine.rest.sub.identity.impl.GroupResourceImpl;
	using PathUtil = org.camunda.bpm.engine.rest.util.PathUtil;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.GROUP;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class GroupRestServiceImpl : AbstractAuthorizedRestResource, GroupRestService
	{

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public GroupRestServiceImpl(String engineName, final com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public GroupRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, GROUP, ANY, objectMapper)
	  {
	  }

	  public virtual GroupResource getGroup(string id)
	  {
		id = PathUtil.decodePathParam(id);
		return new GroupResourceImpl(ProcessEngine.Name, id, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<GroupDto> queryGroups(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		GroupQueryDto queryDto = new GroupQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return queryGroups(queryDto, firstResult, maxResults);
	  }

	  public virtual IList<GroupDto> queryGroups(GroupQueryDto queryDto, int? firstResult, int? maxResults)
	  {

		queryDto.ObjectMapper = ObjectMapper;
		GroupQuery query = queryDto.toQuery(ProcessEngine);

		IList<Group> resultList;
		if (firstResult != null || maxResults != null)
		{
		  resultList = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  resultList = query.list();
		}

		return GroupDto.fromGroupList(resultList);
	  }

	  public virtual CountResultDto getGroupCount(UriInfo uriInfo)
	  {
		GroupQueryDto queryDto = new GroupQueryDto(ObjectMapper, uriInfo.QueryParameters);
		return getGroupCount(queryDto);
	  }

	  protected internal virtual CountResultDto getGroupCount(GroupQueryDto queryDto)
	  {
		GroupQuery query = queryDto.toQuery(ProcessEngine);
		long count = query.count();
		return new CountResultDto(count);
	  }

	  public virtual void createGroup(GroupDto groupDto)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = getIdentityService();
		IdentityService identityService = IdentityService;

		if (identityService.ReadOnly)
		{
		  throw new InvalidRequestException(Status.FORBIDDEN, "Identity service implementation is read-only.");
		}

		Group newGroup = identityService.newGroup(groupDto.Id);
		groupDto.update(newGroup);
		identityService.saveGroup(newGroup);

	  }

	  public virtual ResourceOptionsDto availableOperations(UriInfo context)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.IdentityService identityService = getIdentityService();
		IdentityService identityService = IdentityService;

		UriBuilder baseUriBuilder = context.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.GroupRestService_Fields.PATH);

		ResourceOptionsDto resourceOptionsDto = new ResourceOptionsDto();

		// GET /
		URI baseUri = baseUriBuilder.build();
		resourceOptionsDto.addReflexiveLink(baseUri, HttpMethod.GET, "list");

		// GET /count
		URI countUri = baseUriBuilder.clone().path("/count").build();
		resourceOptionsDto.addReflexiveLink(countUri, HttpMethod.GET, "count");

		// POST /create
		if (!identityService.ReadOnly && isAuthorized(CREATE))
		{
		  URI createUri = baseUriBuilder.clone().path("/create").build();
		  resourceOptionsDto.addReflexiveLink(createUri, HttpMethod.POST, "create");
		}

		return resourceOptionsDto;
	  }

	  // utility methods //////////////////////////////////////

	  protected internal virtual IList<Group> executePaginatedQuery(GroupQuery query, int? firstResult, int? maxResults)
	  {
		if (firstResult == null)
		{
		  firstResult = 0;
		}
		if (maxResults == null)
		{
		  maxResults = int.MaxValue;
		}
		return query.listPage(firstResult, maxResults);
	  }

	  protected internal virtual IdentityService IdentityService
	  {
		  get
		  {
			return ProcessEngine.IdentityService;
		  }
	  }

	}

}