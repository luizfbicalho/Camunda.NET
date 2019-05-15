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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;

	/// <summary>
	/// A relationship between a tenant and an user or a group.
	/// </summary>
	[Serializable]
	public class TenantMembershipEntity : DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal TenantEntity tenant;
	  protected internal UserEntity user;
	  protected internal GroupEntity group;

	  protected internal string id;

	  public virtual object PersistentState
	  {
		  get
		  {
			// entity is not updatable
			return typeof(TenantMembershipEntity);
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual UserEntity User
	  {
		  get
		  {
			return user;
		  }
		  set
		  {
			this.user = value;
		  }
	  }


	  public virtual GroupEntity Group
	  {
		  get
		  {
			return group;
		  }
		  set
		  {
			this.group = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenant.Id;
		  }
	  }

	  public virtual string UserId
	  {
		  get
		  {
			if (user != null)
			{
			  return user.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string GroupId
	  {
		  get
		  {
			if (group != null)
			{
			  return group.Id;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual TenantEntity Tenant
	  {
		  get
		  {
			return tenant;
		  }
		  set
		  {
			this.tenant = value;
		  }
	  }


	  public override string ToString()
	  {
		return "TenantMembershipEntity [id=" + id + ", tenant=" + tenant + ", user=" + user + ", group=" + group + "]";
	  }

	}

}