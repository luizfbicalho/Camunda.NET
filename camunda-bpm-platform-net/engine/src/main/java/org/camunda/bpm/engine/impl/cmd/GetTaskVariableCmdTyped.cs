﻿using System;

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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class GetTaskVariableCmdTyped : Command<TypedValue>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string variableName;
	  protected internal bool isLocal;
	  protected internal bool deserializeValue;

	  public GetTaskVariableCmdTyped(string taskId, string variableName, bool isLocal, bool deserializeValue)
	  {
		this.taskId = taskId;
		this.variableName = variableName;
		this.isLocal = isLocal;
		this.deserializeValue = deserializeValue;
	  }

	  public virtual TypedValue execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);
		ensureNotNull("variableName", variableName);

		TaskEntity task = Context.CommandContext.TaskManager.findTaskById(taskId);

		ensureNotNull("task " + taskId + " doesn't exist", "task", task);

		checkGetTaskVariableTyped(task, commandContext);

		TypedValue value;

		if (isLocal)
		{
		  value = task.getVariableLocalTyped(variableName, deserializeValue);
		}
		else
		{
		  value = task.getVariableTyped(variableName, deserializeValue);
		}

		return value;
	  }

	  protected internal virtual void checkGetTaskVariableTyped(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadTaskVariable(task);
		}
	  }
	}

}