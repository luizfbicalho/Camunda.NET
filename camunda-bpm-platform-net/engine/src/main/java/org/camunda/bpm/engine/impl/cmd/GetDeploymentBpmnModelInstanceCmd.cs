using System;

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

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


	/// <summary>
	/// Gives access to a deploy BPMN model instance which can be accessed by
	/// the BPMN model API.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class GetDeploymentBpmnModelInstanceCmd : Command<BpmnModelInstance>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;

	  public GetDeploymentBpmnModelInstanceCmd(string processDefinitionId)
	  {
		if (string.ReferenceEquals(processDefinitionId, null) || processDefinitionId.Length < 1)
		{
		  throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId + "' has been provided.");
		}
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual BpmnModelInstance execute(CommandContext commandContext)
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache deploymentCache = configuration.getDeploymentCache();
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessDefinition(processDefinition);
		}

		BpmnModelInstance modelInstance = deploymentCache.findBpmnModelInstanceForProcessDefinition(processDefinitionId);

		ensureNotNull("no BPMN model instance found for process definition id " + processDefinitionId, "modelInstance", modelInstance);
		return modelInstance;
	  }
	}

}