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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class DeleteHistoricCaseInstanceCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string caseInstanceId;

	  public DeleteHistoricCaseInstanceCmd(string caseInstanceId)
	  {
		this.caseInstanceId = caseInstanceId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("caseInstanceId", caseInstanceId);
		// Check if case instance is still running
		HistoricCaseInstance instance = commandContext.HistoricCaseInstanceManager.findHistoricCaseInstance(caseInstanceId);

		ensureNotNull("No historic case instance found with id: " + caseInstanceId, "instance", instance);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricCaseInstance(instance);
		}

		ensureNotNull("Case instance is still running, cannot delete historic case instance: " + caseInstanceId, "instance.getCloseTime()", instance.CloseTime);

		commandContext.OperationLogManager.logCaseInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, caseInstanceId, Collections.singletonList(PropertyChange.EMPTY_CHANGE));

		commandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstancesByIds(Arrays.asList(caseInstanceId));

		return null;
	  }

	}

}