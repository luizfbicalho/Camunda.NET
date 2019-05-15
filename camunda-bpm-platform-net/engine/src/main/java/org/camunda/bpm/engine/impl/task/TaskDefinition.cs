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
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using TaskFormHandler = org.camunda.bpm.engine.impl.form.handler.TaskFormHandler;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;

	/// <summary>
	/// Container for task definition information gathered at parsing time.
	/// 
	/// @author Joram Barrez
	/// </summary>

	public class TaskDefinition
	{
	  protected internal string key;

	  // assignment fields
	  protected internal Expression nameExpression;
	  protected internal Expression descriptionExpression;
	  protected internal Expression assigneeExpression;
	  protected internal ISet<Expression> candidateUserIdExpressions = new HashSet<Expression>();
	  protected internal ISet<Expression> candidateGroupIdExpressions = new HashSet<Expression>();
	  protected internal Expression dueDateExpression;
	  protected internal Expression followUpDateExpression;
	  protected internal Expression priorityExpression;

	  // form fields
	  protected internal TaskFormHandler taskFormHandler;
	  protected internal Expression formKey;

	  // task listeners
	  protected internal IDictionary<string, IList<TaskListener>> taskListeners = new Dictionary<string, IList<TaskListener>>();
	  protected internal IDictionary<string, IList<TaskListener>> builtinTaskListeners = new Dictionary<string, IList<TaskListener>>();

	  public TaskDefinition(TaskFormHandler taskFormHandler)
	  {
		this.taskFormHandler = taskFormHandler;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual Expression NameExpression
	  {
		  get
		  {
			return nameExpression;
		  }
		  set
		  {
			this.nameExpression = value;
		  }
	  }


	  public virtual Expression DescriptionExpression
	  {
		  get
		  {
			return descriptionExpression;
		  }
		  set
		  {
			this.descriptionExpression = value;
		  }
	  }


	  public virtual Expression AssigneeExpression
	  {
		  get
		  {
			return assigneeExpression;
		  }
		  set
		  {
			this.assigneeExpression = value;
		  }
	  }


	  public virtual ISet<Expression> CandidateUserIdExpressions
	  {
		  get
		  {
			return candidateUserIdExpressions;
		  }
	  }

	  public virtual void addCandidateUserIdExpression(Expression userId)
	  {
		candidateUserIdExpressions.Add(userId);
	  }

	  public virtual ISet<Expression> CandidateGroupIdExpressions
	  {
		  get
		  {
			return candidateGroupIdExpressions;
		  }
	  }

	  public virtual void addCandidateGroupIdExpression(Expression groupId)
	  {
		candidateGroupIdExpressions.Add(groupId);
	  }

	  public virtual Expression PriorityExpression
	  {
		  get
		  {
			return priorityExpression;
		  }
		  set
		  {
			this.priorityExpression = value;
		  }
	  }


	  public virtual TaskFormHandler TaskFormHandler
	  {
		  get
		  {
			return taskFormHandler;
		  }
		  set
		  {
			this.taskFormHandler = value;
		  }
	  }


	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
		  }
	  }


	  public virtual Expression DueDateExpression
	  {
		  get
		  {
			return dueDateExpression;
		  }
		  set
		  {
			this.dueDateExpression = value;
		  }
	  }


	  public virtual Expression FollowUpDateExpression
	  {
		  get
		  {
			return followUpDateExpression;
		  }
		  set
		  {
			this.followUpDateExpression = value;
		  }
	  }


	  public virtual IDictionary<string, IList<TaskListener>> TaskListeners
	  {
		  get
		  {
			return taskListeners;
		  }
		  set
		  {
			this.taskListeners = value;
		  }
	  }

	  public virtual IDictionary<string, IList<TaskListener>> BuiltinTaskListeners
	  {
		  get
		  {
			return builtinTaskListeners;
		  }
	  }


	  public virtual IList<TaskListener> getTaskListeners(string eventName)
	  {
		return taskListeners[eventName];
	  }

	  public virtual IList<TaskListener> getBuiltinTaskListeners(string eventName)
	  {
		return builtinTaskListeners[eventName];
	  }

	  public virtual void addTaskListener(string eventName, TaskListener taskListener)
	  {
		CollectionUtil.addToMapOfLists(taskListeners, eventName, taskListener);
	  }

	  public virtual void addBuiltInTaskListener(string eventName, TaskListener taskListener)
	  {
		IList<TaskListener> listeners = taskListeners[eventName];
		if (listeners == null)
		{
		  listeners = new List<TaskListener>();
		  taskListeners[eventName] = listeners;
		}

		listeners.Insert(0, taskListener);

		CollectionUtil.addToMapOfLists(builtinTaskListeners, eventName, taskListener);
	  }


	  public virtual Expression FormKey
	  {
		  get
		  {
			return formKey;
		  }
		  set
		  {
			this.formKey = value;
		  }
	  }


	}

}