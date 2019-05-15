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

	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using StringArrayConverter = org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class UserQueryDto : AbstractQueryDto<UserQuery>
	{

	  private const string SORT_BY_USER_ID_VALUE = "userId";
	  private const string SORT_BY_USER_FIRSTNAME_VALUE = "firstName";
	  private const string SORT_BY_USER_LASTNAME_VALUE = "lastName";
	  private const string SORT_BY_USER_EMAIL_VALUE = "email";

	  private static readonly IList<string> VALID_SORT_BY_VALUES;
	  static UserQueryDto()
	  {
		VALID_SORT_BY_VALUES = new List<string>();
		VALID_SORT_BY_VALUES.Add(SORT_BY_USER_ID_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_USER_FIRSTNAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_USER_LASTNAME_VALUE);
		VALID_SORT_BY_VALUES.Add(SORT_BY_USER_EMAIL_VALUE);
	  }

	  protected internal string id;
	  protected internal string[] idIn;
	  protected internal string firstName;
	  protected internal string firstNameLike;
	  protected internal string lastName;
	  protected internal string lastNameLike;
	  protected internal string email;
	  protected internal string emailLike;
	  protected internal string memberOfGroup;
	  protected internal string potentialStarter;
	  protected internal string tenantId;

	  public UserQueryDto()
	  {

	  }

	  public UserQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters) : base(objectMapper, queryParameters)
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

	  [CamundaQueryParam(value : "idIn", converter : org.camunda.bpm.engine.rest.dto.converter.StringArrayConverter.class)]
	  public virtual string[] IdIn
	  {
		  set
		  {
			this.idIn = value;
		  }
	  }

	  [CamundaQueryParam("firstName")]
	  public virtual string FirstName
	  {
		  set
		  {
			this.firstName = value;
		  }
	  }

	  [CamundaQueryParam("firstNameLike")]
	  public virtual string FirstNameLike
	  {
		  set
		  {
			this.firstNameLike = value;
		  }
	  }

	  [CamundaQueryParam("lastName")]
	  public virtual string LastName
	  {
		  set
		  {
			this.lastName = value;
		  }
	  }

	  [CamundaQueryParam("lastNameLike")]
	  public virtual string LastNameLike
	  {
		  set
		  {
			this.lastNameLike = value;
		  }
	  }

	  [CamundaQueryParam("email")]
	  public virtual string Email
	  {
		  set
		  {
			this.email = value;
		  }
	  }

	  [CamundaQueryParam("emailLike")]
	  public virtual string EmailLike
	  {
		  set
		  {
			this.emailLike = value;
		  }
	  }

	  [CamundaQueryParam("memberOfGroup")]
	  public virtual string MemberOfGroup
	  {
		  set
		  {
			this.memberOfGroup = value;
		  }
	  }

	  [CamundaQueryParam("potentialStarter")]
	  public virtual string PotentialStarter
	  {
		  set
		  {
			this.potentialStarter = value;
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

	  protected internal override UserQuery createNewQuery(ProcessEngine engine)
	  {
		return engine.IdentityService.createUserQuery();
	  }

	  protected internal override void applyFilters(UserQuery query)
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  query.userId(id);
		}
		if (idIn != null)
		{
		  query.userIdIn(idIn);
		}
		if (!string.ReferenceEquals(firstName, null))
		{
		  query.userFirstName(firstName);
		}
		if (!string.ReferenceEquals(firstNameLike, null))
		{
		  query.userFirstNameLike(firstNameLike);
		}
		if (!string.ReferenceEquals(lastName, null))
		{
		  query.userLastName(lastName);
		}
		if (!string.ReferenceEquals(lastNameLike, null))
		{
		  query.userLastNameLike(lastNameLike);
		}
		if (!string.ReferenceEquals(email, null))
		{
		  query.userEmail(email);
		}
		if (!string.ReferenceEquals(emailLike, null))
		{
		  query.userEmailLike(emailLike);
		}
		if (!string.ReferenceEquals(memberOfGroup, null))
		{
		  query.memberOfGroup(memberOfGroup);
		}
		if (!string.ReferenceEquals(potentialStarter, null))
		{
		  query.potentialStarter(potentialStarter);
		}
		if (!string.ReferenceEquals(tenantId, null))
		{
		  query.memberOfTenant(tenantId);
		}
	  }

	  protected internal override void applySortBy(UserQuery query, string sortBy, IDictionary<string, object> parameters, ProcessEngine engine)
	  {
		if (sortBy.Equals(SORT_BY_USER_ID_VALUE))
		{
		  query.orderByUserId();
		}
		else if (sortBy.Equals(SORT_BY_USER_FIRSTNAME_VALUE))
		{
		  query.orderByUserFirstName();
		}
		else if (sortBy.Equals(SORT_BY_USER_LASTNAME_VALUE))
		{
		  query.orderByUserLastName();
		}
		else if (sortBy.Equals(SORT_BY_USER_EMAIL_VALUE))
		{
		  query.orderByUserEmail();
		}
	  }

	}

}