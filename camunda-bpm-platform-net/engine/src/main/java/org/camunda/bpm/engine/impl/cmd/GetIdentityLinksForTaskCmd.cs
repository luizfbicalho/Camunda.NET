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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using IdentityLinkEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityLinkEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class GetIdentityLinksForTaskCmd : Command<IList<IdentityLink>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;

	  public GetIdentityLinksForTaskCmd(string taskId)
	  {
		this.taskId = taskId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes" }) public java.util.List<org.camunda.bpm.engine.task.IdentityLink> execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<IdentityLink> execute(CommandContext commandContext)
	  {
		ensureNotNull("taskId", taskId);

		TaskManager taskManager = commandContext.TaskManager;
		TaskEntity task = taskManager.findTaskById(taskId);
		ensureNotNull("Cannot find task with id " + taskId, "task", task);

		checkGetIdentityLink(task, commandContext);

		IList<IdentityLink> identityLinks = (System.Collections.IList) task.IdentityLinks;

		// assignee is not part of identity links in the db.
		// so if there is one, we add it here.
		// @Tom: we discussed this long on skype and you agreed ;-)
		// an assignee *is* an identityLink, and so must it be reflected in the API
		//
		// Note: we cant move this code to the TaskEntity (which would be cleaner),
		// since the task.delete cascased to all associated identityLinks
		// and of course this leads to exception while trying to delete a non-existing identityLink
		if (!string.ReferenceEquals(task.Assignee, null))
		{
		  IdentityLinkEntity identityLink = new IdentityLinkEntity();
		  identityLink.UserId = task.Assignee;
		  identityLink.Task = task;
		  identityLink.Type = IdentityLinkType.ASSIGNEE;
		  identityLinks.Add(identityLink);
		}
		if (!string.ReferenceEquals(task.Owner, null))
		{
		  IdentityLinkEntity identityLink = new IdentityLinkEntity();
		  identityLink.UserId = task.Owner;
		  identityLink.Task = task;
		  identityLink.Type = IdentityLinkType.OWNER;
		  identityLinks.Add(identityLink);
		}

		return (System.Collections.IList) task.IdentityLinks;
	  }

	  protected internal virtual void checkGetIdentityLink(TaskEntity task, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadTask(task);
		}
	  }
	}

}