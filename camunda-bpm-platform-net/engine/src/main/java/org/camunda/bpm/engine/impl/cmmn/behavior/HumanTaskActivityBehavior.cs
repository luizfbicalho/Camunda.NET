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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HumanTaskActivityBehavior : TaskActivityBehavior
	{

	  protected internal TaskDecorator taskDecorator;

	  protected internal override void performStart(CmmnActivityExecution execution)
	  {
		execution.createTask(taskDecorator);
	  }

	  protected internal override void performTerminate(CmmnActivityExecution execution)
	  {
		terminating(execution);
		base.performTerminate(execution);
	  }

	  protected internal override void performExit(CmmnActivityExecution execution)
	  {
		terminating(execution);
		base.performExit(execution);
	  }

	  protected internal virtual void terminating(CmmnActivityExecution execution)
	  {
		TaskEntity task = getTask(execution);
		// it can happen that a there does not exist
		// a task, because the given execution was never
		// active.
		if (task != null)
		{
		  task.delete("terminated", false);
		}
	  }

	  protected internal override void completing(CmmnActivityExecution execution)
	  {
		TaskEntity task = getTask(execution);
		if (task != null)
		{
		  task.caseExecutionCompleted();
		}
	  }

	  protected internal override void manualCompleting(CmmnActivityExecution execution)
	  {
		completing(execution);
	  }

	  protected internal virtual void suspending(CmmnActivityExecution execution)
	  {
		string id = execution.Id;

		Context.CommandContext.TaskManager.updateTaskSuspensionStateByCaseExecutionId(id, org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED);
	  }

	  protected internal override void resuming(CmmnActivityExecution execution)
	  {
		string id = execution.Id;

		Context.CommandContext.TaskManager.updateTaskSuspensionStateByCaseExecutionId(id, org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE);
	  }

	  protected internal virtual TaskEntity getTask(CmmnActivityExecution execution)
	  {
		return Context.CommandContext.TaskManager.findTaskByCaseExecutionId(execution.Id);
	  }

	  protected internal override string TypeName
	  {
		  get
		  {
			return "human task";
		  }
	  }

	  // getters/setters /////////////////////////////////////////////////

	  public virtual TaskDecorator TaskDecorator
	  {
		  get
		  {
			return taskDecorator;
		  }
		  set
		  {
			this.taskDecorator = value;
		  }
	  }


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

	}

}