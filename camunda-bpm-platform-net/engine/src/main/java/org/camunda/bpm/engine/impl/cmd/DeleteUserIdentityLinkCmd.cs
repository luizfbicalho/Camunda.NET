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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	[Serializable]
	public class DeleteUserIdentityLinkCmd : DeleteIdentityLinkCmd
	{

	  private const long serialVersionUID = 1L;

	  public DeleteUserIdentityLinkCmd(string taskId, string userId, string type) : base(taskId, userId, null, type)
	  {
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		base.execute(commandContext);

		PropertyChange propertyChange = new PropertyChange(type, null, userId);

		commandContext.OperationLogManager.logLinkOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_USER_LINK, task, propertyChange);

		return null;
	  }
	}

}