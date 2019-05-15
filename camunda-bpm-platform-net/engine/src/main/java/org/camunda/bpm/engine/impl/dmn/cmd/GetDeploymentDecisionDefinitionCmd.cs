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
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	/// <summary>
	/// Gives access to a deployed decision definition instance.
	/// </summary>
	[Serializable]
	public class GetDeploymentDecisionDefinitionCmd : Command<DecisionDefinition>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string decisionDefinitionId;

	  public GetDeploymentDecisionDefinitionCmd(string decisionDefinitionId)
	  {
		this.decisionDefinitionId = decisionDefinitionId;
	  }

	  public virtual DecisionDefinition execute(CommandContext commandContext)
	  {
		ensureNotNull("decisionDefinitionId", decisionDefinitionId);
		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
		DecisionDefinitionEntity decisionDefinition = deploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadDecisionDefinition(decisionDefinition);
		}

		return decisionDefinition;
	  }

	}

}