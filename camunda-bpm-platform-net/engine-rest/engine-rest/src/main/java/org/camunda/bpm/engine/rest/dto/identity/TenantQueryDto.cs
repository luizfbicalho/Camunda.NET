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
namespace org.camunda.bpm.engine.rest.dto.identity
{

	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using BooleanConverter = org.camunda.bpm.engine.rest.dto.converter.BooleanConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	public class TenantQueryDto : AbstractQueryDto<TenantQuery>
	{

	  private const string SORT_BY_TENANT_ID_VALUE = "id";
	  private const string SORT_BY_TENANT_NAME_VALUE = "name";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;

	  static TenantQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_TENANT_NAME_VALUE);
	  }

	  protected internal string id;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal bool? includingGroupsOfUser;

	  public TenantQueryDto()
	  {
	  }

	  public TenantQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam("userMember")]
	  public virtual string UserMember
	  {
		  set
		  {
			this.userId = value;
		  }
	  }

	  [CamundaQueryParam("groupMember")]
	  public virtual string GroupMember
	  {
		  set
		  {
			this.groupId = value;
		  }
	  }

	  [CamundaQueryParam(value : "includingGroupsOfUser", converter : org.camunda.bpm.engine.rest.dto.converter.BooleanConverter.class)]
	  public virtual bool? IncludingGroupsOfUser
	  {
		  set
		  {
			this.includingGroupsOfUser = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override TenantQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.IdentityService.createTenantQuery();
	  }

	  protected internal override void applyFilters(TenantQuery query)
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  query.tenantId(id);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.tenantName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.tenantNameLike(nameLike);
		}
		if (!string.ReferenceEquals(userId, null))
		{
		  query.userMember(userId);
		}
		if (!string.ReferenceEquals(groupId, null))
		{
		  query.groupMember(groupId);
		}
		if (true.Equals(includingGroupsOfUser))
		{
		  query.includingGroupsOfUser(true);
		}
	  }

	  protected internal override void applySortBy(TenantQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_TENANT_ID_VALUE))
		{
		  query.orderByTenantId();
		}
		else if (sortBy.Equals(SORT_BY_TENANT_NAME_VALUE))
		{
		  query.orderByTenantName();
		}
	  }

	}

}