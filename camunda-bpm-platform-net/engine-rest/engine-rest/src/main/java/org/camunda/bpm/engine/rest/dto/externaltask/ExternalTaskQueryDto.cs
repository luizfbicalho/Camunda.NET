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
namespace org.camunda.bpm.engine.rest.dto.externaltask
{

	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using LongConverter = org.camunda.bpm.engine.rest.dto.converter.LongConverter;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskQueryDto : AbstractQueryDto<ExternalTaskQuery>
	{

	  public const string SORT_BY_ID_VALUE = "id";
	  public const string SORT_BY_LOCK_EXPIRATION_TIME = "lockExpirationTime";
	  public const string SORT_BY_PROCESS_INSTANCE_ID = "processInstanceId";
	  public const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  public const string SORT_BY_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  public const string SORT_BY_TENANT_ID = "tenantId";
	  public const string SORT_BY_PRIORITY = "taskPriority";

	  public static readonly IList<string> VALID_SORT_BY_VALUES;
	  static ExternalTaskQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_LOCK_EXPIRATION_TIME);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PRIORITY);
	  }

	  protected internal string externalTaskId;
	  protected internal string activityId;
	  protected internal IList<string> activityIdIn;
	  protected internal DateTime lockExpirationBefore;
	  protected internal DateTime lockExpirationAfter;
	  protected internal string topicName;
	  protected internal bool? locked;
	  protected internal bool? notLocked;
	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal IList<string> processInstanceIdIn;
	  protected internal string processDefinitionId;
	  protected internal bool? active;
	  protected internal bool? suspended;
	  protected internal bool? withRetriesLeft;
	  protected internal bool? noRetriesLeft;
	  protected internal string workerId;
	  protected internal IList<string> tenantIds;
	  protected internal long? priorityHigherThanOrEquals;
	  protected internal long? priorityLowerThanOrEquals;

	  public ExternalTaskQueryDto()
	  {
	  }

	  public ExternalTaskQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("externalTaskId")]
	  public virtual string ExternalTaskId
	  {
		  set
		  {
			this.externalTaskId = value;
		  }
	  }

	  [CamundaQueryParam("activityId")]
	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityId = value;
		  }
	  }

	  [CamundaQueryParam(value : "activityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ActivityIdIn
	  {
		  set
		  {
			this.activityIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value : "lockExpirationBefore", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime LockExpirationBefore
	  {
		  set
		  {
			this.lockExpirationBefore = value;
		  }
	  }

	  [CamundaQueryParam(value : "lockExpirationAfter", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime LockExpirationAfter
	  {
		  set
		  {
			this.lockExpirationAfter = value;
		  }
	  }

	  [CamundaQueryParam("topicName")]
	  public virtual string TopicName
	  {
		  set
		  {
			this.topicName = value;
		  }
	  }

	  [CamundaQueryParam(value : "locked", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Locked
	  {
		  set
		  {
			this.locked = value;
		  }
	  }

	  [CamundaQueryParam(value : "notLocked", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? NotLocked
	  {
		  set
		  {
			this.notLocked = value;
		  }
	  }

	  [CamundaQueryParam("executionId")]
	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
	  }

	  [CamundaQueryParam("processInstanceId")]
	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	  [CamundaQueryParam(value:"processInstanceIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ProcessInstanceIdIn
	  {
		  set
		  {
			this.processInstanceIdIn = value;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  [CamundaQueryParam(value : "active", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Active
	  {
		  set
		  {
			this.active = value;
		  }
	  }

	  [CamundaQueryParam(value : "suspended", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Suspended
	  {
		  set
		  {
			this.suspended = value;
		  }
	  }

	  [CamundaQueryParam(value : "withRetriesLeft", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithRetriesLeft
	  {
		  set
		  {
			this.withRetriesLeft = value;
		  }
	  }

	  [CamundaQueryParam(value : "noRetriesLeft", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? NoRetriesLeft
	  {
		  set
		  {
			this.noRetriesLeft = value;
		  }
	  }

	  [CamundaQueryParam("workerId")]
	  public virtual string WorkerId
	  {
		  set
		  {
			this.workerId = value;
		  }
	  }

	  [CamundaQueryParam(value : "tenantIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> TenantIdIn
	  {
		  set
		  {
			this.tenantIds = value;
		  }
	  }
	  [CamundaQueryParam(value:"priorityHigherThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? PriorityHigherThanOrEquals
	  {
		  set
		  {
			this.priorityHigherThanOrEquals = value;
		  }
	  }

	  [CamundaQueryParam(value:"priorityLowerThanOrEquals", converter : org.camunda.bpm.engine.rest.dto.converter.LongConverter.class)]
	  public virtual long? PriorityLowerThanOrEquals
	  {
		  set
		  {
			this.priorityLowerThanOrEquals = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override ExternalTaskQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.ExternalTaskService.createExternalTaskQuery();
	  }

	  protected internal override void applyFilters(ExternalTaskQuery query)
	  {
		if (!string.ReferenceEquals(externalTaskId, null))
		{
		  query.externalTaskId(externalTaskId);
		}
		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}
		if (activityIdIn != null && activityIdIn.Count > 0)
		{
		  query.activityIdIn(activityIdIn.ToArray());
		}
		if (lockExpirationBefore != null)
		{
		  query.lockExpirationBefore(lockExpirationBefore);
		}
		if (lockExpirationAfter != null)
		{
		  query.lockExpirationAfter(lockExpirationAfter);
		}
		if (!string.ReferenceEquals(topicName, null))
		{
		  query.topicName(topicName);
		}
		if (locked != null && locked)
		{
		  query.locked();
		}
		if (notLocked != null && notLocked)
		{
		  query.notLocked();
		}
		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}
		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}
		if (processInstanceIdIn != null && processInstanceIdIn.Count > 0)
		{
		  query.processInstanceIdIn(processInstanceIdIn.ToArray());
		}
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (active != null && active)
		{
		  query.active();
		}
		if (suspended != null && suspended)
		{
		  query.suspended();
		}
		if (priorityHigherThanOrEquals != null)
		{
		  query.priorityHigherThanOrEquals(priorityHigherThanOrEquals.Value);
		}
		if (priorityLowerThanOrEquals != null)
		{
		  query.priorityLowerThanOrEquals(priorityLowerThanOrEquals.Value);
		}
		if (withRetriesLeft != null && withRetriesLeft)
		{
		  query.withRetriesLeft();
		}
		if (noRetriesLeft != null && noRetriesLeft)
		{
		  query.noRetriesLeft();
		}
		if (!string.ReferenceEquals(workerId, null))
		{
		  query.workerId(workerId);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
	  }

	  protected internal override void applySortBy(ExternalTaskQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (SORT_BY_ID_VALUE.Equals(sortBy))
		{
		  query.orderById();
		}
		else if (SORT_BY_LOCK_EXPIRATION_TIME.Equals(sortBy))
		{
		  query.orderByLockExpirationTime();
		}
		else if (SORT_BY_PROCESS_DEFINITION_ID.Equals(sortBy))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (SORT_BY_PROCESS_DEFINITION_KEY.Equals(sortBy))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (SORT_BY_PROCESS_INSTANCE_ID.Equals(sortBy))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
		else if (sortBy.Equals(SORT_BY_PRIORITY))
		{
		  query.orderByPriority();
		}
	  }
	}

}