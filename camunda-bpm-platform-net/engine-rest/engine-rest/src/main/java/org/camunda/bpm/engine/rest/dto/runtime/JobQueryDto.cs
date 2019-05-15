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
namespace org.camunda.bpm.engine.rest.dto.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;



	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using ConditionListConverter = org.camunda.bpm.engine.rest.dto.converter.ConditionListConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using LongConverter = org.camunda.bpm.engine.rest.dto.converter.LongConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class JobQueryDto : AbstractQueryDto<JobQuery>
	{

	  private const string SORT_BY_JOB_ID_VALUE = "jobId";
	  private const string SORT_BY_EXECUTION_ID_VALUE = "executionId";
	  private const string SORT_BY_PROCESS_INSTANCE_ID_VALUE = "processInstanceId";
	  private const string SORT_BY_PROCESS_DEFINITION_ID_VALUE = "processDefinitionId";
	  private const string SORT_BY_PROCESS_DEFINITION_KEY_VALUE = "processDefinitionKey";
	  private const string SORT_BY_JOB_RETRIES_VALUE = "jobRetries";
	  private const string SORT_BY_JOB_DUEDATE_VALUE = "jobDueDate";
	  private const string SORT_BY_JOB_PRIORITY_VALUE = "jobPriority";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static JobQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_EXECUTION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_INSTANCE_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_RETRIES_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_DUEDATE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_PRIORITY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string activityId;
	  protected internal string jobId;
	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal bool? withRetriesLeft;
	  protected internal bool? executable;
	  protected internal bool? timers;
	  protected internal bool? messages;
	  protected internal bool? withException;
	  protected internal string exceptionMessage;
	  protected internal bool? noRetriesLeft;
	  protected internal bool? active;
	  protected internal bool? suspended;
	  protected internal long? priorityHigherThanOrEquals;
	  protected internal long? priorityLowerThanOrEquals;
	  protected internal string jobDefinitionId;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;
	  protected internal bool? includeJobsWithoutTenantId;

	  protected internal IList<ConditionQueryParameterDto> dueDates;
	  protected internal IList<ConditionQueryParameterDto> createTimes;

	  public JobQueryDto()
	  {
	  }

	  public JobQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("activityId")]
	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityId = value;
		  }
	  }

	  [CamundaQueryParam("jobId")]
	  public virtual string JobId
	  {
		  set
		  {
			this.jobId = value;
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

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam("processDefinitionKey")]
	  public virtual string ProcessDefinitionKey
	  {
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam(value:"withRetriesLeft", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithRetriesLeft
	  {
		  set
		  {
			this.withRetriesLeft = value;
		  }
	  }

	  [CamundaQueryParam(value:"executable", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Executable
	  {
		  set
		  {
			this.executable = value;
		  }
	  }

	  [CamundaQueryParam(value:"timers", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Timers
	  {
		  set
		  {
			this.timers = value;
		  }
	  }

	  [CamundaQueryParam(value:"withException", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithException
	  {
		  set
		  {
			this.withException = value;
		  }
	  }

	  [CamundaQueryParam(value:"messages", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Messages
	  {
		  set
		  {
			this.messages = value;
		  }
	  }

	  [CamundaQueryParam("exceptionMessage")]
	  public virtual string ExceptionMessage
	  {
		  set
		  {
			this.exceptionMessage = value;
		  }
	  }

	  [CamundaQueryParam(value : "dueDates", converter : org.camunda.bpm.engine.rest.dto.converter.ConditionListConverter.class)]
	  public virtual IList<ConditionQueryParameterDto> DueDates
	  {
		  set
		  {
			this.dueDates = value;
		  }
	  }

	  [CamundaQueryParam(value : "createTimes", converter : org.camunda.bpm.engine.rest.dto.converter.ConditionListConverter.class)]
	  public virtual IList<ConditionQueryParameterDto> CreateTimes
	  {
		  set
		  {
			this.createTimes = value;
		  }
	  }

	  [CamundaQueryParam(value:"noRetriesLeft", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? NoRetriesLeft
	  {
		  set
		  {
			this.noRetriesLeft = value;
		  }
	  }

	  [CamundaQueryParam(value:"active", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Active
	  {
		  set
		  {
			this.active = value;
		  }
	  }

	  [CamundaQueryParam(value:"suspended", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Suspended
	  {
		  set
		  {
			this.suspended = value;
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

	  [CamundaQueryParam("jobDefinitionId")]
	  public virtual string JobDefinitionId
	  {
		  set
		  {
			this.jobDefinitionId = value;
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

	  [CamundaQueryParam(value : "withoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutTenantId
	  {
		  set
		  {
			this.withoutTenantId = value;
		  }
	  }

	  [CamundaQueryParam(value : "includeJobsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeJobsWithoutTenantId
	  {
		  set
		  {
			this.includeJobsWithoutTenantId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override JobQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.ManagementService.createJobQuery();
	  }

	  private abstract class ApplyDates
	  {
		  private readonly JobQueryDto outerInstance;

		  public ApplyDates(JobQueryDto outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal virtual void run(IList<ConditionQueryParameterDto> dates)
		{
		  DateConverter dateConverter = new DateConverter();
		  dateConverter.ObjectMapper = objectMapper;

		  foreach (ConditionQueryParameterDto conditionQueryParam in dates)
		  {
			string op = conditionQueryParam.Operator;
			DateTime date;

			try
			{
			  date = dateConverter.convertQueryParameterToType((string) conditionQueryParam.Value);
			}
			catch (RestException e)
			{
			  throw new InvalidRequestException(e.Status, e, "Invalid " + fieldName() + " format: " + e.Message);
			}

			if (op.Equals(ConditionQueryParameterDto.GREATER_THAN_OPERATOR_NAME))
			{
			  GreaterThan = date;
			}
			else if (op.Equals(ConditionQueryParameterDto.LESS_THAN_OPERATOR_NAME))
			{
			  LowerThan = date;
			}
			else
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Invalid " + fieldName() + " comparator specified: " + op);
			}
		  }
		}

		/// <returns> a descriptive name of the target field, used in error-messages </returns>
		internal abstract string fieldName();

		internal abstract DateTime GreaterThan {set;}

		internal abstract DateTime LowerThan {set;}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected void applyFilters(final org.camunda.bpm.engine.runtime.JobQuery query)
	  protected internal override void applyFilters(JobQuery query)
	  {
		if (!string.ReferenceEquals(activityId, null))
		{
		  query.activityId(activityId);
		}

		if (!string.ReferenceEquals(jobId, null))
		{
		  query.jobId(jobId);
		}

		if (!string.ReferenceEquals(executionId, null))
		{
		  query.executionId(executionId);
		}

		if (!string.ReferenceEquals(processInstanceId, null))
		{
		  query.processInstanceId(processInstanceId);
		}

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}

		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}

		if (TRUE.Equals(withRetriesLeft))
		{
		  query.withRetriesLeft();
		}

		if (TRUE.Equals(executable))
		{
		  query.executable();
		}

		if (TRUE.Equals(timers))
		{
		  if (messages != null && messages)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "Parameter timers cannot be used together with parameter messages.");
		  }
		  query.timers();
		}

		if (TRUE.Equals(messages))
		{
		  if (timers != null && timers)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, "Parameter messages cannot be used together with parameter timers.");
		  }
		  query.messages();
		}

		if (TRUE.Equals(withException))
		{
		  query.withException();
		}

		if (!string.ReferenceEquals(exceptionMessage, null))
		{
		  query.exceptionMessage(exceptionMessage);
		}

		if (TRUE.Equals(noRetriesLeft))
		{
		  query.noRetriesLeft();
		}

		if (TRUE.Equals(active))
		{
		  query.active();
		}

		if (TRUE.Equals(suspended))
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

		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  query.jobDefinitionId(jobDefinitionId);
		}

		if (dueDates != null)
		{
		  new ApplyDatesAnonymousInnerClass(this, query)
		  .run(dueDates);
		}

		if (createTimes != null)
		{
		  new ApplyDatesAnonymousInnerClass2(this, query)
		  .run(createTimes);
		}

		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (TRUE.Equals(includeJobsWithoutTenantId))
		{
		  query.includeJobsWithoutTenantId();
		}
	  }

	  private class ApplyDatesAnonymousInnerClass : ApplyDates
	  {
		  private readonly JobQueryDto outerInstance;

		  private JobQuery query;

		  public ApplyDatesAnonymousInnerClass(JobQueryDto outerInstance, JobQuery query) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.query = query;
		  }

		  internal override DateTime GreaterThan
		  {
			  set
			  {
				query.duedateHigherThan(value);
			  }
		  }

		  internal override DateTime LowerThan
		  {
			  set
			  {
				query.duedateLowerThan(value);
			  }
		  }

		  internal override string fieldName()
		  {
			return "due date";
		  }
	  }

	  private class ApplyDatesAnonymousInnerClass2 : ApplyDates
	  {
		  private readonly JobQueryDto outerInstance;

		  private JobQuery query;

		  public ApplyDatesAnonymousInnerClass2(JobQueryDto outerInstance, JobQuery query) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.query = query;
		  }

		  internal override DateTime GreaterThan
		  {
			  set
			  {
				query.createdAfter(value);
			  }
		  }

		  internal override DateTime LowerThan
		  {
			  set
			  {
				query.createdBefore(value);
			  }
		  }

		  internal override string fieldName()
		  {
			return "create time";
		  }
	  }

	  protected internal override void applySortBy(JobQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_JOB_ID_VALUE))
		{
		  query.orderByJobId();
		}
		else if (sortBy.Equals(SORT_BY_EXECUTION_ID_VALUE))
		{
		  query.orderByExecutionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_INSTANCE_ID_VALUE))
		{
		  query.orderByProcessInstanceId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID_VALUE))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_KEY_VALUE))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_JOB_RETRIES_VALUE))
		{
		  query.orderByJobRetries();
		}
		else if (sortBy.Equals(SORT_BY_JOB_DUEDATE_VALUE))
		{
		  query.orderByJobDuedate();
		}
		else if (sortBy.Equals(SORT_BY_JOB_PRIORITY_VALUE))
		{
		  query.orderByJobPriority();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}