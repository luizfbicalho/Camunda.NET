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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingUserTaskInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingUserTaskInstance;
	using MigratingInstanceParseContext = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;

	/// <summary>
	/// activity implementation for the user task.
	/// 
	/// @author Joram Barrez
	/// @author Roman Smirnov
	/// </summary>
	public class UserTaskActivityBehavior : TaskActivityBehavior, MigrationObserverBehavior
	{

	  protected internal TaskDecorator taskDecorator;

	  [Obsolete]
	  public UserTaskActivityBehavior(ExpressionManager expressionManager, TaskDefinition taskDefinition)
	  {
		this.taskDecorator = new TaskDecorator(taskDefinition, expressionManager);
	  }

	  public UserTaskActivityBehavior(TaskDecorator taskDecorator)
	  {
		this.taskDecorator = taskDecorator;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void performExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void performExecution(ActivityExecution execution)
	  {
		TaskEntity task = TaskEntity.createAndInsert(execution);

		taskDecorator.decorate(task, execution);

		Context.CommandContext.HistoricTaskInstanceManager.createHistoricTask(task);

		// All properties set, now firing 'create' event
		task.fireEvent(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		leave(execution);
	  }

	  // migration

	  public virtual void migrateScope(ActivityExecution scopeExecution)
	  {
	  }

	  public virtual void onParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		ExecutionEntity execution = migratingInstance.resolveRepresentativeExecution();

		foreach (TaskEntity task in execution.Tasks)
		{
		  migratingInstance.addMigratingDependentInstance(new MigratingUserTaskInstance(task, migratingInstance));
		  parseContext.consume(task);

		  ICollection<VariableInstanceEntity> variables = task.VariablesInternal;

		  if (variables != null)
		  {
			foreach (VariableInstanceEntity variable in variables)
			{
			  // we don't need to represent task variables in the migrating instance structure because
			  // they are migrated by the MigratingTaskInstance as well
			  parseContext.consume(variable);
			}
		  }
		}

	  }

	  // getters

	  public virtual TaskDefinition TaskDefinition
	  {
		  get
		  {
			return taskDecorator.TaskDefinition;
		  }
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return taskDecorator.ExpressionManager;
		  }
	  }

	  public virtual TaskDecorator TaskDecorator
	  {
		  get
		  {
			return taskDecorator;
		  }
	  }

	}

}