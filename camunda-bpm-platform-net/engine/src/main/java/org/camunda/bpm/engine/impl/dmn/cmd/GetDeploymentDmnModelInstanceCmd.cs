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

	using DmnModelInstanceNotFoundException = org.camunda.bpm.engine.exception.dmn.DmnModelInstanceNotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;

	/// <summary>
	/// Gives access to a deployed DMN model instance which can be accessed by the
	/// DMN model API.
	/// </summary>
	public class GetDeploymentDmnModelInstanceCmd : Command<DmnModelInstance>
	{

	  protected internal string decisionDefinitionId;

	  public GetDeploymentDmnModelInstanceCmd(string decisionDefinitionId)
	  {
		this.decisionDefinitionId = decisionDefinitionId;
	  }

	  public virtual DmnModelInstance execute(CommandContext commandContext)
	  {
		ensureNotNull("decisionDefinitionId", decisionDefinitionId);

		DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;

		DecisionDefinitionEntity decisionDefinition = deploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadDecisionDefinition(decisionDefinition);
		}

		DmnModelInstance modelInstance = deploymentCache.findDmnModelInstanceForDecisionDefinition(decisionDefinitionId);

		ensureNotNull(typeof(DmnModelInstanceNotFoundException), "No DMN model instance found for decision definition id " + decisionDefinitionId, "modelInstance", modelInstance);
		return modelInstance;
	  }

	}

}