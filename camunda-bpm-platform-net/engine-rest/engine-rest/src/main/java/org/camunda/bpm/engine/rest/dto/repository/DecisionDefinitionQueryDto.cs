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


	using DecisionDefinitionQuery = org.camunda.bpm.engine.repository.DecisionDefinitionQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class DecisionDefinitionQueryDto : AbstractQueryDto<DecisionDefinitionQuery>
	{

	  private const string SORT_BY_ID_VALUE = "id";
	  private const string SORT_BY_KEY_VALUE = "key";
	  private const string SORT_BY_NAME_VALUE = "name";
	  private const string SORT_BY_VERSION_VALUE = "version";
	  private const string SORT_BY_DEPLOYMENT_ID_VALUE = "deploymentId";
	  private const string SORT_BY_CATEGORY_VALUE = "category";
	  private const string SORT_BY_TENANT_ID = "tenantId";
	  private const string SORT_BY_VERSION_TAG = "versionTag";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;

	  static DecisionDefinitionQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();

		VALID_SORT_BY_VALUES.Add(SORT_BY_CATEGORY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_KEY_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VERSION_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_DEPLOYMENT_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID);
		VALID_SORT_BY_VALUES.Add(SORT_BY_VERSION_TAG);
	  }

	  protected internal string decisionDefinitionId;
	  protected internal IList<string> decisionDefinitionIdIn;
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
	  protected internal string decisionRequirementsDefinitionId;
	  protected internal string decisionRequirementsDefinitionKey;
	  protected internal bool? withoutDecisionRequirementsDefinition;
	  protected internal IList<string> tenantIds;
	  protected internal bool? withoutTenantId;
	  protected internal bool? includeDefinitionsWithoutTenantId;
	  private string versionTag;
	  private string versionTagLike;

	  public DecisionDefinitionQueryDto()
	  {
	  }

	  public DecisionDefinitionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("decisionDefinitionId")]
	  public virtual string DecisionDefinitionId
	  {
		  set
		  {
			this.decisionDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> DecisionDefinitionIdIn
	  {
		  set
		  {
			this.decisionDefinitionIdIn = value;
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

	  [CamundaQueryParam(value : "decisionRequirementsDefinitionId")]
	  public virtual string DecisionRequirementsDefinitionId
	  {
		  set
		  {
			this.decisionRequirementsDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "decisionRequirementsDefinitionKey")]
	  public virtual string DecisionRequirementsDefinitionKey
	  {
		  set
		  {
			this.decisionRequirementsDefinitionKey = value;
		  }
	  }

	  [CamundaQueryParam(value : "withoutDecisionRequirementsDefinition", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? WithoutDecisionRequirementsDefinition
	  {
		  set
		  {
			this.withoutDecisionRequirementsDefinition = value;
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

	  [CamundaQueryParam(value : "includeDecisionDefinitionsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeDecisionDefinitionsWithoutTenantId
	  {
		  set
		  {
			this.includeDefinitionsWithoutTenantId = value;
		  }
	  }

	  [CamundaQueryParam(value : "versionTag")]
	  public virtual string VersionTag
	  {
		  set
		  {
			this.versionTag = value;
		  }
	  }

	  [CamundaQueryParam(value : "versionTagLike")]
	  public virtual string VersionTagLike
	  {
		  set
		  {
			this.versionTagLike = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override DecisionDefinitionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RepositoryService.createDecisionDefinitionQuery();
	  }

	  protected internal override void applyFilters(DecisionDefinitionQuery query)
	  {
		if (!string.ReferenceEquals(decisionDefinitionId, null))
		{
		  query.decisionDefinitionId(decisionDefinitionId);
		}
		if (decisionDefinitionIdIn != null && decisionDefinitionIdIn.Count > 0)
		{
		  query.decisionDefinitionIdIn(decisionDefinitionIdIn.ToArray());
		}
		if (!string.ReferenceEquals(category, null))
		{
		  query.decisionDefinitionCategory(category);
		}
		if (!string.ReferenceEquals(categoryLike, null))
		{
		  query.decisionDefinitionCategoryLike(categoryLike);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.decisionDefinitionName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.decisionDefinitionNameLike(nameLike);
		}
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(key, null))
		{
		  query.decisionDefinitionKey(key);
		}
		if (!string.ReferenceEquals(keyLike, null))
		{
		  query.decisionDefinitionKeyLike(keyLike);
		}
		if (!string.ReferenceEquals(resourceName, null))
		{
		  query.decisionDefinitionResourceName(resourceName);
		}
		if (!string.ReferenceEquals(resourceNameLike, null))
		{
		  query.decisionDefinitionResourceNameLike(resourceNameLike);
		}
		if (version != null)
		{
		  query.decisionDefinitionVersion(version);
		}
		if (TRUE.Equals(latestVersion))
		{
		  query.latestVersion();
		}
		if (!string.ReferenceEquals(decisionRequirementsDefinitionId, null))
		{
		  query.decisionRequirementsDefinitionId(decisionRequirementsDefinitionId);
		}
		if (!string.ReferenceEquals(decisionRequirementsDefinitionKey, null))
		{
		  query.decisionRequirementsDefinitionKey(decisionRequirementsDefinitionKey);
		}
		if (TRUE.Equals(withoutDecisionRequirementsDefinition))
		{
		  query.withoutDecisionRequirementsDefinition();
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
		  query.includeDecisionDefinitionsWithoutTenantId();
		}
		if (!string.ReferenceEquals(versionTag, null))
		{
		  query.versionTag(versionTag);
		}
		if (!string.ReferenceEquals(versionTagLike, null))
		{
		  query.versionTagLike(versionTagLike);
		}
	  }

	  protected internal override void applySortBy(DecisionDefinitionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_CATEGORY_VALUE))
		{
		  query.orderByDecisionDefinitionCategory();
		}
		else if (sortBy.Equals(SORT_BY_KEY_VALUE))
		{
		  query.orderByDecisionDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByDecisionDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_VERSION_VALUE))
		{
		  query.orderByDecisionDefinitionVersion();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByDecisionDefinitionName();
		}
		else if (sortBy.Equals(SORT_BY_DEPLOYMENT_ID_VALUE))
		{
		  query.orderByDeploymentId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_ID))
		{
		  query.orderByTenantId();
		}
		else if (sortBy.Equals(SORT_BY_VERSION_TAG))
		{
		  query.orderByVersionTag();
		}
	  }

	}

}