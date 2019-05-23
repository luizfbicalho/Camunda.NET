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


	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringListConverter = org.camunda.bpm.engine.rest.dto.converter.StringListConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class ProcessDefinitionQueryDto : AbstractQueryDto<ProcessDefinitionQuery>
	{

	  private const string SORT_BY_CATEGORY_VALUE = "category";
	  private const string SORT_BY_KEY_VALUE = "key";
	  private const string SORT_BY_ID_VALUE = "id";
	  private const string SORT_BY_NAME_VALUE = "name";
	  private const string SORT_BY_VERSION_VALUE = "version";
	  private const string SORT_BY_DEPLOYMENT_ID_VALUE = "deploymentId";
	  private const string SORT_BY_TENANT_ID = "tenantId";
	  private const string SORT_BY_VERSION_TAG = "versionTag";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static ProcessDefinitionQueryDto()
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

	  private string processDefinitionId;
	  private IList<string> processDefinitionIdIn;
	  private string category;
	  private string categoryLike;
	  private string name;
	  private string nameLike;
	  private string deploymentId;
	  private string key;
	  private string keyLike;
	  private int? version;
	  private bool? latestVersion;
	  private string resourceName;
	  private string resourceNameLike;
	  private string startableBy;
	  private bool? active;
	  private bool? suspended;
	  private string incidentId;
	  private string incidentType;
	  private string incidentMessage;
	  private string incidentMessageLike;
	  private IList<string> tenantIds;
	  private bool? withoutTenantId;
	  private bool? includeDefinitionsWithoutTenantId;
	  private string versionTag;
	  private string versionTagLike;
	  private IList<string> keys;
	  private bool? startableInTasklist;
	  private bool? notStartableInTasklist;
	  private bool? startablePermissionCheck;

	  public ProcessDefinitionQueryDto()
	  {

	  }

	  public ProcessDefinitionQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("processDefinitionId")]
	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }

	  [CamundaQueryParam(value : "processDefinitionIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> ProcessDefinitionIdIn
	  {
		  set
		  {
			this.processDefinitionIdIn = value;
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


	  [CamundaQueryParam(value : "keysIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringListConverter.class)]
	  public virtual IList<string> KeysIn
	  {
		  set
		  {
			this.keys = value;
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

	  /// @deprecated use <seealso cref="setVersion(Integer)"/> 
	  [Obsolete("use <seealso cref=\"setVersion(Integer)\"/>"), CamundaQueryParam(value : "ver", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? Ver
	  {
		  set
		  {
			Version = value;
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

	  /// @deprecated use <seealso cref="setLatestVersion(Boolean)"/> 
	  [Obsolete("use <seealso cref=\"setLatestVersion(Boolean)\"/>"), CamundaQueryParam(value : "latest", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
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

	  [CamundaQueryParam("startableBy")]
	  public virtual string StartableBy
	  {
		  set
		  {
			this.startableBy = value;
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

	  [CamundaQueryParam(value : "incidentId")]
	  public virtual string IncidentId
	  {
		  set
		  {
			this.incidentId = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentType")]
	  public virtual string IncidentType
	  {
		  set
		  {
			this.incidentType = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentMessage")]
	  public virtual string IncidentMessage
	  {
		  set
		  {
			this.incidentMessage = value;
		  }
	  }

	  [CamundaQueryParam(value : "incidentMessageLike")]
	  public virtual string IncidentMessageLike
	  {
		  set
		  {
			this.incidentMessageLike = value;
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

	  [CamundaQueryParam(value : "includeProcessDefinitionsWithoutTenantId", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludeProcessDefinitionsWithoutTenantId
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

	  [CamundaQueryParam(value : "startableInTasklist", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? StartableInTasklist
	  {
		  set
		  {
			this.startableInTasklist = value;
		  }
	  }

	  [CamundaQueryParam(value : "notStartableInTasklist", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? NotStartableInTasklist
	  {
		  set
		  {
			this.notStartableInTasklist = value;
		  }
	  }

	  [CamundaQueryParam(value : "startablePermissionCheck", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? StartablePermissionCheck
	  {
		  set
		  {
			this.startablePermissionCheck = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override ProcessDefinitionQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.RepositoryService.createProcessDefinitionQuery();
	  }

	  protected internal override void applyFilters(ProcessDefinitionQuery query)
	  {
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  query.processDefinitionId(processDefinitionId);
		}
		if (processDefinitionIdIn != null && processDefinitionIdIn.Count > 0)
		{
		  query.processDefinitionIdIn(processDefinitionIdIn.ToArray());
		}
		if (!string.ReferenceEquals(category, null))
		{
		  query.processDefinitionCategory(category);
		}
		if (!string.ReferenceEquals(categoryLike, null))
		{
		  query.processDefinitionCategoryLike(categoryLike);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.processDefinitionName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.processDefinitionNameLike(nameLike);
		}
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  query.deploymentId(deploymentId);
		}
		if (!string.ReferenceEquals(key, null))
		{
		  query.processDefinitionKey(key);
		}
		if (!string.ReferenceEquals(keyLike, null))
		{
		  query.processDefinitionKeyLike(keyLike);
		}

		if (keys != null && keys.Count > 0)
		{
		  query.processDefinitionKeysIn(keys.ToArray());
		}
		if (version != null)
		{
		  query.processDefinitionVersion(version);
		}
		if (TRUE.Equals(latestVersion))
		{
		  query.latestVersion();
		}
		if (!string.ReferenceEquals(resourceName, null))
		{
		  query.processDefinitionResourceName(resourceName);
		}
		if (!string.ReferenceEquals(resourceNameLike, null))
		{
		  query.processDefinitionResourceNameLike(resourceNameLike);
		}
		if (!string.ReferenceEquals(startableBy, null))
		{
		  query.startableByUser(startableBy);
		}
		if (TRUE.Equals(active))
		{
		  query.active();
		}
		if (TRUE.Equals(suspended))
		{
		  query.suspended();
		}
		if (!string.ReferenceEquals(incidentId, null))
		{
		  query.incidentId(incidentId);
		}
		if (!string.ReferenceEquals(incidentType, null))
		{
		  query.incidentType(incidentType);
		}
		if (!string.ReferenceEquals(incidentMessage, null))
		{
		  query.incidentMessage(incidentMessage);
		}
		if (!string.ReferenceEquals(incidentMessageLike, null))
		{
		  query.incidentMessageLike(incidentMessageLike);
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
		  query.includeProcessDefinitionsWithoutTenantId();
		}
		if (!string.ReferenceEquals(versionTag, null))
		{
		  query.versionTag(versionTag);
		}
		if (!string.ReferenceEquals(versionTagLike, null))
		{
		  query.versionTagLike(versionTagLike);
		}
		if (TRUE.Equals(startableInTasklist))
		{
		  query.startableInTasklist();
		}
		if (TRUE.Equals(notStartableInTasklist))
		{
		  query.notStartableInTasklist();
		}
		if (TRUE.Equals(startablePermissionCheck))
		{
		  query.startablePermissionCheck();
		}

	  }

	  protected internal override void applySortBy(ProcessDefinitionQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_CATEGORY_VALUE))
		{
		  query.orderByProcessDefinitionCategory();
		}
		else if (sortBy.Equals(SORT_BY_KEY_VALUE))
		{
		  query.orderByProcessDefinitionKey();
		}
		else if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByProcessDefinitionId();
		}
		else if (sortBy.Equals(SORT_BY_VERSION_VALUE))
		{
		  query.orderByProcessDefinitionVersion();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByProcessDefinitionName();
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