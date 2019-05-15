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
namespace org.camunda.bpm.engine.test.bpmn.@event.compensate
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[Serializable]
	public class ReadLocalVariableListener : ExecutionListener
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<VariableEvent> variableEvents = new List<VariableEvent>();
	  protected internal string variableName;

	  public ReadLocalVariableListener(string variableName)
	  {
		this.variableName = variableName;
	  }

	  public virtual IList<VariableEvent> VariableEvents
	  {
		  get
		  {
			return variableEvents;
		  }
		  set
		  {
			this.variableEvents = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		if (!execution.hasVariableLocal(variableName))
		{
		  return;
		}

		object value = execution.getVariableLocal(variableName);

		VariableEvent @event = new VariableEvent();
		@event.variableName = variableName;
		@event.variableValue = value;
		@event.eventName = execution.EventName;
		@event.activityInstanceId = execution.ActivityInstanceId;

		variableEvents.Add(@event);
	  }

	  [Serializable]
	  public class VariableEvent
	  {

		internal const long serialVersionUID = 1L;

		protected internal string variableName;
		protected internal object variableValue;

		protected internal string activityInstanceId;
		protected internal string eventName;

		public virtual string VariableName
		{
			get
			{
			  return variableName;
			}
		}

		public virtual object VariableValue
		{
			get
			{
			  return variableValue;
			}
		}

		public virtual string EventName
		{
			get
			{
			  return eventName;
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
	}

}