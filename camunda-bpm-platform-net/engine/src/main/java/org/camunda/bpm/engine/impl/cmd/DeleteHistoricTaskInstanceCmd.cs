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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricTaskInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DeleteHistoricTaskInstanceCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;

	  public DeleteHistoricTaskInstanceCmd(string taskId)
	  {
		this.taskId = taskId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);

		HistoricTaskInstanceEntity task = commandContext.HistoricTaskInstanceManager.findHistoricTaskInstanceById(taskId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteHistoricTaskInstance(task);
		}

		writeUserOperationLog(commandContext, task);

		commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstanceById(taskId);

		return null;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, HistoricTaskInstanceEntity historicTask)
	  {
		IList<PropertyChange> propertyChanges = new List<PropertyChange>();
		propertyChanges.Add(new PropertyChange("nrOfInstances", null, 1));
		propertyChanges.Add(new PropertyChange("async", null, false));

		commandContext.OperationLogManager.logTaskOperations(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, historicTask, propertyChanges);
	  }
	}

}