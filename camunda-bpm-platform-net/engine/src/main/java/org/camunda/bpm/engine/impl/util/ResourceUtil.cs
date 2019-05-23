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
namespace org.camunda.bpm.engine.impl.util
{

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class ResourceUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  /// <summary>
	  /// Parse a camunda:resource attribute and loads the resource depending on the url scheme.
	  /// Supported URL schemes are <code>classpath://</code> and <code>deployment://</code>.
	  /// If the scheme is omitted <code>classpath://</code> is assumed.
	  /// </summary>
	  /// <param name="resourcePath"> the path to the resource to load </param>
	  /// <param name="deployment"> the deployment to load resources from </param>
	  /// <returns> the resource content as <seealso cref="string"/> </returns>
	  public static string loadResourceContent(string resourcePath, DeploymentEntity deployment)
	  {
		string[] pathSplit = resourcePath.Split("://", 2);

		string resourceType;
		if (pathSplit.Length == 1)
		{
		  resourceType = "classpath";
		}
		else
		{
		  resourceType = pathSplit[0];
		}

		string resourceLocation = pathSplit[pathSplit.Length - 1];

		sbyte[] resourceBytes = null;

		if (resourceType.Equals("classpath"))
		{
		  Stream resourceAsStream = null;
		  try
		  {
			resourceAsStream = ReflectUtil.getResourceAsStream(resourceLocation);
			if (resourceAsStream != null)
			{
			  resourceBytes = IoUtil.readInputStream(resourceAsStream, resourcePath);
			}
		  }
		  finally
		  {
			IoUtil.closeSilently(resourceAsStream);
		  }
		}
		else if (resourceType.Equals("deployment"))
		{
		  ResourceEntity resourceEntity = deployment.getResource(resourceLocation);
		  if (resourceEntity != null)
		  {
			resourceBytes = resourceEntity.Bytes;
		  }
		}

		if (resourceBytes != null)
		{
		  return StringHelper.NewString(resourceBytes, Charset.forName("UTF-8"));
		}
		else
		{
		  throw LOG.cannotFindResource(resourcePath);
		}
	  }

	}

}