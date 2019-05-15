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
namespace org.camunda.bpm.engine.rest.dto.management
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;


	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class JobDefinitionQueryDto : AbstractQueryDto<JobDefinitionQuery>
	{

	  private const string SORT_BY_JOB_DEFINITION_ID = "jobDefinitionId";
	  private const string SORT_BY_ACTIVITY_ID = "activityId";
	  private const string SORT_BY_PROCESS_DEFINITION_ID = "processDefinitionId";
	  private const string SORT_BY_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  private const string SORT_BY_JOB_TYPE = "jobType";
	  private const string SORT_BY_JOB_CONFIGURATION = "jobConfiguration";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static JobDefinitionQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();

		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ACTIVITY_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_PROCESS_DEFINITION_KEY);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_JOB_CONFIGURATION);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string jobDefinitionId;
	  protected internal string[] activityIdIn;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string jobType;
	  protected internal string jobConfiguration;
	  protected internal bool? active;
	  protected internal bool? suspended;
	  protected internal bool? withOverridingJobPriority;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;
	  protected internal bool? includeJobDefinitionsWithoutTenantId;

	  public JobDefinitionQueryDto()
	  {
	  }

	  public JobDefinitionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("jobDefinitionId")]
	  public virtual string JobDefinitionId
	  {
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value:"activityIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] ActivityIdIn
	  {
		  set
		  {
			this.activityIdIn = value;
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

	  [CamundaQueryParam("jobType")]
	  public virtual string JobType
	  {
		  set
		  {
			this.jobType = value;
		  }
	  }

	  [CamundaQueryParam("jobConfiguration")]
	  public virtual string JobConfiguration
	  {
		  set
		  {
			this.jobConfiguration = value;
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

	  [CamundaQueryParam(value:"withOverridingJobPriority", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithOverridingJobPriority
	  {
		  set
		  {
			this.withOverridingJobPriority = value;
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

	  [CamundaQueryParam(value : "includeJobDefinitionsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeJobDefinitionsWithoutTenantId
	  {
		  set
		  {
			this.includeJobDefinitionsWithoutTenantId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override JobDefinitionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.ManagementService.createJobDefinitionQuery();
	  }

	  protected internal override void applyFilters(JobDefinitionQuery query)
	  {
		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  query.jobDefinitionId(jobDefinitionId);
		}

		if (activityIdIn != null && activityIdIn.Length > 0)
		{
		  query.activityIdIn(activityIdIn);
		}

		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}

		if (!string.ReferenceEquals(processDefinitionKey, null))
		{
		  query.processDefinitionKey(processDefinitionKey);
		}

		if (!string.ReferenceEquals(jobType, null))
		{
		  query.jobType(jobType);
		}

		if (!string.ReferenceEquals(jobConfiguration, null))
		{
		  query.jobConfiguration(jobConfiguration);
		}

		if (TRUE.Equals(active))
		{
		  query.active();
		}

		if (TRUE.Equals(suspended))
		{
		  query.suspended();
		}

		if (TRUE.Equals(withOverridingJobPriority))
		{
		  query.withOverridingJobPriority();
		}

		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (TRUE.Equals(includeJobDefinitionsWithoutTenantId))
		{
		  query.includeJobDefinitionsWithoutTenantId();
		}
	  }

	  protected internal override void applySortBy(JobDefinitionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_JOB_DEFINITION_ID))
		{
		  query.orderByJobDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_ACTIVITY_ID))
		{
		  query.orderByActivityId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_ID))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_PROCESS_DEFINITION_KEY))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_JOB_TYPE))
		{
		  query.orderByJobType();
		}
		else if (sortBy.Equals(SORT_BY_JOB_CONFIGURATION))
		{
		  query.orderByJobConfiguration();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}