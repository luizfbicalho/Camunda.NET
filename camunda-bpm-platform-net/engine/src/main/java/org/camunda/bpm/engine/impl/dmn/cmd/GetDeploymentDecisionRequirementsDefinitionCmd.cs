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
namespace org.camunda.bpm.engine.impl.dmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;

	/// <summary>
	/// Gives access to a deployed decision requirements definition instance.
	/// </summary>
	[Serializable]
	public class GetDeploymentDecisionRequirementsDefinitionCmd : Command<DecisionRequirementsDefinition>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string decisionRequirementsDefinitionId;

	  public GetDeploymentDecisionRequirementsDefinitionCmd(string decisionRequirementsDefinitionId)
	  {
		this.decisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
	  }

	  public virtual DecisionRequirementsDefinition execute(CommandContext commandContext)
	  {
		ensureNotNull("decisionRequirementsDefinitionId", decisionRequirementsDefinitionId);
		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
		DecisionRequirementsDefinitionEntity decisionRequirementsDefinition = deploymentCache.findDeployedDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadDecisionRequirementsDefinition(decisionRequirementsDefinition);
		}

		return decisionRequirementsDefinition;
	  }

	}

}