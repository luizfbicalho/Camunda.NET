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

	using FormField = org.camunda.bpm.engine.form.FormField;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class GetTaskFormVariablesCmd : AbstractGetFormVariablesCmd
	{

	  private const long serialVersionUID = 1L;

	  public GetTaskFormVariablesCmd(string taskId, ICollection<string> variableNames, bool deserializeObjectValues) : base(taskId, variableNames, deserializeObjectValues)
	  {
	  }

	  public override VariableMap execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.TaskManager taskManager = commandContext.getTaskManager();
		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(resourceId);

		ensureNotNull(typeof(BadUserRequestException), "Cannot find task with id '" + resourceId + "'.", "task", task);

		checkGetTaskFormVariables(task, commandContext);

		VariableMapImpl result = new VariableMapImpl();

		// first, evaluate form fields
		TaskDefinition taskDefinition = task.TaskDefinition;
		if (taskDefinition != null)
		{
		  TaskFormData taskFormData = taskDefinition.TaskFormHandler.createTaskForm(task);
		  foreach (FormField formField in taskFormData.FormFields)
		  {
			if (formVariableNames == null || formVariableNames.Contains(formField.Id))
			{
			  result.put(formField.Id, createVariable(formField, task));
			}
		  }
		}

		// collect remaining variables from task scope and parent scopes
		task.collectVariables(result, formVariableNames, false, deserializeObjectValues);

		return result;
	  }

	  protected internal virtual void checkGetTaskFormVariables(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadTaskVariable(task);
		}
	  }
	}

}