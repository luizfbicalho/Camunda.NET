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
	using HistoricEntity = org.camunda.bpm.engine.impl.db.HistoricEntity;
	using Attachment = org.camunda.bpm.engine.task.Attachment;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class AttachmentEntity : Attachment, DbEntity, HasDbRevision, HistoricEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string name;
	  protected internal string description;
	  protected internal string type;
	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string url;
	  protected internal string contentId;
	  protected internal ByteArrayEntity content;
	  protected internal string tenantId;
	  protected internal DateTime createTime;
	  protected internal string rootProcessInstanceId;
	  protected internal DateTime removalTime;

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = name;
			persistentState["description"] = description;
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


	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
		  set
		  {
			this.description = value;
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


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string Url
	  {
		  get
		  {
			return url;
		  }
		  set
		  {
			this.url = value;
		  }
	  }


	  public virtual string ContentId
	  {
		  get
		  {
			return contentId;
		  }
		  set
		  {
			this.contentId = value;
		  }
	  }


	  public virtual ByteArrayEntity Content
	  {
		  get
		  {
			return content;
		  }
		  set
		  {
			this.content = value;
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
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", name=" + name + ", description=" + description + ", type=" + type + ", taskId=" + taskId + ", processInstanceId=" + processInstanceId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + ", url=" + url + ", contentId=" + contentId + ", content=" + content + ", tenantId=" + tenantId + ", createTime=" + createTime + "]";
	  }
	}

}