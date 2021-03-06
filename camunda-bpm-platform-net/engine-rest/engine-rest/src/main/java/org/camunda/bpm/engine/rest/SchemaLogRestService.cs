﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest
{


	using SchemaLogEntryDto = org.camunda.bpm.engine.rest.dto.SchemaLogEntryDto;
	using SchemaLogQueryDto = org.camunda.bpm.engine.rest.dto.SchemaLogQueryDto;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface SchemaLogRestService
	public interface SchemaLogRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.SchemaLogEntryDto> getSchemaLog(@Context Request request, @Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<SchemaLogEntryDto> getSchemaLog(Request request, UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.SchemaLogEntryDto> querySchemaLog(org.camunda.bpm.engine.rest.dto.SchemaLogQueryDto dto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<SchemaLogEntryDto> querySchemaLog(SchemaLogQueryDto dto, int? firstResult, int? maxResults);
	}

	public static class SchemaLogRestService_Fields
	{
	  public const string PATH = "/schema/log";
	}

}