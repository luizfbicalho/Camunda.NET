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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SaveTaskCmd : Command<Void>
	{

		private const long serialVersionUID = 1L;

		protected internal TaskEntity task;

		public SaveTaskCmd(Task task)
		{
			this.task = (TaskEntity) task;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		ensureNotNull("task", task);

		string operation;

		if (task.Revision == 0)
		{

		  try
		  {
			checkCreateTask(task, commandContext);
			task.insert(null);
			commandContext.HistoricTaskInstanceManager.createHistoricTask(task);
			operation = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;
			task.executeMetrics(Metrics.ACTIVTY_INSTANCE_START);
		  }
		  catch (NullValueException e)
		  {
			throw new NotValidException(e.Message, e);
		  }


		}
		else
		{
		  checkTaskAssign(task, commandContext);
		  task.update();
		  operation = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;
		}

		task.fireAuthorizationProvider();
		task.fireEvents();
		task.createHistoricTaskDetails(operation);

		return null;
		}

	  protected internal virtual void checkTaskAssign(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkTaskAssign(task);
		}
	  }

	  protected internal virtual void checkCreateTask(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkCreateTask(task);
		}
	  }
	}

}