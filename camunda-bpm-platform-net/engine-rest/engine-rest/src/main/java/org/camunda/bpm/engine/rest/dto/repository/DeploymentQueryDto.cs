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
namespace org.camunda.bpm.engine.rest.dto.repository
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static true;



	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using DateConverter = org.camunda.bpm.engine.rest.dto.converter.DateConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DeploymentQueryDto : AbstractQueryDto<DeploymentQuery>
	{

	  private const string SORT_BY_ID_VALUE = "id";
	  private const string SORT_BY_NAME_VALUE = "name";
	  private const string SORT_BY_DEPLOYMENT_TIME_VALUE = "deploymentTime";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static DeploymentQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEPLOYMENT_TIME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  private string id;
	  private string name;
	  private string nameLike;
	  private string source;
	  private bool? withoutSource;
	  private DateTime before;
	  private DateTime after;
	  private IList<string> tenantIds;
	  private bool? withoutTenantId;
	  private bool? includeDeploymentsWithoutTenantId;

	  public DeploymentQueryDto()
	  {
	  }

	  public DeploymentQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("id")]
	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
	  }

	  [CamundaQueryParam("name")]
	  public virtual string Name
	  {
		  set
		  {
			this.name = value;
		  }
	  }

	  [CamundaQueryParam("nameLike")]
	  public virtual string NameLike
	  {
		  set
		  {
			this.nameLike = value;
		  }
	  }

	  [CamundaQueryParam("source")]
	  public virtual string Source
	  {
		  set
		  {
			this.source = value;
		  }
	  }

	  [CamundaQueryParam(value : "withoutSource", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutSource
	  {
		  set
		  {
			this.withoutSource = value;
		  }
	  }

	  [CamundaQueryParam(value : "before", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DeploymentBefore
	  {
		  set
		  {
			this.before = value;
		  }
	  }

	  [CamundaQueryParam(value : "after", converter : org.camunda.bpm.engine.rest.dto.converter.DateConverter.class)]
	  public virtual DateTime DeploymentAfter
	  {
		  set
		  {
			this.after = value;
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

	  [CamundaQueryParam(value : "includeDeploymentsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeDeploymentsWithoutTenantId
	  {
		  set
		  {
			this.includeDeploymentsWithoutTenantId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override DeploymentQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RepositoryService.createDeploymentQuery();
	  }

	  protected internal override void applyFilters(DeploymentQuery query)
	  {
		if (withoutSource != null && withoutSource && !string.ReferenceEquals(source, null))
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, "The query parameters \"withoutSource\" and \"source\" cannot be used in combination.");
		}

		if (!string.ReferenceEquals(id, null))
		{
		  query.deploymentId(id);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.deploymentName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.deploymentNameLike(nameLike);
		}
		if (TRUE.Equals(withoutSource))
		{
		  query.deploymentSource(null);
		}
		if (!string.ReferenceEquals(source, null))
		{
		  query.deploymentSource(source);
		}
		if (before != null)
		{
		  query.deploymentBefore(before);
		}
		if (after != null)
		{
		  query.deploymentAfter(after);
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (TRUE.Equals(includeDeploymentsWithoutTenantId))
		{
		  query.includeDeploymentsWithoutTenantId();
		}
	  }

	  protected internal override void applySortBy(DeploymentQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByDeploymentId();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByDeploymentName();
		}
		else if (sortBy.Equals(SORT_BY_DEPLOYMENT_TIME_VALUE))
		{
		  query.orderByDeploymentTime();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}