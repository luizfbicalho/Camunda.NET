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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DelegateTaskCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal string userId;

	  public DelegateTaskCmd(string taskId, string userId)
	  {
		this.taskId = taskId;
		this.userId = userId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);

		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);
		ensureNotNull("Cannot find task with id " + taskId, "task", task);

		checkDelegateTask(task, commandContext);

		task.@delegate(userId);

		task.createHistoricTaskDetails(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELEGATE);

		return null;
	  }

	  protected internal virtual void checkDelegateTask(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkTaskAssign(task);
		}
	  }
	}

}