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
namespace org.camunda.bpm.engine.rest.sub.repository
{


	using DeploymentResourceDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentResourceDto;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public interface DeploymentResourcesResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) java.util.List<org.camunda.bpm.engine.rest.dto.repository.DeploymentResourceDto> getDeploymentResources();
	  IList<DeploymentResourceDto> DeploymentResources {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{resourceId}") @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.repository.DeploymentResourceDto getDeploymentResource(@PathParam("resourceId") String resourceId);
	  DeploymentResourceDto getDeploymentResource(string resourceId);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/{resourceId}/data") javax.ws.rs.core.Response getDeploymentResourceData(@PathParam("resourceId") String resourceId);
	  Response getDeploymentResourceData(string resourceId);

	}

}