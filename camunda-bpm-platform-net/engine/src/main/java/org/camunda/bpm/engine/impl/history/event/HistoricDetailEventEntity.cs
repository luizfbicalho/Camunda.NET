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
namespace org.camunda.bpm.engine.impl.history.@event
{

	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class HistoricDetailEventEntity : HistoryEvent
	{

	  private const long serialVersionUID = 1L;

	  protected internal string activityInstanceId;
	  protected internal string taskId;
	  protected internal DateTime timestamp;
	  protected internal string tenantId;
	  protected internal string userOperationId;

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string UserOperationId
	  {
		  get
		  {
			return userOperationId;
		  }
		  set
		  {
			this.userOperationId = value;
		  }
	  }


	  public override string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public virtual void delete()
	  {
		Context.CommandContext.DbEntityManager.delete(this);
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[activityInstanceId=" + activityInstanceId + ", taskId=" + taskId + ", timestamp=" + timestamp + ", eventType=" + eventType + ", executionId=" + executionId + ", processDefinitionId=" + processDefinitionId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + ", processInstanceId=" + processInstanceId + ", id=" + id + ", tenantId=" + tenantId + ", userOperationId=" + userOperationId + "]";
	  }

	}

}