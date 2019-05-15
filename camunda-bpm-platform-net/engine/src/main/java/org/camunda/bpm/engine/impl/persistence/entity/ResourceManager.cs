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
namespace org.camunda.bpm.engine.impl.persistence.entity
{



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ResourceManager : AbstractManager
	{

	  public virtual void insertResource(ResourceEntity resource)
	  {
		DbEntityManager.insert(resource);
	  }

	  public virtual void deleteResourcesByDeploymentId(string deploymentId)
	  {
		DbEntityManager.delete(typeof(ResourceEntity), "deleteResourcesByDeploymentId", deploymentId);
	  }

	  public virtual ResourceEntity findResourceByDeploymentIdAndResourceName(string deploymentId, string resourceName)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentId"] = deploymentId;
		@params["resourceName"] = resourceName;
		return (ResourceEntity) DbEntityManager.selectOne("selectResourceByDeploymentIdAndResourceName", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourceByDeploymentIdAndResourceNames(String deploymentId, String... resourceNames)
	  public virtual IList<ResourceEntity> findResourceByDeploymentIdAndResourceNames(string deploymentId, params string[] resourceNames)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentId"] = deploymentId;
		@params["resourceNames"] = resourceNames;
		return DbEntityManager.selectList("selectResourceByDeploymentIdAndResourceNames", @params);
	  }

	  public virtual ResourceEntity findResourceByDeploymentIdAndResourceId(string deploymentId, string resourceId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentId"] = deploymentId;
		@params["resourceId"] = resourceId;
		return (ResourceEntity) DbEntityManager.selectOne("selectResourceByDeploymentIdAndResourceId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourceByDeploymentIdAndResourceIds(String deploymentId, String... resourceIds)
	  public virtual IList<ResourceEntity> findResourceByDeploymentIdAndResourceIds(string deploymentId, params string[] resourceIds)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentId"] = deploymentId;
		@params["resourceIds"] = resourceIds;
		return DbEntityManager.selectList("selectResourceByDeploymentIdAndResourceIds", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourcesByDeploymentId(String deploymentId)
	  public virtual IList<ResourceEntity> findResourcesByDeploymentId(string deploymentId)
	  {
		return DbEntityManager.selectList("selectResourcesByDeploymentId", deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, ResourceEntity> findLatestResourcesByDeploymentName(String deploymentName, java.util.Set<String> resourcesToFind, String source, String tenantId)
	  public virtual IDictionary<string, ResourceEntity> findLatestResourcesByDeploymentName(string deploymentName, ISet<string> resourcesToFind, string source, string tenantId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentName"] = deploymentName;
		@params["resourcesToFind"] = resourcesToFind;
		@params["source"] = source;
		@params["tenantId"] = tenantId;

		IList<ResourceEntity> resources = DbEntityManager.selectList("selectLatestResourcesByDeploymentName", @params);

		IDictionary<string, ResourceEntity> existingResourcesByName = new Dictionary<string, ResourceEntity>();
		foreach (ResourceEntity existingResource in resources)
		{
		  existingResourcesByName[existingResource.Name] = existingResource;
		}

		return existingResourcesByName;
	  }

	}

}