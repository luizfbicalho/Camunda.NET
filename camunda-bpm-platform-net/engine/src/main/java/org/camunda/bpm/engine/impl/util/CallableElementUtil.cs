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
namespace org.camunda.bpm.engine.impl.util
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CallableElementUtil
	{

	  public static DeploymentCache DeploymentCache
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.DeploymentCache;
		  }
	  }

	  public static ProcessDefinitionImpl getProcessDefinitionToCall(VariableScope execution, BaseCallableElement callableElement)
	  {
		string processDefinitionKey = callableElement.getDefinitionKey(execution);
		string tenantId = callableElement.getDefinitionTenantId(execution);

		DeploymentCache deploymentCache = DeploymentCache;

		ProcessDefinitionImpl processDefinition = null;

		if (callableElement.LatestBinding)
		{
		  processDefinition = deploymentCache.findDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, tenantId);

		}
		else if (callableElement.DeploymentBinding)
		{
		  string deploymentId = callableElement.DeploymentId;
		  processDefinition = deploymentCache.findDeployedProcessDefinitionByDeploymentAndKey(deploymentId, processDefinitionKey);

		}
		else if (callableElement.VersionBinding)
		{
		  int? version = callableElement.getVersion(execution);
		  processDefinition = deploymentCache.findDeployedProcessDefinitionByKeyVersionAndTenantId(processDefinitionKey, version, tenantId);

		}
		else if (callableElement.VersionTagBinding)
		{
		  string versionTag = callableElement.getVersionTag(execution);
		  processDefinition = deploymentCache.findDeployedProcessDefinitionByKeyVersionTagAndTenantId(processDefinitionKey, versionTag, tenantId);

		}

		return processDefinition;
	  }

	  public static CmmnCaseDefinition getCaseDefinitionToCall(VariableScope execution, BaseCallableElement callableElement)
	  {
		string caseDefinitionKey = callableElement.getDefinitionKey(execution);
		string tenantId = callableElement.getDefinitionTenantId(execution);

		DeploymentCache deploymentCache = DeploymentCache;

		CmmnCaseDefinition caseDefinition = null;
		if (callableElement.LatestBinding)
		{
		  caseDefinition = deploymentCache.findDeployedLatestCaseDefinitionByKeyAndTenantId(caseDefinitionKey, tenantId);

		}
		else if (callableElement.DeploymentBinding)
		{
		  string deploymentId = callableElement.DeploymentId;
		  caseDefinition = deploymentCache.findDeployedCaseDefinitionByDeploymentAndKey(deploymentId, caseDefinitionKey);

		}
		else if (callableElement.VersionBinding)
		{
		  int? version = callableElement.getVersion(execution);
		  caseDefinition = deploymentCache.findDeployedCaseDefinitionByKeyVersionAndTenantId(caseDefinitionKey, version, tenantId);
		}

		return caseDefinition;
	  }

	  public static DecisionDefinition getDecisionDefinitionToCall(VariableScope execution, BaseCallableElement callableElement)
	  {
		string decisionDefinitionKey = callableElement.getDefinitionKey(execution);
		string tenantId = callableElement.getDefinitionTenantId(execution);

		DeploymentCache deploymentCache = DeploymentCache;

		DecisionDefinition decisionDefinition = null;
		if (callableElement.LatestBinding)
		{
		  decisionDefinition = deploymentCache.findDeployedLatestDecisionDefinitionByKeyAndTenantId(decisionDefinitionKey, tenantId);

		}
		else if (callableElement.DeploymentBinding)
		{
		  string deploymentId = callableElement.DeploymentId;
		  decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByDeploymentAndKey(deploymentId, decisionDefinitionKey);

		}
		else if (callableElement.VersionBinding)
		{
		  int? version = callableElement.getVersion(execution);
		  decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByKeyVersionAndTenantId(decisionDefinitionKey, version, tenantId);

		}
		else if (callableElement.VersionTagBinding)
		{
		  string versionTag = callableElement.getVersionTag(execution);
		  decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByKeyVersionTagAndTenantId(decisionDefinitionKey, versionTag, tenantId);
		}

		return decisionDefinition;
	  }

	}

}