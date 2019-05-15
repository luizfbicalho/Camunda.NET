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

	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterQueryDto : AbstractQueryDto<FilterQuery>
	{

	  public const string SORT_BY_ID_VALUE = "filterId";
	  public const string SORT_BY_RESOURCE_TYPE_VALUE = "resourceType";
	  public const string SORT_BY_NAME_VALUE = "name";
	  public const string SORT_BY_OWNER_VALUE = "owner";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static FilterQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_RESOURCE_TYPE_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_NAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_OWNER_VALUE);
	  }

	  protected internal string filterId;
	  protected internal string resourceType;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string owner;

	  public FilterQueryDto()
	  {

	  }

	  public FilterQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
	  {
	  }

	  [CamundaQueryParam("filterId")]
	  public virtual string FilterId
	  {
		  set
		  {
			this.filterId = value;
		  }
	  }

	  [CamundaQueryParam("resourceType")]
	  public virtual string ResourceType
	  {
		  set
		  {
			this.resourceType = value;
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

	  [CamundaQueryParam("owner")]
	  public virtual string Owner
	  {
		  set
		  {
			this.owner = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override FilterQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.FilterService.createFilterQuery();
	  }

	  protected internal virtual void applyFilters(FilterQuery query)
	  {
		if (!string.ReferenceEquals(filterId, null))
		{
		  query.filterId(filterId);
		}
		if (!string.ReferenceEquals(resourceType, null))
		{
		  query.filterResourceType(resourceType);
		}
		if (!string.ReferenceEquals(name, null))
		{
		  query.filterName(name);
		}
		if (!string.ReferenceEquals(nameLike, null))
		{
		  query.filterNameLike(nameLike);
		}
		if (!string.ReferenceEquals(owner, null))
		{
		  query.filterOwner(owner);
		}
	  }

	  protected internal virtual void applySortBy(FilterQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_ID_VALUE))
		{
		  query.orderByFilterId();
		}
		else if (sortBy.Equals(SORT_BY_RESOURCE_TYPE_VALUE))
		{
		  query.orderByFilterResourceType();
		}
		else if (sortBy.Equals(SORT_BY_NAME_VALUE))
		{
		  query.orderByFilterName();
		}
		else if (sortBy.Equals(SORT_BY_OWNER_VALUE))
		{
		  query.orderByFilterOwner();
		}
	  }

	}

}