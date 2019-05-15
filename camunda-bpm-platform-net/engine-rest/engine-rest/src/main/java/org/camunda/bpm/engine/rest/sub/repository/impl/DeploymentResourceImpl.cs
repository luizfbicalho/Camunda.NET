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
namespace org.camunda.bpm.engine.rest.sub.repository.impl
{


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using DeploymentDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentDto;
	using DeploymentWithDefinitionsDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentWithDefinitionsDto;
	using RedeploymentDto = org.camunda.bpm.engine.rest.dto.repository.RedeploymentDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AbstractRestProcessEngineAware = org.camunda.bpm.engine.rest.impl.AbstractRestProcessEngineAware;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DeploymentResourceImpl : AbstractRestProcessEngineAware, DeploymentResource
	{

	  protected internal string deploymentId;

	  public DeploymentResourceImpl(string processEngineName, string deploymentId, string rootResourcePath, ObjectMapper objectMapper) : base(processEngineName, objectMapper)
	  {
		this.deploymentId = deploymentId;
		this.relativeRootResourcePath = rootResourcePath;
	  }

	  public virtual DeploymentDto Deployment
	  {
		  get
		  {
			RepositoryService repositoryService = ProcessEngine.RepositoryService;
			Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult();
    
			if (deployment == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Deployment with id '" + deploymentId + "' does not exist");
			}
    
			return DeploymentDto.fromDeployment(deployment);
		  }
	  }

	  public virtual DeploymentResourcesResource DeploymentResources
	  {
		  get
		  {
			return new DeploymentResourcesResourceImpl(ProcessEngine, deploymentId);
		  }
	  }

	  public virtual DeploymentDto redeploy(UriInfo uriInfo, RedeploymentDto redeployment)
	  {
		DeploymentWithDefinitions deployment = null;
		try
		{
		  deployment = tryToRedeploy(redeployment);

		}
		catch (NotFoundException e)
		{
		  throw createInvalidRequestException("redeploy", Status.NOT_FOUND, e);

		}
		catch (NotValidException e)
		{
		  throw createInvalidRequestException("redeploy", Status.BAD_REQUEST, e);
		}

		DeploymentWithDefinitionsDto deploymentDto = DeploymentWithDefinitionsDto.fromDeployment(deployment);

		URI uri = uriInfo.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH).path(deployment.Id).build();

		// GET /
		deploymentDto.addReflexiveLink(uri, HttpMethod.GET, "self");

		return deploymentDto;
	  }

	  protected internal virtual DeploymentWithDefinitions tryToRedeploy(RedeploymentDto redeployment)
	  {
		RepositoryService repositoryService = ProcessEngine.RepositoryService;

		DeploymentBuilder builder = repositoryService.createDeployment();
		builder.nameFromDeployment(deploymentId);

		string tenantId = Deployment.TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  builder.tenantId(tenantId);
		}

		if (redeployment != null)
		{
		  builder = addRedeploymentResources(builder, redeployment);
		}
		else
		{
		  builder.addDeploymentResources(deploymentId);
		}

		return builder.deployWithResult();
	  }

	  protected internal virtual DeploymentBuilder addRedeploymentResources(DeploymentBuilder builder, RedeploymentDto redeployment)
	  {
		builder.source(redeployment.Source);

		IList<string> resourceIds = redeployment.ResourceIds;
		IList<string> resourceNames = redeployment.ResourceNames;

		bool isResourceIdListEmpty = resourceIds == null || resourceIds.Count == 0;
		bool isResourceNameListEmpty = resourceNames == null || resourceNames.Count == 0;

		if (isResourceIdListEmpty && isResourceNameListEmpty)
		{
		  builder.addDeploymentResources(deploymentId);

		}
		else
		{
		  if (!isResourceIdListEmpty)
		  {
			builder.addDeploymentResourcesById(deploymentId, resourceIds);
		  }
		  if (!isResourceNameListEmpty)
		  {
			builder.addDeploymentResourcesByName(deploymentId, resourceNames);
		  }
		}
		return builder;
	  }

	  public virtual void deleteDeployment(string deploymentId, UriInfo uriInfo)
	  {
		RepositoryService repositoryService = ProcessEngine.RepositoryService;
		Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(deploymentId).singleResult();

		if (deployment == null)
		{
		  throw new InvalidRequestException(Status.NOT_FOUND, "Deployment with id '" + deploymentId + "' do not exist");
		}

		bool cascade = isQueryPropertyEnabled(uriInfo, org.camunda.bpm.engine.rest.sub.repository.DeploymentResource_Fields.CASCADE);
		bool skipCustomListeners = isQueryPropertyEnabled(uriInfo, "skipCustomListeners");
		bool skipIoMappings = isQueryPropertyEnabled(uriInfo, "skipIoMappings");

		repositoryService.deleteDeployment(deploymentId, cascade, skipCustomListeners, skipIoMappings);
	  }

	  protected internal virtual bool isQueryPropertyEnabled(UriInfo uriInfo, string property)
	  {
		MultivaluedMap<string, string> queryParams = uriInfo.QueryParameters;

		return queryParams.containsKey(property) && queryParams.get(property).size() > 0 && "true".Equals(queryParams.get(property).get(0));
	  }

	  protected internal virtual InvalidRequestException createInvalidRequestException(string action, Status status, ProcessEngineException cause)
	  {
		string errorMessage = string.Format("Cannot {0} deployment '{1}': {2}", action, deploymentId, cause.Message);
		return new InvalidRequestException(status, cause, errorMessage);
	  }

	}

}