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
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// Deletes historic decision instances with the given id of the instance.
	/// 
	/// @author Johannes Heinemann
	/// </summary>
	public class DeleteHistoricDecisionInstanceByInstanceIdCmd : Command<object>
	{

	  protected internal readonly string historicDecisionInstanceId;

	  public DeleteHistoricDecisionInstanceByInstanceIdCmd(string historicDecisionInstanceId)
	  {
		this.historicDecisionInstanceId = historicDecisionInstanceId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("historicDecisionInstanceId", historicDecisionInstanceId);

		HistoricDecisionInstance historicDecisionInstance = commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstance(historicDecisionInstanceId);
		ensureNotNull("No historic decision instance found with id: " + historicDecisionInstanceId, "historicDecisionInstance", historicDecisionInstance);
		writeUserOperationLog(commandContext);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricDecisionInstance(historicDecisionInstance);
		}

		commandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionInstanceByIds(Arrays.asList(historicDecisionInstanceId));

		return null;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, 1));
		propertyChanges.Add(new PropertyChange("async", null, false));

		commandContext.OperationLogManager.logDecisionInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, propertyChanges);
	  }
	}

}