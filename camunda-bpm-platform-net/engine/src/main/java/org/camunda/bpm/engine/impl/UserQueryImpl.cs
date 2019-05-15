using System;

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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class UserQueryImpl : AbstractQuery<UserQuery, User>, UserQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string[] ids;
	  protected internal string firstName;
	  protected internal string firstNameLike;
	  protected internal string lastName;
	  protected internal string lastNameLike;
	  protected internal string email;
	  protected internal string emailLike;
	  protected internal string groupId;
	  protected internal string procDefId;
	  protected internal string tenantId;

	  public UserQueryImpl()
	  {
	  }

	  public UserQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual UserQuery userId(string id)
	  {
		ensureNotNull("Provided id", id);
		this.id = id;
		return this;
	  }

	  public virtual UserQuery userIdIn(params string[] ids)
	  {
		ensureNotNull("Provided ids", ids);
		this.ids = ids;
		return this;
	  }

	  public virtual UserQuery userFirstName(string firstName)
	  {
		this.firstName = firstName;
		return this;
	  }

	  public virtual UserQuery userFirstNameLike(string firstNameLike)
	  {
		ensureNotNull("Provided firstNameLike", firstNameLike);
		this.firstNameLike = firstNameLike;
		return this;
	  }

	  public virtual UserQuery userLastName(string lastName)
	  {
		this.lastName = lastName;
		return this;
	  }

	  public virtual UserQuery userLastNameLike(string lastNameLike)
	  {
		ensureNotNull("Provided lastNameLike", lastNameLike);
		this.lastNameLike = lastNameLike;
		return this;
	  }

	  public virtual UserQuery userEmail(string email)
	  {
		this.email = email;
		return this;
	  }

	  public virtual UserQuery userEmailLike(string emailLike)
	  {
		ensureNotNull("Provided emailLike", emailLike);
		this.emailLike = emailLike;
		return this;
	  }

	  public virtual UserQuery memberOfGroup(string groupId)
	  {
		ensureNotNull("Provided groupId", groupId);
		this.groupId = groupId;
		return this;
	  }

	  public virtual UserQuery potentialStarter(string procDefId)
	  {
		ensureNotNull("Provided processDefinitionId", procDefId);
		this.procDefId = procDefId;
		return this;

	  }

	  public virtual UserQuery memberOfTenant(string tenantId)
	  {
		ensureNotNull("Provided tenantId", tenantId);
		this.tenantId = tenantId;
		return this;
	  }

	  //sorting //////////////////////////////////////////////////////////

	  public virtual UserQuery orderByUserId()
	  {
		return orderBy(UserQueryProperty_Fields.USER_ID);
	  }

	  public virtual UserQuery orderByUserEmail()
	  {
		return orderBy(UserQueryProperty_Fields.EMAIL);
	  }

	  public virtual UserQuery orderByUserFirstName()
	  {
		return orderBy(UserQueryProperty_Fields.FIRST_NAME);
	  }

	  public virtual UserQuery orderByUserLastName()
	  {
		return orderBy(UserQueryProperty_Fields.LAST_NAME);
	  }

	  //getters //////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual string[] Ids
	  {
		  get
		  {
			return ids;
		  }
	  }
	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
	  }
	  public virtual string FirstNameLike
	  {
		  get
		  {
			return firstNameLike;
		  }
	  }
	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
	  }
	  public virtual string LastNameLike
	  {
		  get
		  {
			return lastNameLike;
		  }
	  }
	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
	  }
	  public virtual string EmailLike
	  {
		  get
		  {
			return emailLike;
		  }
	  }
	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
	  }
	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }
	}

}