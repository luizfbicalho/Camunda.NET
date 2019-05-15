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
namespace org.camunda.bpm.engine.rest.sub.runtime
{
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using ResourceOptionsDto = org.camunda.bpm.engine.rest.dto.ResourceOptionsDto;
	using FilterDto = org.camunda.bpm.engine.rest.dto.runtime.FilterDto;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface FilterResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.FilterDto getFilter(@QueryParam("itemCount") System.Nullable<bool> itemCount);
	  FilterDto getFilter(bool? itemCount);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE void deleteFilter();
	  void deleteFilter();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void updateFilter(org.camunda.bpm.engine.rest.dto.runtime.FilterDto filterDto);
	  void updateFilter(FilterDto filterDto);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/singleResult") @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) Object executeSingleResult(@Context Request request);
	  object executeSingleResult(Request request);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/singleResult") @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) Object querySingleResult(@Context Request request, String extendingQuery);
	  object querySingleResult(Request request, string extendingQuery);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/list") @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) Object executeList(@Context Request request, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  object executeList(Request request, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/list") @Produces({javax.ws.rs.core.MediaType.APPLICATION_JSON, org.camunda.bpm.engine.rest.hal.Hal.APPLICATION_HAL_JSON}) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) Object queryList(@Context Request request, String extendingQuery, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  object queryList(Request request, string extendingQuery, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto executeCount();
	  CountResultDto executeCount();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryCount(String extendingQuery);
	  CountResultDto queryCount(string extendingQuery);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @OPTIONS @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.ResourceOptionsDto availableOperations(@Context UriInfo context);
	  ResourceOptionsDto availableOperations(UriInfo context);


	}

}