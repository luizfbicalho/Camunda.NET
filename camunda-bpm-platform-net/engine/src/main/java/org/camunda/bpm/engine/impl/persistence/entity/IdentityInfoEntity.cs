using System;
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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using Account = org.camunda.bpm.engine.impl.identity.Account;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class IdentityInfoEntity : DbEntity, HasDbRevision, Account
	{

	  private const long serialVersionUID = 1L;

	  public const string TYPE_USERACCOUNT = "account";
	  public const string TYPE_USERINFO = "userinfo";

	  protected internal string id;
	  protected internal int revision;
	  protected internal string type;
	  protected internal string userId;
	  protected internal string key;
	  protected internal string value;
	  protected internal string password;
	  protected internal sbyte[] passwordBytes;
	  protected internal string parentId;
	  protected internal IDictionary<string, string> details;

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["value"] = value;
			persistentState["password"] = passwordBytes;
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
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


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }


	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
		  }
	  }


	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	  public virtual sbyte[] PasswordBytes
	  {
		  get
		  {
			return passwordBytes;
		  }
		  set
		  {
			this.passwordBytes = value;
		  }
	  }


	  public virtual string Password
	  {
		  get
		  {
			return password;
		  }
		  set
		  {
			this.password = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return key;
		  }
	  }

	  public virtual string Username
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual string ParentId
	  {
		  get
		  {
			return parentId;
		  }
		  set
		  {
			this.parentId = value;
		  }
	  }


	  public virtual IDictionary<string, string> Details
	  {
		  get
		  {
			return details;
		  }
		  set
		  {
			this.details = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", type=" + type + ", userId=" + userId + ", key=" + key + ", value=" + value + ", password=" + password + ", parentId=" + parentId + ", details=" + details + "]";
	  }
	}

}