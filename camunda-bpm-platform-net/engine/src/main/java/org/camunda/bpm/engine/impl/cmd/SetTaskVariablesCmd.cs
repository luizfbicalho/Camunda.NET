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
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SetTaskVariablesCmd : AbstractSetVariableCmd
	{

	  private const long serialVersionUID = 1L;

	  public SetTaskVariablesCmd<T1>(string taskId, IDictionary<T1> variables, bool isLocal) where T1 : object : base(taskId, variables, isLocal)
	  {
	  }

	  protected internal override TaskEntity Entity
	  {
		  get
		  {
			ensureNotNull("taskId", entityId);
    
			TaskEntity task = commandContext.TaskManager.findTaskById(entityId);
    
			ensureNotNull("task " + entityId + " doesn't exist", "task", task);
    
			checkSetTaskVariables(task);
    
			return task;
		  }
	  }

	  protected internal override ExecutionEntity ContextExecution
	  {
		  get
		  {
			return Entity.getExecution();
		  }
	  }

	  protected internal override void logVariableOperation(AbstractVariableScope scope)
	  {
		TaskEntity task = (TaskEntity) scope;
		commandContext.OperationLogManager.logVariableOperation(LogEntryOperation, null, task.Id, PropertyChange.EMPTY_CHANGE);
	  }

	  protected internal virtual void checkSetTaskVariables(TaskEntity task)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateTaskVariable(task);
		}
	  }
	}

}