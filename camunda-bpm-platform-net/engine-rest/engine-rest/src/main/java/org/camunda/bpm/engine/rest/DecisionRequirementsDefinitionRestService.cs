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
	using DecisionRequirementsDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionDto;
	using DecisionRequirementsDefinitionResource = org.camunda.bpm.engine.rest.sub.repository.DecisionRequirementsDefinitionResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public interface DecisionRequirementsDefinitionRestService
	public interface DecisionRequirementsDefinitionRestService
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/{id}") org.camunda.bpm.engine.rest.sub.repository.DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionById(@PathParam("id") String decisionRequirementsDefinitionId);
	  DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionById(string decisionRequirementsDefinitionId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}") org.camunda.bpm.engine.rest.sub.repository.DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKey(@PathParam("key") String decisionRequirementsDefinitionKey);
	  DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKey(string decisionRequirementsDefinitionKey);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/key/{key}/tenant-id/{tenant-id}") org.camunda.bpm.engine.rest.sub.repository.DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKeyAndTenantId(@PathParam("key") String decisionRequirementsDefinitionKey, @PathParam("tenant-id") String tenantId);
	  DecisionRequirementsDefinitionResource getDecisionRequirementsDefinitionByKeyAndTenantId(string decisionRequirementsDefinitionKey, string tenantId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.repository.DecisionRequirementsDefinitionDto> getDecisionRequirementsDefinitions(@Context UriInfo uriInfo, @QueryParam("firstResult") System.Nullable<int> firstResult, @QueryParam("maxResults") System.Nullable<int> maxResults);
	  IList<DecisionRequirementsDefinitionDto> getDecisionRequirementsDefinitions(UriInfo uriInfo, int? firstResult, int? maxResults);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/count") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.CountResultDto getDecisionRequirementsDefinitionsCount(@Context UriInfo uriInfo);
	  CountResultDto getDecisionRequirementsDefinitionsCount(UriInfo uriInfo);

	}

	public static class DecisionRequirementsDefinitionRestService_Fields
	{
	  public const string PATH = "/decision-requirements-definition";
	}

}