using System;
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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using DeploymentResourceNotFoundException = org.camunda.bpm.engine.exception.DeploymentResourceNotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetDeploymentResourceCmd : Command<Stream>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string deploymentId;
	  protected internal string resourceName;

	  public GetDeploymentResourceCmd(string deploymentId, string resourceName)
	  {
		this.deploymentId = deploymentId;
		this.resourceName = resourceName;
	  }

	  public virtual Stream execute(CommandContext commandContext)
	  {
		ensureNotNull("deploymentId", deploymentId);
		ensureNotNull("resourceName", resourceName);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadDeployment(deploymentId);
		}

		ResourceEntity resource = commandContext.ResourceManager.findResourceByDeploymentIdAndResourceName(deploymentId, resourceName);
		ensureNotNull(typeof(DeploymentResourceNotFoundException), "no resource found with name '" + resourceName + "' in deployment '" + deploymentId + "'", "resource", resource);
		return new MemoryStream(resource.Bytes);
	  }

	}

}