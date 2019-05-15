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
namespace org.camunda.bpm.engine.rest.dto.authorization
{

	using AuthorizationQuery = org.camunda.bpm.engine.authorization.AuthorizationQuery;
	using IntegerConverter = org.camunda.bpm.engine.rest.dto.converter.IntegerConverter;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class AuthorizationQueryDto : AbstractQueryDto<AuthorizationQuery>
	{

	  private const string SORT_BY_RESOURCE_TYPE = "resourceType";
	  private const string SORT_BY_RESOURCE_ID = "resourceId";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static AuthorizationQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_RESOURCE_TYPE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_RESOURCE_ID);
	  }

	  protected internal string id;
	  protected internal int? type;
	  protected internal string[] userIdIn;
	  protected internal string[] groupIdIn;
	  protected internal int? resourceType;
	  protected internal string resourceId;

	  public AuthorizationQueryDto()
	  {

	  }

	  public AuthorizationQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam(value:"type", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int? Type
	  {
		  set
		  {
			this.type = value;
		  }
	  }

	  [CamundaQueryParam(value:"userIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] UserIdIn
	  {
		  set
		  {
			this.userIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"groupIdIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] GroupIdIn
	  {
		  set
		  {
			this.groupIdIn = value;
		  }
	  }

	  [CamundaQueryParam(value:"resourceType", converter : org.camunda.bpm.engine.rest.dto.converter.IntegerConverter.class)]
	  public virtual int ResourceType
	  {
		  set
		  {
			this.resourceType = value;
		  }
	  }

	  [CamundaQueryParam("resourceId")]
	  public virtual string ResourceId
	  {
		  set
		  {
			this.resourceId = value;
		  }
	  }

	  protected internal override bool isValidSortByValue(string value)
	  {
		return VALID_SORT_BY_VALUES.Contains(value);
	  }

	  protected internal override AuthorizationQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.AuthorizationService.createAuthorizationQuery();
	  }

	  protected internal virtual void applyFilters(AuthorizationQuery query)
	  {

		if (!string.ReferenceEquals(id, null))
		{
		  query.authorizationId(id);
		}
		if (type != null)
		{
		  query.authorizationType(type);
		}
		if (userIdIn != null)
		{
		  query.userIdIn(userIdIn);
		}
		if (groupIdIn != null)
		{
		  query.groupIdIn(groupIdIn);
		}
		if (resourceType != null)
		{
		  query.resourceType(resourceType);
		}
		if (!string.ReferenceEquals(resourceId, null))
		{
		  query.resourceId(resourceId);
		}
	  }

	  protected internal override void applySortBy(AuthorizationQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_RESOURCE_ID))
		{
		  query.orderByResourceId();
		}
		else if (sortBy.Equals(SORT_BY_RESOURCE_TYPE))
		{
		  query.orderByResourceType();
		}
	  }

	}

}