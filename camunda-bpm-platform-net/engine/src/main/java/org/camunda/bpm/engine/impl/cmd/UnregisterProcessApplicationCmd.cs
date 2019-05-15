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
namespace org.camunda.bpm.engine.impl.cmd
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UnregisterProcessApplicationCmd : Command<Void>
	{

	  protected internal bool removeProcessesFromCache;
	  protected internal ISet<string> deploymentIds;

	  public UnregisterProcessApplicationCmd(string deploymentId, bool removeProcessesFromCache) : this(Collections.singleton(deploymentId), removeProcessesFromCache)
	  {
	  }

	  public UnregisterProcessApplicationCmd(ISet<string> deploymentIds, bool removeProcessesFromCache)
	  {
		this.deploymentIds = deploymentIds;
		this.removeProcessesFromCache = removeProcessesFromCache;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		if (deploymentIds == null)
		{
		  throw new ProcessEngineException("Deployment Ids cannot be null.");
		}

		commandContext.AuthorizationManager.checkCamundaAdmin();

		Context.ProcessEngineConfiguration.ProcessApplicationManager.unregisterProcessApplicationForDeployments(deploymentIds, removeProcessesFromCache);

		return null;

	  }

	}

}