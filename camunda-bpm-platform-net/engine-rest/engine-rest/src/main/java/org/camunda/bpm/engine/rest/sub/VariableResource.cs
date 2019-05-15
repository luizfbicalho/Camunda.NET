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
namespace org.camunda.bpm.engine.rest.sub
{


	using PatchVariablesDto = org.camunda.bpm.engine.rest.dto.PatchVariablesDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using MultipartFormData = org.camunda.bpm.engine.rest.mapper.MultipartFormData;

	public interface VariableResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.Map<String, org.camunda.bpm.engine.rest.dto.VariableValueDto> getVariables(@QueryParam(VariableResource_Fields.DESERIALIZE_VALUES_QUERY_PARAM) @DefaultValue("true") boolean deserializeValues);
	  IDictionary<string, VariableValueDto> getVariables(bool deserializeValues);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{varId}") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.VariableValueDto getVariable(@PathParam("varId") String variableName, @QueryParam(VariableResource_Fields.DESERIALIZE_VALUE_QUERY_PARAM) @DefaultValue("true") boolean deserializeValue);
	  VariableValueDto getVariable(string variableName, bool deserializeValue);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{varId}/data") public javax.ws.rs.core.Response getVariableBinary(@PathParam("varId") String variableName);
	  Response getVariableBinary(string variableName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PUT @Path("/{varId}") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void putVariable(@PathParam("varId") String variableName, org.camunda.bpm.engine.rest.dto.VariableValueDto variable);
	  void putVariable(string variableName, VariableValueDto variable);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/{varId}/data") @Consumes(javax.ws.rs.core.MediaType.MULTIPART_FORM_DATA) void setBinaryVariable(@PathParam("varId") String variableName, org.camunda.bpm.engine.rest.mapper.MultipartFormData multipartFormData);
	  void setBinaryVariable(string variableName, MultipartFormData multipartFormData);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE @Path("/{varId}") void deleteVariable(@PathParam("varId") String variableName);
	  void deleteVariable(string variableName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void modifyVariables(org.camunda.bpm.engine.rest.dto.PatchVariablesDto patch);
	  void modifyVariables(PatchVariablesDto patch);
	}

	public static class VariableResource_Fields
	{
	  public const string DESERIALIZE_VALUE_QUERY_PARAM = "deserializeValue";
	  public static readonly string DESERIALIZE_VALUES_QUERY_PARAM = DESERIALIZE_VALUE_QUERY_PARAM + "s";
	}

}