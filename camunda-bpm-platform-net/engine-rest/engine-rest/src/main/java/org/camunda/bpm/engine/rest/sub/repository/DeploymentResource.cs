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
namespace org.camunda.bpm.engine.rest.sub.repository
{
	using DeploymentDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentDto;
	using RedeploymentDto = org.camunda.bpm.engine.rest.dto.repository.RedeploymentDto;


	public interface DeploymentResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.DeploymentDto getDeployment();
	  DeploymentDto Deployment {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/resources") DeploymentResourcesResource getDeploymentResources();
	  DeploymentResourcesResource DeploymentResources {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/redeploy") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.DeploymentDto redeploy(@Context UriInfo uriInfo, org.camunda.bpm.engine.rest.dto.repository.RedeploymentDto redeployment);
	  DeploymentDto redeploy(UriInfo uriInfo, RedeploymentDto redeployment);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE void deleteDeployment(@PathParam("id") String deploymentId, @Context UriInfo uriInfo);
	  void deleteDeployment(string deploymentId, UriInfo uriInfo);
	}

	public static class DeploymentResource_Fields
	{
	  public const string CASCADE = "cascade";
	}

}