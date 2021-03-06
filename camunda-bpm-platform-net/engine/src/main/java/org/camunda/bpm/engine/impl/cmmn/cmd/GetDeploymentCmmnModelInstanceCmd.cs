﻿/*
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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CmmnModelInstanceNotFoundException = org.camunda.bpm.engine.exception.cmmn.CmmnModelInstanceNotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class GetDeploymentCmmnModelInstanceCmd : Command<CmmnModelInstance>
	{

	  protected internal string caseDefinitionId;

	  public GetDeploymentCmmnModelInstanceCmd(string caseDefinitionId)
	  {
		this.caseDefinitionId = caseDefinitionId;
	  }

	  public virtual CmmnModelInstance execute(CommandContext commandContext)
	  {
		ensureNotNull("caseDefinitionId", caseDefinitionId);

		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache deploymentCache = configuration.getDeploymentCache();
		DeploymentCache deploymentCache = configuration.DeploymentCache;

		CaseDefinitionEntity caseDefinition = deploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadCaseDefinition(caseDefinition);
		}

		CmmnModelInstance modelInstance = Context.ProcessEngineConfiguration.DeploymentCache.findCmmnModelInstanceForCaseDefinition(caseDefinitionId);

		ensureNotNull(typeof(CmmnModelInstanceNotFoundException), "No CMMN model instance found for case definition id " + caseDefinitionId, "modelInstance", modelInstance);
		return modelInstance;
	  }

	}

}