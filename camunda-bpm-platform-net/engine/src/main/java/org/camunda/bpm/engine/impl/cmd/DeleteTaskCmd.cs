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

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeleteTaskCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal ICollection<string> taskIds;
	  protected internal bool cascade;
	  protected internal string deleteReason;

	  public DeleteTaskCmd(string taskId, string deleteReason, bool cascade)
	  {
		this.taskId = taskId;
		this.cascade = cascade;
		this.deleteReason = deleteReason;
	  }

	  public DeleteTaskCmd(ICollection<string> taskIds, string deleteReason, bool cascade)
	  {
		this.taskIds = taskIds;
		this.cascade = cascade;
		this.deleteReason = deleteReason;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (!string.ReferenceEquals(taskId, null))
		{
		  deleteTask(taskId, commandContext);
		}
		else if (taskIds != null)
		{
			foreach (string taskId in taskIds)
			{
			  deleteTask(taskId, commandContext);
			}
		}
		else
		{
		  throw new ProcessEngineException("taskId and taskIds are null");
		}


		return null;
	  }

	  protected internal virtual void deleteTask(string taskId, CommandContext commandContext)
	  {
		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);

		if (task != null)
		{
		  if (!string.ReferenceEquals(task.ExecutionId, null))
		  {
			throw new ProcessEngineException("The task cannot be deleted because is part of a running process");
		  }
		  else if (!string.ReferenceEquals(task.CaseExecutionId, null))
		  {
			throw new ProcessEngineException("The task cannot be deleted because is part of a running case instance");
		  }

		  checkDeleteTask(task, commandContext);

		  string reason = (string.ReferenceEquals(deleteReason, null) || deleteReason.Length == 0) ? TaskEntity.DELETE_REASON_DELETED : deleteReason;
		  task.delete(reason, cascade);
		}
		else if (cascade)
		{
		  Context.CommandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstanceById(taskId);
		}
	  }

	  protected internal virtual void checkDeleteTask(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteTask(task);
		}
	  }
	}

}