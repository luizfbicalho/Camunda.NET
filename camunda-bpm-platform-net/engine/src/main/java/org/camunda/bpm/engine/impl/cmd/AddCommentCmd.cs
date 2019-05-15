using System;

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

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoricProcessInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricProcessInstanceEventEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommentEntity = org.camunda.bpm.engine.impl.persistence.entity.CommentEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Event = org.camunda.bpm.engine.task.Event;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class AddCommentCmd : Command<Comment>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string message;

	  public AddCommentCmd(string taskId, string processInstanceId, string message)
	  {
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.message = message;
	  }

	  public virtual Comment execute(CommandContext commandContext)
	  {

		if (string.ReferenceEquals(processInstanceId, null) && string.ReferenceEquals(taskId, null))
		{
		  throw new ProcessEngineException("Process instance id and task id is null");
		}

		ensureNotNull("Message", message);

		string userId = commandContext.AuthenticatedUserId;
		CommentEntity comment = new CommentEntity();
		comment.UserId = userId;
		comment.Type = CommentEntity.TYPE_COMMENT;
		comment.Time = ClockUtil.CurrentTime;
		comment.TaskId = taskId;
		comment.ProcessInstanceId = processInstanceId;
		comment.Action = org.camunda.bpm.engine.task.Event_Fields.ACTION_ADD_COMMENT;

		ExecutionEntity execution = getExecution(commandContext, taskId, processInstanceId);
		if (execution != null)
		{
		  comment.RootProcessInstanceId = execution.RootProcessInstanceId;
		}

		if (HistoryRemovalTimeStrategyStart)
		{
		  provideRemovalTime(comment);
		}

		string eventMessage = message.replaceAll("\\s+", " ");
		if (eventMessage.Length > 163)
		{
		  eventMessage = eventMessage.Substring(0, 160) + "...";
		}
		comment.Message = eventMessage;

		comment.FullMessage = message;

		commandContext.CommentManager.insert(comment);

		return comment;
	  }

	  protected internal virtual ExecutionEntity getExecution(CommandContext commandContext, string taskId, string processInstanceId)
	  {
		ExecutionEntity execution = null;
		if (!string.ReferenceEquals(taskId, null))
		{
		  TaskEntity task = commandContext.TaskManager.findTaskById(taskId);

		  if (task != null)
		  {
			execution = task.getExecution();
		  }
		}
		else if (!string.ReferenceEquals(processInstanceId, null))
		{
		  execution = commandContext.ExecutionManager.findExecutionById(processInstanceId);
		}

		return execution;
	  }

	  protected internal virtual bool HistoryRemovalTimeStrategyStart
	  {
		  get
		  {
			return HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(HistoryRemovalTimeStrategy);
		  }
	  }

	  protected internal virtual string HistoryRemovalTimeStrategy
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.HistoryRemovalTimeStrategy;
		  }
	  }

	  protected internal virtual HistoricProcessInstanceEventEntity getHistoricRootProcessInstance(string rootProcessInstanceId)
	  {
		return Context.CommandContext.DbEntityManager.selectById(typeof(HistoricProcessInstanceEventEntity), rootProcessInstanceId);
	  }

	  protected internal virtual void provideRemovalTime(CommentEntity comment)
	  {
		string rootProcessInstanceId = comment.RootProcessInstanceId;
		if (!string.ReferenceEquals(rootProcessInstanceId, null))
		{
		  HistoricProcessInstanceEventEntity historicRootProcessInstance = getHistoricRootProcessInstance(rootProcessInstanceId);

		  if (historicRootProcessInstance != null)
		  {
			DateTime removalTime = historicRootProcessInstance.RemovalTime;
			comment.RemovalTime = removalTime;
		  }
		}
	  }

	}

}