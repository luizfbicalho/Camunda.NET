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


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using TaskFormHandler = org.camunda.bpm.engine.impl.form.handler.TaskFormHandler;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionVariableSnapshotObserver = org.camunda.bpm.engine.impl.persistence.entity.ExecutionVariableSnapshotObserver;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SubmitTaskFormCmd : Command<VariableMap>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal VariableMap properties;

	  // only fetch variables if they are actually requested;
	  // this avoids unnecessary loading of variables
	  protected internal bool returnVariables;
	  protected internal bool deserializeValues;

	  public SubmitTaskFormCmd(string taskId, IDictionary<string, object> properties, bool returnVariables, bool deserializeValues)
	  {
		this.taskId = taskId;
		this.properties = Variables.fromMap(properties);
		this.returnVariables = returnVariables;
		this.deserializeValues = deserializeValues;
	  }

	  public virtual VariableMap execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);
		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);
		ensureNotNull("Cannot find task with id " + taskId, "task", task);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkTaskWork(task);
		}

		TaskDefinition taskDefinition = task.TaskDefinition;
		if (taskDefinition != null)
		{
		  TaskFormHandler taskFormHandler = taskDefinition.TaskFormHandler;
		  taskFormHandler.submitFormVariables(properties, task);
		}
		else
		{
		  // set variables on standalone task
		  task.Variables = properties;
		}

		ExecutionEntity execution = task.ProcessInstance;
		ExecutionVariableSnapshotObserver variablesListener = null;
		if (returnVariables && execution != null)
		{
		  variablesListener = new ExecutionVariableSnapshotObserver(execution, false, deserializeValues);
		}

		// complete or resolve the task
		if (DelegationState.PENDING.Equals(task.DelegationState))
		{
		  task.resolve();
		  task.createHistoricTaskDetails(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE);
		}
		else
		{
		  task.complete();
		  task.createHistoricTaskDetails(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_COMPLETE);
		}

		if (returnVariables)
		{
		  if (variablesListener != null)
		  {
			return variablesListener.Variables;
		  }
		  else
		  {
			return string.ReferenceEquals(task.CaseDefinitionId, null) ? null : task.getVariablesTyped(false);
		  }
		}
		else
		{
		  return null;
		}
	  }
	}

}