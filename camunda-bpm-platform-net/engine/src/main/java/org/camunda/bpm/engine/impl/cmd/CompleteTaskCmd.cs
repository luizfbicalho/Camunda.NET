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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionVariableSnapshotObserver = org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class CompleteTaskCmd : Command<VariableMap>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal IDictionary<string, object> variables;

	  // only fetch variables if they are actually requested;
	  // this avoids unnecessary loading of variables
	  protected internal bool returnVariables;
	  protected internal bool deserializeReturnedVariables;

	  public CompleteTaskCmd(string taskId, IDictionary<string, object> variables) : this(taskId, variables, false, false)
	  {
	  }

	  public CompleteTaskCmd(string taskId, IDictionary<string, object> variables, bool returnVariables, bool deserializeReturnedVariables)
	  {
		this.taskId = taskId;
		this.variables = variables;
		this.returnVariables = returnVariables;
		this.deserializeReturnedVariables = deserializeReturnedVariables;
	  }

	  public virtual VariableMap execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);

		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);
		ensureNotNull("Cannot find task with id " + taskId, "task", task);

		checkCompleteTask(task, commandContext);

		if (variables != null)
		{
		  task.ExecutionVariables = variables;
		}

		ExecutionEntity execution = task.ProcessInstance;
		ExecutionVariableSnapshotObserver variablesListener = null;

		if (returnVariables && execution != null)
		{
		  variablesListener = new ExecutionVariableSnapshotObserver(execution, false, deserializeReturnedVariables);
		}

		completeTask(task);

		if (returnVariables)
		{
		  if (variablesListener != null)
		  {
			return variablesListener.Variables;
		  }
		  else
		  {
			return !string.ReferenceEquals(task.CaseDefinitionId, null) ? null : task.getVariablesTyped(false);
		  }
		}
		else
		{
		  return null;
		}

	  }

	  protected internal virtual void completeTask(TaskEntity task)
	  {
		task.complete();
	  }

	  protected internal virtual void checkCompleteTask(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkTaskWork(task);
		}
	  }
	}

}