using System.Collections.Generic;
using System.IO;

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


	using org.camunda.bpm.engine.repository;
	using CountResultDto = org.camunda.bpm.engine.rest.dto.CountResultDto;
	using DeploymentDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentDto;
	using DeploymentQueryDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentQueryDto;
	using DeploymentWithDefinitionsDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentWithDefinitionsDto;
	using ProcessDefinitionDto = org.camunda.bpm.engine.rest.dto.repository.ProcessDefinitionDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MultipartFormData = org.camunda.bpm.engine.rest.mapper.MultipartFormData;
	using FormPart = org.camunda.bpm.engine.rest.mapper.MultipartFormData.FormPart;
	using DeploymentResource = org.camunda.bpm.engine.rest.sub.repository.DeploymentResource;
	using DeploymentResourceImpl = org.camunda.bpm.engine.rest.sub.repository.impl.DeploymentResourceImpl;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DeploymentRestServiceImpl : AbstractRestProcessEngineAware, DeploymentRestService
	{

	  public const string DEPLOYMENT_NAME = "deployment-name";
	  public const string ENABLE_DUPLICATE_FILTERING = "enable-duplicate-filtering";
	  public const string DEPLOY_CHANGED_ONLY = "deploy-changed-only";
	  public const string DEPLOYMENT_SOURCE = "deployment-source";
	  public const string TENANT_ID = "tenant-id";

	  protected internal static readonly ISet<string> RESERVED_KEYWORDS = new HashSet<string>();

	  static DeploymentRestServiceImpl()
	  {
		RESERVED_KEYWORDS.Add(DEPLOYMENT_NAME);
		RESERVED_KEYWORDS.Add(ENABLE_DUPLICATE_FILTERING);
		RESERVED_KEYWORDS.Add(DEPLOY_CHANGED_ONLY);
		RESERVED_KEYWORDS.Add(DEPLOYMENT_SOURCE);
		RESERVED_KEYWORDS.Add(TENANT_ID);
	  }

		public DeploymentRestServiceImpl(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
		{
		}

	  public virtual DeploymentResource getDeployment(string deploymentId)
	  {
		return new DeploymentResourceImpl(ProcessEngine.Name, deploymentId, relativeRootResourcePath, ObjectMapper);
	  }

	  public virtual IList<DeploymentDto> getDeployments(UriInfo uriInfo, int? firstResult, int? maxResults)
	  {
		DeploymentQueryDto queryDto = new DeploymentQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		DeploymentQuery query = queryDto.toQuery(engine);

		IList<Deployment> matchingDeployments;
		if (firstResult != null || maxResults != null)
		{
		  matchingDeployments = executePaginatedQuery(query, firstResult, maxResults);
		}
		else
		{
		  matchingDeployments = query.list();
		}

		IList<DeploymentDto> deployments = new List<DeploymentDto>();
		foreach (Deployment deployment in matchingDeployments)
		{
		  DeploymentDto def = DeploymentDto.fromDeployment(deployment);
		  deployments.Add(def);
		}
		return deployments;
	  }

	  public virtual DeploymentWithDefinitionsDto createDeployment(UriInfo uriInfo, MultipartFormData payload)
	  {
		DeploymentBuilder deploymentBuilder = extractDeploymentInformation(payload);

		if (deploymentBuilder.ResourceNames.Count > 0)
		{
		  DeploymentWithDefinitions deployment = deploymentBuilder.deployWithResult();

		  DeploymentWithDefinitionsDto deploymentDto = DeploymentWithDefinitionsDto.fromDeployment(deployment);


		  URI uri = uriInfo.BaseUriBuilder.path(relativeRootResourcePath).path(org.camunda.bpm.engine.rest.DeploymentRestService_Fields.PATH).path(deployment.Id).build();

		  // GET
		  deploymentDto.addReflexiveLink(uri, HttpMethod.GET, "self");

		  return deploymentDto;

		}
		else
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "No deployment resources contained in the form upload.");
		}
	  }

	  private DeploymentBuilder extractDeploymentInformation(MultipartFormData payload)
	  {
		DeploymentBuilder deploymentBuilder = ProcessEngine.RepositoryService.createDeployment();

		ISet<string> partNames = payload.PartNames;

		foreach (string name in partNames)
		{
		  MultipartFormData.FormPart part = payload.getNamedPart(name);

		  if (!RESERVED_KEYWORDS.Contains(name))
		  {
			string fileName = part.FileName;
			if (!string.ReferenceEquals(fileName, null))
			{
			  deploymentBuilder.addInputStream(part.FileName, new MemoryStream(part.BinaryContent));
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "No file name found in the deployment resource described by form parameter '" + fileName + "'.");
			}
		  }
		}

		MultipartFormData.FormPart deploymentName = payload.getNamedPart(DEPLOYMENT_NAME);
		if (deploymentName != null)
		{
		  deploymentBuilder.name(deploymentName.TextContent);
		}

		MultipartFormData.FormPart deploymentSource = payload.getNamedPart(DEPLOYMENT_SOURCE);
		if (deploymentSource != null)
		{
		  deploymentBuilder.source(deploymentSource.TextContent);
		}

		MultipartFormData.FormPart deploymentTenantId = payload.getNamedPart(TENANT_ID);
		if (deploymentTenantId != null)
		{
		  deploymentBuilder.tenantId(deploymentTenantId.TextContent);
		}

		extractDuplicateFilteringForDeployment(payload, deploymentBuilder);
		return deploymentBuilder;
	  }

	  private void extractDuplicateFilteringForDeployment(MultipartFormData payload, DeploymentBuilder deploymentBuilder)
	  {
		bool enableDuplicateFiltering = false;
		bool deployChangedOnly = false;

		MultipartFormData.FormPart deploymentEnableDuplicateFiltering = payload.getNamedPart(ENABLE_DUPLICATE_FILTERING);
		if (deploymentEnableDuplicateFiltering != null)
		{
		  enableDuplicateFiltering = bool.Parse(deploymentEnableDuplicateFiltering.TextContent);
		}

		MultipartFormData.FormPart deploymentDeployChangedOnly = payload.getNamedPart(DEPLOY_CHANGED_ONLY);
		if (deploymentDeployChangedOnly != null)
		{
		  deployChangedOnly = bool.Parse(deploymentDeployChangedOnly.TextContent);
		}

		// deployChangedOnly overrides the enableDuplicateFiltering setting
		if (deployChangedOnly)
		{
		  deploymentBuilder.enableDuplicateFiltering(true);
		}
		else if (enableDuplicateFiltering)
		{
		  deploymentBuilder.enableDuplicateFiltering(false);
		}
	  }

	  private IList<Deployment> executePaginatedQuery(DeploymentQuery query, int? firstResult, int? maxResults)
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

	  public virtual CountResultDto getDeploymentsCount(UriInfo uriInfo)
	  {
		DeploymentQueryDto queryDto = new DeploymentQueryDto(ObjectMapper, uriInfo.QueryParameters);

		ProcessEngine engine = ProcessEngine;
		DeploymentQuery query = queryDto.toQuery(engine);

		long count = query.count();
		CountResultDto result = new CountResultDto();
		result.Count = count;
		return result;
	  }

	}

}