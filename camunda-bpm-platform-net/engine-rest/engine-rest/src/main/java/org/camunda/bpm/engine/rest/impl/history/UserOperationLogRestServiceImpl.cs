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
namespace org.camunda.bpm.engine.rest.impl.history
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using UserOperationLogEntryDto = org.camunda.bpm.engine.rest.dto.history.UserOperationLogEntryDto;
	using UserOperationLogQueryDto = org.camunda.bpm.engine.rest.dto.history.UserOperationLogQueryDto;
	using UserOperationLogRestService = org.camunda.bpm.engine.rest.history.UserOperationLogRestService;


	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogRestServiceImpl : UserOperationLogRestService
	{

	  protected internal ObjectMapper objectMapper;
	  protected internal ProcessEngine processEngine;

	  public UserOperationLogRestServiceImpl(ObjectMapper objectMapper, ProcessEngine processEngine)
	  {
		this.objectMapper = objectMapper;
		this.processEngine = processEngine;
	  }

	  public virtual CountResultDto queryUserOperationCount(UriInfo uriInfo)
	  {
		UserOperationLogQueryDto queryDto = new UserOperationLogQueryDto(objectMapper, uriInfo.QueryParameters);
		UserOperationLogQuery query = queryDto.toQuery(processEngine);
		return new CountResultDto(query.count());
	  }

	  public virtual IList<UserOperationLogEntryDto> queryUserOperationEntries(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		UserOperationLogQueryDto queryDto = new UserOperationLogQueryDto(objectMapper, uriInfo.QueryParameters);
		UserOperationLogQuery query = queryDto.toQuery(processEngine);

		if (firstResult == null && maxResults == null)
		{
		  return UserOperationLogEntryDto.map(query.list());
		}
		else
		{
		  if (firstResult == null)
		  {
			firstResult = 0;
		  }
		  if (maxResults == null)
		  {
			maxResults = int.MaxValue;
		  }
		  return UserOperationLogEntryDto.map(query.listPage(firstResult, maxResults));
		}
	  }
	}

}