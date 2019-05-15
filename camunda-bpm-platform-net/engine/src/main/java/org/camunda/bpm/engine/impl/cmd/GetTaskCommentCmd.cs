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
namespace org.camunda.bpm.engine.impl.cmd
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using Comment = org.camunda.bpm.engine.task.Comment;

	using static org.camunda.bpm.engine.impl.util.EnsureUtil;


	/// <summary>
	/// @author kristin.polenz@camunda.com
	/// </summary>
	[Serializable]
	public class GetTaskCommentCmd : Command<Comment>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string commentId;

	  public GetTaskCommentCmd(string taskId, string commentId)
	  {
		this.taskId = taskId;
		this.commentId = commentId;
	  }

	  public virtual Comment execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);
		ensureNotNull("commentId", commentId);

		return commandContext.CommentManager.findCommentByTaskIdAndCommentId(taskId, commentId);
	  }

	}

}