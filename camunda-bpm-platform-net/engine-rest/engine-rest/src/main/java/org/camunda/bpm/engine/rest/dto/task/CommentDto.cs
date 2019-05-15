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
	using Comment = org.camunda.bpm.engine.task.Comment;

	public class CommentDto : LinkableDto
	{

	  private string id;
	  private string userId;
	  private DateTime time;
	  private string taskId;
	  private string message;
	  private DateTime removalTime;
	  private string rootProcessInstanceId;

	  public CommentDto()
	  {
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
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


	  public virtual DateTime Time
	  {
		  get
		  {
			return time;
		  }
		  set
		  {
			this.time = value;
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


	  public virtual string Message
	  {
		  get
		  {
			return message;
		  }
		  set
		  {
			this.message = value;
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


	  public static CommentDto fromComment(Comment comment)
	  {
		CommentDto dto = new CommentDto();
		dto.id = comment.Id;
		dto.userId = comment.UserId;
		dto.time = comment.Time;
		dto.taskId = comment.TaskId;
		dto.message = comment.FullMessage;
		dto.removalTime = comment.RemovalTime;
		dto.rootProcessInstanceId = comment.RootProcessInstanceId;

		return dto;
	  }
	}

}