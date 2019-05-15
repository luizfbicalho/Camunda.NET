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
namespace org.camunda.bpm.engine.rest.dto.runtime
{

	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;

	public class EventSubscriptionDto
	{

	  private string id;
	  private string eventType;
	  private string eventName;
	  private string executionId;
	  private string processInstanceId;
	  private string activityId;
	  private DateTime createdDate;
	  private string tenantId;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual string EventType
	  {
		  get
		  {
			return eventType;
		  }
	  }
	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }
	  public virtual DateTime CreatedDate
	  {
		  get
		  {
			return createdDate;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public static EventSubscriptionDto fromEventSubscription(EventSubscription eventSubscription)
	  {
		EventSubscriptionDto dto = new EventSubscriptionDto();
		dto.id = eventSubscription.Id;
		dto.eventType = eventSubscription.EventType;
		dto.eventName = eventSubscription.EventName;
		dto.executionId = eventSubscription.ExecutionId;
		dto.processInstanceId = eventSubscription.ProcessInstanceId;
		dto.activityId = eventSubscription.ActivityId;
		dto.createdDate = eventSubscription.Created;
		dto.tenantId = eventSubscription.TenantId;

		return dto;
	  }


	}

}