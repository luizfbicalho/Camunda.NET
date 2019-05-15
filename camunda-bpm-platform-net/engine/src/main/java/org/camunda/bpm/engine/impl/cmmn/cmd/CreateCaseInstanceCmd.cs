using System;
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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureAtLeastOneNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CaseDefinitionNotFoundException = org.camunda.bpm.engine.exception.cmmn.CaseDefinitionNotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CreateCaseInstanceCmd : Command<CaseInstance>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionId;
	  protected internal IDictionary<string, object> variables;
	  protected internal string businessKey;

	  protected internal string caseDefinitionTenantId;
	  protected internal bool isTenantIdSet = false;

	  public CreateCaseInstanceCmd(CaseInstanceBuilderImpl builder)
	  {
		this.caseDefinitionKey = builder.CaseDefinitionKey;
		this.caseDefinitionId = builder.CaseDefinitionId;
		this.businessKey = builder.BusinessKey;
		this.variables = builder.getVariables();
		this.caseDefinitionTenantId = builder.CaseDefinitionTenantId;
		this.isTenantIdSet = builder.TenantIdSet;
	  }

	  public virtual CaseInstance execute(CommandContext commandContext)
	  {
		ensureAtLeastOneNotNull("caseDefinitionId and caseDefinitionKey are null", caseDefinitionId, caseDefinitionKey);

		CaseDefinitionEntity caseDefinition = find(commandContext);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateCaseInstance(caseDefinition);
		}

		// Start the case instance
		CaseExecutionEntity caseInstance = (CaseExecutionEntity) caseDefinition.createCaseInstance(businessKey);
		caseInstance.create(variables);
		return caseInstance;
	  }

	  protected internal virtual CaseDefinitionEntity find(CommandContext commandContext)
	  {
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

		// Find the case definition
		CaseDefinitionEntity caseDefinition = null;

		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  caseDefinition = findById(deploymentCache, caseDefinitionId);

		  ensureNotNull(typeof(CaseDefinitionNotFoundException), "No case definition found for id = '" + caseDefinitionId + "'", "caseDefinition", caseDefinition);

		}
		else
		{
		  caseDefinition = findByKey(deploymentCache, caseDefinitionKey);

		  ensureNotNull(typeof(CaseDefinitionNotFoundException), "No case definition found for key '" + caseDefinitionKey + "'", "caseDefinition", caseDefinition);
		}

		return caseDefinition;
	  }

	  protected internal virtual CaseDefinitionEntity findById(DeploymentCache deploymentCache, string caseDefinitionId)
	  {
		return deploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);
	  }

	  protected internal virtual CaseDefinitionEntity findByKey(DeploymentCache deploymentCache, string caseDefinitionKey)
	  {
		if (isTenantIdSet)
		{
		  return deploymentCache.findDeployedLatestCaseDefinitionByKeyAndTenantId(caseDefinitionKey, caseDefinitionTenantId);

		}
		else
		{
		  return deploymentCache.findDeployedLatestCaseDefinitionByKey(caseDefinitionKey);
		}
	  }

	}

}