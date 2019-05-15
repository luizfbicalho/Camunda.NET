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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class DeleteIdentityLinkForProcessDefinitionCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processDefinitionId;

	  protected internal string userId;

	  protected internal string groupId;

	  public DeleteIdentityLinkForProcessDefinitionCmd(string processDefinitionId, string userId, string groupId)
	  {
		validateParams(userId, groupId, processDefinitionId);
		this.processDefinitionId = processDefinitionId;
		this.userId = userId;
		this.groupId = groupId;
	  }

	  protected internal virtual void validateParams(string userId, string groupId, string processDefinitionId)
	  {
		ensureNotNull("processDefinitionId", processDefinitionId);

		if (string.ReferenceEquals(userId, null) && string.ReferenceEquals(groupId, null))
		{
		  throw new ProcessEngineException("userId and groupId cannot both be null");
		}
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);

		ensureNotNull("Cannot find process definition with id " + processDefinitionId, "processDefinition", processDefinition);
		processDefinition.deleteIdentityLink(userId, groupId);

		return null;
	  }

	}

}