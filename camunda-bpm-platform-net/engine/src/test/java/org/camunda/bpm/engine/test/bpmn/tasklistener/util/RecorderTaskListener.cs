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
namespace org.camunda.bpm.engine.test.bpmn.tasklistener.util
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	[Serializable]
	public class RecorderTaskListener : TaskListener
	{

	  private const long serialVersionUID = 1L;

	  private static IList<RecorderTaskListener.RecordedTaskEvent> recordedEvents = new List<RecorderTaskListener.RecordedTaskEvent>();

	  public class RecordedTaskEvent
	  {

		protected internal string taskId;
		protected internal string executionId;
		protected internal string @event;
		protected internal string activityInstanceId;

		public RecordedTaskEvent(string taskId, string executionId, string @event, string activityInstanceId)
		{
		  this.executionId = executionId;
		  this.taskId = taskId;
		  this.@event = @event;
		  this.activityInstanceId = activityInstanceId;
		}

		public virtual string ExecutionId
		{
			get
			{
			  return executionId;
			}
		}

		public virtual string TaskId
		{
			get
			{
			  return taskId;
			}
		}

		public virtual string Event
		{
			get
			{
			  return @event;
			}
		}

		public virtual string ActivityInstanceId
		{
			get
			{
			  return activityInstanceId;
			}
		}

	  }

	  public virtual void notify(DelegateTask task)
	  {
		DelegateExecution execution = task.Execution;
		recordedEvents.Add(new RecordedTaskEvent(task.Id, task.ExecutionId, task.EventName, execution.ActivityInstanceId));
	  }

	  public static void clear()
	  {
		recordedEvents.Clear();
	  }

	  public static IList<RecordedTaskEvent> RecordedEvents
	  {
		  get
		  {
			return recordedEvents;
		  }
	  }


	}

}