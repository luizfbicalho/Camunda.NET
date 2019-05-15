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
namespace org.camunda.bpm.engine.impl.task
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using BusinessCalendar = org.camunda.bpm.engine.impl.calendar.BusinessCalendar;
	using DueDateBusinessCalendar = org.camunda.bpm.engine.impl.calendar.DueDateBusinessCalendar;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskDecorator
	{

	  protected internal TaskDefinition taskDefinition;
	  protected internal ExpressionManager expressionManager;

	  public TaskDecorator(TaskDefinition taskDefinition, ExpressionManager expressionManager)
	  {
		this.taskDefinition = taskDefinition;
		this.expressionManager = expressionManager;
	  }

	  public virtual void decorate(TaskEntity task, VariableScope variableScope)
	  {
		// set the taskDefinition
		task.TaskDefinition = taskDefinition;

		// name
		initializeTaskName(task, variableScope);
		// description
		initializeTaskDescription(task, variableScope);
		// dueDate
		initializeTaskDueDate(task, variableScope);
		// followUpDate
		initializeTaskFollowUpDate(task, variableScope);
		// priority
		initializeTaskPriority(task, variableScope);
		// assignments
		initializeTaskAssignments(task, variableScope);
	  }

	  protected internal virtual void initializeTaskName(TaskEntity task, VariableScope variableScope)
	  {
		Expression nameExpression = taskDefinition.NameExpression;
		if (nameExpression != null)
		{
		  string name = (string) nameExpression.getValue(variableScope);
		  task.Name = name;
		}
	  }

	  protected internal virtual void initializeTaskDescription(TaskEntity task, VariableScope variableScope)
	  {
		Expression descriptionExpression = taskDefinition.DescriptionExpression;
		if (descriptionExpression != null)
		{
		  string description = (string) descriptionExpression.getValue(variableScope);
		  task.Description = description;
		}
	  }

	  protected internal virtual void initializeTaskDueDate(TaskEntity task, VariableScope variableScope)
	  {
		Expression dueDateExpression = taskDefinition.DueDateExpression;
		if (dueDateExpression != null)
		{
		  object dueDate = dueDateExpression.getValue(variableScope);
		  if (dueDate != null)
		  {
			if (dueDate is DateTime)
			{
			  task.DueDate = (DateTime) dueDate;

			}
			else if (dueDate is string)
			{
			  BusinessCalendar businessCalendar = BusinessCalender;
			  task.DueDate = businessCalendar.resolveDuedate((string) dueDate);

			}
			else
			{
			  throw new ProcessEngineException("Due date expression does not resolve to a Date or Date string: " + dueDateExpression.ExpressionText);
			}
		  }
		}
	  }

	  protected internal virtual void initializeTaskFollowUpDate(TaskEntity task, VariableScope variableScope)
	  {
		Expression followUpDateExpression = taskDefinition.FollowUpDateExpression;
		if (followUpDateExpression != null)
		{
		  object followUpDate = followUpDateExpression.getValue(variableScope);
		  if (followUpDate != null)
		  {
			if (followUpDate is DateTime)
			{
			  task.FollowUpDate = (DateTime) followUpDate;

			}
			else if (followUpDate is string)
			{
			  BusinessCalendar businessCalendar = BusinessCalender;
			  task.FollowUpDate = businessCalendar.resolveDuedate((string) followUpDate);

			}
			else
			{
			  throw new ProcessEngineException("Follow up date expression does not resolve to a Date or Date string: " + followUpDateExpression.ExpressionText);
			}
		  }
		}
	  }

	  protected internal virtual void initializeTaskPriority(TaskEntity task, VariableScope variableScope)
	  {
		Expression priorityExpression = taskDefinition.PriorityExpression;
		if (priorityExpression != null)
		{
		  object priority = priorityExpression.getValue(variableScope);

		  if (priority != null)
		  {
			if (priority is string)
			{
			  try
			  {
				task.Priority = Convert.ToInt32((string) priority);

			  }
			  catch (System.FormatException e)
			  {
				throw new ProcessEngineException("Priority does not resolve to a number: " + priority, e);
			  }
			}
			else if (priority is Number)
			{
			  task.Priority = ((Number) priority).intValue();

			}
			else
			{
			  throw new ProcessEngineException("Priority expression does not resolve to a number: " + priorityExpression.ExpressionText);
			}
		  }
		}
	  }

	  protected internal virtual void initializeTaskAssignments(TaskEntity task, VariableScope variableScope)
	  {
		// assignee
		initializeTaskAssignee(task, variableScope);
		// candidateUsers
		initializeTaskCandidateUsers(task, variableScope);
		// candidateGroups
		initializeTaskCandidateGroups(task, variableScope);
	  }

	  protected internal virtual void initializeTaskAssignee(TaskEntity task, VariableScope variableScope)
	  {
		Expression assigneeExpression = taskDefinition.AssigneeExpression;
		if (assigneeExpression != null)
		{
		  task.Assignee = (string) assigneeExpression.getValue(variableScope);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void initializeTaskCandidateGroups(org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task, org.camunda.bpm.engine.delegate.VariableScope variableScope)
	  protected internal virtual void initializeTaskCandidateGroups(TaskEntity task, VariableScope variableScope)
	  {
		ISet<Expression> candidateGroupIdExpressions = taskDefinition.CandidateGroupIdExpressions;

		foreach (Expression groupIdExpr in candidateGroupIdExpressions)
		{
		  object value = groupIdExpr.getValue(variableScope);

		  if (value is string)
		  {
			IList<string> candiates = extractCandidates((string) value);
			task.addCandidateGroups(candiates);

		  }
		  else if (value is System.Collections.ICollection)
		  {
			task.addCandidateGroups((System.Collections.ICollection) value);

		  }
		  else
		  {
			throw new ProcessEngineException("Expression did not resolve to a string or collection of strings");
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void initializeTaskCandidateUsers(org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task, org.camunda.bpm.engine.delegate.VariableScope variableScope)
	  protected internal virtual void initializeTaskCandidateUsers(TaskEntity task, VariableScope variableScope)
	  {
		ISet<Expression> candidateUserIdExpressions = taskDefinition.CandidateUserIdExpressions;
		foreach (Expression userIdExpr in candidateUserIdExpressions)
		{
		  object value = userIdExpr.getValue(variableScope);

		  if (value is string)
		  {
			IList<string> candiates = extractCandidates((string) value);
			task.addCandidateUsers(candiates);

		  }
		  else if (value is System.Collections.ICollection)
		  {
			task.addCandidateUsers((System.Collections.ICollection) value);

		  }
		  else
		  {
			throw new ProcessEngineException("Expression did not resolve to a string or collection of strings");
		  }
		}
	  }


	  /// <summary>
	  /// Extract a candidate list from a string.
	  /// </summary>
	  protected internal virtual IList<string> extractCandidates(string str)
	  {
		return Arrays.asList(str.Split("[\\s]*,[\\s]*", true));
	  }

	  // getters ///////////////////////////////////////////////////////////////

	  public virtual TaskDefinition TaskDefinition
	  {
		  get
		  {
			return taskDefinition;
		  }
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
	  }

	  protected internal virtual BusinessCalendar BusinessCalender
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(DueDateBusinessCalendar.NAME);
		  }
	  }

	}

}