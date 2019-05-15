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
namespace org.camunda.bpm.engine.rest.sub.repository.impl
{


	using Resource = org.camunda.bpm.engine.repository.Resource;
	using DeploymentResourceDto = org.camunda.bpm.engine.rest.dto.repository.DeploymentResourceDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DeploymentResourcesResourceImpl : DeploymentResourcesResource
	{

	  protected internal static readonly IDictionary<string, string> MEDIA_TYPE_MAPPING = new Dictionary<string, string>();

	  static DeploymentResourcesResourceImpl()
	  {
		MEDIA_TYPE_MAPPING["bpmn"] = MediaType.APPLICATION_XML;
		MEDIA_TYPE_MAPPING["cmmn"] = MediaType.APPLICATION_XML;
		MEDIA_TYPE_MAPPING["dmn"] = MediaType.APPLICATION_XML;
		MEDIA_TYPE_MAPPING["json"] = MediaType.APPLICATION_JSON;
		MEDIA_TYPE_MAPPING["xml"] = MediaType.APPLICATION_XML;

		MEDIA_TYPE_MAPPING["gif"] = "image/gif";
		MEDIA_TYPE_MAPPING["jpeg"] = "image/jpeg";
		MEDIA_TYPE_MAPPING["jpe"] = "image/jpeg";
		MEDIA_TYPE_MAPPING["jpg"] = "image/jpeg";
		MEDIA_TYPE_MAPPING["png"] = "image/png";
		MEDIA_TYPE_MAPPING["svg"] = "image/svg+xml";
		MEDIA_TYPE_MAPPING["tiff"] = "image/tiff";
		MEDIA_TYPE_MAPPING["tif"] = "image/tiff";

		MEDIA_TYPE_MAPPING["groovy"] = "text/plain";
		MEDIA_TYPE_MAPPING["java"] = "text/plain";
		MEDIA_TYPE_MAPPING["js"] = "text/plain";
		MEDIA_TYPE_MAPPING["php"] = "text/plain";
		MEDIA_TYPE_MAPPING["py"] = "text/plain";
		MEDIA_TYPE_MAPPING["rb"] = "text/plain";

		MEDIA_TYPE_MAPPING["html"] = "text/html";
		MEDIA_TYPE_MAPPING["txt"] = "text/plain";
	  }

	  protected internal readonly ProcessEngine engine;
	  protected internal readonly string deploymentId;

	  public DeploymentResourcesResourceImpl(ProcessEngine engine, string deploymentId)
	  {
		this.engine = engine;
		this.deploymentId = deploymentId;
	  }

	  public virtual IList<DeploymentResourceDto> DeploymentResources
	  {
		  get
		  {
			IList<Resource> resources = engine.RepositoryService.getDeploymentResources(deploymentId);
    
			IList<DeploymentResourceDto> deploymentResources = new List<DeploymentResourceDto>();
			foreach (Resource resource in resources)
			{
			  deploymentResources.Add(DeploymentResourceDto.fromResources(resource));
			}
    
			if (deploymentResources.Count > 0)
			{
			  return deploymentResources;
			}
			else
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, "Deployment resources for deployment id '" + deploymentId + "' do not exist.");
			}
		  }
	  }

	  public virtual DeploymentResourceDto getDeploymentResource(string resourceId)
	  {
		IList<DeploymentResourceDto> deploymentResources = DeploymentResources;
		foreach (DeploymentResourceDto deploymentResource in deploymentResources)
		{
		  if (deploymentResource.Id.Equals(resourceId))
		  {
			return deploymentResource;
		  }
		}

		throw new InvalidRequestException(Response.Status.NOT_FOUND, "Deployment resource with resource id '" + resourceId + "' for deployment id '" + deploymentId + "' does not exist.");
	  }

	  public virtual Response getDeploymentResourceData(string resourceId)
	  {
		RepositoryService repositoryService = engine.RepositoryService;
		Stream resourceAsStream = repositoryService.getResourceAsStreamById(deploymentId, resourceId);

		if (resourceAsStream != null)
		{

		  DeploymentResourceDto resource = getDeploymentResource(resourceId);

		  string name = resource.Name;

		  string filename = null;
		  string mediaType = null;

		  if (!string.ReferenceEquals(name, null))
		  {
			name = name.Replace("\\", "/");
			string[] filenameParts = name.Split("/", true);
			if (filenameParts.Length > 0)
			{
			  int idx = filenameParts.Length - 1;
			  filename = filenameParts[idx];
			}

			string[] extensionParts = name.Split("\\.", true);
			if (extensionParts.Length > 0)
			{
			  int idx = extensionParts.Length - 1;
			  string extension = extensionParts[idx];
			  if (!string.ReferenceEquals(extension, null))
			  {
				mediaType = MEDIA_TYPE_MAPPING[extension];
			  }
			}
		  }

		  if (string.ReferenceEquals(filename, null))
		  {
			filename = "data";
		  }

		  if (string.ReferenceEquals(mediaType, null))
		  {
			mediaType = MediaType.APPLICATION_OCTET_STREAM;
		  }

		  return Response.ok(resourceAsStream, mediaType).header("Content-Disposition", "attachment; filename=" + filename).build();
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, "Deployment resource '" + resourceId + "' for deployment id '" + deploymentId + "' does not exist.");
		}
	  }
	}

}