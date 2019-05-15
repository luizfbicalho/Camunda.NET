using System;
using System.Collections.Generic;
using System.Text;

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
	using HistoricEntity = org.camunda.bpm.engine.impl.db.HistoricEntity;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using Event = org.camunda.bpm.engine.task.Event;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class CommentEntity : Comment, Event, DbEntity, HistoricEntity
	{

	  private const long serialVersionUID = 1L;

	  public const string TYPE_EVENT = "event";
	  public const string TYPE_COMMENT = "comment";

	  protected internal string id;

	  // If comments would be removeable, revision needs to be added!

	  protected internal string type;
	  protected internal string userId;
	  protected internal DateTime time;
	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string action;
	  protected internal string message;
	  protected internal string fullMessage;
	  protected internal string tenantId;
	  protected internal string rootProcessInstanceId;
	  protected internal DateTime removalTime;

	  public virtual object PersistentState
	  {
		  get
		  {
			return typeof(CommentEntity);
		  }
	  }

	  public virtual sbyte[] FullMessageBytes
	  {
		  get
		  {
			return (!string.ReferenceEquals(fullMessage, null) ? fullMessage.GetBytes() : null);
		  }
		  set
		  {
			fullMessage = (value != null ? StringHelper.NewString(value) : null);
		  }
	  }


	  public static string MESSAGE_PARTS_MARKER = "_|_";

	  public virtual string[] Message
	  {
		  set
		  {
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string part in value)
			{
			  if (!string.ReferenceEquals(part, null))
			  {
				stringBuilder.Append(part.Replace(MESSAGE_PARTS_MARKER, " | "));
				stringBuilder.Append(MESSAGE_PARTS_MARKER);
			  }
			  else
			  {
				stringBuilder.Append("null");
				stringBuilder.Append(MESSAGE_PARTS_MARKER);
			  }
			}
			for (int i = 0; i < MESSAGE_PARTS_MARKER.Length; i++)
			{
			  stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			message = stringBuilder.ToString();
		  }
		  get
		  {
			return message;
		  }
	  }

	  public virtual IList<string> MessageParts
	  {
		  get
		  {
			if (string.ReferenceEquals(message, null))
			{
			  return null;
			}
			IList<string> messageParts = new List<string>();
			StringTokenizer tokenizer = new StringTokenizer(message, MESSAGE_PARTS_MARKER);
			while (tokenizer.hasMoreTokens())
			{
			  string nextToken = tokenizer.nextToken();
			  if ("null".Equals(nextToken))
			  {
				messageParts.Add(null);
			  }
			  else
			  {
				messageParts.Add(nextToken);
			  }
			}
			return messageParts;
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
		  set
		  {
			this.message = value;
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


	  public virtual string FullMessage
	  {
		  get
		  {
			return fullMessage;
		  }
		  set
		  {
			this.fullMessage = value;
		  }
	  }


	  public virtual string Action
	  {
		  get
		  {
			return action;
		  }
		  set
		  {
			this.action = value;
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
		return this.GetType().Name + "[id=" + id + ", type=" + type + ", userId=" + userId + ", time=" + time + ", taskId=" + taskId + ", processInstanceId=" + processInstanceId + ", rootProcessInstanceId=" + rootProcessInstanceId + ", removalTime=" + removalTime + ", action=" + action + ", message=" + message + ", fullMessage=" + fullMessage + ", tenantId=" + tenantId + "]";
	  }
	}

}