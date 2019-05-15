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
namespace org.camunda.bpm.engine.rest.dto.task
{

	using Attachment = org.camunda.bpm.engine.task.Attachment;

	public class AttachmentDto : LinkableDto
	{

	  private string id;
	  private string name;
	  private string description;
	  private string taskId;
	  private string type;
	  private string url;
	  private DateTime createTime;
	  private DateTime removalTime;
	  private string rootProcessInstanceId;

	  public AttachmentDto()
	  {
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


	  public static AttachmentDto fromAttachment(Attachment attachment)
	  {
		AttachmentDto dto = new AttachmentDto();
		dto.id = attachment.Id;
		dto.name = attachment.Name;
		dto.type = attachment.Type;
		dto.description = attachment.Description;
		dto.taskId = attachment.TaskId;
		dto.url = attachment.Url;
		dto.createTime = attachment.CreateTime;
		dto.removalTime = attachment.RemovalTime;
		dto.rootProcessInstanceId = attachment.RootProcessInstanceId;
		return dto;
	  }
	}

}