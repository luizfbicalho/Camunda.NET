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

	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using FormEngine = org.camunda.bpm.engine.impl.form.engine.FormEngine;
	using TaskFormHandler = org.camunda.bpm.engine.impl.form.handler.TaskFormHandler;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GetRenderedTaskFormCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string formEngineName;

	  public GetRenderedTaskFormCmd(string taskId, string formEngineName)
	  {
		this.taskId = taskId;
		this.formEngineName = formEngineName;
	  }


	  public virtual object execute(CommandContext commandContext)
	  {
		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);
		ensureNotNull("Task '" + taskId + "' not found", "task", task);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadTaskVariable(task);
		}
		ensureNotNull("Task form definition for '" + taskId + "' not found", "task.getTaskDefinition()", task.TaskDefinition);

		TaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
		if (taskFormHandler == null)
		{
		  return null;
		}

		FormEngine formEngine = Context.ProcessEngineConfiguration.FormEngines[formEngineName];

		ensureNotNull("No formEngine '" + formEngineName + "' defined process engine configuration", "formEngine", formEngine);

		TaskFormData taskForm = taskFormHandler.createTaskForm(task);

		return formEngine.renderTaskForm(taskForm);
	  }
	}

}