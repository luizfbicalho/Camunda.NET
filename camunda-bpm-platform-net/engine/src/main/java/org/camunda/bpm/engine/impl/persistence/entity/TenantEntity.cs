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

	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;

	[Serializable]
	public class TenantEntity : Tenant, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;

	  protected internal int revision;

	  public TenantEntity()
	  {
	  }

	  public TenantEntity(string id)
	  {
		this.id = id;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = name;
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


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
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


	  public override string ToString()
	  {
		return "TenantEntity [id=" + id + ", name=" + name + ", revision=" + revision + "]";
	  }

	}

}