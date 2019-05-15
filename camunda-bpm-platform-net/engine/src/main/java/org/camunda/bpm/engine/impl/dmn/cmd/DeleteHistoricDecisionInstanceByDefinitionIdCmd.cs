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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// Deletes historic decision instances with the given id of the decision definition.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class DeleteHistoricDecisionInstanceByDefinitionIdCmd : Command<object>
	{

	  protected internal readonly string decisionDefinitionId;

	  public DeleteHistoricDecisionInstanceByDefinitionIdCmd(string decisionDefinitionId)
	  {
		this.decisionDefinitionId = decisionDefinitionId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("decisionDefinitionId", decisionDefinitionId);

		DecisionDefinitionEntity decisionDefinition = commandContext.DecisionDefinitionManager.findDecisionDefinitionById(decisionDefinitionId);
		ensureNotNull("No decision definition found with id: " + decisionDefinitionId, "decisionDefinition", decisionDefinition);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricDecisionInstance(decisionDefinition.Key);
		}

		long numInstances = getDecisionInstanceCount(commandContext);
		writeUserOperationLog(commandContext, numInstances);

		commandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionInstancesByDecisionDefinitionId(decisionDefinitionId);

		return null;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, long numInstances)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
		propertyChanges.Add(new PropertyChange("async", null, false));

		commandContext.OperationLogManager.logDecisionInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, propertyChanges);
	  }

	  protected internal virtual long getDecisionInstanceCount(CommandContext commandContext)
	  {
		HistoricDecisionInstanceQueryImpl historicDecisionInstanceQuery = new HistoricDecisionInstanceQueryImpl();
		historicDecisionInstanceQuery.decisionDefinitionId(decisionDefinitionId);

		return commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstanceCountByQueryCriteria(historicDecisionInstanceQuery);
	  }
	}

}