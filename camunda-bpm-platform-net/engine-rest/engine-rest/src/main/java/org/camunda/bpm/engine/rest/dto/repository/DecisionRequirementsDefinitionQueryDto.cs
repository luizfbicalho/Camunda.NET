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


	using DecisionRequirementsDefinitionQuery = org.camunda.bpm.engine.repository.DecisionRequirementsDefinitionQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DecisionRequirementsDefinitionQueryDto : AbstractQueryDto<DecisionRequirementsDefinitionQuery>
	{

	  private const string SORT_BY_ID_VALUE = "id";
	  private const string SORT_BY_KEY_VALUE = "key";
	  private const string SORT_BY_NAME_VALUE = "name";
	  private const string SORT_BY_VERSION_VALUE = "version";
	  private const string SORT_BY_DEPLOYMENT_ID_VALUE = "deploymentId";
	  private const string SORT_BY_CATEGORY_VALUE = "category";
	  private const string SORT_BY_TENANT_ID = "tenantId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;

	  static DecisionRequirementsDefinitionQueryDto()
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

	  protected internal string decisionRequirementsDefinitionId;
	  protected internal IList<string> decisionRequirementsDefinitionIdIn;
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

	  public DecisionRequirementsDefinitionQueryDto()
	  {
	  }

	  public DecisionRequirementsDefinitionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("decisionRequirementsDefinitionId")]
	  public virtual string DecisionRequirementsDefinitionId
	  {
		  set
		  {
			this.decisionRequirementsDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionRequirementsDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> DecisionRequirementsDefinitionIdIn
	  {
		  set
		  {
			this.decisionRequirementsDefinitionIdIn = value;
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

	  [CamundaQueryParam(value : "includeDecisionRequirementsDefinitionsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeDecisionRequirementsDefinitionsWithoutTenantId
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

	  protected internal override DecisionRequirementsDefinitionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RepositoryService.createDecisionRequirementsDefinitionQuery();
	  }

	  protected internal override void applyFilters(DecisionRequirementsDefinitionQuery query)
	  {
		if (!string.ReferenceEquals(decisionRequirementsDefinitionId, null))
		{
		  query.decisionRequirementsDefinitionId(decisionRequirementsDefinitionId);
		}
		if (decisionRequirementsDefinitionIdIn != null && decisionRequirementsDefinitionIdIn.Count > 0)
		{
		  query.decisionRequirementsDefinitionIdIn(decisionRequirementsDefinitionIdIn.ToArray());
		}
		if (!string.ReferenceEquals(category, null))
		{
		  query.decisionRequirementsDefinitionCategory(category);
		}
		if (!string.ReferenceEquals(categoryLike, null))
		{
		  query.decisionRequirementsDefinitionCategoryLike(categoryLike);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.decisionRequirementsDefinitionName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.decisionRequirementsDefinitionNameLike(nameLike);
		}
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(key, null))
		{
		  query.decisionRequirementsDefinitionKey(key);
		}
		if (!string.ReferenceEquals(keyLike, null))
		{
		  query.decisionRequirementsDefinitionKeyLike(keyLike);
		}
		if (!string.ReferenceEquals(resourceName, null))
		{
		  query.decisionRequirementsDefinitionResourceName(resourceName);
		}
		if (!string.ReferenceEquals(resourceNameLike, null))
		{
		  query.decisionRequirementsDefinitionResourceNameLike(resourceNameLike);
		}
		if (version != null)
		{
		  query.decisionRequirementsDefinitionVersion(version);
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
		  query.includeDecisionRequirementsDefinitionsWithoutTenantId();
		}
	  }

	  protected internal override void applySortBy(DecisionRequirementsDefinitionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_CATEGORY_VALUE))
		{
		  query.orderByDecisionRequirementsDefinitionCategory();
		}
		else if (sortBy.Equals(SORT_BY_KEY_VALUE))
		{
		  query.orderByDecisionRequirementsDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByDecisionRequirementsDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_VERSION_VALUE))
		{
		  query.orderByDecisionRequirementsDefinitionVersion();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByDecisionRequirementsDefinitionName();
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