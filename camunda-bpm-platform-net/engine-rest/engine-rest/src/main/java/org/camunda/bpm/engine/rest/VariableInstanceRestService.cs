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
namespace org.camunda.bpm.engine.rest
{
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using VariableInstanceDto = org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceDto;
	using VariableInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceQueryDto;
	using VariableResource = org.camunda.bpm.engine.rest.sub.VariableResource;
	using VariableInstanceResource = org.camunda.bpm.engine.rest.sub.runtime.VariableInstanceResource;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;


	/// <summary>
	/// @author roman.smirnov
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface VariableInstanceRestService
	public interface VariableInstanceRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.runtime.VariableInstanceResource getVariableInstance(@PathParam("id") String id);
	  VariableInstanceResource getVariableInstance(string id);

	  /// <summary>
	  /// Exposes the <seealso cref="VariableInstanceQuery"/> interface as a REST service.
	  /// </summary>
	  /// <param name="uriInfo"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceDto> getVariableInstances(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeObjectValues);
	  IList<VariableInstanceDto> getVariableInstances(UriInfo uriInfo, int? firstResult, int? maxResults, bool deserializeObjectValues);

	  /// <summary>
	  /// Expects the same parameters as
	  /// <seealso cref="VariableInstanceRestService#getVariableInstances(UriInfo, Integer, Integer)"/> (as a JSON message body)
	  /// and allows for any number of variable checks.
	  /// </summary>
	  /// <param name="queryDto"> </param>
	  /// <param name="firstResult"> </param>
	  /// <param name="maxResults">
	  /// @return </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceDto> queryVariableInstances(org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceQueryDto queryDto, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults, @QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeObjectValues);
	  IList<VariableInstanceDto> queryVariableInstances(VariableInstanceQueryDto queryDto, int? firstResult, int? maxResults, bool deserializeObjectValues);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getVariableInstancesCount(@Context UriInfo uriInfo);
	  CountResultDto getVariableInstancesCount(UriInfo uriInfo);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/count") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto queryVariableInstancesCount(org.camunda.bpm.engine.rest.dto.runtime.VariableInstanceQueryDto queryDto);
	  CountResultDto queryVariableInstancesCount(VariableInstanceQueryDto queryDto);

	}

	public static class VariableInstanceRestService_Fields
	{
	  public const string PATH = "/variable-instance";
	}

}