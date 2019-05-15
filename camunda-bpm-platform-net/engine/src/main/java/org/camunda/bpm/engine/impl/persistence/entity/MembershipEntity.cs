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
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class MembershipEntity : DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal UserEntity user;
	  protected internal GroupEntity group;

	  /// <summary>
	  /// To handle a MemberhipEntity in the cache, an id is necessary.
	  /// Even though it is not going to be persisted in the database.
	  /// </summary>
	  protected internal string id;

	  public virtual object PersistentState
	  {
		  get
		  {
			// membership is not updatable
			return typeof(MembershipEntity);
		  }
	  }
	  public virtual string Id
	  {
		  get
		  {
			// For the sake of Entity caching the id is necessary
			return id;
		  }
		  set
		  {
			// For the sake of Entity caching the value is necessary
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

	  // required for mybatis
	  public virtual string UserId
	  {
		  get
		  {
			  return user.Id;
		  }
	  }

	  // required for mybatis
	  public virtual string GroupId
	  {
		  get
		  {
			  return group.Id;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[user=" + user + ", group=" + group + "]";
	  }
	}

}