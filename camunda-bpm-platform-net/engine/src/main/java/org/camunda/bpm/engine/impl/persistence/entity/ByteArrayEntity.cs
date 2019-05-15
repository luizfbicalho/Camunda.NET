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

	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using ResourceType = org.camunda.bpm.engine.repository.ResourceType;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ByteArrayEntity : DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;

	  private static readonly object PERSISTENTSTATE_NULL = new object();

	  protected internal string id;
	  protected internal int revision;
	  protected internal string name;
	  protected internal sbyte[] bytes;
	  protected internal string deploymentId;
	  protected internal string tenantId;
	  protected internal int? type;
	  protected internal DateTime createTime;
	  protected internal string rootProcessInstanceId;
	  protected internal DateTime removalTime;

	  public ByteArrayEntity()
	  {
	  }

	  public ByteArrayEntity(string name, sbyte[] bytes, ResourceType type, string rootProcessInstanceId, DateTime removalTime) : this(name, bytes, type)
	  {
		this.rootProcessInstanceId = rootProcessInstanceId;
		this.removalTime = removalTime;
	  }

	  public ByteArrayEntity(string name, sbyte[] bytes, ResourceType type) : this(name, bytes)
	  {
		this.type = type.Value;
	  }

	  public ByteArrayEntity(string name, sbyte[] bytes)
	  {
		this.name = name;
		this.bytes = bytes;
	  }

	  public ByteArrayEntity(sbyte[] bytes, ResourceType type)
	  {
		this.bytes = bytes;
		this.type = type.Value;
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

	  public virtual object PersistentState
	  {
		  get
		  {
			return (bytes != null ? bytes : PERSISTENTSTATE_NULL);
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

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


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
		  set
		  {
			this.removalTime = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", name=" + name + ", deploymentId=" + deploymentId + ", tenantId=" + tenantId + ", type=" + type + ", createTime=" + createTime + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + "]";
	  }

	}

}