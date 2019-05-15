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

	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	[Serializable]
	public abstract class TenantQueryImpl : AbstractQuery<TenantQuery, Tenant>, TenantQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string[] ids;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string userId;
	  protected internal string groupId;
	  protected internal bool includingGroups = false;

	  public TenantQueryImpl()
	  {
	  }

	  public TenantQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual TenantQuery tenantId(string id)
	  {
		ensureNotNull("tenant ud", id);
		this.id = id;
		return this;
	  }

	  public virtual TenantQuery tenantIdIn(params string[] ids)
	  {
		ensureNotNull("tenant ids", (object[]) ids);
		this.ids = ids;
		return this;
	  }

	  public virtual TenantQuery tenantName(string name)
	  {
		ensureNotNull("tenant name", name);
		this.name = name;
		return this;
	  }

	  public virtual TenantQuery tenantNameLike(string nameLike)
	  {
		ensureNotNull("tenant name like", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual TenantQuery userMember(string userId)
	  {
		ensureNotNull("user id", userId);
		this.userId = userId;
		return this;
	  }

	  public virtual TenantQuery groupMember(string groupId)
	  {
		ensureNotNull("group id", groupId);
		this.groupId = groupId;
		return this;
	  }

	  public virtual TenantQuery includingGroupsOfUser(bool includingGroups)
	  {
		this.includingGroups = includingGroups;
		return this;
	  }

	  //sorting ////////////////////////////////////////////////////////

	  public virtual TenantQuery orderByTenantId()
	  {
		return orderBy(TenantQueryProperty_Fields.GROUP_ID);
	  }

	  public virtual TenantQuery orderByTenantName()
	  {
		return orderBy(TenantQueryProperty_Fields.NAME);
	  }

	  //getters ////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }

	  public virtual string[] Ids
	  {
		  get
		  {
			return ids;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }

	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
	  }

	  public virtual bool IncludingGroups
	  {
		  get
		  {
			return includingGroups;
		  }
	  }

	}

}