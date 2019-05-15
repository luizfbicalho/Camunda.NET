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
namespace org.camunda.bpm.engine.impl.cmd
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureGreaterThanOrEqual;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	[Serializable]
	public class UpdateDecisionDefinitionHistoryTimeToLiveCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string decisionDefinitionId;
	  protected internal int? historyTimeToLive;

	  public UpdateDecisionDefinitionHistoryTimeToLiveCmd(string decisionDefinitionId, int? historyTimeToLive)
	  {
		this.decisionDefinitionId = decisionDefinitionId;
		this.historyTimeToLive = historyTimeToLive;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		checkAuthorization(commandContext);

		ensureNotNull(typeof(BadUserRequestException), "decisionDefinitionId", decisionDefinitionId);
		if (historyTimeToLive != null)
		{
		  ensureGreaterThanOrEqual(typeof(BadUserRequestException), "", "historyTimeToLive", historyTimeToLive, 0);
		}

		DecisionDefinitionEntity decisionDefinitionEntity = commandContext.DecisionDefinitionManager.findDecisionDefinitionById(decisionDefinitionId);
		logUserOperation(commandContext, decisionDefinitionEntity);
		decisionDefinitionEntity.HistoryTimeToLive = historyTimeToLive;

		return null;
	  }

	  protected internal virtual void checkAuthorization(CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
			checker.checkUpdateDecisionDefinitionById(decisionDefinitionId);
		}
	  }

	  protected internal virtual void logUserOperation(CommandContext commandContext, DecisionDefinitionEntity decisionDefinitionEntity)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("historyTimeToLive", decisionDefinitionEntity.HistoryTimeToLive, historyTimeToLive));
		propertyChanges.Add(new PropertyChange("decisionDefinitionId", null, decisionDefinitionId));
		propertyChanges.Add(new PropertyChange("decisionDefinitionKey", null, decisionDefinitionEntity.Key));

		commandContext.OperationLogManager.logDecisionDefinitionOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE_HISTORY_TIME_TO_LIVE, propertyChanges);
	  }
	}

}