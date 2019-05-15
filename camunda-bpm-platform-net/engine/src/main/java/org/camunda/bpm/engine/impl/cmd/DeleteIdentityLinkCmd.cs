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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class DeleteIdentityLinkCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string type;

	  protected internal string taskId;

	  protected internal TaskEntity task;

	  public DeleteIdentityLinkCmd(string taskId, string userId, string groupId, string type)
	  {
		validateParams(userId, groupId, type, taskId);
		this.taskId = taskId;
		this.userId = userId;
		this.groupId = groupId;
		this.type = type;
	  }

	  protected internal virtual void validateParams(string userId, string groupId, string type, string taskId)
	  {
		ensureNotNull("taskId", taskId);
		ensureNotNull("type is required when adding a new task identity link", "type", type);

		// Special treatment for assignee and owner: group cannot be used and userId may be null
		if (IdentityLinkType.ASSIGNEE.Equals(type) || IdentityLinkType.OWNER.Equals(type))
		{
		  if (!string.ReferenceEquals(groupId, null))
		  {
			throw new ProcessEngineException("Incompatible usage: cannot use type '" + type + "' together with a groupId");
		  }
		}
		else
		{
		  if (string.ReferenceEquals(userId, null) && string.ReferenceEquals(groupId, null))
		  {
			throw new ProcessEngineException("userId and groupId cannot both be null");
		  }
		}
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);

		TaskManager taskManager = commandContext.TaskManager;
		task = taskManager.findTaskById(taskId);
		ensureNotNull("Cannot find task with id " + taskId, "task", task);

		checkDeleteIdentityLink(task, commandContext);

		if (IdentityLinkType.ASSIGNEE.Equals(type))
		{
		  task.Assignee = null;
		}
		else if (IdentityLinkType.OWNER.Equals(type))
		{
		  task.Owner = null;
		}
		else
		{
		  task.deleteIdentityLink(userId, groupId, type);
		}

		return null;
	  }

	  protected internal virtual void checkDeleteIdentityLink(TaskEntity task, CommandContext commandContext)
	  {
		 foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		 {
		  checker.checkTaskAssign(task);
		 }
	  }

	}

}