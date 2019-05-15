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

	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class GroupQueryDto : AbstractQueryDto<GroupQuery>
	{

	  private const string SORT_BY_GROUP_ID_VALUE = "id";
	  private const string SORT_BY_GROUP_NAME_VALUE = "name";
	  private const string SORT_BY_GROUP_TYPE_VALUE = "type";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static GroupQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_GROUP_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_GROUP_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_GROUP_TYPE_VALUE);
	  }

	  protected internal string id;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string type;
	  protected internal string member;
	  protected internal string tenantId;

	  public GroupQueryDto()
	  {

	  }

	  public GroupQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam("type")]
	  public virtual string Type
	  {
		  set
		  {
			this.type = value;
		  }
	  }

	  [CamundaQueryParam("member")]
	  public virtual string GroupMember
	  {
		  set
		  {
			this.member = value;
		  }
	  }

	  [CamundaQueryParam("memberOfTenant")]
	  public virtual string MemberOfTenant
	  {
		  set
		  {
			this.tenantId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override GroupQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.IdentityService.createGroupQuery();
	  }

	  protected internal override void applyFilters(GroupQuery query)
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  query.groupId(id);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.groupName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.groupNameLike(nameLike);
		}
		if (!string.ReferenceEquals(type, null))
		{
		  query.groupType(type);
		}
		if (!string.ReferenceEquals(member, null))
		{
		  query.groupMember(member);
		}
		if (!string.ReferenceEquals(tenantId, null))
		{
		  query.memberOfTenant(tenantId);
		}
	  }

	  protected internal override void applySortBy(GroupQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_GROUP_ID_VALUE))
		{
		  query.orderByGroupId();
		}
		else if (sortBy.Equals(SORT_BY_GROUP_NAME_VALUE))
		{
		  query.orderByGroupName();
		}
		else if (sortBy.Equals(SORT_BY_GROUP_TYPE_VALUE))
		{
		  query.orderByGroupType();
		}
	  }

	}

}