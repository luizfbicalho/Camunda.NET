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

	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class GroupQueryImpl : AbstractQuery<GroupQuery, Group>, GroupQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string[] ids;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string type;
	  protected internal string userId;
	  protected internal string procDefId;
	  protected internal string tenantId;

	  public GroupQueryImpl()
	  {
	  }

	  public GroupQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual GroupQuery groupId(string id)
	  {
		ensureNotNull("Provided id", id);
		this.id = id;
		return this;
	  }

	  public virtual GroupQuery groupIdIn(params string[] ids)
	  {
		ensureNotNull("Provided ids", (object[]) ids);
		this.ids = ids;
		return this;
	  }

	  public virtual GroupQuery groupName(string name)
	  {
		ensureNotNull("Provided name", name);
		this.name = name;
		return this;
	  }

	  public virtual GroupQuery groupNameLike(string nameLike)
	  {
		ensureNotNull("Provided nameLike", nameLike);
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual GroupQuery groupType(string type)
	  {
		ensureNotNull("Provided type", type);
		this.type = type;
		return this;
	  }

	  public virtual GroupQuery groupMember(string userId)
	  {
		ensureNotNull("Provided userId", userId);
		this.userId = userId;
		return this;
	  }

	  public virtual GroupQuery potentialStarter(string procDefId)
	  {
		ensureNotNull("Provided processDefinitionId", procDefId);
		this.procDefId = procDefId;
		return this;
	  }

	  public virtual GroupQuery memberOfTenant(string tenantId)
	  {
		ensureNotNull("Provided tenantId", tenantId);
		this.tenantId = tenantId;
		return this;
	  }

	  //sorting ////////////////////////////////////////////////////////

	  public virtual GroupQuery orderByGroupId()
	  {
		return orderBy(GroupQueryProperty_Fields.GROUP_ID);
	  }

	  public virtual GroupQuery orderByGroupName()
	  {
		return orderBy(GroupQueryProperty_Fields.NAME);
	  }

	  public virtual GroupQuery orderByGroupType()
	  {
		return orderBy(GroupQueryProperty_Fields.TYPE);
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
	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }
	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }
	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }
	  public virtual string[] Ids
	  {
		  get
		  {
			return ids;
		  }
	  }
	}

}