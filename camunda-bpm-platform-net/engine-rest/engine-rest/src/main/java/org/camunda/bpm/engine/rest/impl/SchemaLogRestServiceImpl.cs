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


	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;
	using SchemaLogEntryDto = org.camunda.bpm.engine.rest.dto.SchemaLogEntryDto;
	using SchemaLogQueryDto = org.camunda.bpm.engine.rest.dto.SchemaLogQueryDto;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogRestServiceImpl : AbstractRestProcessEngineAware, SchemaLogRestService
	{

	  public SchemaLogRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

	  public virtual IList<SchemaLogEntryDto> getSchemaLog(Request request, UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		return querySchemaLog(new SchemaLogQueryDto(ObjectMapper, uriInfo.QueryParameters), firstResult, maxResults);
	  }

	  public virtual IList<SchemaLogEntryDto> querySchemaLog(SchemaLogQueryDto dto, int? firstResult, int? maxResults)
	  {
		SchemaLogQuery query = dto.toQuery(processEngine);
		IList<SchemaLogEntry> schemaLogEntries;
		if (firstResult != null || maxResults != null)
		{
		  schemaLogEntries = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  schemaLogEntries = query.list();
		}
		return SchemaLogEntryDto.fromSchemaLogEntries(schemaLogEntries);
	  }

	  protected internal virtual IList<SchemaLogEntry> executePaginatedQuery(SchemaLogQuery query, int? firstResult, int? maxResults)
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
	}
}