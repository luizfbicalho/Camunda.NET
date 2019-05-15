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
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

	public class GetDeployedProcessDefinitionCmd : Command<ProcessDefinitionEntity>
	{

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

	  protected internal string processDefinitionTenantId;
	  protected internal bool isTenantIdSet = false;

	  protected internal readonly bool checkReadPermission;

	  public GetDeployedProcessDefinitionCmd(string processDefinitionId, bool checkReadPermission)
	  {
		this.processDefinitionId = processDefinitionId;
		this.checkReadPermission = checkReadPermission;
	  }

	  public GetDeployedProcessDefinitionCmd(ProcessInstantiationBuilderImpl instantiationBuilder, bool checkReadPermission)
	  {
		this.processDefinitionId = instantiationBuilder.ProcessDefinitionId;
		this.processDefinitionKey = instantiationBuilder.ProcessDefinitionKey;
		this.processDefinitionTenantId = instantiationBuilder.ProcessDefinitionTenantId;
		this.isTenantIdSet = instantiationBuilder.ProcessDefinitionTenantIdSet;
		this.checkReadPermission = checkReadPermission;
	  }

	  public virtual ProcessDefinitionEntity execute(CommandContext commandContext)
	  {

		ensureOnlyOneNotNull("either process definition id or key must be set", processDefinitionId, processDefinitionKey);

		ProcessDefinitionEntity processDefinition = find(commandContext);

		if (checkReadPermission)
		{
		  foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		  {
			checker.checkReadProcessDefinition(processDefinition);
		  }
		}

		return processDefinition;
	  }

	  protected internal virtual ProcessDefinitionEntity find(CommandContext commandContext)
	  {
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  return findById(deploymentCache, processDefinitionId);

		}
		else
		{
		  return findByKey(deploymentCache, processDefinitionKey);
		}
	  }

	  protected internal virtual ProcessDefinitionEntity findById(DeploymentCache deploymentCache, string processDefinitionId)
	  {
		return deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
	  }

	  protected internal virtual ProcessDefinitionEntity findByKey(DeploymentCache deploymentCache, string processDefinitionKey)
	  {
		if (isTenantIdSet)
		{
		  return deploymentCache.findDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, processDefinitionTenantId);

		}
		else
		{
		  return deploymentCache.findDeployedLatestProcessDefinitionByKey(processDefinitionKey);
		}
	  }

	}

}