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


	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionQueryDto : AbstractQueryDto<CaseDefinitionQuery>
	{

	  private const string SORT_BY_ID_VALUE = "id";
	  private const string SORT_BY_KEY_VALUE = "key";
	  private const string SORT_BY_NAME_VALUE = "name";
	  private const string SORT_BY_VERSION_VALUE = "version";
	  private const string SORT_BY_DEPLOYMENT_ID_VALUE = "deploymentId";
	  private const string SORT_BY_CATEGORY_VALUE = "category";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;

	  static CaseDefinitionQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();

		VALID_SORT_BY_VALUES.Add(SORT_BY_CATEGORY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VERSION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEPLOYMENT_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
	  }

	  protected internal string caseDefinitionId;
	  protected internal IList<string> caseDefinitionIdIn;
	  protected internal string category;
	  protected internal string categoryLike;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string deploymentId;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string resourceName;
	  protected internal string resourceNameLike;
	  protected internal int? version;
	  protected internal bool? latestVersion;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;
	  protected internal bool? includeDefinitionsWithoutTenantId;

	  public CaseDefinitionQueryDto()
	  {
	  }

	  public CaseDefinitionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("caseDefinitionId")]
	  public virtual string CaseDefinitionId
	  {
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "caseDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> CaseDefinitionIdIn
	  {
		  set
		  {
			this.caseDefinitionIdIn = value;
		  }
	  }

	  [CamundaQueryParam("category")]
	  public virtual string Category
	  {
		  set
		  {
			this.category = value;
		  }
	  }

	  [CamundaQueryParam("categoryLike")]
	  public virtual string CategoryLike
	  {
		  set
		  {
			this.categoryLike = value;
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

	  [CamundaQueryParam("deploymentId")]
	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
		  }
	  }

	  [CamundaQueryParam("key")]
	  public virtual string Key
	  {
		  set
		  {
			this.key = value;
		  }
	  }

	  [CamundaQueryParam("keyLike")]
	  public virtual string KeyLike
	  {
		  set
		  {
			this.keyLike = value;
		  }
	  }

	  [CamundaQueryParam("resourceName")]
	  public virtual string ResourceName
	  {
		  set
		  {
			this.resourceName = value;
		  }
	  }

	  [CamundaQueryParam("resourceNameLike")]
	  public virtual string ResourceNameLike
	  {
		  set
		  {
			this.resourceNameLike = value;
		  }
	  }

	  [CamundaQueryParam(value : "version", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? Version
	  {
		  set
		  {
			this.version = value;
		  }
	  }

	  /// @deprecated use <seealso cref="#setLatestVersion(Boolean)"/> 
	  [Obsolete("use <seealso cref="#setLatestVersion(bool?)"/>"), CamundaQueryParam(value : "latest", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? Latest
	  {
		  set
		  {
			LatestVersion = value;
		  }
	  }

	  [CamundaQueryParam(value : "latestVersion", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? LatestVersion
	  {
		  set
		  {
			this.latestVersion = value;
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

	  [CamundaQueryParam(value : "includeCaseDefinitionsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeCaseDefinitionsWithoutTenantId
	  {
		  set
		  {
			this.includeDefinitionsWithoutTenantId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override CaseDefinitionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RepositoryService.createCaseDefinitionQuery();
	  }

	  protected internal override void applyFilters(CaseDefinitionQuery query)
	  {
		if (!string.ReferenceEquals(caseDefinitionId, null))
		{
		  query.caseDefinitionId(caseDefinitionId);
		}
		if (caseDefinitionIdIn != null && caseDefinitionIdIn.Count > 0)
		{
		  query.caseDefinitionIdIn(caseDefinitionIdIn.ToArray());
		}
		if (!string.ReferenceEquals(category, null))
		{
		  query.caseDefinitionCategory(category);
		}
		if (!string.ReferenceEquals(categoryLike, null))
		{
		  query.caseDefinitionCategoryLike(categoryLike);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.caseDefinitionName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.caseDefinitionNameLike(nameLike);
		}
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(key, null))
		{
		  query.caseDefinitionKey(key);
		}
		if (!string.ReferenceEquals(keyLike, null))
		{
		  query.caseDefinitionKeyLike(keyLike);
		}
		if (!string.ReferenceEquals(resourceName, null))
		{
		  query.caseDefinitionResourceName(resourceName);
		}
		if (!string.ReferenceEquals(resourceNameLike, null))
		{
		  query.caseDefinitionResourceNameLike(resourceNameLike);
		}
		if (version != null)
		{
		  query.caseDefinitionVersion(version);
		}
		if (TRUE.Equals(latestVersion))
		{
		  query.latestVersion();
		}
		if (tenantIds != null && tenantIds.Count > 0)
		{
		  query.tenantIdIn(tenantIds.ToArray());
		}
		if (TRUE.Equals(withoutTenantId))
		{
		  query.withoutTenantId();
		}
		if (TRUE.Equals(includeDefinitionsWithoutTenantId))
		{
		  query.includeCaseDefinitionsWithoutTenantId();
		}
	  }

	  protected internal override void applySortBy(CaseDefinitionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_CATEGORY_VALUE))
		{
		  query.orderByCaseDefinitionCategory();
		}
		else if (sortBy.Equals(SORT_BY_KEY_VALUE))
		{
		  query.orderByCaseDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByCaseDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_VERSION_VALUE))
		{
		  query.orderByCaseDefinitionVersion();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByCaseDefinitionName();
		}
		else if (sortBy.Equals(SORT_BY_DEPLOYMENT_ID_VALUE))
		{
		  query.orderByDeploymentId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
	  }

	}

}