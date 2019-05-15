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
namespace org.camunda.bpm.engine.impl.dmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil.evaluateDecision;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureOnlyOneNotNull;


	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// Evaluates the decision with the given key or id.
	/// 
	/// If the decision definition key given then specify the version and tenant-id.
	/// If no version is provided then the latest version is taken.
	/// </summary>
	public class EvaluateDecisionCmd : Command<DmnDecisionResult>
	{

	  protected internal string decisionDefinitionKey;
	  protected internal string decisionDefinitionId;
	  protected internal int? version;
	  protected internal VariableMap variables;
	  protected internal string decisionDefinitionTenantId;
	  protected internal bool isTenandIdSet;

	  public EvaluateDecisionCmd(DecisionEvaluationBuilderImpl builder)
	  {
		this.decisionDefinitionKey = builder.DecisionDefinitionKey;
		this.decisionDefinitionId = builder.DecisionDefinitionId;
		this.version = builder.Version;
		this.variables = Variables.fromMap(builder.Variables);
		this.decisionDefinitionTenantId = builder.DecisionDefinitionTenantId;
		this.isTenandIdSet = builder.TenantIdSet;
	  }

	  public virtual DmnDecisionResult execute(CommandContext commandContext)
	  {
		ensureOnlyOneNotNull("either decision definition id or key must be set", decisionDefinitionId, decisionDefinitionKey);

		DecisionDefinition decisionDefinition = getDecisionDefinition(commandContext);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkEvaluateDecision(decisionDefinition);
		}

		writeUserOperationLog(commandContext, decisionDefinition);

		return doEvaluateDecision(decisionDefinition, variables);

	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, DecisionDefinition decisionDefinition)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("decisionDefinitionId", null, decisionDefinition.Id));
		propertyChanges.Add(new PropertyChange("decisionDefinitionKey", null, decisionDefinition.Key));
		commandContext.OperationLogManager.logDecisionDefinitionOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EVALUATE, propertyChanges);
	  }

	  protected internal virtual DmnDecisionResult doEvaluateDecision(DecisionDefinition decisionDefinition, VariableMap variables)
	  {
		try
		{
		  return evaluateDecision(decisionDefinition, variables);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Exception while evaluating decision with key '" + decisionDefinitionKey + "'", e);
		}
	  }

	  protected internal virtual DecisionDefinition getDecisionDefinition(CommandContext commandContext)
	  {
		DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

		if (!string.ReferenceEquals(decisionDefinitionId, null))
		{
		  return findById(deploymentCache);
		}
		else
		{
		  return findByKey(deploymentCache);
		}
	  }

	  protected internal virtual DecisionDefinition findById(DeploymentCache deploymentCache)
	  {
		return deploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);
	  }

	  protected internal virtual DecisionDefinition findByKey(DeploymentCache deploymentCache)
	  {
		DecisionDefinition decisionDefinition = null;

		if (version == null && !isTenandIdSet)
		{
		  decisionDefinition = deploymentCache.findDeployedLatestDecisionDefinitionByKey(decisionDefinitionKey);
		}
		else if (version == null && isTenandIdSet)
		{
		  decisionDefinition = deploymentCache.findDeployedLatestDecisionDefinitionByKeyAndTenantId(decisionDefinitionKey, decisionDefinitionTenantId);
		}
		else if (version != null && !isTenandIdSet)
		{
		  decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByKeyAndVersion(decisionDefinitionKey, version);
		}
		else if (version != null && isTenandIdSet)
		{
		  decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByKeyVersionAndTenantId(decisionDefinitionKey, version, decisionDefinitionTenantId);
		}

		return decisionDefinition;
	  }

	}

}