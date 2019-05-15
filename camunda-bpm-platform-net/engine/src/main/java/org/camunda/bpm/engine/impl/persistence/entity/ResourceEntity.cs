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
	using Resource = org.camunda.bpm.engine.repository.Resource;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ResourceEntity : DbEntity, Resource
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal sbyte[] bytes;
	  protected internal string deploymentId;
	  protected internal bool generated = false;
	  protected internal string tenantId;
	  protected internal int? type;
	  protected internal DateTime createTime;

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


	  public virtual sbyte[] Bytes
	  {
		  get
		  {
			return bytes;
		  }
		  set
		  {
			this.bytes = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			return typeof(ResourceEntity);
		  }
	  }

	  public virtual bool Generated
	  {
		  set
		  {
			this.generated = value;
		  }
		  get
		  {
			return generated;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual int? Type
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


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", name=" + name + ", deploymentId=" + deploymentId + ", generated=" + generated + ", tenantId=" + tenantId + ", type=" + type + ", createTime=" + createTime + "]";
	  }
	}

}