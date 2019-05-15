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

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class RegisterDeploymentCmd : Command<Void>
	{

	  protected internal string deploymentId;

	  public RegisterDeploymentCmd(string deploymentId)
	  {
		this.deploymentId = deploymentId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		Deployment deployment = commandContext.DeploymentManager.findDeploymentById(deploymentId);

		ensureNotNull("Deployment " + deploymentId + " does not exist", "deployment", deployment);

		commandContext.AuthorizationManager.checkCamundaAdmin();

		Context.ProcessEngineConfiguration.RegisteredDeployments.Add(deploymentId);
		return null;
	  }

	}

}