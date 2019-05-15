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
namespace org.camunda.bpm.engine.cdi.impl.@event
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CdiBusinessProcessEvent : BusinessProcessEvent
	{

	  protected internal readonly string activityId;
	  protected internal readonly ProcessDefinition processDefinition;
	  protected internal readonly string transitionName;
	  protected internal readonly string processInstanceId;
	  protected internal readonly string executionId;
	  protected internal readonly DelegateTask delegateTask;
	  protected internal readonly BusinessProcessEventType type;
	  protected internal readonly DateTime timeStamp;

	  public CdiBusinessProcessEvent(string activityId, string transitionName, ProcessDefinition processDefinition, DelegateExecution execution, BusinessProcessEventType type, DateTime timeStamp)
	  {
		  this.activityId = activityId;
		  this.transitionName = transitionName;
		  this.processInstanceId = execution.ProcessInstanceId;
		  this.executionId = execution.Id;
		  this.type = type;
		  this.timeStamp = timeStamp;
		  this.processDefinition = processDefinition;
		  this.delegateTask = null;
	  }

	  public CdiBusinessProcessEvent(DelegateTask task, ProcessDefinitionEntity processDefinition, BusinessProcessEventType type, DateTime timeStamp)
	  {
		this.activityId = null;
		this.transitionName = null;
		this.processInstanceId = task.ProcessInstanceId;
		this.executionId = task.ExecutionId;
		this.type = type;
		this.timeStamp = timeStamp;
		this.processDefinition = processDefinition;
		this.delegateTask = task;
	  }

	  public virtual ProcessDefinition ProcessDefinition
	  {
		  get
		  {
			return processDefinition;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string TransitionName
	  {
		  get
		  {
			return transitionName;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual BusinessProcessEventType Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual DateTime TimeStamp
	  {
		  get
		  {
			return timeStamp;
		  }
	  }

	  public virtual DelegateTask Task
	  {
		  get
		  {
			return delegateTask;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			if (delegateTask != null)
			{
			  return delegateTask.Id;
			}
			return null;
		  }
	  }

	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			if (delegateTask != null)
			{
			  return delegateTask.TaskDefinitionKey;
			}
			return null;
		  }
	  }

	  public override string ToString()
	  {
		return "Event '" + processDefinition.Key + "' ['" + type + "', " + (type == org.camunda.bpm.engine.cdi.BusinessProcessEventType_Fields.TAKE ? transitionName : activityId) + "]";
	  }

	}

}