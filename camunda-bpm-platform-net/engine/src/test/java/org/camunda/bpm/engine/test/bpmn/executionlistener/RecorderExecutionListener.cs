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
namespace org.camunda.bpm.engine.test.bpmn.executionlistener
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using PropertyKey = org.camunda.bpm.engine.impl.core.model.PropertyKey;
	using FixedValue = org.camunda.bpm.engine.impl.el.FixedValue;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Bernd Ruecker
	/// </summary>
	[Serializable]
	public class RecorderExecutionListener : ExecutionListener
	{

	  private const long serialVersionUID = 1L;

	  private FixedValue parameter;

	  private static IList<RecorderExecutionListener.RecordedEvent> recordedEvents = new List<RecorderExecutionListener.RecordedEvent>();

	  public class RecordedEvent
	  {
		internal readonly string activityId;
		internal readonly string eventName;
		internal readonly string activityName;
		internal readonly string parameter;
		internal readonly string activityInstanceId;
		internal readonly string transitionId;
		internal readonly bool canceled;
		internal readonly string executionId;

		public RecordedEvent(string activityId, string activityName, string eventName, string parameter, string activityInstanceId, string transitionId, bool canceled, string executionId)
		{
		  this.activityId = activityId;
		  this.activityName = activityName;
		  this.parameter = parameter;
		  this.eventName = eventName;
		  this.activityInstanceId = activityInstanceId;
		  this.transitionId = transitionId;
		  this.canceled = canceled;
		  this.executionId = executionId;
		}

		public virtual string ActivityId
		{
			get
			{
			  return activityId;
			}
		}

		public virtual string EventName
		{
			get
			{
			  return eventName;
			}
		}


		public virtual string ActivityName
		{
			get
			{
			  return activityName;
			}
		}


		public virtual string Parameter
		{
			get
			{
			  return parameter;
			}
		}

		public virtual string ActivityInstanceId
		{
			get
			{
			  return activityInstanceId;
			}
		}

		public virtual string TransitionId
		{
			get
			{
			  return transitionId;
			}
		}

		public virtual bool Canceled
		{
			get
			{
			  return canceled;
			}
		}

		public virtual string ExecutionId
		{
			get
			{
			  return executionId;
			}
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		ExecutionEntity executionCasted = ((ExecutionEntity)execution);
		string parameterValue = null;
		if (parameter != null)
		{
		  parameterValue = (string)parameter.getValue(execution);
		}

		string activityName = null;
		if (executionCasted.getActivity() != null)
		{
		  activityName = executionCasted.getActivity().Properties.get(new PropertyKey<string>("name"));
		}

		recordedEvents.Add(new RecordedEvent(executionCasted.ActivityId, activityName, execution.EventName, parameterValue, execution.ActivityInstanceId, execution.CurrentTransitionId, execution.Canceled, execution.Id));
	  }

	  public static void clear()
	  {
		recordedEvents.Clear();
	  }

	  public static IList<RecordedEvent> RecordedEvents
	  {
		  get
		  {
			return recordedEvents;
		  }
	  }


	}

}